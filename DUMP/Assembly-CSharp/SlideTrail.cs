using System;
using UnityEngine;

// Token: 0x02000343 RID: 835
public class SlideTrail : MonoBehaviour
{
	// Token: 0x0600163E RID: 5694 RVA: 0x00070F20 File Offset: 0x0006F120
	private void Start()
	{
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		this.l = componentsInChildren[0];
		this.r = componentsInChildren[1];
		this.character = base.GetComponentInParent<Character>();
	}

	// Token: 0x0600163F RID: 5695 RVA: 0x00070F54 File Offset: 0x0006F154
	private void Update()
	{
		this.l.transform.position = this.character.GetBodypartRig(BodypartType.Hand_L).position;
		this.r.transform.position = this.character.GetBodypartRig(BodypartType.Hand_R).position;
		if (this.character.IsSliding() && this.character.data.outOfStaminaFor > 2f)
		{
			this.HandlePart(this.l, this.l.transform.position);
			this.HandlePart(this.r, this.r.transform.position);
			return;
		}
		this.SetPartOn(this.l, false);
		this.SetPartOn(this.r, false);
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x0007101C File Offset: 0x0006F21C
	private void HandlePart(ParticleSystem part, Vector3 position)
	{
		if (HelperFunctions.LineCheck(position, position - this.character.data.groundNormal * 0.3f, HelperFunctions.LayerType.Terrain, 0f, QueryTriggerInteraction.Ignore).transform)
		{
			this.SetPartOn(part, true);
			return;
		}
		this.SetPartOn(part, false);
	}

	// Token: 0x06001641 RID: 5697 RVA: 0x00071076 File Offset: 0x0006F276
	private void SetPartOn(ParticleSystem part, bool on)
	{
		if (on && !part.isPlaying)
		{
			part.Play(true);
			return;
		}
		if (!on && part.isPlaying)
		{
			part.Stop(true);
		}
	}

	// Token: 0x04001454 RID: 5204
	private ParticleSystem l;

	// Token: 0x04001455 RID: 5205
	private ParticleSystem r;

	// Token: 0x04001456 RID: 5206
	private Character character;
}
