using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Peak;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x0200000A RID: 10
[DefaultExecutionOrder(-100)]
public class Character : MonoBehaviourPun
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000041 RID: 65 RVA: 0x00002F4C File Offset: 0x0000114C
	public bool IsPlayerControlled
	{
		get
		{
			return !this.isBot && !this.isZombie && !this.isScoutmaster;
		}
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000042 RID: 66 RVA: 0x00002F69 File Offset: 0x00001169
	public Player player
	{
		get
		{
			return PlayerHandler.GetPlayer(this.view.Owner);
		}
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000043 RID: 67 RVA: 0x00002F7C File Offset: 0x0000117C
	public static Character observedCharacter
	{
		get
		{
			Character specCharacter = MainCameraMovement.specCharacter;
			if (specCharacter)
			{
				return specCharacter;
			}
			return Character.localCharacter;
		}
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000044 RID: 68 RVA: 0x00002F9E File Offset: 0x0000119E
	public bool inAirport
	{
		get
		{
			return this.refs.afflictions.m_inAirport;
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000045 RID: 69 RVA: 0x00002FB0 File Offset: 0x000011B0
	// (set) Token: 0x06000046 RID: 70 RVA: 0x00002FB8 File Offset: 0x000011B8
	public PlayerGhost Ghost { get; set; }

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000047 RID: 71 RVA: 0x00002FC1 File Offset: 0x000011C1
	public bool IsGhost
	{
		get
		{
			return this.Ghost != null;
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000048 RID: 72 RVA: 0x00002FD0 File Offset: 0x000011D0
	public bool IsRegisteredToPlayer
	{
		get
		{
			Character x;
			return this.IsPlayerControlled && PlayerHandler.TryGetCharacter(base.photonView.OwnerActorNr, out x) && x == this;
		}
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000049 RID: 73 RVA: 0x00003002 File Offset: 0x00001202
	public bool IsLocal
	{
		get
		{
			return this == Character.localCharacter;
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x0600004A RID: 74 RVA: 0x0000300F File Offset: 0x0000120F
	public bool IsObserved
	{
		get
		{
			return this == Character.observedCharacter;
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x0600004B RID: 75 RVA: 0x0000301C File Offset: 0x0000121C
	public Vector3 VirtualCenter
	{
		get
		{
			if (this.data.dead)
			{
				return this.LastLivingPosition;
			}
			if (!this.warping)
			{
				return this.Center;
			}
			return this._lastWarpTarget;
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x0600004C RID: 76 RVA: 0x00003047 File Offset: 0x00001247
	public Vector3 Center
	{
		get
		{
			return this.GetBodypart(BodypartType.Torso).transform.position;
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x0600004D RID: 77 RVA: 0x0000305A File Offset: 0x0000125A
	public Vector3 Head
	{
		get
		{
			return this.GetBodypart(BodypartType.Head).transform.position;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x0600004E RID: 78 RVA: 0x0000306D File Offset: 0x0000126D
	public string characterName
	{
		get
		{
			if (!this.isBot)
			{
				return this.view.Owner.NickName;
			}
			return "Bot";
		}
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003090 File Offset: 0x00001290
	public static bool GetCharacterWithPhotonID(int photonID, out Character characterResult)
	{
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (Character.AllCharacters[i] != null && Character.AllCharacters[i].photonView.ViewID == photonID)
			{
				characterResult = Character.AllCharacters[i];
				return true;
			}
		}
		characterResult = null;
		return false;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x000030F0 File Offset: 0x000012F0
	private void OnDestroy()
	{
		Character.AllCharacters.Remove(this);
		Character.AllBotCharacters.Remove(this);
	}

	// Token: 0x06000051 RID: 81 RVA: 0x0000310C File Offset: 0x0000130C
	private void Awake()
	{
		if (!this.isBot)
		{
			Character.AllCharacters.Add(this);
		}
		else
		{
			Character.AllBotCharacters.Add(this);
		}
		this.view = base.GetComponent<PhotonView>();
		if (this.view != null)
		{
			if (!this.isBot)
			{
				PlayerHandler.RegisterCharacter(this);
				if (this.view.IsMine)
				{
					Character.localCharacter = this;
					VoiceClientHandler.LocalPlayerAssigned(base.GetComponentInChildren<Recorder>());
				}
				base.gameObject.name = string.Format("Character [{0} : {1}]", this.view.Owner.NickName, this.view.Owner.ActorNumber);
			}
			else
			{
				base.gameObject.name = "Bot";
			}
		}
		this.InitializeRefs();
		this.input.Init();
	}

	// Token: 0x06000052 RID: 82 RVA: 0x000031DC File Offset: 0x000013DC
	public void InitializeRefs()
	{
		if (this._refsInitialized)
		{
			return;
		}
		this.refs.animatedVariables = base.GetComponentInChildren<AnimatedVariables>();
		this.refs.movement = base.GetComponent<CharacterMovement>();
		this.refs.carriying = base.GetComponent<CharacterCarrying>();
		this.refs.ragdoll = base.GetComponent<CharacterRagdoll>();
		this.refs.balloons = base.GetComponent<CharacterBalloons>();
		this.refs.ropeHandling = base.GetComponent<CharacterRopeHandling>();
		this.refs.rigCreator = base.GetComponentInChildren<RigCreator>();
		this.refs.animations = base.GetComponentInChildren<CharacterAnimations>();
		this.refs.animator = this.refs.rigCreator.GetComponent<Animator>();
		this.refs.items = base.GetComponent<CharacterItems>();
		this.refs.climbing = base.GetComponent<CharacterClimbing>();
		this.refs.afflictions = base.GetComponent<CharacterAfflictions>();
		this.refs.view = base.GetComponent<PhotonView>();
		this.refs.heatEmission = base.GetComponentInChildren<CharacterHeatEmission>();
		this.refs.vineClimbing = base.GetComponentInChildren<CharacterVineClimbing>();
		this.refs.interactible = base.GetComponent<CharacterInteractible>();
		this.refs.customization = base.GetComponentInChildren<CharacterCustomization>();
		this.refs.stats = base.GetComponentInChildren<CharacterStats>();
		this.refs.grabbing = base.GetComponent<CharacterGrabbing>();
		this.refs.hideTheBody = base.GetComponentInChildren<HideTheBody>();
		this.refs.badgeUnlocker = base.GetComponent<BadgeUnlocker>();
		this.jumpAction = (Action)Delegate.Combine(this.jumpAction, new Action(this.JumpStickEffect));
		this.refs.ikRigBuilder = this.refs.rigCreator.GetComponent<RigBuilder>();
		if (this.refs.ikRigBuilder)
		{
			this.refs.ikRig = this.refs.rigCreator.GetComponentInChildren<Rig>();
			this.refs.IKHandTargetLeft = this.refs.ikRig.transform.Find("IK_Arm_Left/Target");
			this.refs.IKHandTargetRight = this.refs.ikRig.transform.Find("IK_Arm_Right/Target");
			if (this.refs.IKHandTargetLeft)
			{
				this.refs.ikLeft = this.refs.IKHandTargetLeft.transform.parent.GetComponent<TwoBoneIKConstraint>();
				this.refs.ikRight = this.refs.IKHandTargetRight.transform.parent.GetComponent<TwoBoneIKConstraint>();
			}
		}
		this.CreateHelperObjects();
		this._refsInitialized = true;
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00003481 File Offset: 0x00001681
	public void BreakCharacterCarrying(bool sendRPC = false)
	{
		if (this.data.isCarried)
		{
			this.data.carrier.DropCarriedCharacter(sendRPC);
			return;
		}
		if (this.data.IsCarryingCharacter)
		{
			this.DropCarriedCharacter(sendRPC);
		}
	}

	// Token: 0x06000054 RID: 84 RVA: 0x000034B8 File Offset: 0x000016B8
	private void DropCarriedCharacter(bool sendRPC)
	{
		if (!this.data.IsCarryingCharacter)
		{
			Debug.LogWarning("Called DropCarriedCharacter but we're not carrying anyone. Doing nothing.");
			return;
		}
		if (sendRPC)
		{
			this.refs.carriying.Drop(this.data.carriedPlayer);
			return;
		}
		this.refs.carriying.RPCA_Drop(this.data.carriedPlayer.photonView);
	}

	// Token: 0x06000055 RID: 85 RVA: 0x0000351C File Offset: 0x0000171C
	internal void SetDeadAfterReconnect(Vector3 lastLivingPosition)
	{
		this.LastLivingPosition = lastLivingPosition;
		base.photonView.RPC("RPCA_SetDead", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000056 RID: 86 RVA: 0x0000353B File Offset: 0x0000173B
	// (set) Token: 0x06000057 RID: 87 RVA: 0x00003543 File Offset: 0x00001743
	public Vector3 LastLivingPosition { get; private set; }

	// Token: 0x06000058 RID: 88 RVA: 0x0000354C File Offset: 0x0000174C
	public Vector3 GetSpectatePosition()
	{
		if (this.data.dead)
		{
			return this.LastLivingPosition;
		}
		return this.Center;
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00003568 File Offset: 0x00001768
	internal void AddForceAtPosition(Vector3 force, Vector3 point, float radius)
	{
		this.view.RPC("RPCA_AddForceAtPosition", RpcTarget.All, new object[]
		{
			force,
			point,
			radius
		});
	}

	// Token: 0x0600005A RID: 90 RVA: 0x0000359C File Offset: 0x0000179C
	[PunRPC]
	public void RPCA_AddForceAtPosition(Vector3 force, Vector3 point, float radius)
	{
		foreach (Bodypart bodypart in this.refs.ragdoll.partList)
		{
			float value = Vector3.Distance(bodypart.Rig.worldCenterOfMass, point);
			float d = Mathf.InverseLerp(radius, radius * 0.1f, value);
			Rigidbody rig = bodypart.Rig;
			Vector3 position = Vector3.Lerp(point, rig.worldCenterOfMass, 0.5f);
			rig.AddForceAtPosition(force * d, position, ForceMode.Impulse);
		}
	}

	// Token: 0x0600005B RID: 91 RVA: 0x0000363C File Offset: 0x0000183C
	[ConsoleCommand]
	public static void GainFullStamina()
	{
		Character.localCharacter.AddStamina(1f);
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00003650 File Offset: 0x00001850
	private void CreateHelperObjects()
	{
		this.refs.helperObjects = new GameObject("helperObjects").transform;
		this.refs.helperObjects.transform.SetParent(base.transform);
		this.refs.helperObjects.transform.localPosition = Vector3.zero;
		this.refs.helperObjects.transform.localRotation = Quaternion.identity;
		this.refs.animationHeadTransform = Object.Instantiate<GameObject>(this.refs.helperObjects.gameObject, this.refs.helperObjects).transform;
		this.refs.animationHeadTransform.gameObject.name = "animationHead";
		this.refs.animationHipTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationHipTransform.gameObject.name = "animationHip";
		this.refs.animationItemTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationItemTransform.gameObject.name = "animationItem";
		this.refs.animationLookTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationLookTransform.gameObject.name = "animationLook";
		this.refs.animationPositionTransform = Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
		this.refs.animationPositionTransform.gameObject.name = "animationPosition";
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00003838 File Offset: 0x00001A38
	public void Start()
	{
		if (this.started)
		{
			return;
		}
		this.started = true;
		this.refs.hip = this.GetBodypart(BodypartType.Hip);
		this.refs.head = this.GetBodypart(BodypartType.Head);
		base.gameObject.name = string.Format("Character [{0} : {1}]", this.view.Owner.NickName, this.view.Owner.ActorNumber);
		CharacterAfflictions afflictions = this.refs.afflictions;
		afflictions.OnAddedIncrementalStatus = (Action<CharacterAfflictions.STATUSTYPE, float>)Delegate.Combine(afflictions.OnAddedIncrementalStatus, new Action<CharacterAfflictions.STATUSTYPE, float>(this.OnAddedStatus));
		this.smoothedCamPos = this.GetBodypart(BodypartType.Head).transform.TransformPoint(Vector3.up * 1f);
	}

	// Token: 0x0600005E RID: 94 RVA: 0x00003905 File Offset: 0x00001B05
	private void OnAddedStatus(CharacterAfflictions.STATUSTYPE sTATUSTYPE, float amount)
	{
		if (sTATUSTYPE == CharacterAfflictions.STATUSTYPE.Cold && amount > 0f)
		{
			this.data.sinceAddedCold = 0f;
		}
	}

	// Token: 0x0600005F RID: 95 RVA: 0x00003924 File Offset: 0x00001B24
	private void Update()
	{
		this.HandleStickUpdate();
		this.UpdateVariables();
		if (this.data.dead)
		{
			this.data.sinceDied += Time.deltaTime;
		}
		else
		{
			this.data.sinceDied = 0f;
		}
		if (!base.photonView.IsMine)
		{
			return;
		}
		if (this.data.dead)
		{
			this.HandleDeath();
			return;
		}
		if (this.data.passedOut || this.data.fullyPassedOut)
		{
			this.HandlePassedOut();
			return;
		}
		this.HandleLife();
	}

	// Token: 0x06000060 RID: 96 RVA: 0x000039BC File Offset: 0x00001BBC
	private void UpdateVariables()
	{
		this.data.ragdollControlClamp = Mathf.MoveTowards(this.data.ragdollControlClamp, 1f, Time.deltaTime * 5f);
		this.data.sinceUnstuck += Time.deltaTime;
	}

	// Token: 0x06000061 RID: 97 RVA: 0x00003A0B File Offset: 0x00001C0B
	public static Vector3 DeathPos()
	{
		return new Vector3(0f, 5000f, -5000f);
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00003A21 File Offset: 0x00001C21
	private void HandleDeath()
	{
		this.data.sinceDied += Time.deltaTime;
	}

	// Token: 0x06000063 RID: 99 RVA: 0x00003A3C File Offset: 0x00001C3C
	private void HandlePassedOut()
	{
		if (this.refs.afflictions.statusSum < 1f && Time.time - this.data.lastPassedOut > 3f)
		{
			if (!this.UnPassOutCalled)
			{
				this.view.RPC("RPCA_UnPassOut", RpcTarget.All, Array.Empty<object>());
				this.passOutFailsafeTick = 0f;
			}
			else
			{
				this.passOutFailsafeTick += Time.deltaTime;
				if (this.passOutFailsafeTick > 3f)
				{
					Debug.Log("Passed out failsafe triggered.");
					this.UnPassOutCalled = false;
				}
			}
		}
		this.ZombieFailsafe();
		if (this.data.deathTimer > 1f)
		{
			this.refs.items.EquipSlot(Optionable<byte>.None);
			if (!this.TryCheckpoint())
			{
				if (this.refs.afflictions.willZombify && !this.data.zombified)
				{
					if (!PhotonNetwork.IsMasterClient)
					{
						this.data.zombified = true;
					}
					this.view.RPC("RPCA_Zombify", RpcTarget.MasterClient, new object[]
					{
						this.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f
					});
					return;
				}
				this.view.RPC("RPCA_Die", RpcTarget.All, new object[]
				{
					this.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f
				});
			}
		}
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00003BDB File Offset: 0x00001DDB
	private void ZombieFailsafe()
	{
		if (this.data.zombified && !this.data.dead && Time.time > this.lastZombified + 5f)
		{
			this.data.zombified = false;
		}
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00003C18 File Offset: 0x00001E18
	private bool TryCheckpoint()
	{
		if (this.data.checkpointFlags.Count == 0)
		{
			return false;
		}
		for (int i = this.data.checkpointFlags.Count - 1; i >= 0; i--)
		{
			if (this.data.checkpointFlags[i])
			{
				CheckpointFlag checkpointFlag = this.data.checkpointFlags[i];
				this.data.checkpointFlags.Remove(checkpointFlag);
				this.data.deathTimer = 0f;
				this.refs.afflictions.ClearAllStatus(false);
				this.refs.afflictions.ApplyStatusesFromFloatArray(checkpointFlag.currentStatuses);
				this.refs.afflictions.RemoveAllThorns();
				this.data.fallSeconds = 0f;
				this.data.currentRagdollControll = 0f;
				Character.localCharacter.view.RPC("RegainItems", RpcTarget.MasterClient, new object[]
				{
					Character.localCharacter.view
				});
				this.WarpPlayer(checkpointFlag.transform.position + Vector3.up, true);
				checkpointFlag.DestroySelf();
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00003D50 File Offset: 0x00001F50
	[PunRPC]
	private void RegainItems(PhotonView regainingCharacterView)
	{
		Character component = regainingCharacterView.GetComponent<Character>();
		if (!component)
		{
			return;
		}
		for (int i = 0; i < component.refs.items.droppedItems.Count; i++)
		{
			if (component.refs.items.droppedItems[i])
			{
				Item component2 = component.refs.items.droppedItems[i].GetComponent<Item>();
				if (component2)
				{
					component.refs.items.lastEquippedSlotTime = 0f;
					component2.Interact(component);
				}
			}
		}
		component.refs.items.droppedItems.Clear();
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00003E00 File Offset: 0x00002000
	[ConsoleCommand]
	public static void Die()
	{
		Character.localCharacter.refs.items.EquipSlot(Optionable<byte>.None);
		Debug.Log("DYING");
		Character.localCharacter.view.RPC("RPCA_Die", RpcTarget.All, new object[]
		{
			Character.localCharacter.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f
		});
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00003E85 File Offset: 0x00002085
	internal void DieInstantly()
	{
		if (!this.TryCheckpoint())
		{
			this.view.RPC("RPCA_Die", RpcTarget.All, new object[]
			{
				this.Center
			});
		}
	}

	// Token: 0x06000069 RID: 105 RVA: 0x00003EB4 File Offset: 0x000020B4
	[PunRPC]
	public void RPCA_SetDead()
	{
		this.data.dead = true;
		this.data.fullyPassedOut = true;
		this.data.deathTimer = 1f;
		this.data.passedOut = true;
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00003EEC File Offset: 0x000020EC
	[PunRPC]
	public void RPCA_Die(Vector3 itemSpawnPoint)
	{
		this.refs.items.EquipSlot(Optionable<byte>.None);
		this.RPCA_SetDead();
		this.refs.stats.justDied = true;
		this.refs.stats.Record(false, 0f);
		ItemSlot[] itemSlots = this.player.itemSlots;
		this.refs.items.DropAllItems(true);
		if (this.IsLocal)
		{
			this.SetExtraStamina(0f);
		}
		Debug.Log(base.gameObject.name + " died");
		if (this.data.isSkeleton)
		{
			((GameObject)Object.Instantiate(Resources.Load("SkeletonExplosion"))).GetComponent<SkeletonExplosion>().Boom(this);
			if (this.IsLocal)
			{
				this.data.SetSkeleton(false);
			}
		}
		else
		{
			((GameObject)Object.Instantiate(Resources.Load("Skeleton"))).GetComponent<Skelleton>().SpawnSkelly(this);
		}
		this.WarpPlayer(Character.DeathPos(), false);
		this.CheckEndGame();
		Debug.Log("DIE");
		GlobalEvents.TriggerCharacterDied(this);
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00004008 File Offset: 0x00002208
	[ConsoleCommand]
	public static void Zombify()
	{
		Character.localCharacter.view.RPC("RPCA_Zombify", RpcTarget.All, new object[]
		{
			Character.localCharacter.Center
		});
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00004038 File Offset: 0x00002238
	[ConsoleCommand]
	public static void TestWarp()
	{
		foreach (Bodypart bodypart in Character.localCharacter.transform.GetComponentsInChildren<Bodypart>())
		{
			Debug.Log("Warping body part " + bodypart.partType.ToString());
			bodypart.transform.position = bodypart.transform.position + Vector3.up * 50f;
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x000040B4 File Offset: 0x000022B4
	[PunRPC]
	public void RPCA_Zombify(Vector3 itemSpawnPoint)
	{
		Debug.Log(base.gameObject.name + " became a zombie");
		if (!this.data.zombified)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Instantiate("MushroomZombie_Player", this.Center, base.transform.rotation, 0, null).GetComponent<PhotonView>().RPC("RPC_Arise", RpcTarget.All, new object[]
				{
					base.photonView.ViewID
				});
			}
			this.data.zombified = true;
		}
	}

	// Token: 0x0600006E RID: 110 RVA: 0x00004144 File Offset: 0x00002344
	public void FinishZombifying()
	{
		this.refs.items.EquipSlot(Optionable<byte>.None);
		this.data.dead = true;
		this.data.zombified = true;
		this.data.fullyPassedOut = true;
		this.data.deathTimer = 1f;
		this.data.passedOut = true;
		this.refs.stats.justDied = true;
		this.refs.stats.Record(false, 0f);
		if (this.IsLocal)
		{
			this.SetExtraStamina(0f);
		}
		ItemSlot[] itemSlots = this.player.itemSlots;
		this.refs.items.DropAllItems(true);
		this.WarpPlayer(Character.DeathPos(), false);
		GlobalEvents.TriggerCharacterDied(this);
		this.CheckEndGame();
	}

	// Token: 0x0600006F RID: 111 RVA: 0x00004218 File Offset: 0x00002418
	public void CheckEndGame()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			bool flag = true;
			for (int i = 0; i < Character.AllCharacters.Count; i++)
			{
				if (!Character.AllCharacters[i].data.dead)
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.EndGame();
			}
		}
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00004265 File Offset: 0x00002465
	[ConsoleCommand]
	internal static void TestWin()
	{
		Character.localCharacter.photonView.RPC("RPCEndGame_ForceWin", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000071 RID: 113 RVA: 0x00004281 File Offset: 0x00002481
	internal void EndGame()
	{
		base.photonView.RPC("RPCEndGame", RpcTarget.All, Array.Empty<object>());
		RunManager.Instance.EndGame();
	}

	// Token: 0x06000072 RID: 114 RVA: 0x000042A3 File Offset: 0x000024A3
	[PunRPC]
	public void RPCA_SyncScoutReport(CharacterStats.SyncData reconnectData, bool wasRevived, PhotonMessageInfo msgInfo)
	{
		this.refs.stats.GetCaughtUp(reconnectData, wasRevived);
	}

	// Token: 0x06000073 RID: 115 RVA: 0x000042B8 File Offset: 0x000024B8
	[PunRPC]
	public void RPCA_InitializeScoutReport()
	{
		CharacterStats characterStats;
		if (!base.TryGetComponent<CharacterStats>(out characterStats))
		{
			Debug.LogError("Unable to find " + base.name + "'s stats!! Scout report will be broken.");
			return;
		}
		if (characterStats.IsInitialized)
		{
			Debug.LogWarning("OPE! Scout Report was already initialized. Doing nothing (it's probably broken).");
			return;
		}
		characterStats.Initialize();
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00004303 File Offset: 0x00002503
	[PunRPC]
	private void RPCEndGame_ForceWin()
	{
		Character.forceWin = true;
		this.RPCEndGame();
		Character.forceWin = false;
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00004318 File Offset: 0x00002518
	[PunRPC]
	private void RPCEndGame()
	{
		Quicksave.DestroySaveData();
		bool flag = false;
		foreach (Character character in Character.AllCharacters)
		{
			if (Character.CheckWinCondition(character))
			{
				character.refs.stats.Win();
				flag = true;
			}
		}
		foreach (Character character2 in Character.AllCharacters)
		{
			if (!Character.CheckWinCondition(character2))
			{
				character2.refs.stats.Lose(flag);
			}
		}
		MenuWindow.CloseAllWindows();
		if (flag)
		{
			GlobalEvents.TriggerSomeoneWonRun();
			if (RunSettings.isMiniRun)
			{
				GUIManager.instance.endScreen.Open();
			}
			else
			{
				Singleton<PeakHandler>.Instance.EndCutscene();
			}
		}
		else
		{
			GUIManager.instance.endScreen.Open();
		}
		GlobalEvents.TriggerRunEnded();
	}

	// Token: 0x06000076 RID: 118 RVA: 0x0000441C File Offset: 0x0000261C
	public static bool CheckWinCondition(Character c)
	{
		if (Character.forceWin)
		{
			return true;
		}
		if (RunSettings.isMiniRun)
		{
			return !c.data.dead;
		}
		return ((c.data.isRopeClimbing && c.data.heldRope.isHelicopterRope) || Singleton<MountainProgressHandler>.Instance.IsAtPeak(c.Center)) && !c.data.dead;
	}

	// Token: 0x06000077 RID: 119 RVA: 0x0000448C File Offset: 0x0000268C
	[PunRPC]
	private void RPCA_UnPassOut()
	{
		this.UnPassOutCalled = true;
		this.data.deathTimer = 0f;
		if (this.IsLocal)
		{
			Transitions.instance.PlayTransition(TransitionType.FadeToBlack, new Action(this.UnPassOutDone), 1f, 1f);
			return;
		}
		this.UnPassOutDone();
	}

	// Token: 0x06000078 RID: 120 RVA: 0x000044E0 File Offset: 0x000026E0
	private void UnPassOutDone()
	{
		Debug.Log("UhPassOut");
		Action unPassOutAction = this.UnPassOutAction;
		if (unPassOutAction != null)
		{
			unPassOutAction();
		}
		this.data.fullyPassedOut = false;
		this.data.passedOut = false;
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00004515 File Offset: 0x00002715
	[ConsoleCommand]
	public static void PassOut()
	{
		CharacterAfflictions.Starve();
		Character.localCharacter.view.RPC("RPCA_PassOut", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600007A RID: 122 RVA: 0x00004538 File Offset: 0x00002738
	[PunRPC]
	public void RPCA_PassOut()
	{
		this.UnPassOutCalled = false;
		this.data.passedOut = true;
		this.refs.stats.justPassedOut = true;
		this.data.lastPassedOut = Time.time;
		this.refs.stats.Record(false, 0f);
		if (this.IsLocal)
		{
			GUIManager.instance.strugglePrompt.gameObject.SetActive(false);
		}
		GlobalEvents.TriggerCharacterPassedOut(this);
		if (PhotonNetwork.IsMasterClient)
		{
			this.refs.items.droppedItems.Clear();
		}
		if (this.IsLocal)
		{
			Transitions.instance.PlayTransition(TransitionType.FadeToBlack, new Action(this.<RPCA_PassOut>g__PassOutDone|94_0), 1f, 1f);
		}
		else
		{
			this.<RPCA_PassOut>g__PassOutDone|94_0();
		}
		Debug.Log("PASS OUT");
	}

	// Token: 0x0600007B RID: 123 RVA: 0x0000460C File Offset: 0x0000280C
	private void HandleLife()
	{
		if (this.refs.afflictions.shouldPassOut)
		{
			if (this.data.isSkeleton)
			{
				if (!this.TryCheckpoint())
				{
					this.view.RPC("RPCA_Die", RpcTarget.All, new object[]
					{
						Character.localCharacter.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f
					});
				}
				return;
			}
			this.data.passOutValue = Mathf.MoveTowards(this.data.passOutValue, 1f, Time.deltaTime / 5f);
			if (this.data.passOutValue > 0.999f)
			{
				this.view.RPC("RPCA_PassOut", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.data.passOutValue = Mathf.MoveTowards(this.data.passOutValue, 0f, Time.deltaTime / 5f);
		}
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00004714 File Offset: 0x00002914
	public void PassOutInstantly()
	{
		this.data.passOutValue = 1f;
		this.view.RPC("RPCA_PassOut", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600007D RID: 125 RVA: 0x0000473C File Offset: 0x0000293C
	private void FixedUpdate()
	{
		this.UpdateVariablesFixed();
		if (this.data.dead)
		{
			this.refs.ragdoll.MoveAllRigsInDirection(Character.DeathPos() - this.Center);
			this.refs.ragdoll.HaltBodyVelocity();
			return;
		}
		this.LastLivingPosition = this.Center;
	}

	// Token: 0x0600007E RID: 126 RVA: 0x0000479C File Offset: 0x0000299C
	private void UpdateVariablesFixed()
	{
		float targetRagdollControll = this.data.GetTargetRagdollControll();
		if (targetRagdollControll < this.data.currentRagdollControll)
		{
			this.data.currentRagdollControll = targetRagdollControll;
		}
		else if (this.data.currentRagdollControll > 0.5f)
		{
			this.data.currentRagdollControll = Mathf.MoveTowards(this.data.currentRagdollControll, targetRagdollControll, Time.fixedDeltaTime * 1f);
		}
		else
		{
			this.data.currentRagdollControll = Mathf.MoveTowards(this.data.currentRagdollControll, targetRagdollControll, Time.fixedDeltaTime * 0.5f);
		}
		this.data.staminaDelta = this.data.currentStamina + this.data.extraStamina - this.data.lastFrameTotalStamina;
		this.data.lastFrameTotalStamina = this.data.currentStamina + this.data.extraStamina;
		if (this.data.isGrounded)
		{
			this.data.groundedFor += Time.fixedDeltaTime;
			this.data.sinceGrounded = 0f;
			this.data.lastGroundedHeight = this.Center.y;
		}
		else
		{
			this.data.groundedFor = 0f;
			if (this.data.sinceGrounded < 1f || this.data.avarageVelocity.y < -1f)
			{
				this.data.sinceGrounded += Time.fixedDeltaTime;
			}
		}
		if (this.data.isClimbing || this.data.isRopeClimbing || this.data.isVineClimbing)
		{
			this.data.sinceClimb = 0f;
		}
		if (this.data.dead)
		{
			this.data.sinceDead = 0f;
		}
		if (this.OutOfStamina())
		{
			this.data.outOfStaminaFor += Time.fixedDeltaTime;
		}
		else
		{
			this.data.outOfStaminaFor = 0f;
		}
		this.data.staminaMod = Mathf.Max(Mathf.Clamp01(this.GetTotalStamina() * 5f), 0.2f);
		this.data.sinceClimbJump += Time.fixedDeltaTime;
		if (this.data.fallSeconds > 0f)
		{
			if (this.data.isGrounded)
			{
				this.data.fallSeconds -= Time.fixedDeltaTime;
			}
			else
			{
				this.data.fallSeconds -= Time.fixedDeltaTime * 0.2f;
			}
			if (this.data.fallSeconds <= 0f)
			{
				this.StoppedForcedRagdolling();
			}
		}
		if (this.data.fullyPassedOut)
		{
			if (this.input.interactIsPressed)
			{
				this.data.deathTimer += Time.fixedDeltaTime * 0.33f;
			}
			else if (!this.data.carrier || this.refs.afflictions.willZombify)
			{
				if (!this.HasMeaningfulTempStatuses() && this.NobodyIsAlive())
				{
					this.data.deathTimer += Time.fixedDeltaTime / 10f;
				}
				else
				{
					this.data.deathTimer += Time.fixedDeltaTime / 60f;
				}
			}
		}
		else
		{
			this.data.sinceDied = 0f;
		}
		if (this.input.usePrimaryIsPressed && this.data.currentItem == null)
		{
			this.data.sincePressClimb = 0f;
		}
		if (this.input.useSecondaryIsPressed && this.data.currentItem == null)
		{
			this.data.sincePressReach = 0f;
		}
		this.data.sincePressClimb += Time.fixedDeltaTime;
		this.data.sincePressReach += Time.fixedDeltaTime;
		this.data.sinceAddedCold += Time.fixedDeltaTime;
		this.data.sinceStartClimb += Time.fixedDeltaTime;
		this.data.sinceGrabFriend += Time.fixedDeltaTime;
		this.data.sinceClimbHandle += Time.fixedDeltaTime;
		this.data.sinceFallSlide += Time.fixedDeltaTime;
		this.data.sinceUseStamina += Time.fixedDeltaTime;
		this.data.sinceClimb += Time.fixedDeltaTime;
		this.data.sinceJump += Time.fixedDeltaTime;
		this.data.sinceDead += Time.fixedDeltaTime;
		this.data.overrideIKForSeconds -= Time.fixedDeltaTime;
		this.data.slippy -= Time.deltaTime;
		this.data.sinceLetGoOfFriend += Time.fixedDeltaTime;
		this.data.sinceStandOnPlayer += Time.fixedDeltaTime;
		this.data.sincePalJump += Time.fixedDeltaTime;
		this.data.sinceItemAttach += Time.fixedDeltaTime;
		this.data.sinceCanClimb += Time.fixedDeltaTime;
		this.data.passedOutOnTheBeach -= Time.fixedDeltaTime;
		if (this.CanRegenStamina())
		{
			this.AddStamina(Time.fixedDeltaTime * 0.2f);
		}
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00004D40 File Offset: 0x00002F40
	private bool NobodyIsAlive()
	{
		List<Character> allCharacters = Character.AllCharacters;
		for (int i = 0; i < allCharacters.Count; i++)
		{
			if (allCharacters[i].data.fullyConscious)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000080 RID: 128 RVA: 0x00004D7C File Offset: 0x00002F7C
	private bool HasMeaningfulTempStatuses()
	{
		float num = this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy) + this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot) + this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison) + this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Thorns);
		if (!this.data.isInFog)
		{
			num += this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Cold);
		}
		return this.refs.afflictions.statusSum - num < 1f;
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00004E10 File Offset: 0x00003010
	private bool CanRegenStamina()
	{
		if (this.data.currentClimbHandle)
		{
			return true;
		}
		if (this.IsStuck())
		{
			return true;
		}
		float num = (this.data.currentStamina > 0f) ? 1f : 2f;
		return (this.data.sinceGrounded <= 0.2f || this.refs.afflictions.isWebbed) && this.data.sinceUseStamina >= num;
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00004E8F File Offset: 0x0000308F
	public float GetTotalStamina()
	{
		return this.data.currentStamina + this.data.extraStamina;
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00004EA8 File Offset: 0x000030A8
	internal Bodypart GetBodypart(BodypartType head)
	{
		return this.refs.ragdoll.partDict[head];
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00004EC0 File Offset: 0x000030C0
	internal Rigidbody GetBodypartRig(BodypartType head)
	{
		return this.refs.ragdoll.partDict[head].Rig;
	}

	// Token: 0x06000085 RID: 133 RVA: 0x00004EE0 File Offset: 0x000030E0
	internal void CalculateWorldMovementDir()
	{
		Vector3 a = default(Vector3) + this.data.lookDirection * this.input.movementInput.y;
		a.y = 0f;
		a = a.normalized;
		a += this.data.lookDirection_Right * this.input.movementInput.x;
		this.data.worldMovementInput = a.normalized;
		Vector3 lookDirection = this.data.lookDirection;
		Vector3 lookDirection_Right = this.data.lookDirection_Right;
		lookDirection.y = 0f;
		lookDirection.Normalize();
		Vector3 planeNormal = this.data.groundNormal;
		if (this.data.sinceGrounded > 0.2f)
		{
			planeNormal = Vector3.up;
		}
		Vector3 vector = HelperFunctions.GroundDirection(planeNormal, -lookDirection_Right);
		Vector3 vector2 = HelperFunctions.GroundDirection(planeNormal, lookDirection);
		if (this.data.sinceGrounded < 0.2f)
		{
			this.data.groundedForward = vector;
			this.data.groundedRight = vector2;
		}
		Vector3 vector3 = vector * this.input.movementInput.y + vector2 * this.input.movementInput.x;
		vector3 = Vector3.ClampMagnitude(vector3, 1f);
		this.data.worldMovementInput_Grounded = vector3;
		Vector3 target = this.data.worldMovementInput_Grounded;
		float num = Mathf.Lerp(this.refs.movement.movementTurnSpeed, this.refs.movement.airMovementTurnSpeed, this.data.sinceGrounded * 4f);
		if (!this.data.isGrounded)
		{
			target = this.data.worldMovementInput;
		}
		this.data.worldMovementInput_Lerp = Vector3.MoveTowards(this.data.worldMovementInput_Lerp, target, Time.deltaTime * num);
	}

	// Token: 0x06000086 RID: 134 RVA: 0x000050D0 File Offset: 0x000032D0
	internal void RecalculateLookDirections()
	{
		Vector3 normalized = HelperFunctions.LookToDirection(this.data.lookValues, Vector3.forward).normalized;
		this.data.lookDirection = normalized;
		normalized.y = 0f;
		normalized.Normalize();
		this.data.lookDirection_Flat = normalized;
		this.data.lookDirection_Right = Vector3.Cross(Vector3.up, this.data.lookDirection).normalized;
		this.data.lookDirection_Up = Vector3.Cross(this.data.lookDirection, this.data.lookDirection_Right).normalized;
	}

	// Token: 0x06000087 RID: 135 RVA: 0x00005181 File Offset: 0x00003381
	internal Vector3 GetCameraPos(float forwardOffset)
	{
		return this.GetBodypart(BodypartType.Head).transform.TransformPoint(Vector3.up * 1f + Vector3.forward * forwardOffset);
	}

	// Token: 0x06000088 RID: 136 RVA: 0x000051B4 File Offset: 0x000033B4
	internal Vector3 GetAnimationRelativePosition(Vector3 position)
	{
		Vector3 b = position - this.refs.animationHipTransform.position;
		return this.refs.hip.Rig.position + b;
	}

	// Token: 0x06000089 RID: 137 RVA: 0x000051F3 File Offset: 0x000033F3
	internal void OnLand(float sinceGrounded)
	{
		Action<float> action = this.landAction;
		if (action == null)
		{
			return;
		}
		action(sinceGrounded);
	}

	// Token: 0x0600008A RID: 138 RVA: 0x00005206 File Offset: 0x00003406
	internal void OnStartJump()
	{
		Action action = this.startJumpAction;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x0600008B RID: 139 RVA: 0x00005218 File Offset: 0x00003418
	internal void OnJump()
	{
		Action action = this.jumpAction;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x0600008C RID: 140 RVA: 0x0000522A File Offset: 0x0000342A
	internal void OnStartClimb()
	{
		Action action = this.startClimbAction;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x0600008D RID: 141 RVA: 0x0000523C File Offset: 0x0000343C
	internal Vector3 HipPos()
	{
		return this.GetBodypart(BodypartType.Hip).Rig.position;
	}

	// Token: 0x0600008E RID: 142 RVA: 0x0000524F File Offset: 0x0000344F
	internal Vector3 TorsoPos()
	{
		return this.GetBodypart(BodypartType.Torso).Rig.position;
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00005264 File Offset: 0x00003464
	internal void AddForce(Vector3 move, float minRandomMultiplier = 1f, float maxRandomMultiplier = 1f)
	{
		foreach (Bodypart bodypart in this.refs.ragdoll.partList)
		{
			Vector3 vector = move;
			if (minRandomMultiplier != maxRandomMultiplier)
			{
				vector *= Random.Range(minRandomMultiplier, maxRandomMultiplier);
			}
			bodypart.AddForce(vector, ForceMode.Acceleration);
		}
	}

	// Token: 0x06000090 RID: 144 RVA: 0x000052D4 File Offset: 0x000034D4
	internal bool CheckStand()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing;
	}

	// Token: 0x06000091 RID: 145 RVA: 0x00005304 File Offset: 0x00003504
	internal bool CheckGravity()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null);
	}

	// Token: 0x06000092 RID: 146 RVA: 0x00005354 File Offset: 0x00003554
	internal bool CheckMovement()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null) && !this.data.isKicking;
	}

	// Token: 0x06000093 RID: 147 RVA: 0x000053B4 File Offset: 0x000035B4
	internal bool CheckJump()
	{
		return !this.data.fullyPassedOut && !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null);
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00005414 File Offset: 0x00003614
	internal bool CheckSprint()
	{
		return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !(this.data.currentClimbHandle != null) && this.data.fullyConscious && (!this.data.currentItem || (!this.data.currentItem.isUsingPrimary && !this.data.currentItem.isUsingSecondary));
	}

	// Token: 0x06000095 RID: 149 RVA: 0x000054AC File Offset: 0x000036AC
	internal void SetRotation()
	{
		if (this.data.carrier)
		{
			this.refs.rigCreator.transform.rotation = this.data.carrier.refs.carryPosRef.rotation;
			return;
		}
		if (this.data.isRopeClimbing)
		{
			this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(-this.data.ropeClimbWorldNormal, this.data.ropeClimbWorldUp);
			return;
		}
		if (this.data.isClimbing)
		{
			this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(-this.data.climbNormal);
			return;
		}
		if (this.data.lookDirection_Flat != Vector3.zero)
		{
			this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(this.data.lookDirection_Flat);
		}
	}

	// Token: 0x06000096 RID: 150 RVA: 0x000055B4 File Offset: 0x000037B4
	internal bool UseStamina(float usage, bool useBonusStamina = true)
	{
		if (usage == 0f)
		{
			return false;
		}
		usage *= Ascents.climbStaminaMultiplier;
		if (!this.view.IsMine)
		{
			return this.data.currentStamina + this.data.extraStamina > usage;
		}
		if (this.data.currentStamina == 0f)
		{
			if (this.data.extraStamina > 0f && useBonusStamina)
			{
				this.data.extraStamina -= usage;
				this.data.extraStamina = Mathf.Clamp(this.data.extraStamina, 0f, 1f);
				this.data.sinceUseStamina = 0f;
				GUIManager.instance.bar.ChangeBar();
				return true;
			}
			return false;
		}
		else
		{
			this.data.currentStamina -= usage;
			this.data.sinceUseStamina = 0f;
			GUIManager.instance.bar.ChangeBar();
			if (this.data.currentStamina <= 0f)
			{
				this.ClampStamina();
				return this.data.extraStamina > 0f;
			}
			return true;
		}
	}

	// Token: 0x06000097 RID: 151 RVA: 0x000056E0 File Offset: 0x000038E0
	[PunRPC]
	public void MoraleBoost(float staminaAdd, int scoutCount)
	{
		GUIManager.instance.bar.PlayMoraleBoost(scoutCount);
		this.AddExtraStamina(staminaAdd);
	}

	// Token: 0x06000098 RID: 152 RVA: 0x000056F9 File Offset: 0x000038F9
	public void AddStamina(float add)
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.data.currentStamina += add;
		this.ClampStamina();
		GUIManager.instance.bar.ChangeBar();
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00005731 File Offset: 0x00003931
	public void ClampStamina()
	{
		this.data.currentStamina = Mathf.Clamp(this.data.currentStamina, 0f, this.GetMaxStamina());
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00005759 File Offset: 0x00003959
	public float GetMaxStamina()
	{
		return Mathf.Max(1f - this.refs.afflictions.statusSum, 0f);
	}

	// Token: 0x0600009B RID: 155 RVA: 0x0000577B File Offset: 0x0000397B
	public void SetExtraStamina(float amt)
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.data.extraStamina = Mathf.Clamp(amt, 0f, 1f);
		GUIManager.instance.bar.ChangeBar();
	}

	// Token: 0x0600009C RID: 156 RVA: 0x000057B8 File Offset: 0x000039B8
	public void AddExtraStamina(float add)
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.data.extraStamina += add;
		this.data.extraStamina = Mathf.Clamp(this.data.extraStamina, 0f, 1f);
		GUIManager.instance.bar.ChangeBar();
	}

	// Token: 0x0600009D RID: 157 RVA: 0x0000581A File Offset: 0x00003A1A
	public void FeedItem(Item item)
	{
		base.photonView.RPC("GetFedItemRPC", base.photonView.Owner, new object[]
		{
			item.photonView.ViewID
		});
	}

	// Token: 0x0600009E RID: 158 RVA: 0x00005850 File Offset: 0x00003A50
	[PunRPC]
	public void GetFedItemRPC(int itemPhotonID)
	{
		if (!base.photonView.IsMine)
		{
			return;
		}
		PhotonView photonView = PhotonView.Find(itemPhotonID);
		if (photonView == null)
		{
			return;
		}
		Item item = (photonView != null) ? photonView.GetComponent<Item>() : null;
		if (item == null)
		{
			return;
		}
		Debug.Log("I just got fed a: " + item.UIData.itemName);
		item.overrideHolderCharacter = this;
		if (item.OnPrimaryFinishedCast != null)
		{
			item.OnPrimaryFinishedCast();
		}
		if (!item.consuming)
		{
			item.overrideHolderCharacter = null;
		}
	}

	// Token: 0x0600009F RID: 159 RVA: 0x000058D8 File Offset: 0x00003AD8
	internal void DragTowards(Vector3 target, float force)
	{
		Action<Vector3, float> action = this.dragTowardsAction;
		if (action != null)
		{
			action(target, force);
		}
		Vector3 a = Vector3.ClampMagnitude(target - this.Center, 1f);
		this.AddForce(a * force, 1f, 1f);
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x00005926 File Offset: 0x00003B26
	internal bool OutOfStamina()
	{
		return this.data.currentStamina < 0.005f && this.data.extraStamina < 0.001f;
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x0000594E File Offset: 0x00003B4E
	internal bool OutOfRegularStamina()
	{
		return this.data.currentStamina < 0.005f;
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x00005962 File Offset: 0x00003B62
	internal bool IsSliding()
	{
		return this.data.isClimbing && this.OutOfStamina();
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00005979 File Offset: 0x00003B79
	internal bool CanDoInput()
	{
		return !GUIManager.instance.windowBlockingInput && !GUIManager.instance.wheelActive;
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00005998 File Offset: 0x00003B98
	internal int GetPlayerListID(List<Character> playerList)
	{
		for (int i = 0; i < playerList.Count; i++)
		{
			if (playerList[i] == this)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x000059C8 File Offset: 0x00003BC8
	internal void Fall(float seconds, float screenShake = 0f)
	{
		if (screenShake <= 1E-05f || !this.refs.view.IsMine)
		{
			this.refs.view.RPC("RPCA_Fall", RpcTarget.All, new object[]
			{
				seconds
			});
		}
		else
		{
			this.refs.view.RPC("RPCA_FallWithScreenShake", RpcTarget.All, new object[]
			{
				seconds,
				screenShake
			});
		}
		GlobalEvents.TriggerCharacterFell(this, seconds);
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00005A4B File Offset: 0x00003C4B
	[PunRPC]
	public void RPCA_UnFall()
	{
		this.data.fallSeconds = 0f;
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00005A5D File Offset: 0x00003C5D
	[PunRPC]
	public void RPCA_Fall(float seconds)
	{
		if (base.photonView.IsMine)
		{
			Debug.Log(string.Format("I fell for {0} seconds", seconds));
		}
		if (seconds > this.data.fallSeconds)
		{
			this.data.fallSeconds = seconds;
		}
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00005A9C File Offset: 0x00003C9C
	[PunRPC]
	public void RPCA_FallWithScreenShake(float seconds, float shake)
	{
		if (base.photonView.IsMine)
		{
			Debug.Log(string.Format("I fell for {0} seconds", seconds));
		}
		GamefeelHandler.instance.AddPerlinShake(shake, 0.4f, 15f);
		if (seconds > this.data.fallSeconds)
		{
			this.data.fallSeconds = seconds;
		}
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x00005AFC File Offset: 0x00003CFC
	[ConsoleCommand]
	public static void Revive()
	{
		Debug.Log(string.Format("Reviving, status: {0}, fullyPassedOut: {1}", Character.localCharacter.data.dead, Character.localCharacter.data.fullyPassedOut));
		if (Character.localCharacter.data.dead || Character.localCharacter.data.fullyPassedOut)
		{
			Character.localCharacter.view.RPC("RPCA_Revive", RpcTarget.All, new object[]
			{
				true
			});
		}
	}

	// Token: 0x060000AA RID: 170 RVA: 0x00005B88 File Offset: 0x00003D88
	[PunRPC]
	internal void RPCA_Revive(bool applyStatus)
	{
		Action action = this.reviveAction;
		if (action != null)
		{
			action();
		}
		this.data.dead = false;
		this.data.deathTimer = 0f;
		this.data.passedOut = false;
		this.data.fullyPassedOut = false;
		this.data.sinceGrounded = 0f;
		this.refs.afflictions.ClearAllStatus(true);
		this.refs.afflictions.RemoveAllThorns();
		this.refs.afflictions.ClearAllAfflictions();
		this.data.fallSeconds = 0f;
		if (applyStatus)
		{
			Character.ApplyPostReviveStatus(this.refs.afflictions);
		}
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00005C3E File Offset: 0x00003E3E
	public static void ApplyPostReviveStatus(CharacterAfflictions afflictions)
	{
		afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 0.05f, true, true, true);
		afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0.3f, true, true, true);
	}

	// Token: 0x060000AC RID: 172 RVA: 0x00005C60 File Offset: 0x00003E60
	[PunRPC]
	internal void RPCA_ReviveAtPosition(Vector3 position, bool applyStatus, int statueSegment)
	{
		this.refs.items.DropAllItems(true);
		this.RPCA_Revive(applyStatus);
		this.WarpPlayer(position, true);
		if (statueSegment > -1)
		{
			this.data.lastRevivedSegment = (Segment)statueSegment;
			if (this.IsLocal)
			{
				MountainProgressHandler.MarkBiomeSkipped(MapHandler.GetBiomeForSegment(statueSegment));
			}
		}
		this.refs.stats.justDied = false;
		this.refs.stats.justRevived = true;
		this.refs.stats.Record(true, position.y);
		Singleton<ReconnectHandler>.Instance.UpdateFromRevive(this, position);
	}

	// Token: 0x060000AD RID: 173 RVA: 0x00005CF6 File Offset: 0x00003EF6
	[PunRPC]
	public void WarpPlayerRPC(Vector3 position, bool poof)
	{
		this.WarpPlayer(position, poof);
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00005D00 File Offset: 0x00003F00
	public void PlayPoofVFX(Vector3 pos)
	{
		this.refs.poof.transform.position = pos;
		this.refs.poof.main.startColor = this.refs.customization.PlayerColor;
		this.refs.poof.Play();
		for (int i = 0; i < this.poofSFX.Length; i++)
		{
			this.poofSFX[i].Play(pos);
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x060000AF RID: 175 RVA: 0x00005D81 File Offset: 0x00003F81
	// (set) Token: 0x060000B0 RID: 176 RVA: 0x00005D89 File Offset: 0x00003F89
	public float timeLastWarped { get; private set; }

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060000B1 RID: 177 RVA: 0x00005D92 File Offset: 0x00003F92
	// (set) Token: 0x060000B2 RID: 178 RVA: 0x00005D9A File Offset: 0x00003F9A
	public bool warping { get; private set; }

	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060000B3 RID: 179 RVA: 0x00005DA4 File Offset: 0x00003FA4
	// (remove) Token: 0x060000B4 RID: 180 RVA: 0x00005DDC File Offset: 0x00003FDC
	public event Action<Character> WarpCompleted;

	// Token: 0x060000B5 RID: 181 RVA: 0x00005E11 File Offset: 0x00004011
	private void TryWarpAgain()
	{
		if (this.warping)
		{
			return;
		}
		this.WarpPlayer(this._lastWarpTarget, false);
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00005E2C File Offset: 0x0000402C
	internal void WarpPlayer(Vector3 position, bool poof)
	{
		Character.<>c__DisplayClass169_0 CS$<>8__locals1 = new Character.<>c__DisplayClass169_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.poof = poof;
		CS$<>8__locals1.position = position;
		if (this.warping)
		{
			Debug.LogError("WHOA! We started a new warp before the old one wrapped up. Stopping the previous warp routine first, but this is very likely to break something.");
			base.StopCoroutine(this._warpRoutine);
			this.warping = false;
		}
		Debug.Log(string.Format("Starting move {0} to position {1} from {2} via MovePlayer routine", this.characterName, CS$<>8__locals1.position, this.Center));
		this._warpRoutine = base.StartCoroutine(CS$<>8__locals1.<WarpPlayer>g__IMove|0());
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x00005EB8 File Offset: 0x000040B8
	internal void MoveBodypartTowardsPoint(BodypartType bodypart, Vector3 pos, float force, float clampDistance = 1f)
	{
		Bodypart bodypart2 = this.GetBodypart(bodypart);
		bodypart2.AddForce(Vector3.ClampMagnitude(pos - bodypart2.Rig.position, clampDistance) * force, ForceMode.Acceleration);
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x00005EF4 File Offset: 0x000040F4
	public static bool PlayerIsDeadOrDown()
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (character.data.dead || character.data.fullyPassedOut)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00005F60 File Offset: 0x00004160
	internal BodypartType GetPartType(Rigidbody rigidbody)
	{
		foreach (Bodypart bodypart in this.refs.ragdoll.partList)
		{
			if (bodypart.Rig == rigidbody)
			{
				return bodypart.partType;
			}
		}
		return (BodypartType)(-1);
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00005FD0 File Offset: 0x000041D0
	internal void LimitFalling()
	{
		this.data.sinceGrounded = Mathf.Min(this.data.sinceGrounded, 0.5f);
		this.data.sinceJump = Mathf.Min(this.data.sinceJump, 0.5f);
	}

	// Token: 0x060000BB RID: 187 RVA: 0x0000601D File Offset: 0x0000421D
	internal void AddIllegalStatus(string illegalStatus, float amount)
	{
		Action<string, float> action = this.illegalStatusAction;
		if (action == null)
		{
			return;
		}
		action(illegalStatus, amount);
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060000BC RID: 188 RVA: 0x00006031 File Offset: 0x00004231
	// (set) Token: 0x060000BD RID: 189 RVA: 0x00006039 File Offset: 0x00004239
	public bool infiniteStam { get; private set; }

	// Token: 0x060000BE RID: 190 RVA: 0x00006044 File Offset: 0x00004244
	[ConsoleCommand]
	public static void InfiniteStamina()
	{
		if (!Character.localCharacter.infiniteStam)
		{
			Character.localCharacter.data.currentStamina = 1f;
		}
		Character.localCharacter.infiniteStam = !Character.localCharacter.infiniteStam;
		Debug.LogError(string.Format("Infinite Stamina: {0}", Character.localCharacter.infiniteStam));
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060000BF RID: 191 RVA: 0x000060A6 File Offset: 0x000042A6
	// (set) Token: 0x060000C0 RID: 192 RVA: 0x000060AE File Offset: 0x000042AE
	public bool statusesLocked { get; private set; }

	// Token: 0x060000C1 RID: 193 RVA: 0x000060B7 File Offset: 0x000042B7
	[ConsoleCommand]
	public static void LockStatuses()
	{
		Character.localCharacter.statusesLocked = !Character.localCharacter.statusesLocked;
		Debug.LogError(string.Format("Statuses Locked: {0}", Character.localCharacter.statusesLocked));
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x000060EE File Offset: 0x000042EE
	private void OnGetMic(float db)
	{
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x000060F0 File Offset: 0x000042F0
	internal void StartPassedOutOnTheBeach()
	{
		Debug.Log("Starting passed out!");
		this.data.passedOutOnTheBeach = 3f;
		this.Fall(7f, 0f);
		this.TestSpawnChallengeItems();
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x00006122 File Offset: 0x00004322
	private void TestSpawnChallengeItems()
	{
		if (!this.IsLocal)
		{
			return;
		}
		if (RunSettings.GetValue(RunSettings.SETTINGTYPE.GrappleMode, false) == 1)
		{
			base.StartCoroutine(this.<TestSpawnChallengeItems>g__SpawnChallengeItem|188_0(RunSettings.GRAPPLE_MODE_ITEM));
		}
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00006150 File Offset: 0x00004350
	public void AddForceToBodyPart(Rigidbody rig, Vector3 partForce, Vector3 wholeBodyForce)
	{
		Bodypart component = rig.GetComponent<Bodypart>();
		if (component == null)
		{
			return;
		}
		this.view.RPC("RPCA_AddForceToBodyPart", RpcTarget.All, new object[]
		{
			component.partType,
			partForce,
			wholeBodyForce
		});
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x000061A5 File Offset: 0x000043A5
	[PunRPC]
	public void RPCA_AddForceToBodyPart(BodypartType bodypartType, Vector3 force, Vector3 wholeBodyForce)
	{
		this.GetBodypart(bodypartType).AddForce(force, ForceMode.Acceleration);
		this.AddForce(wholeBodyForce, 1f, 1f);
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x000061C6 File Offset: 0x000043C6
	internal void ClampSinceGrounded(float maxSinceGrounded)
	{
		this.data.sinceGrounded = Mathf.Clamp(this.data.sinceGrounded, 0f, maxSinceGrounded);
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x000061E9 File Offset: 0x000043E9
	internal void ClampRagdollControl(float maxRagdollControlClamp)
	{
		this.data.ragdollControlClamp = maxRagdollControlClamp;
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x000061F8 File Offset: 0x000043F8
	private void HandleStickUpdate()
	{
		this.data.sinceUnstuck += Time.deltaTime;
		if (this.stickParts.Count == 0)
		{
			return;
		}
		bool flag = true;
		foreach (StickPart stickPart in this.stickParts)
		{
			if (stickPart.joint)
			{
				flag = false;
			}
			stickPart.sinceStick += Time.deltaTime;
			if (this.view.IsMine && stickPart.sinceStick > 4f && stickPart.joint)
			{
				this.view.RPC("RPCA_ClearJoint", RpcTarget.All, new object[]
				{
					stickPart.bodypart.partType
				});
			}
		}
		if (flag && this.view.IsMine)
		{
			this.view.RPC("RPCA_ClearStickData", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.ClampSinceGrounded(0.5f);
	}

	// Token: 0x060000CA RID: 202 RVA: 0x00006314 File Offset: 0x00004514
	[PunRPC]
	public void RPCA_ClearStickData()
	{
		this.stickParts.Clear();
		this.data.sinceUnstuck = 0f;
	}

	// Token: 0x060000CB RID: 203 RVA: 0x00006334 File Offset: 0x00004534
	[PunRPC]
	public void RPCA_ClearJoint(BodypartType bodypartType)
	{
		foreach (StickPart stickPart in this.stickParts)
		{
			if (bodypartType == stickPart.bodypart.partType)
			{
				Object.Destroy(stickPart.joint);
			}
		}
	}

	// Token: 0x060000CC RID: 204 RVA: 0x0000639C File Offset: 0x0000459C
	private void JumpStickEffect()
	{
		foreach (StickPart stickPart in this.stickParts)
		{
			stickPart.sinceStick += Random.Range(0.5f, 1.5f);
		}
	}

	// Token: 0x060000CD RID: 205 RVA: 0x00006404 File Offset: 0x00004604
	public bool IsStuck()
	{
		return this.stickParts.Count > 0;
	}

	// Token: 0x060000CE RID: 206 RVA: 0x00006418 File Offset: 0x00004618
	internal bool TryStickBodypart(Bodypart bodypart, Vector3 stickAnchor, CharacterAfflictions.STATUSTYPE statusType, float statusAmount)
	{
		if (this.data.sinceUnstuck < 3f)
		{
			return false;
		}
		if (this.StickPartExists(bodypart))
		{
			return false;
		}
		this.view.RPC("RPCA_Stick", RpcTarget.All, new object[]
		{
			bodypart.partType,
			bodypart.transform.position,
			stickAnchor,
			statusType,
			statusAmount
		});
		return true;
	}

	// Token: 0x060000CF RID: 207 RVA: 0x0000649C File Offset: 0x0000469C
	[PunRPC]
	private void RPCA_Stick(BodypartType bodypartType, Vector3 pos, Vector3 stickAnchor, CharacterAfflictions.STATUSTYPE statusType, float statusAmount)
	{
		Bodypart bodypart = this.GetBodypart(bodypartType);
		bodypart.Rig.transform.position = pos;
		StickPart stickPart = new StickPart();
		stickPart.bodypart = bodypart;
		stickPart.sinceStick = 0f;
		ConfigurableJoint configurableJoint = bodypart.Rig.gameObject.AddComponent<ConfigurableJoint>();
		stickPart.joint = configurableJoint;
		this.stickParts.Add(stickPart);
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Locked;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Free;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Free;
		configurableJoint.anchor = bodypart.transform.InverseTransformPoint(stickAnchor);
		if (statusAmount > 0f)
		{
			this.refs.afflictions.AddStatus(statusType, statusAmount, true, true, true);
		}
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x0000655B File Offset: 0x0000475B
	internal void UnStick()
	{
		this.refs.view.RPC("RPCA_Unstick", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x00006578 File Offset: 0x00004778
	[PunRPC]
	public void RPCA_Unstick()
	{
		for (int i = this.stickParts.Count - 1; i >= 0; i--)
		{
			Object.Destroy(this.stickParts[i].joint);
		}
		this.RPCA_ClearStickData();
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x000065BC File Offset: 0x000047BC
	private bool StickPartExists(Bodypart bodypart)
	{
		foreach (StickPart stickPart in this.stickParts)
		{
			if (bodypart == stickPart.bodypart)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00006620 File Offset: 0x00004820
	internal void AddForce(object value)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00006627 File Offset: 0x00004827
	private void StoppedForcedRagdolling()
	{
		this.data.launchedByCannon = false;
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00006638 File Offset: 0x00004838
	[ConsoleCommand]
	public static void RagdollExorcism()
	{
		CharacterSyncer.SnapToRemoteValues();
		foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
		{
			if (character && character.refs.ragdoll)
			{
				if (character.data.avarageVelocity.sqrMagnitude < 5f)
				{
					Debug.Log("Probably shouldn't have touched " + character.characterName + " because their average rb velocity is only " + string.Format("{0}", character.data.avarageVelocity.magnitude));
				}
				if (character.data.currentItem && character.refs.items.currentSelectedSlot.IsSome)
				{
					Debug.Log("Forcing " + character.characterName + " to drop their current item as we tame all the ragdolls.");
					Item currentItem = character.data.currentItem;
					byte value = character.refs.items.currentSelectedSlot.Value;
					character.refs.items.photonView.RPC("DropItemRpc", RpcTarget.All, new object[]
					{
						0f,
						value,
						currentItem.transform.position,
						Vector3.zero,
						currentItem.transform.rotation,
						character.player.GetItemSlot(value).data,
						false
					});
				}
				FixedJoint[] componentsInChildren = character.refs.ragdoll.GetComponentsInChildren<FixedJoint>();
				if (componentsInChildren.Length != 0)
				{
					Debug.Log(string.Format("Found {0} on {1} but there should be none.", componentsInChildren.Length, character.characterName));
				}
				FixedJoint[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					Object.Destroy(array[i]);
				}
				if (!character.IsLocal)
				{
					Vector3 lastPosition = character.GetComponent<CharacterSyncer>().LastPosition;
					character.WarpPlayer(lastPosition, true);
				}
				character.StartCoroutine(Character.<RagdollExorcism>g__ForceRagdollToChill|206_0(character));
			}
		}
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x0000687C File Offset: 0x00004A7C
	[ConsoleCommand]
	public static void WarpToSpawn()
	{
		Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[]
		{
			SpawnPoint.allSpawnPoints[0].transform.position,
			false
		});
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x0000690C File Offset: 0x00004B0C
	[CompilerGenerated]
	private void <RPCA_PassOut>g__PassOutDone|94_0()
	{
		this.data.fullyPassedOut = true;
		this.refs.items.DropAllItems(false);
	}

	// Token: 0x060000DA RID: 218 RVA: 0x0000692B File Offset: 0x00004B2B
	[CompilerGenerated]
	private IEnumerator <TestSpawnChallengeItems>g__SpawnChallengeItem|188_0(string itemName)
	{
		this.input.itemSwitchBlocked = true;
		while (this.data.passedOutOnTheBeach > 0f || this.warping)
		{
			yield return null;
		}
		float timeStartedWaiting = Time.realtimeSinceStartup;
		while (!this.data.isGrounded || Time.realtimeSinceStartup - timeStartedWaiting < 3f)
		{
			yield return null;
		}
		this.refs.items.SpawnItemInHand(itemName);
		while (this.data.currentItem == null)
		{
			yield return null;
		}
		this.input.itemSwitchBlocked = false;
		yield break;
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00006941 File Offset: 0x00004B41
	[CompilerGenerated]
	internal static IEnumerator <RagdollExorcism>g__ForceRagdollToChill|206_0(Character cursedCharacter)
	{
		while (cursedCharacter.warping)
		{
			yield return new WaitForFixedUpdate();
		}
		string characterName = cursedCharacter.characterName;
		Debug.Log("Attempting to damp " + characterName + "'s ragdoll to death.");
		float timeStarted = Time.realtimeSinceStartup;
		CharacterRagdoll ragdoll = cursedCharacter.refs.ragdoll;
		while (Time.realtimeSinceStartup - timeStarted < 3f)
		{
			cursedCharacter.refs.ragdoll.SnapToAnimation();
			ragdoll.SetRigDamping(20f);
			yield return new WaitForFixedUpdate();
		}
		ragdoll.SetRigDamping(0f);
		CharacterSyncer.SnapToRemoteValues();
		cursedCharacter.refs.ragdoll.SnapToAnimation();
		yield break;
	}

	// Token: 0x0400002E RID: 46
	public bool isBot;

	// Token: 0x0400002F RID: 47
	public bool isZombie;

	// Token: 0x04000030 RID: 48
	public bool isScoutmaster;

	// Token: 0x04000031 RID: 49
	public static Character localCharacter;

	// Token: 0x04000032 RID: 50
	public CharacterInput input;

	// Token: 0x04000033 RID: 51
	public CharacterData data;

	// Token: 0x04000034 RID: 52
	public Character.CharacterRefs refs;

	// Token: 0x04000036 RID: 54
	private PhotonView view;

	// Token: 0x04000037 RID: 55
	public static List<Character> AllCharacters = new List<Character>();

	// Token: 0x04000038 RID: 56
	public static List<Character> AllBotCharacters = new List<Character>();

	// Token: 0x04000039 RID: 57
	private Vector3 smoothedCamPos;

	// Token: 0x0400003A RID: 58
	private bool _refsInitialized;

	// Token: 0x0400003C RID: 60
	private bool started;

	// Token: 0x0400003D RID: 61
	private float passOutFailsafeTick;

	// Token: 0x0400003E RID: 62
	private float lastZombified;

	// Token: 0x0400003F RID: 63
	public SFX_Instance[] poofSFX;

	// Token: 0x04000040 RID: 64
	private static bool forceWin;

	// Token: 0x04000041 RID: 65
	private bool UnPassOutCalled;

	// Token: 0x04000042 RID: 66
	public Action UnPassOutAction;

	// Token: 0x04000043 RID: 67
	private bool unPassOutCalled;

	// Token: 0x04000044 RID: 68
	public Action<float> landAction;

	// Token: 0x04000045 RID: 69
	public Action startJumpAction;

	// Token: 0x04000046 RID: 70
	public Action jumpAction;

	// Token: 0x04000047 RID: 71
	internal Action startClimbAction;

	// Token: 0x04000048 RID: 72
	public Action<Vector3, float> dragTowardsAction;

	// Token: 0x04000049 RID: 73
	public Action reviveAction;

	// Token: 0x0400004B RID: 75
	private Vector3 _lastWarpTarget;

	// Token: 0x0400004E RID: 78
	private static WaitForSeconds warpWait = new WaitForSeconds(0.5f);

	// Token: 0x0400004F RID: 79
	private static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

	// Token: 0x04000050 RID: 80
	private Coroutine _warpRoutine;

	// Token: 0x04000051 RID: 81
	public Action<string, float> illegalStatusAction;

	// Token: 0x04000054 RID: 84
	private List<StickPart> stickParts = new List<StickPart>();

	// Token: 0x0200040D RID: 1037
	[Serializable]
	public class CharacterRefs
	{
		// Token: 0x040017B1 RID: 6065
		public Transform carryPosRef;

		// Token: 0x040017B2 RID: 6066
		public CharacterRopeHandling ropeHandling;

		// Token: 0x040017B3 RID: 6067
		public CharacterClimbing climbing;

		// Token: 0x040017B4 RID: 6068
		public CharacterMovement movement;

		// Token: 0x040017B5 RID: 6069
		public CharacterRagdoll ragdoll;

		// Token: 0x040017B6 RID: 6070
		public CharacterBalloons balloons;

		// Token: 0x040017B7 RID: 6071
		public CharacterInteractible interactible;

		// Token: 0x040017B8 RID: 6072
		public RigCreator rigCreator;

		// Token: 0x040017B9 RID: 6073
		public Bodypart head;

		// Token: 0x040017BA RID: 6074
		public Bodypart hip;

		// Token: 0x040017BB RID: 6075
		public CharacterAnimations animations;

		// Token: 0x040017BC RID: 6076
		public Animator animator;

		// Token: 0x040017BD RID: 6077
		public RigBuilder ikRigBuilder;

		// Token: 0x040017BE RID: 6078
		public Rig ikRig;

		// Token: 0x040017BF RID: 6079
		public TwoBoneIKConstraint ikLeft;

		// Token: 0x040017C0 RID: 6080
		public TwoBoneIKConstraint ikRight;

		// Token: 0x040017C1 RID: 6081
		public CharacterItems items;

		// Token: 0x040017C2 RID: 6082
		public AnimatedVariables animatedVariables;

		// Token: 0x040017C3 RID: 6083
		public CharacterAfflictions afflictions;

		// Token: 0x040017C4 RID: 6084
		public BadgeUnlocker badgeUnlocker;

		// Token: 0x040017C5 RID: 6085
		public PhotonView view;

		// Token: 0x040017C6 RID: 6086
		public CharacterHeatEmission heatEmission;

		// Token: 0x040017C7 RID: 6087
		public CharacterVineClimbing vineClimbing;

		// Token: 0x040017C8 RID: 6088
		public SkinnedMeshRenderer mainRenderer;

		// Token: 0x040017C9 RID: 6089
		public CharacterCarrying carriying;

		// Token: 0x040017CA RID: 6090
		public CharacterCustomization customization;

		// Token: 0x040017CB RID: 6091
		public CharacterStats stats;

		// Token: 0x040017CC RID: 6092
		public CharacterGrabbing grabbing;

		// Token: 0x040017CD RID: 6093
		public HideTheBody hideTheBody;

		// Token: 0x040017CE RID: 6094
		public ParticleSystem poof;

		// Token: 0x040017CF RID: 6095
		public Transform IKHandTargetLeft;

		// Token: 0x040017D0 RID: 6096
		public Transform IKHandTargetRight;

		// Token: 0x040017D1 RID: 6097
		public Transform helperObjects;

		// Token: 0x040017D2 RID: 6098
		public Transform animationHeadTransform;

		// Token: 0x040017D3 RID: 6099
		public Transform animationHipTransform;

		// Token: 0x040017D4 RID: 6100
		public Transform animationItemTransform;

		// Token: 0x040017D5 RID: 6101
		public Transform animationLookTransform;

		// Token: 0x040017D6 RID: 6102
		public Transform animationPositionTransform;

		// Token: 0x040017D7 RID: 6103
		public Transform backpackTransform;
	}
}
