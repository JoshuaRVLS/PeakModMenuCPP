using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D5 RID: 213
public class ItemInstanceDataHandler : RetrievableSingleton<ItemInstanceDataHandler>
{
	// Token: 0x06000842 RID: 2114 RVA: 0x0002E908 File Offset: 0x0002CB08
	public IEnumerable<ItemInstanceData> GetAllItemInstances()
	{
		return this.m_instanceData.Values;
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x0002E915 File Offset: 0x0002CB15
	protected override void OnCreated()
	{
		base.OnCreated();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x0002E928 File Offset: 0x0002CB28
	public static void AddInstanceData(ItemInstanceData instanceData)
	{
		if (!RetrievableSingleton<ItemInstanceDataHandler>.Instance.m_instanceData.TryAdd(instanceData.guid, instanceData))
		{
			throw new Exception(string.Format("Adding item instance with duplicate guid: {0}", instanceData.guid));
		}
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x0002E960 File Offset: 0x0002CB60
	public static bool TryGetInstanceData(Guid guid, out ItemInstanceData o)
	{
		ItemInstanceData itemInstanceData;
		if (RetrievableSingleton<ItemInstanceDataHandler>.Instance.m_instanceData.TryGetValue(guid, out itemInstanceData))
		{
			o = itemInstanceData;
			return true;
		}
		o = null;
		return false;
	}

	// Token: 0x04000802 RID: 2050
	private Dictionary<Guid, ItemInstanceData> m_instanceData = new Dictionary<Guid, ItemInstanceData>();
}
