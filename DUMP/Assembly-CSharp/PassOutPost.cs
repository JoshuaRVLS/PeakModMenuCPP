using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020002C5 RID: 709
public class PassOutPost : MonoBehaviour
{
	// Token: 0x06001402 RID: 5122 RVA: 0x0006532F File Offset: 0x0006352F
	private void Start()
	{
		this.vol = base.GetComponent<Volume>();
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x00065340 File Offset: 0x00063540
	private void Update()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		this.vol.enabled = (this.vol.weight > 0.0001f);
		if (Character.localCharacter.data.fullyPassedOut)
		{
			this.vol.weight = 0f;
			return;
		}
		this.vol.weight = Character.localCharacter.data.passOutValue;
	}

	// Token: 0x0400123A RID: 4666
	private Volume vol;
}
