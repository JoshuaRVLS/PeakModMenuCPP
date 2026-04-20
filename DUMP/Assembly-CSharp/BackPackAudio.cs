using System;
using UnityEngine;

// Token: 0x0200020F RID: 527
public class BackPackAudio : MonoBehaviour
{
	// Token: 0x06001042 RID: 4162 RVA: 0x00050C5E File Offset: 0x0004EE5E
	private void Start()
	{
		this.item = base.GetComponent<Backpack>();
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x00050C6C File Offset: 0x0004EE6C
	private void Update()
	{
		if (this.item)
		{
			if (this.item.holderCharacter)
			{
				if (!this.hT)
				{
					for (int i = 0; i < this.holdSFX.Length; i++)
					{
						this.holdSFX[i].Play(base.transform.position);
					}
					this.hT = true;
				}
			}
			else
			{
				this.hT = false;
			}
			if (this.item.rig.useGravity)
			{
				if (!this.dT)
				{
					for (int j = 0; j < this.dropSFX.Length; j++)
					{
						this.dropSFX[j].Play(base.transform.position);
					}
				}
				this.dT = true;
				return;
			}
			this.dT = false;
		}
	}

	// Token: 0x04000E2A RID: 3626
	private Backpack item;

	// Token: 0x04000E2B RID: 3627
	public SFX_Instance[] holdSFX;

	// Token: 0x04000E2C RID: 3628
	private bool hT;

	// Token: 0x04000E2D RID: 3629
	public SFX_Instance[] dropSFX;

	// Token: 0x04000E2E RID: 3630
	private bool dT;
}
