using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

// Token: 0x02000120 RID: 288
public class ItemRenderFeatureManager : MonoBehaviour
{
	// Token: 0x0600096B RID: 2411 RVA: 0x00032A32 File Offset: 0x00030C32
	private void Start()
	{
		this.getRendererFeature();
		if (this.disabledOnStart)
		{
			this.setFeatureActive(false);
		}
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x00032A4C File Offset: 0x00030C4C
	private void getRendererFeature()
	{
		foreach (ScriptableRendererFeature scriptableRendererFeature in this.rend.rendererFeatures)
		{
			if (scriptableRendererFeature.name == this.featureName)
			{
				this.rendererFeature = scriptableRendererFeature;
			}
		}
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x00032AB8 File Offset: 0x00030CB8
	public void setFeatureActive(bool active)
	{
		this.rendererFeature.SetActive(active);
		MonoBehaviour.print(active);
		this.rendererFeature != null;
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x00032ADE File Offset: 0x00030CDE
	private void OnDisable()
	{
		this.setFeatureActive(false);
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x00032AE7 File Offset: 0x00030CE7
	private void Update()
	{
	}

	// Token: 0x040008C5 RID: 2245
	private ScriptableRendererFeature rendererFeature;

	// Token: 0x040008C6 RID: 2246
	[FormerlySerializedAs("falseOnStart")]
	public bool disabledOnStart;

	// Token: 0x040008C7 RID: 2247
	public UniversalRendererData rend;

	// Token: 0x040008C8 RID: 2248
	public string featureName;
}
