using System;
using UnityEngine;

// Token: 0x020002AE RID: 686
public class MainCamera : MonoBehaviour
{
	// Token: 0x06001368 RID: 4968 RVA: 0x00062A0E File Offset: 0x00060C0E
	private void Awake()
	{
		this.cam = base.GetComponent<Camera>();
		MainCamera.instance = this;
	}

	// Token: 0x06001369 RID: 4969 RVA: 0x00062A22 File Offset: 0x00060C22
	public void SetCameraOverride(CameraOverride setOverride)
	{
		this.camOverride = setOverride;
		this.sinceOverride = 0;
	}

	// Token: 0x0600136A RID: 4970 RVA: 0x00062A32 File Offset: 0x00060C32
	private void Update()
	{
		AudioListener.volume = Mathf.Lerp(AudioListener.volume, 1f, 0.1f * Time.deltaTime);
	}

	// Token: 0x0600136B RID: 4971 RVA: 0x00062A53 File Offset: 0x00060C53
	private void LateUpdate()
	{
	}

	// Token: 0x040011B1 RID: 4529
	public static MainCamera instance;

	// Token: 0x040011B2 RID: 4530
	internal Camera cam;

	// Token: 0x040011B3 RID: 4531
	internal CameraOverride camOverride;

	// Token: 0x040011B4 RID: 4532
	private int sinceOverride = 10;
}
