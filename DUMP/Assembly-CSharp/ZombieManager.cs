using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

// Token: 0x020001FC RID: 508
public class ZombieManager : MonoBehaviourPunCallbacks
{
	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000FEA RID: 4074 RVA: 0x0004DFE0 File Offset: 0x0004C1E0
	public static ZombieManager Instance
	{
		get
		{
			if (GameUtils.instance == null)
			{
				return null;
			}
			if (ZombieManager._instance == null)
			{
				ZombieManager._instance = GameUtils.instance.GetComponent<ZombieManager>();
				if (ZombieManager._instance == null)
				{
					ZombieManager._instance = GameUtils.instance.gameObject.AddComponent<ZombieManager>();
				}
			}
			return ZombieManager._instance;
		}
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x0004E040 File Offset: 0x0004C240
	public void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		for (int i = this.zombies.Count - 1; i >= 0; i--)
		{
			MushroomZombie mushroomZombie = this.zombies[i];
			if (mushroomZombie.ReadyToDisable())
			{
				mushroomZombie.DestroyZombie();
				return;
			}
		}
		if (this.zombies.Count < this.maxActiveZombies)
		{
			for (int j = this.spawners.Count - 1; j >= 0; j--)
			{
				MushroomZombieSpawner mushroomZombieSpawner = this.spawners[j];
				if (mushroomZombieSpawner.ReadyToSpawn())
				{
					mushroomZombieSpawner.Spawn();
					return;
				}
			}
		}
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x0004E0D0 File Offset: 0x0004C2D0
	[PunRPC]
	private void RPC_EnableZombie(int zombieID)
	{
		MushroomZombie component = PhotonNetwork.GetPhotonView(zombieID).GetComponent<MushroomZombie>();
		if (component)
		{
			base.StartCoroutine(this.EnableZombie(component));
		}
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x0004E0FF File Offset: 0x0004C2FF
	private IEnumerator EnableZombie(MushroomZombie zombie)
	{
		yield return null;
		if (zombie)
		{
			zombie.character.Start();
			zombie.StartSleeping();
			zombie.StartCoroutine(zombie.RevealZombie());
		}
		yield break;
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x0004E10E File Offset: 0x0004C30E
	public void RegisterZombie(MushroomZombie zombie)
	{
		if (!this.zombies.Contains(zombie))
		{
			this.zombies.Add(zombie);
		}
	}

	// Token: 0x06000FEF RID: 4079 RVA: 0x0004E12A File Offset: 0x0004C32A
	public void DeRegisterZombie(MushroomZombie zombie)
	{
		this.zombies.Remove(zombie);
	}

	// Token: 0x04000D82 RID: 3458
	private static ZombieManager _instance;

	// Token: 0x04000D83 RID: 3459
	public List<MushroomZombie> zombies = new List<MushroomZombie>();

	// Token: 0x04000D84 RID: 3460
	public List<MushroomZombieSpawner> spawners = new List<MushroomZombieSpawner>();

	// Token: 0x04000D85 RID: 3461
	public int maxActiveZombies;
}
