using System;
using UnityEngine;

// Token: 0x02000142 RID: 322
public class Beetle : Mob
{
	// Token: 0x06000A91 RID: 2705 RVA: 0x000383DC File Offset: 0x000365DC
	protected override void InflictAttack(Character character)
	{
		if (character.IsLocal)
		{
			character.Fall(this.ragdollTime, 0f);
			character.AddForceAtPosition(base.transform.forward * this.bonkForce + base.transform.up * this.bonkForceUp, this.bonkPoint.position, this.bonkRange);
		}
	}

	// Token: 0x040009CE RID: 2510
	public float bonkForce = 100f;

	// Token: 0x040009CF RID: 2511
	public float bonkForceUp = 100f;

	// Token: 0x040009D0 RID: 2512
	public float bonkRange = 4f;

	// Token: 0x040009D1 RID: 2513
	public float ragdollTime = 2f;

	// Token: 0x040009D2 RID: 2514
	public Transform bonkPoint;
}
