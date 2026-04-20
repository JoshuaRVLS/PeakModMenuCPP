using System;
using UnityEngine;

// Token: 0x0200024B RID: 587
public abstract class CustomSpawnCondition : MonoBehaviour
{
	// Token: 0x060011CF RID: 4559
	public abstract bool CheckCondition(PropSpawner.SpawnData data);
}
