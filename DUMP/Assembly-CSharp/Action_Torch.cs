using System;
using UnityEngine;

// Token: 0x020000FF RID: 255
public class Action_Torch : OnItemStateChangedAction
{
	// Token: 0x060008B9 RID: 2233 RVA: 0x0002FF14 File Offset: 0x0002E114
	public override void RunAction(ItemState state)
	{
		if (state == ItemState.Held)
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				ParticleSystem.MainModule main = this.particles[i].main;
				Debug.Log("char is null? " + (base.character == null).ToString());
				main.customSimulationSpace = base.character.refs.animationPositionTransform;
			}
		}
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x0002FF80 File Offset: 0x0002E180
	private void Update()
	{
		this.torchLight.intensity = this.lightCurve.Evaluate(Time.time * this.lightSpeed) * this.lightIntensity;
	}

	// Token: 0x04000847 RID: 2119
	public ParticleSystem[] particles;

	// Token: 0x04000848 RID: 2120
	public Light torchLight;

	// Token: 0x04000849 RID: 2121
	public AnimationCurve lightCurve;

	// Token: 0x0400084A RID: 2122
	public float lightSpeed = 1f;

	// Token: 0x0400084B RID: 2123
	public float lightIntensity = 10f;
}
