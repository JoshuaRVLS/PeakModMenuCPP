using System;
using System.Collections;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001CB RID: 459
public class GoToAirport : MonoBehaviour
{
	// Token: 0x06000EB5 RID: 3765 RVA: 0x00049638 File Offset: 0x00047838
	public void GoFromMainMenu()
	{
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Plane, null, new IEnumerator[]
		{
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f)
		});
	}
}
