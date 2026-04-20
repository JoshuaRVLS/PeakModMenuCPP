using System;
using UnityEngine;

// Token: 0x02000228 RID: 552
public class CameraOverride : MonoBehaviour
{
	// Token: 0x060010E4 RID: 4324 RVA: 0x00053F99 File Offset: 0x00052199
	public void DoOverride()
	{
		MainCamera.instance.SetCameraOverride(this);
	}

	// Token: 0x04000EE8 RID: 3816
	public float fov = 35f;
}
