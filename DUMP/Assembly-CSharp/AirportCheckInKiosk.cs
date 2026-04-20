using System;
using System.Collections;
using Peak.Network;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000A5 RID: 165
public class AirportCheckInKiosk : MonoBehaviourPun, IInteractibleConstant, IInteractible
{
	// Token: 0x0600064D RID: 1613 RVA: 0x00024966 File Offset: 0x00022B66
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00024969 File Offset: 0x00022B69
	public void Awake()
	{
		this.mpb = new MaterialPropertyBlock();
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x00024978 File Offset: 0x00022B78
	private void Start()
	{
		if (GameHandler.GetService<NextLevelService>().Data.IsSome)
		{
			Debug.Log(string.Format("seconds left until next map... {0}", GameHandler.GetService<NextLevelService>().Data.Value.SecondsLeft));
		}
		GameHandler.GetService<RichPresenceService>().SetState(RichPresenceState.Status_Airport);
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x000249CC File Offset: 0x00022BCC
	public void Interact(Character interactor)
	{
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000651 RID: 1617 RVA: 0x000249CE File Offset: 0x00022BCE
	// (set) Token: 0x06000652 RID: 1618 RVA: 0x000249FC File Offset: 0x00022BFC
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

	// Token: 0x06000653 RID: 1619 RVA: 0x00024A08 File Offset: 0x00022C08
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

	// Token: 0x06000654 RID: 1620 RVA: 0x00024A68 File Offset: 0x00022C68
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

	// Token: 0x06000655 RID: 1621 RVA: 0x00024AB8 File Offset: 0x00022CB8
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x00024AC5 File Offset: 0x00022CC5
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x00024ACD File Offset: 0x00022CCD
	public string GetInteractionText()
	{
		return LocalizedText.GetText("BOARDFLIGHT", true);
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x00024ADA File Offset: 0x00022CDA
	public string GetName()
	{
		return LocalizedText.GetText("GATEKIOSK", true);
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x00024AE7 File Offset: 0x00022CE7
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.IsInteractible(interactor);
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x00024AF0 File Offset: 0x00022CF0
	public float GetInteractTime(Character interactor)
	{
		return this.interactTime;
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x00024AF8 File Offset: 0x00022CF8
	public void Interact_CastFinished(Character interactor)
	{
		GUIManager.instance.boardingPass.Open();
		GUIManager.instance.boardingPass.kiosk = this;
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x00024B1C File Offset: 0x00022D1C
	public void StartGame(int ascent)
	{
		if (LoadingScreenHandler.loading)
		{
			return;
		}
		byte[] serializedRunSettings = RunSettings.GetSerializedRunSettings();
		base.photonView.RPC("LoadIslandMaster", RpcTarget.MasterClient, new object[]
		{
			ascent,
			serializedRunSettings
		});
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00024B5B File Offset: 0x00022D5B
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00024B5D File Offset: 0x00022D5D
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00024B60 File Offset: 0x00022D60
	[PunRPC]
	public void LoadIslandMaster(int ascent, byte[] serializedRunSettings)
	{
		if (!NetCode.Session.IsHost)
		{
			return;
		}
		Debug.Log("Loading scene as master.");
		int nextLevelIndexOrFallback = GameHandler.GetService<NextLevelService>().NextLevelIndexOrFallback;
		string text = SingletonAsset<MapBaker>.Instance.GetLevel(nextLevelIndexOrFallback + NextLevelService.debugLevelIndexOffset);
		if (string.IsNullOrEmpty(text))
		{
			text = "WilIsland";
		}
		base.photonView.RPC("BeginIslandLoadRPC", RpcTarget.All, new object[]
		{
			text,
			ascent,
			serializedRunSettings
		});
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00024BE0 File Offset: 0x00022DE0
	[PunRPC]
	public void BeginIslandLoadRPC(string sceneName, int ascent, byte[] serializedRunSettings)
	{
		MenuWindow.CloseAllWindows();
		GameHandler.AddStatus<SceneSwitchingStatus>(new SceneSwitchingStatus());
		Debug.Log("Begin scene load RPC: " + sceneName);
		Ascents.currentAscent = ascent;
		GameUtils.ApplySerializedRunSettings(serializedRunSettings);
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Plane, null, new IEnumerator[]
		{
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess(sceneName, true, true, 0f)
		});
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000661 RID: 1633 RVA: 0x00024C3F File Offset: 0x00022E3F
	public bool holdOnFinish { get; }

	// Token: 0x0400065B RID: 1627
	public float interactTime;

	// Token: 0x0400065C RID: 1628
	private MaterialPropertyBlock mpb;

	// Token: 0x0400065D RID: 1629
	private MeshRenderer[] _mr;
}
