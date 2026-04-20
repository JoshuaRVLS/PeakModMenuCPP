using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000199 RID: 409
public class LobbyTypeSetting : CustomLocalizedEnumSetting<LobbyTypeSetting.LobbyType>, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000D65 RID: 3429 RVA: 0x000454B6 File Offset: 0x000436B6
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x000454B8 File Offset: 0x000436B8
	public override void Load(ISettingsSaveLoad loader)
	{
		base.Load(loader);
		if (!PlayerPrefs.HasKey("DEFAULT_CHANGE_LOBBY_TYPE"))
		{
			base.Value = this.GetDefaultValue();
		}
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x000454D9 File Offset: 0x000436D9
	public override void Save(ISettingsSaveLoad saver)
	{
		base.Save(saver);
		PlayerPrefs.SetInt("DEFAULT_CHANGE_LOBBY_TYPE", 1);
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x000454ED File Offset: 0x000436ED
	protected override LobbyTypeSetting.LobbyType GetDefaultValue()
	{
		return LobbyTypeSetting.LobbyType.InviteOnly;
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x000454F0 File Offset: 0x000436F0
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x000454F3 File Offset: 0x000436F3
	public override List<string> GetUnlocalizedChoices()
	{
		return new List<string>
		{
			"Friends",
			"Invite Only"
		};
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x00045510 File Offset: 0x00043710
	public string GetDisplayName()
	{
		return "Lobby Mode";
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x00045517 File Offset: 0x00043717
	public string GetCategory()
	{
		return "General";
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x0004551E File Offset: 0x0004371E
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}

	// Token: 0x020004B9 RID: 1209
	public enum LobbyType
	{
		// Token: 0x04001AC9 RID: 6857
		Friends,
		// Token: 0x04001ACA RID: 6858
		InviteOnly
	}
}
