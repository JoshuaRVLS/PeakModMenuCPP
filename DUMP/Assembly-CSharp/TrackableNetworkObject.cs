using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001BA RID: 442
[RequireComponent(typeof(PhotonView))]
public class TrackableNetworkObject : ItemComponent
{
	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000E28 RID: 3624 RVA: 0x0004738E File Offset: 0x0004558E
	// (set) Token: 0x06000E29 RID: 3625 RVA: 0x00047396 File Offset: 0x00045596
	public new PhotonView photonView { get; private set; }

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000E2A RID: 3626 RVA: 0x0004739F File Offset: 0x0004559F
	public bool hasTracker
	{
		get
		{
			return this.currentTracker != null;
		}
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x000473AD File Offset: 0x000455AD
	public override void Awake()
	{
		base.Awake();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x000473C1 File Offset: 0x000455C1
	private new void OnEnable()
	{
		TrackableNetworkObject.ALL_TRACKABLES.Add(this);
		GlobalEvents.OnItemConsumed = (Action<Item, Character>)Delegate.Combine(GlobalEvents.OnItemConsumed, new Action<Item, Character>(this.TestItemConsumed));
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x000473EE File Offset: 0x000455EE
	public new void OnDisable()
	{
		TrackableNetworkObject.ALL_TRACKABLES.Remove(this);
		GlobalEvents.OnItemConsumed = (Action<Item, Character>)Delegate.Remove(GlobalEvents.OnItemConsumed, new Action<Item, Character>(this.TestItemConsumed));
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x0004741C File Offset: 0x0004561C
	private void TestItemConsumed(Item consumedItem, Character consumer)
	{
		if (consumedItem == this.item && TrackableNetworkObject.OnTrackableObjectConsumed != null)
		{
			TrackableNetworkObject.OnTrackableObjectConsumed(this.instanceID);
		}
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x00047443 File Offset: 0x00045643
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom)
		{
			yield return null;
		}
		this.TryInitIfMaster();
		yield break;
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x00047452 File Offset: 0x00045652
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.TryInitIfMaster();
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x00047460 File Offset: 0x00045660
	private void TryInitIfMaster()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.Init();
		}
		if (TrackableNetworkObject.OnTrackableObjectCreated != null)
		{
			TrackableNetworkObject.OnTrackableObjectCreated(this.instanceID);
		}
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x00047488 File Offset: 0x00045688
	public static TrackableNetworkObject GetTrackableObject(int instanceID)
	{
		for (int i = 0; i < TrackableNetworkObject.ALL_TRACKABLES.Count; i++)
		{
			if (TrackableNetworkObject.ALL_TRACKABLES[i] != null && TrackableNetworkObject.ALL_TRACKABLES[i].instanceID == instanceID)
			{
				return TrackableNetworkObject.ALL_TRACKABLES[i];
			}
		}
		return null;
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x000474E0 File Offset: 0x000456E0
	private void Init()
	{
		if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (base.GetData<IntItemData>(DataEntryKey.InstanceID).Value != 0)
		{
			return;
		}
		this.instanceID = TrackableNetworkObject.currentMaxInstanceID;
		TrackableNetworkObject.currentMaxInstanceID++;
		this.photonView.RPC("SetInstanceIDRPC", RpcTarget.All, new object[]
		{
			this.instanceID,
			TrackableNetworkObject.currentMaxInstanceID,
			this.autoSpawnTracker
		});
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x00047562 File Offset: 0x00045762
	public override void OnInstanceDataSet()
	{
		this.instanceID = base.GetData<IntItemData>(DataEntryKey.InstanceID).Value;
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x00047578 File Offset: 0x00045778
	[PunRPC]
	public void SetInstanceIDRPC(int instanceID, int maxInstanceID, bool autoSpawnTracker)
	{
		base.GetData<IntItemData>(DataEntryKey.InstanceID).Value = instanceID;
		this.instanceID = instanceID;
		TrackableNetworkObject.currentMaxInstanceID = maxInstanceID;
		if (autoSpawnTracker)
		{
			Object.Instantiate<TrackNetworkedObject>(this.trackerToSpawn, base.transform.position, base.transform.rotation).SetObject(this);
		}
	}

	// Token: 0x04000BF0 RID: 3056
	public static List<TrackableNetworkObject> ALL_TRACKABLES = new List<TrackableNetworkObject>();

	// Token: 0x04000BF1 RID: 3057
	public int instanceID;

	// Token: 0x04000BF2 RID: 3058
	private static int currentMaxInstanceID = 1;

	// Token: 0x04000BF3 RID: 3059
	public TrackNetworkedObject currentTracker;

	// Token: 0x04000BF5 RID: 3061
	public static Action<int> OnTrackableObjectCreated;

	// Token: 0x04000BF6 RID: 3062
	public static Action<int> OnTrackableObjectConsumed;

	// Token: 0x04000BF7 RID: 3063
	public bool autoSpawnTracker;

	// Token: 0x04000BF8 RID: 3064
	public TrackNetworkedObject trackerToSpawn;
}
