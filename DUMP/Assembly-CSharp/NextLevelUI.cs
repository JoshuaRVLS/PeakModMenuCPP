using System;
using TMPro;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public class NextLevelUI : MonoBehaviour
{
	// Token: 0x06000EFB RID: 3835 RVA: 0x0004A9B2 File Offset: 0x00048BB2
	private void Start()
	{
		this.nextLevelService = GameHandler.GetService<NextLevelService>();
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x0004A9C0 File Offset: 0x00048BC0
	private void Update()
	{
		if (this.nextLevelService.Data.IsSome)
		{
			this.timer.text = this.ParseSeconds(this.nextLevelService.Data.Value.SecondsLeft);
			return;
		}
		this.timer.text = this.ParseSeconds(this.nextLevelService.SecondsLeftFallback);
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x0004AA28 File Offset: 0x00048C28
	public string ParseSeconds(int seconds)
	{
		if (seconds < 0)
		{
			return "-- -- --";
		}
		int num = Mathf.FloorToInt((float)seconds / 3600f);
		int num2 = Mathf.FloorToInt((float)(seconds - num * 3600) / 60f);
		float num3 = (float)(seconds - (num * 3600 + num2 * 60));
		return string.Format("{0}h {1}m {2}s", num, num2, num3);
	}

	// Token: 0x04000CB4 RID: 3252
	public TextMeshProUGUI timer;

	// Token: 0x04000CB5 RID: 3253
	private NextLevelService nextLevelService;
}
