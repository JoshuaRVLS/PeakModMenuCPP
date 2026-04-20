using System;
using UnityEngine;

// Token: 0x02000156 RID: 342
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemCenter : MonoBehaviour
{
	// Token: 0x06000B48 RID: 2888 RVA: 0x0003CAA7 File Offset: 0x0003ACA7
	private void Start()
	{
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x0003CAA9 File Offset: 0x0003ACA9
	private void Update()
	{
		this.setPosition();
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x0003CAB4 File Offset: 0x0003ACB4
	public void setPosition()
	{
		if (this.psr == null)
		{
			this.psr = base.GetComponent<ParticleSystemRenderer>();
			this.material = this.psr.material;
		}
		this.pos = base.transform.position;
		this.material.SetVector(ParticleSystemCenter.Center, this.pos);
	}

	// Token: 0x04000A5C RID: 2652
	private static readonly int Center = Shader.PropertyToID("_Center");

	// Token: 0x04000A5D RID: 2653
	private Vector3 pos;

	// Token: 0x04000A5E RID: 2654
	public Material material;

	// Token: 0x04000A5F RID: 2655
	private ParticleSystemRenderer psr;
}
