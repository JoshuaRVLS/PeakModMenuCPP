using System;
using Zorro.Core.Serizalization;

// Token: 0x02000111 RID: 273
public class BoolItemData : DataEntryValue
{
	// Token: 0x06000918 RID: 2328 RVA: 0x000319EC File Offset: 0x0002FBEC
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteBool(this.Value);
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x000319FA File Offset: 0x0002FBFA
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.Value = deserializer.ReadBool();
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00031A08 File Offset: 0x0002FC08
	public override string ToString()
	{
		return this.Value.ToString();
	}

	// Token: 0x0400089C RID: 2204
	public bool Value;
}
