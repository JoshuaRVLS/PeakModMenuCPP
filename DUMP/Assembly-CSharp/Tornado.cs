using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000362 RID: 866
public class Tornado : MonoBehaviour
{
	// Token: 0x060016E0 RID: 5856 RVA: 0x00075698 File Offset: 0x00073898
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		base.transform.localScale = Vector3.zero;
		this.lifeTime = Random.Range(this.tornadoLifetimeMin, this.tornadoLifetimeMax);
		this.tornadoPos = base.transform.position;
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x000756EC File Offset: 0x000738EC
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(base.transform.position, this.range);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.captureDistance);
	}

	// Token: 0x060016E2 RID: 5858 RVA: 0x0007573C File Offset: 0x0007393C
	private void Update()
	{
		if (this.view.IsMine)
		{
			this.syncCounter += Time.deltaTime;
			if (this.syncCounter > 0.5f)
			{
				this.syncCounter = 0f;
				this.view.RPC("RPCA_SyncTornado", RpcTarget.All, new object[]
				{
					this.vel
				});
			}
		}
		if (!this.dying)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.deltaTime * 0.25f);
			this.lifeTime -= Time.deltaTime;
			if (this.lifeTime < 0f && this.view.IsMine)
			{
				this.view.RPC("RPCA_TornadoDie", RpcTarget.All, Array.Empty<object>());
			}
		}
		else
		{
			base.transform.localScale = Vector3.MoveTowards(base.transform.localScale, Vector3.zero, Time.deltaTime * 0.2f);
			this.tornadoSFX.SetBool("Die", true);
			if (base.transform.localScale.x < 0.01f && this.view.IsMine)
			{
				PhotonNetwork.Destroy(base.gameObject);
			}
		}
		if (this.view.IsMine)
		{
			this.TargetSelection();
		}
		this.Movement();
	}

	// Token: 0x060016E3 RID: 5859 RVA: 0x000758A6 File Offset: 0x00073AA6
	[PunRPC]
	private void RPCA_SyncTornado(Vector3 syncVel)
	{
		this.vel = syncVel;
	}

	// Token: 0x060016E4 RID: 5860 RVA: 0x000758AF File Offset: 0x00073AAF
	[PunRPC]
	private void RPCA_TornadoDie()
	{
		this.dying = true;
	}

	// Token: 0x060016E5 RID: 5861 RVA: 0x000758B8 File Offset: 0x00073AB8
	private void TargetSelection()
	{
		this.selectNewTargetInSeconds -= Time.deltaTime;
		if (!this.target || this.selectNewTargetInSeconds < 0f || (this.target && HelperFunctions.FlatDistance(base.transform.position, this.target.position) < 10f))
		{
			this.selectNewTargetInSeconds = Random.Range(5f, 30f);
			this.PickTarget();
		}
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x0007593B File Offset: 0x00073B3B
	private void PickTarget()
	{
		this.view.RPC("RPCA_SelectTargetPos", RpcTarget.All, new object[]
		{
			Random.Range(0, this.targetParent.childCount)
		});
	}

	// Token: 0x060016E7 RID: 5863 RVA: 0x0007596D File Offset: 0x00073B6D
	[PunRPC]
	public void RPCA_SelectTargetPos(int targetID)
	{
		this.target = this.targetParent.GetChild(targetID);
	}

	// Token: 0x060016E8 RID: 5864 RVA: 0x00075984 File Offset: 0x00073B84
	private void Movement()
	{
		if (this.target == null)
		{
			return;
		}
		this.vel = FRILerp.Lerp(this.vel, (this.target.position - this.tornadoPos).Flat().normalized * 15f, 0.15f, true);
		this.tornadoPos += this.vel * Time.deltaTime;
		RaycastHit groundPosRaycast = HelperFunctions.GetGroundPosRaycast(this.tornadoPos + Vector3.up * 200f, HelperFunctions.LayerType.Terrain, 0f);
		if (groundPosRaycast.transform && Vector3.Distance(this.tornadoPos, groundPosRaycast.point) < 100f)
		{
			base.transform.position = groundPosRaycast.point;
			return;
		}
		base.transform.position = this.tornadoPos;
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x00075A78 File Offset: 0x00073C78
	private void FixedUpdate()
	{
		if (base.transform.localScale.x < 0.1f)
		{
			if (this.caughtCharacters.Count > 0)
			{
				this.caughtCharacters.Clear();
			}
			if (this.ignoredCharacters.Count > 0)
			{
				this.ignoredCharacters.Clear();
			}
			return;
		}
		this.AttractCharacters();
		this.CapturedCharacter();
		this.Feedback();
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x00075AE4 File Offset: 0x00073CE4
	private void CapturedCharacter()
	{
		if (this.caughtCharacters.Count == 0)
		{
			return;
		}
		foreach (Character character in new List<Character>(this.caughtCharacters))
		{
			if (!this.ignoredCharacters.Contains(character) && !(character == null))
			{
				float d = 15f;
				Vector3 vector = (character.Center - base.transform.position).Flat().normalized * d;
				Vector3 vector2 = base.transform.position + vector;
				float y = HelperFunctions.GetGroundPos(vector2, HelperFunctions.LayerType.Terrain, 0f).y;
				if (y > vector2.y)
				{
					vector2.y = y;
				}
				Vector3 a = (vector2 - character.Center).Flat();
				Vector3 normalized = Vector3.Cross(Vector3.up, vector).normalized;
				character.AddForce(normalized * this.force, 1f, 1f);
				character.AddForce(a * this.force * 0.2f, 1f, 1f);
				character.AddForce(Vector3.up * (19f + Mathf.Abs(this.Height(character) * 1f)), 1f, 1f);
				character.ClampSinceGrounded(0.5f);
				if (character.IsLocal)
				{
					character.GetBodypartRig(BodypartType.Torso).AddTorque(Vector3.up * 200f, ForceMode.Acceleration);
					character.GetBodypartRig(BodypartType.Hip).AddTorque(Vector3.up * 200f, ForceMode.Acceleration);
					character.GetBodypartRig(BodypartType.Torso).AddTorque(vector.normalized * 100f, ForceMode.Acceleration);
					character.GetBodypartRig(BodypartType.Hip).AddTorque(vector.normalized * 100f, ForceMode.Acceleration);
				}
				else
				{
					character.GetBodypartRig(BodypartType.Torso).AddTorque(Vector3.up * 500f, ForceMode.Acceleration);
					character.GetBodypartRig(BodypartType.Hip).AddTorque(Vector3.up * 500f, ForceMode.Acceleration);
					character.GetBodypartRig(BodypartType.Torso).AddTorque(vector.normalized * 500f, ForceMode.Acceleration);
					character.GetBodypartRig(BodypartType.Hip).AddTorque(vector.normalized * 500f, ForceMode.Acceleration);
				}
				character.refs.movement.ApplyExtraDrag(0.95f, true);
				character.RPCA_Fall(0.5f);
				if (character.IsLocal && this.LetTargetGo(character, vector2))
				{
					this.view.RPC("RPCA_ThrowPlayer", RpcTarget.All, new object[]
					{
						character.refs.view.ViewID
					});
				}
			}
		}
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x00075DE0 File Offset: 0x00073FE0
	private bool LetTargetGo(Character target, Vector3 orbitSpot)
	{
		return (this.Height(target) > 50f && target.data.avarageVelocity.x > 0f != target.Center.x > 0f) || Vector3.Distance(orbitSpot, target.Center) > 30f || target.IsStuck();
	}

	// Token: 0x060016EC RID: 5868 RVA: 0x00075E48 File Offset: 0x00074048
	private float Height(Character target)
	{
		return target.Center.y - base.transform.position.y;
	}

	// Token: 0x060016ED RID: 5869 RVA: 0x00075E68 File Offset: 0x00074068
	[PunRPC]
	private void RPCA_ThrowPlayer(int targetView)
	{
		Tornado.<>c__DisplayClass32_0 CS$<>8__locals1 = new Tornado.<>c__DisplayClass32_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.target = PhotonView.Find(targetView).GetComponent<Character>();
		if (this.caughtCharacters.Contains(CS$<>8__locals1.target))
		{
			this.caughtCharacters.Remove(CS$<>8__locals1.target);
		}
		base.StartCoroutine(CS$<>8__locals1.<RPCA_ThrowPlayer>g__IIgnoreChar|0());
	}

	// Token: 0x060016EE RID: 5870 RVA: 0x00075EC8 File Offset: 0x000740C8
	private void Feedback()
	{
		this.counter += Time.deltaTime;
		if (this.counter > 0.2f)
		{
			GamefeelHandler.instance.AddPerlinShakeProximity(base.transform.position, 3f, 0.4f, 15f, this.range * 1f);
			this.counter = 0f;
		}
	}

	// Token: 0x060016EF RID: 5871 RVA: 0x00075F30 File Offset: 0x00074130
	private void AttractCharacters()
	{
		float num = base.transform.localScale.x * this.range;
		float num2 = base.transform.localScale.x * this.captureDistance;
		foreach (Character character in Character.AllCharacters)
		{
			if (!this.ignoredCharacters.Contains(character) && !this.caughtCharacters.Contains(character))
			{
				float num3 = HelperFunctions.FlatDistance(base.transform.position, character.Center);
				if (num3 <= num && this.Height(character) >= -10f && this.Height(character) <= 50f)
				{
					float time = Mathf.Clamp01(1f - num3 / num);
					float d = this.inStrC.Evaluate(time);
					float d2 = this.upStrC.Evaluate(time);
					Vector3 normalized = (base.transform.position - character.Center).Flat().normalized;
					float d3 = 1f;
					if (character.data.isCrouching)
					{
						d3 = 0.25f;
					}
					character.AddForce(normalized * this.force * d * 1.2f * d3 + Vector3.up * this.force * d2 * d3, 0.8f, 1f);
					if (num3 < num2 && character.IsLocal && !character.IsStuck())
					{
						this.view.RPC("RPCA_CaptureCharacter", RpcTarget.All, new object[]
						{
							character.refs.view.ViewID
						});
					}
				}
			}
		}
	}

	// Token: 0x060016F0 RID: 5872 RVA: 0x00076130 File Offset: 0x00074330
	[PunRPC]
	private void RPCA_CaptureCharacter(int targetViewID)
	{
		Character component = PhotonView.Find(targetViewID).GetComponent<Character>();
		this.caughtCharacters.Add(component);
	}

	// Token: 0x060016F1 RID: 5873 RVA: 0x00076155 File Offset: 0x00074355
	[PunRPC]
	internal void RPCA_InitTornado(int targetViewID)
	{
		this.view = base.GetComponent<PhotonView>();
		this.targetParent = PhotonView.Find(targetViewID).transform.Find("TornadoPoints");
	}

	// Token: 0x0400154A RID: 5450
	public Animator tornadoSFX;

	// Token: 0x0400154B RID: 5451
	public float force;

	// Token: 0x0400154C RID: 5452
	public float range = 25f;

	// Token: 0x0400154D RID: 5453
	public float captureDistance = 10f;

	// Token: 0x0400154E RID: 5454
	public AnimationCurve inStrC;

	// Token: 0x0400154F RID: 5455
	public AnimationCurve upStrC;

	// Token: 0x04001550 RID: 5456
	private PhotonView view;

	// Token: 0x04001551 RID: 5457
	private float lifeTime;

	// Token: 0x04001552 RID: 5458
	private bool dying;

	// Token: 0x04001553 RID: 5459
	private Vector3 tornadoPos;

	// Token: 0x04001554 RID: 5460
	public float tornadoLifetimeMin = 30f;

	// Token: 0x04001555 RID: 5461
	public float tornadoLifetimeMax = 120f;

	// Token: 0x04001556 RID: 5462
	private Transform targetParent;

	// Token: 0x04001557 RID: 5463
	public Transform target;

	// Token: 0x04001558 RID: 5464
	private float syncCounter;

	// Token: 0x04001559 RID: 5465
	private float selectNewTargetInSeconds;

	// Token: 0x0400155A RID: 5466
	private Vector3 vel;

	// Token: 0x0400155B RID: 5467
	private List<Character> ignoredCharacters = new List<Character>();

	// Token: 0x0400155C RID: 5468
	private List<Character> caughtCharacters = new List<Character>();

	// Token: 0x0400155D RID: 5469
	private float counter;
}
