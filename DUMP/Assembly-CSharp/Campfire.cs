using System;
using System.Collections.Generic;
using Peak;
using Peak.Afflictions;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x020000A8 RID: 168
public class Campfire : MonoBehaviour, IInteractibleConstant, IInteractible
{
	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000669 RID: 1641 RVA: 0x00025078 File Offset: 0x00023278
	public bool CanBurnOut
	{
		get
		{
			return !this.EveryoneInRange(this.moraleBoostRadius);
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x0600066A RID: 1642 RVA: 0x00025089 File Offset: 0x00023289
	public bool Lit
	{
		get
		{
			return this.state == Campfire.FireState.Lit;
		}
	}

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x0600066B RID: 1643 RVA: 0x00025094 File Offset: 0x00023294
	public float LitProgress
	{
		get
		{
			return (this.beenBurningFor / this.burnsFor).Clamp01();
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x000250A8 File Offset: 0x000232A8
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this.mainRenderer = base.GetComponentInChildren<Renderer>();
		this.startRot = this.fireParticles.emission.rateOverTime.constant;
		this.startSize = new Vector2(this.fireParticles.main.startSize.constantMin, this.fireParticles.main.startSize.constantMax);
		this.UpdateLit();
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00025138 File Offset: 0x00023338
	private void Update()
	{
		if (this.Lit)
		{
			if (this.CanBurnOut)
			{
				this.beenBurningFor += Time.deltaTime;
			}
			this.ApplyCampfireProtection();
			ParticleSystem.MainModule main = this.fireParticles.main;
			ParticleSystem.MinMaxCurve minMaxCurve = main.startSize;
			minMaxCurve.constantMin = Mathf.Lerp(this.startSize.x, this.endSize.x, this.LitProgress);
			minMaxCurve.constantMax = Mathf.Lerp(this.startSize.y, this.endSize.y, this.LitProgress);
			main.startSize = minMaxCurve;
			ParticleSystem.EmissionModule emission = this.fireParticles.emission;
			ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
			rateOverTime.constant = Mathf.Lerp(this.startRot, this.endRot, this.LitProgress);
			emission.rateOverTime = rateOverTime;
			if (!this.fireHasStarted)
			{
				if (MoraleBoost.SpawnMoraleBoost(base.transform.position, this.moraleBoostRadius, this.moraleBoostBaseline, this.moraleBoostPerAdditionalScout, false, 2))
				{
					for (int i = 0; i < this.moraleBoost.Length; i++)
					{
						this.moraleBoost[i].Play(base.transform.position);
					}
					Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.MoraleBoosts, 1);
				}
				if (Character.localCharacter != null && Vector3.Distance(base.transform.position, Character.localCharacter.Center) <= this.moraleBoostRadius)
				{
					Character.localCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Injury, -0.2f, false);
				}
				for (int j = 0; j < this.fireStart.Length; j++)
				{
					this.fireStart[j].Play(base.transform.position);
				}
				this.fireHasStarted = true;
			}
			if (this.view.IsMine && this.beenBurningFor > this.burnsFor)
			{
				this.view.RPC("Extinguish_Rpc", RpcTarget.AllBuffered, Array.Empty<object>());
			}
		}
		else if (this.fireHasStarted)
		{
			for (int k = 0; k < this.extinguish.Length; k++)
			{
				this.extinguish[k].Play(base.transform.position);
			}
			this.fireHasStarted = false;
		}
		this.StupidTextUpdate();
		this.UpdateAudioLoop();
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00025384 File Offset: 0x00023584
	private void ApplyCampfireProtection()
	{
		if (Singleton<OrbFogHandler>.Instance)
		{
			Singleton<OrbFogHandler>.Instance.PlayersAreResting = this.EveryoneInRange();
		}
		if (!this.PlayerCharactersInRadius(this.moraleBoostRadius).Contains(Character.localCharacter))
		{
			return;
		}
		CharacterAfflictions afflictions = Character.localCharacter.refs.afflictions;
		if (afflictions.canGetHungry)
		{
			afflictions.AddAffliction(Campfire.s_CampfireBuff, false);
		}
		if (Time.time - this._timebuffLastApplied < 2f)
		{
			return;
		}
		this._timebuffLastApplied = Time.time;
		afflictions.AddAffliction(Campfire.s_CampfireBuff, false);
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x00025415 File Offset: 0x00023615
	private void StupidTextUpdate()
	{
		if (GUIManager.instance.currentInteractable == this)
		{
			GUIManager.instance.RefreshInteractablePrompt();
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x0002542E File Offset: 0x0002362E
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.moraleBoostRadius);
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00025450 File Offset: 0x00023650
	public Vector3 Center()
	{
		return this.mainRenderer.bounds.center;
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00025470 File Offset: 0x00023670
	public string GetInteractionText()
	{
		if (!this.Lit)
		{
			string result;
			if (!this.EveryoneInRange(out result, this.moraleBoostRadius))
			{
				return result;
			}
			return LocalizedText.GetText("LIGHT", true);
		}
		else
		{
			if (this.Lit)
			{
				return LocalizedText.GetText("COOK", true);
			}
			return "";
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x000254BC File Offset: 0x000236BC
	public string GetName()
	{
		if (!string.IsNullOrEmpty(this.nameOverride))
		{
			return LocalizedText.GetText(this.nameOverride, true);
		}
		return LocalizedText.GetText("CAMPFIRE", true);
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x000254E3 File Offset: 0x000236E3
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x000254EB File Offset: 0x000236EB
	public void HoverEnter()
	{
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x000254ED File Offset: 0x000236ED
	public void HoverExit()
	{
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x000254F0 File Offset: 0x000236F0
	public void Interact(Character interactor)
	{
		if (this.Lit && interactor.data.currentItem != null && interactor.data.currentItem.cooking.canBeCooked)
		{
			this.currentlyCookingItem = interactor.data.currentItem;
			interactor.data.currentItem.GetComponent<ItemCooking>().StartCookingVisuals();
		}
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00025558 File Offset: 0x00023758
	public void Interact_CastFinished(Character interactor)
	{
		if (this.Lit)
		{
			if (this.currentlyCookingItem)
			{
				if (this.currentlyCookingItem.GetData<IntItemData>(DataEntryKey.CookedAmount).Value == 0)
				{
					Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.MealsCooked, 1);
				}
				this.currentlyCookingItem.GetComponent<ItemCooking>().FinishCooking();
				return;
			}
		}
		else if (this.EveryoneInRange(this.moraleBoostRadius))
		{
			this.view.RPC("Light_Rpc", RpcTarget.All, new object[]
			{
				true
			});
		}
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x000255D9 File Offset: 0x000237D9
	public void CancelCast(Character interactor)
	{
		if (this.currentlyCookingItem)
		{
			this.currentlyCookingItem.GetComponent<ItemCooking>().CancelCookingVisuals();
		}
		this.currentlyCookingItem = null;
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x000255FF File Offset: 0x000237FF
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x0600067B RID: 1659 RVA: 0x00025601 File Offset: 0x00023801
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00025604 File Offset: 0x00023804
	public bool IsInteractible(Character interactor)
	{
		return this.state == Campfire.FireState.Off || (this.state != Campfire.FireState.Spent && interactor.data.currentItem != null && interactor.data.currentItem.cooking.canBeCooked);
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00025650 File Offset: 0x00023850
	public List<Character> PlayerCharactersInRadius(float radius)
	{
		this._charactersInRadius.Clear();
		Vector3 position = base.transform.position;
		foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
		{
			if (!character.data.dead && Vector3.Distance(position, character.Center) <= radius)
			{
				this._charactersInRadius.Add(character);
			}
		}
		return this._charactersInRadius;
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x000256E0 File Offset: 0x000238E0
	public bool EveryoneInRange()
	{
		return this.EveryoneInRange(this.moraleBoostRadius);
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x000256F0 File Offset: 0x000238F0
	public bool EveryoneInRange(float range)
	{
		List<Character> list = this.PlayerCharactersInRadius(range);
		foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
		{
			if (!character.data.dead && !list.Contains(character))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00025760 File Offset: 0x00023960
	public bool EveryoneInRange(out string printout, float range)
	{
		bool flag = true;
		printout = "";
		List<Character> list = this.PlayerCharactersInRadius(range);
		Vector3 position = base.transform.position;
		foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
		{
			if (!character.data.dead && !list.Contains(character))
			{
				float num = Vector3.Distance(position, character.Center);
				flag = false;
				printout += string.Format("\n{0} {1}m", character.photonView.Owner.NickName, Mathf.RoundToInt(num * CharacterStats.unitsToMeters));
			}
		}
		if (!flag)
		{
			printout = LocalizedText.GetText("CANTLIGHT", true) + "\n" + printout;
		}
		return flag;
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x00025848 File Offset: 0x00023A48
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.state == Campfire.FireState.Off || (this.state != Campfire.FireState.Spent && interactor.data.currentItem != null && interactor.data.currentItem.cooking.canBeCooked);
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x00025894 File Offset: 0x00023A94
	public float GetInteractTime(Character interactor)
	{
		return this.cookTime;
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x0002589C File Offset: 0x00023A9C
	public void DebugLight()
	{
		this.view.RPC("Light_Rpc", RpcTarget.All, new object[]
		{
			true
		});
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x000258C0 File Offset: 0x00023AC0
	private void UpdateAudioLoop()
	{
		if (this.loop)
		{
			float b = this.Lit ? 0.5f : 0f;
			this.loop.volume = Mathf.Lerp(this.loop.volume, b, Time.deltaTime * 5f);
		}
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x00025918 File Offset: 0x00023B18
	private void HideLogs()
	{
		foreach (object obj in this.logRoot)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x00025974 File Offset: 0x00023B74
	public void LightWithoutReveal()
	{
		this.view.RPC("Light_Rpc", RpcTarget.AllBuffered, new object[]
		{
			false
		});
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x00025998 File Offset: 0x00023B98
	[PunRPC]
	private void Light_Rpc(bool updateSegment)
	{
		this.state = Campfire.FireState.Lit;
		Shader.SetGlobalFloat("FakeMountainEnabled", (float)(this.disableFogFakeMountain ? 0 : 1));
		this.UpdateLit();
		this.smokeParticlesOff.Stop();
		this.smokeParticlesLit.Play();
		GUIManager.instance.RefreshInteractablePrompt();
		if (this.IsMiniRunEndingCampfire())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				Character.localCharacter.EndGame();
			}
			return;
		}
		if (updateSegment && Singleton<MapHandler>.Instance)
		{
			Singleton<MapHandler>.Instance.GoToSegment(this.advanceToSegment);
		}
		if (updateSegment)
		{
			GUIManager.instance.Quicksave();
			Quicksave.SaveNow();
		}
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00025A34 File Offset: 0x00023C34
	private bool IsMiniRunEndingCampfire()
	{
		if (!RunSettings.isMiniRun)
		{
			return false;
		}
		int value = RunSettings.GetValue(RunSettings.SETTINGTYPE.MiniRunBiome, false);
		if (value == 0)
		{
			return this.advanceToSegment == Segment.Tropics;
		}
		if (value == 1)
		{
			return this.advanceToSegment == Segment.Alpine;
		}
		return value == 2 && this.advanceToSegment == Segment.Caldera;
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x00025A80 File Offset: 0x00023C80
	[PunRPC]
	private void Extinguish_Rpc()
	{
		this.beenBurningFor = 0f;
		this.state = Campfire.FireState.Spent;
		this.HideLogs();
		this.UpdateLit();
		this.smokeParticlesOff.Stop();
		this.smokeParticlesLit.Stop();
		this.fireParticles.Stop();
		if (Singleton<OrbFogHandler>.Instance)
		{
			Singleton<OrbFogHandler>.Instance.PlayersAreResting = false;
		}
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x00025AE4 File Offset: 0x00023CE4
	private void UpdateLit()
	{
		if (this.enableWhenLit)
		{
			this.enableWhenLit.SetActive(this.state == Campfire.FireState.Lit);
		}
		if (this.disableWhenLit)
		{
			this.disableWhenLit.SetActive(this.state == Campfire.FireState.Off || this.state == Campfire.FireState.Spent);
		}
	}

	// Token: 0x04000665 RID: 1637
	private const float k_BuffTick = 2f;

	// Token: 0x04000666 RID: 1638
	private static Affliction_NoHunger s_CampfireBuff = new Affliction_NoHunger
	{
		totalTime = 3f
	};

	// Token: 0x04000667 RID: 1639
	public Segment advanceToSegment;

	// Token: 0x04000668 RID: 1640
	public Campfire.FireState state;

	// Token: 0x04000669 RID: 1641
	public GameObject enableWhenLit;

	// Token: 0x0400066A RID: 1642
	public GameObject disableWhenLit;

	// Token: 0x0400066B RID: 1643
	[FormerlySerializedAs("litTime")]
	public float burnsFor = 180f;

	// Token: 0x0400066C RID: 1644
	public float cookTime = 5f;

	// Token: 0x0400066D RID: 1645
	public Transform logRoot;

	// Token: 0x0400066E RID: 1646
	public Vector2 endSize = new Vector2(0.1f, 0.2f);

	// Token: 0x0400066F RID: 1647
	public float endRot = 3f;

	// Token: 0x04000670 RID: 1648
	[FormerlySerializedAs("litTimeElapsed")]
	public float beenBurningFor;

	// Token: 0x04000671 RID: 1649
	public ParticleSystem fireParticles;

	// Token: 0x04000672 RID: 1650
	public ParticleSystem smokeParticlesOff;

	// Token: 0x04000673 RID: 1651
	public ParticleSystem smokeParticlesLit;

	// Token: 0x04000674 RID: 1652
	public float moraleBoostRadius;

	// Token: 0x04000675 RID: 1653
	public float moraleBoostBaseline;

	// Token: 0x04000676 RID: 1654
	public float moraleBoostPerAdditionalScout;

	// Token: 0x04000677 RID: 1655
	public float injuryReduction = 0.2f;

	// Token: 0x04000678 RID: 1656
	public SFX_Instance[] fireStart;

	// Token: 0x04000679 RID: 1657
	public SFX_Instance[] extinguish;

	// Token: 0x0400067A RID: 1658
	public SFX_Instance[] moraleBoost;

	// Token: 0x0400067B RID: 1659
	public AudioSource loop;

	// Token: 0x0400067C RID: 1660
	public string nameOverride;

	// Token: 0x0400067D RID: 1661
	private Item currentlyCookingItem;

	// Token: 0x0400067E RID: 1662
	private Renderer mainRenderer;

	// Token: 0x0400067F RID: 1663
	private float startRot;

	// Token: 0x04000680 RID: 1664
	private Vector2 startSize;

	// Token: 0x04000681 RID: 1665
	private bool fireHasStarted;

	// Token: 0x04000682 RID: 1666
	private PhotonView view;

	// Token: 0x04000683 RID: 1667
	private float _timebuffLastApplied;

	// Token: 0x04000684 RID: 1668
	public bool disableFogFakeMountain;

	// Token: 0x04000685 RID: 1669
	private List<Character> _charactersInRadius = new List<Character>();

	// Token: 0x02000449 RID: 1097
	public enum FireState
	{
		// Token: 0x040018DA RID: 6362
		Off,
		// Token: 0x040018DB RID: 6363
		Lit,
		// Token: 0x040018DC RID: 6364
		Spent
	}
}
