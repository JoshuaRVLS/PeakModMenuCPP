using System;
using UnityEngine;

// Token: 0x02000329 RID: 809
public class RotationTest : MonoBehaviour
{
	// Token: 0x06001581 RID: 5505 RVA: 0x0006CE27 File Offset: 0x0006B027
	private void Update()
	{
		base.transform.Rotate(this.refVector.up, Time.deltaTime * 90f, Space.World);
	}

	// Token: 0x040013A9 RID: 5033
	public Transform refVector;
}
