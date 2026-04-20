using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Editor;

// Token: 0x0200013A RID: 314
[CreateAssetMenu(menuName = "Peak/MapBaker")]
public class MapBaker : SingletonAsset<MapBaker>
{
	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000A38 RID: 2616 RVA: 0x0003678B File Offset: 0x0003498B
	[Obsolete("Renamed to ScenePaths.")]
	public string[] AllLevels
	{
		get
		{
			return this.ScenePaths;
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000A39 RID: 2617 RVA: 0x00036793 File Offset: 0x00034993
	private static bool IsMidBuild
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x00036796 File Offset: 0x00034996
	public string GetBiomeID(int levelIndex)
	{
		if (this.BiomeIDs.Count == 0)
		{
			return "";
		}
		levelIndex %= this.BiomeIDs.Count;
		return this.BiomeIDs[levelIndex];
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x000367C6 File Offset: 0x000349C6
	public string GetLevel(int levelIndex)
	{
		if (this.ScenePaths.Length == 0)
		{
			Debug.LogError("No levels found, using WilIsland...");
			return "";
		}
		levelIndex %= this.ScenePaths.Length;
		string result = PathUtil.WithoutExtensions(PathUtil.GetFileName(this.ScenePaths[levelIndex]));
		bool isEditor = Application.isEditor;
		return result;
	}

	// Token: 0x0400098F RID: 2447
	public string[] ScenePaths;

	// Token: 0x04000990 RID: 2448
	public List<string> BiomeIDs;

	// Token: 0x04000991 RID: 2449
	public List<MapBaker.BiomeResult> selectedBiomes = new List<MapBaker.BiomeResult>();

	// Token: 0x02000483 RID: 1155
	[Serializable]
	public class BiomeResult
	{
		// Token: 0x06001CAE RID: 7342 RVA: 0x00088854 File Offset: 0x00086A54
		public BiomeResult(List<Biome.BiomeType> biomeTypes)
		{
			this.biomeTypes = new List<Biome.BiomeType>(biomeTypes);
		}

		// Token: 0x06001CAF RID: 7343 RVA: 0x00088868 File Offset: 0x00086A68
		public bool IsIdenticalTo(MapBaker.BiomeResult other)
		{
			int num = 0;
			while (num < this.biomeTypes.Count && num < other.biomeTypes.Count)
			{
				if (other.biomeTypes[num] != this.biomeTypes[num])
				{
					return false;
				}
				num++;
			}
			return true;
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x000888B6 File Offset: 0x00086AB6
		public bool IsIdenticalTo(MapBaker.BiomeResult other, int biomeIndex)
		{
			return biomeIndex < other.biomeTypes.Count && biomeIndex < this.biomeTypes.Count && other.biomeTypes[biomeIndex] == this.biomeTypes[biomeIndex];
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x000888F4 File Offset: 0x00086AF4
		public override string ToString()
		{
			string text = "";
			foreach (Biome.BiomeType biomeType in this.biomeTypes)
			{
				text += biomeType.ToString().ToUpper()[0].ToString();
			}
			return text;
		}

		// Token: 0x040019DB RID: 6619
		public List<Biome.BiomeType> biomeTypes;
	}
}
