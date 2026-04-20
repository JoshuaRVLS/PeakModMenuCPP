using System;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x020003A1 RID: 929
	public struct MicRef
	{
		// Token: 0x060018B6 RID: 6326 RVA: 0x0007D5D3 File Offset: 0x0007B7D3
		public MicRef(MicType micType, DeviceInfo device)
		{
			this.MicType = micType;
			this.Device = device;
		}

		// Token: 0x060018B7 RID: 6327 RVA: 0x0007D5E3 File Offset: 0x0007B7E3
		public override string ToString()
		{
			return string.Format("Mic reference: {0}", this.Device.Name);
		}

		// Token: 0x040016B3 RID: 5811
		public readonly MicType MicType;

		// Token: 0x040016B4 RID: 5812
		public readonly DeviceInfo Device;
	}
}
