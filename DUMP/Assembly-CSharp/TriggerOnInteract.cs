using System;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class TriggerOnInteract : MonoBehaviour, IInteractible
{
	// Token: 0x06000696 RID: 1686 RVA: 0x00025E6E File Offset: 0x0002406E
	private void Awake()
	{
		this.mpb = new MaterialPropertyBlock();
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x00025E7B File Offset: 0x0002407B
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x00025E7E File Offset: 0x0002407E
	public void Interact(Character interactor)
	{
		this.triggerEvent.TriggerEntered();
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x00025E8B File Offset: 0x0002408B
	public void HoverEnter()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
			base.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(this.mpb);
		}
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00025EBB File Offset: 0x000240BB
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			base.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(this.mpb);
		}
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00025EEB File Offset: 0x000240EB
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00025EF8 File Offset: 0x000240F8
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00025F00 File Offset: 0x00024100
	public string GetInteractionText()
	{
		return LocalizedText.GetText("PICKUP", true);
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00025F0D File Offset: 0x0002410D
	public string GetName()
	{
		return this.interactableName;
	}

	// Token: 0x0400068F RID: 1679
	private MaterialPropertyBlock mpb;

	// Token: 0x04000690 RID: 1680
	public string interactText;

	// Token: 0x04000691 RID: 1681
	public TriggerEvent triggerEvent;

	// Token: 0x04000692 RID: 1682
	public string interactableName;
}
