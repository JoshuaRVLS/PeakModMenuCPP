using System;
using UnityEngine;

// Token: 0x02000360 RID: 864
public class TombCheck : MonoBehaviour
{
	// Token: 0x060016DA RID: 5850 RVA: 0x0007551E File Offset: 0x0007371E
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x0007552C File Offset: 0x0007372C
	private void Update()
	{
		if (!this.character)
		{
			this.character = Character.localCharacter;
		}
		if (this.character && this.character.refs.animations && this.character.refs.animations.ambienceAudio)
		{
			if (this.character.refs.animations.ambienceAudio.inTomb)
			{
				this.anim.SetBool("Tomb", true);
				return;
			}
			this.anim.SetBool("Tomb", false);
		}
	}

	// Token: 0x04001547 RID: 5447
	private Character character;

	// Token: 0x04001548 RID: 5448
	private Animator anim;
}
