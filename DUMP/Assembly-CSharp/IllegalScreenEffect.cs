using System;
using UnityEngine;

// Token: 0x02000280 RID: 640
public class IllegalScreenEffect : MonoBehaviour
{
	// Token: 0x06001291 RID: 4753 RVA: 0x0005D2FC File Offset: 0x0005B4FC
	private void Start()
	{
		this.rend = base.GetComponent<MeshRenderer>();
		this.rend.enabled = false;
		this.mat = this.rend.material;
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x0005D328 File Offset: 0x0005B528
	private void Update()
	{
		if (!this.character)
		{
			if (Character.localCharacter)
			{
				this.character = Character.localCharacter;
				Character character = this.character;
				character.illegalStatusAction = (Action<string, float>)Delegate.Combine(character.illegalStatusAction, new Action<string, float>(this.AddStatus));
			}
			return;
		}
		if (this.character.data.fullyPassedOut || this.character.data.dead)
		{
			this.activeForSeconds = 0f;
		}
		this.activeForSeconds -= Time.deltaTime;
		if (this.activeForSeconds > 0f)
		{
			this.rend.enabled = true;
			float num = Mathf.Clamp01(this.activeForSeconds / 3f);
			float @float = this.mat.GetFloat(this.shaderVarName);
			if (num > @float)
			{
				this.mat.SetFloat(this.shaderVarName, Mathf.Lerp(@float, num, Time.deltaTime));
			}
			else
			{
				this.mat.SetFloat(this.shaderVarName, num);
			}
			this.character.data.isBlind = true;
			return;
		}
		this.rend.enabled = false;
		this.character.data.isBlind = false;
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x0005D463 File Offset: 0x0005B663
	private void AddStatus(string status, float duration)
	{
		if (status.ToUpper() != this.statusName.ToUpper())
		{
			return;
		}
		this.activeForSeconds = duration;
	}

	// Token: 0x040010A3 RID: 4259
	public string statusName = "BLIND";

	// Token: 0x040010A4 RID: 4260
	public string shaderVarName = "_Alpha";

	// Token: 0x040010A5 RID: 4261
	private float activeForSeconds;

	// Token: 0x040010A6 RID: 4262
	private Character character;

	// Token: 0x040010A7 RID: 4263
	private MeshRenderer rend;

	// Token: 0x040010A8 RID: 4264
	private Material mat;
}
