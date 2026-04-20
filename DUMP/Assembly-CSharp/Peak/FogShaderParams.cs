using System;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003C6 RID: 966
	public class FogShaderParams : MonoBehaviour
	{
		// Token: 0x06001992 RID: 6546 RVA: 0x00081824 File Offset: 0x0007FA24
		private void Start()
		{
			Shader.SetGlobalTexture("FogTexture", this.fogTexture);
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x00081836 File Offset: 0x0007FA36
		private void Update()
		{
		}

		// Token: 0x0400173F RID: 5951
		private MeshRenderer mr;

		// Token: 0x04001740 RID: 5952
		public Texture fogTexture;
	}
}
