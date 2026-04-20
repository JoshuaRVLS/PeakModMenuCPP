using System;
using System.Collections.Generic;
using Zorro.Core.Serizalization;

// Token: 0x02000151 RID: 337
[Serializable]
public struct RunSettingSyncData : IBinarySerializable
{
	// Token: 0x170000CE RID: 206
	// (get) Token: 0x06000B2C RID: 2860 RVA: 0x0003C48E File Offset: 0x0003A68E
	// (set) Token: 0x06000B2D RID: 2861 RVA: 0x0003C496 File Offset: 0x0003A696
	public bool isCustomRun { readonly get; private set; }

	// Token: 0x06000B2E RID: 2862 RVA: 0x0003C4A0 File Offset: 0x0003A6A0
	public void SetData(Dictionary<string, RunSettings.RunSetting> dict, bool isCustomRun)
	{
		this.settingValues = new Dictionary<string, int>();
		foreach (KeyValuePair<string, RunSettings.RunSetting> keyValuePair in dict)
		{
			this.settingValues.Add(keyValuePair.Key, keyValuePair.Value.currentValue);
		}
		this.isCustomRun = isCustomRun;
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x0003C518 File Offset: 0x0003A718
	public void Serialize(BinarySerializer serializer)
	{
		if (this.settingValues == null)
		{
			this.settingValues = new Dictionary<string, int>();
		}
		serializer.WriteBool(this.isCustomRun);
		int count = this.settingValues.Count;
		serializer.WriteUshort((ushort)count);
		ushort num = 0;
		while ((int)num < RunSettings.alphabetizedIndices.Length)
		{
			int value = 0;
			this.settingValues.TryGetValue(RunSettings.alphabetizedIndices[(int)num], out value);
			serializer.WriteUshort(num);
			serializer.WriteInt(value);
			num += 1;
		}
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x0003C590 File Offset: 0x0003A790
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.isCustomRun = deserializer.ReadBool();
		this.settingValues = new Dictionary<string, int>();
		this.settingValues.Clear();
		ushort num = deserializer.ReadUShort();
		for (int i = 0; i < (int)num; i++)
		{
			ushort num2 = deserializer.ReadUShort();
			string key = RunSettings.alphabetizedIndices[(int)num2];
			int value = deserializer.ReadInt();
			this.settingValues.Add(key, value);
		}
	}

	// Token: 0x04000A4E RID: 2638
	public Dictionary<string, int> settingValues;
}
