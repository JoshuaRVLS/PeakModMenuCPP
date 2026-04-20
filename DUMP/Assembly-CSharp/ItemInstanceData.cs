using System;
using System.Collections.Generic;
using System.Linq;
using Zorro.Core.Serizalization;

// Token: 0x020000D2 RID: 210
public class ItemInstanceData : IBinarySerializable
{
	// Token: 0x06000838 RID: 2104 RVA: 0x0002E5B5 File Offset: 0x0002C7B5
	public ItemInstanceData()
	{
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0002E5C8 File Offset: 0x0002C7C8
	public ItemInstanceData(Guid guid)
	{
		this.guid = guid;
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x0002E5E4 File Offset: 0x0002C7E4
	public void Serialize(BinarySerializer serializer)
	{
		List<KeyValuePair<DataEntryKey, DataEntryValue>> list = this.data.ToList<KeyValuePair<DataEntryKey, DataEntryValue>>();
		byte value = (byte)list.Count;
		serializer.WriteByte(value);
		foreach (KeyValuePair<DataEntryKey, DataEntryValue> keyValuePair in list)
		{
			DataEntryKey key = keyValuePair.Key;
			DataEntryValue value2 = keyValuePair.Value;
			serializer.WriteByte((byte)key);
			serializer.WriteByte(DataEntryValue.GetTypeValue(value2.GetType()));
			value2.Serialize(serializer);
		}
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x0002E678 File Offset: 0x0002C878
	public void Deserialize(BinaryDeserializer deserializer)
	{
		byte b = deserializer.ReadByte();
		this.data = new Dictionary<DataEntryKey, DataEntryValue>((int)b);
		for (int i = 0; i < (int)b; i++)
		{
			DataEntryKey key = (DataEntryKey)deserializer.ReadByte();
			DataEntryValue newFromValue = DataEntryValue.GetNewFromValue(deserializer.ReadByte());
			newFromValue.Init();
			newFromValue.Deserialize(deserializer);
			this.data.Add(key, newFromValue);
		}
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x0002E6D1 File Offset: 0x0002C8D1
	public bool HasData(DataEntryKey key)
	{
		return this.data.ContainsKey(key);
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0002E6E0 File Offset: 0x0002C8E0
	public bool TryGetDataEntry<T>(DataEntryKey key, out T value) where T : DataEntryValue
	{
		DataEntryValue dataEntryValue;
		bool flag = this.data.TryGetValue(key, out dataEntryValue);
		if (flag)
		{
			value = (T)((object)dataEntryValue);
			return flag;
		}
		value = default(T);
		return flag;
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x0002E714 File Offset: 0x0002C914
	public T RegisterNewEntry<T>(DataEntryKey key) where T : DataEntryValue, new()
	{
		T t = Activator.CreateInstance<T>();
		t.Init();
		this.data.Add(key, t);
		return t;
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x0002E745 File Offset: 0x0002C945
	public T RegisterEntry<T>(DataEntryKey key, T value) where T : DataEntryValue, new()
	{
		value.Init();
		this.data.Add(key, value);
		return value;
	}

	// Token: 0x040007EF RID: 2031
	public Guid guid;

	// Token: 0x040007F0 RID: 2032
	public Dictionary<DataEntryKey, DataEntryValue> data = new Dictionary<DataEntryKey, DataEntryValue>();
}
