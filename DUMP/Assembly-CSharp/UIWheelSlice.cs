using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001F3 RID: 499
public class UIWheelSlice : MonoBehaviour
{
	// Token: 0x06000FC2 RID: 4034 RVA: 0x0004D5F5 File Offset: 0x0004B7F5
	public Vector3 GetUpVector()
	{
		return Quaternion.Euler(0f, 0f, this.offsetRotation) * base.transform.up;
	}

	// Token: 0x04000D5F RID: 3423
	public Button button;

	// Token: 0x04000D60 RID: 3424
	private float offsetRotation = 22.5f;
}
