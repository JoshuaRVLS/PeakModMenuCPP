using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002B3 RID: 691
public class MountainProgressHandler : Singleton<MountainProgressHandler>
{
	// Token: 0x17000141 RID: 321
	// (get) Token: 0x0600138A RID: 5002 RVA: 0x00063439 File Offset: 0x00061639
	// (set) Token: 0x0600138B RID: 5003 RVA: 0x00063441 File Offset: 0x00061641
	public int JoinedInSegment { get; private set; } = -1;

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x0600138C RID: 5004 RVA: 0x0006344A File Offset: 0x0006164A
	// (set) Token: 0x0600138D RID: 5005 RVA: 0x00063452 File Offset: 0x00061652
	public int maxProgressPointReached { get; private set; }

	// Token: 0x0600138E RID: 5006 RVA: 0x0006345B File Offset: 0x0006165B
	private void Start()
	{
		this.InitProgressPoints();
		GameHandler.GetService<RichPresenceService>().SetState(RichPresenceState.Status_Shore);
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x00063470 File Offset: 0x00061670
	private void InitProgressPoints()
	{
		if (!Singleton<MapHandler>.Instance || this._isInitialized)
		{
			return;
		}
		this._skippedBiomes = new List<Biome.BiomeType>();
		List<MountainProgressHandler.ProgressPoint> list = new List<MountainProgressHandler.ProgressPoint>();
		using (List<Biome.BiomeType>.Enumerator enumerator = Singleton<MapHandler>.Instance.biomes.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Biome.BiomeType biomeInCurrentMap = enumerator.Current;
				IEnumerable<MountainProgressHandler.ProgressPoint> collection = from point in this.progressPoints
				where point.biome == biomeInCurrentMap
				select point;
				list.AddRange(collection);
			}
		}
		list.Add(this.progressPoints.Last<MountainProgressHandler.ProgressPoint>());
		this.progressPoints = list.ToArray();
		this._isInitialized = true;
	}

	// Token: 0x06001390 RID: 5008 RVA: 0x00063534 File Offset: 0x00061734
	public void SetSegmentComplete(int segment)
	{
		Debug.Log("Segment complete: " + segment.ToString());
		MountainProgressHandler.ProgressPoint progressPoint = this.progressPoints[segment];
		progressPoint.Reached = true;
		this.TriggerReached(progressPoint, true);
		if (segment > this.maxProgressPointReached)
		{
			this.maxProgressPointReached = segment;
		}
	}

	// Token: 0x06001391 RID: 5009 RVA: 0x00063580 File Offset: 0x00061780
	public static void JumpToSegment(int segment, float delayTitle = 5f)
	{
		Singleton<MountainProgressHandler>.Instance.InitProgressPoints();
		for (int i = 0; i < segment; i++)
		{
			MountainProgressHandler.ProgressPoint progressPoint = Singleton<MountainProgressHandler>.Instance.progressPoints[i];
			progressPoint.Reached = true;
			if (i != segment)
			{
				MountainProgressHandler.MarkBiomeSkipped(progressPoint.biome);
			}
		}
		Singleton<MountainProgressHandler>.Instance.JoinedInSegment = segment;
		MountainProgressHandler.SetRichPresence(Singleton<MountainProgressHandler>.Instance.progressPoints[segment]);
		if (segment > Singleton<MountainProgressHandler>.Instance.maxProgressPointReached)
		{
			Singleton<MountainProgressHandler>.Instance.maxProgressPointReached = segment;
		}
	}

	// Token: 0x06001392 RID: 5010 RVA: 0x000635FC File Offset: 0x000617FC
	public static void DisplaySegmentTitleAfterDelay(int segment, float delay)
	{
		MountainProgressHandler.<>c__DisplayClass17_0 CS$<>8__locals1 = new MountainProgressHandler.<>c__DisplayClass17_0();
		CS$<>8__locals1.segment = segment;
		CS$<>8__locals1.delay = delay;
		Singleton<MountainProgressHandler>.Instance.StartCoroutine(CS$<>8__locals1.<DisplaySegmentTitleAfterDelay>g__ShowTitleLater|0());
	}

