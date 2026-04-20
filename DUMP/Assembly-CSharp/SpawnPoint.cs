using System;
using System.Collections.Generic;
using System.Linq;
using Peak.Network;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class SpawnPoint : MonoBehaviour
{
	// Token: 0x1700004C RID: 76
	// (get) Token: 0x060003C1 RID: 961 RVA: 0x000190A9 File Offset: 0x000172A9
	public static SpawnPoint LocalSpawnPoint
	{
		get
		{
			return SpawnPoint.GetSpawnPoint(NetCode.Session.SeatNumber);
		}
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x000190BC File Offset: 0x000172BC
	public static SpawnPoint GetSpawnPoint(int actorNumber)
	{
		int spawnIndex = (actorNumber - 1) % SpawnPoint.allSpawnPoints.Count + 1;
		SpawnPoint spawnPoint = SpawnPoint.allSpawnPoints.FirstOrDefault((SpawnPoint s) => s.index == spawnIndex);
		if (spawnPoint == null)
		{
			spawnPoint = SpawnPoint.allSpawnPoints.FirstOrDefault<SpawnPoint>();
		}
		if (spawnPoint == null)
		{
			Debug.LogError("No spawn points in this scene! Creating a dummy one but it will suck.");
			spawnPoint = new GameObject("DummySpawnPoint").AddComponent<SpawnPoint>();
		}
		return spawnPoint;
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x0001912E File Offset: 0x0001732E
	private void Awake()
	{
		SpawnPoint.allSpawnPoints.Add(this);
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x0001913B File Offset: 0x0001733B
	private void OnDestroy()
	{
		SpawnPoint.allSpawnPoints.Remove(this);
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x00019149 File Offset: 0x00017349
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}

	// Token: 0x04000409 RID: 1033
	public int index;

	// Token: 0x0400040A RID: 1034
	public bool startPassedOut;

	// Token: 0x0400040B RID: 1035
	public static List<SpawnPoint> allSpawnPoints = new List<SpawnPoint>();
}
