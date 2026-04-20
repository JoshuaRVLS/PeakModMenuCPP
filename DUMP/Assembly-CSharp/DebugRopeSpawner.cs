using System;
using UnityEngine;

// Token: 0x0200024E RID: 590
public class DebugRopeSpawner : MonoBehaviour
{
	// Token: 0x060011E8 RID: 4584 RVA: 0x0005A338 File Offset: 0x00058538
	public void Spawn()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < this.segments; j++)
		{
			GameObject gameObject = HelperFunctions.SpawnPrefab(this.ropeSegment, base.transform.position + base.transform.up * -this.spacing * (float)j, base.transform.rotation, base.transform);
			if (j > 0)
			{
				gameObject.GetComponent<ConfigurableJoint>().connectedBody = base.transform.GetChild(j - 1).GetComponent<Rigidbody>();
			}
		}
	}

	// Token: 0x04000FF5 RID: 4085
	public GameObject ropeSegment;

	// Token: 0x04000FF6 RID: 4086
	public int segments = 10;

	// Token: 0x04000FF7 RID: 4087
	public float spacing = 0.4f;
}