	// Token: 0x06001393 RID: 5011 RVA: 0x0006362E File Offset: 0x0006182E
	public static void MarkBiomeSkipped(Biome.BiomeType biome)
	{
		Singleton<MountainProgressHandler>.Instance._skippedBiomes.Add(biome);
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x00063640 File Offset: 0x00061840
	private void Update()
	{
		this.CheckProgress(true);
	}

	// Token: 0x06001395 RID: 5013 RVA: 0x0006364C File Offset: 0x0006184C
	public void CheckProgress(bool playAnimation = true)
	{
		if (!Singleton<MapHandler>.Instance)
		{
			base.enabled = false;
			Debug.LogWarning("No MapHandler in Scene, so no progress to check.");
			return;
		}
		if (!Character.localCharacter || Character.localCharacter.data.dead)
		{
			return;
		}
		for (int i = this.progressPoints.Length - 1; i >= 0; i--)
		{
			if (!this.progressPoints[i].Reached)
			{
				if (this.progressPoints[i].transform != null)
				{
					this.progressPoints[i].Reached = this.CheckReached(this.progressPoints[i]);
				}
				if (playAnimation && this.progressPoints[i].Reached)
				{
					this.TriggerReached(this.progressPoints[i], i > this.maxProgressPointReached);
				}
			}
			if (i > this.maxProgressPointReached)
			{
				this.maxProgressPointReached = i;
			}
		}
	}

	// Token: 0x06001396 RID: 5014 RVA: 0x00063722 File Offset: 0x00061922
	public void DebugTriggerReached()
	{
		this.TriggerReached(this.progressPoints[this.debugProgress], true);
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x00063738 File Offset: 0x00061938
	private static void SetRichPresence(MountainProgressHandler.ProgressPoint progressPoint)
	{
		GameHandler.GetService<RichPresenceService>().SetState(MountainProgressHandler.<SetRichPresence>g__GetRichPresenceState|23_0(progressPoint));
	}

	// Token: 0x06001398 RID: 5016 RVA: 0x0006374A File Offset: 0x0006194A
	public void TriggerReached(MountainProgressHandler.ProgressPoint progressPoint, bool isFurthestPoint = true)
	{
		if (Time.time <= 2f)
		{
			return;
		}
		this.CheckAreaAchievement(progressPoint);
		if (!isFurthestPoint)
		{
			return;
		}
		GUIManager.instance.SetHeroTitle(progressPoint.localizedTitle, progressPoint.clip);
		MountainProgressHandler.SetRichPresence(progressPoint);
	}

	// Token: 0x06001399 RID: 5017 RVA: 0x00063780 File Offset: 0x00061980
	public bool IsAtPeak(Transform tf)
	{
		return this.IsAtPeak(tf.position);
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x0006378E File Offset: 0x0006198E
	public bool IsAtPeak(Vector3 position)
	{
		return this.progressPoints != null && this.progressPoints.Length != 0 && position.z > this.progressPoints.Last<MountainProgressHandler.ProgressPoint>().transform.position.z;
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x000637C8 File Offset: 0x000619C8
	private bool CheckReached(MountainProgressHandler.ProgressPoint point)
	{
		return Character.localCharacter && (Character.localCharacter.Center.z > point.transform.position.z && !Character.localCharacter.data.dead && (Singleton<MapHandler>.Instance.BiomeIsPresent(point.biome) || point.biome == Biome.BiomeType.Peak));
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x00063834 File Offset: 0x00061A34
	private void CheckAreaAchievement(MountainProgressHandler.ProgressPoint pointReached)
	{
		if (!AchievementManager.Initialized)
		{
			return;
		}
		Debug.Log("Checking achievement. We just reached: " + pointReached.title);
		for (int i = 0; i < this.progressPoints.Length; i++)
		{
			if (this.progressPoints[i].achievement == pointReached.achievement)
			{
				return;
			}
			if (this.progressPoints[i].achievement != ACHIEVEMENTTYPE.NONE && i > this.JoinedInSegment)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(this.progressPoints[i].achievement);
			}
			if (!this._skippedBiomes.Contains(this.progressPoints[i].biome) && !Character.localCharacter.data.dead)
			{
				if (this.progressPoints[i].biome == Biome.BiomeType.Mesa)
				{
					Singleton<AchievementManager>.Instance.TestCoolCucumberAchievement();
				}
				else if (this.progressPoints[i].biome == Biome.BiomeType.Alpine)
				{
					Singleton<AchievementManager>.Instance.TestBundledUpAchievement();
				}
				else if (this.progressPoints[i].biome == Biome.BiomeType.Roots)
				{
					Singleton<AchievementManager>.Instance.TestTreadLightlyAchievement();
				}
			}
		}
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x00063948 File Offset: 0x00061B48
	[CompilerGenerated]
	internal static RichPresenceState <SetRichPresence>g__GetRichPresenceState|23_0(MountainProgressHandler.ProgressPoint p)
	{
		string title = p.title;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(title);
		if (num <= 2351483618U)
		{
			if (num <= 359194709U)
			{
				if (num != 283912690U)
				{
					if (num == 359194709U)
					{
						if (title == "TROPICS")
						{
							return RichPresenceState.Status_Tropics;
						}
					}
				}
				else if (title == "SHORE")
				{
					return RichPresenceState.Status_Shore;
				}
			}
			else if (num != 1404730151U)
			{
				if (num == 2351483618U)
				{
					if (title == "ROOTS")
					{
						return RichPresenceState.Status_Roots;
					}
				}
			}
			else if (title == "MESA")
			{
				return RichPresenceState.Status_Mesa;
			}
		}
		else if (num <= 2698620434U)
		{
			if (num != 2684125921U)
			{
				if (num == 2698620434U)
				{
					if (title == "PEAK")
					{
						return RichPresenceState.Status_Peak;
					}
				}
			}
			else if (title == "CALDERA")
			{
				return RichPresenceState.Status_Caldera;
			}
		}
		else if (num != 3092950474U)
		{
			if (num == 3587017822U)
			{
				if (title == "ALPINE")
				{
					return RichPresenceState.Status_Alpine;
				}
			}
		}
		else if (title == "THE KILN")
		{
			return RichPresenceState.Status_Kiln;
		}
		Debug.LogError("Failed to find Rich Presence State from " + p.title);
		return RichPresenceState.Status_Shore;
	}

	// Token: 0x040011E9 RID: 4585
	private bool _isInitialized;

	// Token: 0x040011EB RID: 4587
	public MountainProgressHandler.ProgressPoint[] progressPoints;

	// Token: 0x040011EC RID: 4588
	public MountainProgressHandler.ProgressPoint tombProgressPoint;

	// Token: 0x040011ED RID: 4589
	private List<Biome.BiomeType> _skippedBiomes;

	// Token: 0x040011EF RID: 4591
	public int debugProgress;

	// Token: 0x02000515 RID: 1301
	[Serializable]
	public class ProgressPoint
	{
		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06001EC8 RID: 7880 RVA: 0x0008E25A File Offset: 0x0008C45A
		// (set) Token: 0x06001EC9 RID: 7881 RVA: 0x0008E262 File Offset: 0x0008C462
		public bool Reached { get; set; }

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06001ECA RID: 7882 RVA: 0x0008E26B File Offset: 0x0008C46B
		public string localizedTitle
		{
			get
			{
				return LocalizedText.GetText(this.title, true);
			}
		}

		// Token: 0x04001C30 RID: 7216
		public Transform transform;

		// Token: 0x04001C31 RID: 7217
		public string title;

		// Token: 0x04001C32 RID: 7218
		public AudioClip clip;

		// Token: 0x04001C33 RID: 7219
		public ACHIEVEMENTTYPE achievement;

		// Token: 0x04001C34 RID: 7220
		public Biome.BiomeType biome;
	}
}
