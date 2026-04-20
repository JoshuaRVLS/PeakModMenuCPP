using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000032 RID: 50
[Serializable]
public class RigPart
{
	// Token: 0x0400032C RID: 812
	[HideInInspector]
	public bool justCreated;

	// Token: 0x0400032D RID: 813
	public BodypartType partType;

	// Token: 0x0400032E RID: 814
	public float mass = 10f;

	// Token: 0x0400032F RID: 815
	public float spring = 10f;

	// Token: 0x04000330 RID: 816
	public Transform transform;

	// Token: 0x04000331 RID: 817
	public List<RigCreatorColliderData> colliders = new List<RigCreatorColliderData>();

	// Token: 0x04000332 RID: 818
	public RigCreatorRigidbody rigHandler;

	// Token: 0x04000333 RID: 819
	public Rigidbody rig;

	// Token: 0x04000334 RID: 820
	public ConfigurableJoint joint;

	// Token: 0x04000335 RID: 821
	public RigCreatorJoint jointHandler;
}
