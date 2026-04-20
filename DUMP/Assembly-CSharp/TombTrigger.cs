using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000361 RID: 865
public class TombTrigger : MonoBehaviour
{
	// Token: 0x060016DD RID: 5853 RVA: 0x000755D8 File Offset: 0x000737D8
	private void OnTriggerEnter(Collider other)
	{
		Debug.LogError("Attempting tomb trigger");
		Character x;
		if (!CharacterRagdoll.TryGetCharacterFromCollider(other, out x))
		{
			return;
		}
		if (x == Character.localCharacter && !this.triggered)
		{
			this.TriggerTomb();
		}
		if (x == Character.localCharacter && Character.localCharacter.GetComponent<CharacterAnimations>() && Character.localCharacter.GetComponent<CharacterAnimations>().ambienceAudio)
		{
			Character.localCharacter.GetComponent<CharacterAnimations>().ambienceAudio.inTomb = true;
		}
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x0007565E File Offset: 0x0007385E
	private void TriggerTomb()
	{
		this.triggered = true;
		GUIManager.instance.SetHeroTitle(Singleton<MountainProgressHandler>.Instance.tombProgressPoint.localizedTitle, Singleton<MountainProgressHandler>.Instance.tombProgressPoint.clip);
	}

	// Token: 0x04001549 RID: 5449
	private bool triggered;
}
