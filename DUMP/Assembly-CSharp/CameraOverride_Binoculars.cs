using System;
using UnityEngine;

// Token: 0x02000229 RID: 553
public class CameraOverride_Binoculars : CameraOverride
{
	// Token: 0x060010E6 RID: 4326 RVA: 0x00053FB9 File Offset: 0x000521B9
	private void Start()
	{
		this.lerpedFOV = this.fov;
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x00053FC8 File Offset: 0x000521C8
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(Character.localCharacter.data.lookDirection);
		this.fov = Mathf.Lerp(this.fov, this.lerpedFOV, Time.deltaTime * 5f);
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x00054024 File Offset: 0x00052224
	public void AdjustFOV(float value)
	{
		this.lerpedFOV += value;
		this.lerpedFOV = Mathf.Clamp(this.lerpedFOV, this.minFov, this.maxFov);
	}

	// Token: 0x04000EE9 RID: 3817
	public float minFov;

	// Token: 0x04000EEA RID: 3818
	public float maxFov;

	// Token: 0x04000EEB RID: 3819
	public float fovChangeRate;

	// Token: 0x04000EEC RID: 3820
	public float lerpedFOV;
}
