using System;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class MirageLuggage : MonoBehaviour
{
	// Token: 0x06000A7D RID: 2685 RVA: 0x00037D0F File Offset: 0x00035F0F
	private void OnEnable()
	{
		this.setMirageState(1f);
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x00037D1C File Offset: 0x00035F1C
	private void setMirageState(float mirageState)
	{
		this.renderers = base.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < this.renderers.Length; i++)
		{
			Material[] materials = this.renderers[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].SetFloat("_DoMirage", mirageState);
			}
		}
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x00037D72 File Offset: 0x00035F72
	private void Update()
	{
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x00037D74 File Offset: 0x00035F74
	private void OnDisable()
	{
		this.setMirageState(0f);
	}

	// Token: 0x040009B9 RID: 2489
	private Renderer[] renderers;
}
