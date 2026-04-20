using System;
using ExitGames.Client.Photon;
using Zorro.Core.Serizalization;
using Zorro.PhotonUtility;

// Token: 0x02000080 RID: 128
public class SyncMapHandlerDebugCommandPackage : CustomCommandPackage<CustomCommandType>
{
	// Token: 0x06000581 RID: 1409 RVA: 0x00020548 File Offset: 0x0001E748
	public SyncMapHandlerDebugCommandPackage()
	{
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x00020550 File Offset: 0x0001E750
	public SyncMapHandlerDebugCommandPackage(Segment segment, int[] playersToTeleport)
	{
		this.Segment = segment;
		this.PlayerToTeleport = playersToTeleport;
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x00020568 File Offset: 0x0001E768
	protected override void SerializeData(BinarySerializer binarySerializer)
	{
		binarySerializer.WriteByte((byte)this.Segment);
		binarySerializer.WriteByte((byte)this.PlayerToTeleport.Length);
		foreach (int value in this.PlayerToTeleport)
		{
			binarySerializer.WriteInt(value);
		}
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x000205B0 File Offset: 0x0001E7B0
	public override void DeserializeData(BinaryDeserializer binaryDeserializer)
	{
		this.Segment = (Segment)binaryDeserializer.ReadByte();
		byte b = binaryDeserializer.ReadByte();
		this.PlayerToTeleport = new int[(int)b];
		for (int i = 0; i < (int)b; i++)
		{
			this.PlayerToTeleport[i] = binaryDeserializer.ReadInt();
		}
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x000205F6 File Offset: 0x0001E7F6
	public override CustomCommandType GetCommandType()
	{
		return CustomCommandType.SyncMapHandlerDebugCommand;
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x000205F9 File Offset: 0x0001E7F9
	public override SendOptions GetSendOptions()
	{
		return SendOptions.SendReliable;
	}

	// Token: 0x040005B9 RID: 1465
	public int[] PlayerToTeleport;

	// Token: 0x040005BA RID: 1466
	public Segment Segment;
}
