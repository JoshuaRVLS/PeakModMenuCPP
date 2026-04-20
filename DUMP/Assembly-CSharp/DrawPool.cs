using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001AF RID: 431
[Serializable]
public class DrawPool
{
	// Token: 0x04000BCC RID: 3020
	public Material material;

	// Token: 0x04000BCD RID: 3021
	public Mesh mesh;

	// Token: 0x04000BCE RID: 3022
	[HideInInspector]
	public Matrix4x4[] matricies;

	// Token: 0x04000BCF RID: 3023
	[FormerlySerializedAs("pool")]
	public GameObject transformsParent;
}
