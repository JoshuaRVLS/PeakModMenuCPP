using System;
using UnityEngine;

// Token: 0x0200030D RID: 781
[Serializable]
public abstract class PropSpawnerConstraintPost
{
	// Token: 0x1700015A RID: 346
	// (get) Token: 0x060014FB RID: 5371 RVA: 0x00069EC7 File Offset: 0x000680C7
	public bool Muted
	{
		get
		{
			return this.mute;
		}
	}

	// Token: 0x060014FC RID: 5372
	public abstract bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData);

	// Token: 0x060014FD RID: 5373 RVA: 0x00069ECF File Offset: 0x000680CF
	public virtual bool Validate(GameObject spawnedProp, PropSpawner.SpawnData spawnData)
	{
		return this.CheckConstraint(spawnedProp, spawnData);
	}

	// Token: 0x04001326 RID: 4902
	public bool mute;
}
