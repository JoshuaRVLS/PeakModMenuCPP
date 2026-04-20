using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000353 RID: 851
public class StormVisual : MonoBehaviour
{
	// Token: 0x06001687 RID: 5767 RVA: 0x000732B8 File Offset: 0x000714B8
	private void Start()
	{
		this.zone = base.GetComponentInParent<WindChillZone>();
		this.fogConfig = base.GetComponentInParent<FogConfig>();
		if (this.quadRend)
		{
			this.quadMat = this.quadRend.material;
		}
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x000732F0 File Offset: 0x000714F0
	private void LateUpdate()
	{
		this.observedPlayerInWindZone = (this.zone.windActive && this.zone.observedCharacterInsideBounds);
		if (this.useWindChillZoneIntensity)
		{
			Mathf.Clamp01(this.zone.windIntensity * this.windchillZoneMult);
		}
		if (this.observedPlayerInWindZone)
		{
			if (this.useWindChillZoneIntensity)
			{
				this.windIntensity = Mathf.Clamp01(this.zone.windIntensity * this.windchillZoneMult);
				this.windFactor = Mathf.Lerp(this.windFactor, this.windIntensity, Time.deltaTime / 2f);
				for (int i = 0; i < this.part.Length; i++)
				{
					if (!this.part[i].isPlaying && this.zone.windIntensity > 0.1f)
					{
						this.part[i].Play();
					}
				}
				Shader.SetGlobalFloat("GlobalWind", this.windIntensity);
			}
			else
			{
				for (int j = 0; j < this.part.Length; j++)
				{
					if (!this.part[j].isPlaying)
					{
						this.part[j].Play();
					}
				}
				this.windFactor = Mathf.Lerp(this.windFactor, Mathf.Clamp01(this.zone.hasBeenActiveFor * 0.2f), Time.deltaTime);
			}
		}
		else
		{
			if (this.useWindChillZoneIntensity)
			{
				for (int k = 0; k < this.part.Length; k++)
				{
					if (this.part[k].isPlaying && this.zone.windIntensity < 0.1f)
					{
						this.part[k].Stop();
					}
				}
			}
			else
			{
				for (int l = 0; l < this.part.Length; l++)
				{
					if (this.part[l].isPlaying)
					{
						this.part[l].Stop();
					}
				}
			}
			this.windIntensity = Mathf.Lerp(this.windIntensity, 0f, Time.deltaTime);
			Shader.SetGlobalFloat("GlobalWind", this.windIntensity);
			this.windFactor = Mathf.Lerp(this.windFactor, 0f, Time.deltaTime);
		}
		if (this.stormType == StormVisual.StormType.Rain)
		{
			DayNightManager.instance.rainstormWindFactor = this.windFactor;
		}
		else if (this.stormType == StormVisual.StormType.Snow)
		{
			DayNightManager.instance.snowstormWindFactor = this.windFactor;
		}
		else if (this.stormType == StormVisual.StormType.Wind)
		{
			DayNightManager.instance.rainstormWindFactor = this.windFactor;
			this.particleForceField.directionX = this.zone.currentWindDirection.x * this.windIntensity * this.windParticleMult;
			this.particleForceField.directionZ = this.zone.currentWindDirection.z * this.windIntensity * this.windParticleMult;
		}
		if (this.zone.observedCharacterInsideBounds)
		{
			base.transform.position = Character.observedCharacter.Center;
			if (this.zone.currentWindDirection != Vector3.zero)
			{
				base.transform.rotation = Quaternion.LookRotation(this.zone.currentWindDirection);
			}
			if (base.gameObject.CompareTag("Storm"))
			{
				Character.observedCharacter.refs.animations.stormAudio.stormVisual = this;
			}
			if (base.gameObject.CompareTag("Rain"))
			{
				Character.observedCharacter.refs.animations.stormAudio.rainVisual = this;
			}
			if (this.fogConfig && this.zone.windActive)
			{
				this.fogConfig.SetFog();
			}
			if (this.quadMat)
			{
				this.quadRend.enabled = true;
				this.quadMat.SetFloat("_Alpha", this.windFactor);
				return;
			}
		}
		else if (this.quadRend)
		{
			this.quadRend.enabled = false;
		}
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x000736D3 File Offset: 0x000718D3
	private void OnDisable()
	{
		Shader.SetGlobalFloat("GlobalWind", 0f);
	}

	// Token: 0x040014E2 RID: 5346
	public ParticleSystem[] part;

	// Token: 0x040014E3 RID: 5347
	public MeshRenderer quadRend;

	// Token: 0x040014E4 RID: 5348
	private Material quadMat;

	// Token: 0x040014E5 RID: 5349
	private FogConfig fogConfig;

	// Token: 0x040014E6 RID: 5350
	public AudioLoop stormSFX;

	// Token: 0x040014E7 RID: 5351
	[FormerlySerializedAs("playerInWindZone")]
	public bool observedPlayerInWindZone;

	// Token: 0x040014E8 RID: 5352
	private WindChillZone zone;

	// Token: 0x040014E9 RID: 5353
	public bool useWindChillZoneIntensity;

	// Token: 0x040014EA RID: 5354
	public float windchillZoneMult;

	// Token: 0x040014EB RID: 5355
	private float windIntensity;

	// Token: 0x040014EC RID: 5356
	public ParticleSystemForceField particleForceField;

	// Token: 0x040014ED RID: 5357
	public float windParticleMult = 50f;

	// Token: 0x040014EE RID: 5358
	public StormVisual.StormType stormType;

	// Token: 0x040014EF RID: 5359
	public float windFactor;

	// Token: 0x02000540 RID: 1344
	public enum StormType
	{
		// Token: 0x04001CBE RID: 7358
		Rain,
		// Token: 0x04001CBF RID: 7359
		Snow,
		// Token: 0x04001CC0 RID: 7360
		Wind
	}
}
