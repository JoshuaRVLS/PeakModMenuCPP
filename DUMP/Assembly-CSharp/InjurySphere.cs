using System;
using UnityEngine;

// Token: 0x02000281 RID: 641
public class InjurySphere : MonoBehaviour
{
	// Token: 0x06001295 RID: 4757 RVA: 0x0005D4A3 File Offset: 0x0005B6A3
	private void Start()
	{
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x0005D4A8 File Offset: 0x0005B6A8
	private void Update()
	{
		if (Vector3.Distance(Character.localCharacter.data.groundPos, base.transform.position) < base.transform.localScale.x / 2f)
		{
			if (this.isHealing)
			{
				Character.localCharacter.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Injury, Time.deltaTime * 0.2f, false, false);
				return;
			}
			Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, Time.deltaTime * 0.2f, false, true, true);
		}
	}

	// Token: 0x040010A9 RID: 4265
	public bool isHealing;
}
