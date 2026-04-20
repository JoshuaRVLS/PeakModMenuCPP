using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000349 RID: 841
public class Spider : MonoBehaviour
{
	// Token: 0x06001651 RID: 5713 RVA: 0x000715D5 File Offset: 0x0006F7D5
	public void Awake()
	{
		this.line.positionCount = 40;
		this.startingSpiderLocalPos = this.spider.transform.localPosition;
		this.StopDisplayRope();
	}

	// Token: 0x06001652 RID: 5714 RVA: 0x00071600 File Offset: 0x0006F800
	public void Start()
	{
		SpiderManager.instance.Register(this);
		float y = this.startPoint.eulerAngles.y;
		this.startPoint.rotation = Quaternion.identity;
		this.startPoint.SetYEuler(y);
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x00071645 File Offset: 0x0006F845
	public void OnDestroy()
	{
		if (SpiderManager.instance != null)
		{
			SpiderManager.instance.Unregister(this);
		}
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x00071660 File Offset: 0x0006F860
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		if (this.lastScannedTime + 0.01f > Time.time)
		{
			Gizmos.DrawSphere(base.transform.position - Vector3.up * this.minRaycastDistance, this.castRadius);
			Gizmos.DrawSphere(base.transform.position - Vector3.up * this.castDistance, this.castRadius);
			return;
		}
		Gizmos.DrawWireSphere(base.transform.position - Vector3.up * this.minRaycastDistance, this.castRadius);
		Gizmos.DrawWireSphere(base.transform.position - Vector3.up * this.castDistance, this.castRadius);
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x00071738 File Offset: 0x0006F938
	public void LateUpdate()
	{
		if (this.spiderState == Spider.SpiderState.Grabbing && this.targetPlayer && !this.targetPlayer.data.dead)
		{
			this.spider.transform.position = this.targetPlayer.Head + Vector3.up;
			Vector3 normalized = (base.transform.position - this.spider.transform.position).normalized;
			Vector3 vector = this.targetPlayer.refs.head.transform.forward;
			vector = Vector3.ProjectOnPlane(vector, normalized).normalized;
			this.spider.transform.rotation = Quaternion.Lerp(this.spider.transform.rotation, Quaternion.LookRotation(vector, normalized), Time.deltaTime * 5f);
		}
		if (this._stunnedTime > 0f)
		{
			this._stunnedTime -= Time.deltaTime;
		}
		this.ropeRender.DisplayRope(this.startPoint.position, this.spiderButt.transform.position, Mathf.Clamp01(this.sinceGrab), this.line);
		this.TestAchievement();
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x00071884 File Offset: 0x0006FA84
	private void UpdateAttack(bool isLocal)
	{
		this.sincePoison += Time.fixedDeltaTime;
		if (this.sincePoison >= this.poisonFrequency - 0.25f && !this.attacked)
		{
			this.anim.SetTrigger("Attack");
			this.attacked = true;
		}
		if (this.sincePoison > this.poisonFrequency)
		{
			this.sincePoison = 0f;
			this.attacked = false;
			if (isLocal)
			{
				this.targetPlayer.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, this.poisonDamage, false, true, true);
			}
		}
	}

	// Token: 0x06001657 RID: 5719 RVA: 0x0007191C File Offset: 0x0006FB1C
	private void FixedUpdate()
	{
		if (this.spiderState == Spider.SpiderState.Grabbing)
		{
			this.sinceGrab += Time.fixedDeltaTime;
			this.anim.SetBool("Weaving", this.sinceGrab < this.pullWaitTime);
			if (this.targetPlayer)
			{
				if (this.targetPlayer.data.isCarried)
				{
					this.photonView.RPC("RPCA_LetGo", RpcTarget.All, Array.Empty<object>());
					return;
				}
				this.targetPos = this.targetRig.position;
				if (this.targetPlayer.IsLocal && this.sinceGrab > this.pullWaitTime && !this.targetPlayer.data.dead)
				{
					this.UpdateAttack(true);
				}
				if (!this.targetPlayer.IsLocal)
				{
					this.UpdateAttack(false);
				}
				if (this.targetPlayer.data.dead)
				{
					this.photonView.RPC("RPCA_LetGo", RpcTarget.All, Array.Empty<object>());
					return;
				}
			}
			if (this.targetPlayer && this.targetPlayer.photonView.IsMine)
			{
				if (this.sinceGrab > this.pullWaitTime)
				{
					Vector3 normalized = (this.dragToPoint.position - (this.targetPlayer.Head + Vector3.up)).normalized;
					float d = this.distancePullStrengthCurve.Evaluate(Vector3.Distance(this.dragToPoint.position, this.targetPlayer.Head + Vector3.up));
					this.targetPlayer.refs.movement.ApplyExtraDrag(this.extraDrag, true);
					this.targetPlayer.AddForceToBodyPart(this.targetRig, normalized * (this.dragForce * 0.2f) * d, normalized * this.dragForce * d);
					this.targetPlayer.data.sinceGrounded = Mathf.Clamp(this.targetPlayer.data.sinceGrounded, 0f, 1f);
				}
				else
				{
					Vector3 normalized2 = (this.dragToPoint.position - (this.targetPlayer.Head + Vector3.up)).normalized;
					this.targetPlayer.refs.movement.ApplyExtraDrag(this.extraDrag, true);
					this.targetPlayer.AddForceToBodyPart(this.targetRig, normalized2 * (this.keepUprightForce * 0.2f), normalized2 * this.keepUprightForce);
					this.targetPlayer.data.sinceGrounded = Mathf.Clamp(this.targetPlayer.data.sinceGrounded, 0f, 1f);
				}
				if (this.targetPlayer.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Web) < 0.025f)
				{
					this.photonView.RPC("RPCA_LetGo", RpcTarget.All, Array.Empty<object>());
				}
			}
			this.currentDistance = Vector3.Distance(this.startPoint.position, this.targetPos);
			return;
		}
		this.anim.SetBool("Weaving", false);
	}

	// Token: 0x06001658 RID: 5720 RVA: 0x00071C4A File Offset: 0x0006FE4A
	private void StopDisplayRope()
	{
	}

	// Token: 0x06001659 RID: 5721 RVA: 0x00071C4C File Offset: 0x0006FE4C
	public void Scan()
	{
		this.UpdateCulled();
		if (this.spiderState == Spider.SpiderState.Idle && !this.isCulled && this._stunnedTime <= 0f)
		{
			RaycastHit hit = this.GetHit();
			if (hit.transform)
			{
				if (!this.setShadow)
				{
					this.shadow.transform.position = new Vector3(this.shadow.transform.position.x, hit.point.y, this.shadow.transform.position.z);
					this.setShadow = true;
				}
				Character character;
				if (CharacterRagdoll.TryGetCharacterFromCollider(hit.collider, out character))
				{
					this.photonView.RPC("RPCA_DropSpider", RpcTarget.All, new object[]
					{
						character.photonView,
						Vector3.Distance(this.startPoint.position, hit.point)
					});
				}
			}
			this.lastScannedTime = Time.time;
		}
	}

	// Token: 0x0600165A RID: 5722 RVA: 0x00071D54 File Offset: 0x0006FF54
	private void UpdateCulled()
	{
		Vector2 b = new Vector2(this.spiderTriggerTransform.position.x, this.spiderTriggerTransform.position.z);
		bool culled = true;
		foreach (Character character in Character.AllCharacters)
		{
			if (!(character == null) && Vector2.Distance(new Vector2(character.Center.x, character.Center.z), b) < this.cullDistanceXZ)
			{
				culled = false;
			}
		}
		this.SetCulled(culled);
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x00071E04 File Offset: 0x00070004
	private void SetCulled(bool culled)
	{
		this.isCulled = culled;
		base.gameObject.SetActive(!culled);
	}

	// Token: 0x0600165C RID: 5724 RVA: 0x00071E1C File Offset: 0x0007001C
	private RaycastHit GetHit()
	{
		RaycastHit result = default(RaycastHit);
		if (Physics.SphereCast(this.startPoint.position, this.castRadius, -Vector3.up, out result, this.castDistance, HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysicalExceptDefault)) && Vector3.Distance(this.startPoint.position, result.point) < this.minRaycastDistance)
		{
			result = default(RaycastHit);
		}
		return result;
	}

	// Token: 0x0600165D RID: 5725 RVA: 0x00071E90 File Offset: 0x00070090
	[PunRPC]
	public void RPCA_DropSpider(PhotonView characterView, float distance)
	{
		Spider.<>c__DisplayClass61_0 CS$<>8__locals1 = new Spider.<>c__DisplayClass61_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.characterView = characterView;
		CS$<>8__locals1.distance = distance;
		base.StartCoroutine(CS$<>8__locals1.<RPCA_DropSpider>g__DropRoutine|0());
	}

	// Token: 0x0600165E RID: 5726 RVA: 0x00071EC5 File Offset: 0x000700C5
	public void GrabCharacter(PhotonView characterView)
	{
		if (this.spiderState == Spider.SpiderState.Dropped && this._stunnedTime <= 0f)
		{
			this.photonView.RPC("RPCA_GrabCharacter", RpcTarget.All, new object[]
			{
				characterView
			});
		}
	}

	// Token: 0x0600165F RID: 5727 RVA: 0x00071EF8 File Offset: 0x000700F8
	public void Bonk()
	{
		this.photonView.RPC("BonkRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06001660 RID: 5728 RVA: 0x00071F10 File Offset: 0x00070110
	[PunRPC]
	private void BonkRPC()
	{
		this.anim.SetTrigger("Bonk");
		if (this.targetPlayer)
		{
			this.targetPlayer.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Web, 1f, true, false);
		}
		this.stunnedParticle.Stop();
		this.stunnedParticle.Play();
		this._stunnedTime = this.bonkStunTime;
	}

	// Token: 0x06001661 RID: 5729 RVA: 0x00071F7C File Offset: 0x0007017C
	[PunRPC]
	public void RPCA_GrabCharacter(PhotonView characterView)
	{
		Character component = characterView.GetComponent<Character>();
		this.targetPlayer = component;
		this.targetRig = component.GetBodypart(BodypartType.Torso).Rig;
		this.sinceGrab = 0f;
		if (this.targetPlayer.photonView.IsMine || this.photonView.IsMine)
		{
			GamefeelHandler.instance.perlin.AddShake(base.transform.position, 5f, 0.2f, 15f, 40f);
		}
		this.spiderState = Spider.SpiderState.Grabbing;
		component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Web, this.webDamage, true, true, true);
		this.targetPlayer.Fall(this.fallTime, 0f);
		if (this.targetPlayer.IsLocal)
		{
			this.caughtLocalPlayer = true;
		}
		this.targetPlayer.BreakCharacterCarrying(false);
	}

	// Token: 0x06001662 RID: 5730 RVA: 0x0007205C File Offset: 0x0007025C
	[PunRPC]
	private void RPCA_LetGo()
	{
		if (this.targetPlayer != null)
		{
			this.targetPlayer.data.sinceGrounded = 0f;
			this.targetPlayer.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Web, 1f, true, false);
		}
		this.targetRig = null;
		this.targetPlayer = null;
		this.spiderState = Spider.SpiderState.LetGo;
		this.spider.transform.DORotate(new Vector3(0f, 0f, 0f), 3f, RotateMode.Fast).SetEase(Ease.OutBack).OnUpdate(delegate
		{
		});
	}

