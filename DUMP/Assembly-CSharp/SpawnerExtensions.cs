using System;
using Peak;
using UnityEngine;

// Token: 0x020000CE RID: 206
public static class SpawnerExtensions
{
	// Token: 0x0600081D RID: 2077 RVA: 0x0002DEB0 File Offset: 0x0002C0B0
	public static bool HasSpawnTracking(this ISpawner self, out SpawnedItemTracker tracker)
	{
		tracker = null;
		Component component = self as Component;
		return component != null && component.TryGetComponent<SpawnedItemTracker>(out tracker);
	}
}
