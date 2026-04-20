using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x0200001E RID: 30
public class Guidebook : Item
{
	// Token: 0x06000251 RID: 593 RVA: 0x00011A7C File Offset: 0x0000FC7C
	public override void OnEnable()
	{
		base.OnEnable();
		if (SettingsHandler.Instance != null && SettingsHandler.Instance.GetSetting<RenderScaleSetting>().Value == RenderScaleSetting.RenderScaleQuality.Low)
		{
			this.canvasScaler.scaleFactor = 2f;
			this.currentRenderTexture.width = 3600;
			this.currentRenderTexture.height = 1800;
			this.lastRenderTexture.width = 3600;
			this.lastRenderTexture.height = 1800;
		}
		else
		{
			this.canvasScaler.scaleFactor = 1f;
			this.currentRenderTexture.width = 1800;
			this.currentRenderTexture.height = 900;
			this.lastRenderTexture.width = 1800;
			this.lastRenderTexture.height = 900;
		}
		if (this.isSinglePage)
		{
			base.Invoke("OpenSinglePage", 0.01f);
		}
	}

	// Token: 0x06000252 RID: 594 RVA: 0x00011B64 File Offset: 0x0000FD64
	private void OpenSinglePage()
	{
		RenderTexture renderTexture = new RenderTexture(this.guidebookRenderTexture);
		renderTexture.Create();
		this.guidebookRenderTexture = renderTexture;
		this.currentRenderTexture = renderTexture;
		this.renderCamera.targetTexture = this.guidebookRenderTexture;
		if (base.itemState == ItemState.InBackpack)
		{
			this.canvasScaler.gameObject.SetActive(false);
		}
		this.currentlyVisibleLeftPageIndex = 2;
		this.currentlyVisibleRightPageIndex = 3;
		this.nextVisibleLeftPageIndex = 0;
		this.nextVisibleRightPageIndex = 1;
		this.UpdatePageDisplay();
		for (int i = 0; i < this.pageRenderers.Length; i++)
		{
			this.pageRenderers[i].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
		}
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00011C0F File Offset: 0x0000FE0F
	public override void OnDisable()
	{
		base.OnDisable();
		if (this.isSinglePage)
		{
			Object.Destroy(this.renderCamera.targetTexture);
		}
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00011C2F File Offset: 0x0000FE2F
	internal void ToggleGuidebook()
	{
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("ToggleGuidebook_RPC", RpcTarget.All, new object[]
			{
				!this.isOpen
			});
		}
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00011C68 File Offset: 0x0000FE68
	[PunRPC]
	public void ToggleGuidebook_RPC(bool open)
	{
		this.isOpen = open;
		if (this.isOpen)
		{
			if (!this.isSinglePage)
			{
				this.anim.Play("Open", 0, 0f);
			}
			this.coll.enabled = false;
			this.renderCamera.targetTexture = this.guidebookRenderTexture;
			this.currentlyVisibleLeftPageIndex = 2;
			this.currentlyVisibleRightPageIndex = 3;
			this.nextVisibleLeftPageIndex = 0;
			this.nextVisibleRightPageIndex = 1;
			this.UpdatePageDisplay();
			for (int i = 0; i < this.pageRenderers.Length; i++)
			{
				this.pageRenderers[i].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
			}
			return;
		}
		if (!this.isSinglePage)
		{
			this.anim.Play("Close", 0, 0f);
		}
		this.coll.enabled = true;
		this.bookTransform.DOLocalMove(Vector3.zero, 0.25f, false);
		this.bookTransform.DOLocalRotate(Vector3.zero, 0.25f, RotateMode.Fast);
		for (int j = 0; j < this.pageRenderers.Length; j++)
		{
			this.pageRenderers[j].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
		}
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00011DA0 File Offset: 0x0000FFA0
	private void LateUpdate()
	{
		if (this.isOpen && base.holderCharacter.IsLocal)
		{
			this.bookTransform.position = Vector3.Lerp(this.bookTransform.position, MainCamera.instance.cam.transform.position + MainCamera.instance.cam.transform.forward * this.readingDistance, Time.deltaTime * 10f);
			this.bookTransform.forward = MainCamera.instance.cam.transform.forward;
		}
	}

	// Token: 0x06000257 RID: 599 RVA: 0x00011E42 File Offset: 0x00010042
	private void PopulatePages()
	{
		this.pageSpreads = base.GetComponentsInChildren<GuidebookSpread>(true).ToList<GuidebookSpread>();
	}

	// Token: 0x06000258 RID: 600 RVA: 0x00011E58 File Offset: 0x00010058
	private void PopulatePageNumbers()
	{
		for (int i = 0; i < this.pageSpreads.Count; i++)
		{
		}
	}

	// Token: 0x06000259 RID: 601 RVA: 0x00011E7C File Offset: 0x0001007C
	internal void FlipPageRight()
	{
		if (base.photonView.IsMine && this.currentPageSet < this.pageSpreads.Count - 1)
		{
			this.currentPageSet++;
			base.photonView.RPC("FlipPageRight_RPC", RpcTarget.All, new object[]
			{
				this.currentPageSet
			});
		}
	}

	// Token: 0x0600025A RID: 602 RVA: 0x00011EE0 File Offset: 0x000100E0
	internal void FlipPageLeft()
	{
		if (base.photonView.IsMine && this.currentPageSet >= 1)
		{
			this.currentPageSet--;
			base.photonView.RPC("FlipPageLeft_RPC", RpcTarget.All, new object[]
			{
				this.currentPageSet
			});
		}
	}

	// Token: 0x0600025B RID: 603 RVA: 0x00011F38 File Offset: 0x00010138
	[PunRPC]
	public void FlipPageRight_RPC(int currentPage)
	{
		this.currentlyVisibleLeftPageIndex = 2;
		this.currentlyVisibleRightPageIndex = 3;
		this.nextVisibleLeftPageIndex = 4;
		this.nextVisibleRightPageIndex = 5;
		this.anim.Play("Guidebook_FlipRight", 0, 0f);
		this.currentPageSet = currentPage;
		this.UpdatePageDisplay();
	}

	// Token: 0x0600025C RID: 604 RVA: 0x00011F84 File Offset: 0x00010184
	[PunRPC]
	public void FlipPageLeft_RPC(int currentPage)
	{
		this.currentlyVisibleLeftPageIndex = 2;
		this.currentlyVisibleRightPageIndex = 3;
		this.nextVisibleLeftPageIndex = 0;
		this.nextVisibleRightPageIndex = 1;
		this.anim.Play("Guidebook_FlipLeft", 0, 0f);
		this.currentPageSet = currentPage;
		this.UpdatePageDisplay();
	}

	// Token: 0x0600025D RID: 605 RVA: 0x00011FD0 File Offset: 0x000101D0
	private void UpdatePageDisplay()
	{
		Graphics.CopyTexture(this.currentRenderTexture, this.lastRenderTexture);
		for (int i = 0; i < this.pageSpreads.Count; i++)
		{
			this.pageSpreads[i].gameObject.SetActive(i == this.currentPageSet);
		}
		this.renderCamera.Render();
		this.pageRenderers[this.currentlyVisibleLeftPageIndex].material.SetTexture(Guidebook.BASETEX, this.lastRenderTexture);
		this.pageRenderers[this.currentlyVisibleRightPageIndex].material.SetTexture(Guidebook.BASETEX, this.lastRenderTexture);
		this.pageRenderers[this.nextVisibleLeftPageIndex].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
		this.pageRenderers[this.nextVisibleRightPageIndex].material.SetTexture(Guidebook.BASETEX, this.currentRenderTexture);
	}

	// Token: 0x0400022C RID: 556
	public static int BASETEX = Shader.PropertyToID("_BaseTexture");

	// Token: 0x0400022D RID: 557
	public bool isSinglePage;

	// Token: 0x0400022E RID: 558
	public Animator anim;

	// Token: 0x0400022F RID: 559
	public int currentPageSet;

	// Token: 0x04000230 RID: 560
	[FormerlySerializedAs("pages")]
	[PreviouslySerializedAs("pages")]
	public List<GuidebookSpread> pageSpreads;

	// Token: 0x04000231 RID: 561
	public List<RectTransform> pagePrefabs;

	// Token: 0x04000232 RID: 562
	public Camera renderCamera;

	// Token: 0x04000233 RID: 563
	public CanvasScaler canvasScaler;

	// Token: 0x04000234 RID: 564
	public Texture currentRenderTexture;

	// Token: 0x04000235 RID: 565
	public Texture lastRenderTexture;

	// Token: 0x04000236 RID: 566
	public Renderer[] pageRenderers;

	// Token: 0x04000237 RID: 567
	public Transform bookTransform;

	// Token: 0x04000238 RID: 568
	public float readingDistance = 0.4f;

	// Token: 0x04000239 RID: 569
	public Collider coll;

	// Token: 0x0400023A RID: 570
	[HideInInspector]
	public bool isOpen;

	// Token: 0x0400023B RID: 571
	public RenderTexture guidebookRenderTexture;

	// Token: 0x0400023C RID: 572
	private int currentlyVisibleLeftPageIndex;

	// Token: 0x0400023D RID: 573
	private int currentlyVisibleRightPageIndex;

	// Token: 0x0400023E RID: 574
	private int nextVisibleLeftPageIndex;

	// Token: 0x0400023F RID: 575
	private int nextVisibleRightPageIndex;
}
