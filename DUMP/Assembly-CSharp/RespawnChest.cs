using System;
using System.Collections.Generic;
using Peak;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000320 RID: 800
public class RespawnChest : Luggage, IInteractible
{
	// Token: 0x17000161 RID: 353
	// (get) Token: 0x0600155A RID: 5466 RVA: 0x0006C369 File Offset: 0x0006A569
	// (set) Token: 0x0600155B RID: 5467 RVA: 0x0006C371 File Offset: 0x0006A571
	public Segment SegmentNumber { get; set; }

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x0600155C RID: 5468 RVA: 0x0006C37A File Offset: 0x0006A57A
	public bool IsSpent
	{
		get
		{
			return this.state == Luggage.LuggageState.Open;
		}
	}

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x0600155D RID: 5469 RVA: 0x0006C388 File Offset: 0x0006A588
	// (remove) Token: 0x0600155E RID: 5470 RVA: 0x0006C3C0 File Offset: 0x0006A5C0
	public event Action<RespawnChest> ReviveUsed;

	// Token: 0x0600155F RID: 5471 RVA: 0x0006C3F5 File Offset: 0x0006A5F5
	public override string GetInteractionText()
	{
		if (Character.PlayerIsDeadOrDown())
		{
			return LocalizedText.GetText("REVIVESCOUTS", true);
		}
		return LocalizedText.GetText("TOUCH", true);
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x0006C415 File Offset: 0x0006A615
	private void DebugSpawn()
	{
		this.SpawnItems(this.GetSpawnSpots());
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x0006C424 File Offset: 0x0006A624
	public override void Interact(Character interactor)
	{
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x0006C426 File Offset: 0x0006A626
	public override void Interact_CastFinished(Character interactor)
	{
		base.Interact_CastFinished(interactor);
		GlobalEvents.TriggerRespawnChestOpened(this, interactor);
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x0006C438 File Offset: 0x0006A638
	public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		SpawnedItemTracker spawnedItemTracker;
		if (this.HasSpawnTracking(out spawnedItemTracker) && spawnedItemTracker.HasSpawnHistory)
		{
			return spawnedItemTracker.SpawnAndTrackFromItemHistory();
		}
		if (Ascents.canReviveDead && Character.PlayerIsDeadOrDown())
		{
			base.photonView.RPC("RemoveSkeletonRPC", RpcTarget.AllBuffered, Array.Empty<object>());
			this.RespawnAllPlayersHere();
		}
		else
		{
			list = base.SpawnItems(spawnSpots);
			spawnedItemTracker.TrackSpawnedItems(list);
		}
		return list;
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x0006C4A9 File Offset: 0x0006A6A9
	public void Break()
	{
		base.photonView.RPC("OpenLuggageRPC", RpcTarget.AllBuffered, new object[]
		{
			false
		});
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x0006C4CB File Offset: 0x0006A6CB
	[PunRPC]
	private void RemoveSkeletonRPC()
	{
		this.skeleton.SetActive(false);
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06001566 RID: 5478 RVA: 0x0006C4D9 File Offset: 0x0006A6D9
	public Vector3 RandomRevivePoint
	{
		get
		{
			return base.transform.position + base.transform.up * 6f + Random.onUnitSphere;
		}
	}

	// Token: 0x06001567 RID: 5479 RVA: 0x0006C50C File Offset: 0x0006A70C
	private void RespawnAllPlayersHere()
	{
		Action<RespawnChest> reviveUsed = this.ReviveUsed;
		if (reviveUsed != null)
		{
			reviveUsed(this);
		}
		foreach (Character character in Character.AllCharacters)
		{
			if (character.data.dead || character.data.fullyPassedOut)
			{
				character.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[]
				{
					this.RandomRevivePoint,
					true,
					(int)this.SegmentNumber
				});
			}
		}
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x0006C5C0 File Offset: 0x0006A7C0
	public new bool IsInteractible(Character interactor)
	{
		return this.state == Luggage.LuggageState.Closed;
	}

	// Token: 0x0400138B RID: 5003
	public GameObject skeleton;
}
