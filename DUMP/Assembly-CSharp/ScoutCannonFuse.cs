using System;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class ScoutCannonFuse : MonoBehaviour, IInteractibleConstant, IInteractible
{
	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000CD2 RID: 3282 RVA: 0x00044A60 File Offset: 0x00042C60
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000CD3 RID: 3283 RVA: 0x00044A63 File Offset: 0x00042C63
	// (set) Token: 0x06000CD4 RID: 3284 RVA: 0x00044A7F File Offset: 0x00042C7F
	private MeshRenderer[] meshRenderers
	{
		get
		{
			if (this._mr == null)
			{
				this._mr = base.GetComponentsInChildren<MeshRenderer>();
			}
			return this._mr;
		}
		set
		{
			this._mr = value;
		}
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x00044A88 File Offset: 0x00042C88
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x00044A8A File Offset: 0x00042C8A
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00044A97 File Offset: 0x00042C97
	public string GetInteractionText()
	{
		return LocalizedText.GetText("LIGHT", true);
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00044AA4 File Offset: 0x00042CA4
	public float GetInteractTime(Character interactor)
	{
		return 1f;
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x00044AAB File Offset: 0x00042CAB
	public string GetName()
	{
		return LocalizedText.GetText("SCOUTCANNONFUSE", true);
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00044AB8 File Offset: 0x00042CB8
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x00044AC0 File Offset: 0x00042CC0
	public void HoverEnter()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				if (this.meshRenderers[i] != null)
				{
					this.meshRenderers[i].SetPropertyBlock(this.mpb);
				}
			}
		}
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x00044B20 File Offset: 0x00042D20
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				if (this.meshRenderers[i] != null)
				{
					this.meshRenderers[i].SetPropertyBlock(this.mpb);
				}
			}
		}
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x00044B80 File Offset: 0x00042D80
	public void Interact(Character interactor)
	{
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x00044B82 File Offset: 0x00042D82
	public void Interact_CastFinished(Character interactor)
	{
		this.scoutCannon.Light();
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x00044B8F File Offset: 0x00042D8F
	public bool IsConstantlyInteractable(Character interactor)
	{
		return !this.scoutCannon.lit;
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x00044B9F File Offset: 0x00042D9F
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x00044BA2 File Offset: 0x00042DA2
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x04000B97 RID: 2967
	public ScoutCannon scoutCannon;

	// Token: 0x04000B98 RID: 2968
	private MaterialPropertyBlock mpb;

	// Token: 0x04000B99 RID: 2969
	private MeshRenderer[] _mr;
}
