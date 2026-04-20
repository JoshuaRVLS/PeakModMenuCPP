using System;
using System.Collections;
using Photon.Pun;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000055 RID: 85
public class AirportPhotoboothKiosk : MonoBehaviour, IInteractible
{
	// Token: 0x06000463 RID: 1123 RVA: 0x0001B367 File Offset: 0x00019567
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x06000464 RID: 1124 RVA: 0x0001B36A File Offset: 0x0001956A
	// (set) Token: 0x06000465 RID: 1125 RVA: 0x0001B398 File Offset: 0x00019598
	private MeshRenderer[] meshRenderers
	{
		get
		{
			if (this._mr == null)
			{
				this._mr = base.GetComponentsInChildren<MeshRenderer>();
				MonoBehaviour.print(this._mr.Length);
			}
			return this._mr;
		}
		set
		{
			this._mr = value;
		}
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x0001B3A1 File Offset: 0x000195A1
	public void Awake()
	{
		this.mpb = new MaterialPropertyBlock();
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x0001B3AE File Offset: 0x000195AE
	private void Start()
	{
		this.flashImage.enabled = !GUIManager.instance.photosensitivity;
		this.photosensitiveFlashImage.enabled = GUIManager.instance.photosensitivity;
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x0001B3E0 File Offset: 0x000195E0
	private void Update()
	{
		this.inPhotobooth = (Character.localCharacter != null && Character.localCharacter.Center.x < this.insidePlaneTf.position.x);
		this.displayCamera.enabled = this.inPhotobooth;
		this.screen.SetActive(this.inPhotobooth);
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x0001B446 File Offset: 0x00019646
	public void Interact(Character interactor)
	{
		if (!this.takingPhoto)
		{
			this.view.RPC("InteractRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0001B466 File Offset: 0x00019666
	[PunRPC]
	private void InteractRPC()
	{
		this.takingPhoto = true;
		base.StartCoroutine(this.PhotoboothRoutine());
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x0001B47C File Offset: 0x0001967C
	private IEnumerator PhotoboothRoutine()
	{
		this.anim.SetTrigger("Start");
		yield return new WaitForSeconds(3f);
		this.actualCamera.targetTexture = this.photoTextures[0];
		this.actualCamera.Render();
		yield return new WaitForSeconds(1f);
		this.anim.SetTrigger("Start");
		yield return new WaitForSeconds(3f);
		this.actualCamera.targetTexture = this.photoTextures[1];
		this.actualCamera.Render();
		yield return new WaitForSeconds(1f);
		this.anim.SetTrigger("Start");
		yield return new WaitForSeconds(3f);
		this.actualCamera.targetTexture = this.photoTextures[2];
		this.actualCamera.Render();
		yield return new WaitForSeconds(1f);
		this.anim.SetTrigger("Start");
		yield return new WaitForSeconds(3f);
		this.actualCamera.targetTexture = this.photoTextures[3];
		this.actualCamera.Render();
		yield return new WaitForSeconds(1f);
		if (this.inPhotobooth)
		{
			this.photoCanvas.SetActive(true);
			yield return new WaitForSeconds(3f);
			SteamScreenshots.TriggerScreenshot();
			this.takingPhoto = false;
			yield return new WaitForSeconds(2f);
			this.photoCanvas.SetActive(false);
		}
		else
		{
			yield return new WaitForSeconds(5f);
			this.takingPhoto = false;
		}
		yield break;
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0001B48C File Offset: 0x0001968C
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

	// Token: 0x0600046D RID: 1133 RVA: 0x0001B4EC File Offset: 0x000196EC
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				this.meshRenderers[i].SetPropertyBlock(this.mpb);
			}
		}
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x0001B53C File Offset: 0x0001973C
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x0001B549 File Offset: 0x00019749
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x0001B551 File Offset: 0x00019751
	public string GetInteractionText()
	{
		return LocalizedText.GetText("START2", true);
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x0001B55E File Offset: 0x0001975E
	public string GetName()
	{
		return LocalizedText.GetText("PHOTOBOOTH", true);
	}

	// Token: 0x040004C9 RID: 1225
	public PhotonView view;

	// Token: 0x040004CA RID: 1226
	public Camera displayCamera;

	// Token: 0x040004CB RID: 1227
	public Camera actualCamera;

	// Token: 0x040004CC RID: 1228
	public Animator anim;

	// Token: 0x040004CD RID: 1229
	private MaterialPropertyBlock mpb;

	// Token: 0x040004CE RID: 1230
	public GameObject photoCanvas;

	// Token: 0x040004CF RID: 1231
	public GameObject screen;

	// Token: 0x040004D0 RID: 1232
	public RenderTexture[] photoTextures;

	// Token: 0x040004D1 RID: 1233
	public Image flashImage;

	// Token: 0x040004D2 RID: 1234
	public Image photosensitiveFlashImage;

	// Token: 0x040004D3 RID: 1235
	public Transform insidePlaneTf;

	// Token: 0x040004D4 RID: 1236
	private bool inPhotobooth;

	// Token: 0x040004D5 RID: 1237
	private MeshRenderer[] _mr;

	// Token: 0x040004D6 RID: 1238
	private bool takingPhoto;
}
