using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200024D RID: 589
public class DayNightManager : MonoBehaviour
{
	// Token: 0x060011D3 RID: 4563 RVA: 0x00059714 File Offset: 0x00057914
	public string getShaderValue(DayNightManager.ShaderParams parameter)
	{
		switch (parameter)
		{
		case DayNightManager.ShaderParams.AirDistortion:
			return "_GlobalHazeAmount";
		case DayNightManager.ShaderParams.RimFresnel:
			return "RimFresnelIntensity";
		case DayNightManager.ShaderParams.LavaAlpha:
			return "LavaAlpha";
		case DayNightManager.ShaderParams.BandFog:
			return "BandFogAmount";
		case DayNightManager.ShaderParams.SunSize:
			return "SunSizeMult";
		case DayNightManager.ShaderParams.HeightFogAmount:
			return "HeightFogAmount";
		case DayNightManager.ShaderParams.HeightFogHeight:
			return "HeightFogHeight";
		default:
			return "";
		}
	}

	// Token: 0x060011D4 RID: 4564 RVA: 0x00059774 File Offset: 0x00057974
	public void clearAllShaderParams()
	{
		foreach (object obj in Enum.GetValues(typeof(DayNightManager.ShaderParams)))
		{
			DayNightManager.ShaderParams parameter = (DayNightManager.ShaderParams)obj;
			Shader.SetGlobalFloat(this.getShaderValue(parameter), 0f);
		}
	}

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x060011D5 RID: 4565 RVA: 0x000597E0 File Offset: 0x000579E0
	public float timeOfDayNormalized
	{
		get
		{
			return this.timeOfDay % 24f / 24f;
		}
	}

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x060011D6 RID: 4566 RVA: 0x000597F4 File Offset: 0x000579F4
	public float isDay
	{
		get
		{
			return (float)((this.timeOfDay >= this.dayStart && this.timeOfDay < this.dayEnd) ? 1 : 0);
		}
	}

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x060011D7 RID: 4567 RVA: 0x00059818 File Offset: 0x00057A18
	public float dayNightBlend
	{
		get
		{
			return Mathf.Lerp(this.currentProfile.sunGradient.Evaluate(this.timeOfDayNormalized).a, this.newProfile.sunGradient.Evaluate(this.timeOfDayNormalized).a, this.profileBlend);
		}
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x00059866 File Offset: 0x00057A66
	private void Awake()
	{
		DayNightManager.instance = this;
		this.newProfile = this.currentProfile;
		this.clearAllShaderParams();
	}

	// Token: 0x060011D9 RID: 4569 RVA: 0x00059880 File Offset: 0x00057A80
	public void BlendProfiles(DayNightProfile profile)
	{
		DayNightManager.<>c__DisplayClass46_0 CS$<>8__locals1 = new DayNightManager.<>c__DisplayClass46_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.profile = profile;
		base.StartCoroutine(CS$<>8__locals1.<BlendProfiles>g__blendRoutine|0());
		List<string> list = new List<string>();
		if (this.newProfile.globalShaderFloats != null)
		{
			for (int i = 0; i < this.newProfile.globalShaderFloats.Length; i++)
			{
				this.allShaderFloats.Add(this.getShaderValue(this.newProfile.globalShaderFloats[i].parameter));
				list.Add(this.getShaderValue(this.newProfile.globalShaderFloats[i].parameter));
			}
		}
		if (this.newProfile.animatedGlobalShaderFloats != null)
		{
			for (int j = 0; j < this.newProfile.animatedGlobalShaderFloats.Length; j++)
			{
				this.allShaderFloats.Add(this.getShaderValue(this.newProfile.animatedGlobalShaderFloats[j].parameter));
				list.Add(this.getShaderValue(this.newProfile.animatedGlobalShaderFloats[j].parameter));
			}
		}
		List<string> list2 = new List<string>();
		for (int k = 0; k < this.lastShaderFloats.Count; k++)
		{
			if (!list.Contains(this.lastShaderFloats[k]))
			{
				list2.Add(this.lastShaderFloats[k]);
			}
		}
		this.lastShaderFloats = list2;
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x000599D8 File Offset: 0x00057BD8
	private void Start()
	{
		this.timeOfDay = this.startingTimeOfDay;
		this.UpdateCycle();
		this.photonView = base.GetComponent<PhotonView>();
		float num = (this.dayEnd - this.dayStart) / 24f;
		float num2 = 1f - num;
		this.dayNightRatio = num / num2;
	}

	// Token: 0x060011DB RID: 4571 RVA: 0x00059A28 File Offset: 0x00057C28
	public void setTimeOfDay(float timeToSet)
	{
		if (timeToSet > 48f)
		{
			this.timeOfDay = 48f;
		}
		this.timeOfDay = timeToSet;
	}

	// Token: 0x060011DC RID: 4572 RVA: 0x00059A44 File Offset: 0x00057C44
	private void Update()
	{
		this.HazeDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.AirDistortion));
		this.RimFresnelDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.RimFresnel));
		this.LavaAlphaDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.LavaAlpha));
		this.BandFogDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.BandFog));
		this.SunSizeDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.SunSize));
		this.timeOfDay += 1f / (this.dayLengthInMinutes * 60f) * Time.deltaTime * 24f;
		if (this.timeOfDay > 24f)
		{
			this.timeOfDay -= 24f;
			this.passedMidnight = true;
		}
		if (this.passedMidnight && this.timeOfDay >= 5.5f)
		{
			this.dayCount++;
			this.passedMidnight = false;
		}
		if (PhotonNetwork.IsMasterClient && GameUtils.instance && !GameUtils.instance.m_inAirport)
		{
			this.syncTimer += Time.deltaTime;
			if (this.syncTimer > 5f)
			{
				this.photonView.RPC("RPCA_SyncTime", RpcTarget.All, new object[]
				{
					this.timeOfDay
				});
				this.syncTimer = 0f;
			}
		}
		this.UpdateCycle();
	}

	// Token: 0x060011DD RID: 4573 RVA: 0x00059B9A File Offset: 0x00057D9A
	public string DayCountString()
	{
		return LocalizedText.GetText("DAY", true).Replace("#", DayNightManager.IntToNumberWord(DayNightManager.instance.dayCount) ?? "");
	}

	// Token: 0x060011DE RID: 4574 RVA: 0x00059BCC File Offset: 0x00057DCC
	public string TimeOfDayString()
	{
		if (this.timeOfDay >= 23.5f)
		{
			return "night";
		}
		if (this.timeOfDay >= 17.5f)
		{
			return "evening";
		}
		if (this.timeOfDay >= 11.5f)
		{
			return "afternoon";
		}
		if (this.timeOfDay >= 5.5f)
		{
			return "morning";
		}
		return "night";
	}

	// Token: 0x060011DF RID: 4575 RVA: 0x00059C2C File Offset: 0x00057E2C
	private static string IntToNumberWord(int x)
	{
		if (x == 1)
		{
			return LocalizedText.GetText("One", true);
		}
		if (x == 2)
		{
			return LocalizedText.GetText("Two", true);
		}
		if (x == 3)
		{
			return LocalizedText.GetText("Three", true);
		}
		if (x == 4)
		{
			return LocalizedText.GetText("Four", true);
		}
		if (x == 5)
		{
			return LocalizedText.GetText("Five", true);
		}
		if (x == 6)
		{
			return LocalizedText.GetText("Six", true);
		}
		if (x == 7)
		{
			return LocalizedText.GetText("Seven", true);
		}
		if (x == 8)
		{
			return LocalizedText.GetText("Eight", true);
		}
		if (x == 9)
		{
			return LocalizedText.GetText("Nine", true);
		}
		if (x == 10)
		{
			return LocalizedText.GetText("Ten", true);
		}
		return x.ToString() ?? "";
	}

	// Token: 0x060011E0 RID: 4576 RVA: 0x00059CEC File Offset: 0x00057EEC
	public string FloatToTimeString(float time)
	{
		time = Mathf.Clamp(time, 0f, 24f);
		int num = Mathf.FloorToInt(time);
		int num2 = Mathf.FloorToInt((time - (float)num) * 60f);
		string arg = (num < 12) ? "AM" : "PM";
		int num3 = (num % 12 == 0) ? 12 : (num % 12);
		return string.Format("{0:D2}:{1:D2} {2}", num3, num2, arg);
	}

	// Token: 0x060011E1 RID: 4577 RVA: 0x00059D5A File Offset: 0x00057F5A
	[PunRPC]
	public void RPCA_SyncTime(float time)
	{
		this.timeOfDay = time;
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x00059D63 File Offset: 0x00057F63
	private void OnValidate()
	{
		this.UpdateCycle();
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x00059D6C File Offset: 0x00057F6C
	public void UpdateCycle()
	{
		float timeOfDayNormalized = this.timeOfDayNormalized;
		Vector3 euler = this.highNoonRotation + new Vector3(0f, 0f, this.angleOffsetZ.Evaluate(timeOfDayNormalized));
		float num = timeOfDayNormalized;
		if (this.isDay < 0.5f)
		{
			if (num > this.dayEnd / 24f)
			{
				num = this.dayEnd / 24f - (num - this.dayEnd / 24f) * this.dayNightRatio;
			}
			else if (num < this.dayStart / 24f)
			{
				num = this.dayStart / 24f + (this.dayStart / 24f - num) * this.dayNightRatio;
			}
		}
		Vector3 euler2 = new Vector3((num * this.rotDir - 0.5f) * 360f, 0f, 0f);
		this.earth.transform.rotation = Quaternion.Euler(euler) * Quaternion.Euler(euler2);
		Color color = Color.Lerp(Color.Lerp(this.currentProfile.sunGradient.Evaluate(timeOfDayNormalized), this.newProfile.sunGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialSunColor, this.specialDaySunBlend);
		Color value = Color.Lerp(Color.Lerp(this.currentProfile.skyTopGradient.Evaluate(timeOfDayNormalized), this.newProfile.skyTopGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialTopColor, this.specialDaySkyBlend);
		Color value2 = Color.Lerp(Color.Lerp(this.currentProfile.skyMidGradient.Evaluate(timeOfDayNormalized), this.newProfile.skyMidGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialMidColor, this.specialDaySkyBlend);
		Color value3 = Color.Lerp(Color.Lerp(this.currentProfile.skyBottomGradient.Evaluate(timeOfDayNormalized), this.newProfile.skyBottomGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialBottomColor, this.specialDaySkyBlend);
		Shader.SetGlobalColor(DayNightManager.SkyTopColor, value);
		Shader.SetGlobalColor(DayNightManager.SkyMidColor, value2);
		Shader.SetGlobalColor(DayNightManager.SkyBottomColor, value3);
		Shader.SetGlobalFloat(DayNightManager.TIMEOFDAY, timeOfDayNormalized);
		Shader.SetGlobalFloat(DayNightManager.Name, this.isDay);
		Shader.SetGlobalFloat(DayNightManager.FOG, Mathf.Lerp(this.currentProfile.fogGradient.Evaluate(timeOfDayNormalized).r, this.newProfile.fogGradient.Evaluate(timeOfDayNormalized).r, this.profileBlend));
		this.sun.color = color;
		this.moon.color = color;
		if (!this.isBlending && this.currentProfile.animatedGlobalShaderFloats != null)
		{
			for (int i = 0; i < this.currentProfile.animatedGlobalShaderFloats.Length; i++)
			{
				Shader.SetGlobalFloat(this.getShaderValue(this.currentProfile.animatedGlobalShaderFloats[i].parameter), this.currentProfile.animatedGlobalShaderFloats[i].paramValue.Evaluate(timeOfDayNormalized));
			}
		}
		float num2 = -(this.snowstormWindFactor * 1.75f + this.rainstormWindFactor * 1.25f);
		float num3 = Mathf.Lerp(this.currentProfile.sunIntensity, this.newProfile.sunIntensity, this.profileBlend);
		this.sun.intensity = Mathf.Max(0.015f, (color.a * 2f - 1f) * 0.5f * num3 + num2);
		float num4 = Mathf.Lerp(this.currentProfile.sunIntensity, this.newProfile.sunIntensity, this.profileBlend);
		this.moon.intensity = Mathf.Max(0.015f, (1f - color.a * 2f) * 0.5f * num4 + num2);
		this.lensFlare.intensity = (color.a - 0.5f) * 2f + num2;
		if (this.specialDaySunBlend < 0.5f)
		{
			if (color.a < 0.5f)
			{
				this.sun.enabled = false;
				this.moon.enabled = true;
				Shader.SetGlobalInt(DayNightManager.IsDayReal, 0);
			}
			else
			{
				this.moon.enabled = false;
				this.sun.enabled = true;
				Shader.SetGlobalInt(DayNightManager.IsDayReal, 1);
			}
		}
		else
		{
			this.moon.enabled = false;
			this.sun.enabled = false;
		}
		this.sun.shadowStrength = math.saturate(num3);
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x0005A1E0 File Offset: 0x000583E0
	private void OnDisable()
	{
		for (int i = 0; i < this.allShaderFloats.Count; i++)
		{
			Shader.SetGlobalFloat(this.allShaderFloats[i], 0f);
		}
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x0005A219 File Offset: 0x00058419
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(this.earth.transform.position, this.sun.transform.position);
	}

	// Token: 0x04000FC9 RID: 4041
	public static DayNightManager instance;

	// Token: 0x04000FCA RID: 4042
	private static readonly int TIMEOFDAY = Shader.PropertyToID("_TimeOfDay");

	// Token: 0x04000FCB RID: 4043
	private static readonly int FOG = Shader.PropertyToID("EXTRAFOG");

	// Token: 0x04000FCC RID: 4044
	private static readonly int Name = Shader.PropertyToID("IsDay");

	// Token: 0x04000FCD RID: 4045
	private static readonly int IsDayReal = Shader.PropertyToID("IsDayReal");

	// Token: 0x04000FCE RID: 4046
	private static readonly int SkyTopColor = Shader.PropertyToID("SkyTopColor");

	// Token: 0x04000FCF RID: 4047
	private static readonly int SkyMidColor = Shader.PropertyToID("SkyMidColor");

	// Token: 0x04000FD0 RID: 4048
	private static readonly int SkyBottomColor = Shader.PropertyToID("SkyBottomColor");

	// Token: 0x04000FD1 RID: 4049
	[Range(0f, 48f)]
	public float timeOfDay;

	// Token: 0x04000FD2 RID: 4050
	public float dayLengthInMinutes = 10f;

	// Token: 0x04000FD3 RID: 4051
	public float startingTimeOfDay = 9f;

	// Token: 0x04000FD4 RID: 4052
	public float dayStart = 5f;

	// Token: 0x04000FD5 RID: 4053
	public float dayEnd = 21f;

	// Token: 0x04000FD6 RID: 4054
	public int dayCount = 1;

	// Token: 0x04000FD7 RID: 4055
	public LensFlareComponentSRP lensFlare;

	// Token: 0x04000FD8 RID: 4056
	public AnimationCurve angleOffsetZ;

	// Token: 0x04000FD9 RID: 4057
	public Vector3 highNoonRotation;

	// Token: 0x04000FDA RID: 4058
	public Transform earth;

	// Token: 0x04000FDB RID: 4059
	public Light sun;

	// Token: 0x04000FDC RID: 4060
	public Light moon;

	// Token: 0x04000FDD RID: 4061
	public DayNightProfile currentProfile;

	// Token: 0x04000FDE RID: 4062
	public DayNightProfile newProfile;

	// Token: 0x04000FDF RID: 4063
	private float profileBlend;

	// Token: 0x04000FE0 RID: 4064
	[Header("Special Day")]
	[Range(0f, 1f)]
	public float specialDaySunBlend;

	// Token: 0x04000FE1 RID: 4065
	public Color specialSunColor;

	// Token: 0x04000FE2 RID: 4066
	public float specialDaySkyBlend;

	// Token: 0x04000FE3 RID: 4067
	public Color specialTopColor;

	// Token: 0x04000FE4 RID: 4068
	public Color specialMidColor;

	// Token: 0x04000FE5 RID: 4069
	public Color specialBottomColor;

	// Token: 0x04000FE6 RID: 4070
	private List<string> lastShaderFloats = new List<string>();

	// Token: 0x04000FE7 RID: 4071
	private List<string> allShaderFloats = new List<string>();

	// Token: 0x04000FE8 RID: 4072
	private bool isBlending;

	// Token: 0x04000FE9 RID: 4073
	public float rotDir = 1f;

	// Token: 0x04000FEA RID: 4074
	public float snowstormWindFactor;

	// Token: 0x04000FEB RID: 4075
	public float rainstormWindFactor;

	// Token: 0x04000FEC RID: 4076
	private PhotonView photonView;

	// Token: 0x04000FED RID: 4077
	public float dayNightRatio = 2f;

	// Token: 0x04000FEE RID: 4078
	public float syncTimer;

	// Token: 0x04000FEF RID: 4079
	private bool passedMidnight;

	// Token: 0x04000FF0 RID: 4080
	public float HazeDebug;

	// Token: 0x04000FF1 RID: 4081
	public float RimFresnelDebug;

	// Token: 0x04000FF2 RID: 4082
	public float LavaAlphaDebug;

	// Token: 0x04000FF3 RID: 4083
	public float BandFogDebug;

	// Token: 0x04000FF4 RID: 4084
	public float SunSizeDebug;

	// Token: 0x02000500 RID: 1280
	public enum ShaderParams
	{
		// Token: 0x04001BE9 RID: 7145
		AirDistortion,
		// Token: 0x04001BEA RID: 7146
		RimFresnel,
		// Token: 0x04001BEB RID: 7147
		LavaAlpha,
		// Token: 0x04001BEC RID: 7148
		BandFog,
		// Token: 0x04001BED RID: 7149
		SunSize,
		// Token: 0x04001BEE RID: 7150
		HeightFogAmount,
		// Token: 0x04001BEF RID: 7151
		HeightFogHeight
	}

	// Token: 0x02000501 RID: 1281
	[Serializable]
	public class ShaderParameters
	{
		// Token: 0x04001BF0 RID: 7152
		public string paramName;

		// Token: 0x04001BF1 RID: 7153
		public string paramValue;
	}
}
