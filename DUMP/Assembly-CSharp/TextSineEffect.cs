using System;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class TextSineEffect : DialogueEffect
{
	// Token: 0x06000FB8 RID: 4024 RVA: 0x0004D404 File Offset: 0x0004B604
	public override void UpdateCharacter(int index)
	{
		float num = this.offset * (float)index;
		Vector3 vector = Vector3.up * (Mathf.Sin((Time.time + num) / this.period) * this.amplitude);
		if (this.abs)
		{
			vector = new Vector3(vector.x, Mathf.Abs(vector.y), vector.z);
		}
		this.DTanimator.SetCharOffset(index, vector);
	}

	// Token: 0x04000D54 RID: 3412
	public bool abs;

	// Token: 0x04000D55 RID: 3413
	public float amplitude = 3f;

	// Token: 0x04000D56 RID: 3414
	public float period = 0.15f;

	// Token: 0x04000D57 RID: 3415
	public float offset = 0.1f;
}
