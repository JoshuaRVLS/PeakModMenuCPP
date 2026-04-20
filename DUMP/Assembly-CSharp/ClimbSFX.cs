using System;
using UnityEngine;

// Token: 0x0200023A RID: 570
public class ClimbSFX : MonoBehaviour
{
	// Token: 0x0600118D RID: 4493 RVA: 0x0005860C File Offset: 0x0005680C
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x00058624 File Offset: 0x00056824
	private void Update()
	{
		if (this.character)
		{
			if (!this.character.data.isClimbing && this.sToggle)
			{
				this.sToggle = false;
				this.surfaceOff.SetActive(true);
				this.surfaceOnHeavy.SetActive(false);
				this.surfaceOn.SetActive(false);
			}
			if (this.character.data.isClimbing && !this.sToggle)
			{
				this.sToggle = true;
				this.surfaceOn.SetActive(true);
				if (this.character.data.avarageVelocity.y <= -6f)
				{
					this.surfaceOnHeavy.SetActive(true);
				}
				this.surfaceOff.SetActive(false);
			}
			if (!this.character.data.isRopeClimbing && this.rToggle)
			{
				this.rToggle = false;
				this.ropeOff.SetActive(true);
				this.ropeOn.SetActive(false);
			}
			if (this.character.data.isRopeClimbing && !this.rToggle)
			{
				this.rToggle = true;
				this.ropeOn.SetActive(true);
				this.ropeOff.SetActive(false);
			}
		}
	}

	// Token: 0x04000F5C RID: 3932
	private Character character;

	// Token: 0x04000F5D RID: 3933
	public GameObject ropeOn;

	// Token: 0x04000F5E RID: 3934
	public GameObject ropeOff;

	// Token: 0x04000F5F RID: 3935
	private bool rToggle;

	// Token: 0x04000F60 RID: 3936
	public GameObject surfaceOn;

	// Token: 0x04000F61 RID: 3937
	public GameObject surfaceOff;

	// Token: 0x04000F62 RID: 3938
	public GameObject surfaceOnHeavy;

	// Token: 0x04000F63 RID: 3939
	private bool sToggle;
}
