using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class Arrow : MonoBehaviour
{
	// Token: 0x06000483 RID: 1155 RVA: 0x0001BA5E File Offset: 0x00019C5E
	private void Start()
	{
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x0001BA60 File Offset: 0x00019C60
	public void stuckArrow(bool stuck)
	{
		this.isStuck = stuck;
		this.arrowRB.isKinematic = stuck;
		this.arrowCollider.enabled = !stuck;
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0001BA84 File Offset: 0x00019C84
	private void Update()
	{
		if (base.transform.parent == null && this.isStuck)
		{
			this.stuckArrow(false);
		}
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0001BAA8 File Offset: 0x00019CA8
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x040004F7 RID: 1271
	public bool isStuck = true;

	// Token: 0x040004F8 RID: 1272
	public Rigidbody arrowRB;

	// Token: 0x040004F9 RID: 1273
	public Collider arrowCollider;
}
