using System;
using UnityEngine;

// Token: 0x0200030C RID: 780
public interface IValidationConstraint
{
	// Token: 0x17000159 RID: 345
	// (get) Token: 0x060014F9 RID: 5369
	bool Muted { get; }

	// Token: 0x060014FA RID: 5370
	bool Validate(GameObject spawnedProp, PropSpawner.SpawnData spawnData);
}
