using System;
using Peak.Afflictions;
using Peak.Math;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000143 RID: 323
public class Mob : MonoBehaviourPunCallbacks
{
	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000A93 RID: 2707 RVA: 0x0003847E File Offset: 0x0003667E
	// (set) Token: 0x06000A94 RID: 2708 RVA: 0x00038486 File Offset: 0x00036686
	public Character forcedCharacterTarget { get; private set; }

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000A95 RID: 2709 RVA: 0x0003848F File Offset: 0x0003668F
	private bool InTargetingCooldown
	{
		get
		{
			return Time.time < Mathf.Max(this._timeLastCheckedForTarget + this.targetCheckCooldown, this._timeLastSwitchedTarget + this.targetSwitchCooldown);
		}
	}

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000A96 RID: 2710 RVA: 0x000384B7 File Offset: 0x000366B7
	private bool IsResting
	{
		get
		{
			return Time.time < this._timeLastAttacked + this.postAttackRest;
		}
	}

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06000A97 RID: 2711 RVA: 0x000384CD File Offset: 0x000366CD
	private bool IsInAttackCooldown
	{
		get
		{
			return Time.time < this._timeLastAttacked + this.attackCooldown;
		}
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x000384E3 File Offset: 0x000366E3
	public void SetForcedTarget(Character character)
	{
		this.forcedCharacterTarget = character;
	}

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06000A99 RID: 2713 RVA: 0x000384EC File Offset: 0x000366EC
	// (set) Token: 0x06000A9A RID: 2714 RVA: 0x000384F4 File Offset: 0x000366F4
	internal Mob.MobState mobState
	{
		get
		{
			return this._mobState;
		}
		set
		{
			if (this._mobState != value)
			{
				this._mobState = value;
				if (value == Mob.MobState.Flipping)
				{
					this.lastStartedFlipping = Time.time;
				}
				if (base.photonView.IsMine)
				{
					base.photonView.RPC("RPC_SyncMobState", RpcTarget.Others, new object[]
					{
						(int)value
					});
				}
			}
		}
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x0003854D File Offset: 0x0003674D
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("RPC_SyncMobState", newPlayer, new object[]
			{
				(int)this.mobState
			});
		}
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x00038581 File Offset: 0x00036781
	[PunRPC]
	protected void RPC_SyncMobState(int newMobState)
	{
		this.mobState = (Mob.MobState)newMobState;
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x0003858A File Offset: 0x0003678A
	private void Awake()
	{
		this.rig = base.GetComponent<Rigidbody>();
		this._mobItem = base.GetComponent<MobItem>();
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x000385A4 File Offset: 0x000367A4
	private void Start()
	{
		this._timeSpawned = Time.time;
		this._timeLastAttacked = this._timeSpawned + this.attackCooldown + this.postAttackRest;
		this.juicedViewForward = base.transform.forward;
		this.juicedViewUp = base.transform.up;
		this.normal = base.transform.up;
		this.ResetPatrolCounter();
		this.GetNewPatrolPos();
		this.startPos = base.transform.position;
		this.lastPos = base.transform.position;
		this.startUp = base.transform.up;
		MobManager.instance.Register(this);
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x00038653 File Offset: 0x00036853
	public void OnDestroy()
	{
		if (MobManager.instance != null)
		{
			MobManager.instance.Unregister(this);
		}
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x00038670 File Offset: 0x00036870
	private void GetNewPatrolPos()
	{
		Vector3 onUnitSphere = Random.onUnitSphere;
		Vector3 vector = this.startPos + this.startUp * 1f;
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + onUnitSphere * 15f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			this.patrolPos = raycastHit.point;
			return;
		}
		this.untilNextPatrolPos = 0f;
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x000386E3 File Offset: 0x000368E3
	private void ResetPatrolCounter()
	{
		this.untilNextPatrolPos = Random.Range(0.5f, 5f);
	}

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x06000AA2 RID: 2722 RVA: 0x000386FA File Offset: 0x000368FA
	private bool dead
	{
		get
		{
			return this.mobState == Mob.MobState.Dead;
		}
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x00038708 File Offset: 0x00036908
	private void Update()
	{
		if (this.sleeping)
		{
			return;
		}
		this.UpdateAnimation();
		if (this.dead)
		{
			this.HandleDeath();
			return;
		}
		if (Time.time - this._timeSpawned < 0.2f)
		{
			return;
		}
		this.GetTargetPos();
		this.CalcVars();
		if (base.photonView.IsMine)
		{
			if (this.forceNoMovement)
			{
				this.Attacking();
				this.Targeting();
				return;
			}
			if (this.mobState == Mob.MobState.Walking)
			{
				this.Attacking();
				this.Targeting();
				this.Patrol();
			}
		}
		else if (this.forceNoMovement || this.mobState == Mob.MobState.Walking)
		{
			this.Attacking();
		}
		if (!this.forceNoMovement)
		{
			this.JiggleViewTowardsTransform();
		}
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x000387B8 File Offset: 0x000369B8
	public void UpdateAnimation()
	{
		this.lerpedWalkSpeed = Mathf.Lerp(this.lerpedWalkSpeed, Vector3.Distance(base.transform.position, this.lastPos) / Time.fixedDeltaTime, 5f * Time.deltaTime);
		this.anim.SetFloat(Mob.WALKSPEED, (this.mobState == Mob.MobState.Walking) ? this.lerpedWalkSpeed : 0f);
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x00038824 File Offset: 0x00036A24
	public void UpdateSleeping()
	{
		this.visuals.gameObject.SetActive(!this.sleeping);
		if (this.anim.enabled && this.sleeping)
		{
			this.anim.enabled = false;
			return;
		}
		if (!this.anim.enabled && !this.sleeping)
		{
			this.anim.enabled = true;
		}
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00038890 File Offset: 0x00036A90
	private void FixedUpdate()
	{
		if (this.sleeping)
		{
			return;
		}
		if (this.mobState == Mob.MobState.Dead)
		{
			this.rig.constraints = RigidbodyConstraints.None;
			return;
		}
		if (Time.time - this._timeSpawned < 0.2f)
		{
			return;
		}
		this.DoGroundRaycast();
		this.lastPos = base.transform.position;
		if (this.forceNoMovement)
		{
			this.rig.constraints = RigidbodyConstraints.None;
			return;
		}
		if (!base.photonView.IsMine)
		{
			this.rig.constraints = RigidbodyConstraints.None;
			return;
		}
		if (this.mobState == Mob.MobState.RigidbodyControlled)
		{
			this.rig.constraints = RigidbodyConstraints.None;
			this.fallTick = 0f;
			this.TestStartFlippingMyself();
			return;
		}
		this.rig.angularVelocity = Vector3.zero;
		this.rig.linearVelocity = Vector3.zero;
		this.rig.constraints = RigidbodyConstraints.FreezeRotation;
		if (this.mobState == Mob.MobState.Walking)
		{
			this.Movement();
			this.GroundSnapping();
			this.SetRotationWalking();
			this.TestFalling();
		}
		if (this.mobState == Mob.MobState.Flipping)
		{
			this.SetRotationFlipping();
			if (this.hitGround && Time.time - this.lastStartedFlipping > 1f)
			{
				this.mobState = Mob.MobState.Walking;
			}
		}
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x000389BC File Offset: 0x00036BBC
	private void LateUpdate()
	{
		if (Time.time - this._timeSpawned < 0.2f)
		{
			return;
		}
		if (this.mobState == Mob.MobState.Walking)
		{
			this.VisualSnapping();
		}
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x000389E1 File Offset: 0x00036BE1
	private void HandleDeath()
	{
		this.anim.SetBool("Cooked", true);
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x000389F4 File Offset: 0x00036BF4
	private void TestFalling()
	{
		if (!this.hitGround)
		{
			this.fallTick += Time.fixedDeltaTime;
			if (this.fallTick > 0.5f)
			{
				this.lastFell = Time.time;
				this.mobState = Mob.MobState.RigidbodyControlled;
				return;
			}
		}
		else
		{
			this.fallTick = 0f;
		}
	}

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x06000AAA RID: 2730 RVA: 0x00038A46 File Offset: 0x00036C46
	[SerializeField]
	private float currentAttackCooldown
	{
		get
		{
			return Time.time - this._timeLastAttacked;
		}
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x00038A54 File Offset: 0x00036C54
	private void Attacking()
	{
		if (this.targetChar)
		{
			if (!this.attacking)
			{
				if (this.distanceToTarget < this.attackStartDistance && !this.IsInAttackCooldown && base.photonView.IsMine)
				{
					base.photonView.RPC("RPC_StartAttack", RpcTarget.All, Array.Empty<object>());
				}
				this.inRangeForAttackTime = 0f;
			}
			if (this.attacking)
			{
				if (this.inRangeForAttackTime == 0f)
				{
					this.anim.SetTrigger("Attack");
				}
				this.inRangeForAttackTime += Time.deltaTime;
				if (this.inRangeForAttackTime > this.attackTime)
				{
					Debug.Log("angle " + Vector3.Angle((base.transform.forward + Vector3.up).normalized, (this.targetChar.Center - base.transform.position).normalized).ToString());
					if (this.distanceToTarget < this.attackDistance && Vector3.Angle((base.transform.forward + Vector3.up).normalized, (this.targetChar.Center - base.transform.position).normalized) < this.attackAngle)
					{
						this.InflictAttack(this.targetChar);
						this._timeLastAttacked = Time.time;
					}
					else
					{
						this._timeLastAttacked = Time.time - this.whiffRefund * Mathf.Max(this.attackCooldown, this.postAttackRest);
					}
					this.attacking = false;
					return;
				}
			}
		}
		else
		{
			this.inRangeForAttackTime = 0f;
		}
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x00038C0F File Offset: 0x00036E0F
	[PunRPC]
	protected void RPC_StartAttack()
	{
		this.attacking = true;
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x06000AAD RID: 2733 RVA: 0x00038C18 File Offset: 0x00036E18
	// (set) Token: 0x06000AAE RID: 2734 RVA: 0x00038C20 File Offset: 0x00036E20
	private Character targetChar
	{
		get
		{
			return this._targetChar;
		}
		set
		{
			if (value == this._targetChar)
			{
				return;
			}
			if (value != null)
			{
				this._timeLastSwitchedTarget = Time.time;
			}
			this._targetChar = value;
			if (base.photonView.IsMine)
			{
				int num = (value == null) ? -1 : value.photonView.ViewID;
				base.photonView.RPC("RPC_SyncTargetCharacter", RpcTarget.Others, new object[]
				{
					num
				});
			}
			this.GetTargetPos();
		}
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00038CA0 File Offset: 0x00036EA0
	[PunRPC]
	protected void RPC_SyncTargetCharacter(int viewID)
	{
		if (viewID == -1)
		{
			this.targetChar = null;
			return;
		}
		PhotonView photonView = PhotonView.Find(viewID);
		Character targetChar;
		if (photonView && photonView.TryGetComponent<Character>(out targetChar))
		{
			this.targetChar = targetChar;
		}
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x00038CD9 File Offset: 0x00036ED9
	protected virtual void InflictAttack(Character character)
	{
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x00038CDC File Offset: 0x00036EDC
	private void Targeting()
	{
		if (this.InTargetingCooldown)
		{
			return;
		}
		if (this.attacking && this.targetChar != null)
		{
			return;
		}
		this._timeLastCheckedForTarget = Time.time;
		Character targetChar = null;
		float num = this.aggroDistance;
		foreach (Character character in Character.AllCharacters)
		{
			if (character.data.fullyConscious)
			{
				float num2 = Vector3.Distance(base.transform.position, character.Center);
				Affliction affliction;
				if (num2 < num && (!(this.forcedCharacterTarget != null) || !(character != this.forcedCharacterTarget)) && !character.refs.afflictions.HasAfflictionType(Affliction.AfflictionType.PoisonOverTime, out affliction) && !HelperFunctions.LineCheck(this.Center(), character.Center, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform)
				{
					num = num2;
					targetChar = character;
				}
			}
		}
		this.targetChar = targetChar;
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x00038DF0 File Offset: 0x00036FF0
	private void JiggleViewTowardsTransform()
	{
		this.viewForwardSpringVelocity = FRILerp.Lerp(this.viewForwardSpringVelocity, (base.transform.forward - this.juicedViewForward) * this.spring, this.drag, true);
		this.viewUpSpringVelocity = FRILerp.Lerp(this.viewUpSpringVelocity, (base.transform.up - this.juicedViewUp) * this.spring, this.drag, true);
		this.juicedViewForward += this.viewForwardSpringVelocity * Time.deltaTime;
		this.juicedViewUp += this.viewUpSpringVelocity * Time.deltaTime;
		if (this.juicedViewForward.NearZero())
		{
			this.juicedViewForward = base.transform.forward;
		}
		if (this.juicedViewUp.NearZero())
		{
			this.juicedViewUp = base.transform.up;
		}
		Quaternion rotation = Quaternion.LookRotation(this.juicedViewForward.normalized, this.juicedViewUp.normalized);
		this.mesh.rotation = rotation;
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x00038F18 File Offset: 0x00037118
	private void Patrol()
	{
		this.untilNextPatrolPos -= Time.deltaTime * Mathf.Lerp(30f, 1f, this.overShootFactor);
		if (this.untilNextPatrolPos <= 0f)
		{
			this.ResetPatrolCounter();
			this.GetNewPatrolPos();
		}
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x00038F68 File Offset: 0x00037168
	private void CalcVars()
	{
		this.distanceToTarget = Vector3.Distance(base.transform.position, this.targetPos);
		if (this.distanceToTarget < this.closestDistance)
		{
			this.closestDistance = this.distanceToTarget;
		}
		float value = Mathf.Clamp01(this.distanceToTarget - this.closestDistance);
		this.overShootFactor = Mathf.InverseLerp(0.02f, 0f, value);
		this.closeToTargetFactor = Mathf.InverseLerp(1.2f, 1.5f, this.distanceToTarget);
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x00038FF0 File Offset: 0x000371F0
	private void SetRotationWalking()
	{
		if (this.IsResting)
		{
			return;
		}
		Vector3 vector = this.targetPos - base.transform.position;
		vector = Vector3.ProjectOnPlane(vector, this.normal);
		Quaternion to = (vector.sqrMagnitude > 0.01f) ? Quaternion.LookRotation(vector.normalized, this.normal) : base.transform.rotation;
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, this.turnRate * Time.deltaTime * this.overShootFactor);
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x00039088 File Offset: 0x00037288
	private void SetRotationFlipping()
	{
		Vector3 up = Vector3.up;
		if (this.groundCheckHitDown.collider)
		{
			up = this.groundCheckHitDown.normal;
		}
		base.transform.up = Vector3.Slerp(base.transform.up, up, 8f * Time.fixedDeltaTime);
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x000390E0 File Offset: 0x000372E0
	private void TestStartFlippingMyself()
	{
		if (Time.time - this.lastFell > 0.5f && this.rig.linearVelocity.magnitude < this.maxVelocityToStartFlipping)
		{
			this.startFlippingTick += Time.fixedDeltaTime;
			if (this.startFlippingTick > this.minTimeToStartFlipping)
			{
				this.mobState = Mob.MobState.Flipping;
				return;
			}
		}
		else
		{
			this.startFlippingTick = 0f;
		}
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00039150 File Offset: 0x00037350
	private Vector3 GetTargetPos()
	{
		Vector3 center = this.patrolPos;
		if (this.targetChar)
		{
			center = this.targetChar.Center;
		}
		if (!this.targetPos.Same(center, 0.1f))
		{
			this.targetPos = center;
			this.closestDistance = float.PositiveInfinity;
		}
		return this.targetPos;
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x000391A8 File Offset: 0x000373A8
	private bool DoGroundRaycast()
	{
		this.hitGround = false;
		this.groundCheckHitWalking = default(RaycastHit);
		this.groundCheckHitDown = default(RaycastHit);
		this.groundCheckHitWalking = HelperFunctions.LineCheck(this.lastPos, base.transform.position, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		this.groundCheckHitDown = HelperFunctions.LineCheck(base.transform.position, base.transform.position + Vector3.down * 2f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (!this.groundCheckHitWalking.transform)
		{
			this.groundCheckHitWalking = HelperFunctions.LineCheck(this.Center(), this.Under(), HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		}
		if (this.groundCheckHitWalking.transform)
		{
			Vector3 from = this.groundCheckHitWalking.normal;
			if (Vector3.Angle(from, Vector3.up) < this.maxStandingAngle)
			{
				this.normal = from;
				this.hitGround = true;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x000392A5 File Offset: 0x000374A5
	private void GroundSnapping()
	{
		if (this.groundCheckHitWalking.transform)
		{
			this.rig.MovePosition(this.groundCheckHitWalking.point);
		}
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x000392D0 File Offset: 0x000374D0
	private void VisualSnapping()
	{
		if (this.groundCheckHitWalking.transform)
		{
			this.visuals.transform.position = this.groundCheckHitWalking.point;
			return;
		}
		this.visuals.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00039320 File Offset: 0x00037520
	private Vector3 Under()
	{
		return base.transform.position - this.normal * 1f;
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x00039342 File Offset: 0x00037542
	private Vector3 Center()
	{
		return base.transform.position + this.normal * 0.2f;
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x00039364 File Offset: 0x00037564
	private void Movement()
	{
		if (this.IsResting)
		{
			return;
		}
		base.transform.position += base.transform.forward * this.movementSpeed * Time.deltaTime * this.overShootFactor * this.closeToTargetFactor;
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x000393C8 File Offset: 0x000375C8
	public bool IsNearCharacter()
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (character != null && Vector3.Distance(this.Center(), character.Center) < this.sleepDistance)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x0003943C File Offset: 0x0003763C
	public void TestSleepMode()
	{
		if (this.sleeping)
		{
			if (this.IsNearCharacter())
			{
				this.sleeping = false;
				this.UpdateSleeping();
				return;
			}
		}
		else if (this.mobState == Mob.MobState.Walking && (!this._mobItem || this._mobItem.itemState == ItemState.Ground) && this.rig.linearVelocity.magnitude < 1f && !this.IsNearCharacter())
		{
			this.sleeping = true;
			this.UpdateSleeping();
		}
	}

	// Token: 0x040009D3 RID: 2515
	private MobItem _mobItem;

	// Token: 0x040009D4 RID: 2516
	public float movementSpeed = 5f;

	// Token: 0x040009D5 RID: 2517
	public float turnRate = 300f;

	// Token: 0x040009D6 RID: 2518
	public float aggroDistance = 5f;

	// Token: 0x040009D7 RID: 2519
	[SerializeField]
	[Range(0f, 10f)]
	private float targetSwitchCooldown = 5f;

	// Token: 0x040009D8 RID: 2520
	[SerializeField]
	[Range(0f, 5f)]
	private float targetCheckCooldown = 2f;

	// Token: 0x040009D9 RID: 2521
	public float closeToTargetFactor = 1f;

	// Token: 0x040009DA RID: 2522
	public float overShootFactor = 1f;

	// Token: 0x040009DB RID: 2523
	[SerializeField]
	public float distanceToTarget;

	// Token: 0x040009DC RID: 2524
	public float closestDistance;

	// Token: 0x040009DD RID: 2525
	public float attackStartDistance = 2.5f;

	// Token: 0x040009DE RID: 2526
	public float attackDistance = 1.5f;

	// Token: 0x040009DF RID: 2527
	[SerializeField]
	private float attackCooldown = 2f;

	// Token: 0x040009E0 RID: 2528
	[Range(0f, 10f)]
	[SerializeField]
	private float postAttackRest;

	// Token: 0x040009E1 RID: 2529
	[Range(0f, 1f)]
	[SerializeField]
	private float whiffRefund = 0.5f;

	// Token: 0x040009E2 RID: 2530
	public float maxVelocityToStartFlipping = 5f;

	// Token: 0x040009E3 RID: 2531
	public float minTimeToStartFlipping = 1f;

	// Token: 0x040009E4 RID: 2532
	public float maxStandingAngle = 70f;

	// Token: 0x040009E5 RID: 2533
	private Vector3 juicedViewForward;

	// Token: 0x040009E6 RID: 2534
	private Vector3 viewForwardSpringVelocity;

	// Token: 0x040009E7 RID: 2535
	private Vector3 juicedViewUp;

	// Token: 0x040009E8 RID: 2536
	private Vector3 viewUpSpringVelocity;

	// Token: 0x040009E9 RID: 2537
	public float spring = 35f;

	// Token: 0x040009EA RID: 2538
	public float drag = 15f;

	// Token: 0x040009EB RID: 2539
	public float attackTime = 1f;

	// Token: 0x040009EC RID: 2540
	public float attackAngle = 180f;

	// Token: 0x040009ED RID: 2541
	public Transform mesh;

	// Token: 0x040009EE RID: 2542
	public Animator anim;

	// Token: 0x040009EF RID: 2543
	public Transform visuals;

	// Token: 0x040009F0 RID: 2544
	public float sleepDistance = 50f;

	// Token: 0x040009F1 RID: 2545
	public bool sleeping;

	// Token: 0x040009F2 RID: 2546
	[SerializeField]
	private Mob.MobState _mobState;

	// Token: 0x040009F4 RID: 2548
	private Rigidbody rig;

	// Token: 0x040009F5 RID: 2549
	[SerializeField]
	private bool attacking;

	// Token: 0x040009F6 RID: 2550
	private float _timeLastSwitchedTarget;

	// Token: 0x040009F7 RID: 2551
	private float _timeLastCheckedForTarget;

	// Token: 0x040009F8 RID: 2552
	private float _timeLastAttacked;

	// Token: 0x040009F9 RID: 2553
	private float _timeSpawned;

	// Token: 0x040009FA RID: 2554
	private Vector3 targetPos;

	// Token: 0x040009FB RID: 2555
	private float untilNextPatrolPos;

	// Token: 0x040009FC RID: 2556
	private Vector3 patrolPos;

	// Token: 0x040009FD RID: 2557
	private float lastStartedFlipping;

	// Token: 0x040009FE RID: 2558
	private static readonly int WALKSPEED = Animator.StringToHash("WalkSpeed");

	// Token: 0x040009FF RID: 2559
	private Vector3 startPos;

	// Token: 0x04000A00 RID: 2560
	private Vector3 startUp;

	// Token: 0x04000A01 RID: 2561
	private float lerpedWalkSpeed;

	// Token: 0x04000A02 RID: 2562
	private float fallTick;

	// Token: 0x04000A03 RID: 2563
	[SerializeField]
	internal bool forceNoMovement;

	// Token: 0x04000A04 RID: 2564
	[SerializeField]
	private float inRangeForAttackTime;

	// Token: 0x04000A05 RID: 2565
	[SerializeField]
	private Character _targetChar;

	// Token: 0x04000A06 RID: 2566
	private float startFlippingTick;

	// Token: 0x04000A07 RID: 2567
	private Vector3 normal;

	// Token: 0x04000A08 RID: 2568
	private bool hitGround = true;

	// Token: 0x04000A09 RID: 2569
	private float lastFell;

	// Token: 0x04000A0A RID: 2570
	private RaycastHit groundCheckHitWalking;

	// Token: 0x04000A0B RID: 2571
	private RaycastHit groundCheckHitDown;

	// Token: 0x04000A0C RID: 2572
	private Vector3 lastPos;

	// Token: 0x0200048B RID: 1163
	internal enum MobState
	{
		// Token: 0x040019FA RID: 6650
		RigidbodyControlled,
		// Token: 0x040019FB RID: 6651
		Walking,
		// Token: 0x040019FC RID: 6652
		Flipping,
		// Token: 0x040019FD RID: 6653
		Dead
	}
}
