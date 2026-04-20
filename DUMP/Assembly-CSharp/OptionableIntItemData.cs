using System;
using Zorro.Core.Serizalization;

// Token: 0x02000116 RID: 278
public class OptionableIntItemData : DataEntryValue
{
	// Token: 0x0600092C RID: 2348 RVA: 0x00031B78 File Offset: 0x0002FD78
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteBool(this.HasData);
		if (this.HasData)
		{
			serializer.WriteInt(this.Value);
		}
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x00031B9A File Offset: 0x0002FD9A
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.HasData = deserializer.ReadBool();
		if (this.HasData)
		{
			this.Value = deserializer.ReadInt();
		}
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00031BBC File Offset: 0x0002FDBC
	public override string ToString()
	{
		if (!this.HasData)
		{
			return "No Data";
		}
		return this.Value.ToString();
	}

	// Token: 0x040008A2 RID: 2210
	public bool HasData;

	// Token: 0x040008A3 RID: 2211
	public int Value;
}
