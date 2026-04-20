using System;
using System.Collections.Generic;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000186 RID: 390
public struct SerializableRunBasedValues : IBinarySerializable
{
	// Token: 0x06000CE3 RID: 3299 RVA: 0x00044BAC File Offset: 0x00042DAC
	public static SerializableRunBasedValues ConstructNew()
	{
		SerializableRunBasedValues result = default(SerializableRunBasedValues);
		result.Allocate();
		result.PrimeExistingAchievements();
		return result;
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x00044BD0 File Offset: 0x00042DD0
	private void Allocate()
	{
		this.runBasedInts = new Dictionary<RUNBASEDVALUETYPE, int>();
		this.runBasedFloats = new Dictionary<RUNBASEDVALUETYPE, float>();
		this.runBasedFruitsEaten = new List<ushort>();
		this.shroomBerriesEaten = new List<ushort>();
		this.nonToxicMushroomsEaten = new List<ushort>();
		this.gourmandRequirementsEaten = new List<ushort>();
		this.achievementsEarnedThisRun = new List<ACHIEVEMENTTYPE>();
		this.completedAscentsThisRun = new List<int>();
		this.steamAchievementsPreviouslyUnlocked = new List<ACHIEVEMENTTYPE>();
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x00044C40 File Offset: 0x00042E40
	internal void PrimeExistingAchievements()
	{
		this.steamAchievementsPreviouslyUnlocked.Clear();
		foreach (object obj in Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			ACHIEVEMENTTYPE achievementtype = (ACHIEVEMENTTYPE)obj;
			if (Singleton<AchievementManager>.Instance.IsAchievementUnlocked(achievementtype))
			{
				this.steamAchievementsPreviouslyUnlocked.Add(achievementtype);
			}
		}
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00044CC0 File Offset: 0x00042EC0
	public void Serialize(BinarySerializer serializer)
	{
		this.SerializeRunBasedValues(serializer);
		this.SerializeUshortList(this.runBasedFruitsEaten, serializer);
		this.SerializeUshortList(this.shroomBerriesEaten, serializer);
		this.SerializeUshortList(this.gourmandRequirementsEaten, serializer);
		this.SerializeAchievementList(this.achievementsEarnedThisRun, serializer);
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x00044D00 File Offset: 0x00042F00
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.Allocate();
		this.DeserializeRunBasedValues(deserializer);
		this.runBasedFruitsEaten = this.DeserializeUshortList(deserializer);
		this.shroomBerriesEaten = this.DeserializeUshortList(deserializer);
		this.gourmandRequirementsEaten = this.DeserializeUshortList(deserializer);
		this.achievementsEarnedThisRun = this.DeserializeAchievementList(deserializer);
		this.PrimeExistingAchievements();
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x00044D54 File Offset: 0x00042F54
	public void SerializeRunBasedValues(BinarySerializer serializer)
	{
		int count = this.runBasedInts.Count;
		serializer.WriteInt(count);
		foreach (KeyValuePair<RUNBASEDVALUETYPE, int> keyValuePair in this.runBasedInts)
		{
			serializer.WriteInt((int)keyValuePair.Key);
			serializer.WriteInt(keyValuePair.Value);
		}
		int count2 = this.runBasedFloats.Count;
		serializer.WriteInt(count2);
		foreach (KeyValuePair<RUNBASEDVALUETYPE, float> keyValuePair2 in this.runBasedFloats)
		{
			serializer.WriteInt((int)keyValuePair2.Key);
			serializer.WriteFloat(keyValuePair2.Value);
		}
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x00044E38 File Offset: 0x00043038
	public void DeserializeRunBasedValues(BinaryDeserializer deserializer)
	{
		this.runBasedInts.Clear();
		int num = deserializer.ReadInt();
		for (int i = 0; i < num; i++)
		{
			RUNBASEDVALUETYPE key = (RUNBASEDVALUETYPE)deserializer.ReadInt();
			int value = deserializer.ReadInt();
			this.runBasedInts.TryAdd(key, value);
		}
		this.runBasedFloats.Clear();
		int num2 = deserializer.ReadInt();
		for (int j = 0; j < num2; j++)
		{
			RUNBASEDVALUETYPE key2 = (RUNBASEDVALUETYPE)deserializer.ReadInt();
			float value2 = deserializer.ReadFloat();
			this.runBasedFloats.TryAdd(key2, value2);
		}
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x00044EC4 File Offset: 0x000430C4
	public void SerializeUshortList(List<ushort> list, BinarySerializer serializer)
	{
		if (list == null)
		{
			list = new List<ushort>();
		}
		serializer.WriteInt(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			serializer.WriteUshort(list[i]);
		}
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x00044F08 File Offset: 0x00043108
	public List<ushort> DeserializeUshortList(BinaryDeserializer deserializer)
	{
		List<ushort> list = new List<ushort>();
		int num = deserializer.ReadInt();
		for (int i = 0; i < num; i++)
		{
			list.Add(deserializer.ReadUShort());
		}
		return list;
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x00044F3C File Offset: 0x0004313C
	public void SerializeAchievementList(List<ACHIEVEMENTTYPE> list, BinarySerializer serializer)
	{
		if (list == null)
		{
			list = new List<ACHIEVEMENTTYPE>();
		}
		serializer.WriteInt(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			serializer.WriteInt((int)list[i]);
		}
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x00044F80 File Offset: 0x00043180
	public List<ACHIEVEMENTTYPE> DeserializeAchievementList(BinaryDeserializer deserializer)
	{
		List<ACHIEVEMENTTYPE> list = new List<ACHIEVEMENTTYPE>();
		int num = deserializer.ReadInt();
		for (int i = 0; i < num; i++)
		{
			list.Add((ACHIEVEMENTTYPE)deserializer.ReadInt());
		}
		return list;
	}

	// Token: 0x04000BA1 RID: 2977
	internal Dictionary<RUNBASEDVALUETYPE, int> runBasedInts;

	// Token: 0x04000BA2 RID: 2978
	internal Dictionary<RUNBASEDVALUETYPE, float> runBasedFloats;

	// Token: 0x04000BA3 RID: 2979
	internal List<ushort> runBasedFruitsEaten;

	// Token: 0x04000BA4 RID: 2980
	internal List<ushort> shroomBerriesEaten;

	// Token: 0x04000BA5 RID: 2981
	internal List<ushort> nonToxicMushroomsEaten;

	// Token: 0x04000BA6 RID: 2982
	internal List<ushort> gourmandRequirementsEaten;

	// Token: 0x04000BA7 RID: 2983
	internal List<ACHIEVEMENTTYPE> achievementsEarnedThisRun;

	// Token: 0x04000BA8 RID: 2984
	internal List<int> completedAscentsThisRun;

	// Token: 0x04000BA9 RID: 2985
	internal List<ACHIEVEMENTTYPE> steamAchievementsPreviouslyUnlocked;
}
