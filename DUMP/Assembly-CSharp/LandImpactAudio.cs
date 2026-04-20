using System;
using UnityEngine;

// Token: 0x0200028F RID: 655
public class LandImpactAudio : MonoBehaviour
{
	// Token: 0x060012E1 RID: 4833 RVA: 0x0005F198 File Offset: 0x0005D398
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x0005F1B0 File Offset: 0x0005D3B0
	private void Update()
	{
		this.yVel = base.transform.position.y - this.prevY;
		this.prevY = base.transform.position.y;
		if (this.yVel < -0.025f)
		{
			this.storeYVel = this.yVel;
		}
		if (this.yVel > 0.025f)
		{
			this.storeYVel = 0f;
		}
		this.impactVelocity = this.storeYVel;
		if (!this.t && this.character.data.isGrounded)
		{
			if (this.impactVelocity < -0.3f && !this.t)
			{
				if (this.impactGiant)
				{
					Object.Instantiate<GameObject>(this.impactGiant, base.transform.position, base.transform.rotation);
				}
				this.t = true;
			}
			if (this.impactVelocity < -0.2f && !this.t)
			{
				this.impactHeavy.SetActive(true);
				this.t = true;
			}
			if (this.impactVelocity < -0.1f && !this.t)
			{
				this.impactMedium.SetActive(true);
				this.t = true;
			}
			if (this.impactVelocity < -0.05f && !this.t)
			{
				this.impactSmall.SetActive(true);
				this.t = true;
			}
			this.storeYVel = 0f;
		}
		if (this.character.data.isGrounded)
		{
			this.storeYVel = 0f;
		}
		if (!this.character.data.isGrounded)
		{
			this.t = false;
			this.impactHeavy.SetActive(false);
			this.impactMedium.SetActive(false);
			this.impactSmall.SetActive(false);
		}
	}

	// Token: 0x040010F0 RID: 4336
	private Character character;

	// Token: 0x040010F1 RID: 4337
	public float impactVelocity;

	// Token: 0x040010F2 RID: 4338
	private float yVel;

	// Token: 0x040010F3 RID: 4339
	private float storeYVel;

	// Token: 0x040010F4 RID: 4340
	private float prevY;

	// Token: 0x040010F5 RID: 4341
	private bool t;

	// Token: 0x040010F6 RID: 4342
	public GameObject impactSmall;

	// Token: 0x040010F7 RID: 4343
	public GameObject impactMedium;

	// Token: 0x040010F8 RID: 4344
	public GameObject impactHeavy;

	// Token: 0x040010F9 RID: 4345
	public GameObject impactGiant;
}
