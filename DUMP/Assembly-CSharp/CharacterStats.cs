using System;
using System.Collections.Generic;
using Peak.Dev;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Core.Serizalization;

// Token: 0x02000017 RID: 23
public class CharacterStats : MonoBehaviour
{
	// Token: 0x1700002E RID: 46
	// (get) Token: 0x06000215 RID: 533 RVA: 0x0001045C File Offset: 0x0000E65C
	// (set) Token: 0x06000216 RID: 534 RVA: 0x0001046E File Offset: 0x0000E66E
	public bool IsInitialized
	{
		get
		{
			return Time.frameCount >= this._frameInitialized;
		}
		set
		{
			if (!value)
			{
				Debug.LogError("Can't UN-initialize you BUFFOON!!!");
				return;
			}
			this._frameInitialized = Time.frameCount;
		}
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0001048C File Offset: 0x0000E68C
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
		this._timeAwoken = Time.time;
		if (!GameHandler.IsInGameplayScene)
		{
			Debug.Log(SceneManager.GetActiveScene().name + " isn't a gameplay scene so we don't need stats. Disabling self for " + base.name + ".");
			base.enabled = false;
		}
	}

	// Token: 0x06000218 RID: 536 RVA: 0x000104E8 File Offset: 0x0000E6E8
	public void GetCaughtUp(CharacterStats.SyncData reconnectData, bool wasRevived)
	{
		if (this.timelineInfo.Count > 0 || this.IsInitialized)
		{
			Debug.LogWarning("Uh oh! Sent scout report history even though we've already initialized " + string.Format("and recorded {0} new entries. Clearing them", this.timelineInfo.Count));
			this.timelineInfo.Clear();
		}
		Debug.Log("Catching up with " + Pretty.Print(reconnectData.Timeline));
		this.timelineInfo = reconnectData.Timeline;
		this.IsInitialized = true;
		this.tick = 0f;
		this.justRevived = wasRevived;
	}

	// Token: 0x06000219 RID: 537 RVA: 0x00010580 File Offset: 0x0000E780
	private void CheckHeightAchievement()
	{
		if (this.character.IsLocal && !this.character.data.dead && !this.character.warping && this.character.data.sinceDead > this.tickRate)
		{
			Singleton<AchievementManager>.Instance.RecordMaxHeight(Mathf.RoundToInt(this.heightInMeters));
		}
	}

	// Token: 0x0600021A RID: 538 RVA: 0x000105E8 File Offset: 0x0000E7E8
	private void Update()
	{
		if ((Time.timeSinceLevelLoad < 3f || !this.IsInitialized) && Time.time - this._timeAwoken > 20f)
		{
			return;
		}
		if (!this.IsInitialized)
		{
			Debug.LogWarning("What da heck!! Failed to initialize 20 seconds into waking up. Something is donked.");
			this.IsInitialized = true;
			this.Record(false, 0f);
		}
		if (this.character.warping)
		{
			return;
		}
		this.heightInUnits = this.character.HipPos().y;
		this.heightInMeters = (float)Mathf.RoundToInt(this.heightInUnits * CharacterStats.unitsToMeters);
		this.tick += Time.deltaTime;
		if (this.tick > this.tickRate && !this.won && !this.lost)
		{
			this.CheckHeightAchievement();
			this.Record(false, 0f);
		}
	}

	// Token: 0x0600021B RID: 539 RVA: 0x000106C0 File Offset: 0x0000E8C0
	public void Initialize()
	{
		this._frameInitialized = Time.frameCount;
	}

	// Token: 0x0600021C RID: 540 RVA: 0x000106CD File Offset: 0x0000E8CD
	public EndScreen.TimelineInfo GetFirstTimelineInfo()
	{
		return this.timelineInfo[0];
	}

	// Token: 0x0600021D RID: 541 RVA: 0x000106DB File Offset: 0x0000E8DB
	public EndScreen.TimelineInfo GetFinalTimelineInfo()
	{
		List<EndScreen.TimelineInfo> list = this.timelineInfo;
		return list[list.Count - 1];
	}

