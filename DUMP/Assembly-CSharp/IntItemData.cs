using System;
using Zorro.Core.Serizalization;

// Token: 0x02000114 RID: 276
public class IntItemData : DataEntryValue
{
	// Token: 0x06000924 RID: 2340 RVA: 0x00031AE0 File Offset: 0x0002FCE0
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteInt(this.Value);
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x00031AEE File Offset: 0x0002FCEE
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.Value = deserializer.ReadInt();
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x00031AFC File Offset: 0x0002FCFC
	public override string ToString()
	{
		return this.Value.ToString();
	}

	// Token: 0x0400089F RID: 2207
	public int Value;
}
