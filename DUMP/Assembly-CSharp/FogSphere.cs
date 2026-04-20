using System;
using UnityEngine;

// Token: 0x0200026D RID: 621
[ExecuteAlways]
public class FogSphere : MonoBehaviour
{
	// Token: 0x0600124C RID: 4684 RVA: 0x0005BA84 File Offset: 0x00059C84
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	// Token: 0x0600124D RID: 4685 RVA: 0x0005BA92 File Offset: 0x00059C92
	private void OnDisable()
	{
		Shader.SetGlobalFloat("FogEnabled", 0f);
		Shader.SetGlobalFloat("_FogSphereSize", 9999999f);
	}

	// Token: 0x0600124E RID: 4686 RVA: 0x0005BAB4 File Offset: 0x00059CB4
	private void Update()
	{
		this.SetSize();
		this.SetSharderVars();
		if (this.currentSize > 120f)
		{
			this.t = false;
		}
		if (!this.t && this.currentSize < 120f)
		{
			this.t = true;
			for (int i = 0; i < this.fogStart.Length; i++)
			{
				this.fogStart[i].Play(default(Vector3));
			}
		}
		if (!this.t2 && this.REVEAL_AMOUNT > 0.1f)
		{
			this.t2 = true;
			for (int j = 0; j < this.fogReveal.Length; j++)
			{
				this.fogReveal[j].Play(default(Vector3));
			}
		}
		if (this.REVEAL_AMOUNT < 0.1f)
		{
			this.t2 = false;
		}
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x0005BB80 File Offset: 0x00059D80
	private void SetSharderVars()
	{
		if (this.mpb == null)
		{
			this.mpb = new MaterialPropertyBlock();
		}
		this.rend.GetPropertyBlock(this.mpb);
		this.mpb.SetFloat("_PADDING", this.PADDING);
		this.mpb.SetFloat("_FogDepth", this.currentSize);
		this.mpb.SetFloat("_RevealAmount", this.REVEAL_AMOUNT);
		this.mpb.SetVector("_FogCenter", this.fogPoint);
		Shader.SetGlobalFloat("_FogSphereSize", this.currentSize);
		Shader.SetGlobalVector("FogCenter", this.fogPoint);
		Shader.SetGlobalFloat("FogEnabled", this.ENABLE);
		if (Application.isPlaying && Character.localCharacter != null)
		{
			Character.localCharacter.data.isInFog = false;
			if (Mathf.Approximately(this.ENABLE, 1f) && Character.localCharacter != null && Vector3.Distance(this.fogPoint, Character.localCharacter.Center) > this.currentSize)
			{
				float num = 0.0105f;
				if (Character.localCharacter.data.isSkeleton)
				{
					Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, num / 8f * Time.deltaTime, false, true, true);
				}
				else
				{
					Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, num * Time.deltaTime, false, true, true);
				}
				Character.localCharacter.data.isInFog = true;
			}
		}
		this.rend.SetPropertyBlock(this.mpb);
	}

	// Token: 0x06001250 RID: 4688 RVA: 0x0005BD30 File Offset: 0x00059F30
	private void SetSize()
	{
		float d = (this.currentSize + this.PADDING) * this.ratio;
		base.transform.localScale = Vector3.one * d;
	}

	// Token: 0x06001251 RID: 4689 RVA: 0x0005BD68 File Offset: 0x00059F68
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(this.fogPoint, this.currentSize);
	}

	// Token: 0x0400105A RID: 4186
	public float currentSize = 50f;

	// Token: 0x0400105B RID: 4187
	[Range(0f, 1f)]
	public float ENABLE = 1f;

	// Token: 0x0400105C RID: 4188
	[Range(0f, 1f)]
	public float REVEAL_AMOUNT;

	// Token: 0x0400105D RID: 4189
	public float PADDING = 300f;

	// Token: 0x0400105E RID: 4190
	public Vector3 fogPoint;

	// Token: 0x0400105F RID: 4191
	private float ratio = 2f;

	// Token: 0x04001060 RID: 4192
	private Renderer rend;

	// Token: 0x04001061 RID: 4193
	public SFX_Instance[] fogStart;

	// Token: 0x04001062 RID: 4194
	private bool t;

	// Token: 0x04001063 RID: 4195
	public SFX_Instance[] fogReveal;

	// Token: 0x04001064 RID: 4196
	private bool t2;

	// Token: 0x04001065 RID: 4197
	private MaterialPropertyBlock mpb;
}