	// Token: 0x0600021E RID: 542 RVA: 0x000106F0 File Offset: 0x0000E8F0
	public static int UnitsToMeters(float units)
	{
		return Mathf.RoundToInt(Mathf.Min(units, 1200f) * CharacterStats.unitsToMeters);
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00010708 File Offset: 0x0000E908
	public static List<EndScreen.TimelineInfo> Downsample(List<EndScreen.TimelineInfo> originalSeries, int numSamplesNeeded)
	{
		if (originalSeries.Count <= numSamplesNeeded)
		{
			Debug.Log(string.Format("No need to downsample this. We only have {0} samples which is under {1}", originalSeries.Count, numSamplesNeeded));
			return originalSeries;
		}
		float num = originalSeries[originalSeries.Count - 1].time - originalSeries[0].time;
		float time = originalSeries[0].time;
		float num2 = (float)(originalSeries.Count - numSamplesNeeded) / (float)numSamplesNeeded;
		float num3 = num / (float)numSamplesNeeded;
		List<EndScreen.TimelineInfo> list = new List<EndScreen.TimelineInfo>(numSamplesNeeded);
		list.Add(originalSeries[0]);
		int num4 = 0;
		List<int> list2 = new List<int>();
		float num5 = time + 2f * num3;
		float num6 = 2f * num2 - 1f;
		int num7 = 1;
		while (list.Count < numSamplesNeeded - 1 && num7 < originalSeries.Count - 1 && num4++ < 100000)
		{
			list2.Clear();
			int num8 = num7;
			while (originalSeries[num7].time >= num5 || num6 < 1f)
			{
				num5 += num3;
				num6 += num2;
			}
			while (num7 < originalSeries.Count - 2 && originalSeries[num7].time < num5)
			{
				if (originalSeries[num7].Note == EndScreen.TimelineNote.None)
				{
					list2.Add(num7);
				}
				num7++;
			}
			int num9 = num8;
			while (num9 <= num7 && list.Count < numSamplesNeeded - 1)
			{
				if (list2.Contains(num9) && num6 >= 1f)
				{
					num6 -= 1f;
				}
				else
				{
					list.Add(originalSeries[num9]);
				}
				num9++;
			}
			num7++;
			num5 += num3;
			num6 += num2;
		}
		list.Add(originalSeries[originalSeries.Count - 1]);
		if (num6 > 2f || list.Count != numSamplesNeeded)
		{
			Debug.LogError(string.Format("Failed to properly downsample {0} points to {1}!", originalSeries.Count, numSamplesNeeded) + string.Format("Final array had {0} in it and we recorded {1} ", list.Count, num6 - 1f) + "unresolved cuts.");
		}
		if (num4 == 100000)
		{
			Debug.LogError("UH OHHHHH! Almost got stuck in an infinite loop. That's pretty bad.");
		}
		return list;
	}

	// Token: 0x06000220 RID: 544 RVA: 0x00010954 File Offset: 0x0000EB54
	public void Record(bool useOverridePosition = false, float overrideHeight = 0f)
	{
		if (!this.IsInitialized)
		{
			Debug.LogWarning("Attempting to record before we've initialized!! This will break the scout report");
		}
		this.tick = 0f;
		float num = useOverridePosition ? overrideHeight : this.heightInUnits;
		if (num > 1200f)
		{
			return;
		}
		EndScreen.TimelineInfo item = new EndScreen.TimelineInfo(num, RunManager.Instance.timeSinceRunStarted * CharacterStats.debugTickScaler, EndScreen.TimelineNote.None);
		if (this.justDied)
		{
			this.justDied = false;
			item.died = true;
		}
		else if (this.character.data.dead)
		{
			item.dead = true;
		}
		else if (this.justRevived)
		{
			this.justRevived = false;
			item.revived = true;
			Debug.LogError("RECORD REVIVED!");
		}
		else if (this.justPassedOut)
		{
			this.justPassedOut = false;
			item.justPassedOut = true;
		}
		else if (this.character.data.passedOut)
		{
			item.passedOut = true;
		}
		this.timelineInfo.Add(item);
	}

	// Token: 0x06000221 RID: 545 RVA: 0x00010A44 File Offset: 0x0000EC44
	public void Win()
	{
		this.won = true;
		if (this.character.IsLocal)
		{
			EndScreen.TimelineInfo value = this.timelineInfo[this.timelineInfo.Count - 1];
			value.won = true;
			GlobalEvents.TriggerLocalCharacterWonRun();
			this.timelineInfo[this.timelineInfo.Count - 1] = value;
		}
	}

	// Token: 0x06000222 RID: 546 RVA: 0x00010AA4 File Offset: 0x0000ECA4
	public void Lose(bool somebodyElseWon)
	{
		this.lost = true;
		this.somebodyElseWon = somebodyElseWon;
	}

	// Token: 0x06000223 RID: 547 RVA: 0x00010AB4 File Offset: 0x0000ECB4
	[ConsoleCommand]
	public static void DebugTickRate(float val)
	{
		CharacterStats[] array = Object.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None);
		Debug.Log(string.Format("setting the scout report tick rate for all active {0} characters to {1}.", array.Length, val));
		foreach (CharacterStats characterStats in array)
		{
			CharacterStats.debugTickScaler = characterStats.tickRate / val;
			characterStats.tickRate = val;
		}
	}

