using System;
using System.Collections.Generic;
using UnityEngine.LowLevel;

namespace Peak.Dev
{
	// Token: 0x020003EF RID: 1007
	public static class PlayerLoopHelper
	{
		// Token: 0x06001A96 RID: 6806 RVA: 0x00083278 File Offset: 0x00081478
		public static PlayerLoopSystem InsertSystemAfter<T>(this PlayerLoopSystem self, in PlayerLoopSystem insertedSystem) where T : struct
		{
			PlayerLoopSystem result = new PlayerLoopSystem
			{
				loopConditionFunction = self.loopConditionFunction,
				type = self.type,
				updateDelegate = self.updateDelegate,
				updateFunction = self.updateFunction
			};
			List<PlayerLoopSystem> list = new List<PlayerLoopSystem>();
			if (self.subSystemList != null)
			{
				for (int i = 0; i < self.subSystemList.Length; i++)
				{
					list.Add(self.subSystemList[i]);
					if (self.subSystemList[i].type == typeof(T))
					{
						list.Add(insertedSystem);
					}
				}
			}
			result.subSystemList = list.ToArray();
			return result;
		}
	}
}
