using System;
using UnityEngine;

// Token: 0x0200034B RID: 843
public class SpiderTrigger : MonoBehaviour
{
	// Token: 0x0600166B RID: 5739 RVA: 0x00072384 File Offset: 0x00070584
	public void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		Character character;
		if (!CharacterRagdoll.TryGetCharacterFromCollider(other, out character))
		{
			return;
		}
		if (character.IsLocal && !character.refs.afflictions.isWebbed)
		{
			this.spider.GrabCharacter(character.photonView);
		}
	}

	// Token: 0x0600166C RID: 5740 RVA: 0x000723D6 File Offset: 0x000705D6
	public void Bonk()
	{
		this.spider.Bonk();
	}

	// Token: 0x040014A4 RID: 5284
	public Spider spider;
}
