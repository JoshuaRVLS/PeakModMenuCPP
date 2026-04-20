using System;
using Zorro.Core.Serizalization;

// Token: 0x0200010F RID: 271
public abstract class DataEntryValue : IBinarySerializable
{
	// Token: 0x06000908 RID: 2312 RVA: 0x00031650 File Offset: 0x0002F850
	public void Serialize(BinarySerializer serializer)
	{
		this.SerializeValue(serializer);
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x00031659 File Offset: 0x0002F859
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.DeserializeValue(deserializer);
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00031662 File Offset: 0x0002F862
	public virtual void Init()
	{
	}

	// Token: 0x0600090B RID: 2315
	public abstract void SerializeValue(BinarySerializer serializer);

	// Token: 0x0600090C RID: 2316
	public abstract void DeserializeValue(BinaryDeserializer deserializer);

	// Token: 0x0600090D RID: 2317 RVA: 0x00031664 File Offset: 0x0002F864
	public static byte GetTypeValue(Type type)
	{
		if (type == typeof(IntItemData))
		{
			return 1;
		}
		if (type == typeof(OptionableIntItemData))
		{
			return 2;
		}
		if (type == typeof(BoolItemData))
		{
			return 3;
		}
		if (type == typeof(FloatItemData))
		{
			return 4;
		}
		if (type == typeof(OptionableBoolItemData))
		{
			return 5;
		}
		if (type == typeof(BackpackData))
		{
			return 6;
		}
		if (type == typeof(ColorItemData))
		{
			return 7;
		}
		return 0;
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x00031700 File Offset: 0x0002F900
	public static DataEntryValue GetNewFromValue(byte value)
	{
		switch (value)
		{
		case 1:
			return new IntItemData();
		case 2:
			return new OptionableIntItemData();
		case 3:
			return new BoolItemData();
		case 4:
			return new FloatItemData();
		case 5:
			return new OptionableBoolItemData();
		case 6:
			return new BackpackData();
		case 7:
			return new ColorItemData();
		default:
			throw new NotImplementedException();
		}
	}
}
