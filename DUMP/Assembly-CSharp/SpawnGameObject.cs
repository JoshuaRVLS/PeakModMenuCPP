using System;
using UnityEngine;

// Token: 0x02000346 RID: 838
public class SpawnGameObject : MonoBehaviour
{
	// Token: 0x06001649 RID: 5705 RVA: 0x0007127A File Offset: 0x0006F47A
	public void Go()
	{
		Object.Instantiate<GameObject>(this.toSpawn, base.transform.position, base.transform.rotation);
	}

	// Token: 0x0400145B RID: 5211
	public GameObject toSpawn;
}
