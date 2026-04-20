using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020002A3 RID: 675
public class LocalPlayerRenderer : MonoBehaviour
{
	// Token: 0x06001337 RID: 4919 RVA: 0x00060EC4 File Offset: 0x0005F0C4
	private void Start()
	{
		Character componentInParent = base.GetComponentInParent<Character>();
		if (componentInParent && componentInParent.IsLocal)
		{
			base.GetComponent<MeshRenderer>().shadowCastingMode = this.renderMode;
		}
	}

	// Token: 0x0400114B RID: 4427
	public ShadowCastingMode renderMode;
}
