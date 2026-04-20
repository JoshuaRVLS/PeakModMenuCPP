using System;
using System.Collections.Generic;
using Zorro.Core;

namespace Peak.ProcGen
{
	// Token: 0x020003E8 RID: 1000
	[Serializable]
	public class BiomeSelection
	{
		// Token: 0x06001A8D RID: 6797 RVA: 0x000831B9 File Offset: 0x000813B9
		public int GetSelectionResult(out BiomeSelectionOption biomeResult)
		{
			biomeResult = this.biomeOptions.SelectRandomWeighted((BiomeSelectionOption biome) => biome.weight);
			return this.biomeOptions.IndexOf(biomeResult);
		}

		// Token: 0x04001778 RID: 6008
		public List<BiomeSelectionOption> biomeOptions = new List<BiomeSelectionOption>();
	}
}
