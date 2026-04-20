using System;
using UnityEngine;

// Token: 0x020002E4 RID: 740
[Serializable]
public abstract class PropSpawnerMod
{
	// Token: 0x060014A5 RID: 5285
	public abstract void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData);

	// Token: 0x040012C5 RID: 4805
	public bool mute;
}
