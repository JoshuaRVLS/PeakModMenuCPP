using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Token: 0x02000139 RID: 313
[ExecuteInEditMode]
public class LogoCombineSequence : MonoBehaviour
{
	// Token: 0x06000A34 RID: 2612 RVA: 0x00036578 File Offset: 0x00034778
	private void Start()
	{
		if (this.volume.profile.TryGet<ChromaticAberration>(out this.chromaticAberration))
		{
			this.chromaticAberration.intensity.value = 0f;
		}
		if (this.volume.profile.TryGet<Bloom>(out this.bloom))
		{
			this.bloom.intensity.value = 0f;
		}
		if (this.volume.profile.TryGet<LensDistortion>(out this.lensDistortion))
		{
			this.lensDistortion.intensity.value = 0f;
		}
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0003660C File Offset: 0x0003480C
	private void Update()
	{
		if (this.chromaticAberration != null)
		{
			this.chromaticAberration.intensity.value = this.chromaticAmplitude;
		}
		if (this.bloom != null)
		{
			this.bloom.intensity.value = this.bloomIntensity;
		}
		if (this.lensDistortion != null)
		{
			this.lensDistortion.intensity.value = this.lensIntensity;
			this.lensDistortion.scale.value = this.lensScale;
		}
		this.material.SetFloat("_StreakAmount", this.streakAmount);
		this.material.SetFloat("_StretchAmount", this.stretchAmount);
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x000366C8 File Offset: 0x000348C8
	private void OnValidate()
	{
		if (this.bloom != null)
		{
			this.bloom.intensity.value = this.bloomIntensity;
		}
		if (this.chromaticAberration != null)
		{
			this.chromaticAberration.intensity.value = this.chromaticAmplitude;
		}
		if (this.lensDistortion != null)
		{
			this.lensDistortion.intensity.value = this.lensIntensity;
			this.lensDistortion.scale.value = this.lensScale;
		}
		this.material.SetFloat("_StreakAmount", this.streakAmount);
		this.material.SetFloat("_StretchAmount", this.stretchAmount);
	}

	// Token: 0x04000984 RID: 2436
	public float streakAmount;

	// Token: 0x04000985 RID: 2437
	public float stretchAmount;

	// Token: 0x04000986 RID: 2438
	public float chromaticAmplitude;

	// Token: 0x04000987 RID: 2439
	public float lensScale;

	// Token: 0x04000988 RID: 2440
	public float lensIntensity;

	// Token: 0x04000989 RID: 2441
	public float bloomIntensity;

	// Token: 0x0400098A RID: 2442
	public Material material;

	// Token: 0x0400098B RID: 2443
	public Volume volume;

	// Token: 0x0400098C RID: 2444
	private ChromaticAberration chromaticAberration;

	// Token: 0x0400098D RID: 2445
	private Bloom bloom;

	// Token: 0x0400098E RID: 2446
	private LensDistortion lensDistortion;
}
