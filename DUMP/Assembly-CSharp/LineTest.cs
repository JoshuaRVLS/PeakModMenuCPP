using System;
using UnityEngine;

// Token: 0x0200029F RID: 671
[ExecuteInEditMode]
public class LineTest : MonoBehaviour
{
	// Token: 0x0600132D RID: 4909 RVA: 0x00060C5B File Offset: 0x0005EE5B
	private void Update()
	{
		this.lr.SetPosition(0, this.start.position);
		this.lr.SetPosition(1, this.end.position);
	}

	// Token: 0x0400113C RID: 4412
	public LineRenderer lr;

	// Token: 0x0400113D RID: 4413
	public Transform start;

	// Token: 0x0400113E RID: 4414
	public Transform end;
}
