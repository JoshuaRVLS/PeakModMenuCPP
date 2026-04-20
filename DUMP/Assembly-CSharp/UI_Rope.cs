using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200036F RID: 879
public class UI_Rope : MonoBehaviour
{
	// Token: 0x06001723 RID: 5923 RVA: 0x00076EE9 File Offset: 0x000750E9
	private void OnEnable()
	{
		this.segments = 1;
		this.ropeLength = 1f;
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x00076F00 File Offset: 0x00075100
	private void Update()
	{
		this.ropeLength = Mathf.Lerp(this.ropeLength, (float)this.segments, Time.deltaTime * 5f);
		float num = (Mathf.Max(this.ropeLength, 0f) + this.ropeLengthOffset) * this.ropeLengthMult;
		this.rope.sizeDelta = new Vector2(num, this.rope.sizeDelta.y);
		for (int i = 0; i < this.ropeImages.Length; i++)
		{
			this.ropeImages[i].color = new Color(this.ropeImages[i].color.r, this.ropeImages[i].color.g, this.ropeImages[i].color.b, num * this.ropeLengthAlphaMult - Mathf.Floor(num * this.ropeLengthAlphaMult) + 0.01f);
		}
		bool flag = false;
		for (int j = 0; j < 3; j++)
		{
			this.ropeImages[j].fillAmount = this.ropeSpinA - (this.ropeLength * this.ropeSpinB / this.maxRopeLength - (float)j);
			if (this.ropeImages[j].fillAmount > 0f && !flag)
			{
				flag = true;
				this.ropeEnd.position = this.ropeImages[j].transform.position;
				this.ropeEnd.eulerAngles = this.ropeImages[j].transform.eulerAngles + new Vector3(0f, 0f, this.ropeImages[j].fillAmount * 360f + this.ropeEndOffset);
				this.ropeEndImage.color = new Color(this.ropeImages[j].color.r, this.ropeImages[j].color.g, this.ropeImages[j].color.b, 1f);
			}
		}
		string str = "m";
		int num2 = Mathf.RoundToInt(this.ropeLength * 100f * 0.25f);
		this.ropeLengthText.text = (num2 / 100).ToString() + "." + (num2 % 100).ToString() + str;
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x0007714B File Offset: 0x0007534B
	public void UpdateRope(int newSegments)
	{
		this.segments = newSegments;
	}

	// Token: 0x04001594 RID: 5524
	public RectTransform rope;

	// Token: 0x04001595 RID: 5525
	public float maxRopeLength = 40f;

	// Token: 0x04001596 RID: 5526
	public float ropeLength = 40f;

	// Token: 0x04001597 RID: 5527
	public float ropeLengthOffset;

	// Token: 0x04001598 RID: 5528
	public float ropeLengthMult = 20f;

	// Token: 0x04001599 RID: 5529
	public float ropeLengthAlphaMult;

	// Token: 0x0400159A RID: 5530
	public Image[] ropeImages;

	// Token: 0x0400159B RID: 5531
	private const string M = "m";

	// Token: 0x0400159C RID: 5532
	private const string FT = "ft";

	// Token: 0x0400159D RID: 5533
	public TextMeshProUGUI ropeLengthText;

	// Token: 0x0400159E RID: 5534
	private int segments;

	// Token: 0x0400159F RID: 5535
	public Transform ropeEnd;

	// Token: 0x040015A0 RID: 5536
	public Image ropeEndImage;

	// Token: 0x040015A1 RID: 5537
	public float ropeSpinA = 2f;

	// Token: 0x040015A2 RID: 5538
	public float ropeSpinB = 3f;

	// Token: 0x040015A3 RID: 5539
	public float ropeEndOffset;
}
