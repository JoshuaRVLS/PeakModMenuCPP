using System;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public class SpecialDayManager : MonoBehaviour
{
	// Token: 0x06000E0C RID: 3596 RVA: 0x00046A42 File Offset: 0x00044C42
	private void Start()
	{
		this.zones = Object.FindObjectsByType<SpecialDayZone>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		this.startFog = AmbienceManager.instance.maxFog;
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x00046A64 File Offset: 0x00044C64
	private void Update()
	{
		float a = 0f;
		if (!Character.observedCharacter)
		{
			return;
		}
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].outerBounds.Contains(Character.observedCharacter.Center))
			{
				Color specialSunColor = DayNightManager.instance.specialSunColor;
				Color specialTopColor = DayNightManager.instance.specialTopColor;
				Color specialMidColor = DayNightManager.instance.specialMidColor;
				Color specialBottomColor = DayNightManager.instance.specialBottomColor;
				float maxFog = AmbienceManager.instance.maxFog;
				float num = Vector3.Distance(Character.observedCharacter.Center, this.zones[i].bounds.ClosestPoint(Character.observedCharacter.Center));
				num /= this.zones[i].blendSize;
				float num2 = 1f - num * 2f;
				this.debugblend = num2;
				num2 = Mathf.Clamp01(num2);
				if (this.zones[i].overrideSun)
				{
					float b = Mathf.Lerp(this.zones[i].nightLightIntensity, this.zones[i].daylLightIntensity, DayNightManager.instance.dayNightBlend);
					DayNightManager.instance.specialSunColor = Color.Lerp(specialSunColor, this.zones[i].specialSunColor * b, num2);
				}
				if (this.zones[i].useCustomSun)
				{
					DayNightManager.instance.specialDaySunBlend = Mathf.Max(a, num2);
					if (this.zones[i].specialLight != null)
					{
						this.zones[i].specialLight.enabled = true;
						Color color = Color.Lerp(specialSunColor, this.zones[i].specialSunColor, num2);
						color *= num2;
						this.zones[i].specialLight.color = color;
						DayNightManager.instance.specialSunColor *= 1f - num2;
					}
				}
				Shader.SetGlobalFloat("SpecialDayBlend", Mathf.Lerp(0f, 1f, num2));
				if (this.zones[i].useCustomColorVals)
				{
					DayNightManager.instance.specialDaySkyBlend = Mathf.Max(a, num2);
					DayNightManager.instance.specialTopColor = Color.Lerp(specialTopColor, this.zones[i].specialTopColor, num2);
					DayNightManager.instance.specialMidColor = Color.Lerp(specialMidColor, this.zones[i].specialMidColor, num2);
					DayNightManager.instance.specialBottomColor = Color.Lerp(specialBottomColor, this.zones[i].specialBottomColor, num2);
				}
				if (this.zones[i].overrideFog)
				{
					float num3 = Mathf.Lerp(maxFog, this.zones[i].fogDensity, num2);
					num3 = Mathf.Lerp(this.startFog, num3, Mathf.Max(a, num2));
					AmbienceManager.instance.maxFog = num3;
				}
				if (this.zones[i].globalShaderVals.Length != 0)
				{
					float a2 = 0f;
					for (int j = 0; j < this.zones[i].globalShaderVals.Length; j++)
					{
						float num4 = Mathf.Lerp(a2, this.zones[i].globalShaderVals[j].value, num2);
						num4 *= Mathf.Max(a, num2);
						Shader.SetGlobalFloat(DayNightManager.instance.getShaderValue(this.zones[i].globalShaderVals[j].parameter), num4);
						a2 = num4;
					}
				}
				a = num2;
			}
			else if (this.zones[i].specialLight != null)
			{
				this.zones[i].specialLight.enabled = false;
			}
		}
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x00046E04 File Offset: 0x00045004
	private void OnDisable()
	{
		Shader.SetGlobalFloat("SpecialDayBlend", 0f);
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].globalShaderVals.Length != 0)
			{
				for (int j = 0; j < this.zones[i].globalShaderVals.Length; j++)
				{
					Shader.SetGlobalFloat(DayNightManager.instance.getShaderValue(this.zones[i].globalShaderVals[j].parameter), 0f);
				}
			}
		}
	}

	// Token: 0x04000BE0 RID: 3040
	public SpecialDayZone[] zones;

	// Token: 0x04000BE1 RID: 3041
	public float debug;

	// Token: 0x04000BE2 RID: 3042
	private float startFog;

	// Token: 0x04000BE3 RID: 3043
	public float debugblend;
}
