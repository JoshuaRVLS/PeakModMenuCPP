using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000205 RID: 517
public class AnimationJuice : MonoBehaviour
{
	// Token: 0x06001014 RID: 4116 RVA: 0x0004F710 File Offset: 0x0004D910
	public void Screenshake(float amount)
	{
		Vector3 position = base.transform.position;
		if (this.overrideGameFeelTransform)
		{
			position = this.overrideGameFeelTransform.position;
		}
		if (GamefeelHandler.instance)
		{
			GamefeelHandler.instance.AddPerlinShakeProximity(position, amount, 0.3f, 15f, 5f);
		}
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x0004F76C File Offset: 0x0004D96C
	public void PlayParticle(int index)
	{
		if (!this.particles.WithinRange(index))
		{
			Debug.LogError("PlayParticle index out of range");
			return;
		}
		ParticleSystem particleSystem = this.particles[index];
		if (particleSystem != null)
		{
			particleSystem.Play();
			return;
		}
		Debug.LogError("Particle could not be played, is null");
	}

	// Token: 0x04000DED RID: 3565
	public Transform overrideGameFeelTransform;

	// Token: 0x04000DEE RID: 3566
	public ParticleSystem[] particles;
}
