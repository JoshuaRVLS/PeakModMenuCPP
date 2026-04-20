using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000291 RID: 657
public class LavaPost : MonoBehaviour
{
	// Token: 0x060012F0 RID: 4848 RVA: 0x0005FA44 File Offset: 0x0005DC44
	private void Start()
	{
		this.rend = base.GetComponent<MeshRenderer>();
		Shader.SetGlobalFloat("LavaAlpha", 0f);
		this.lava1Height = this.lava1.position.y;
		this.lava2Height = this.lava2.position.y;
		this.currentLavaHeight = this.lava1Height;
		this.lastLavaHeight = this.lava2Height;
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x0005FAB0 File Offset: 0x0005DCB0
	private void LateUpdate()
	{
		if (this.lava1 == null)
		{
			return;
		}
		bool flag = MainCamera.instance.transform.position.z < this.thresholdTransform.position.z;
		if (this.firstIsActive != flag)
		{
			this.alpha = Mathf.MoveTowards(this.alpha, 0f, Time.deltaTime);
			if (this.alpha < 0.001f)
			{
				this.firstIsActive = flag;
			}
		}
		else
		{
			this.alpha = Mathf.MoveTowards(this.alpha, 1f, Time.deltaTime);
		}
		float num = this.firstIsActive ? this.lava1Height : this.lava2Height;
		if (this.lastLavaHeight != num)
		{
			this.lastLavaHeight = num;
			base.StopAllCoroutines();
			base.StartCoroutine(this.lavaMove(num));
		}
		if (!this.blending)
		{
			float value = this.firstIsActive ? this.lava1.position.y : this.lava2.position.y;
			Shader.SetGlobalFloat("LavaHeight", value);
		}
		if (MainCamera.instance.transform.position.z < this.lavaFadeIn.position.z)
		{
			this.rend.enabled = false;
		}
		else
		{
			this.rend.enabled = true;
		}
		Shader.SetGlobalFloat("LavaStart", this.lavaStart.position.z);
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x0005FC1B File Offset: 0x0005DE1B
	public IEnumerator lavaMove(float newLavaHeight)
	{
		this.blending = true;
		float normalizedTime = 0f;
		while (normalizedTime < 1f)
		{
			this.currentLavaHeight = Mathf.Lerp(this.currentLavaHeight, newLavaHeight, normalizedTime / 3f);
			Shader.SetGlobalFloat("LavaHeight", this.currentLavaHeight);
			normalizedTime += Time.deltaTime;
			yield return null;
		}
		this.blending = false;
		yield break;
	}

	// Token: 0x04001104 RID: 4356
	private MeshRenderer rend;

	// Token: 0x04001105 RID: 4357
	public Transform lava1;

	// Token: 0x04001106 RID: 4358
	public Transform lava2;

	// Token: 0x04001107 RID: 4359
	private float lava1Height;

	// Token: 0x04001108 RID: 4360
	private float lava2Height;

	// Token: 0x04001109 RID: 4361
	public Transform thresholdTransform;

	// Token: 0x0400110A RID: 4362
	public Transform lavaFadeIn;

	// Token: 0x0400110B RID: 4363
	public Transform lavaStart;

	// Token: 0x0400110C RID: 4364
	private float alpha;

	// Token: 0x0400110D RID: 4365
	private bool firstIsActive;

	// Token: 0x0400110E RID: 4366
	private float currentLavaHeight;

	// Token: 0x0400110F RID: 4367
	private float lastLavaHeight;

	// Token: 0x04001110 RID: 4368
	private bool blending;
}
