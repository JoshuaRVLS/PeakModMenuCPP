using System;

// Token: 0x020000F2 RID: 242
public class Action_OverrideCamera : ItemAction
{
	// Token: 0x06000897 RID: 2199 RVA: 0x0002F9C1 File Offset: 0x0002DBC1
	public override void RunAction()
	{
		MainCamera.instance.SetCameraOverride(this.cameraOverride);
	}

	// Token: 0x04000831 RID: 2097
	public CameraOverride cameraOverride;
}
