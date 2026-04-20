using System;
using UnityEngine;

// Token: 0x02000306 RID: 774
public class PSC_NearObject : PropSpawnerConstraint
{
	// Token: 0x060014ED RID: 5357 RVA: 0x00069B38 File Offset: 0x00067D38
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		this.outVal = this.inverted;
		foreach (Collider collider in Physics.OverlapSphere(spawnData.hit.point, this.radius))
		{
			for (int j = 0; j < this.objects.Length; j++)
			{
				if (collider.transform.parent.name == this.objects[j].name)
				{
					this.outVal = !this.inverted;
				}
			}
		}
		return this.outVal;
	}

	// Token: 0x04001316 RID: 4886
	public bool inverted;

	// Token: 0x04001317 RID: 4887
	public GameObject[] objects;

	// Token: 0x04001318 RID: 4888
	public float radius;

	// Token: 0x04001319 RID: 4889
	private bool outVal;
}