	// Token: 0x040001E6 RID: 486
	public const float peakHeightInUnits = 1200f;

	// Token: 0x040001E7 RID: 487
	private Character character;

	// Token: 0x040001E8 RID: 488
	public float heightInUnits;

	// Token: 0x040001E9 RID: 489
	public float heightInMeters;

	// Token: 0x040001EA RID: 490
	public static float unitsToMeters = 1.6f;

	// Token: 0x040001EB RID: 491
	private float tick;

	// Token: 0x040001EC RID: 492
	public float tickRate = 1f;

	// Token: 0x040001ED RID: 493
	private static float debugTickScaler = 1f;

	// Token: 0x040001EE RID: 494
	public List<EndScreen.TimelineInfo> timelineInfo = new List<EndScreen.TimelineInfo>();

	// Token: 0x040001EF RID: 495
	public bool won;

	// Token: 0x040001F0 RID: 496
	public bool lost;

	// Token: 0x040001F1 RID: 497
	public bool somebodyElseWon;

	// Token: 0x040001F2 RID: 498
	public bool justDied;

	// Token: 0x040001F3 RID: 499
	public bool justPassedOut;

	// Token: 0x040001F4 RID: 500
	public bool justRevived;

	// Token: 0x040001F5 RID: 501
	private int _frameInitialized = int.MaxValue;

	// Token: 0x040001F6 RID: 502
	private float _timeAwoken;

	// Token: 0x0200041D RID: 1053
	public struct SyncData : IBinarySerializable, IPrettyPrintable
	{
		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06001B86 RID: 7046 RVA: 0x0008532B File Offset: 0x0008352B
		// (set) Token: 0x06001B87 RID: 7047 RVA: 0x00085333 File Offset: 0x00083533
		public List<EndScreen.TimelineInfo> Timeline { readonly get; private set; }

		// Token: 0x06001B88 RID: 7048 RVA: 0x0008533C File Offset: 0x0008353C
		public SyncData(List<EndScreen.TimelineInfo> timeline)
		{
			if (timeline.Count > 512)
			{
				timeline = CharacterStats.Downsample(timeline, 512);
			}
			this.Timeline = timeline;
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x00085360 File Offset: 0x00083560
		public void Serialize(BinarySerializer serializer)
		{
			int num = this.Timeline.Count;
			if (num > 512)
			{
				Debug.LogError("BIG OOPS! Downsampled timeline is still too large to fit in SyncData. We'll have to truncate");
				num = 512;
				while (this.Timeline.Count > 512)
				{
					this.Timeline.RemoveAt(512);
				}
			}
			serializer.WriteUshort((ushort)num);
			foreach (EndScreen.TimelineInfo timelineInfo in this.Timeline)
			{
				serializer.WriteUshort(timelineInfo.heightRecord);
				serializer.WriteUshort(timelineInfo.timestamp);
				serializer.WriteByte((byte)timelineInfo.Note);
			}
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x00085424 File Offset: 0x00083624
		public void Deserialize(BinaryDeserializer deserializer)
		{
			ushort num = deserializer.ReadUShort();
			this.Timeline = new List<EndScreen.TimelineInfo>((int)num);
			for (int i = 0; i < (int)num; i++)
			{
				ushort height = deserializer.ReadUShort();
				ushort time = deserializer.ReadUShort();
				EndScreen.TimelineNote note = (EndScreen.TimelineNote)deserializer.ReadByte();
				this.Timeline.Add(new EndScreen.TimelineInfo(height, time, note));
			}
		}

		// Token: 0x06001B8B RID: 7051 RVA: 0x0008547C File Offset: 0x0008367C
		public string ToPrettyString()
		{
			if (this.Timeline.Count < 2)
			{
				return string.Format("{0} entries", this.Timeline.Count);
			}
			string str = string.Format("{0} entries |  ", this.Timeline.Count);
			string str2 = string.Format("{0}sec/{1}units => ", this.Timeline[0].timestamp, this.Timeline[0].heightRecord);
			string format = "{0}sec/{1}units";
			List<EndScreen.TimelineInfo> timeline = this.Timeline;
			object arg = timeline[timeline.Count - 1].timestamp;
			List<EndScreen.TimelineInfo> timeline2 = this.Timeline;
			return str + str2 + string.Format(format, arg, timeline2[timeline2.Count - 1].heightRecord);
		}

		// Token: 0x04001813 RID: 6163
		private const int MaxAllowedSize = 512;
	}
}
