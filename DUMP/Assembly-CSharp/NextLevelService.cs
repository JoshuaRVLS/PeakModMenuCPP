using System;
using UnityEngine;
using Zorro.Core;
using Zorro.UI.Modal;

// Token: 0x02000154 RID: 340
public class NextLevelService : GameService
{
	// Token: 0x170000CF RID: 207
	// (get) Token: 0x06000B3B RID: 2875 RVA: 0x0003C8B8 File Offset: 0x0003AAB8
	private NextLevelService.NextLevelData FallbackData
	{
		get
		{
			NextLevelService.NextLevelData value = this._offlineDataFallback.GetValueOrDefault();
			if (this._offlineDataFallback == null)
			{
				value = NextLevelService.CreateFallbackData();
				this._offlineDataFallback = new NextLevelService.NextLevelData?(value);
			}
			return this._offlineDataFallback.Value;
		}
	}

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x06000B3C RID: 2876 RVA: 0x0003C8FC File Offset: 0x0003AAFC
	public int SecondsLeftFallback
	{
		get
		{
			return this.FallbackData.SecondsLeft;
		}
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x06000B3D RID: 2877 RVA: 0x0003C917 File Offset: 0x0003AB17
	public bool HasReceivedLevelIndex
	{
		get
		{
			return this.Data.IsSome;
		}
	}

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x06000B3E RID: 2878 RVA: 0x0003C924 File Offset: 0x0003AB24
	public int NextLevelIndexOrFallback
	{
		get
		{
			if (!this.HasReceivedLevelIndex)
			{
				return this.OfflineLevelIndex;
			}
			return this.Data.Value.CurrentLevelIndex;
		}
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x0003C948 File Offset: 0x0003AB48
	public void NewData(LoginResponse response)
	{
		this.Data = Optionable<NextLevelService.NextLevelData>.Some(new NextLevelService.NextLevelData(response));
		Debug.Log("Setting new NextLevelData: " + this.Data.Value.ToString());
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x0003C990 File Offset: 0x0003AB90
	private static NextLevelService.NextLevelData CreateFallbackData()
	{
		DateTime d = new DateTime(2025, 6, 14, 17, 0, 0);
		double num = (double)((int)Math.Floor((DateTime.UtcNow - d).TotalHours));
		int num2 = 24;
		int num3 = (int)Math.Floor(num / (double)num2);
		TimeSpan timeSpan = d.AddHours((double)(num3 * num2)).AddHours((double)num2) - DateTime.UtcNow;
		int hoursUntilLevel = (int)Math.Floor(timeSpan.TotalHours);
		int minutes = timeSpan.Minutes;
		int seconds = timeSpan.Seconds;
		return new NextLevelService.NextLevelData(new LoginResponse
		{
			VersionOkay = true,
			HoursUntilLevel = hoursUntilLevel,
			MinutesUntilLevel = minutes,
			SecondsUntilLevel = seconds,
			LevelIndex = num3,
			Message = string.Empty
		});
	}

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x06000B41 RID: 2881 RVA: 0x0003CA55 File Offset: 0x0003AC55
	public int OfflineLevelIndex
	{
		get
		{
			return this.FallbackData.CurrentLevelIndex;
		}
	}

	// Token: 0x04000A58 RID: 2648
	public static int debugLevelIndexOffset;

	// Token: 0x04000A59 RID: 2649
	public Optionable<NextLevelService.NextLevelData> Data;

	// Token: 0x04000A5A RID: 2650
	private NextLevelService.NextLevelData? _offlineDataFallback;

	// Token: 0x02000499 RID: 1177
	public struct NextLevelData
	{
		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06001CFC RID: 7420 RVA: 0x0008918C File Offset: 0x0008738C
		public int SecondsLeft
		{
			get
			{
				float num = Time.realtimeSinceStartup - this.StartupTimeWhenQueried;
				float num2 = this.SecondsLeftFromQueryTime - num;
				QueryingGameTimeStatus queryingGameTimeStatus;
				if (num2 < 0f && !GameHandler.TryGetStatus<QueryingGameTimeStatus>(out queryingGameTimeStatus))
				{
					CloudAPI.CheckVersion(delegate(LoginResponse response)
					{
						GameHandler.GetService<NextLevelService>().NewData(response);
						if (!response.VersionOkay)
						{
							Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_OUTOFDATE_TITLE", true), LocalizedText.GetText("MODAL_OUTOFDATE_BODY", true)), new ModalButtonsOption(new ModalButtonsOption.Option[]
							{
								new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
							}), new Action(Application.Quit));
						}
					});
				}
				return Mathf.RoundToInt(num2);
			}
		}

		// Token: 0x06001CFD RID: 7421 RVA: 0x000891E8 File Offset: 0x000873E8
		public NextLevelData(LoginResponse login)
		{
			this.CurrentLevelIndex = login.LevelIndex;
			this.StartupTimeWhenQueried = Time.realtimeSinceStartup;
			float secondsLeftFromQueryTime = (float)(login.HoursUntilLevel * 60 * 60 + login.MinutesUntilLevel * 60 + login.SecondsUntilLevel);
			this.SecondsLeftFromQueryTime = secondsLeftFromQueryTime;
			this.DevMessage = login.Message;
		}

		// Token: 0x06001CFE RID: 7422 RVA: 0x0008923E File Offset: 0x0008743E
		public override string ToString()
		{
			return string.Format("CurrentIndex: {0}, seconds left {1}", this.CurrentLevelIndex, this.SecondsLeft);
		}

		// Token: 0x04001A2D RID: 6701
		public int CurrentLevelIndex;

		// Token: 0x04001A2E RID: 6702
		public float StartupTimeWhenQueried;

		// Token: 0x04001A2F RID: 6703
		public float SecondsLeftFromQueryTime;

		// Token: 0x04001A30 RID: 6704
		public string DevMessage;
	}
}
