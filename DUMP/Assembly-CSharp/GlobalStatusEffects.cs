using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class GlobalStatusEffects : MonoBehaviour
{
	// Token: 0x06001276 RID: 4726 RVA: 0x0005C897 File Offset: 0x0005AA97
	private void Start()
	{
	}

	// Token: 0x06001277 RID: 4727 RVA: 0x0005C89C File Offset: 0x0005AA9C
	private void Update()
	{
		foreach (GlobalStatusEffects.Effect effect in this.effects)
		{
			foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
			{
				character.refs.afflictions.AddStatus(effect.type, effect.amount / effect.inTime * Time.deltaTime, false, true, true);
			}
		}
	}

	// Token: 0x04001088 RID: 4232
	public List<GlobalStatusEffects.Effect> effects = new List<GlobalStatusEffects.Effect>();

	// Token: 0x02000506 RID: 1286
	[Serializable]
	public class Effect
	{
		// Token: 0x04001BFE RID: 7166
		public CharacterAfflictions.STATUSTYPE type;

		// Token: 0x04001BFF RID: 7167
		public float amount;

		// Token: 0x04001C00 RID: 7168
		public float inTime = 60f;
	}
}
