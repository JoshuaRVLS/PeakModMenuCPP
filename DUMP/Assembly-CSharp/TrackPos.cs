using System;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class TrackPos : MonoBehaviour
{
	// Token: 0x06000E40 RID: 3648 RVA: 0x0004781E File Offset: 0x00045A1E
	private void Start()
	{
		this.startPos = base.transform.position;
		this.startRot = base.transform.rotation;
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x00047844 File Offset: 0x00045A44
	private void Update()
	{
		if (this.trackPos)
		{
			base.transform.position = this.trackTransform.position + this.startPos;
		}
		if (this.trackRot)
		{
			base.transform.rotation = this.trackTransform.rotation * this.startRot;
		}
	}

	// Token: 0x04000BFD RID: 3069
	public Transform trackTransform;

	// Token: 0x04000BFE RID: 3070
	private Vector3 startPos;

	// Token: 0x04000BFF RID: 3071
	private Quaternion startRot;

	// Token: 0x04000C00 RID: 3072
	public bool trackPos;

	// Token: 0x04000C01 RID: 3073
	public bool trackRot;
}
