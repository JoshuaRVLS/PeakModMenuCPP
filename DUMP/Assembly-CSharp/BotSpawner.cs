using System;
using System.Runtime.CompilerServices;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200006A RID: 106
public class BotSpawner : MonoBehaviour
{
	// Token: 0x0600050F RID: 1295 RVA: 0x0001E07C File Offset: 0x0001C27C
	private void Go()
	{
		this.SpawnBot(PatrolBoss.me.transform.position);
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0001E094 File Offset: 0x0001C294
	public void SpawnBot(Vector3 spawnPosition)
	{
		bool flag = false;
		for (int i = 0; i < 10; i++)
		{
			if (this.<SpawnBot>g__TrySpawnBot|2_0(spawnPosition + ExtMath.RandInsideUnitCircle().xoy() * 2f))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Debug.LogWarning("Could not spawn troop");
		}
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x0001E0EC File Offset: 0x0001C2EC
	[CompilerGenerated]
	private bool <SpawnBot>g__TrySpawnBot|2_0(Vector3 spawnPosition)
	{
		foreach (Collider collider in Physics.OverlapSphere(spawnPosition, 2f))
		{
			if (collider.gameObject.layer != LayerMask.NameToLayer("Terrain") && collider.gameObject.layer != LayerMask.NameToLayer("Prop"))
			{
				return false;
			}
		}
		Object.Instantiate<GameObject>(this.botPrefab, spawnPosition, Quaternion.identity);
		Debug.Log("Spawn Bot");
		return true;
	}

	// Token: 0x0400056A RID: 1386
	public GameObject botPrefab;
}
