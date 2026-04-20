using System;
using UnityEngine;

// Token: 0x020002FF RID: 767
[Serializable]
public abstract class PropSpawnerConstraint
{
	// Token: 0x17000158 RID: 344
	// (get) Token: 0x060014DA RID: 5338 RVA: 0x000695E1 File Offset: 0x000677E1
	public bool Muted
	{
		get
		{
			return this.mute;
		}
	}

	// Token: 0x060014DB RID: 5339
	public abstract bool CheckConstraint(PropSpawner.SpawnData spawnData);

	// Token: 0x060014DC RID: 5340 RVA: 0x000695E9 File Offset: 0x000677E9
	public virtual bool Validate(GameObject _, PropSpawner.SpawnData spawnData)
	{
		return this.CheckConstraint(spawnData);
	}

	// Token: 0x04001303 RID: 4867
	public bool mute;
}
