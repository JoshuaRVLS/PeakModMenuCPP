using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000158 RID: 344
public class ItemPhysicsSyncer : PhotonBinaryStreamSerializer<ItemPhysicsSyncData>
{
	// Token: 0x06000B65 RID: 2917 RVA: 0x0003D151 File Offset: 0x0003B351
	protected override void Awake()
	{
		base.Awake();
		this.m_photonView = base.GetComponent<PhotonView>();
		this.m_item = base.GetComponent<Item>();
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0003D171 File Offset: 0x0003B371
	public void ForceSyncForFrames(int frames = 10)
	{
		this.forceSyncFrames = frames;
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x0003D17C File Offset: 0x0003B37C
	private void FixedUpdate()
	{
		Rigidbody rig = this.m_item.rig;
		if (rig == null)
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (this.m_photonView.IsMine)
		{
			return;
		}
		if (this.RemoteValue.IsNone)
		{
			if (this.debug)
			{
				Debug.Log("NO REMOTE VALUE");
			}
			return;
		}
		if (!this.shouldSync)
		{
			if (this.debug)
			{
				Debug.Log("NOT SYNCING");
			}
			return;
		}
		if (this.m_item.itemState != ItemState.Ground)
		{
			return;
		}
		if (this.m_lastPos.IsNone)
		{
			if (this.debug)
			{
				Debug.Log("NO LAST POSITION");
			}
			return;
		}
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackage += Time.fixedDeltaTime * 0.6f;
		float t = (float)((double)this.sinceLastPackage / num);
		ItemPhysicsSyncData value = this.RemoteValue.Value;
		Vector3 b = value.position;
		Vector3 vector = Vector3.Lerp(this.m_lastPos.Value, b, t);
		Vector3 a = vector - rig.position;
		this.lastRecievedPosition = vector;
		rig.MovePosition(rig.position + a * 0.5f);
		if (this.debug)
		{
			Debug.Log(string.Format("Rotating from {0} to {1}", rig.rotation, value.rotation));
		}
		rig.MoveRotation(Quaternion.RotateTowards(rig.rotation, value.rotation, Time.fixedDeltaTime * this.maxAngleChangePerSecond));
		if (this.debug)
		{
			string str = "MOVING TO POSITION ";
			Vector3 vector2 = vector;
			Debug.Log(str + vector2.ToString());
		}
	}

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000B68 RID: 2920 RVA: 0x0003D329 File Offset: 0x0003B529
	protected virtual float maxAngleChangePerSecond
	{
		get
		{
			return 90f;
		}
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x0003D330 File Offset: 0x0003B530
	public void ResetRecievedData()
	{
		this.lastRecievedPosition = base.transform.position;
		this.RemoteValue = Optionable<ItemPhysicsSyncData>.None;
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x0003D350 File Offset: 0x0003B550
	public override ItemPhysicsSyncData GetDataToWrite()
	{
		ItemPhysicsSyncData itemPhysicsSyncData = default(ItemPhysicsSyncData);
		Rigidbody rig = this.m_item.rig;
		if (rig != null)
		{
			itemPhysicsSyncData.linearVelocity = rig.linearVelocity;
			itemPhysicsSyncData.angularVelocity = rig.angularVelocity;
			itemPhysicsSyncData.position = rig.position;
			itemPhysicsSyncData.rotation = rig.rotation;
			if (this.debug)
			{
				string str = "SENDING POSITION ";
				float3 position = itemPhysicsSyncData.position;
				Debug.Log(str + position.ToString());
			}
		}
		return itemPhysicsSyncData;
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x0003D3EC File Offset: 0x0003B5EC
	public override bool ShouldSendData()
	{
		if (this.forceSyncFrames > 0 && this.m_item.itemState == ItemState.Ground)
		{
			this.forceSyncFrames--;
			return true;
		}
		return this.shouldSync && (!this.m_item.rig.isKinematic && !this.m_item.rig.IsSleeping()) && this.m_item.itemState == ItemState.Ground;
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0003D460 File Offset: 0x0003B660
	public override void OnDataReceived(ItemPhysicsSyncData data)
	{
		if (this.debug)
		{
			Debug.Log("RECIEVED DATA 1");
		}
		base.OnDataReceived(data);
		Rigidbody rig = this.m_item.rig;
		if (rig == null)
		{
			return;
		}
		if (this.m_item.itemState != ItemState.Ground)
		{
			return;
		}
		if (!this.shouldSync)
		{
			if (this.debug)
			{
				Debug.Log("SHOULDSYNC OFF");
			}
			return;
		}
		if (rig.isKinematic)
		{
			return;
		}
		if (this.debug)
		{
			Debug.Log("RECIEVED DATA 2");
		}
		this.m_lastPos = Optionable<Vector3>.Some(rig.position);
		rig.linearVelocity = data.linearVelocity;
		rig.angularVelocity = data.angularVelocity;
		this.lastRecievedLinearVelocity = data.linearVelocity;
		this.lastRecievedAngularVelocity = data.angularVelocity;
	}

	// Token: 0x04000A78 RID: 2680
	private Item m_item;

	// Token: 0x04000A79 RID: 2681
	private PhotonView m_photonView;

	// Token: 0x04000A7A RID: 2682
	private Optionable<Vector3> m_lastPos;

	// Token: 0x04000A7B RID: 2683
	public bool debug;

	// Token: 0x04000A7C RID: 2684
	[SerializeField]
	internal bool shouldSync = true;

	// Token: 0x04000A7D RID: 2685
	private int forceSyncFrames;

	// Token: 0x04000A7E RID: 2686
	[SerializeField]
	private Vector3 lastRecievedLinearVelocity;

	// Token: 0x04000A7F RID: 2687
	[SerializeField]
	private Vector3 lastRecievedAngularVelocity;

	// Token: 0x04000A80 RID: 2688
	[SerializeField]
	private Vector3 lastRecievedPosition;
}
