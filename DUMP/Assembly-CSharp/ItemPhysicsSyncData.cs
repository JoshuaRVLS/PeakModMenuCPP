using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x0200015B RID: 347
public struct ItemPhysicsSyncData : IBinarySerializable
{
	// Token: 0x06000B79 RID: 2937 RVA: 0x0003D78D File Offset: 0x0003B98D
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteFloat3(this.position);
		serializer.WriteQuaternion(this.rotation);
		serializer.WriteHalf3((half3)this.linearVelocity);
		serializer.WriteHalf3((half3)this.angularVelocity);
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x0003D7C9 File Offset: 0x0003B9C9
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.position = deserializer.ReadFloat3();
		this.rotation = deserializer.ReadQuaternion();
		this.linearVelocity = deserializer.ReadHalf3();
		this.angularVelocity = deserializer.ReadHalf3();
	}

	// Token: 0x04000A86 RID: 2694
	public float3 position;

	// Token: 0x04000A87 RID: 2695
	public Quaternion rotation;

	// Token: 0x04000A88 RID: 2696
	public float3 linearVelocity;

	// Token: 0x04000A89 RID: 2697
	public float3 angularVelocity;
}
