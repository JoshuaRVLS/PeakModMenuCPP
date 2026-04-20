using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;
using Zorro.Settings;

// Token: 0x0200002B RID: 43
public class MushroomZombie : MonoBehaviourPunCallbacks
{
	// Token: 0x17000044 RID: 68
	// (get) Token: 0x0600030C RID: 780 RVA: 0x00015574 File Offset: 0x00013774
	private bool targetForced
	{
		get
		{
			return Time.time < this.targetForcedUntil;
		}
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x0600030D RID: 781 RVA: 0x00015583 File Offset: 0x00013783
	public bool isMushroomMan
	{
		get
		{
			return this._setting.Value == OffOnMode.ON;
		}
	}

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x0600030E RID: 782 RVA: 0x00015593 File Offset: 0x00013793
	// (set) Token: 0x0600030F RID: 783 RVA: 0x0001559C File Offset: 0x0001379C
	public MushroomZombie.State currentState
	{
		get
		{
			return this._currentState;
		}
		set
		{
			if (this._currentState != value)
			{
				Debug.Log("Zombie state set to " + value.ToString());
				this._currentState = value;
				if (value == MushroomZombie.State.Dead)
				{
					this.timeDiedAt = Time.time;
				}
				if (base.photonView.IsMine)
				{
					this.PushState();
				}
			}
		}
	}

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x06000310 RID: 784 RVA: 0x000155F7 File Offset: 0x000137F7
	// (set) Token: 0x06000311 RID: 785 RVA: 0x000155FF File Offset: 0x000137FF
	public Character currentTarget
	{
		get
		{
			return this._currentTarget;
		}
		set
		{
			if (this.targetForced)
			{
				return;
			}
			this._currentTarget = value;
		}
	}

	// Token: 0x06000312 RID: 786 RVA: 0x00015614 File Offset: 0x00013814
	internal void SetCurrentTarget(Character setCurrentTarget, float forceForTime = 0f)
	{
		if (setCurrentTarget != this.currentTarget)
		{
			this.view.RPC("RPCA_SetCurrentTarget", RpcTarget.All, new object[]
			{
				(setCurrentTarget == null) ? -1 : setCurrentTarget.photonView.ViewID,
				forceForTime
			});
		}
	}

	// Token: 0x06000313 RID: 787 RVA: 0x0001566E File Offset: 0x0001386E
	[PunRPC]
	private void RPCA_SetCurrentTarget(int targetViewID, float forceForTime)
	{
		if (targetViewID == -1)
		{
			this.currentTarget = null;
		}
		else
		{
			this.currentTarget = PhotonNetwork.GetPhotonView(targetViewID).GetComponent<Character>();
		}
		if (forceForTime > 0f)
		{
			this.targetForcedUntil = Time.time + forceForTime;
		}
	}

	// Token: 0x06000314 RID: 788 RVA: 0x000156A4 File Offset: 0x000138A4
	private void OnDestroy()
	{
		GlobalEvents.OnCharacterFell = (Action<Character, float>)Delegate.Remove(GlobalEvents.OnCharacterFell, new Action<Character, float>(this.TestCharacterFell));
		GlobalEvents.OnCharacterPassedOut = (Action<Character>)Delegate.Remove(GlobalEvents.OnCharacterPassedOut, new Action<Character>(this.TestCharacterPassedOut));
		ZombieManager.Instance.DeRegisterZombie(this);
	}

	// Token: 0x06000315 RID: 789 RVA: 0x000156FC File Offset: 0x000138FC
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.view = base.GetComponent<PhotonView>();
		this.timeAwoke = Time.time;
		GlobalEvents.OnCharacterFell = (Action<Character, float>)Delegate.Combine(GlobalEvents.OnCharacterFell, new Action<Character, float>(this.TestCharacterFell));
		GlobalEvents.OnCharacterPassedOut = (Action<Character>)Delegate.Combine(GlobalEvents.OnCharacterPassedOut, new Action<Character>(this.TestCharacterPassedOut));
		if (this.isNPCZombie)
		{
			ZombieManager.Instance.RegisterZombie(this);
		}
		this._setting = GameHandler.Instance.SettingsHandler.GetSetting<ZombiePhobiaSetting>();
		GameUtils.instance.StartCoroutine(this.AwakeRoutine());
		this.InitMushroomVisuals();
	}

	// Token: 0x06000316 RID: 790 RVA: 0x000157AB File Offset: 0x000139AB
	private void Start()
	{
		if (this.isNPCZombie)
		{
			this.StartSleeping();
			base.StartCoroutine(this.RevealZombie());
		}
		this.character.isZombie = true;
		base.StartCoroutine(this.ZombieGrunts());
	}

