using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x02000112 RID: 274
public class ColorItemData : DataEntryValue
{
	// Token: 0x0600091C RID: 2332 RVA: 0x00031A1D File Offset: 0x0002FC1D
	public override void SerializeValue(BinarySerializer serializer)
	{
		serializer.WriteFloat4(new float4(this.Value.r, this.Value.g, this.Value.b, this.Value.a));
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x00031A58 File Offset: 0x0002FC58
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		float4 @float = deserializer.ReadFloat4();
		this.Value = new Color(@float.x, @float.y, @float.z, @float.w);
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00031A8F File Offset: 0x0002FC8F
	public override string ToString()
	{
		return this.Value.ToString();
	}

	// Token: 0x0400089D RID: 2205
	public Color Value;
}
