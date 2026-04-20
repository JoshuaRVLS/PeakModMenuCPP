using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x02000384 RID: 900
	[AttributeUsage(AttributeTargets.Class)]
	public class ModifierID : Attribute
	{
		// Token: 0x0600177A RID: 6010 RVA: 0x00079132 File Offset: 0x00077332
		public ModifierID(string name)
		{
			this.name = name;
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x0600177B RID: 6011 RVA: 0x00079141 File Offset: 0x00077341
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x040015F3 RID: 5619
		private string name;
	}
}
