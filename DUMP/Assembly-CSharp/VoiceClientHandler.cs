using System;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020001F9 RID: 505
public class VoiceClientHandler : MonoBehaviour
{
	// Token: 0x06000FDF RID: 4063 RVA: 0x0004DC14 File Offset: 0x0004BE14
	private void Awake()
	{
		PunVoiceClient component = base.GetComponent<PunVoiceClient>();
		if (component == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (PunVoiceClient.Instance != component)
		{
			Debug.Log("Already Found VoiceClient, Destroying...");
			Object.Destroy(base.gameObject);
			return;
		}
		base.transform.SetParent(null);
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x0004DC78 File Offset: 0x0004BE78
	private void Start()
	{
		VoiceClientHandler.m_VoiceConnection = base.GetComponent<VoiceConnection>();
		if (VoiceClientHandler.m_VoiceConnection.Client.State != ClientState.Joined)
		{
			VoiceClientHandler.m_VoiceConnection.Client.StateChanged += this.OnStateChanged;
			return;
		}
		VoiceClientHandler.InitNetworkVoice();
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x0004DCC4 File Offset: 0x0004BEC4
	private void OnStateChanged(ClientState state, ClientState toState)
	{
		if (toState == ClientState.Joined)
		{
			VoiceClientHandler.InitNetworkVoice();
		}
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x0004DCD0 File Offset: 0x0004BED0
	public static void InitNetworkVoice()
	{
		if (VoiceClientHandler.m_LocalRecorder == null || VoiceClientHandler.m_VoiceConnection == null || VoiceClientHandler.m_VoiceConnection.Client.State != ClientState.Joined)
		{
			return;
		}
		VoiceClientHandler.m_VoiceConnection.Client.LoadBalancingPeer.OpChangeGroups(Array.Empty<byte>(), Array.Empty<byte>());
		VoiceClientHandler.m_LocalRecorder.InterestGroup = 0;
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x0004DD35 File Offset: 0x0004BF35
	public static void LocalPlayerAssigned(Recorder r)
	{
		VoiceClientHandler.m_LocalRecorder = r;
		VoiceClientHandler.InitNetworkVoice();
	}

	// Token: 0x04000D76 RID: 3446
	private static VoiceConnection m_VoiceConnection;

	// Token: 0x04000D77 RID: 3447
	private static Recorder m_LocalRecorder;
}
