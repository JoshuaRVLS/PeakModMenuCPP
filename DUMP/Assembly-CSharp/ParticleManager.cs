using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002C4 RID: 708
public class ParticleManager : MonoBehaviour
{
	// Token: 0x060013FD RID: 5117 RVA: 0x00065278 File Offset: 0x00063478
	private void Awake()
	{
		ParticleManager.instance = this;
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x00065280 File Offset: 0x00063480
	private void Update()
	{
		if (this.particles.Count == 0)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			this.particles[this.currentIndex].Scan();
			this.currentIndex = (this.currentIndex + 1) % this.particles.Count;
		}
	}

	// Token: 0x060013FF RID: 5119 RVA: 0x000652D7 File Offset: 0x000634D7
	public void Register(ParticleCuller particle)
	{
		if (!this.particles.Contains(particle))
		{
			this.particles.Add(particle);
		}
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x000652F3 File Offset: 0x000634F3
	public void Unregister(ParticleCuller particle)
	{
		this.particles.Remove(particle);
		if (this.currentIndex >= this.particles.Count)
		{
			this.currentIndex = 0;
		}
	}

	// Token: 0x04001236 RID: 4662
	public static ParticleManager instance;

	// Token: 0x04001237 RID: 4663
	public List<ParticleCuller> particles = new List<ParticleCuller>();

	// Token: 0x04001238 RID: 4664
	private int currentIndex;

	// Token: 0x04001239 RID: 4665
	private const int particlesPerFrame = 3;
}
