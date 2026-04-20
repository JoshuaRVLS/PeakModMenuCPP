using System;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000399 RID: 921
	public class ThirdPersonController : BaseController
	{
		// Token: 0x0600184C RID: 6220 RVA: 0x0007B2B4 File Offset: 0x000794B4
		protected override void Move(float h, float v)
		{
			this.rigidBody.linearVelocity = v * this.speed * base.transform.forward;
			base.transform.rotation *= Quaternion.AngleAxis(this.movingTurnSpeed * h * Time.deltaTime, Vector3.up);
		}

		// Token: 0x04001657 RID: 5719
		[SerializeField]
		private float movingTurnSpeed = 360f;
	}
}
