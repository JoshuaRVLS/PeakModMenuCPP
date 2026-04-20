using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// Token: 0x020002CF RID: 719
[Serializable]
public class MouseLookHelper
{
	// Token: 0x06001433 RID: 5171 RVA: 0x000662B7 File Offset: 0x000644B7
	public void Init(Transform character, Transform camera)
	{
		this.m_CharacterTargetRot = character.localRotation;
		this.m_CameraTargetRot = camera.localRotation;
	}

	// Token: 0x06001434 RID: 5172 RVA: 0x000662D4 File Offset: 0x000644D4
	public void LookRotation(Transform character, Transform camera)
	{
		float y = CrossPlatformInputManager.GetAxis("Mouse X") * this.XSensitivity;
		float num = CrossPlatformInputManager.GetAxis("Mouse Y") * this.YSensitivity;
		this.m_CharacterTargetRot *= Quaternion.Euler(0f, y, 0f);
		this.m_CameraTargetRot *= Quaternion.Euler(-num, 0f, 0f);
		if (this.clampVerticalRotation)
		{
			this.m_CameraTargetRot = this.ClampRotationAroundXAxis(this.m_CameraTargetRot);
		}
		if (this.smooth)
		{
			character.localRotation = Quaternion.Slerp(character.localRotation, this.m_CharacterTargetRot, this.smoothTime * Time.deltaTime);
			camera.localRotation = Quaternion.Slerp(camera.localRotation, this.m_CameraTargetRot, this.smoothTime * Time.deltaTime);
			return;
		}
		character.localRotation = this.m_CharacterTargetRot;
		camera.localRotation = this.m_CameraTargetRot;
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x000663CC File Offset: 0x000645CC
	private Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1f;
		float num = 114.59156f * Mathf.Atan(q.x);
		num = Mathf.Clamp(num, this.MinimumX, this.MaximumX);
		q.x = Mathf.Tan(0.008726646f * num);
		return q;
	}

	// Token: 0x04001264 RID: 4708
	public float XSensitivity = 2f;

	// Token: 0x04001265 RID: 4709
	public float YSensitivity = 2f;

	// Token: 0x04001266 RID: 4710
	public bool clampVerticalRotation = true;

	// Token: 0x04001267 RID: 4711
	public float MinimumX = -90f;

	// Token: 0x04001268 RID: 4712
	public float MaximumX = 90f;

	// Token: 0x04001269 RID: 4713
	public bool smooth;

	// Token: 0x0400126A RID: 4714
	public float smoothTime = 5f;

	// Token: 0x0400126B RID: 4715
	private Quaternion m_CharacterTargetRot;

	// Token: 0x0400126C RID: 4716
	private Quaternion m_CameraTargetRot;
}
