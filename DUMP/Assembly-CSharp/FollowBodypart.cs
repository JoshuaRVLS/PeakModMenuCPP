using System;
using UnityEngine;

// Token: 0x0200026F RID: 623
public class FollowBodypart : MonoBehaviour
{
	// Token: 0x06001255 RID: 4693 RVA: 0x0005BE42 File Offset: 0x0005A042
	private void Start()
	{
		this.target = base.GetComponentInParent<Character>().GetBodypart(this.followPart).transform;
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x0005BE60 File Offset: 0x0005A060
	private void LateUpdate()
	{
		base.transform.position = this.target.position;
	}

	// Token: 0x0400106A RID: 4202
	public BodypartType followPart;

	// Token: 0x0400106B RID: 4203
	private Transform target;
}
