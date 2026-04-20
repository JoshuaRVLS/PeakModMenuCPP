using System;
using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x020000B7 RID: 183
public class WindChillZone : MonoBehaviour
{
	// Token: 0x060006E1 RID: 1761 RVA: 0x000276F0 File Offset: 0x000258F0
	private void Awake()
	{
		WindChillZone.instance = this;
		this.windZoneBounds.center = base.transform.position;
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00027710 File Offset: 0x00025910
	private void OnDrawGizmosSelected()
	{
		this.windZoneBounds.center = base.transform.position;
		Gizmos.color = this.gizmoColor;
		Gizmos.DrawCube(this.windZoneBounds.center, this.windZoneBounds.extents * 2f);
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00027763 File Offset: 0x00025963
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		if (RunSettings.IsCustomRun && RunSettings.GetValue(this.disableIfSettingDisabled, false) == 0)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00027794 File Offset: 0x00025994
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.HandleTime();
		if (this.windActive)
		{
			this.hasBeenActiveFor += Time.deltaTime;
			this.StormProgress = Mathf.Clamp01(1f - this.untilSwitch / this.timeUntilNextWind);
			this.timeUntilStorm = 0f;
			this.windIntensity = this.windIntensityCurve.Evaluate(this.StormProgress);
		}
		else
		{
			this.timeUntilStorm = Mathf.Clamp01(1f - this.untilSwitch / this.timeUntilNextWind);
			this.StormProgress = 0f;
			this.hasBeenActiveFor = 0f;
		}
		this.localCharacterInsideBounds = this.windZoneBounds.Contains(Character.localCharacter.Center);
		this.observedCharacterInsideBounds = (Character.observedCharacter != null && this.windZoneBounds.Contains(Character.observedCharacter.Center));
		if (this.localCharacterInsideBounds && this.windActive)
		{
			this.ApplyStatus(Character.localCharacter);
			return;
		}
		this.windPlayerFactor = 0f;
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x000278B4 File Offset: 0x00025AB4
	private void HandleTime()
	{
		this.untilSwitch -= Time.deltaTime;
		Vector3 zero = Vector3.zero;
		if (this.untilSwitch < 0f && PhotonNetwork.IsMasterClient)
		{
			this.untilSwitch = this.GetNextWindTime(!this.windActive);
			this.view.RPC("RPCA_ToggleWind", RpcTarget.All, new object[]
			{
				!this.windActive,
				this.RandomWindDirection(),
				this.untilSwitch
			});
		}
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x00027948 File Offset: 0x00025B48
	private void FixedUpdate()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.currentForceMult = Mathf.MoveTowards(this.currentForceMult, (float)(this.windActive ? 1 : 0), Time.fixedDeltaTime);
		if (this.timeSpentWaiting >= this.delayBeforeForce)
		{
			if (this.localCharacterInsideBounds && this.windActive)
			{
				this.AddWindForceToCharacter(Character.localCharacter, this.currentForceMult);
			}
			foreach (Character character in Character.AllBotCharacters)
			{
				if (this.windZoneBounds.Contains(character.Center))
				{
					this.AddWindForceToCharacter(character, 0.6f * this.currentForceMult);
				}
			}
		}
		if (this.windMovesItems)
		{
			for (int i = 0; i < Item.ALL_ACTIVE_ITEMS.Count; i++)
			{
				Item item = Item.ALL_ACTIVE_ITEMS[i];
				if (item.UnityObjectExists<Item>() && item.itemState == ItemState.Ground && this.windZoneBounds.Contains(item.Center()))
				{
					this.AddWindForceToItem(item);
				}
			}
		}
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00027A6C File Offset: 0x00025C6C
	private void ApplyStatus(Character character)
	{
		this.windPlayerFactor = WindChillZone.GetWindIntensityAtPoint(character.Center, this.lightVolumeSampleThreshold_lower, this.lightVolumeSampleThreshold_margin);
		float climbingStamMinimumMultiplier = Mathf.Max(this.grabStaminaMultiplierDuringWind * this.windPlayerFactor * this.currentForceMult, 1f);
		character.refs.climbing.climbingStamMinimumMultiplier = climbingStamMinimumMultiplier;
		if (this.statusApplicationPerSecond > 0f)
		{
			character.refs.afflictions.AddStatus(this.statusType, this.windPlayerFactor * this.statusApplicationPerSecond * Time.deltaTime * Mathf.Clamp01(this.hasBeenActiveFor * 0.2f), false, true, true);
		}
		if (this.setSlippy)
		{
			character.data.slippy = Mathf.Clamp01(Mathf.Max(character.data.slippy, this.windPlayerFactor * 10f));
		}
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x00027B48 File Offset: 0x00025D48
	private void AddWindForceToCharacter(Character character, float mult = 1f)
	{
		if (!character.photonView.IsMine)
		{
			return;
		}
		if (character.data.currentClimbHandle != null)
		{
			return;
		}
		float num = this.useIntensityCurve ? (Mathf.Clamp01(this.windIntensity - 0.5f) * 2f) : 1f;
		bool flag = false;
		Parasol parasol;
		if (character.data.currentItem && character.data.currentItem.TryGetComponent<Parasol>(out parasol) && parasol.isOpen)
		{
			num *= 10f;
			flag = true;
		}
		if (character.refs.balloons.currentBalloonCount > 0)
		{
			int num2 = 2;
			if (flag)
			{
				num2 = 0;
			}
			num *= (float)(num2 + character.refs.balloons.currentBalloonCount);
		}
		Affliction affliction;
		if (character.refs.afflictions.HasAfflictionType(Affliction.AfflictionType.LowGravity, out affliction))
		{
			num = 0f;
		}
		if (this.useRaycast && Physics.Raycast(character.Center, -this.currentWindDirection, out this.hitInfo, this.maxRaycastDistance, HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap)))
		{
			float num3 = Mathf.InverseLerp(this.minRaycastDistance, this.maxRaycastDistance, this.hitInfo.distance);
			float num4 = Mathf.Clamp01(Vector3.Dot(this.hitInfo.normal, this.currentWindDirection));
			num3 += 1f - num4;
			num *= Mathf.Clamp01(num3);
		}
		num *= ((character.data.fallSeconds >= 0.01f) ? this.ragdolledWindForceMult : 1f);
		num *= mult;
		character.AddForceAtPosition(this.currentWindDirection * this.windForce * this.windPlayerFactor * num, character.Center, this.forceRadius);
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x00027D10 File Offset: 0x00025F10
	private void AddWindForceToItem(Item item)
	{
		float d = this.useIntensityCurve ? (Mathf.Clamp01(this.windIntensity - 0.5f) * 2f) : 1f;
		item.rig.AddForce(this.currentWindDirection * this.windForce * this.windItemFactor * d, ForceMode.Acceleration);
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00027D74 File Offset: 0x00025F74
	private Vector3 RandomWindDirection()
	{
		return Vector3.Lerp(Vector3.right * ((Random.value > 0.5f) ? 1f : -1f), Vector3.forward, 0.2f).normalized;
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x00027DBC File Offset: 0x00025FBC
	internal static float GetWindIntensityAtPoint(Vector3 point, float thresholdLower, float thresholdMargin)
	{
		float num = LightVolume.Instance().SamplePositionAlpha(point);
		float result;
		if (num > thresholdLower + thresholdMargin)
		{
			result = 1f;
		}
		else if (num < thresholdLower)
		{
			result = 0f;
		}
		else
		{
			result = Util.RangeLerp(0f, 1f, thresholdLower, thresholdLower + thresholdMargin, num, true, null);
		}
		return result;
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x060006EC RID: 1772 RVA: 0x00027E0D File Offset: 0x0002600D
	private float timeSpentWaiting
	{
		get
		{
			return this.timeUntilNextWind - this.untilSwitch;
		}
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00027E1C File Offset: 0x0002601C
	[PunRPC]
	private void RPCA_ToggleWind(bool set, Vector3 windDir, float untilSwitch)
	{
		this.windActive = set;
		this.untilSwitch = untilSwitch;
		this.timeUntilNextWind = untilSwitch;
		if (this.windActive)
		{
			this.currentWindDirection = windDir;
		}
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00027E44 File Offset: 0x00026044
	private float GetNextWindTime(bool windActive)
	{
		if (windActive)
		{
			return Random.Range(this.windTimeRangeOn.x, this.windTimeRangeOn.y);
		}
		if (this.debubEnable)
		{
			return 0f;
		}
		return Random.Range(this.windTimeRangeOff.x, this.windTimeRangeOff.y);
	}

	// Token: 0x040006D7 RID: 1751
	public static WindChillZone instance;

	// Token: 0x040006D8 RID: 1752
	[Range(0f, 1f)]
	public float StormProgress;

	// Token: 0x040006D9 RID: 1753
	[Range(0f, 1f)]
	public float timeUntilStorm;

	// Token: 0x040006DA RID: 1754
	public float grabStaminaMultiplierDuringWind = 1f;

	// Token: 0x040006DB RID: 1755
	public Vector2 windTimeRangeOn;

	// Token: 0x040006DC RID: 1756
	public Vector2 windTimeRangeOff;

	// Token: 0x040006DD RID: 1757
	[Range(0f, 1f)]
	public float lightVolumeSampleThreshold_lower;

	// Token: 0x040006DE RID: 1758
	[Range(0f, 1f)]
	public float lightVolumeSampleThreshold_margin;

	// Token: 0x040006DF RID: 1759
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x040006E0 RID: 1760
	public Bounds windZoneBounds;

	// Token: 0x040006E1 RID: 1761
	internal Vector3 currentWindDirection;

	// Token: 0x040006E2 RID: 1762
	private Color gizmoColor = new Color(0f, 0f, 1f, 0.5f);

	// Token: 0x040006E3 RID: 1763
	private float timeUntilNextWind;

	// Token: 0x040006E4 RID: 1764
	public float forceRadius = 2f;

	// Token: 0x040006E5 RID: 1765
	public float delayBeforeForce = 2f;

	// Token: 0x040006E6 RID: 1766
	public float ragdolledWindForceMult = 0.5f;

	// Token: 0x040006E7 RID: 1767
	public bool windMovesItems;

	// Token: 0x040006E8 RID: 1768
	private float untilSwitch;

	// Token: 0x040006E9 RID: 1769
	[FormerlySerializedAs("windChillPerSecond")]
	public float statusApplicationPerSecond = 0.01f;

	// Token: 0x040006EA RID: 1770
	public float windForce = 15f;

	// Token: 0x040006EB RID: 1771
	internal float hasBeenActiveFor;

	// Token: 0x040006EC RID: 1772
	private PhotonView view;

	// Token: 0x040006ED RID: 1773
	public bool setSlippy;

	// Token: 0x040006EE RID: 1774
	[FormerlySerializedAs("characterInsideBounds")]
	public bool localCharacterInsideBounds;

	// Token: 0x040006EF RID: 1775
	[FormerlySerializedAs("characterInsideBounds")]
	public bool observedCharacterInsideBounds;

	// Token: 0x040006F0 RID: 1776
	public bool windActive;

	// Token: 0x040006F1 RID: 1777
	public float windPlayerFactor;

	// Token: 0x040006F2 RID: 1778
	public bool debubEnable;

	// Token: 0x040006F3 RID: 1779
	public float windItemFactor = 1f;

	// Token: 0x040006F4 RID: 1780
	[Header("Curve")]
	public float windIntensity;

	// Token: 0x040006F5 RID: 1781
	public AnimationCurve windIntensityCurve;

	// Token: 0x040006F6 RID: 1782
	public bool useIntensityCurve;

	// Token: 0x040006F7 RID: 1783
	public bool useRaycast;

	// Token: 0x040006F8 RID: 1784
	public float maxRaycastDistance;

	// Token: 0x040006F9 RID: 1785
	public float minRaycastDistance;

	// Token: 0x040006FA RID: 1786
	public RunSettings.SETTINGTYPE disableIfSettingDisabled;

	// Token: 0x040006FB RID: 1787
	private float currentForceMult = 1f;

	// Token: 0x040006FC RID: 1788
	private RaycastHit hitInfo;
}
