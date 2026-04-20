using System;
using HorizonBasedAmbientOcclusion.Universal;
using UnityEngine;
using UnityEngine.Rendering;
using Zorro.Settings;

// Token: 0x02000162 RID: 354
public class PostHandler : MonoBehaviour
{
	// Token: 0x06000BAC RID: 2988 RVA: 0x0003EAD1 File Offset: 0x0003CCD1
	private void Start()
	{
		this.AOSetting = GameHandler.Instance.SettingsHandler.GetSetting<AOSetting>();
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x0003EAE8 File Offset: 0x0003CCE8
	private void LateUpdate()
	{
		HBAO hbao;
		if (this.volume.sharedProfile.TryGet<HBAO>(out hbao))
		{
			hbao.active = (this.AOSetting.Value == OffOnMode.ON);
		}
	}

	// Token: 0x04000AC0 RID: 2752
	public AOSetting AOSetting;

	// Token: 0x04000AC1 RID: 2753
	public Volume volume;
}
