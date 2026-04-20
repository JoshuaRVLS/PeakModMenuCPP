using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200015C RID: 348
public class PhysicsSyncer : PhotonBinaryStreamSerializer<ItemPhysicsSyncData>
{
	// Token: 0x06000B7B RID: 2939 RVA: 0x0003D805 File Offset: 0x0003BA05
	protected override void Awake()
	{
		base.Awake();
		this.rig = base.GetComponent<Rigidbody>();
		this.m_photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x0003D825 File Offset: 0x0003BA25
	public void ForceSyncForFrames(int frames = 10)
	{
		this.forceSyncFrames = frames;
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x0003D830 File Offset: 0x0003BA30
	private void FixedUpdate()
	{
		if (this.rig == null)
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
		Vector3 a = vector - this.rig.position;
		this.lastRecievedPosition = vector;
		if (this.debug)
		{
			Debug.Log(string.Format("Position before move: {0}", this.rig.position));
		}
		this.rig.MovePosition(this.rig.position + a * 0.5f);
		this.rig.MoveRotation(Quaternion.RotateTowards(this.rig.rotation, value.rotation, Time.fixedDeltaTime * this.maxAngleChangePerSecond));
		if (this.debug)
		{
			string str = "MOVING TO POSITION ";
			Vector3 vector2 = vector;
			Debug.Log(str + vector2.ToString());
		}
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x0003D9D9 File Offset: 0x0003BBD9
	public void ResetRecievedData()
	{
		this.lastRecievedPosition = base.transform.position;
		this.RemoteValue = Optionable<ItemPhysicsSyncData>.None;
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x0003D9F8 File Offset: 0x0003BBF8
	public override ItemPhysicsSyncData GetDataToWrite()
	{
		ItemPhysicsSyncData itemPhysicsSyncData = default(ItemPhysicsSyncData);
		if (this.rig != null)
		{
			itemPhysicsSyncData.linearVelocity = this.rig.linearVelocity;
			itemPhysicsSyncData.angularVelocity = this.rig.angularVelocity;
			itemPhysicsSyncData.position = this.rig.position;
			itemPhysicsSyncData.rotation = this.rig.rotation;
			if (this.debug)
			{
				string str = "SENDING POSITION ";
				float3 position = itemPhysicsSyncData.position;
				Debug.Log(str + position.ToString());
			}
		}
		return itemPhysicsSyncData;
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x0003DAA1 File Offset: 0x0003BCA1
	public override bool ShouldSendData()
	{
		return true;
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x0003DAA4 File Offset: 0x0003BCA4
	public override void OnDataReceived(ItemPhysicsSyncData data)
	{
		if (this.debug)
		{
			Debug.Log("RECIEVED DATA 1");
		}
		base.OnDataReceived(data);
		if (this.rig == null)
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
		if (this.rig.isKinematic)
		{
			return;
		}
		if (this.debug)
		{
			Debug.Log("RECIEVED DATA 2");
		}
		this.m_lastPos = Optionable<Vector3>.Some(this.rig.position);
		this.rig.linearVelocity = data.linearVelocity;
		this.rig.angularVelocity = data.angularVelocity;
		this.lastRecievedLinearVelocity = data.linearVelocity;
		this.lastRecievedAngularVelocity = data.angularVelocity;
	}

	// Token: 0x04000A8A RID: 2698
	private PhotonView m_photonView;

	// Token: 0x04000A8B RID: 2699
	private Optionable<Vector3> m_lastPos;

	// Token: 0x04000A8C RID: 2700
	private Rigidbody rig;

	// Token: 0x04000A8D RID: 2701
	public bool debug;

	// Token: 0x04000A8E RID: 2702
	[SerializeField]
	internal bool shouldSync = true;

	// Token: 0x04000A8F RID: 2703
	private int forceSyncFrames;

	// Token: 0x04000A90 RID: 2704
	public float maxAngleChangePerSecond = 180f;

	// Token: 0x04000A91 RID: 2705
	[SerializeField]
	private Vector3 lastRecievedLinearVelocity;

	// Token: 0x04000A92 RID: 2706
	[SerializeField]
	private Vector3 lastRecievedAngularVelocity;

	// Token: 0x04000A93 RID: 2707
	[SerializeField]
	private Vector3 lastRecievedPosition;
}
