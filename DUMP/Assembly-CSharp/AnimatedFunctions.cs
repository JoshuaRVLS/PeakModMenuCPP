using System;
using UnityEngine;

// Token: 0x02000203 RID: 515
public class AnimatedFunctions : MonoBehaviour
{
	// Token: 0x0600100E RID: 4110 RVA: 0x0004F6A7 File Offset: 0x0004D8A7
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x0004F6B5 File Offset: 0x0004D8B5
	private void Start()
	{
		this.left = this.character.GetBodypart(BodypartType.Foot_L).GetComponentInChildren<RigCreatorCollider>().GetComponent<Collider>();
		this.right = this.character.GetBodypart(BodypartType.Foot_R).GetComponentInChildren<RigCreatorCollider>().GetComponent<Collider>();
	}

	// Token: 0x04000DE9 RID: 3561
	private Collider left;

	// Token: 0x04000DEA RID: 3562
	private Collider right;

	// Token: 0x04000DEB RID: 3563
	private Character character;
}
