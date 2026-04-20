using System;
using ExitGames.Client.Photon;
using Zorro.Core.Serizalization;
using Zorro.PhotonUtility;

// Token: 0x0200007F RID: 127
public class SyncLavaRisingPackage : CustomCommandPackage<CustomCommandType>
{
	// Token: 0x0600057B RID: 1403 RVA: 0x000204AD File Offset: 0x0001E6AD
	public SyncLavaRisingPackage()
	{
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x000204B5 File Offset: 0x0001E6B5
	public SyncLavaRisingPackage(bool started, bool ended, float time, float timeWaited)
	{
		this.Started = started;
		this.Ended = ended;
		this.Time = time;
		this.TimeWaited = timeWaited;
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x000204DA File Offset: 0x0001E6DA
	protected override void SerializeData(BinarySerializer binarySerializer)
	{
		binarySerializer.WriteBool(this.Started);
		binarySerializer.WriteBool(this.Ended);
		binarySerializer.WriteFloat(this.Time);
		binarySerializer.WriteFloat(this.TimeWaited);
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x0002050C File Offset: 0x0001E70C
	public override void DeserializeData(BinaryDeserializer binaryDeserializer)
	{
		this.Started = binaryDeserializer.ReadBool();
		this.Ended = binaryDeserializer.ReadBool();
		this.Time = binaryDeserializer.ReadFloat();
		this.TimeWaited = binaryDeserializer.ReadFloat();
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x0002053E File Offset: 0x0001E73E
	public override CustomCommandType GetCommandType()
	{
		return CustomCommandType.SyncLavaRising;
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x00020541 File Offset: 0x0001E741
	public override SendOptions GetSendOptions()
	{
		return SendOptions.SendReliable;
	}

	// Token: 0x040005B5 RID: 1461
	public bool Started;

	// Token: 0x040005B6 RID: 1462
	public bool Ended;

	// Token: 0x040005B7 RID: 1463
	public float Time;

	// Token: 0x040005B8 RID: 1464
	public float TimeWaited;
}
