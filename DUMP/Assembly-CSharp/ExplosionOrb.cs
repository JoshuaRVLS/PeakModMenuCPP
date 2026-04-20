using System;
using UnityEngine;

// Token: 0x02000263 RID: 611
internal readonly struct ExplosionOrb
{
	// Token: 0x06001225 RID: 4645 RVA: 0x0005B0AF File Offset: 0x000592AF
	public ExplosionOrb(Vector3 pos, Vector3 dir, float delay = 0f, float size = 1f, float speed = 1f)
	{
		this.position = pos;
		this.direction = dir;
		this.delay = delay;
		this.size = size;
		this.speed = speed;
	}

	// Token: 0x0400102E RID: 4142
	public readonly Vector3 position;

	// Token: 0x0400102F RID: 4143
	public readonly Vector3 direction;

	// Token: 0x04001030 RID: 4144
	public readonly float delay;

	// Token: 0x04001031 RID: 4145
	public readonly float size;

	// Token: 0x04001032 RID: 4146
	public readonly float speed;
}
