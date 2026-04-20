using System;
using UnityEngine;

// Token: 0x02000056 RID: 86
[ExecuteInEditMode]
public class AmbienceManager : MonoBehaviour
{
	// Token: 0x06000473 RID: 1139 RVA: 0x0001B573 File Offset: 0x00019773
	private void Awake()
	{
		AmbienceManager.instance = this;
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x0001B57B File Offset: 0x0001977B
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			this.Start();
		}
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x0001B58A File Offset: 0x0001978A
	private void UpdateFog()
	{
		if (Application.isPlaying)
		{
			this.useFog = true;
		}
		Shader.SetGlobalFloat(AmbienceManager.Usefog, (float)(this.useFog ? 1 : 0));
		Shader.SetGlobalFloat(AmbienceManager.Maxfog, this.maxFog);
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x0001B5C1 File Offset: 0x000197C1
	private void Start()
	{
		this.UpdateFog();
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x0001B5CC File Offset: 0x000197CC
	private void Update()
	{
		this.UpdateFog();
		RenderSettings.skybox = this.skyboxMaterial;
		RenderSettings.fogColor = this.fogColor;
		if (!this.dayNight)
		{
			return;
		}
		Color color = this.ambienceGradient.Evaluate(this.dayNight.timeOfDayNormalized);
		RenderSettings.ambientLight = color * this.brightness * color.a;
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x0001B636 File Offset: 0x00019836
	public void ToggleFog()
	{
		this.useFog = !this.useFog;
		Shader.SetGlobalFloat(AmbienceManager.Usefog, (float)(this.useFog ? 1 : 0));
	}

	// Token: 0x040004D7 RID: 1239
	private static readonly int Maxfog = Shader.PropertyToID("MAXFOG");

	// Token: 0x040004D8 RID: 1240
	private static readonly int Usefog = Shader.PropertyToID("USEFOG");

	// Token: 0x040004D9 RID: 1241
	public Color ambienceColor;

	// Token: 0x040004DA RID: 1242
	public Gradient ambienceGradient;

	// Token: 0x040004DB RID: 1243
	public Color fogColor;

	// Token: 0x040004DC RID: 1244
	public float brightness = 1f;

	// Token: 0x040004DD RID: 1245
	public Material skyboxMaterial;

	// Token: 0x040004DE RID: 1246
	public DayNightManager dayNight;

	// Token: 0x040004DF RID: 1247
	public float maxFog = 500f;

	// Token: 0x040004E0 RID: 1248
	public bool useFog = true;

	// Token: 0x040004E1 RID: 1249
	public static AmbienceManager instance;
}
