using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x0200017B RID: 379
public struct RopeSyncData : IBinarySerializable
{
	// Token: 0x06000C5E RID: 3166 RVA: 0x000427D4 File Offset: 0x000409D4
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteBool(this.isVisible);
		serializer.WriteBool(this.updateVisualizerManually);
		if (this.segments == null)
		{
			serializer.WriteUshort(0);
			return;
		}
		ushort num = (ushort)this.segments.Length;
		serializer.WriteUshort(num);
		for (int i = 0; i < (int)num; i++)
		{
			this.segments[i].Serialize(serializer);
		}
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x00042838 File Offset: 0x00040A38
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.isVisible = deserializer.ReadBool();
		this.updateVisualizerManually = deserializer.ReadBool();
		ushort num = deserializer.ReadUShort();
		this.segments = new RopeSyncData.SegmentData[(int)num];
		for (int i = 0; i < (int)num; i++)
		{
			this.segments[i] = IBinarySerializable.Deserialize<RopeSyncData.SegmentData>(deserializer);
		}
	}

	// Token: 0x04000B4D RID: 2893
	public RopeSyncData.SegmentData[] segments;

	// Token: 0x04000B4E RID: 2894
	public bool isVisible;

	// Token: 0x04000B4F RID: 2895
	public bool updateVisualizerManually;

	// Token: 0x020004AA RID: 1194
	public struct SegmentData : IBinarySerializable
	{
		// Token: 0x06001D30 RID: 7472 RVA: 0x00089784 File Offset: 0x00087984
		public void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat3(this.position);
			serializer.WriteQuaternion(this.rotation);
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x0008979E File Offset: 0x0008799E
		public void Deserialize(BinaryDeserializer deserializer)
		{
			this.position = deserializer.ReadFloat3();
			this.rotation = deserializer.ReadQuaternion();
		}

		// Token: 0x04001A60 RID: 6752
		public float3 position;

		// Token: 0x04001A61 RID: 6753
		public Quaternion rotation;
	}
}
