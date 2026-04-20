using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x02000387 RID: 903
	[DisallowMultipleComponent]
	public abstract class ProceduralImageModifier : MonoBehaviour
	{
		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06001796 RID: 6038 RVA: 0x0007971A File Offset: 0x0007791A
		protected Graphic _Graphic
		{
			get
			{
				if (this.graphic == null)
				{
					this.graphic = base.GetComponent<Graphic>();
				}
				return this.graphic;
			}
		}

		// Token: 0x06001797 RID: 6039
		public abstract Vector4 CalculateRadius(Rect imageRect);

		// Token: 0x040015FE RID: 5630
		protected Graphic graphic;
	}
}
