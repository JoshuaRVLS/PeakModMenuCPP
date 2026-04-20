using System;
using UnityEngine;

// Token: 0x020000A1 RID: 161
public class DynamicBoneColliderBase : MonoBehaviour
{
	// Token: 0x06000645 RID: 1605 RVA: 0x000247C3 File Offset: 0x000229C3
	public virtual bool Collide(ref Vector3 particlePosition, float particleRadius)
	{
		return false;
	}

	// Token: 0x04000658 RID: 1624
	[Tooltip("The axis of the capsule's height.")]
	public DynamicBoneColliderBase.Direction m_Direction = DynamicBoneColliderBase.Direction.Y;

	// Token: 0x04000659 RID: 1625
	[Tooltip("The center of the sphere or capsule, in the object's local space.")]
	public Vector3 m_Center = Vector3.zero;

	// Token: 0x0400065A RID: 1626
	[Tooltip("Constrain bones to outside bound or inside bound.")]
	public DynamicBoneColliderBase.Bound m_Bound;

	// Token: 0x02000447 RID: 1095
	public enum Direction
	{
		// Token: 0x040018D3 RID: 6355
		X,
		// Token: 0x040018D4 RID: 6356
		Y,
		// Token: 0x040018D5 RID: 6357
		Z
	}

	// Token: 0x02000448 RID: 1096
	public enum Bound
	{
		// Token: 0x040018D7 RID: 6359
		Outside,
		// Token: 0x040018D8 RID: 6360
		Inside
	}
}
