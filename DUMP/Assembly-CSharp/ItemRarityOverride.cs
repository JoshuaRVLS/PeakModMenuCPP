using System;
using UnityEngine.Serialization;

// Token: 0x020002A9 RID: 681
[Serializable]
public class ItemRarityOverride
{
	// Token: 0x0400118B RID: 4491
	public Rarity Rarity;

	// Token: 0x0400118C RID: 4492
	[FormerlySerializedAs("spawnType")]
	public SpawnPool spawnPool;
}
