using System;
using UnityEngine;
using UnityEngine.Audio;
using Zorro.Core;

// Token: 0x020001B6 RID: 438
[CreateAssetMenu(fileName = "StaticReferences", menuName = "Peak/StaticReferences")]
public class StaticReferences : SingletonAsset<StaticReferences>
{
	// Token: 0x04000BE4 RID: 3044
	public AudioMixerGroup masterMixerGroup;
}
