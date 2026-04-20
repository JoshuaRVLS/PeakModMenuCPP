using System;
using UnityEngine;

// Token: 0x02000336 RID: 822
public class setright : MonoBehaviour
{
	// Token: 0x060015D0 RID: 5584 RVA: 0x0006E8CB File Offset: 0x0006CACB
	private void Start()
	{
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x0006E8CD File Offset: 0x0006CACD
	private void Update()
	{
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x0006E8CF File Offset: 0x0006CACF
	public void go()
	{
		base.transform.right = this.right;
		base.transform.up = this.up;
	}

	// Token: 0x040013D7 RID: 5079
	public Vector3 right;

	// Token: 0x040013D8 RID: 5080
	public Vector3 up;
}
