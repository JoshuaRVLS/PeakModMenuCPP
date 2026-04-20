using System;
using UnityEngine;
using Zorro.Settings;

// Token: 0x02000226 RID: 550
public class BugPhobia : MonoBehaviour
{
	// Token: 0x060010DB RID: 4315 RVA: 0x00053DE0 File Offset: 0x00051FE0
	private void Start()
	{
		this.setting = GameHandler.Instance.SettingsHandler.GetSetting<BugPhobiaSetting>();
		if (this.setting != null)
		{
			for (int i = 0; i < this.bugPhobiaGameObjects.Length; i++)
			{
				this.bugPhobiaGameObjects[i].SetActive(this.setting.Value == OffOnMode.ON);
			}
			for (int j = 0; j < this.defaultGameObjects.Length; j++)
			{
				this.defaultGameObjects[j].SetActive(this.setting.Value != OffOnMode.ON);
			}
			if (this.bbas)
			{
				this.bbas.Init();
			}
		}
	}

	// Token: 0x04000EE1 RID: 3809
	public GameObject[] defaultGameObjects;

	// Token: 0x04000EE2 RID: 3810
	public GameObject[] bugPhobiaGameObjects;

	// Token: 0x04000EE3 RID: 3811
	private BugPhobiaSetting setting;

	// Token: 0x04000EE4 RID: 3812
	public BingBongAudioSwitch bbas;
}
