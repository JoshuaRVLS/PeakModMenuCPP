using System;

namespace Peak.ProcGen
{
	// Token: 0x020003E9 RID: 1001
	[Serializable]
	public class BiomeSelectionOption
	{
		// Token: 0x04001779 RID: 6009
		public Biome.BiomeType biome;

		// Token: 0x0400177A RID: 6010
		public float weight = 1f;

		// Token: 0x0400177B RID: 6011
		public int preventMoreThanXInARow = 2;
	}
}
