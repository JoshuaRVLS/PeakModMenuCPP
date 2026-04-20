using System;
using UnityEngine;

// Token: 0x02000288 RID: 648
public class ItemVFX : MonoBehaviour
{
	// Token: 0x060012A9 RID: 4777 RVA: 0x0005DCCA File Offset: 0x0005BECA
	protected virtual void Start()
	{
		this.item = base.GetComponent<Item>();
		if (this.item.holderCharacter == null)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x0005DCF2 File Offset: 0x0005BEF2
	protected virtual void Update()
	{
		this.Shake();
		this.shakeSFX.volume = this.item.castProgress / 2f;
		this.shakeSFX.pitch = 1f + this.item.castProgress;
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x0005DD34 File Offset: 0x0005BF34
	protected virtual void Shake()
	{
		if (!this.item.finishedCast)
		{
			GamefeelHandler.instance.AddPerlinShake(this.item.castProgress * this.shakeAmount * Time.deltaTime * 60f, 0.2f, 15f);
		}
		if (this.item.finishedCast)
		{
			for (int i = 0; i < this.doneSFX.Length; i++)
			{
				this.doneSFX[i].Play(base.transform.position);
			}
		}
		this.castProgress = this.item.castProgress;
	}

	// Token: 0x040010C2 RID: 4290
	protected Item item;

	// Token: 0x040010C3 RID: 4291
	public bool shake;

	// Token: 0x040010C4 RID: 4292
	public float shakeAmount = 1f;

	// Token: 0x040010C5 RID: 4293
	public float castProgress;

	// Token: 0x040010C6 RID: 4294
	public AudioSource shakeSFX;

	// Token: 0x040010C7 RID: 4295
	public SFX_Instance[] doneSFX;
}
