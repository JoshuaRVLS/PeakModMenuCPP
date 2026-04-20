using System;
using UnityEngine;

// Token: 0x02000324 RID: 804
public class RootsWind : MonoBehaviour
{
	// Token: 0x06001574 RID: 5492 RVA: 0x0006C9EA File Offset: 0x0006ABEA
	private void Awake()
	{
		RootsWind.instance = this;
	}

	// Token: 0x04001399 RID: 5017
	public static RootsWind instance;

	// Token: 0x0400139A RID: 5018
	public WindChillZone windZone;
}
