using System;
using System.Globalization;
using Zorro.Core.Serizalization;

// Token: 0x02000113 RID: 275
public class FloatItemData : DataEntryValue
{
	// Token: 0x06000920 RID: 2336 RVA: 0x00031AAA File Offset: 0x0002FCAA
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteFloat(this.Value);
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x00031AB8 File Offset: 0x0002FCB8
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		this.Value = deserializer.ReadFloat();
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x00031AC6 File Offset: 0x0002FCC6
	public override string ToString()
	{
		return this.Value.ToString(CultureInfo.InvariantCulture);
	}

	// Token: 0x0400089E RID: 2206
	public float Value;
}
