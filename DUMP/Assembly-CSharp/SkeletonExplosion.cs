using System;
using UnityEngine;

// Token: 0x02000341 RID: 833
public class SkeletonExplosion : MonoBehaviour
{
	// Token: 0x06001629 RID: 5673 RVA: 0x00070B48 File Offset: 0x0006ED48
	public void Boom(Character character)
	{
		base.transform.forward = character.GetBodypart(BodypartType.Hip).transform.forward;
		base.transform.position = character.Center;
		foreach (Rigidbody rigidbody in this.rb)
		{
			rigidbody.AddExplosionForce(this.force * Random.Range(this.randomForceRange.x, this.randomForceRange.y), this.explosionOrigin.position, this.radius, this.upwardsModifier, ForceMode.Impulse);
			Object.Destroy(rigidbody.gameObject, 10f + Random.value);
		}
	}

	// Token: 0x0400144A RID: 5194
	public Transform explosionOrigin;

	// Token: 0x0400144B RID: 5195
	public Rigidbody[] rb;

	// Token: 0x0400144C RID: 5196
	public float force;

	// Token: 0x0400144D RID: 5197
	public Vector2 randomForceRange;

	// Token: 0x0400144E RID: 5198
	public float radius;

	// Token: 0x0400144F RID: 5199
	public float upwardsModifier;
}
