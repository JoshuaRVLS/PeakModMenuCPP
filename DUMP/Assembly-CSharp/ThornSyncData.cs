using System;
using System.Collections.Generic;
using Zorro.Core.Serizalization;

// Token: 0x0200003F RID: 63
public struct ThornSyncData : IBinarySerializable
{
	// Token: 0x060003EC RID: 1004 RVA: 0x000197FC File Offset: 0x000179FC
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteInt(this.stuckThornIndices.Count);
		for (int i = 0; i < this.stuckThornIndices.Count; i++)
		{
			serializer.WriteUshort(this.stuckThornIndices[i]);
		}
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x00019844 File Offset: 0x00017A44
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.stuckThornIndices = new List<ushort>();
		int num = deserializer.ReadInt();
		ushort num2 = 0;
		while ((int)num2 < num)
		{
			this.stuckThornIndices.Add(deserializer.ReadUShort());
			num2 += 1;
		}
	}

	// Token: 0x04000422 RID: 1058
	public List<ushort> stuckThornIndices;
}
