using System;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class KnockOutPlayerOnImpact : ItemComponent
{
	// Token: 0x06000971 RID: 2417 RVA: 0x00032AF1 File Offset: 0x00030CF1
	private void Start()
	{
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x00032AF3 File Offset: 0x00030CF3
	private void FixedUpdate()
	{
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x00032AF5 File Offset: 0x00030CF5
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x00032AF7 File Offset: 0x00030CF7
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x040008C9 RID: 2249
	public float knockoutVelocity;

	// Token: 0x040008CA RID: 2250
	public float damage;

	// Token: 0x040008CB RID: 2251
	public float forceMult;
}
