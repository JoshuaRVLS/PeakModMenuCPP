using System;
using UnityEngine;

// Token: 0x02000350 RID: 848
public class StepSoundCollection : MonoBehaviour
{
	// Token: 0x0600167D RID: 5757 RVA: 0x00072B0C File Offset: 0x00070D0C
	public void PlayStep(Vector3 pos, int index, AudioSource sourceOverride = null)
	{
		if (index == 0)
		{
			for (int i = 0; i < this.stepDefault.Length; i++)
			{
				if (sourceOverride)
				{
					this.stepDefault[i].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.stepDefault[i].Play(pos);
				}
			}
		}
		if (index == 1)
		{
			for (int j = 0; j < this.beachSand.Length; j++)
			{
				if (sourceOverride)
				{
					this.beachSand[j].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.beachSand[j].Play(pos);
				}
			}
		}
		if (index == 2)
		{
			for (int k = 0; k < this.beachRock.Length; k++)
			{
				if (sourceOverride)
				{
					this.beachRock[k].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.beachRock[k].Play(pos);
				}
			}
		}
		if (index == 3)
		{
			for (int l = 0; l < this.jungleGrass.Length; l++)
			{
				if (sourceOverride)
				{
					this.jungleGrass[l].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.jungleGrass[l].Play(pos);
				}
			}
		}
		if (index == 4)
		{
			for (int m = 0; m < this.jungleRock.Length; m++)
			{
				if (sourceOverride)
				{
					this.jungleRock[m].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.jungleRock[m].Play(pos);
				}
			}
		}
		if (index == 5)
		{
			for (int n = 0; n < this.iceSnow.Length; n++)
			{
				if (sourceOverride)
				{
					this.iceSnow[n].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.iceSnow[n].Play(pos);
				}
			}
		}
		if (index == 6)
		{
			for (int num = 0; num < this.iceRock.Length; num++)
			{
				if (sourceOverride)
				{
					this.iceRock[num].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.iceRock[num].Play(pos);
				}
			}
		}
		if (index == 7)
		{
			for (int num2 = 0; num2 < this.metal.Length; num2++)
			{
				if (sourceOverride)
				{
					this.metal[num2].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.metal[num2].Play(pos);
				}
			}
		}
		if (index == 8)
		{
			for (int num3 = 0; num3 < this.wood.Length; num3++)
			{
				if (sourceOverride)
				{
					this.wood[num3].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.wood[num3].Play(pos);
				}
			}
		}
		if (index == 9)
		{
			for (int num4 = 0; num4 < this.volcanoRock.Length; num4++)
			{
				if (sourceOverride)
				{
					this.volcanoRock[num4].PlayFromSource(pos, sourceOverride);
				}
				else
				{
					this.volcanoRock[num4].Play(pos);
				}
			}
		}
	}

	// Token: 0x040014CB RID: 5323
	public SFX_Instance[] stepDefault;

	// Token: 0x040014CC RID: 5324
	public SFX_Instance[] beachSand;

	// Token: 0x040014CD RID: 5325
	public SFX_Instance[] beachRock;

	// Token: 0x040014CE RID: 5326
	public SFX_Instance[] jungleGrass;

	// Token: 0x040014CF RID: 5327
	public SFX_Instance[] jungleRock;

	// Token: 0x040014D0 RID: 5328
	public SFX_Instance[] iceSnow;

	// Token: 0x040014D1 RID: 5329
	public SFX_Instance[] iceRock;

	// Token: 0x040014D2 RID: 5330
	public SFX_Instance[] metal;

	// Token: 0x040014D3 RID: 5331
	public SFX_Instance[] wood;

	// Token: 0x040014D4 RID: 5332
	public SFX_Instance[] volcanoRock;
}
