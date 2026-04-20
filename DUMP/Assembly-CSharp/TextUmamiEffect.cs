using System;
using UnityEngine;

// Token: 0x020001F1 RID: 497
public class TextUmamiEffect : DialogueEffect
{
	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000FBA RID: 4026 RVA: 0x0004D49C File Offset: 0x0004B69C
	public virtual float colorSpeedMult
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x0004D4A4 File Offset: 0x0004B6A4
	public override void UpdateCharacter(int index)
	{
		float num = this.offset * (float)index;
		float num2 = Mathf.Sin((Time.time + num) / this.period);
		float d = 1f + num2 * this.amplitude;
		Vector3 scale = Vector3.one * d;
		this.DTanimator.SetCharScale(index, scale);
		this.DTanimator.SetCharOffset(index, Vector3.up * d * this.charOffset);
		float time = (Mathf.Sin((Time.time + num) / (this.period / this.colorSpeedMult)) + 1f) * 0.5f;
		this.DTanimator.SetCharColor(index, this.colorGradient.Evaluate(time));
	}

	// Token: 0x04000D58 RID: 3416
	public bool abs;

	// Token: 0x04000D59 RID: 3417
	public float amplitude = 0.2f;

	// Token: 0x04000D5A RID: 3418
	public float period = 0.5f;

	// Token: 0x04000D5B RID: 3419
	public float offset = 0.1f;

	// Token: 0x04000D5C RID: 3420
	public float charOffset = 10f;

	// Token: 0x04000D5D RID: 3421
	public Gradient colorGradient;
}
