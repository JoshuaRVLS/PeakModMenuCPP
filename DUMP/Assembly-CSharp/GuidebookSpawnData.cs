using System;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class GuidebookSpawnData : MonoBehaviour
{
	// Token: 0x06000260 RID: 608 RVA: 0x000120DA File Offset: 0x000102DA
	public bool CanSpawnRightNow()
	{
		return Character.localCharacter.Center.y >= this.minHeightToSpawn;
	}

	// Token: 0x04000240 RID: 576
	public float minHeightToSpawn;
}
