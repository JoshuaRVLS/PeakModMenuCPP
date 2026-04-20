using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class MushroomZombieSpawner : MonoBehaviour
{
	// Token: 0x06000AC7 RID: 2759 RVA: 0x00039784 File Offset: 0x00037984
	private void OnDestroy()
	{
		if (ZombieManager.Instance)
		{
			ZombieManager.Instance.spawners.Remove(this);
		}
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x000397A4 File Offset: 0x000379A4
	private void Start()
	{
		if (!Ascents.shouldSpawnZombie)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (this.cullRandomly && Random.Range(0f, 1f) > 0.333f)
		{
			Debug.Log("Destroying zombie spawner.");
			Object.Destroy(base.gameObject);
			return;
		}
		if (!ZombieManager.Instance.spawners.Contains(this))
		{
			ZombieManager.Instance.spawners.Add(this);
		}
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x0003981C File Offset: 0x00037A1C
	public bool ReadyToSpawn()
	{
		if (this.spawnedZombie != null)
		{
			return false;
		}
		if (this.spawned)
		{
			return false;
		}
		using (List<Character>.Enumerator enumerator = Character.AllCharacters.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.Center, base.transform.position) <= this.mushroomZombiePrefab.distanceToEnable)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x000398AC File Offset: 0x00037AAC
	public void Spawn()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.spawned = true;
		Debug.Log("Spawning new zombie");
		this.spawnedZombie = PhotonNetwork.Instantiate(this.mushroomZombiePrefab.gameObject.name, base.transform.position, base.transform.rotation, 0, null).GetComponent<MushroomZombie>();
		this.spawnedZombie.spawner = this;
	}

	// Token: 0x04000A0E RID: 2574
	public MushroomZombie mushroomZombiePrefab;

	// Token: 0x04000A0F RID: 2575
	public MushroomZombie spawnedZombie;

	// Token: 0x04000A10 RID: 2576
	public const float SPAWN_CHANCE = 0.333f;

	// Token: 0x04000A11 RID: 2577
	public bool cullRandomly = true;

	// Token: 0x04000A12 RID: 2578
	private bool spawned;
}
