using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200031D RID: 797
public class RescueHook : ItemComponent
{
	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06001545 RID: 5445 RVA: 0x0006B427 File Offset: 0x00069627
	public Character playerHoldingItem
	{
		get
		{
			return this.item.holderCharacter;
		}
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x0006B434 File Offset: 0x00069634
	public override void Awake()
	{
		this.actionReduceUses = base.GetComponent<Action_ReduceUses>();
		this.camera = Camera.main;
		base.Awake();
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Combine(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
		this.line.positionCount = 40;
		this.startingClawLocalPos = this.claw.transform.localPosition;
		this.StopDisplayRope();
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x0006B4B0 File Offset: 0x000696B0
	public void Update()
	{
		Vector3 vector;
		RaycastHit hit = this.GetHit(out vector);
		this.item.overrideUsability = Optionable<bool>.Some(hit.transform);
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x0006B4E4 File Offset: 0x000696E4
	public void LateUpdate()
	{
		if (this.isPulling)
		{
			if (this.targetPlayer)
			{
				this.claw.transform.position = this.targetPlayer.Center;
			}
			else
			{
				this.claw.transform.position = this.targetPos;
			}
			this.claw.transform.forward = (this.claw.transform.position - base.transform.position).normalized;
			this.ropeRender.DisplayRope(this.dragPoint.position, this.clawButt.transform.position, this.sinceFire, this.line);
		}
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x0006B5A8 File Offset: 0x000697A8
	private void FixedUpdate()
	{
		this.sinceFire += Time.fixedDeltaTime;
		if (this.isPulling)
		{
			if (this.targetPlayer)
			{
				this.targetPos = this.targetRig.position;
			}
			if (this.sinceFire > 0.25f)
			{
				if (this.targetPlayer)
				{
					this.targetPlayer.refs.movement.ApplyExtraDrag(this.extraDragOther, true);
					this.targetPlayer.data.sinceGrounded = Mathf.Clamp(this.targetPlayer.data.sinceGrounded, 0f, 1f);
					if (this.photonView.IsMine)
					{
						Vector3 normalized = (base.transform.position - this.targetPlayer.Center).normalized;
						Vector3 b = Mathf.Clamp((base.transform.position - this.targetPlayer.Center).y, 0f, this.maxLiftDistance) * Vector3.up * this.liftDragForce;
						Vector3 a = this.dragForce * normalized;
						a *= this.pulLStrengthCurve.Evaluate(Vector3.Distance(base.transform.position, this.targetPlayer.Center));
						this.targetPlayer.AddForceToBodyPart(this.targetRig, (a + b) * 0.2f, a + b);
					}
					if (!this.threwAchievement && this.photonView.IsMine)
					{
						float num = Vector3.Distance(this.rescuedCharacterStartingPos, this.targetPlayer.Center) * CharacterStats.unitsToMeters;
						if (!this.targetPlayer.data.fullyConscious && num > 30f)
						{
							this.threwAchievement = true;
							Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.DisasterResponseBadge);
						}
					}
					if ((Vector3.Distance(this.dragPoint.position, this.targetRig.position) < this.stopPullFriendDistance || this.sinceFire > this.maxScoutHookTime) && this.photonView.IsMine)
					{
						this.photonView.RPC("RPCA_LetGo", RpcTarget.All, Array.Empty<object>());
					}
				}
				else
				{
					this.playerHoldingItem.refs.movement.ApplyExtraDrag(this.extraDragSelf, true);
					if (!this.hitNothing)
					{
						float num2 = 1f;
						num2 *= this.pulLStrengthCurve.Evaluate(Vector3.Distance(this.targetPos, this.playerHoldingItem.Center));
						if (this.startingPos.y < this.targetPos.y && this.playerHoldingItem.Center.y > this.targetPos.y)
						{
							Vector3 a2 = (this.targetPos - this.playerHoldingItem.Center).normalized * this.launchForce * num2;
							Vector3 b2 = Vector3.up * this.liftForce;
							a2.y = 0f;
							this.playerHoldingItem.AddForce(a2 + b2, 1f, 1f);
							this.fly = false;
						}
						else
						{
							Vector3 a3 = (this.targetPos - this.playerHoldingItem.Center).normalized * this.launchForce * num2;
							Vector3 b3 = Vector3.up * this.liftForce;
							this.fly = true;
							this.playerHoldingItem.AddForce(a3 + b3, 1f, 1f);
							this.playerHoldingItem.data.sinceGrounded = 0f;
						}
					}
					Vector3 center = this.playerHoldingItem.Center;
					if (this.playerHoldingItem.Center.y > this.targetPos.y)
					{
						center.y = this.targetPos.y;
					}
					if (Vector3.Distance(this.playerHoldingItem.Center, this.targetPos) < this.stopPullDistance || this.sinceFire > this.maxWallHookTime)
					{
						this.RPCA_LetGo();
					}
				}
			}
			this.currentDistance = Vector3.Distance(this.dragPoint.position, this.targetPos);
			return;
		}
		this.fly = false;
		this.StopDisplayRope();
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x0006BA14 File Offset: 0x00069C14
	private void OnDestroy()
	{
		if (this.playerHoldingItem && this.isPulling)
		{
			this.playerHoldingItem.data.sinceGrounded = 0f;
		}
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Remove(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x0600154B RID: 5451 RVA: 0x0006BA72 File Offset: 0x00069C72
	private void StopDisplayRope()
	{
		this.line.enabled = false;
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x0006BA80 File Offset: 0x00069C80
	private void Fire()
	{
		Debug.Log("Fire");
		this.sinceFire = 0f;
		for (int i = 0; i < this.rescueShot.Length; i++)
		{
			this.rescueShot[i].Play(base.transform.position);
		}
		Vector3 vector;
		RaycastHit hit = this.GetHit(out vector);
		if (!hit.transform)
		{
			this.photonView.RPC("RPCA_RescueWall", RpcTarget.All, new object[]
			{
				true,
				vector
			});
			return;
		}
		Debug.Log(string.Format("Hit: {0} Rig: {1}, !hit.rigidbody: {2}", hit.collider.name, hit.rigidbody, !hit.rigidbody), hit.collider.gameObject);
		Character character;
		if (CharacterRagdoll.TryGetCharacterFromCollider(hit.collider, out character))
		{
			this.photonView.RPC("RPCA_RescueCharacter", RpcTarget.All, new object[]
			{
				character.photonView
			});
			return;
		}
		this.photonView.RPC("RPCA_RescueWall", RpcTarget.All, new object[]
		{
			false,
			hit.point
		});
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x0006BBB4 File Offset: 0x00069DB4
	private RaycastHit GetHit(out Vector3 endFire)
	{
		Ray middleScreenRay = PExt.GetMiddleScreenRay();
		this.curRange = this.range;
		if (Vector3.Dot(middleScreenRay.direction, Vector3.up) < 0f)
		{
			this.curRange = this.rangeDownward;
		}
		List<RaycastHit> list = Physics.RaycastAll(middleScreenRay, this.curRange, HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysicalExceptDefault)).ToList<RaycastHit>();
		endFire = middleScreenRay.origin + middleScreenRay.direction * this.curRange;
		RaycastHit result = default(RaycastHit);
		foreach (RaycastHit raycastHit in list)
		{
			if (raycastHit.rigidbody != null)
			{
				Item component = raycastHit.rigidbody.GetComponent<Item>();
				Character x;
				if ((component != null && component.holderCharacter == this.item.holderCharacter) || (CharacterRagdoll.TryGetCharacterFromCollider(raycastHit.collider, out x) && x == this.item.holderCharacter))
				{
					continue;
				}
			}
			if (result.distance == 0f || raycastHit.distance < result.distance)
			{
				result = raycastHit;
			}
		}
		if (Vector3.Distance(middleScreenRay.origin, result.point) < this.minRaycastDistance)
		{
			result = default(RaycastHit);
		}
		return result;
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x0006BD28 File Offset: 0x00069F28
	[PunRPC]
	public void RPCA_RescueItem(PhotonView objectView)
	{
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x0006BD2C File Offset: 0x00069F2C
	[PunRPC]
	public void RPCA_RescueCharacter(PhotonView characterView)
	{
		Character component = characterView.GetComponent<Character>();
		this.targetPlayer = component;
		if (this.photonView.IsMine && this.targetPlayer.data.currentClimbHandle != null)
		{
			this.targetPlayer.refs.climbing.CancelHandle(false);
		}
		this.targetRig = component.GetBodypart(BodypartType.Torso).Rig;
		this.sinceFire = 0f;
		if (this.targetPlayer.photonView.IsMine || this.photonView.IsMine)
		{
			GamefeelHandler.instance.perlin.AddShake(base.transform.position, 5f, 0.2f, 15f, 40f);
		}
		this.isPulling = true;
		this.rescuedCharacterStartingPos = this.targetPlayer.Center;
		this.targetPlayer.Fall(2f, 0f);
		for (int i = 0; i < this.rescueHit.Length; i++)
		{
			this.rescueHit[i].Play(this.targetRig.transform.position);
		}
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x0006BE4C File Offset: 0x0006A04C
	[PunRPC]
	private void RPCA_LetGo()
	{
		Character holderCharacter = this.targetPlayer;
		float extraDrag = this.extraDragOther;
		if (holderCharacter == null)
		{
			holderCharacter = this.item.holderCharacter;
			extraDrag = this.extraDragSelf;
		}
		holderCharacter.refs.movement.ApplyExtraDrag(extraDrag, true);
		this.playerHoldingItem.data.sinceGrounded = 0f;
		this.targetRig = null;
		this.targetPlayer = null;
		this.isPulling = false;
		this.afterLetGoDragTime = this.afterLetGoDragSeconds;
		this.claw.transform.localPosition = this.startingClawLocalPos;
		this.claw.transform.localRotation = Quaternion.identity;
		if (this.playerHoldingItem.IsLocal)
		{
			this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
			OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
			if (data.HasData && data.Value == 0)
			{
				this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			}
		}
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x0006BF48 File Offset: 0x0006A148
	[PunRPC]
	public void RPCA_RescueWall(bool hitNothing, Vector3 targetPos)
	{
		Debug.Log(string.Format("RPCA_RescueWall, hitnothing:{0}", hitNothing));
		this.targetPos = targetPos;
		this.startingPos = this.item.holderCharacter.Center;
		this.hitNothing = hitNothing;
		this.sinceFire = 0f;
		GamefeelHandler.instance.perlin.AddShake(base.transform.position, 5f, 0.2f, 15f, 40f);
		this.isPulling = true;
		this.playerHoldingItem.Fall(this.selfFallSeconds, 0f);
		if (this.photonView.IsMine && this.item.holderCharacter.data.currentClimbHandle != null)
		{
			this.item.holderCharacter.refs.climbing.CancelHandle(false);
		}
		for (int i = 0; i < this.rescueHit.Length; i++)
		{
			this.rescueHit[i].Play(targetPos);
		}
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x0006C04C File Offset: 0x0006A24C
	private void OnPrimaryFinishedCast()
	{
		this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		if (data.HasData && data.Value == 0)
		{
			this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			return;
		}
		this.Fire();
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x0006C0A2 File Offset: 0x0006A2A2
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x04001356 RID: 4950
	public Transform dragPoint;

	// Token: 0x04001357 RID: 4951
	public GameObject claw;

	// Token: 0x04001358 RID: 4952
	public Transform clawButt;

	// Token: 0x04001359 RID: 4953
	public float maxLength = 40f;

	// Token: 0x0400135A RID: 4954
	public float dragForce = 100f;

	// Token: 0x0400135B RID: 4955
	public float liftDragForce = 100f;

	// Token: 0x0400135C RID: 4956
	public float extraDragSelf = 0.95f;

	// Token: 0x0400135D RID: 4957
	public float extraDragOther = 0.95f;

	// Token: 0x0400135E RID: 4958
	public float launchForce = 10f;

	// Token: 0x0400135F RID: 4959
	public float liftForce = 1000f;

	// Token: 0x04001360 RID: 4960
	private Action_ReduceUses actionReduceUses;

	// Token: 0x04001361 RID: 4961
	private Camera camera;

	// Token: 0x04001362 RID: 4962
	private float currentDistance;

	// Token: 0x04001363 RID: 4963
	private bool fly;

	// Token: 0x04001364 RID: 4964
	private bool hitNothing;

	// Token: 0x04001365 RID: 4965
	private bool isPulling;

	// Token: 0x04001366 RID: 4966
	private float sinceFire;

	// Token: 0x04001367 RID: 4967
	private Character targetPlayer;

	// Token: 0x04001368 RID: 4968
	private Vector3 targetPos;

	// Token: 0x04001369 RID: 4969
	private Vector3 startingPos;

	// Token: 0x0400136A RID: 4970
	private Vector3 rescuedCharacterStartingPos;

	// Token: 0x0400136B RID: 4971
	private bool threwAchievement;

	// Token: 0x0400136C RID: 4972
	private Rigidbody targetRig;

	// Token: 0x0400136D RID: 4973
	public RopeRender ropeRender;

	// Token: 0x0400136E RID: 4974
	public LineRenderer line;

	// Token: 0x0400136F RID: 4975
	private Vector3 startingClawLocalPos;

	// Token: 0x04001370 RID: 4976
	public float maxWallHookTime = 1f;

	// Token: 0x04001371 RID: 4977
	public float maxScoutHookTime = 2f;

	// Token: 0x04001372 RID: 4978
	public float maxLiftDistance = 10f;

	// Token: 0x04001373 RID: 4979
	public float stopPullDistance = 5f;

	// Token: 0x04001374 RID: 4980
	public float stopPullFriendDistance = 5f;

	// Token: 0x04001375 RID: 4981
	public float minRaycastDistance = 5f;

	// Token: 0x04001376 RID: 4982
	public SFX_Instance[] rescueShot;

	// Token: 0x04001377 RID: 4983
	public SFX_Instance[] rescueHit;

	// Token: 0x04001378 RID: 4984
	public AnimationCurve pulLStrengthCurve;

	// Token: 0x04001379 RID: 4985
	public Transform firePoint;

	// Token: 0x0400137A RID: 4986
	public float range = 30f;

	// Token: 0x0400137B RID: 4987
	public float rangeDownward = 40f;

	// Token: 0x0400137C RID: 4988
	public float curRange;

	// Token: 0x0400137D RID: 4989
	public float afterLetGoDragSeconds = 1f;

	// Token: 0x0400137E RID: 4990
	private float afterLetGoDragTime;

	// Token: 0x0400137F RID: 4991
	public float selfFallSeconds = 0.5f;
}
