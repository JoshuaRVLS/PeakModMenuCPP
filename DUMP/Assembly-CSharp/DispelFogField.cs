using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200025A RID: 602
public class DispelFogField : MonoBehaviour
{
	// Token: 0x06001206 RID: 4614 RVA: 0x0005A814 File Offset: 0x00058A14
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.innerRadius);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(base.transform.position, this.outerRadius);
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x0005A861 File Offset: 0x00058A61
	public void OnDisable()
	{
		Singleton<OrbFogHandler>.Instance.dispelFogAmount = 0f;
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x0005A874 File Offset: 0x00058A74
	public void Update()
	{
		float num = Vector3.Distance(Character.observedCharacter.Center, base.transform.position);
		if (Character.observedCharacter && num <= this.outerRadius)
		{
			Singleton<OrbFogHandler>.Instance.dispelFogAmount = Mathf.InverseLerp(this.outerRadius, this.innerRadius, num);
			return;
		}
		Singleton<OrbFogHandler>.Instance.dispelFogAmount = 0f;
	}

	// Token: 0x04001007 RID: 4103
	public float innerRadius = 7.5f;

	// Token: 0x04001008 RID: 4104
	public float outerRadius = 12.5f;

	// Token: 0x04001009 RID: 4105
	private float lastEnteredTime;

	// Token: 0x0400100A RID: 4106
	private bool inflicting;
}
