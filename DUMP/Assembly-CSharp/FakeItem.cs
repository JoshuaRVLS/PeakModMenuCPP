using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI.Extensions;

// Token: 0x0200001A RID: 26
[ExecuteAlways]
public class FakeItem : MonoBehaviour, IInteractible
{
	// Token: 0x06000235 RID: 565 RVA: 0x00010FF2 File Offset: 0x0000F1F2
	private void Awake()
	{
		if (Application.isPlaying)
		{
			this.AddPropertyBlock();
		}
	}

	// Token: 0x06000236 RID: 566 RVA: 0x00011004 File Offset: 0x0000F204
	private void AddPropertyBlock()
	{
		this.mpb = new MaterialPropertyBlock();
		this.mainRenderer = base.GetComponentInChildren<MeshRenderer>();
		if (!this.mainRenderer)
		{
			this.mainRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>();
		}
		this.mainRenderer.GetPropertyBlock(this.mpb);
	}

	// Token: 0x06000237 RID: 567 RVA: 0x00011054 File Offset: 0x0000F254
	public void HoverEnter()
	{
		if (this.mpb != null && this.mainRenderer != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
			this.mainRenderer.SetPropertyBlock(this.mpb);
			return;
		}
		if (this.mpb == null)
		{
			Debug.LogError("Fake item is missing it's material property block");
			return;
		}
		Debug.LogError("Fake item is missing it's renderer");
	}

	// Token: 0x06000238 RID: 568 RVA: 0x000110BC File Offset: 0x0000F2BC
	public void HoverExit()
	{
		if (this.mpb != null && this.mainRenderer != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			this.mainRenderer.SetPropertyBlock(this.mpb);
			return;
		}
		if (this.mpb == null)
		{
			Debug.LogError("Fake item is missing it's material property block");
			return;
		}
		Debug.LogError("Fake item is missing it's renderer");
	}

	// Token: 0x06000239 RID: 569 RVA: 0x00011123 File Offset: 0x0000F323
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x0600023A RID: 570 RVA: 0x00011128 File Offset: 0x0000F328
	public void Interact(Character interactor)
	{
		if (!interactor.player.HasEmptySlot(this.realItemPrefab.itemID))
		{
			return;
		}
		base.gameObject.SetActive(false);
		FakeItemManager.Instance.photonView.RPC("RPC_RequestFakeItemPickup", RpcTarget.MasterClient, new object[]
		{
			interactor.GetComponent<PhotonView>(),
			this.index
		});
		Debug.Log("Picking up " + base.gameObject.name);
	}

	// Token: 0x0600023B RID: 571 RVA: 0x000111A6 File Offset: 0x0000F3A6
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x0600023C RID: 572 RVA: 0x000111B3 File Offset: 0x0000F3B3
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x0600023D RID: 573 RVA: 0x000111BB File Offset: 0x0000F3BB
	public string GetInteractionText()
	{
		return LocalizedText.GetText("PICKUP", true);
	}

	// Token: 0x0600023E RID: 574 RVA: 0x000111C8 File Offset: 0x0000F3C8
	public string GetName()
	{
		return LocalizedText.GetText(LocalizedText.GetNameIndex(this.itemName), true);
	}

	// Token: 0x0600023F RID: 575 RVA: 0x000111DB File Offset: 0x0000F3DB
	public virtual void PickUpVisibly()
	{
		base.gameObject.SetActive(false);
		FakeItemManager.Instance.fakeItemData.hiddenItems.Add(this.index);
		this.pickedUp = true;
	}

	// Token: 0x06000240 RID: 576 RVA: 0x0001120A File Offset: 0x0000F40A
	public virtual void UnPickUpVisibly()
	{
		base.gameObject.SetActive(true);
		FakeItemManager.Instance.fakeItemData.hiddenItems.Remove(this.index);
		this.pickedUp = false;
	}

	// Token: 0x0400020E RID: 526
	public string itemName;

	// Token: 0x0400020F RID: 527
	public Item realItemPrefab;

	// Token: 0x04000210 RID: 528
	[ReadOnly]
	public bool pickedUp;

	// Token: 0x04000211 RID: 529
	[ReadOnly]
	public int index;

	// Token: 0x04000212 RID: 530
	private MaterialPropertyBlock mpb;

	// Token: 0x04000213 RID: 531
	public Renderer mainRenderer;

	// Token: 0x04000214 RID: 532
	private double lastCulledItems;
}
