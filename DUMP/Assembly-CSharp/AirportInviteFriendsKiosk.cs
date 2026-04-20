using System;
using Steamworks;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class AirportInviteFriendsKiosk : MonoBehaviour, IInteractible
{
	// Token: 0x06000457 RID: 1111 RVA: 0x0001B20D File Offset: 0x0001940D
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x06000458 RID: 1112 RVA: 0x0001B210 File Offset: 0x00019410
	// (set) Token: 0x06000459 RID: 1113 RVA: 0x0001B23E File Offset: 0x0001943E
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

	// Token: 0x0600045A RID: 1114 RVA: 0x0001B247 File Offset: 0x00019447
	public void Awake()
	{
		this.mpb = new MaterialPropertyBlock();
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x0001B254 File Offset: 0x00019454
	public void Interact(Character interactor)
	{
		CSteamID steamIDLobby;
		if (GameHandler.GetService<SteamLobbyHandler>().InSteamLobby(out steamIDLobby))
		{
			Debug.Log("Open Invite Friends UI...");
			SteamFriends.ActivateGameOverlayInviteDialog(steamIDLobby);
		}
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x0001B280 File Offset: 0x00019480
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

	// Token: 0x0600045D RID: 1117 RVA: 0x0001B2E0 File Offset: 0x000194E0
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

	// Token: 0x0600045E RID: 1118 RVA: 0x0001B330 File Offset: 0x00019530
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x0001B33D File Offset: 0x0001953D
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0001B345 File Offset: 0x00019545
	public string GetInteractionText()
	{
		return LocalizedText.GetText("INVITEFRIENDS", true);
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x0001B352 File Offset: 0x00019552
	public string GetName()
	{
		return LocalizedText.GetText("INVITEKIOSK", true);
	}

	// Token: 0x040004C7 RID: 1223
	private MaterialPropertyBlock mpb;

	// Token: 0x040004C8 RID: 1224
	private MeshRenderer[] _mr;
}
