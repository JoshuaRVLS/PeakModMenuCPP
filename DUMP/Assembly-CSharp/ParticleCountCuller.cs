using System;
using UnityEngine;

// Token: 0x020002C2 RID: 706
public class ParticleCountCuller : MonoBehaviour
{
	// Token: 0x060013F5 RID: 5109 RVA: 0x000650BC File Offset: 0x000632BC
	private void Start()
	{
		this.ps = base.GetComponent<ParticleSystem>();
		this.startVal = this.ps.emission.rateOverTimeMultiplier;
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x000650F0 File Offset: 0x000632F0
	private void Update()
	{
		float t = Mathf.Clamp01((Vector3.Distance(base.transform.position, Character.observedCharacter.Head) - this.minDistance) / this.maxDistance);
		this.attenuationVal = Mathf.Lerp(1f, 0.2f, t);
		this.ps.emission.rateOverTimeMultiplier = this.attenuationVal * this.startVal;
	}

	// Token: 0x0400122F RID: 4655
	private ParticleSystem ps;

	// Token: 0x04001230 RID: 4656
	public float minDistance = 20f;

	// Token: 0x04001231 RID: 4657
	public float maxDistance = 80f;

	// Token: 0x04001232 RID: 4658
	[Range(0f, 1f)]
	public float attenuationVal;

	// Token: 0x04001233 RID: 4659
	private float startVal;
}
