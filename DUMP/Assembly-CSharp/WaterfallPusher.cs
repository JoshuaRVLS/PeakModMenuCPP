using System;
using UnityEngine;

// Token: 0x0200037F RID: 895
public class WaterfallPusher : MonoBehaviour
{
	// Token: 0x0600176F RID: 5999 RVA: 0x00078E1C File Offset: 0x0007701C
	private void OnTriggerEnter(Collider other)
	{
		Character character;
		if (!CharacterRagdoll.TryGetCharacterFromCollider(other, out character))
		{
			return;
		}
		if (character.IsLocal && Time.time > this.cooldown + this.cooldownTimer)
		{
			if (this.fallTime > 0f)
			{
				character.Fall(this.fallTime, 0f);
			}
			this.cooldownTimer = Time.time;
			this.sfx.Play();
			GamefeelHandler.instance.AddPerlinShake(30f, 0.8f, 20f);
			Vector3 normalized = (character.Center - base.transform.position).normalized;
			normalized.y = 0f;
			Vector3 a = normalized * this.knockback;
			Vector3 b = Vector3.down * this.downwardKnockback;
			character.AddForce(a + b, 0.7f, 1.3f);
		}
	}

	// Token: 0x040015E4 RID: 5604
	public float fallTime = 0.5f;

	// Token: 0x040015E5 RID: 5605
	public float knockback = 25f;

	// Token: 0x040015E6 RID: 5606
	public float downwardKnockback = 25f;

	// Token: 0x040015E7 RID: 5607
	public float cooldown = 1f;

	// Token: 0x040015E8 RID: 5608
	private float cooldownTimer;

	// Token: 0x040015E9 RID: 5609
	public SFX_PlayOneShot sfx;
}
