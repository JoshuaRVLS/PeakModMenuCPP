using System;
using UnityEngine;

// Token: 0x0200026B RID: 619
[ExecuteInEditMode]
public class FogConfig : MonoBehaviour
{
	// Token: 0x06001246 RID: 4678 RVA: 0x0005B731 File Offset: 0x00059931
	private void Start()
	{
		Shader.SetGlobalFloat("_WeatherBlend", 0f);
	}

	// Token: 0x06001247 RID: 4679 RVA: 0x0005B744 File Offset: 0x00059944
	private void Update()
	{
		this.sinceSet += Time.deltaTime;
		if (FogConfig.currentFog == this && this.sinceSet > 0.1f && this.sinceSet < 10f)
		{
			float num = Shader.GetGlobalFloat("_WeatherBlend");
			if (num > 0f)
			{
				num = Mathf.MoveTowards(num, 0f, Time.deltaTime * 0.3f);
				Shader.SetGlobalFloat("_WeatherBlend", num);
			}
		}
	}

	// Token: 0x06001248 RID: 4680 RVA: 0x0005B7C0 File Offset: 0x000599C0
	public void SetFog()
	{
		FogConfig.currentFog = this;
		this.sinceSet = 0f;
		Shader.SetGlobalTexture("_WindTexture", this.windTexture);
		float num = Shader.GetGlobalFloat("_WeatherBlend");
		if (this.useWindChillZoneIntensity)
		{
			num = this.zone.windIntensity;
		}
		else
		{
			num = Mathf.MoveTowards(num, this.maxVal, Time.deltaTime * 0.3f);
		}
		Shader.SetGlobalFloat("_WeatherBlend", num);
		Shader.SetGlobalColor("WindTint", this.windTint);
		Shader.SetGlobalFloat("WindSkyBrightnessValue", this.windSkyBrightnessValue);
		Shader.SetGlobalFloat("WindTextureInfluence", this.windTextureInfluence);
		Shader.SetGlobalFloat("WindFogDensity", this.windFogDensity);
		Shader.SetGlobalFloat("WindFogTextureDensity", this.WindFogTextureDensity);
		Shader.SetGlobalFloat("WindMixInfluence", this.windMixInfluence);
		Shader.SetGlobalVector("WindSpeed", new Vector4(this.windSpeed.x, this.windSpeed.y, 0f, 0f));
		Vector3 forward = base.transform.forward;
		Vector3 vector = -Vector3.Cross(Vector3.up, forward);
		float value = Vector3.Angle(Vector3.up, forward);
		if (this.straightDown)
		{
			vector = Vector3.forward;
			value = 180f;
		}
		Shader.SetGlobalVector("WindRotationAxis", new Vector4(vector.x, vector.y, vector.z, 0f));
		Shader.SetGlobalFloat("WindRotationAngle", value);
		Shader.SetGlobalFloat("WindSphereScale", this.windSphereScale);
	}

	// Token: 0x04001047 RID: 4167
	public static FogConfig currentFog;

	// Token: 0x04001048 RID: 4168
	public float windSkyBrightnessValue = 0.2f;

	// Token: 0x04001049 RID: 4169
	public float windTextureInfluence = 0.2f;

	// Token: 0x0400104A RID: 4170
	public Color windTint = Color.white;

	// Token: 0x0400104B RID: 4171
	public Texture windTexture;

	// Token: 0x0400104C RID: 4172
	public float windSphereScale = 5f;

	// Token: 0x0400104D RID: 4173
	public Vector2 windSpeed;

	// Token: 0x0400104E RID: 4174
	public float windFogDensity = 50f;

	// Token: 0x0400104F RID: 4175
	public float WindFogTextureDensity = 15f;

	// Token: 0x04001050 RID: 4176
	public float windMixInfluence;

	// Token: 0x04001051 RID: 4177
	public float maxVal = 1f;

	// Token: 0x04001052 RID: 4178
	public bool straightDown;

	// Token: 0x04001053 RID: 4179
	private float sinceSet = 10f;

	// Token: 0x04001054 RID: 4180
	public WindChillZone zone;

	// Token: 0x04001055 RID: 4181
	public bool useWindChillZoneIntensity;
}
