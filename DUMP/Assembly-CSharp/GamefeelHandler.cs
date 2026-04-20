using System;
using UnityEngine;
using Zorro.Settings;

// Token: 0x02000274 RID: 628
public class GamefeelHandler : MonoBehaviour
{
	// Token: 0x0600126E RID: 4718 RVA: 0x0005C769 File Offset: 0x0005A969
	private void Awake()
	{
		GamefeelHandler.instance = this;
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x0005C771 File Offset: 0x0005A971
	private void Start()
	{
		this.setting = GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>();
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x0005C788 File Offset: 0x0005A988
	public Vector3 GetRotation()
	{
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			vector += base.transform.GetChild(i).localEulerAngles;
		}
		return vector;
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x0005C7CA File Offset: 0x0005A9CA
	public void AddRotationShake_Local_Stiff(Vector3 force)
	{
		this.stiff.AddForce(force);
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x0005C7D8 File Offset: 0x0005A9D8
	public void AddRotationShake_Local_Loose(Vector3 force)
	{
		this.loose.AddForce(force);
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x0005C7E6 File Offset: 0x0005A9E6
	public void AddPerlinShake(float amount = 1f, float duration = 0.2f, float scale = 15f)
	{
		if (this.setting.Value == OffOnMode.ON)
		{
			amount *= 0.5f;
			amount = Mathf.Min(amount, 3f);
		}
		this.perlin.AddShake(amount, duration, scale);
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x0005C81C File Offset: 0x0005AA1C
	public void AddPerlinShakeProximity(Vector3 position, float amount = 1f, float duration = 0.2f, float scale = 15f, float maxProximity = 10f)
	{
		if (this.setting.Value == OffOnMode.ON)
		{
			amount *= 0.5f;
			amount = Mathf.Min(amount, 3f);
		}
		float num = 1f;
		if (Character.observedCharacter)
		{
			num = 1f - Mathf.Clamp01(Vector3.Distance(Character.observedCharacter.Center, position) / maxProximity);
		}
		this.perlin.AddShake(amount * num, duration, scale);
	}

	// Token: 0x04001083 RID: 4227
	public static GamefeelHandler instance;

	// Token: 0x04001084 RID: 4228
	public PhotosensitiveSetting setting;

	// Token: 0x04001085 RID: 4229
	public RotationSpring stiff;

	// Token: 0x04001086 RID: 4230
	public RotationSpring loose;

	// Token: 0x04001087 RID: 4231
	public PerlinShake perlin;
}
