using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200030A RID: 778
public class PSC_SurfaceRestrictions : PropSpawnerConstraint
{
	// Token: 0x060014F5 RID: 5365 RVA: 0x00069D84 File Offset: 0x00067F84
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		if ((this.effectedLayers.value & 1 << spawnData.hit.transform.gameObject.layer) != 0)
		{
			for (int i = 0; i < this.whitelistedTagWords.Count; i++)
			{
				if (spawnData.hit.transform.tag.ToUpper().Contains(this.whitelistedTagWords[i].ToUpper()))
				{
					return true;
				}
			}
			return false;
		}
		for (int j = 0; j < this.blacklistedTagWords.Count; j++)
		{
			if (spawnData.hit.transform.tag.ToUpper().Contains(this.blacklistedTagWords[j].ToUpper()))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04001322 RID: 4898
	public LayerMask effectedLayers;

	// Token: 0x04001323 RID: 4899
	public List<string> whitelistedTagWords = new List<string>();

	// Token: 0x04001324 RID: 4900
	public List<string> blacklistedTagWords = new List<string>();
}
