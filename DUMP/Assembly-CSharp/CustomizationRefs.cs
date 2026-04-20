using System;
using UnityEngine;

// Token: 0x02000244 RID: 580
public class CustomizationRefs : MonoBehaviour
{
	// Token: 0x17000134 RID: 308
	// (get) Token: 0x060011A9 RID: 4521 RVA: 0x00059042 File Offset: 0x00057242
	// (set) Token: 0x060011AA RID: 4522 RVA: 0x0005904A File Offset: 0x0005724A
	public bool accessoryEnabled
	{
		get
		{
			return this._accessoryEnabled;
		}
		set
		{
			this._accessoryEnabled = value;
			this.accessoryRenderer.enabled = this._accessoryEnabled;
		}
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x00059064 File Offset: 0x00057264
	public void SetSkeleton(bool active, bool isLocal)
	{
		if (active)
		{
			for (int i = 0; i < this.AllRenderers.Length; i++)
			{
				this.AllRenderers[i].enabled = false;
			}
			this.skeletonRenderer.gameObject.SetActive(true);
			if (isLocal)
			{
				((SkinnedMeshRenderer)this.skeletonRenderer).sharedMesh = this.skeletonFirstPerson;
				this.skeletonRenderer.material = this.skeletonFirstPersonMat;
			}
			else
			{
				((SkinnedMeshRenderer)this.skeletonRenderer).sharedMesh = this.skeletonThirdPerson;
				this.skeletonRenderer.material = this.skeletonThirdPersonMat;
			}
			this.mainRendererShadow.sharedMesh = this.skeletonThirdPerson;
			this.skirtShadow.enabled = false;
			this.shortsShadow.enabled = false;
			if (this.headShadow)
			{
				this.headShadow.enabled = false;
			}
			else
			{
				Debug.Log("No headshadow renderer detected. Assuming we're in PEAK sequence and ignoring");
			}
			this.sashRenderer.enabled = true;
			this.accessoryEnabled = false;
			this.thirdEye.GetComponent<Renderer>().enabled = false;
			return;
		}
		for (int j = 0; j < this.AllRenderers.Length; j++)
		{
			this.AllRenderers[j].enabled = true;
		}
		this.skeletonRenderer.gameObject.SetActive(false);
		this.mainRendererShadow.sharedMesh = this.mainRenderer.sharedMesh;
		this.skirtShadow.enabled = true;
		this.shortsShadow.enabled = true;
		this.headShadow.enabled = true;
		this.thirdEye.GetComponent<Renderer>().enabled = true;
		this.accessoryEnabled = !this.thirdEye.activeSelf;
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x00059200 File Offset: 0x00057400
	public void SetMushroomMan(bool active)
	{
		if (active)
		{
			this.customization.HideAllRenderers();
			this.skeletonRenderer.gameObject.SetActive(true);
			((SkinnedMeshRenderer)this.skeletonRenderer).sharedMesh = this.mushroomManMesh;
			this.skeletonRenderer.material = this.mushroomManMaterial;
			this.mainRendererShadow.enabled = false;
			this.skirtShadow.enabled = false;
			this.shortsShadow.enabled = false;
			this.headShadow.enabled = false;
			this.sashRenderer.enabled = false;
			this.thirdEye.GetComponent<Renderer>().enabled = false;
			return;
		}
		this.customization.ShowAllRenderers();
		this.skeletonRenderer.gameObject.SetActive(false);
		this.mainRendererShadow.enabled = true;
		this.skirtShadow.enabled = true;
		this.shortsShadow.enabled = true;
		this.headShadow.enabled = true;
		this.sashRenderer.enabled = true;
		this.thirdEye.GetComponent<Renderer>().enabled = true;
		if (this.thirdEye.activeSelf)
		{
			this.accessoryEnabled = false;
		}
	}

	// Token: 0x04000F94 RID: 3988
	public CharacterCustomization customization;

	// Token: 0x04000F95 RID: 3989
	public SkinnedMeshRenderer mainRenderer;

	// Token: 0x04000F96 RID: 3990
	public SkinnedMeshRenderer mainRendererShadow;

	// Token: 0x04000F97 RID: 3991
	public Renderer[] PlayerRenderers;

	// Token: 0x04000F98 RID: 3992
	public Renderer[] EyeRenderers;

	// Token: 0x04000F99 RID: 3993
	public Renderer mouthRenderer;

	// Token: 0x04000F9A RID: 3994
	private bool _accessoryEnabled = true;

	// Token: 0x04000F9B RID: 3995
	public Renderer accessoryRenderer;

	// Token: 0x04000F9C RID: 3996
	public Renderer shorts;

	// Token: 0x04000F9D RID: 3997
	public Renderer skirt;

	// Token: 0x04000F9E RID: 3998
	public Renderer skirtShadow;

	// Token: 0x04000F9F RID: 3999
	public Renderer shortsShadow;

	// Token: 0x04000FA0 RID: 4000
	public Renderer[] playerHats;

	// Token: 0x04000FA1 RID: 4001
	public MeshFilter hatShadowMeshFilter;

	// Token: 0x04000FA2 RID: 4002
	public Renderer sashRenderer;

	// Token: 0x04000FA3 RID: 4003
	public Renderer blindRenderer;

	// Token: 0x04000FA4 RID: 4004
	public Renderer chickenRenderer;

	// Token: 0x04000FA5 RID: 4005
	public Renderer headShadow;

	// Token: 0x04000FA6 RID: 4006
	public Renderer skeletonRenderer;

	// Token: 0x04000FA7 RID: 4007
	public Mesh skeletonFirstPerson;

	// Token: 0x04000FA8 RID: 4008
	public Mesh skeletonThirdPerson;

	// Token: 0x04000FA9 RID: 4009
	public Mesh mushroomManMesh;

	// Token: 0x04000FAA RID: 4010
	public Material mushroomManMaterial;

	// Token: 0x04000FAB RID: 4011
	public Material skeletonFirstPersonMat;

	// Token: 0x04000FAC RID: 4012
	public Material skeletonThirdPersonMat;

	// Token: 0x04000FAD RID: 4013
	public Material[] sashAscentMaterials;

	// Token: 0x04000FAE RID: 4014
	public Transform hatTransform;

	// Token: 0x04000FAF RID: 4015
	public Renderer[] AllRenderers;

	// Token: 0x04000FB0 RID: 4016
	public GameObject thirdEye;
}
