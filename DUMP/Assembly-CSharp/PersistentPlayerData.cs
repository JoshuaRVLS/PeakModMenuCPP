using System;
using Zorro.Core.Serizalization;

// Token: 0x0200008B RID: 139
public class PersistentPlayerData : IBinarySerializable
{
	// Token: 0x060005BC RID: 1468 RVA: 0x00020DB9 File Offset: 0x0001EFB9
	public void Serialize(BinarySerializer serializer)
	{
		this.customizationData.Serialize(serializer);
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x00020DC7 File Offset: 0x0001EFC7
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.customizationData = IBinarySerializable.DeserializeClass<CharacterCustomizationData>(deserializer);
	}

	// Token: 0x040005C8 RID: 1480
	public CharacterCustomizationData customizationData = new CharacterCustomizationData();
}
