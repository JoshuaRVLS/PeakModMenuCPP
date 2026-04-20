using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000328 RID: 808
public class RotationSprings : MonoBehaviour
{
	// Token: 0x0600157E RID: 5502 RVA: 0x0006CD80 File Offset: 0x0006AF80
	private void Update()
	{
		Transform parent = base.transform.parent;
		Vector3 forward = parent.forward;
		Vector3 up = parent.up;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < this.springs.Count; i++)
		{
			this.springs[i].DoUpdate(forward, up);
		}
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x0006CDD4 File Offset: 0x0006AFD4
	public void AddForce(Vector3 force, float spring, float drag)
	{
		RotationSprings.RotationSpringInstance rotationSpringInstance = new RotationSprings.RotationSpringInstance();
		rotationSpringInstance.spring = spring;
		rotationSpringInstance.drag = drag;
		rotationSpringInstance.forward = base.transform.parent.forward;
		rotationSpringInstance.up = base.transform.parent.up;
	}

	// Token: 0x040013A8 RID: 5032
	public List<RotationSprings.RotationSpringInstance> springs = new List<RotationSprings.RotationSpringInstance>();

	// Token: 0x02000535 RID: 1333
	[Serializable]
	public class RotationSpringInstance
	{
		// Token: 0x06001F2B RID: 7979 RVA: 0x0008EDEC File Offset: 0x0008CFEC
		public void DoUpdate(Vector3 targetForward, Vector3 targetUp)
		{
			Vector3 a = Vector3.Cross(this.forward, targetForward) * Vector3.Angle(this.forward, targetForward);
			a += Vector3.Cross(this.up, targetUp) * Vector3.Angle(this.up, targetUp);
			this.vel = FRILerp.Lerp(this.vel, a * this.spring, this.drag, true);
			this.forward = Quaternion.AngleAxis(Time.deltaTime * this.vel.magnitude, this.vel) * this.forward;
			this.up = Quaternion.AngleAxis(Time.deltaTime * this.vel.magnitude, this.vel) * this.up;
		}

		// Token: 0x04001C97 RID: 7319
		public float spring;

		// Token: 0x04001C98 RID: 7320
		public float drag;

		// Token: 0x04001C99 RID: 7321
		public Vector3 vel;

		// Token: 0x04001C9A RID: 7322
		public Vector3 forward;

		// Token: 0x04001C9B RID: 7323
		public Vector3 up;
	}
}
