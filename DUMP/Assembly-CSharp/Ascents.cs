using System;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x0200005B RID: 91
public static class Ascents
{
	// Token: 0x17000054 RID: 84
	// (get) Token: 0x06000495 RID: 1173 RVA: 0x0001C07B File Offset: 0x0001A27B
	// (set) Token: 0x06000496 RID: 1174 RVA: 0x0001C082 File Offset: 0x0001A282
	public static int currentAscent
	{
		get
		{
			return Ascents._currentAscent;
		}
		set
		{
			Ascents._currentAscent = value;
			Debug.Log("Ascent set to " + value.ToString());
		}
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x06000497 RID: 1175 RVA: 0x0001C0A0 File Offset: 0x0001A2A0
	public static bool shouldSpawnZombie
	{
		get
		{
			return RunSettings.GetValue(RunSettings.SETTINGTYPE.Hazard_Zombies, false) != 0 && Ascents.currentAscent >= 0;
		}
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x06000498 RID: 1176 RVA: 0x0001C0BC File Offset: 0x0001A2BC
	public static bool fogEnabled
	{
		get
		{
			if (RunSettings.IsCustomRun)
			{
				return RunSettings.GetValue(RunSettings.SETTINGTYPE.Fog, false) > 0;
			}
			return Ascents.currentAscent >= 0;
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x06000499 RID: 1177 RVA: 0x0001C0DC File Offset: 0x0001A2DC
	public static float fallDamageMultiplier
	{
		get
		{
			if (Ascents.currentAscent >= 1)
			{
				return 2f;
			}
			int value = RunSettings.GetValue(RunSettings.SETTINGTYPE.FallDamage, false);
			if (value == 0)
			{
				return 0f;
			}
			if (value == 1)
			{
				return 0.5f;
			}
			if (value == 2)
			{
				return 1f;
			}
			if (value == 3)
			{
				return 2f;
			}
			return 1f;
		}
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x0600049A RID: 1178 RVA: 0x0001C130 File Offset: 0x0001A330
	public static float etcDamageMultiplier
	{
		get
		{
			if (RunSettings.IsCustomRun)
			{
				int value = RunSettings.GetValue(RunSettings.SETTINGTYPE.EtcDamage, false);
				if (value == 0)
				{
					return 0f;
				}
				if (value == 1)
				{
					return 0.5f;
				}
				if (value == 2)
				{
					return 1f;
				}
				if (value == 3)
				{
					return 2f;
				}
			}
			return 1f;
		}
	}

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x0600049B RID: 1179 RVA: 0x0001C17C File Offset: 0x0001A37C
	public static float hungerRateMultiplier
	{
		get
		{
			if (RunSettings.IsCustomRun)
			{
				switch (RunSettings.GetValue(RunSettings.SETTINGTYPE.HungerRate, false))
				{
				case 0:
					return 0f;
				case 1:
					return 0.7f;
				case 2:
					return 1f;
				case 3:
					return 1.6f;
				}
			}
			if (Ascents.currentAscent == -1)
			{
				return 0.7f;
			}
			if (Ascents.currentAscent >= 2)
			{
				return 1.6f;
			}
			return 1f;
		}
	}

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x0600049C RID: 1180 RVA: 0x0001C1F0 File Offset: 0x0001A3F0
	public static int itemWeightModifier
	{
		get
		{
			if (RunSettings.IsCustomRun)
			{
				switch (RunSettings.GetValue(RunSettings.SETTINGTYPE.ItemWeight, false))
				{
				case 0:
					return -1;
				case 1:
					return 0;
				case 2:
					return 1;
				}
			}
			if (Ascents.currentAscent < 3)
			{
				return 0;
			}
			return 1;
		}
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x0600049D RID: 1181 RVA: 0x0001C235 File Offset: 0x0001A435
	public static bool shouldSpawnFlare
	{
		get
		{
			if (RunSettings.IsCustomRun)
			{
				return RunSettings.GetValue(RunSettings.SETTINGTYPE.FlaresAtPeak, false) > 0;
			}
			return Ascents.currentAscent < 4;
		}
	}

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x0600049E RID: 1182 RVA: 0x0001C255 File Offset: 0x0001A455
	public static bool isNightCold
	{
		get
		{
			if (RunSettings.IsCustomRun)
			{
				return RunSettings.GetValue(RunSettings.SETTINGTYPE.ColdNight, false) == 1;
			}
			return Ascents.currentAscent >= 5;
		}
	}

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x0600049F RID: 1183 RVA: 0x0001C278 File Offset: 0x0001A478
	public static float nightColdRate
	{
		get
		{
			return 0.005f;
		}
	}

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x060004A0 RID: 1184 RVA: 0x0001C27F File Offset: 0x0001A47F
	public static bool canReviveDead
	{
		get
		{
			if (RunSettings.IsCustomRun)
			{
				return RunSettings.GetValue(RunSettings.SETTINGTYPE.RevivalAllowed, false) > 0;
			}
			return Ascents.currentAscent < 7;
		}
	}

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0001C2A0 File Offset: 0x0001A4A0
	public static float climbStaminaMultiplier
	{
		get
		{
			if (RunSettings.IsCustomRun)
			{
				float num = 1f;
				switch (RunSettings.GetValue(RunSettings.SETTINGTYPE.ClimbingStaminaUsage, false))
				{
				case 0:
					num = 0f;
					break;
				case 1:
					num = 0.7f;
					break;
				case 2:
					num = 1f;
					break;
				case 3:
					num = 1.4f;
					break;
				}
				if (RunSettings.GetValue(RunSettings.SETTINGTYPE.GrappleMode, false) == 1)
				{
					return 4f * num;
				}
				return num;
			}
			else
			{
				if (Ascents.currentAscent >= 6)
				{
					return 1.4f;
				}
				if (Ascents.currentAscent == -1)
				{
					return 0.7f;
				}
				return 1f;
			}
		}
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0001C335 File Offset: 0x0001A535
	[ConsoleCommand]
	public static void UnlockAll()
	{
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, 7);
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0001C344 File Offset: 0x0001A544
	[ConsoleCommand]
	public static void UnlockOne()
	{
		int num;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out num))
		{
			Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, num + 1);
		}
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x0001C370 File Offset: 0x0001A570
	[ConsoleCommand]
	public static void LockAll()
	{
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, 0);
	}

	// Token: 0x04000512 RID: 1298
	internal static int _currentAscent;

	// Token: 0x04000513 RID: 1299
	public const float HUNGER_RATE_LOW = 0.7f;

	// Token: 0x04000514 RID: 1300
	public const float HUNGER_RATE_HIGH = 1.6f;

	// Token: 0x04000515 RID: 1301
	public const float CLIMB_STAMINA_MULTIPLIER_LOW = 0.7f;

	// Token: 0x04000516 RID: 1302
	public const float CLIMB_STAMINA_MULTIPLIER_HIGH = 1.4f;

	// Token: 0x04000517 RID: 1303
	public const float STAMINA_MULTIPLIER_GRAPPLEMODE = 4f;
}
