using System;
using UnityEngine;

// Token: 0x02000257 RID: 599
public class DisableIfWindActive : MonoBehaviour
{
	// Token: 0x060011FF RID: 4607 RVA: 0x0005A754 File Offset: 0x00058954
	private void FixedUpdate()
	{
		if (RootsWind.instance && this.gameObjectToDisable)
		{
			if (RootsWind.instance.windZone.windActive)
			{
				this.gameObjectToDisable.SetActive(false);
				return;
			}
			if (!this.disablePermanently)
			{
				this.gameObjectToDisable.SetActive(true);
			}
		}
	}

	// Token: 0x04001003 RID: 4099
	public GameObject gameObjectToDisable;

	// Token: 0x04001004 RID: 4100
	public bool disablePermanently;
}
