using System;
using UnityEngine;

// Token: 0x02000338 RID: 824
public class SFXOnChildCount : MonoBehaviour
{
	// Token: 0x060015D6 RID: 5590 RVA: 0x0006E94A File Offset: 0x0006CB4A
	private void Start()
	{
		this.index = base.transform.childCount;
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x0006E960 File Offset: 0x0006CB60
	private void Update()
	{
		if (this.index != base.transform.childCount)
		{
			for (int i = 0; i < this.sfx.Length; i++)
			{
				this.sfx[i].Play(default(Vector3));
			}
		}
		this.index = base.transform.childCount;
	}

	// Token: 0x040013DC RID: 5084
	public SFX_Instance[] sfx;

	// Token: 0x040013DD RID: 5085
	private int index;
}
