using System;
using UnityEngine;

// Token: 0x020002AD RID: 685
public class MirrorScript : MonoBehaviour
{
	// Token: 0x040011A8 RID: 4520
	[Tooltip("Maximum number of per pixel lights that will show in the mirrored image")]
	public int MaximumPerPixelLights = 2;

	// Token: 0x040011A9 RID: 4521
	[Tooltip("Texture size for the mirror, depending on how close the player can get to the mirror, this will need to be larger")]
	public int TextureSize = 768;

	// Token: 0x040011AA RID: 4522
	[Tooltip("Subtracted from the near plane of the mirror")]
	public float ClipPlaneOffset = 0.07f;

	// Token: 0x040011AB RID: 4523
	[Tooltip("Far clip plane for mirro camera")]
	public float FarClipPlane = 1000f;

	// Token: 0x040011AC RID: 4524
	[Tooltip("What layers will be reflected?")]
	public LayerMask ReflectLayers = -1;

	// Token: 0x040011AD RID: 4525
	[Tooltip("Add a flare layer to the reflection camera?")]
	public bool AddFlareLayer;

	// Token: 0x040011AE RID: 4526
	[Tooltip("For quads, the normal points forward (true). For planes, the normal points up (false)")]
	public bool NormalIsForward = true;

	// Token: 0x040011AF RID: 4527
	[Tooltip("Aspect ratio (width / height). Set to 0 to use default.")]
	public float AspectRatio;

	// Token: 0x040011B0 RID: 4528
	[Tooltip("Set to true if you have multiple mirrors facing each other to get an infinite effect, otherwise leave as false for a more realistic mirror effect.")]
	public bool MirrorRecursion;
}
