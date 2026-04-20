using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000279 RID: 633
public class HideTheBody : MonoBehaviour
{
	// Token: 0x06001280 RID: 4736 RVA: 0x0005CC9E File Offset: 0x0005AE9E
	private void Start()
	{
		this.character = base.GetComponentInParent<Character>();
		this.Toggle(true);
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x0005CCB4 File Offset: 0x0005AEB4
	private void Update()
	{
		bool flag = !this.character.IsLocal || this.character.data.fullyPassedOut || this.character.data.dead || this.isDummy;
		if (!this.character.IsLocal && this.character.data.carrier != null && this.character.data.carrier.IsLocal)
		{
			flag = false;
		}
		if (flag != this.isShowing)
		{
			this.Toggle(flag);
		}
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x0005CD4A File Offset: 0x0005AF4A
	public void Refresh()
	{
		this.Toggle(this.isShowing);
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x0005CD58 File Offset: 0x0005AF58
	private void Toggle(bool show)
	{
		this.isShowing = show;
		this.shadowCaster.SetActive(!show);
		this.shadowCasterHat.SetActive(!show);
		if (show)
		{
			this.SetShowing(this.body, 0f);
			this.SetShowing(this.headRend, 0f);
			this.SetShowing(this.sash, 0f);
			for (int i = 0; i < this.costumes.Length; i++)
			{
				this.SetShowing(this.costumes[i], 0f);
			}
			Renderer[] componentsInChildren = this.face.GetComponentsInChildren<Renderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				this.SetShowing(componentsInChildren[j], 0f);
			}
			for (int k = 0; k < this.refs.playerHats.Length; k++)
			{
				this.SetShowing(this.refs.playerHats[k], 0f);
			}
			return;
		}
		this.SetShowing(this.body, 1f);
		this.SetShowing(this.headRend, 1f);
		this.SetShowing(this.sash, 1f);
		for (int l = 0; l < this.costumes.Length; l++)
		{
			this.SetShowing(this.costumes[l], 1f);
		}
		Renderer[] componentsInChildren2 = this.face.GetComponentsInChildren<Renderer>();
		for (int m = 0; m < componentsInChildren2.Length; m++)
		{
			this.SetShowing(componentsInChildren2[m], 1f);
		}
		for (int n = 0; n < this.refs.playerHats.Length; n++)
		{
			this.SetShowing(this.refs.playerHats[n], 1f);
		}
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x0005CF08 File Offset: 0x0005B108
	public void SetShowing(Renderer r, float x)
	{
		Material[] materials = r.materials;
		Material[] array = materials;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(this.VERTEXGHOST, x);
		}
		r.materials = materials;
	}

	// Token: 0x0400108D RID: 4237
	public bool isDummy;

	// Token: 0x0400108E RID: 4238
	public SkinnedMeshRenderer body;

	// Token: 0x0400108F RID: 4239
	public Renderer headRend;

	// Token: 0x04001090 RID: 4240
	public CustomizationRefs refs;

	// Token: 0x04001091 RID: 4241
	public Transform face;

	// Token: 0x04001092 RID: 4242
	public GameObject shadowCaster;

	// Token: 0x04001093 RID: 4243
	public GameObject shadowCasterHat;

	// Token: 0x04001094 RID: 4244
	public SkinnedMeshRenderer[] costumes;

	// Token: 0x04001095 RID: 4245
	[FormerlySerializedAs("Sash")]
	public SkinnedMeshRenderer sash;

	// Token: 0x04001096 RID: 4246
	private bool isShowing = true;

	// Token: 0x04001097 RID: 4247
	private Character character;

	// Token: 0x04001098 RID: 4248
	private int VERTEXGHOST = Shader.PropertyToID("_VertexGhost");
}
