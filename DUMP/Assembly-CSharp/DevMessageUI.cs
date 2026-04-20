using System;
using TMPro;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class DevMessageUI : MonoBehaviour
{
	// Token: 0x06000E78 RID: 3704 RVA: 0x000489AB File Offset: 0x00046BAB
	private void Start()
	{
		this.service = GameHandler.GetService<NextLevelService>();
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x000489B8 File Offset: 0x00046BB8
	private void Update()
	{
		bool flag = this.service.Data.IsSome && !string.IsNullOrEmpty(this.service.Data.Value.DevMessage);
		if (flag)
		{
			this.message = (this.useDebugMessage ? this.debugMessage : this.service.Data.Value.DevMessage);
			if (this.message.StartsWith("#"))
			{
				this.message = this.message.Remove(0, 1);
				this.parent.SetActive(false);
				this.shillParent.SetActive(true);
			}
			else
			{
				this.parent.SetActive(true);
				this.shillParent.SetActive(false);
			}
		}
		else
		{
			this.parent.SetActive(false);
			this.shillParent.SetActive(false);
		}
		if (flag)
		{
			TextMeshProUGUI[] array = this.texts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].text = this.message;
			}
		}
	}

	// Token: 0x04000C29 RID: 3113
	public GameObject parent;

	// Token: 0x04000C2A RID: 3114
	public GameObject shillParent;

	// Token: 0x04000C2B RID: 3115
	public TextMeshProUGUI[] texts;

	// Token: 0x04000C2C RID: 3116
	private NextLevelService service;

	// Token: 0x04000C2D RID: 3117
	public bool useDebugMessage;

	// Token: 0x04000C2E RID: 3118
	public string debugMessage;

	// Token: 0x04000C2F RID: 3119
	private string message;
}
