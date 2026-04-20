using System;
using Zorro.Core.Serizalization;

// Token: 0x02000115 RID: 277
public class OptionableBoolItemData : DataEntryValue
{
	// Token: 0x06000928 RID: 2344 RVA: 0x00031B11 File Offset: 0x0002FD11
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteBool(this.HasData);
		if (this.HasData)
		{
			serializer.WriteBool(this.Value);
		}
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x00031B33 File Offset: 0x0002FD33
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.HasData = deserializer.ReadBool();
		if (this.HasData)
		{
			this.Value = deserializer.ReadBool();
		}
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x00031B55 File Offset: 0x0002FD55
	public override string ToString()
	{
		if (!this.HasData)
		{
			return "No Data";
		}
		return this.Value.ToString();
	}

	// Token: 0x040008A0 RID: 2208
	public bool HasData;

	// Token: 0x040008A1 RID: 2209
	public bool Value;
}
