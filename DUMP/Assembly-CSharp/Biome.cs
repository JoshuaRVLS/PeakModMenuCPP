using System;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class Biome : MonoBehaviour
{
	// Token: 0x04000546 RID: 1350
	public MapHandler mapHandler;

	// Token: 0x04000547 RID: 1351
	public Biome.BiomeType biomeType;

	// Token: 0x02000430 RID: 1072
	public enum BiomeType
	{
		// Token: 0x04001867 RID: 6247
		Shore,
		// Token: 0x04001868 RID: 6248
		Tropics,
		// Token: 0x04001869 RID: 6249
		Alpine,
		// Token: 0x0400186A RID: 6250
		Volcano,
		// Token: 0x0400186B RID: 6251
		Peak = 5,
		// Token: 0x0400186C RID: 6252
		Mesa,
		// Token: 0x0400186D RID: 6253
		Roots,
		// Token: 0x0400186E RID: 6254
		Swamp,
		// Token: 0x0400186F RID: 6255
		Temple,
		// Token: 0x04001870 RID: 6256
		Grasslands,
		// Token: 0x04001871 RID: 6257
		Ocean,
		// Token: 0x04001872 RID: 6258
		Skyblock,
		// Token: 0x04001873 RID: 6259
		Hell,
		// Token: 0x04001874 RID: 6260
		Heaven,
		// Token: 0x04001875 RID: 6261
		Mars,
		// Token: 0x04001876 RID: 6262
		Wisconsin,
		// Token: 0x04001877 RID: 6263
		Void
	}
}
