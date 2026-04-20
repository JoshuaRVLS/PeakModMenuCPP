using System;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x0200015D RID: 349
public class PositionSyncer : PhotonBinaryStreamSerializer<PositionSyncer.Pos>
{
	// Token: 0x06000B83 RID: 2947 RVA: 0x0003DB94 File Offset: 0x0003BD94
	public override PositionSyncer.Pos GetDataToWrite()
	{
		this.lastSent = Optionable<float3>.Some(base.transform.position);
		return new PositionSyncer.Pos
		{
			Position = base.transform.position
		};
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x0003DBDC File Offset: 0x0003BDDC
	public override void OnDataReceived(PositionSyncer.Pos data)
	{
		base.OnDataReceived(data);
		this.currentPos = base.transform.position;
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x0003DBF8 File Offset: 0x0003BDF8
	public override bool ShouldSendData()
	{
		PositionSyncer.<>c__DisplayClass6_0 CS$<>8__locals1;
		CS$<>8__locals1.last = this.lastSent.Value;
		CS$<>8__locals1.n = base.transform.position;
		if (!PositionSyncer.<ShouldSendData>g__IsSame|6_0(ref CS$<>8__locals1))
		{
			return true;
		}
		if (this.forceSyncFrames > 0)
		{
			this.forceSyncFrames--;
			return true;
		}
		return false;
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x0003DC50 File Offset: 0x0003BE50
	private void Update()
	{
		if (this.photonView.IsMine)
		{
			return;
		}
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackage += Time.deltaTime;
		float t = (float)((double)this.sinceLastPackage / num);
		if (this.RemoteValue.IsSome)
		{
			PositionSyncer.Pos value = this.RemoteValue.Value;
			base.transform.position = Vector3.Lerp(this.currentPos, value.Position, t);
		}
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x0003DCD1 File Offset: 0x0003BED1
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (this.photonView.IsMine)
		{
			this.forceSyncFrames = 10;
		}
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x0003DCF8 File Offset: 0x0003BEF8
	[CompilerGenerated]
	internal static bool <ShouldSendData>g__IsSame|6_0(ref PositionSyncer.<>c__DisplayClass6_0 A_0)
	{
		return Mathf.Approximately(A_0.last.x, A_0.n.x) && Mathf.Approximately(A_0.last.y, A_0.n.y) && Mathf.Approximately(A_0.last.z, A_0.n.z);
	}

	// Token: 0x04000A94 RID: 2708
	private Vector3 currentPos;

	// Token: 0x04000A95 RID: 2709
	private int forceSyncFrames;

	// Token: 0x04000A96 RID: 2710
	private Optionable<float3> lastSent;

	// Token: 0x0200049B RID: 1179
	public struct Pos : IBinarySerializable
	{
		// Token: 0x06001D05 RID: 7429 RVA: 0x00089307 File Offset: 0x00087507
		public void Serialize(BinarySerializer serializer)
		{
			serializer.WriteHalf3((half3)this.Position);
		}

		// Token: 0x06001D06 RID: 7430 RVA: 0x0008931A File Offset: 0x0008751A
		public void Deserialize(BinaryDeserializer deserializer)
		{
			this.Position = deserializer.ReadHalf3();
		}

		// Token: 0x04001A34 RID: 6708
		public float3 Position;
	}
}
