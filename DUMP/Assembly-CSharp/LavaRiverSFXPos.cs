using System;
using UnityEngine;

// Token: 0x02000294 RID: 660
public class LavaRiverSFXPos : MonoBehaviour
{
	// Token: 0x06001312 RID: 4882 RVA: 0x00060A00 File Offset: 0x0005EC00
	private void Update()
	{
		if (MainCamera.instance)
		{
			base.transform.position = new Vector3(MainCamera.instance.transform.position.x, base.transform.position.y, MainCamera.instance.transform.position.z);
			if (base.transform.position.z < 1050f)
			{
				base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 1050f);
			}
		}
	}
}
