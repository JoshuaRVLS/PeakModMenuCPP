using System;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class CampfireSectionGroundStealer : MonoBehaviour
{
	// Token: 0x06000521 RID: 1313 RVA: 0x0001E4B8 File Offset: 0x0001C6B8
	private void Awake()
	{
		foreach (object obj in this.groundParent.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.GetComponent<MeshRenderer>().bounds.center.y > base.transform.position.y + this.offset)
			{
				transform.SetParent(base.transform, true);
			}
		}
	}

	// Token: 0x04000574 RID: 1396
	public float offset;

	// Token: 0x04000575 RID: 1397
	public GameObject groundParent;
}