	// Token: 0x06000317 RID: 791 RVA: 0x000157E1 File Offset: 0x000139E1
	private IEnumerator ZombieGrunts()
	{
		for (;;)
		{
			if (!base.photonView.IsMine)
			{
				yield return null;
			}
			else
			{
				if (this.currentState != MushroomZombie.State.Dead && this.currentState != MushroomZombie.State.Sleeping)
				{
					float seconds = Random.Range(this.zombieGruntWaitTimeMinMax.x, this.zombieGruntWaitTimeMinMax.y);
					yield return new WaitForSeconds(seconds);
					base.photonView.RPC("RPC_PlaySFX", RpcTarget.All, new object[]
					{
						0
					});
				}
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06000318 RID: 792 RVA: 0x000157F0 File Offset: 0x000139F0
	[PunRPC]
	private void RPC_PlaySFX(int index)
	{
		if (this.isMushroomMan)
		{
			return;
		}
		switch (index)
		{
		case 0:
			this.gruntSFX.PlayFromSource(this.character.Center, this.gruntSource);
			return;
		case 1:
			this.knockoutSFX.Play(this.character.Center);
			return;
		case 2:
		{
			SFX_Instance[] array = this.biteSFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play(this.character.Center);
			}
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000319 RID: 793 RVA: 0x00015878 File Offset: 0x00013A78
	private void InitMushroomVisuals()
	{
		this.mushroomGrowTimes.Clear();
		foreach (GameObject gameObject in this.mushroomVisuals)
		{
			this.mushroomGrowTimes.Add(Random.Range(this.minMaxMushroomGrowTime.x, this.minMaxMushroomGrowTime.y));
			gameObject.SetActive(true);
			gameObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		}
	}

	// Token: 0x0600031A RID: 794 RVA: 0x0001591C File Offset: 0x00013B1C
	private void ClearMushroomVisuals()
	{
		foreach (GameObject gameObject in this.mushroomVisuals)
		{
			gameObject.SetActive(false);
		}
	}

	// Token: 0x0600031B RID: 795 RVA: 0x00015970 File Offset: 0x00013B70
	[PunRPC]
	private void RPC_SetOutfit(bool hasSkirt)
	{
		this.wearingSkirt = hasSkirt;
		this.skirt.SetActive(hasSkirt);
		this.shorts.SetActive(!hasSkirt);
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00015994 File Offset: 0x00013B94
	private IEnumerator AwakeRoutine()
	{
		yield return null;
		if (this.character.refs.customization.overridePhotonPlayer != null)
		{
			base.gameObject.name = string.Format("Zombie [{0} : {1}]", this.character.refs.customization.overridePhotonPlayer.NickName, this.character.refs.customization.overridePhotonPlayer.ActorNumber);
		}
		else
		{
			base.gameObject.name = "Zombie (NPC)";
			bool flag = Util.Coinflip();
			base.photonView.RPC("RPC_SetOutfit", RpcTarget.All, new object[]
			{
				flag
			});
		}
		this.SetZombieEyes();
		yield break;
	}

	// Token: 0x0600031D RID: 797 RVA: 0x000159A4 File Offset: 0x00013BA4
	private void SetZombieEyes()
	{
		CustomizationRefs refs = this.character.refs.customization.refs;
		for (int i = 0; i < refs.EyeRenderers.Length; i++)
		{
			refs.EyeRenderers[i].material.SetTexture(CharacterCustomization.MainTex, this.zombieEyeTexture);
		}
	}

	// Token: 0x0600031E RID: 798 RVA: 0x000159F8 File Offset: 0x00013BF8
	[PunRPC]
	public void RPC_Arise(int sourceCharacterPhotonID)
	{
		PhotonView photonView = PhotonNetwork.GetPhotonView(sourceCharacterPhotonID);
		if (photonView == null)
		{
			return;
		}
		this.isNPCZombie = false;
		this.spawnedFromCharacter = photonView.GetComponent<Character>();
		foreach (Bodypart bodypart in base.transform.GetComponentsInChildren<Bodypart>())
		{
			Bodypart bodypart2 = this.spawnedFromCharacter.GetBodypart(bodypart.partType);
			if (!(bodypart2 == null))
			{
				bodypart.Rig.MovePosition(bodypart2.Rig.position);
				bodypart.Rig.MoveRotation(bodypart2.Rig.rotation);
			}
		}
		this.character.data.currentRagdollControll = 0f;
		Debug.Log("Spawned Zombie from " + photonView.Owner.NickName);
		base.gameObject.name = string.Format("Zombie [{0} : {1}]", photonView.Owner.NickName, photonView.Owner.ActorNumber);
		this.character.refs.customization.overridePhotonPlayer = this.spawnedFromCharacter.photonView.Owner;
		this.spawnedFromCharacter.FinishZombifying();
		this.currentState = MushroomZombie.State.WakingUp;
		this.SetMushroomMan();
	}

	// Token: 0x0600031F RID: 799 RVA: 0x00015B30 File Offset: 0x00013D30
	private void CalcVars()
	{
		this.sinceLookForTarget += Time.deltaTime;
		bool flag = this.currentTarget && this.CanSeeTarget(this.currentTarget);
		if (this.currentTarget)
		{
			if (!flag)
			{
				this.sinceSeenTarget += Time.deltaTime;
			}
			else
			{
				this.sinceSeenTarget = 0f;
			}
		}
		else
		{
			this.sinceSeenTarget = 0f;
		}
		if (this.currentTarget)
		{
			this.distanceToTarget = Vector3.Distance(this.character.Center, this.currentTarget.Center);
		}
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00015BD8 File Offset: 0x00013DD8
	private bool CanSeeTarget(Character currentTarget)
	{
		return !(currentTarget == null) && HelperFunctions.LineCheck(this.character.Head, currentTarget.Center + Random.insideUnitSphere * 0.5f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null;
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00015C2F File Offset: 0x00013E2F
	private void FixedUpdate()
	{
	}

	// Token: 0x06000322 RID: 802 RVA: 0x00015C34 File Offset: 0x00013E34
	private void Update()
	{
		if (this.mushroomsGrowing)
		{
			this.UpdateMushroomGrowth();
		}
		if (this.view.IsMine)
		{
			if (this.spawnedFromCharacter && !this.spawnedFromCharacter.data.dead)
			{
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
			this.ResetInput();
			this.CalcVars();
			this.ValidateTarget();
			switch (this.currentState)
			{
			case MushroomZombie.State.Sleeping:
				this.DoSleeping();
				break;
			case MushroomZombie.State.WakingUp:
				this.DoWakingUp();
				break;
			case MushroomZombie.State.Idle:
				this.DoIdle();
				break;
			case MushroomZombie.State.Chasing:
				this.DoChasing();
				break;
			case MushroomZombie.State.Lunging:
				this.DoLunging();
				break;
			case MushroomZombie.State.LungeRecovery:
				this.DoLungeRecovery();
				break;
			case MushroomZombie.State.Dead:
				this.character.data.passedOut = true;
				this.character.data.fallSeconds = 10f;
				break;
			}
		}
		else if (this.currentState == MushroomZombie.State.Dead || this.currentState == MushroomZombie.State.Sleeping)
		{
			this.character.data.passedOut = true;
		}
		else if (this.currentState == MushroomZombie.State.WakingUp)
		{
			this.DoWakingUp();
		}
		this.UpdateMouth();
		this.biteColliderObject.SetActive(this.currentState == MushroomZombie.State.Lunging);
	}

	// Token: 0x06000323 RID: 803 RVA: 0x00015D74 File Offset: 0x00013F74
	private void UpdateMushroomGrowth()
	{
		bool flag = false;
		for (int i = 0; i < this.mushroomVisuals.Count; i++)
		{
			GameObject gameObject = this.mushroomVisuals[i];
			if (gameObject.transform.localScale.x < 1f)
			{
				float num = this.mushroomGrowTimes[i];
				gameObject.transform.localScale = Vector3.MoveTowards(gameObject.transform.localScale, Vector3.one, 1f / num * Time.deltaTime);
				flag = true;
			}
		}
		if (!flag)
		{
			this.mushroomsGrowing = false;
		}
	}

	// Token: 0x06000324 RID: 804 RVA: 0x00015E03 File Offset: 0x00014003
	public void StartSleeping()
	{
		this.currentState = MushroomZombie.State.Sleeping;
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00015E0C File Offset: 0x0001400C
	public void HideAllRenderers()
	{
		this.character.refs.customization.HideAllRenderers();
		this.visible = false;
	}

	// Token: 0x06000326 RID: 806 RVA: 0x00015E2A File Offset: 0x0001402A
	public IEnumerator RevealZombie()
	{
		this.character.refs.customization.HideAllRenderers();
		yield return new WaitForSeconds(1f);
		if (this != null)
		{
			this.FadeInRenderers();
		}
		this.visible = true;
		yield break;
	}

	// Token: 0x06000327 RID: 807 RVA: 0x00015E3C File Offset: 0x0001403C
	private void DoSleeping()
	{
		this.character.data.fallSeconds = 10f;
		if (!this.visible)
		{
			return;
		}
		foreach (Character character in Character.AllCharacters)
		{
			if (this.TargetIsValid(character))
			{
				float num = Vector3.Distance(this.character.Center, character.Center);
				float num2 = Vector3.Angle(character.refs.head.transform.forward, this.character.Center - character.refs.head.transform.position);
				if (num < this.distanceBeforeWakeup && num2 <= this.lookAngleBeforeWakeup && this.HasLineOfSight(character))
				{
					this.WakeUpFromSleep();
					break;
				}
			}
		}
	}

	// Token: 0x06000328 RID: 808 RVA: 0x00015F2C File Offset: 0x0001412C
	private void WakeUpFromSleep()
	{
		this.character.data.fallSeconds = 0f;
		this.character.data.currentRagdollControll = 0f;
		this.character.refs.animations.ResetForceSpeed();
		this.character.refs.ragdoll.ToggleKinematic(false);
		this.currentState = MushroomZombie.State.WakingUp;
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00015F95 File Offset: 0x00014195
	private bool HasLineOfSight(Character otherCharacter)
	{
		return !Physics.Linecast(otherCharacter.Center, this.character.Center + Vector3.up, HelperFunctions.terrainMapMask);
	}

	// Token: 0x0600032A RID: 810 RVA: 0x00015FC4 File Offset: 0x000141C4
	private void FadeInRenderers()
	{
		this.character.refs.customization.ShowAllRenderers();
		this.SetMushroomMan();
		for (int i = 0; i < this.character.refs.customization.refs.AllRenderers.Length; i++)
		{
			for (int j = 0; j < this.character.refs.customization.refs.AllRenderers[i].materials.Length; j++)
			{
				this.character.refs.customization.refs.AllRenderers[i].materials[j].SetFloat("_Opacity", 0f);
				this.character.refs.customization.refs.AllRenderers[i].materials[j].DOFloat(1f, "_Opacity", 1.5f);
			}
		}
	}

	// Token: 0x0600032B RID: 811 RVA: 0x000160B4 File Offset: 0x000142B4
	private void SetMushroomMan()
	{
		if (this.isMushroomMan)
		{
			this.character.refs.customization.refs.SetMushroomMan(true);
			this.character.refs.animator.runtimeAnimatorController = this.animatorMushroomMan;
			this.ClearMushroomVisuals();
			this.character.refs.customization.refs.sashRenderer.enabled = false;
			for (int i = 0; i < this.character.refs.customization.refs.skeletonRenderer.materials.Length; i++)
			{
				this.character.refs.customization.refs.skeletonRenderer.materials[i].SetFloat("_Opacity", 0f);
				this.character.refs.customization.refs.skeletonRenderer.materials[i].DOFloat(1f, "_Opacity", 1.5f);
			}
		}
	}

	// Token: 0x0600032C RID: 812 RVA: 0x000161BD File Offset: 0x000143BD
	private void ValidateTarget()
	{
		if (this.currentTarget && !this.TargetIsValid(this.currentTarget))
		{
			this.currentTarget = null;
		}
	}

	// Token: 0x0600032D RID: 813 RVA: 0x000161E1 File Offset: 0x000143E1
	private void StartIdle()
	{
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("RPC_PlaySFX", RpcTarget.All, new object[]
			{
				0
			});
		}
		this.currentState = MushroomZombie.State.Idle;
	}

	// Token: 0x0600032E RID: 814 RVA: 0x00016218 File Offset: 0x00014418
	private void DoIdle()
	{
		this.idledFor += Time.deltaTime;
		if (this.idledFor > 2f)
		{
			this.TryLookForTarget();
			if (this.currentTarget)
			{
				this.LookAt(this.currentTarget.Head);
				if ((this.distanceToTarget >= this.distanceBeforeChase || this.timeSpentAwake > 2f) && this.HasLineOfSight(this.currentTarget))
				{
					this.StartChasing();
				}
			}
		}
	}

	// Token: 0x0600032F RID: 815 RVA: 0x00016298 File Offset: 0x00014498
	private void StartLunging()
	{
		if (this.currentTarget == null)
		{
			return;
		}
		this.lungeTargetForward = this.character.Center + (this.currentTarget.Head - this.character.Center) * 100f;
		this.character.input.jumpWasPressed = true;
		this.currentState = MushroomZombie.State.Lunging;
		this.timeSpentLunging = 0f;
	}

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x06000330 RID: 816 RVA: 0x00016312 File Offset: 0x00014512
	private float timeSpentAwake
	{
		get
		{
			return Time.time - this.timeAwoke;
		}
	}

	// Token: 0x06000331 RID: 817 RVA: 0x00016320 File Offset: 0x00014520
	private void DoLungeRecovery()
	{
		if (this.character.data.fallSeconds > 0f || this.character.data.passedOut || this.character.data.fullyPassedOut)
		{
			return;
		}
		this.timeSpentRecoveringFromLunge += Time.deltaTime;
		if (this.timeSpentRecoveringFromLunge >= this.lungeRecoveryTime)
		{
			this.timeSpentRecoveringFromLunge = 0f;
			this.StartChasing();
		}
	}

	// Token: 0x06000332 RID: 818 RVA: 0x0001639C File Offset: 0x0001459C
	private void DoLunging()
	{
		this.WalkTowards(this.lungeTargetForward, 1.2f, false, false, true);
		this.timeSpentLunging += Time.deltaTime;
		if (this.timeSpentLunging >= this.lungeTime)
		{
			this.timeSpentLunging = 0f;
			this.currentState = MushroomZombie.State.LungeRecovery;
			this.character.Fall(3f, 0f);
			this.PushState();
		}
	}

	// Token: 0x06000333 RID: 819 RVA: 0x0001640C File Offset: 0x0001460C
	private void DoWakingUp()
	{
		this.character.data.passedOut = true;
		this.character.data.currentRagdollControll = 0f;
		if (base.photonView.IsMine)
		{
			this.timeSpentWakingUp += Time.deltaTime;
			if (this.timeSpentWakingUp >= this.initialWakeUpTime)
			{
				this.timeSpentWakingUp = 0f;
				this.character.data.passedOut = false;
				this.StartIdle();
			}
		}
	}

	// Token: 0x06000334 RID: 820 RVA: 0x0001648E File Offset: 0x0001468E
	private void StartChasing()
	{
		this.timeSpentChasing = 0f;
		this.currentState = MushroomZombie.State.Chasing;
	}

	// Token: 0x06000335 RID: 821 RVA: 0x000164A2 File Offset: 0x000146A2
	private void DoChasing()
	{
		this.timeSpentChasing += Time.deltaTime;
		this.TryLookForTarget();
		if (this.currentTarget != null)
		{
			this.Chase();
			this.TryLunge();
			this.VerifyTarget();
		}
	}

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000336 RID: 822 RVA: 0x000164DC File Offset: 0x000146DC
	private bool readyToSprint
	{
		get
		{
			return this.timeSpentChasing > this.chaseTimeBeforeSprint;
		}
	}

	// Token: 0x06000337 RID: 823 RVA: 0x000164EC File Offset: 0x000146EC
	private void VerifyTarget()
	{
		if (this.currentTarget == null)
		{
			return;
		}
		if (!this.TargetIsValid(this.currentTarget))
		{
			this.SetCurrentTarget(null, 0f);
		}
	}

	// Token: 0x06000338 RID: 824 RVA: 0x00016518 File Offset: 0x00014718
	private void Chase()
	{
		if (this.character.data.isClimbing)
		{
			this.ClimbTowards(this.currentTarget.Head, 1f);
			if (this.currentTarget.Center.y < this.character.Center.y && !HelperFunctions.LineCheck(this.character.Center, this.currentTarget.Center, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform)
			{
				this.character.refs.climbing.StopClimbing();
				return;
			}
		}
		else
		{
			this.WalkTowards(this.currentTarget.Head, 1f, true, true, false);
		}
	}

	// Token: 0x06000339 RID: 825 RVA: 0x000165D0 File Offset: 0x000147D0
	private void TryLunge()
	{
		if (this.currentState == MushroomZombie.State.Chasing && this.readyToSprint && this.character.data.isGrounded && this.character.input.sprintIsPressed && this.distanceToTarget <= this.zombieLungeDistance && this.CanSeeTarget(this.currentTarget))
		{
			this.StartLunging();
		}
	}

	// Token: 0x0600033A RID: 826 RVA: 0x00016634 File Offset: 0x00014834
	private void ResetInput()
	{
		this.character.input.ResetInput();
	}

	// Token: 0x0600033B RID: 827 RVA: 0x00016648 File Offset: 0x00014848
	private void ClimbTowards(Vector3 targetPos, float mult)
	{
		this.LookAt(targetPos);
		float x = Mathf.Clamp(this.character.GetBodypart(BodypartType.Torso).transform.InverseTransformPoint(targetPos).x * 0.25f, -1f, 1f);
		this.character.input.movementInput = new Vector2(x, mult);
		this.character.data.currentStamina = 1f;
	}

	// Token: 0x0600033C RID: 828 RVA: 0x000166BA File Offset: 0x000148BA
	private void SetSprint(bool sprinting)
	{
		this.character.input.sprintIsPressed = sprinting;
		if (sprinting != this.character.data.isSprinting)
		{
			this.character.data.isSprinting = sprinting;
			this.PushState();
		}
	}

	// Token: 0x0600033D RID: 829 RVA: 0x000166F8 File Offset: 0x000148F8
	private void WalkTowards(Vector3 targetPos, float mult, bool tryClimb = true, bool tryJump = true, bool forceSprint = false)
	{
		float num = HelperFunctions.FlatDistance(this.character.Center, targetPos);
		if (Vector3.Distance(this.character.Center, targetPos) < 5f && num < 1.5f)
		{
			mult *= 0f;
		}
		this.character.input.movementInput = new Vector2(0f, mult);
		this.SetSprint(forceSprint || (this.readyToSprint && this.distanceToTarget < this.zombieSprintDistance && this.CanSeeTarget(this.currentTarget)));
		if (this.character.input.sprintIsPressed)
		{
			this.LookAt(targetPos);
		}
		else
		{
			this.LookAt(targetPos);
		}
		if (tryClimb)
		{
			this.character.refs.climbing.TryClimb(1.25f);
		}
		if (tryJump && HelperFunctions.LineCheck(this.character.Center, this.character.Center + Vector3.down * 3f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null)
		{
			this.SetSprint(false);
			this.character.input.jumpWasPressed = true;
		}
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0001682E File Offset: 0x00014A2E
	private void LookAt(Vector3 lookAtPos)
	{
		this.character.data.lookValues = HelperFunctions.DirectionToLook(lookAtPos - this.character.Head);
	}

	// Token: 0x0600033F RID: 831 RVA: 0x0001685C File Offset: 0x00014A5C
	private void TryLookForTarget()
	{
		if (this.sinceLookForTarget < 10f)
		{
			return;
		}
		Character closestCharacter = this.GetClosestCharacter(null);
		this.SetCurrentTarget(closestCharacter, 0f);
		this.sinceLookForTarget = 0f;
	}

	// Token: 0x06000340 RID: 832 RVA: 0x00016896 File Offset: 0x00014A96
	private bool TargetIsValid(Character target)
	{
		return !target.isBot && !target.data.dead && !target.data.fullyPassedOut;
	}

	// Token: 0x06000341 RID: 833 RVA: 0x000168C0 File Offset: 0x00014AC0
	private Character GetClosestCharacter(Character ignoredCharacter)
	{
		List<Character> allCharacters = Character.AllCharacters;
		Character character = null;
		float num = float.MaxValue;
		foreach (Character character2 in allCharacters)
		{
			if (!(character2 == ignoredCharacter) && this.TargetIsValid(character2))
			{
				float num2 = Vector3.Distance(character2.Center, this.character.Center);
				if (character == null || num2 < num)
				{
					character = character2;
					num = num2;
				}
			}
		}
		return character;
	}

	// Token: 0x06000342 RID: 834 RVA: 0x00016954 File Offset: 0x00014B54
	private void PushState()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		base.photonView.RPC("RPC_SyncState", RpcTarget.Others, new object[]
		{
			(int)this.currentState,
			this.character.data.isSprinting,
			this.character.data.fallSeconds,
			this.character.data.passedOut
		});
	}

	// Token: 0x06000343 RID: 835 RVA: 0x000169DC File Offset: 0x00014BDC
	[PunRPC]
	private void RPC_SyncState(int state, bool isSprinting, float fallSeconds, bool passedOut)
	{
		this.currentState = (MushroomZombie.State)state;
		this.character.data.isSprinting = isSprinting;
		this.character.data.fallSeconds = fallSeconds;
		this.character.data.passedOut = passedOut;
	}

	// Token: 0x06000344 RID: 836 RVA: 0x00016A1C File Offset: 0x00014C1C
	private void TestCharacterFell(Character c, float time)
	{
		if (this.view.IsMine && c == this.character && this.currentState != MushroomZombie.State.Dead)
		{
			if (this.currentState != MushroomZombie.State.Lunging && this.currentState != MushroomZombie.State.LungeRecovery)
			{
				base.photonView.RPC("RPC_PlaySFX", RpcTarget.All, new object[]
				{
					1
				});
			}
			if (this.timeSpentAwake > this.lifetime)
			{
				this.Die();
				return;
			}
			if (this.currentState != MushroomZombie.State.LungeRecovery)
			{
				this.timeSpentLunging = 0f;
				this.currentState = MushroomZombie.State.LungeRecovery;
				this.character.data.fallSeconds = 3f;
				this.PushState();
			}
		}
	}

	// Token: 0x06000345 RID: 837 RVA: 0x00016AD0 File Offset: 0x00014CD0
	private void Die()
	{
		this.currentState = MushroomZombie.State.Dead;
	}

	// Token: 0x06000346 RID: 838 RVA: 0x00016ADC File Offset: 0x00014CDC
	private void TestCharacterPassedOut(Character c)
	{
		if (this.view.IsMine && c == this.character && this.currentState != MushroomZombie.State.Dead)
		{
			if (this.currentState != MushroomZombie.State.Lunging)
			{
				base.photonView.RPC("RPC_PlaySFX", RpcTarget.All, new object[]
				{
					1
				});
			}
			if (this.timeSpentAwake > this.lifetime)
			{
				this.Die();
				return;
			}
			this.timeSpentLunging = 0f;
			this.currentState = MushroomZombie.State.LungeRecovery;
		}
	}

	// Token: 0x06000347 RID: 839 RVA: 0x00016B5D File Offset: 0x00014D5D
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		if (base.photonView.IsMine)
		{
			this.PushState();
			base.photonView.RPC("RPC_SetOutfit", newPlayer, new object[]
			{
				this.wearingSkirt
			});
		}
	}

	// Token: 0x06000348 RID: 840 RVA: 0x00016B98 File Offset: 0x00014D98
	private void UpdateMouth()
	{
		if (this.currentState == MushroomZombie.State.Lunging)
		{
			this.animatedMouth.mouthRenderer.material.SetInt("_UseTalkSprites", 1);
			Material material = this.animatedMouth.mouthRenderer.material;
			string name = "_TalkSprite";
			Texture2D[] mouthTextures = this.animatedMouth.mouthTextures;
			material.SetTexture(name, mouthTextures[mouthTextures.Length - 1]);
			return;
		}
		this.animatedMouth.mouthRenderer.material.SetInt("_UseTalkSprites", 0);
	}

	// Token: 0x06000349 RID: 841 RVA: 0x00016C10 File Offset: 0x00014E10
	public void OnBitCharacter(Character c)
	{
		this.character.Fall(8f, 0f);
		if (c.IsLocal)
		{
			if (this.currentState != MushroomZombie.State.Lunging)
			{
				base.photonView.RPC("RPC_PlaySFX", RpcTarget.All, new object[]
				{
					2
				});
			}
			Debug.Log("Bit by zombie");
			Singleton<AchievementManager>.Instance.SetRunBasedInt(RUNBASEDVALUETYPE.BitByZombie, 1);
		}
	}

	// Token: 0x0600034A RID: 842 RVA: 0x00016C7C File Offset: 0x00014E7C
	public void DestroyZombie()
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		if (this.currentState == MushroomZombie.State.Dead)
		{
			base.photonView.RPC("RPC_SpawnSkelly", RpcTarget.All, Array.Empty<object>());
			if (this.spawner)
			{
				Object.Destroy(this.spawner.gameObject);
			}
			PhotonNetwork.Destroy(base.gameObject);
			return;
		}
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x0600034B RID: 843 RVA: 0x00016CEA File Offset: 0x00014EEA
	[PunRPC]
	private void RPC_SpawnSkelly()
	{
		((GameObject)Object.Instantiate(Resources.Load("Skeleton"))).GetComponent<Skelleton>().SpawnSkelly(this.character);
	}

	// Token: 0x0600034C RID: 844 RVA: 0x00016D10 File Offset: 0x00014F10
	public bool ReadyToDisable()
	{
		if (this.currentState == MushroomZombie.State.Dead && this.timeDiedAt + 10f < Time.time)
		{
			return true;
		}
		bool flag = false;
		bool flag2 = false;
		foreach (Character character in Character.AllCharacters)
		{
			float num = Vector3.Distance(character.Center, this.character.Center);
			if (num <= 100f)
			{
				flag2 = true;
			}
			if (num <= this.distanceToEnable + 5f)
			{
				flag = true;
			}
		}
		return !flag2 || ((this.currentState == MushroomZombie.State.Dead || this.currentState == MushroomZombie.State.Sleeping) && !flag);
	}

	// Token: 0x040002CE RID: 718
	public float reachForce;

	// Token: 0x040002CF RID: 719
	public float targetForcedUntil;

	// Token: 0x040002D0 RID: 720
	public AnimatedMouth animatedMouth;

	// Token: 0x040002D1 RID: 721
	public Character _currentTarget;

	// Token: 0x040002D2 RID: 722
	public float zombieSprintDistance = 20f;

	// Token: 0x040002D3 RID: 723
	public float lookAngleBeforeWakeup = 30f;

	// Token: 0x040002D4 RID: 724
	public float distanceBeforeWakeup = 30f;

	// Token: 0x040002D5 RID: 725
	public float initialWakeUpTime = 5f;

	// Token: 0x040002D6 RID: 726
	public float distanceBeforeChase = 30f;

	// Token: 0x040002D7 RID: 727
	public float chaseTimeBeforeSprint = 3f;

	// Token: 0x040002D8 RID: 728
	public float zombieLungeDistance = 10f;

	// Token: 0x040002D9 RID: 729
	public float lungeTime = 1.5f;

	// Token: 0x040002DA RID: 730
	public float lungeRecoveryTime = 5f;

	// Token: 0x040002DB RID: 731
	public float biteStunTime = 3f;

	// Token: 0x040002DC RID: 732
	public float biteInitialInjury;

	// Token: 0x040002DD RID: 733
	public float biteInitialSpores;

	// Token: 0x040002DE RID: 734
	public float biteDelayBeforeSpores;

	// Token: 0x040002DF RID: 735
	public float biteSporesPerSecond;

	// Token: 0x040002E0 RID: 736
	public float totalBiteSporesTime;

	// Token: 0x040002E1 RID: 737
	public GameObject skirt;

	// Token: 0x040002E2 RID: 738
	public GameObject shorts;

	// Token: 0x040002E3 RID: 739
	public Texture zombieEyeTexture;

	// Token: 0x040002E4 RID: 740
	public bool isNPCZombie = true;

	// Token: 0x040002E5 RID: 741
	public float distanceToEnable;

	// Token: 0x040002E6 RID: 742
	public List<GameObject> mushroomVisuals;

	// Token: 0x040002E7 RID: 743
	public SFX_Instance gruntSFX;

	// Token: 0x040002E8 RID: 744
	public SFX_Instance knockoutSFX;

	// Token: 0x040002E9 RID: 745
	public SFX_Instance[] biteSFX;

	// Token: 0x040002EA RID: 746
	public AudioSource gruntSource;

	// Token: 0x040002EB RID: 747
	private Character spawnedFromCharacter;

	// Token: 0x040002EC RID: 748
	public float lifetime = 120f;

	// Token: 0x040002ED RID: 749
	private bool mushroomsGrowing = true;

	// Token: 0x040002EE RID: 750
	public Vector2 minMaxMushroomGrowTime;

	// Token: 0x040002EF RID: 751
	public GameObject biteColliderObject;

	// Token: 0x040002F0 RID: 752
	private bool wearingSkirt = true;

	// Token: 0x040002F1 RID: 753
	private float timeDiedAt;

	// Token: 0x040002F2 RID: 754
	public RuntimeAnimatorController animatorMushroomMan;

	// Token: 0x040002F3 RID: 755
	public MushroomZombieSpawner spawner;

	// Token: 0x040002F4 RID: 756
	public MushroomZombie.State _currentState;

	// Token: 0x040002F5 RID: 757
	public Character discovered;

	// Token: 0x040002F6 RID: 758
	internal Character character;

	// Token: 0x040002F7 RID: 759
	private PhotonView view;

	// Token: 0x040002F8 RID: 760
	private float timeAwoke;

	// Token: 0x040002F9 RID: 761
	private ZombiePhobiaSetting _setting;

	// Token: 0x040002FA RID: 762
	public Vector2 zombieGruntWaitTimeMinMax = new Vector2(10f, 20f);

	// Token: 0x040002FB RID: 763
	private List<float> mushroomGrowTimes = new List<float>();

	// Token: 0x040002FC RID: 764
	public float sinceLookForTarget = 99f;

	// Token: 0x040002FD RID: 765
	public float distanceToTarget;

	// Token: 0x040002FE RID: 766
	private float sinceSeenTarget;

	// Token: 0x040002FF RID: 767
	private float achievementTestTick;

	// Token: 0x04000300 RID: 768
	public bool visible;

	// Token: 0x04000301 RID: 769
	private float idledFor;

	// Token: 0x04000302 RID: 770
	private float timeSpentLunging;

	// Token: 0x04000303 RID: 771
	private float timeSpentRecoveringFromLunge;

	// Token: 0x04000304 RID: 772
	private float timeSpentWakingUp;

	// Token: 0x04000305 RID: 773
	private float timeSpentChasing;

	// Token: 0x04000306 RID: 774
	private float attackHeightDelta = 100f;

	// Token: 0x04000307 RID: 775
	private Vector3 lungeTargetForward;

	// Token: 0x02000422 RID: 1058
	public enum State
	{
		// Token: 0x0400183C RID: 6204
		Sleeping,
		// Token: 0x0400183D RID: 6205
		WakingUp,
		// Token: 0x0400183E RID: 6206
		Idle,
		// Token: 0x0400183F RID: 6207
		Chasing,
		// Token: 0x04001840 RID: 6208
		Lunging,
		// Token: 0x04001841 RID: 6209
		LungeRecovery,
		// Token: 0x04001842 RID: 6210
		Dead
	}
}
