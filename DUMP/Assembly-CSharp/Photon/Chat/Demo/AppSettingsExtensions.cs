using System;
using Photon.Realtime;

namespace Photon.Chat.Demo
{
	// Token: 0x020003A6 RID: 934
	public static class AppSettingsExtensions
	{
		// Token: 0x060018FE RID: 6398 RVA: 0x0007E300 File Offset: 0x0007C500
		public static ChatAppSettings GetChatSettings(this AppSettings appSettings)
		{
			return new ChatAppSettings
			{
				AppIdChat = appSettings.AppIdChat,
				AppVersion = appSettings.AppVersion,
				FixedRegion = (appSettings.IsBestRegion ? null : appSettings.FixedRegion),
				NetworkLogging = appSettings.NetworkLogging,
				Protocol = appSettings.Protocol,
				EnableProtocolFallback = appSettings.EnableProtocolFallback,
				Server = (appSettings.IsDefaultNameServer ? null : appSettings.Server),
				Port = (ushort)appSettings.Port,
				ProxyServer = appSettings.ProxyServer
			};
		}
	}
}
