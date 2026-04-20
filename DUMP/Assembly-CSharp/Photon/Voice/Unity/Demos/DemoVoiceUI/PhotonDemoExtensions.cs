using System;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x020003A3 RID: 931
	public static class PhotonDemoExtensions
	{
		// Token: 0x060018C5 RID: 6341 RVA: 0x0007DA4A File Offset: 0x0007BC4A
		public static bool Mute(this Photon.Realtime.Player player)
		{
			return player.SetCustomProperties(new Hashtable(1)
			{
				{
					"mu",
					true
				}
			}, null, null);
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x0007DA6B File Offset: 0x0007BC6B
		public static bool Unmute(this Photon.Realtime.Player player)
		{
			return player.SetCustomProperties(new Hashtable(1)
			{
				{
					"mu",
					false
				}
			}, null, null);
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x0007DA8C File Offset: 0x0007BC8C
		public static bool IsMuted(this Photon.Realtime.Player player)
		{
			return player.HasBoolProperty("mu");
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x0007DA99 File Offset: 0x0007BC99
		public static bool SetPhotonVAD(this Photon.Realtime.Player player, bool value)
		{
			return player.SetCustomProperties(new Hashtable(1)
			{
				{
					"pv",
					value
				}
			}, null, null);
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x0007DABA File Offset: 0x0007BCBA
		public static bool SetWebRTCVAD(this Photon.Realtime.Player player, bool value)
		{
			return player.SetCustomProperties(new Hashtable(1)
			{
				{
					"wv",
					value
				}
			}, null, null);
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x0007DADB File Offset: 0x0007BCDB
		public static bool SetAEC(this Photon.Realtime.Player player, bool value)
		{
			return player.SetCustomProperties(new Hashtable(1)
			{
				{
					"ec",
					value
				}
			}, null, null);
		}

		// Token: 0x060018CB RID: 6347 RVA: 0x0007DAFC File Offset: 0x0007BCFC
		public static bool SetAGC(this Photon.Realtime.Player player, bool agcEnabled, int gain, int level)
		{
			return player.SetCustomProperties(new Hashtable(1)
			{
				{
					"gc",
					new object[]
					{
						agcEnabled,
						gain,
						level
					}
				}
			}, null, null);
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x0007DB45 File Offset: 0x0007BD45
		public static bool SetMic(this Photon.Realtime.Player player, Recorder.MicType type)
		{
			return player.SetCustomProperties(new Hashtable(1)
			{
				{
					"m",
					type
				}
			}, null, null);
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x0007DB66 File Offset: 0x0007BD66
		public static bool HasPhotonVAD(this Photon.Realtime.Player player)
		{
			return player.HasBoolProperty("pv");
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x0007DB73 File Offset: 0x0007BD73
		public static bool HasWebRTCVAD(this Photon.Realtime.Player player)
		{
			return player.HasBoolProperty("wv");
		}

		// Token: 0x060018CF RID: 6351 RVA: 0x0007DB80 File Offset: 0x0007BD80
		public static bool HasAEC(this Photon.Realtime.Player player)
		{
			return player.HasBoolProperty("ec");
		}

		// Token: 0x060018D0 RID: 6352 RVA: 0x0007DB90 File Offset: 0x0007BD90
		public static bool HasAGC(this Photon.Realtime.Player player)
		{
			object[] array = player.GetObjectProperty("gc") as object[];
			return array != null && array.Length != 0 && (bool)array[0];
		}

		// Token: 0x060018D1 RID: 6353 RVA: 0x0007DBC0 File Offset: 0x0007BDC0
		public static int GetAGCGain(this Photon.Realtime.Player player)
		{
			object[] array = player.GetObjectProperty("gc") as object[];
			if (array == null || array.Length <= 1)
			{
				return 0;
			}
			return (int)array[1];
		}

		// Token: 0x060018D2 RID: 6354 RVA: 0x0007DBF4 File Offset: 0x0007BDF4
		public static int GetAGCLevel(this Photon.Realtime.Player player)
		{
			object[] array = player.GetObjectProperty("gc") as object[];
			if (array == null || array.Length <= 2)
			{
				return 0;
			}
			return (int)array[2];
		}

		// Token: 0x060018D3 RID: 6355 RVA: 0x0007DC28 File Offset: 0x0007BE28
		public static Recorder.MicType? GetMic(this Photon.Realtime.Player player)
		{
			Recorder.MicType? result = null;
			try
			{
				result = new Recorder.MicType?((Recorder.MicType)player.GetObjectProperty("m"));
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060018D4 RID: 6356 RVA: 0x0007DC74 File Offset: 0x0007BE74
		private static bool HasBoolProperty(this Photon.Realtime.Player player, string prop)
		{
			object obj;
			return player.CustomProperties.TryGetValue(prop, out obj) && (bool)obj;
		}

		// Token: 0x060018D5 RID: 6357 RVA: 0x0007DC9C File Offset: 0x0007BE9C
		private static int? GetIntProperty(this Photon.Realtime.Player player, string prop)
		{
			object obj;
			if (player.CustomProperties.TryGetValue(prop, out obj))
			{
				return new int?((int)obj);
			}
			return null;
		}

		// Token: 0x060018D6 RID: 6358 RVA: 0x0007DCD0 File Offset: 0x0007BED0
		private static object GetObjectProperty(this Photon.Realtime.Player player, string prop)
		{
			object result;
			if (player.CustomProperties.TryGetValue(prop, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x040016C0 RID: 5824
		internal const string MUTED_KEY = "mu";

		// Token: 0x040016C1 RID: 5825
		internal const string PHOTON_VAD_KEY = "pv";

		// Token: 0x040016C2 RID: 5826
		internal const string WEBRTC_AEC_KEY = "ec";

		// Token: 0x040016C3 RID: 5827
		internal const string WEBRTC_VAD_KEY = "wv";

		// Token: 0x040016C4 RID: 5828
		internal const string WEBRTC_AGC_KEY = "gc";

		// Token: 0x040016C5 RID: 5829
		internal const string MIC_KEY = "m";
	}
}
