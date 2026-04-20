using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Peak;
using Peak.Network;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class Luggage : Spawner, IInteractibleConstant, IInteractible
{
	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000976 RID: 2422 RVA: 0x00032B01 File Offset: 0x00030D01
	public bool IsOpen
	{
		get
		{
			return this.state == Luggage.LuggageState.Open;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000977 RID: 2423 RVA: 0x00032B0C File Offset: 0x00030D0C
	// (set) Token: 0x06000978 RID: 2424 RVA: 0x00032B28 File Offset: 0x00030D28
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

	// Token: 0x06000979 RID: 2425 RVA: 0x00032B34 File Offset: 0x00030D34
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.anim = base.GetComponent<Animator>();
		this.mpb = new MaterialPropertyBlock();
		PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Combine(PlayerHandler.CharacterRegistered, new Action<Character>(this.OnCharacterRegistered));
		Luggage.ALL_LUGGAGE.Add(this);
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x00032B8F File Offset: 0x00030D8F
	public virtual void Interact(Character interactor)
	{
		this.anim.Play("Luggage_Unclasp");
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x00032BA4 File Offset: 0x00030DA4
	[PunRPC]
	protected void OpenLuggageRPC(bool spawnItems)
	{
		if (this.state == Luggage.LuggageState.Open)
		{
			return;
		}
		if (spawnItems)
		{
			this.anim.Play("Luggage_Open");
		}
		this.state = Luggage.LuggageState.Open;
		if (spawnItems)
		{
			base.StartCoroutine(this.<OpenLuggageRPC>g__SpawnItemRoutine|16_0());
			return;
		}
		base.StartCoroutine(this.<OpenLuggageRPC>g__OpenLaterRoutine|16_1());
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x00032BF3 File Offset: 0x00030DF3
	private void OnDestroy()
	{
		PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Remove(PlayerHandler.CharacterRegistered, new Action<Character>(this.OnCharacterRegistered));
		if (Luggage.ALL_LUGGAGE.Contains(this))
		{
			Luggage.ALL_LUGGAGE.Remove(this);
		}
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00032C30 File Offset: 0x00030E30
	public Vector3 Center()
	{
		return HelperFunctions.GetTotalBounds(this.meshRenderers).center;
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00032C50 File Offset: 0x00030E50
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00032C58 File Offset: 0x00030E58
	public virtual string GetInteractionText()
	{
		return LocalizedText.GetText("OPEN", true);
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00032C65 File Offset: 0x00030E65
	public string GetName()
	{
		return LocalizedText.GetText(this.displayName, true);
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x00032C73 File Offset: 0x00030E73
	public bool IsInteractible(Character interactor)
	{
		return this.state == Luggage.LuggageState.Closed;
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x00032C80 File Offset: 0x00030E80
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

	// Token: 0x06000983 RID: 2435 RVA: 0x00032CE0 File Offset: 0x00030EE0
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

	// Token: 0x06000984 RID: 2436 RVA: 0x00032D40 File Offset: 0x00030F40
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x00032D42 File Offset: 0x00030F42
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.state == Luggage.LuggageState.Closed;
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x00032D4D File Offset: 0x00030F4D
	public float GetInteractTime(Character interactor)
	{
		return this.timeToOpen;
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x00032D58 File Offset: 0x00030F58
	public void OnCharacterRegistered(Character newCharacter)
	{
		if (!this.photonView.IsMine || this.state != Luggage.LuggageState.Open || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.photonView.RPC("OpenLuggageRPC", newCharacter.photonView.Owner, new object[]
		{
			false
		});
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00032DB3 File Offset: 0x00030FB3
	public void OpenImmediatelyNoNotify()
	{
		if (this.state == Luggage.LuggageState.Open)
		{
			Debug.LogWarning("Attempted to open " + base.name + " fwhich is already open", this);
			return;
		}
		base.StartCoroutine(this.<OpenImmediatelyNoNotify>g__WaitForRoomAndOpen|29_0());
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x00032DE7 File Offset: 0x00030FE7
	public virtual void Interact_CastFinished(Character interactor)
	{
		if (this.state == Luggage.LuggageState.Closed)
		{
			this.photonView.RPC("OpenLuggageRPC", RpcTarget.All, new object[]
			{
				true
			});
			GlobalEvents.TriggerLuggageOpened(this, interactor);
		}
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x00032E18 File Offset: 0x00031018
	public void CancelCast(Character interactor)
	{
		this.anim.SetTrigger("Reclasp");
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x0600098B RID: 2443 RVA: 0x00032E2A File Offset: 0x0003102A
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x00032E30 File Offset: 0x00031030
	protected override void OffsetSpawn(Item item)
	{
		if (item.offsetLuggageSpawn && !(this is RespawnChest))
		{
			item.transform.position += base.transform.rotation * item.offsetLuggagePosition;
			item.transform.rotation *= Quaternion.Euler(item.offsetLuggageRotation);
		}
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x00032EAE File Offset: 0x000310AE
	[CompilerGenerated]
	private IEnumerator <OpenLuggageRPC>g__SpawnItemRoutine|16_0()
	{
		if (!NetCode.Session.IsHost)
		{
			yield break;
		}
		while (!Luggage.ALL_LUGGAGE.Contains(this))
		{
			yield return null;
		}
		Luggage.ALL_LUGGAGE.Remove(this);
		yield return new WaitForSeconds(0.1f);
		SpawnedItemTracker spawnedItemTracker;
		bool flag = this.HasSpawnTracking(out spawnedItemTracker);
		if (flag && spawnedItemTracker.HasSpawnHistory)
		{
			using (List<PhotonView>.Enumerator enumerator = spawnedItemTracker.SpawnAndTrackFromItemHistory().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PhotonView photonView = enumerator.Current;
					base.InitializePhysics(photonView.GetComponent<Item>());
				}
				yield break;
			}
		}
		List<PhotonView> spawnedItems = this.SpawnItems(this.GetSpawnSpots());
		if (flag)
		{
			spawnedItemTracker.TrackSpawnedItems(spawnedItems);
		}
		yield break;
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x00032EBD File Offset: 0x000310BD
	[CompilerGenerated]
	private IEnumerator <OpenLuggageRPC>g__OpenLaterRoutine|16_1()
	{
		Luggage.ALL_LUGGAGE.Remove(this);
		yield return new WaitForSeconds(this.timeToOpen);
		this.anim.Play("Luggage_Open");
		yield break;
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x00032ECC File Offset: 0x000310CC
	[CompilerGenerated]
	private IEnumerator <OpenImmediatelyNoNotify>g__WaitForRoomAndOpen|29_0()
	{
		while (!PhotonNetwork.InRoom)
		{
			yield return null;
		}
		this.photonView.RPC("OpenLuggageRPC", RpcTarget.All, new object[]
		{
			true
		});
		yield break;
	}

	// Token: 0x040008CC RID: 2252
	public string displayName;

	// Token: 0x040008CD RID: 2253
	private Animator anim;

	// Token: 0x040008CE RID: 2254
	[SerializeField]
	protected Luggage.LuggageState state;

	// Token: 0x040008CF RID: 2255
	private new PhotonView photonView;

	// Token: 0x040008D0 RID: 2256
	public float timeToOpen;

	// Token: 0x040008D1 RID: 2257
	private MaterialPropertyBlock mpb;

	// Token: 0x040008D2 RID: 2258
	public static List<Luggage> ALL_LUGGAGE = new List<Luggage>();

	// Token: 0x040008D3 RID: 2259
	private MeshRenderer[] _mr;

	// Token: 0x02000471 RID: 1137
	public enum LuggageState
	{
		// Token: 0x04001980 RID: 6528
		Closed,
		// Token: 0x04001981 RID: 6529
		Open
	}
}
