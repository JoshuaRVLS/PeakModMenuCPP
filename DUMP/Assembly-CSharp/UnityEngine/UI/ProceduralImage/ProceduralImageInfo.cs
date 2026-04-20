using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x02000386 RID: 902
	public struct ProceduralImageInfo
	{
		// Token: 0x06001795 RID: 6037 RVA: 0x000796B8 File Offset: 0x000778B8
		public ProceduralImageInfo(float width, float height, float fallOffDistance, float pixelSize, Vector4 radius, float borderWidth)
		{
			this.width = Mathf.Abs(width);
			this.height = Mathf.Abs(height);
			this.fallOffDistance = Mathf.Max(0f, fallOffDistance);
			this.radius = radius;
			this.borderWidth = Mathf.Max(borderWidth, 0f);
			this.pixelSize = Mathf.Max(0f, pixelSize);
		}

		// Token: 0x040015F8 RID: 5624
		public float width;

		// Token: 0x040015F9 RID: 5625
		public float height;

		// Token: 0x040015FA RID: 5626
		public float fallOffDistance;

		// Token: 0x040015FB RID: 5627
		public Vector4 radius;

		// Token: 0x040015FC RID: 5628
		public float borderWidth;

		// Token: 0x040015FD RID: 5629
		public float pixelSize;
	}
}
