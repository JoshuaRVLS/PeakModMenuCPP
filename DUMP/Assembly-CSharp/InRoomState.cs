using System;
using Zorro.PhotonUtility;

// Token: 0x02000088 RID: 136
public class InRoomState : ConnectionState
{
	// Token: 0x060005B8 RID: 1464 RVA: 0x00020D4C File Offset: 0x0001EF4C
	public override void Enter()
	{
		base.Enter();
		this.verifiedLobby = null;
		this.hasLoadedCustomization = false;
		CommandListener commandListener = CustomCommands<CustomCommandType>.SpawnCommandListener<CommandListener>();
		commandListener.RegisterPackage<SyncPersistentPlayerDataPackage>(new SyncPersistentPlayerDataPackage());
		commandListener.RegisterPackage<SyncMapHandlerDebugCommandPackage>(new SyncMapHandlerDebugCommandPackage());
		commandListener.RegisterPackage<SyncLavaRisingPackage>(new SyncLavaRisingPackage());
		GameHandler.ClearAllStatuses();
		GameHandler.GetService<RichPresenceService>().Dirty();
	}

	// Token: 0x040005C4 RID: 1476
	public bool hasLoadedCustomization;

	// Token: 0x040005C5 RID: 1477
	public string verifiedLobby;
}