	// Token: 0x06001663 RID: 5731 RVA: 0x00072110 File Offset: 0x00070310
	public bool WillAttach(out RaycastHit hit)
	{
		hit = default(RaycastHit);
		return Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, out hit, this.maxLength, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), QueryTriggerInteraction.UseGlobal);
	}

	// Token: 0x06001664 RID: 5732 RVA: 0x00072160 File Offset: 0x00070360
	private void TestAchievement()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		if (this.caughtLocalPlayer)
		{
			if (Character.localCharacter.data.dead)
			{
				this.caughtLocalPlayer = false;
				return;
			}
			if (Character.localCharacter.data.isGrounded && Character.localCharacter.refs.afflictions.statusSum < 1f && Vector3.Distance(this.spiderTriggerTransform.position, Character.localCharacter.Center) > this.escapeRadiusForAchievement && Character.localCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Web) < 0.025f)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.WebSecurityBadge);
				this.caughtLocalPlayer = false;
			}
		}
	}

	// Token: 0x04001470 RID: 5232
	public PhotonView photonView;

	// Token: 0x04001471 RID: 5233
	public Transform startPoint;

	// Token: 0x04001472 RID: 5234
	public Transform dragToPoint;

	// Token: 0x04001473 RID: 5235
	public GameObject spider;

	// Token: 0x04001474 RID: 5236
	public Transform spiderButt;

	// Token: 0x04001475 RID: 5237
	public Animator anim;

	// Token: 0x04001476 RID: 5238
	public Transform shadow;

	// Token: 0x04001477 RID: 5239
	public Renderer spiderRenderer;

	// Token: 0x04001478 RID: 5240
	private bool setShadow;

	// Token: 0x04001479 RID: 5241
	public bool isCulled;

	// Token: 0x0400147A RID: 5242
	public float spiderMoveSpeed;

	// Token: 0x0400147B RID: 5243
	public float spiderWaitTime;

	// Token: 0x0400147C RID: 5244
	public float maxLength = 20f;

	// Token: 0x0400147D RID: 5245
	public float castRadius;

	// Token: 0x0400147E RID: 5246
	public float castDistance = 100f;

	// Token: 0x0400147F RID: 5247
	public float dragForce = 100f;

	// Token: 0x04001480 RID: 5248
	public float keepUprightForce = 50f;

	// Token: 0x04001481 RID: 5249
	private bool caughtLocalPlayer;

	// Token: 0x04001482 RID: 5250
	public float escapeRadiusForAchievement;

	// Token: 0x04001483 RID: 5251
	private float currentDistance;

	// Token: 0x04001484 RID: 5252
	public Spider.SpiderState spiderState;

	// Token: 0x04001485 RID: 5253
	public SFX_Instance[] webMovement;

	// Token: 0x04001486 RID: 5254
	public Transform spiderTriggerTransform;

	// Token: 0x04001487 RID: 5255
	private float sinceGrab;

	// Token: 0x04001488 RID: 5256
	private float sincePoison;

	// Token: 0x04001489 RID: 5257
	private Character targetPlayer;

	// Token: 0x0400148A RID: 5258
	private Vector3 targetPos;

	// Token: 0x0400148B RID: 5259
	private Rigidbody targetRig;

	// Token: 0x0400148C RID: 5260
	public RopeRender ropeRender;

	// Token: 0x0400148D RID: 5261
	public LineRenderer line;

	// Token: 0x0400148E RID: 5262
	public ParticleSystem stunnedParticle;

	// Token: 0x0400148F RID: 5263
	private Vector3 startingSpiderLocalPos;

	// Token: 0x04001490 RID: 5264
	public float maxScoutHookTime = 2f;

	// Token: 0x04001491 RID: 5265
	public float minRaycastDistance = 5f;

	// Token: 0x04001492 RID: 5266
	public float bonkStunTime = 5f;

	// Token: 0x04001493 RID: 5267
	public Transform firePoint;

	// Token: 0x04001494 RID: 5268
	public float poisonDamage = 0.05f;

	// Token: 0x04001495 RID: 5269
	public float poisonFrequency = 1f;

	// Token: 0x04001496 RID: 5270
	public float webDamage = 0.25f;

	// Token: 0x04001497 RID: 5271
	public float extraDrag = 0.5f;

	// Token: 0x04001498 RID: 5272
	public float pullWaitTime = 1f;

	// Token: 0x04001499 RID: 5273
	public float fallTime = 2f;

	// Token: 0x0400149A RID: 5274
	public float cullDistanceXZ = 30f;

	// Token: 0x0400149B RID: 5275
	private float _stunnedTime;

	// Token: 0x0400149C RID: 5276
	public AnimationCurve distancePullStrengthCurve;

	// Token: 0x0400149D RID: 5277
	public AnimationCurve letGoCurve;

	// Token: 0x0400149E RID: 5278
	private bool attacked;

	// Token: 0x0400149F RID: 5279
	private float lastScannedTime;

	// Token: 0x0200053C RID: 1340
	public enum SpiderState
	{
		// Token: 0x04001CB2 RID: 7346
		Idle,
		// Token: 0x04001CB3 RID: 7347
		Dropped,
		// Token: 0x04001CB4 RID: 7348
		Grabbing,
		// Token: 0x04001CB5 RID: 7349
		LetGo
	}
}
