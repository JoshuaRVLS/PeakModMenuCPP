using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200017D RID: 381
public class RopeTier : ItemComponent
{
	// Token: 0x06000C65 RID: 3173 RVA: 0x00042A23 File Offset: 0x00040C23
	private new void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this.item = base.GetComponent<Item>();
		this.spool = base.GetComponent<RopeSpool>();
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x00042A49 File Offset: 0x00040C49
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x06000C67 RID: 3175 RVA: 0x00042A4B File Offset: 0x00040C4B
	public bool LookingToPlaceAnchor
	{
		get
		{
			return this.ropeAnchor != null;
		}
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x00042A59 File Offset: 0x00040C59
	private void OnDestroy()
	{
		if (this.ropeAnchor)
		{
			Object.DestroyImmediate(this.ropeAnchor.gameObject);
		}
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x00042A78 File Offset: 0x00040C78
	public void Update()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.item.itemState != ItemState.Held)
		{
			return;
		}
		if (this.releaseCheck)
		{
			if (Character.localCharacter.input.usePrimaryWasReleased)
			{
				this.releaseCheck = false;
			}
			return;
		}
		if (!Character.localCharacter.input.usePrimaryIsPressed)
		{
			this.item.overrideProgress = 0f;
			this.item.overrideForceProgress = false;
			if (this.ropeAnchor != null)
			{
				Object.DestroyImmediate(this.ropeAnchor.gameObject);
			}
			return;
		}
		if (this.ropeAnchor != null && this.goodAnchorPlace != null && Vector3.Distance(this.goodAnchorPlace.Value.point, base.transform.position) > this.maxAnchorGhostDistance)
		{
			this.item.overrideProgress = 0f;
			this.item.overrideForceProgress = false;
			Object.DestroyImmediate(this.ropeAnchor.gameObject);
			return;
		}
		if (this.ropeAnchor == null)
		{
			this.ropeAnchor = Object.Instantiate<GameObject>(this.anchorPreview).GetComponent<RopeAnchor>();
			this.ropeAnchor.anchorPoint.gameObject.SetActive(false);
			this.goodAnchorPlace = null;
			this.timeWithGoodAnchor = 0f;
		}
		if (this.goodAnchorPlace == null)
		{
			Transform transform = MainCamera.instance.transform;
			Vector3 position = transform.position;
			RaycastHit value = HelperFunctions.LineCheck(position, position + transform.forward * this.maxAnchorGhostDistance, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
			Debug.DrawLine(position, value.point, Color.red);
			if (value.collider == null)
			{
				return;
			}
			if (this.item == null)
			{
				Debug.Log("Item is null");
			}
			if (this.item.holderCharacter == null)
			{
				Debug.Log("Item holder is null");
			}
			float num = Vector3.Distance(value.point, this.item.holderCharacter.Center);
			this.ropeAnchor.Ghost = true;
			this.ropeAnchor.transform.position = value.point;
			this.ropeAnchor.transform.forward = Vector3.Cross(transform.right, value.normal);
			this.ropeAnchor.transform.up = value.normal;
			if (num < this.maxAnchorDistance)
			{
				this.goodAnchorPlace = new RaycastHit?(value);
				this.ropeAnchor.Ghost = false;
			}
			return;
		}
		else
		{
			this.item.overrideForceProgress = false;
			if (this.goodAnchorPlace == null)
			{
				return;
			}
			this.timeWithGoodAnchor += Time.deltaTime;
			this.item.overrideForceProgress = true;
			this.item.overrideProgress = this.timeWithGoodAnchor / this.castTime;
			if (this.timeWithGoodAnchor < this.castTime)
			{
				return;
			}
			Debug.Log("Cast anchor");
			this.item.overrideForceProgress = false;
			this.item.overrideProgress = 0f;
			GameObject gameObject = PhotonNetwork.Instantiate(this.anchorPrefab.name, this.ropeAnchor.transform.position, this.ropeAnchor.transform.rotation, 0, null);
			if (this.item.photonView.IsMine)
			{
				Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.RopePlaced, this.spool.rope.GetLengthInMeters());
				GameUtils.instance.IncrementPermanentItemsPlaced();
			}
			this.spool.rope.photonView.RPC("AttachToAnchor_Rpc", RpcTarget.AllBuffered, new object[]
			{
				gameObject.GetComponent<PhotonView>(),
				this.spool.rope.Segments
			});
			Object.DestroyImmediate(this.ropeAnchor.gameObject);
			this.releaseCheck = true;
			this.ropeAnchor = null;
			return;
		}
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x00042E57 File Offset: 0x00041057
	public override void OnDisable()
	{
		this.item.overrideForceProgress = false;
		this.item.overrideProgress = 0f;
		base.OnDisable();
	}

	// Token: 0x04000B54 RID: 2900
	public GameObject anchorPreview;

	// Token: 0x04000B55 RID: 2901
	public GameObject anchorPrefab;

	// Token: 0x04000B56 RID: 2902
	public float maxAnchorGhostDistance = 10f;

	// Token: 0x04000B57 RID: 2903
	public float maxAnchorDistance = 5f;

	// Token: 0x04000B58 RID: 2904
	public float castTime;

	// Token: 0x04000B59 RID: 2905
	private RaycastHit? goodAnchorPlace;

	// Token: 0x04000B5A RID: 2906
	public float timeWithGoodAnchor;

	// Token: 0x04000B5B RID: 2907
	private new Item item;

	// Token: 0x04000B5C RID: 2908
	private RopeSpool spool;

	// Token: 0x04000B5D RID: 2909
	public RopeAnchor ropeAnchor;

	// Token: 0x04000B5E RID: 2910
	private PhotonView view;

	// Token: 0x04000B5F RID: 2911
	private bool releaseCheck;
}
