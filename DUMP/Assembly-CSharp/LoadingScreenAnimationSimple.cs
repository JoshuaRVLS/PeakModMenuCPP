using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x020002A1 RID: 673
public class LoadingScreenAnimationSimple : MonoBehaviour
{
	// Token: 0x06001332 RID: 4914 RVA: 0x00060E70 File Offset: 0x0005F070
	private void Start()
	{
		base.StartCoroutine(this.AnimateRoutine());
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x00060E7F File Offset: 0x0005F07F
	private IEnumerator AnimateRoutine()
	{
		float dots = 0f;
		for (;;)
		{
			yield return new WaitForSeconds(this.yieldTime);
			if (dots == 0f)
			{
				this.loading.text = LocalizedText.GetText("LOADING", true);
			}
			else if (dots == 1f)
			{
				this.loading.text = LocalizedText.GetText("LOADING", true) + ".";
			}
			else if (dots == 2f)
			{
				this.loading.text = LocalizedText.GetText("LOADING", true) + "..";
			}
			else if (dots == 3f)
			{
				this.loading.text = LocalizedText.GetText("LOADING", true) + "...";
			}
			float num = dots;
			dots = num + 1f;
			if (dots > 3f)
			{
				dots = 0f;
			}
		}
		yield break;
	}

	// Token: 0x04001148 RID: 4424
	public float yieldTime = 1f;

	// Token: 0x04001149 RID: 4425
	public TMP_Text loading;
}
