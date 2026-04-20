using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Token: 0x020000B9 RID: 185
public class EyeBlinkController : MonoBehaviour
{
	// Token: 0x060006F5 RID: 1781 RVA: 0x0002805C File Offset: 0x0002625C
	private void Start()
	{
		this.character = base.GetComponentInParent<Character>();
		if (!this.character.IsLocal)
		{
			base.enabled = false;
			return;
		}
		foreach (ScriptableRendererFeature scriptableRendererFeature in this.rend.rendererFeatures)
		{
			if (scriptableRendererFeature.name == "Eye Blink")
			{
				this.rendererFeature = scriptableRendererFeature;
			}
		}
		this.rendererFeature.SetActive(true);
		this.setEyeBlinkActive();
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x000280FC File Offset: 0x000262FC
	private void setEyeBlinkActive()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		this.eyeBlinkMaterial.SetFloat("_EyeOpen", (float)(this.enableEyeBlink ? 1 : 0));
		if (!this.enableEyeBlink)
		{
			this.rendererFeature.SetActive(false);
			this.eyeBlinkMaterial.SetFloat("_EyeOpen", 1f);
		}
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x00028160 File Offset: 0x00026360
	private void Update()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		if (this.character.data.passedOutOnTheBeach > 0f)
		{
			this.eyeOpenValue = 0f;
			this.enableEyeBlink = true;
		}
		else
		{
			this.eyeOpenValue = Mathf.MoveTowards(this.eyeOpenValue, 1f, Time.deltaTime * 0.15f);
			if (this.eyeOpenValue >= 0.999f)
			{
				this.enableEyeBlink = false;
				this.rendererFeature.SetActive(false);
			}
			else
			{
				this.enableEyeBlink = true;
			}
		}
		if (this.enableEyeBlink)
		{
			this.rendererFeature.SetActive(true);
			this.eyeBlinkMaterial.SetFloat("_EyeOpen", Mathf.Clamp01(this.openCurve.Evaluate(this.eyeOpenValue)));
		}
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x0002822A File Offset: 0x0002642A
	private void OnDisable()
	{
		if (this.rendererFeature != null)
		{
			this.rendererFeature.SetActive(false);
		}
		this.eyeBlinkMaterial.SetFloat("_EyeOpen", 1f);
	}

	// Token: 0x04000704 RID: 1796
	private Character character;

	// Token: 0x04000705 RID: 1797
	public UniversalRendererData rend;

	// Token: 0x04000706 RID: 1798
	public Material eyeBlinkMaterial;

	// Token: 0x04000707 RID: 1799
	public bool enableEyeBlink;

	// Token: 0x04000708 RID: 1800
	public AnimationCurve openCurve;

	// Token: 0x04000709 RID: 1801
	[Range(0f, 1f)]
	public float eyeOpenValue;

	// Token: 0x0400070A RID: 1802
	private ScriptableRendererFeature rendererFeature;
}
