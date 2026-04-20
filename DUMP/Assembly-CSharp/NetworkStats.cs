using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000153 RID: 339
public class NetworkStats : Singleton<NetworkStats>
{
	// Token: 0x06000B35 RID: 2869 RVA: 0x0003C670 File Offset: 0x0003A870
	private void Update()
	{
		this.m_timer += Time.deltaTime;
		if (this.m_timer > 1f)
		{
			this.m_timer -= 1f;
			this.m_lastRecievedDelta = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesIn - this.m_bytesReceivedLastSecond;
			this.m_bytesReceivedLastSecond = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesIn;
			this.m_lastSentDelta = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesOut - this.m_bytesSentLastSecond;
			this.m_bytesSentLastSecond = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesOut;
			foreach (KeyValuePair<string, ulong> keyValuePair in this.m_binaryStreamsByType)
			{
				string key = keyValuePair.Key;
				ulong value = keyValuePair.Value;
				this.<Update>g__UpdateEntry|8_0(key, value);
			}
			this.<Update>g__UpdateEntry|8_0("VoiceData", (ulong)PhotonVoiceStats.bytesSent);
		}
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x0003C77C File Offset: 0x0003A97C
	public static void RegisterBytesSent<T>(ulong bytesSent)
	{
		Type typeFromHandle = typeof(T);
		if (!Singleton<NetworkStats>.Instance.m_binaryStreamsByType.ContainsKey(typeFromHandle.Name))
		{
			Singleton<NetworkStats>.Instance.m_binaryStreamsByType.Add(typeFromHandle.Name, 0UL);
		}
		Dictionary<string, ulong> binaryStreamsByType = Singleton<NetworkStats>.Instance.m_binaryStreamsByType;
		string name = typeFromHandle.Name;
		binaryStreamsByType[name] += bytesSent;
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0003C7E4 File Offset: 0x0003A9E4
	public List<ValueTuple<string, ulong>> GetBytesSent()
	{
		return (from pair in this.m_binaryStreamsByType
		select new ValueTuple<string, ulong>(pair.Key, pair.Value)).ToList<ValueTuple<string, ulong>>();
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x0003C815 File Offset: 0x0003AA15
	public List<ValueTuple<string, ulong>> GetBytesDeltaSent()
	{
		return (from pair in this.m_binaryStreamsByTypeDelta
		select new ValueTuple<string, ulong>(pair.Key, pair.Value)).ToList<ValueTuple<string, ulong>>();
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x0003C870 File Offset: 0x0003AA70
	[CompilerGenerated]
	private void <Update>g__UpdateEntry|8_0(string key, ulong value)
	{
		if (this.m_binaryStreamsByTypeSecond.ContainsKey(key))
		{
			ulong num = this.m_binaryStreamsByTypeSecond[key];
			ulong value2 = value - num;
			this.m_binaryStreamsByTypeDelta[key] = value2;
		}
		this.m_binaryStreamsByTypeSecond[key] = value;
	}

	// Token: 0x04000A50 RID: 2640
	public long m_bytesReceivedLastSecond;

	// Token: 0x04000A51 RID: 2641
	public long m_lastRecievedDelta;

	// Token: 0x04000A52 RID: 2642
	public long m_bytesSentLastSecond;

	// Token: 0x04000A53 RID: 2643
	public long m_lastSentDelta;

	// Token: 0x04000A54 RID: 2644
	private float m_timer;

	// Token: 0x04000A55 RID: 2645
	private Dictionary<string, ulong> m_binaryStreamsByType = new Dictionary<string, ulong>();

	// Token: 0x04000A56 RID: 2646
	private Dictionary<string, ulong> m_binaryStreamsByTypeSecond = new Dictionary<string, ulong>();

	// Token: 0x04000A57 RID: 2647
	private Dictionary<string, ulong> m_binaryStreamsByTypeDelta = new Dictionary<string, ulong>();
}
