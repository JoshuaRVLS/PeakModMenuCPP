using System;
using System.Collections.Generic;
using Photon.Pun;

namespace Peak
{
	// Token: 0x020003C5 RID: 965
	public class EndgameFlareSpawner : SingleItemSpawner
	{
		// Token: 0x06001990 RID: 6544 RVA: 0x00081807 File Offset: 0x0007FA07
		public override List<PhotonView> TrySpawnItems()
		{
			if (!Ascents.shouldSpawnFlare)
			{
				return new List<PhotonView>();
			}
			return base.TrySpawnItems();
		}
	}
}
