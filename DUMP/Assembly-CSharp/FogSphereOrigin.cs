using System;
using UnityEngine;

// Token: 0x0200026E RID: 622
public class FogSphereOrigin : MonoBehaviour
{
	// Token: 0x06001253 RID: 4691 RVA: 0x0005BDB0 File Offset: 0x00059FB0
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawCube(new Vector3(0f, this.moveOnHeight, this.moveOnForward), new Vector3(200f, 200f, 2f));
		Gizmos.color = Color.green;
		Gizmos.DrawCube(new Vector3(0f, this.moveOnHeight, this.moveOnForward), new Vector3(200f, 2f, 1000f));
	}

	// Token: 0x04001066 RID: 4198
	public float size = 650f;

	// Token: 0x04001067 RID: 4199
	public float moveOnHeight;

	// Token: 0x04001068 RID: 4200
	public float moveOnForward;

	// Token: 0x04001069 RID: 4201
	public bool disableFog;
}
