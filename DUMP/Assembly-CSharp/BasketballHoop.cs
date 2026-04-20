using System;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class BasketballHoop : MonoBehaviour
{
	// Token: 0x0600105B RID: 4187 RVA: 0x00051418 File Offset: 0x0004F618
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null)
		{
			Item component = other.attachedRigidbody.GetComponent<Item>();
			if (component != null && component.transform.position.y > base.transform.position.y)
			{
				this.ballRb = other.attachedRigidbody;
			}
		}
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x00051478 File Offset: 0x0004F678
	private void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody != null && other.attachedRigidbody == this.ballRb && other.attachedRigidbody.linearVelocity.y < 0f && this.ballRb.transform.position.y < base.transform.position.y && other.attachedRigidbody.GetComponent<Item>() != null && Time.time > this.lastScoredTime + 2f)
		{
			this.ballRb = null;
			this.confetti.Play();
			this.success.Play();
			this.anim.SetTrigger("Score");
			this.lastScoredTime = Time.time;
		}
	}

	// Token: 0x04000E4A RID: 3658
	public Animator anim;

	// Token: 0x04000E4B RID: 3659
	public ParticleSystem confetti;

	// Token: 0x04000E4C RID: 3660
	public SFX_PlayOneShot success;

	// Token: 0x04000E4D RID: 3661
	private float lastScoredTime;

	// Token: 0x04000E4E RID: 3662
	private Rigidbody ballRb;
}
