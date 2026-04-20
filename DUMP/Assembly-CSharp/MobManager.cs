using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002B2 RID: 690
public class MobManager : MonoBehaviour
{
	// Token: 0x06001385 RID: 4997 RVA: 0x00063399 File Offset: 0x00061599
	private void Awake()
	{
		MobManager.instance = this;
	}

	// Token: 0x06001386 RID: 4998 RVA: 0x000633A1 File Offset: 0x000615A1
	private void Update()
	{
		if (this.mobs.Count == 0)
		{
			return;
		}
		this.mobs[this.currentIndex].TestSleepMode();
		this.currentIndex = (this.currentIndex + 1) % this.mobs.Count;
	}

	// Token: 0x06001387 RID: 4999 RVA: 0x000633E1 File Offset: 0x000615E1
	public void Register(Mob mob)
	{
		if (!this.mobs.Contains(mob))
		{
			this.mobs.Add(mob);
		}
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x000633FD File Offset: 0x000615FD
	public void Unregister(Mob mob)
	{
		this.mobs.Remove(mob);
		if (this.currentIndex >= this.mobs.Count)
		{
			this.currentIndex = 0;
		}
	}

	// Token: 0x040011E6 RID: 4582
	public static MobManager instance;

	// Token: 0x040011E7 RID: 4583
	public List<Mob> mobs = new List<Mob>();

	// Token: 0x040011E8 RID: 4584
	private int currentIndex;
}
