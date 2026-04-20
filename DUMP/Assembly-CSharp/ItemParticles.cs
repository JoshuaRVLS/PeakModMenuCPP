using System;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class ItemParticles : MonoBehaviour
{
	// Token: 0x060002DB RID: 731 RVA: 0x0001455B File Offset: 0x0001275B
	public void EnableSmoke(bool active)
	{
		if (this.smoke)
		{
			if (active)
			{
				this.smoke.Play();
				return;
			}
			this.smoke.Stop();
		}
	}

	// Token: 0x040002A4 RID: 676
	public ParticleSystem smoke;
}
