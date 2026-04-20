using System;
using UnityEngine;

// Token: 0x02000070 RID: 112
public class CastToGround : MonoBehaviour
{
	// Token: 0x06000523 RID: 1315 RVA: 0x0001E558 File Offset: 0x0001C758
	private void Start()
	{
		if (this.castOnStart)
		{
			this.castToGround();
		}
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x0001E568 File Offset: 0x0001C768
	public void castToGround()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit))
		{
			base.transform.position = raycastHit.point + this.offset;
			base.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
		}
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x0001E5C7 File Offset: 0x0001C7C7
	private void Update()
	{
	}

	// Token: 0x04000576 RID: 1398
	public bool castOnStart = true;

	// Token: 0x04000577 RID: 1399
	public Vector3 offset;
}
