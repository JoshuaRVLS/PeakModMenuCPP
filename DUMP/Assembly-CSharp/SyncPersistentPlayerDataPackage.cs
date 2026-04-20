using System;
using ExitGames.Client.Photon;
using Zorro.Core.Serizalization;
using Zorro.PhotonUtility;

// Token: 0x02000081 RID: 129
public class SyncPersistentPlayerDataPackage : CustomCommandPackage<CustomCommandType>
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000587 RID: 1415 RVA: 0x00020600 File Offset: 0x0001E800
	// (set) Token: 0x06000588 RID: 1416 RVA: 0x00020608 File Offset: 0x0001E808
	public PersistentPlayerData Data { get; set; }

	// Token: 0x06000589 RID: 1417 RVA: 0x00020611 File Offset: 0x0001E811
	protected override void SerializeData(BinarySerializer binarySerializer)
	{
		binarySerializer.WriteInt(this.ActorNumber);
		this.Data.Serialize(binarySerializer);
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x0002062B File Offset: 0x0001E82B
	public override void DeserializeData(BinaryDeserializer binaryDeserializer)
	{
		this.ActorNumber = binaryDeserializer.ReadInt();
		this.Data = IBinarySerializable.DeserializeClass<PersistentPlayerData>(binaryDeserializer);
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x00020645 File Offset: 0x0001E845
	public override CustomCommandType GetCommandType()
	{
		return CustomCommandType.SyncPersistentPlayerData;
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00020648 File Offset: 0x0001E848
	public override SendOptions GetSendOptions()
	{
		return SendOptions.SendReliable;
	}

	// Token: 0x040005BB RID: 1467
	public int ActorNumber;
}
