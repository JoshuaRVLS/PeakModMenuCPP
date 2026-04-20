using System;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000398 RID: 920
	public class OrthographicController : ThirdPersonController
	{
		// Token: 0x06001847 RID: 6215 RVA: 0x0007B207 File Offset: 0x00079407
		protected override void Init()
		{
			base.Init();
			this.ControllerCamera = Camera.main;
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x0007B21A File Offset: 0x0007941A
		protected override void SetCamera()
		{
			base.SetCamera();
			this.offset = this.camTrans.position - base.transform.position;
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x0007B243 File Offset: 0x00079443
		protected override void Move(float h, float v)
		{
			base.Move(h, v);
			this.CameraFollow();
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x0007B254 File Offset: 0x00079454
		private void CameraFollow()
		{
			Vector3 b = base.transform.position + this.offset;
			this.camTrans.position = Vector3.Lerp(this.camTrans.position, b, this.smoothing * Time.deltaTime);
		}

		// Token: 0x04001655 RID: 5717
		public float smoothing = 5f;

		// Token: 0x04001656 RID: 5718
		private Vector3 offset;
	}
}
