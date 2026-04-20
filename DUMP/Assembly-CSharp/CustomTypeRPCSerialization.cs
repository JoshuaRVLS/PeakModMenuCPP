using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Unity.Collections;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x0200014A RID: 330
public static class CustomTypeRPCSerialization
{
	// Token: 0x06000AF7 RID: 2807 RVA: 0x0003B064 File Offset: 0x00039264
	public static void Initialize()
	{
		PhotonPeer.RegisterType(typeof(PhotonView), byte.MaxValue, new SerializeMethod(CustomTypeRPCSerialization.SerializePhotonView), new DeserializeMethod(CustomTypeRPCSerialization.DeserializePhotonView));
		PhotonPeer.RegisterType(typeof(ItemInstanceData), 254, new SerializeMethod(CustomTypeRPCSerialization.SerializeItemData), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeItemData));
		PhotonPeer.RegisterType(typeof(BackpackReference), 253, new SerializeMethod(CustomTypeRPCSerialization.SerializeBackpackRef), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeBackpackRef));
		PhotonPeer.RegisterType(typeof(ReconnectData), 252, new SerializeMethod(CustomTypeRPCSerialization.SerializeReconnectData), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeReconnectData));
		PhotonPeer.RegisterType(typeof(CharacterStats.SyncData), 251, new SerializeMethod(CustomTypeRPCSerialization.SerializeScoutReport), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeScoutReport));
		PhotonPeer.RegisterType(typeof(SerializableRunBasedValues), 250, new SerializeMethod(CustomTypeRPCSerialization.SerializeAchievementProgress), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeAchievementProgress));
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x0003B180 File Offset: 0x00039380
	public static object DeserializeAchievementProgress(byte[] buffer)
	{
		BinaryDeserializer binaryDeserializer = new BinaryDeserializer(buffer, Allocator.Temp);
		SerializableRunBasedValues serializableRunBasedValues = IBinarySerializable.Deserialize<SerializableRunBasedValues>(binaryDeserializer);
		binaryDeserializer.Dispose();
		return serializableRunBasedValues;
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x0003B1A8 File Offset: 0x000393A8
	public static byte[] SerializeAchievementProgress(object obj)
	{
		if (obj is SerializableRunBasedValues)
		{
			SerializableRunBasedValues serializableRunBasedValues = (SerializableRunBasedValues)obj;
			BinarySerializer binarySerializer = new BinarySerializer(500, Allocator.Temp);
			serializableRunBasedValues.Serialize(binarySerializer);
			byte[] result = binarySerializer.buffer.ToByteArray();
			binarySerializer.Dispose();
			return result;
		}
		throw new InvalidCastException("Called SerializeAchievementProgress on " + obj.GetType().Name);
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0003B208 File Offset: 0x00039408
	private static object DeserializeScoutReport(byte[] buffer)
	{
		BinaryDeserializer binaryDeserializer = new BinaryDeserializer(buffer, Allocator.Temp);
		CharacterStats.SyncData syncData = IBinarySerializable.Deserialize<CharacterStats.SyncData>(binaryDeserializer);
		binaryDeserializer.Dispose();
		return syncData;
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x0003B230 File Offset: 0x00039430
	private static byte[] SerializeScoutReport(object obj)
	{
		if (obj is CharacterStats.SyncData)
		{
			CharacterStats.SyncData syncData = (CharacterStats.SyncData)obj;
			BinarySerializer binarySerializer = new BinarySerializer(3000, Allocator.Temp);
			syncData.Serialize(binarySerializer);
			byte[] result = binarySerializer.buffer.ToByteArray();
			binarySerializer.Dispose();
			return result;
		}
		throw new InvalidCastException("Called SerializeScoutReport on " + obj.GetType().Name);
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x0003B28E File Offset: 0x0003948E
	public static object DeserializeReconnectData(byte[] buffer)
	{
		return ReconnectData.Deserialize(buffer);
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0003B29C File Offset: 0x0003949C
	public static byte[] SerializeReconnectData(object customObject)
	{
		if (customObject is ReconnectData)
		{
			return ((ReconnectData)customObject).Serialize();
		}
		throw new Exception("Could not serialize reconnect data, type: " + customObject.GetType().Name);
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0003B2DA File Offset: 0x000394DA
	private static object DeserializeBackpackRef(byte[] serializedcustomobject)
	{
		return IBinarySerializable.GetFromManagedArray<BackpackReference>(serializedcustomobject);
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0003B2E7 File Offset: 0x000394E7
	private static byte[] SerializeBackpackRef(object customobject)
	{
		return IBinarySerializable.ToManagedArray<BackpackReference>((BackpackReference)customobject);
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x0003B2F4 File Offset: 0x000394F4
	private static object DeserializeItemData(byte[] serializedcustomobject)
	{
		NativeArray<byte> buffer = serializedcustomobject.ToNativeArray(Allocator.Temp);
		BinaryDeserializer binaryDeserializer = new BinaryDeserializer(buffer);
		Guid guid = binaryDeserializer.ReadGuid();
		ItemInstanceData itemInstanceData;
		if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out itemInstanceData))
		{
			itemInstanceData = new ItemInstanceData(guid);
			ItemInstanceDataHandler.AddInstanceData(itemInstanceData);
		}
		itemInstanceData.Deserialize(binaryDeserializer);
		buffer.Dispose();
		return itemInstanceData;
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x0003B340 File Offset: 0x00039540
	private static byte[] SerializeItemData(object d)
	{
		ItemInstanceData itemInstanceData = (ItemInstanceData)d;
		BinarySerializer binarySerializer = new BinarySerializer(24, Allocator.Temp);
		binarySerializer.WriteGuid(itemInstanceData.guid);
		itemInstanceData.Serialize(binarySerializer);
		byte[] result = binarySerializer.buffer.ToByteArray();
		binarySerializer.Dispose();
		return result;
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x0003B381 File Offset: 0x00039581
	public static object DeserializePhotonView(byte[] data)
	{
		return PhotonView.Find(BitConverter.ToInt32(data));
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x0003B393 File Offset: 0x00039593
	public static byte[] SerializePhotonView(object customType)
	{
		return BitConverter.GetBytes(((PhotonView)customType).ViewID);
	}
}
