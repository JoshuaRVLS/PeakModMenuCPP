using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A0 RID: 672
[ExecuteAlways]
public class LoadingScreenAnimation : MonoBehaviour
{
	// Token: 0x0600132F RID: 4911 RVA: 0x00060C94 File Offset: 0x0005EE94
	private void Start()
	{
		string text = LocalizedText.GetText("LOADING", true);
		if (LocalizedText.CURRENT_LANGUAGE == LocalizedText.Language.SimplifiedChinese)
		{
			this.loadingString = string.Concat(new string[]
			{
				text,
				"...",
				text,
				"...",
				text,
				"...",
				text,
				"..."
			});
			this.defaultLoadingStringLength = (float)this.loadingString.Length;
			return;
		}
		if (LocalizedText.CURRENT_LANGUAGE == LocalizedText.Language.Korean || LocalizedText.CURRENT_LANGUAGE == LocalizedText.Language.Japanese)
		{
			this.loadingString = string.Concat(new string[]
			{
				text,
				"...",
				text,
				"...",
				text,
				"..."
			});
			this.defaultLoadingStringLength = (float)this.loadingString.Length;
			return;
		}
		this.loadingString = string.Concat(new string[]
		{
			text,
			"...",
			text,
			"...",
			text,
			"...",
			text,
			"...",
			text,
			"..."
		});
		this.defaultLoadingStringLength = 50f;
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x00060DC0 File Offset: 0x0005EFC0
	private void Update()
	{
		this.barFill.fillAmount = Mathf.Lerp(this.barFillMinMax.x, this.barFillMinMax.y, this.fillAmount);
		this.planeRotation.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(this.planeRotationMinMax.x, this.planeRotationMinMax.y, this.fillAmount));
		this.loadingText.text = this.loadingString.Substring(0, Mathf.RoundToInt(this.defaultLoadingStringLength * this.fillAmount));
	}

	// Token: 0x0400113F RID: 4415
	public Image barFill;

	// Token: 0x04001140 RID: 4416
	public Transform planeRotation;

	// Token: 0x04001141 RID: 4417
	public TMP_Text loadingText;

	// Token: 0x04001142 RID: 4418
	[Range(0f, 1f)]
	public float fillAmount;

	// Token: 0x04001143 RID: 4419
	public Vector2 barFillMinMax;

	// Token: 0x04001144 RID: 4420
	public Vector2 planeRotationMinMax;

	// Token: 0x04001145 RID: 4421
	private string loadingString;

	// Token: 0x04001146 RID: 4422
	private float defaultLoadingStringLength = 50f;

	// Token: 0x04001147 RID: 4423
	public float maxFill;
}
