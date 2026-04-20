using System;
using UnityEngine;
using WebSocketSharp;

// Token: 0x02000284 RID: 644
public class ItemAudioManager : MonoBehaviour
{
	// Token: 0x0600129F RID: 4767 RVA: 0x0005D6AF File Offset: 0x0005B8AF
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x0005D6C8 File Offset: 0x0005B8C8
	private void Update()
	{
		this.character.refs.animator.SetBool("Eat", false);
		this.character.refs.animator.SetBool("Heal", false);
		this.character.refs.animator.SetBool("Drink", false);
		this.character.refs.animator.SetBool("Antidote", false);
		this.throwCharge.volume = Mathf.Lerp(this.throwCharge.volume, 0f, Time.deltaTime * 5f);
		this.throwCharge.pitch = Mathf.Lerp(this.throwCharge.pitch, 1f, Time.deltaTime * 5f);
		if (!string.IsNullOrEmpty(this.prevUse) && !this.prevUse.IsNullOrEmpty())
		{
			this.character.refs.animator.SetBool(this.prevUse, false);
		}
		if (!this.character.data.currentItem && !string.IsNullOrEmpty(this.prevUse))
		{
			this.character.refs.animator.SetBool(this.prevUse, false);
		}
		if (this.character.refs.animator.GetBool("Consumed Item"))
		{
			this.finishTimer -= Time.deltaTime;
		}
		else
		{
			this.finishTimer = 0.25f;
		}
		if (this.finishTimer <= 0f)
		{
			this.character.refs.animator.SetBool("Consumed Item", false);
		}
		if (this.character.data.currentItem)
		{
			if (this.character.refs.items.throwChargeLevel > 0f)
			{
				this.throwCharge.volume = Mathf.Lerp(this.throwCharge.volume, 0.3f, Time.deltaTime * 10f);
				this.throwCharge.pitch = Mathf.Lerp(this.throwCharge.pitch, 2f + this.character.refs.items.throwChargeLevel * 3f, Time.deltaTime * 10f);
			}
			if (this.prevItem != this.character.data.currentItem)
			{
				for (int i = 0; i < this.switchGeneric.Length; i++)
				{
					this.switchGeneric[i].Play(base.transform.position);
				}
			}
			ItemUseFeedback itemUseFeedback;
			if (this.character.data.currentItem.TryGetComponent<ItemUseFeedback>(out itemUseFeedback))
			{
				if (this.prevItem != this.character.data.currentItem)
				{
					SFX_Instance[] equip = itemUseFeedback.equip;
					for (int j = 0; j < equip.Length; j++)
					{
						equip[j].Play(base.transform.position);
					}
				}
				string useAnimation = itemUseFeedback.useAnimation;
				if (!string.IsNullOrEmpty(useAnimation))
				{
					if (this.character.data.currentItem.isUsingPrimary && this.character.data.currentItem.castProgress < 1f)
					{
						this.character.refs.animator.SetBool(useAnimation, true);
					}
					else
					{
						this.character.refs.animator.SetBool(useAnimation, false);
					}
				}
				this.prevUse = useAnimation;
			}
		}
		if (this.prevItem && !this.character.data.currentItem)
		{
			for (int k = 0; k < this.switchGeneric.Length; k++)
			{
				this.switchGeneric[k].Play(base.transform.position);
			}
		}
		this.prevItem = this.character.data.currentItem;
	}

	// Token: 0x040010B1 RID: 4273
	private string prevUse;

	// Token: 0x040010B2 RID: 4274
	private Item prevItem;

	// Token: 0x040010B3 RID: 4275
	public AudioSource throwCharge;

	// Token: 0x040010B4 RID: 4276
	private Character character;

	// Token: 0x040010B5 RID: 4277
	[HideInInspector]
	public float finishTimer;

	// Token: 0x040010B6 RID: 4278
	private float increase;

	// Token: 0x040010B7 RID: 4279
	public SFX_Instance[] switchGeneric;
}
