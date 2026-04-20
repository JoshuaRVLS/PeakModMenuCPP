using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peak.ProcGen
{
	// Token: 0x020003EA RID: 1002
	[CreateAssetMenu(menuName = "Peak/BiomeSelector")]
	public class BiomeSelectionSettings : ScriptableObject
	{
		// Token: 0x0400177C RID: 6012
		public List<BiomeSelection> Settings = new List<BiomeSelection>();
	}
}
