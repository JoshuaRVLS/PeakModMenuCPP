using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200033A RID: 826
public class ShaderEffect : MonoBehaviour
{
	// Token: 0x060015DB RID: 5595 RVA: 0x0006EA2B File Offset: 0x0006CC2B
	private void Start()
	{
		this.prop = new MaterialPropertyBlock();
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x0006EA38 File Offset: 0x0006CC38
	private void Update()
	{
		foreach (Renderer item in this.renderers)
		{
			this.PerRendere(item);
		}
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x0006EA65 File Offset: 0x0006CC65
	private void PerRendere(Renderer item)
	{
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x0006EA68 File Offset: 0x0006CC68
	internal void SetEffect(Material mat, string key, float value)
	{
		if (!this.currentEffects.Contains(mat))
		{
			this.AddEffect(mat);
		}
		foreach (Renderer renderer in this.renderers)
		{
			this.prop.SetFloat(key, value);
			renderer.SetPropertyBlock(this.prop);
		}
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x0006EABC File Offset: 0x0006CCBC
	private void AddEffect(Material mat)
	{
		foreach (Renderer renderer in this.renderers)
		{
			List<Material> list = new List<Material>();
			list.AddRange(renderer.sharedMaterials);
			list.Add(mat);
			renderer.sharedMaterials = list.ToArray();
		}
		this.currentEffects.Add(mat);
	}

	// Token: 0x060015E0 RID: 5600 RVA: 0x0006EB13 File Offset: 0x0006CD13
	internal void ClearEffect(Material mat)
	{
		if (this.currentEffects.Count == 0)
		{
			return;
		}
		if (this.currentEffects.Contains(mat))
		{
			this.RemoveEffect(mat);
		}
	}

	// Token: 0x060015E1 RID: 5601 RVA: 0x0006EB38 File Offset: 0x0006CD38
	private void RemoveEffect(Material mat)
	{
		foreach (Renderer renderer in this.renderers)
		{
			List<Material> list = new List<Material>();
			list.AddRange(renderer.sharedMaterials);
			list.Remove(mat);
			renderer.sharedMaterials = list.ToArray();
		}
		this.currentEffects.Remove(mat);
	}

	// Token: 0x040013E1 RID: 5089
	public Renderer[] renderers;

	// Token: 0x040013E2 RID: 5090
	private List<Material> currentEffects = new List<Material>();

	// Token: 0x040013E3 RID: 5091
	private MaterialPropertyBlock prop;
}
