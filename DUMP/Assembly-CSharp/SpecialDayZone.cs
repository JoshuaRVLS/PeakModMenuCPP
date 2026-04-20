using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000347 RID: 839
public class SpecialDayZone : MonoBehaviour
{
	// Token: 0x0600164B RID: 5707 RVA: 0x000712A8 File Offset: 0x0006F4A8
	private void Start()
	{
		this.bounds.center = base.transform.position;
		this.outerBounds.center = base.transform.position;
		this.outerBounds.size = this.bounds.size + Vector3.one * this.blendSize;
		if (this.specialLight)
		{
			this.specialLight.color = Color.black;
		}
		if (this.specialLight)
		{
			this.specialLight.enabled = false;
		}
	}

	// Token: 0x0600164C RID: 5708 RVA: 0x00071344 File Offset: 0x0006F544
	private void OnDrawGizmosSelected()
	{
		this.bounds.center = base.transform.position;
		this.outerBounds.center = base.transform.position;
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
		Gizmos.DrawWireCube(this.bounds.center, new Vector3(this.bounds.size.x / 2f, this.bounds.size.y, this.bounds.size.z));
		Gizmos.DrawWireCube(this.bounds.center, new Vector3(this.bounds.size.x, this.bounds.size.y / 2f, this.bounds.size.z));
		Gizmos.DrawWireCube(this.bounds.center, new Vector3(this.bounds.size.x, this.bounds.size.y, this.bounds.size.z / 2f));
		Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
		this.outerBounds.size = this.bounds.size + Vector3.one * this.blendSize;
		Gizmos.DrawWireCube(this.outerBounds.center, this.outerBounds.size);
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x000714E0 File Offset: 0x0006F6E0
	private void Update()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		if (this.outerBounds.Contains(Character.localCharacter.Center))
		{
			if (this.specialLight)
			{
				this.specialLight.enabled = true;
			}
			this.inBounds = true;
			return;
		}
		if (this.specialLight)
		{
			this.specialLight.enabled = false;
		}
		this.inBounds = false;
	}

	// Token: 0x0600164E RID: 5710 RVA: 0x00071552 File Offset: 0x0006F752
	private void OnDisable()
	{
		if (this.specialLight)
		{
			this.specialLight.color = Color.black;
		}
		this.specialLight.enabled = false;
	}

	// Token: 0x0400145C RID: 5212
	public bool overrideSun;

	// Token: 0x0400145D RID: 5213
	[FormerlySerializedAs("lightIntensity")]
	public float daylLightIntensity = 1.5f;

	// Token: 0x0400145E RID: 5214
	public float nightLightIntensity = 1.5f;

	// Token: 0x0400145F RID: 5215
	public Color specialSunColor;

	// Token: 0x04001460 RID: 5216
	public bool useCustomSun;

	// Token: 0x04001461 RID: 5217
	public Light specialLight;

	// Token: 0x04001462 RID: 5218
	public bool useCustomColorVals = true;

	// Token: 0x04001463 RID: 5219
	public Color specialTopColor;

	// Token: 0x04001464 RID: 5220
	public Color specialMidColor;

	// Token: 0x04001465 RID: 5221
	public Color specialBottomColor;

	// Token: 0x04001466 RID: 5222
	[FormerlySerializedAs("shaderValsToBlend")]
	public ShaderVal[] globalShaderVals;

	// Token: 0x04001467 RID: 5223
	private float baseFog;

	// Token: 0x04001468 RID: 5224
	public bool overrideFog = true;

	// Token: 0x04001469 RID: 5225
	public float fogDensity = 400f;

	// Token: 0x0400146A RID: 5226
	public Bounds bounds;

	// Token: 0x0400146B RID: 5227
	public Bounds outerBounds;

	// Token: 0x0400146C RID: 5228
	public float blendSize = 50f;

	// Token: 0x0400146D RID: 5229
	[Header("Debug")]
	public bool inBounds;
}
