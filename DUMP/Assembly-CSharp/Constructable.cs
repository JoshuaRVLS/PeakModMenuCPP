using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200010D RID: 269
public class Constructable : ItemComponent
{
	// Token: 0x060008F6 RID: 2294 RVA: 0x00031009 File Offset: 0x0002F209
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x0003100C File Offset: 0x0002F20C
	protected virtual void Update()
	{
		if (this.item.holderCharacter && this.item.holderCharacter.IsLocal)
		{
			if (!this.constructing)
			{
				this.TryUpdatePreview();
			}
			else if (this.constructing && Vector3.Distance(MainCamera.instance.transform.position, this.currentConstructHit.point) > this.maxConstructDistance)
			{
				this.DestroyPreview();
				this.item.CancelUsePrimary();
			}
		}
		else
		{
			this.DestroyPreview();
		}
		if (!this.valid)
		{
			this.item.overrideUsability = Optionable<bool>.Some(false);
			return;
		}
		this.item.overrideUsability = Optionable<bool>.None;
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x000310BF File Offset: 0x0002F2BF
	private void OnDestroy()
	{
		this.DestroyPreview();
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x000310C8 File Offset: 0x0002F2C8
	public virtual void TryUpdatePreview()
	{
		this.hitCache = new RaycastHit[64];
		RaycastHit raycastHit = HelperFunctions.LineCheckIgnoreItem(MainCamera.instance.transform.position, MainCamera.instance.transform.position + MainCamera.instance.transform.forward.normalized * this.maxConstructDistance, HelperFunctions.LayerType.TerrainMap, this.hitCache, this.item);
		this.currentConstructHit = raycastHit;
		this.valid = this.CurrentHitIsValid();
		if (raycastHit.collider == null)
		{
			this.DestroyPreview();
			return;
		}
		this.CreateOrMovePreview();
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x00031169 File Offset: 0x0002F369
	private void OnDrawGizmosSelected()
	{
		if (this.currentConstructHit.collider != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(this.currentConstructHit.point, 0.5f);
		}
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x000311A0 File Offset: 0x0002F3A0
	private void CreateOrMovePreview()
	{
		if (this.currentPreview == null)
		{
			this.currentPreview = Object.Instantiate<ConstructablePreview>(this.previewPrefab);
			if (this.isAngleable)
			{
				this.UpdateAngle();
			}
		}
		this.currentPreview.transform.position = this.currentConstructHit.point;
		if (this.angleToNormal)
		{
			Vector3 normalized = Vector3.ProjectOnPlane(MainCamera.instance.transform.forward, this.currentConstructHit.normal).normalized;
			this.currentPreview.transform.rotation = Quaternion.LookRotation(normalized, this.currentConstructHit.normal);
		}
		else
		{
			Vector3 normalized2 = Vector3.ProjectOnPlane(MainCamera.instance.transform.forward, Vector3.up).normalized;
			this.currentPreview.transform.rotation = Quaternion.LookRotation(normalized2, Vector3.up);
		}
		if (!this.currentPreview.CollisionValid())
		{
			this.valid = false;
		}
		this.currentPreview.SetValid(this.valid);
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x000312AA File Offset: 0x0002F4AA
	internal void DestroyPreview()
	{
		this.constructing = false;
		if (this.currentPreview != null)
		{
			Object.Destroy(this.currentPreview.gameObject);
		}
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x000312D4 File Offset: 0x0002F4D4
	private bool CurrentHitIsValid()
	{
		return this.currentConstructHit.distance <= this.maxConstructDistance && (this.maxConstructVerticalAngle <= 0f || Vector3.Angle(Vector3.up, this.currentConstructHit.normal) <= this.maxConstructVerticalAngle);
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00031323 File Offset: 0x0002F523
	public virtual void StartConstruction()
	{
		if (this.valid)
		{
			this.constructing = true;
		}
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x00031334 File Offset: 0x0002F534
	public virtual GameObject FinishConstruction()
	{
		if (!this.constructing)
		{
			return null;
		}
		if (this.currentPreview == null)
		{
			return null;
		}
		GameObject gameObject = null;
		if (this.constructedPrefab.GetComponent<PhotonView>() == null)
		{
			this.photonView.RPC("CreatePrefabRPC", RpcTarget.AllBuffered, new object[]
			{
				this.currentPreview.transform.position,
				this.currentPreview.transform.rotation
			});
		}
		else
		{
			gameObject = PhotonNetwork.Instantiate(this.constructedPrefab.name, this.currentPreview.transform.position, this.currentPreview.transform.rotation, 0, null);
			if (this.isAngleable)
			{
				this.photonView.RPC("AngleIt", RpcTarget.AllBuffered, new object[]
				{
					gameObject.GetComponent<PhotonView>(),
					this.angleOffset
				});
			}
		}
		if (this.item.holderCharacter.IsLocal)
		{
			GameUtils.instance.IncrementPermanentItemsPlaced();
		}
		if (Character.localCharacter.data.currentItem == this.item)
		{
			Player.localPlayer.EmptySlot(Character.localCharacter.refs.items.currentSelectedSlot);
			Character.localCharacter.refs.afflictions.UpdateWeight();
		}
		PhotonNetwork.Destroy(base.gameObject);
		return gameObject;
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00031498 File Offset: 0x0002F698
	public void UpdateAngle()
	{
		if (this.currentPreview != null)
		{
			this.currentPreview.transform.GetChild(0).GetChild(0).localEulerAngles = new Vector3(-45f + this.angleOffset, 0f, 0f);
		}
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x000314EA File Offset: 0x0002F6EA
	[PunRPC]
	protected void CreatePrefabRPC(Vector3 position, Quaternion rotation)
	{
		Object.Instantiate<GameObject>(this.constructedPrefab, position, rotation);
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x000314FA File Offset: 0x0002F6FA
	[PunRPC]
	protected void AngleIt(PhotonView view, float angle)
	{
		view.transform.GetChild(0).transform.localEulerAngles = new Vector3(-45f + angle, 0f, 0f);
	}

	// Token: 0x0400088A RID: 2186
	public ConstructablePreview previewPrefab;

	// Token: 0x0400088B RID: 2187
	public GameObject constructedPrefab;

	// Token: 0x0400088C RID: 2188
	public float maxPreviewDistance;

	// Token: 0x0400088D RID: 2189
	public float maxConstructDistance;

	// Token: 0x0400088E RID: 2190
	public float maxConstructVerticalAngle;

	// Token: 0x0400088F RID: 2191
	public bool angleToNormal;

	// Token: 0x04000890 RID: 2192
	[SerializeField]
	public ConstructablePreview currentPreview;

	// Token: 0x04000891 RID: 2193
	public float angleOffset;

	// Token: 0x04000892 RID: 2194
	public bool isAngleable;

	// Token: 0x04000893 RID: 2195
	protected RaycastHit currentConstructHit;

	// Token: 0x04000894 RID: 2196
	protected bool constructing;

	// Token: 0x04000895 RID: 2197
	private RaycastHit[] hitCache;

	// Token: 0x04000896 RID: 2198
	private bool valid;
}
