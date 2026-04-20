using System;
using UnityEngine;

// Token: 0x02000319 RID: 793
public class RandomAnimatorSpeed : MonoBehaviour
{
	// Token: 0x0600153A RID: 5434 RVA: 0x0006B27B File Offset: 0x0006947B
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.anim.speed = Random.Range(this.minSpeed, this.maxSpeed);
	}

	// Token: 0x0400134E RID: 4942
	private Animator anim;

	// Token: 0x0400134F RID: 4943
	public float minSpeed = 0.5f;

	// Token: 0x04001350 RID: 4944
	public float maxSpeed = 2f;
}
