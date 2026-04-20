using System;
using System.Collections.Generic;
using Zorro.Core.Serizalization;

// Token: 0x0200003C RID: 60
public struct StatusSyncData : IBinarySerializable
{
	// Token: 0x060003C8 RID: 968 RVA: 0x00019180 File Offset: 0x00017380
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteInt(this.statusList.Count);
		for (int i = 0; i < this.statusList.Count; i++)
		{
			serializer.WriteFloat(this.statusList[i]);
		}
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x000191C8 File Offset: 0x000173C8
	public void Deserialize(BinaryDeserializer deserializer)
	{
		int num = deserializer.ReadInt();
		this.statusList = new List<float>();
		for (int i = 0; i < num; i++)
		{
			this.statusList.Add(deserializer.ReadFloat());
		}
	}

	// Token: 0x0400040C RID: 1036
	public List<float> statusList;
}
