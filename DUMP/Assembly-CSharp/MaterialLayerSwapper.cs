using System;
using UnityEngine;

// Token: 0x020002B1 RID: 689
public class MaterialLayerSwapper : MonoBehaviour
{
	// Token: 0x06001383 RID: 4995 RVA: 0x00062F38 File Offset: 0x00061138
	private void Swap()
	{
		string name = "_Color" + this.layer.x.ToString("F0");
		string name2 = "_Smooth" + this.layer.x.ToString("F0");
		string name3 = "_Height" + this.layer.x.ToString("F0");
		string name4 = "_Texture" + this.layer.x.ToString("F0");
		string name5 = "_Triplanar" + this.layer.x.ToString("F0");
		string name6 = "_UV" + this.layer.x.ToString("F0");
		string name7 = "_Flip" + this.layer.x.ToString("F0");
		string name8 = "_Remap" + this.layer.x.ToString("F0");
		Material material = base.GetComponentInChildren<Renderer>().sharedMaterials[this.targetMaterial];
		this.color = material.GetColor(name);
		this.smooth = material.GetFloat(name2);
		this.height = material.GetFloat(name3);
		this.texture = material.GetTexture(name4);
		this.triplanar = material.GetFloat(name5);
		this.uv = material.GetFloat(name6);
		this.flip = material.GetFloat(name7);
		this.remap = material.GetVector(name8);
		string name9 = "_Color" + this.layer.y.ToString("F0");
		string name10 = "_Smooth" + this.layer.y.ToString("F0");
		string name11 = "_Height" + this.layer.y.ToString("F0");
		string name12 = "_Texture" + this.layer.y.ToString("F0");
		string name13 = "_Triplanar" + this.layer.y.ToString("F0");
		string name14 = "_UV" + this.layer.y.ToString("F0");
		string name15 = "_Flip" + this.layer.y.ToString("F0");
		string name16 = "_Remap" + this.layer.y.ToString("F0");
		this.color2 = material.GetColor(name9);
		this.smooth2 = material.GetFloat(name10);
		this.height2 = material.GetFloat(name11);
		this.texture2 = material.GetTexture(name12);
		this.triplanar2 = material.GetFloat(name13);
		this.uv2 = material.GetFloat(name14);
		this.flip2 = material.GetFloat(name15);
		this.remap2 = material.GetVector(name16);
		material.SetColor(name9, this.color);
		material.SetFloat(name10, this.smooth);
		material.SetFloat(name11, this.height);
		material.SetTexture(name12, this.texture);
		material.SetFloat(name13, this.triplanar);
		material.SetFloat(name14, this.uv);
		material.SetFloat(name15, this.flip);
		material.SetVector(name16, this.remap);
		material.SetColor(name, this.color2);
		material.SetFloat(name2, this.smooth2);
		material.SetFloat(name3, this.height2);
		material.SetTexture(name4, this.texture2);
		material.SetFloat(name5, this.triplanar2);
		material.SetFloat(name6, this.uv2);
		material.SetFloat(name7, this.flip2);
		material.SetVector(name8, this.remap2);
	}

	// Token: 0x040011D4 RID: 4564
	public int targetMaterial;

	// Token: 0x040011D5 RID: 4565
	public Vector2Int layer;

	// Token: 0x040011D6 RID: 4566
	[ColorUsage(true, true)]
	public Color color;

	// Token: 0x040011D7 RID: 4567
	public float smooth;

	// Token: 0x040011D8 RID: 4568
	public float height;

	// Token: 0x040011D9 RID: 4569
	public Texture texture;

	// Token: 0x040011DA RID: 4570
	public float triplanar;

	// Token: 0x040011DB RID: 4571
	public float uv;

	// Token: 0x040011DC RID: 4572
	public float flip;

	// Token: 0x040011DD RID: 4573
	public Vector2 remap;

	// Token: 0x040011DE RID: 4574
	[ColorUsage(true, true)]
	public Color color2;

	// Token: 0x040011DF RID: 4575
	public float smooth2;

	// Token: 0x040011E0 RID: 4576
	public float height2;

	// Token: 0x040011E1 RID: 4577
	public Texture texture2;

	// Token: 0x040011E2 RID: 4578
	public float triplanar2;

	// Token: 0x040011E3 RID: 4579
	public float uv2;

	// Token: 0x040011E4 RID: 4580
	public float flip2;

	// Token: 0x040011E5 RID: 4581
	public Vector2 remap2;
}
