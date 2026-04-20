using System;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000BF RID: 191
public static class GlobalEvents
{
	// Token: 0x0600074A RID: 1866 RVA: 0x00029450 File Offset: 0x00027650
	public static void TriggerItemRequested(Item interactor, Character character)
	{
		try
		{
			if (GlobalEvents.OnItemRequested != null)
			{
				GlobalEvents.OnItemRequested(interactor, character);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x0002948C File Offset: 0x0002768C
	public static void TriggerItemConsumed(Item item, Character character)
	{
		try
		{
			if (item != null && character != null)
			{
				Debug.Log(item.UIData.itemName + " consumed by " + character.gameObject.name);
			}
			if (GlobalEvents.OnItemConsumed != null)
			{
				GlobalEvents.OnItemConsumed(item, character);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x000294FC File Offset: 0x000276FC
	public static void TriggerRespawnChestOpened(RespawnChest chest, Character character)
	{
		try
		{
			if (GlobalEvents.OnRespawnChestOpened != null)
			{
				GlobalEvents.OnRespawnChestOpened(chest, character);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00029538 File Offset: 0x00027738
	public static void TriggerLuggageOpened(Luggage luggage, Character character)
	{
		try
		{
			if (GlobalEvents.OnLuggageOpened != null)
			{
				GlobalEvents.OnLuggageOpened(luggage, character);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x00029574 File Offset: 0x00027774
	public static void TriggerLocalCharacterWonRun()
	{
		try
		{
			if (GlobalEvents.OnLocalCharacterWonRun != null)
			{
				GlobalEvents.OnLocalCharacterWonRun();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x000295AC File Offset: 0x000277AC
	public static void TriggerSomeoneWonRun()
	{
		try
		{
			if (GlobalEvents.OnSomeoneWonRun != null)
			{
				GlobalEvents.OnSomeoneWonRun();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x000295E4 File Offset: 0x000277E4
	public static void TriggerCharacterPassedOut(Character character)
	{
		try
		{
			if (GlobalEvents.OnCharacterPassedOut != null)
			{
				GlobalEvents.OnCharacterPassedOut(character);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0002961C File Offset: 0x0002781C
	public static void TriggerCharacterDied(Character character)
	{
		try
		{
			if (GlobalEvents.OnCharacterDied != null)
			{
				GlobalEvents.OnCharacterDied(character);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00029654 File Offset: 0x00027854
	public static void TriggerLocalCharacterStatusIncremented(Character character, CharacterAfflictions.STATUSTYPE statusType, bool changeWasPositive)
	{
		try
		{
			if (GlobalEvents.OnLocalStatusIncremented != null)
			{
				GlobalEvents.OnLocalStatusIncremented(character, statusType, changeWasPositive);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x00029690 File Offset: 0x00027890
	public static void TriggerCharacterFell(Character character, float time)
	{
		try
		{
			if (GlobalEvents.OnCharacterFell != null)
			{
				GlobalEvents.OnCharacterFell(character, time);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x000296CC File Offset: 0x000278CC
	public static void TriggerRunEnded()
	{
		try
		{
			if (GlobalEvents.OnRunEnded != null)
			{
				GlobalEvents.OnRunEnded();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00029704 File Offset: 0x00027904
	public static void TriggerBugleTooted(Item bugle)
	{
		try
		{
			if (GlobalEvents.OnBugleTooted != null)
			{
				GlobalEvents.OnBugleTooted(bugle);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x0002973C File Offset: 0x0002793C
	public static void TriggerCharacterSpawned(Character character)
	{
		try
		{
			if (GlobalEvents.OnCharacterSpawned != null)
			{
				GlobalEvents.OnCharacterSpawned(character);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x00029774 File Offset: 0x00027974
	public static void TriggerCharacterDestroyed(Character character)
	{
		try
		{
			if (GlobalEvents.OnCharacterOwnerDisconnected != null)
			{
				GlobalEvents.OnCharacterOwnerDisconnected(character);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x000297AC File Offset: 0x000279AC
	public static void TriggerCharacterAudioLevelsUpdated()
	{
		try
		{
			if (GlobalEvents.OnCharacterAudioLevelsUpdated != null)
			{
				GlobalEvents.OnCharacterAudioLevelsUpdated();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x000297E4 File Offset: 0x000279E4
	public static void TriggerPlayerConnected(Photon.Realtime.Player player)
	{
		try
		{
			if (GlobalEvents.OnPlayerConnected != null)
			{
				GlobalEvents.OnPlayerConnected(player);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x0002981C File Offset: 0x00027A1C
	public static void TriggerPlayerDisconnected(Photon.Realtime.Player player)
	{
		try
		{
			if (GlobalEvents.OnPlayerDisconnected != null)
			{
				GlobalEvents.OnPlayerDisconnected(player);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00029854 File Offset: 0x00027A54
	public static void TriggerItemThrown(Item item)
	{
		try
		{
			if (GlobalEvents.OnItemThrown != null)
			{
				GlobalEvents.OnItemThrown(item);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x0002988C File Offset: 0x00027A8C
	public static void TriggerItemSetKinematic(Item item, bool kinematic)
	{
		try
		{
			if (GlobalEvents.OnItemSetKinematic != null)
			{
				GlobalEvents.OnItemSetKinematic(item, kinematic);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x000298C8 File Offset: 0x00027AC8
	public static void TriggerAchievementThrown(ACHIEVEMENTTYPE cheevo)
	{
		try
		{
			if (GlobalEvents.OnAchievementThrown != null)
			{
				GlobalEvents.OnAchievementThrown(cheevo);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x00029900 File Offset: 0x00027B00
	public static void TriggerGemActivated(bool activated)
	{
		try
		{
			if (GlobalEvents.OnGemActivated != null)
			{
				GlobalEvents.OnGemActivated(activated);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x00029938 File Offset: 0x00027B38
	public static void TriggerRunSettingsUpdated()
	{
		try
		{
			if (GlobalEvents.OnRunSettingsUpdated != null)
			{
				GlobalEvents.OnRunSettingsUpdated();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0400071F RID: 1823
	public static Action<Item, Character> OnItemRequested;

	// Token: 0x04000720 RID: 1824
	public static Action<Item, Character> OnItemConsumed;

	// Token: 0x04000721 RID: 1825
	public static Action<RespawnChest, Character> OnRespawnChestOpened;

	// Token: 0x04000722 RID: 1826
	public static Action<Luggage, Character> OnLuggageOpened;

	// Token: 0x04000723 RID: 1827
	public static Action OnLocalCharacterWonRun;

	// Token: 0x04000724 RID: 1828
	public static Action OnSomeoneWonRun;

	// Token: 0x04000725 RID: 1829
	public static Action<Character> OnCharacterPassedOut;

	// Token: 0x04000726 RID: 1830
	public static Action<Character> OnCharacterDied;

	// Token: 0x04000727 RID: 1831
	public static Action<Character, CharacterAfflictions.STATUSTYPE, bool> OnLocalStatusIncremented;

	// Token: 0x04000728 RID: 1832
	public static Action<Character, float> OnCharacterFell;

	// Token: 0x04000729 RID: 1833
	public static Action OnRunEnded;

	// Token: 0x0400072A RID: 1834
	public static Action<Item> OnBugleTooted;

	// Token: 0x0400072B RID: 1835
	public static Action<Character> OnCharacterSpawned;

	// Token: 0x0400072C RID: 1836
	public static Action<Character> OnCharacterOwnerDisconnected;

	// Token: 0x0400072D RID: 1837
	public static Action OnCharacterAudioLevelsUpdated;

	// Token: 0x0400072E RID: 1838
	public static Action<Photon.Realtime.Player> OnPlayerConnected;

	// Token: 0x0400072F RID: 1839
	public static Action<Photon.Realtime.Player> OnPlayerDisconnected;

	// Token: 0x04000730 RID: 1840
	public static Action<Item> OnItemThrown;

	// Token: 0x04000731 RID: 1841
	public static Action<Item, bool> OnItemSetKinematic;

	// Token: 0x04000732 RID: 1842
	public static Action<ACHIEVEMENTTYPE> OnAchievementThrown;

	// Token: 0x04000733 RID: 1843
	public static Action<bool> OnGemActivated;

	// Token: 0x04000734 RID: 1844
	public static Action OnRunSettingsUpdated;
}
