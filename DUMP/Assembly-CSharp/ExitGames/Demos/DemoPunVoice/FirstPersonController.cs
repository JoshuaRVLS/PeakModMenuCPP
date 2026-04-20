using System;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000396 RID: 918
	public class FirstPersonController : BaseController
	{
		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06001839 RID: 6201 RVA: 0x0007AF10 File Offset: 0x00079110
		public Vector3 Velocity
		{
			get
			{
				return this.rigidBody.linearVelocity;
			}
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x0007AF1D File Offset: 0x0007911D
		protected override void SetCamera()
		{
			base.SetCamera();
			this.mouseLook.Init(base.transform, this.camTrans);
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x0007AF3C File Offset: 0x0007913C
		protected override void Move(float h, float v)
		{
			Vector3 vector = this.camTrans.forward * v + this.camTrans.right * h;
			vector.x *= this.speed;
			vector.z *= this.speed;
			vector.y = 0f;
			this.rigidBody.linearVelocity = vector;
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x0007AFB1 File Offset: 0x000791B1
		private void Update()
		{
			this.RotateView();
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x0007AFBC File Offset: 0x000791BC
		private void RotateView()
		{
			this.oldYRotation = base.transform.eulerAngles.y;
			this.mouseLook.LookRotation(base.transform, this.camTrans);
			this.velRotation = Quaternion.AngleAxis(base.transform.eulerAngles.y - this.oldYRotation, Vector3.up);
			this.rigidBody.linearVelocity = this.velRotation * this.rigidBody.linearVelocity;
		}

		// Token: 0x0400164C RID: 5708
		[SerializeField]
		private MouseLookHelper mouseLook = new MouseLookHelper();

		// Token: 0x0400164D RID: 5709
		private float oldYRotation;

		// Token: 0x0400164E RID: 5710
		private Quaternion velRotation;
	}
}
