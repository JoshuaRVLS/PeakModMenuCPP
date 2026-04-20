using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200013E RID: 318
public class Mirage : MonoBehaviour
{
	// Token: 0x06000A74 RID: 2676 RVA: 0x00037BFC File Offset: 0x00035DFC
	public void fadeMirage()
	{
		if (Application.isPlaying)
		{
			MirageManager.instance.sampleCamera();
		}
		this.ps.Play();
		this.setParticleData();
		if (this.hideObjects)
		{
			base.StartCoroutine(this.HideObject());
		}
		this.hasFaded = true;
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x00037C3C File Offset: 0x00035E3C
	private IEnumerator HideObject()
	{
		yield return new WaitForSeconds(this.hideDelay);
		for (int i = 0; i < this.objectsToHide.Length; i++)
		{
			this.objectsToHide[i].SetActive(false);
		}
		yield break;
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x00037C4B File Offset: 0x00035E4B
	private void setParticleData()
	{
		base.StartCoroutine(this.<setParticleData>g__particleDataRoutine|11_0());
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x00037C5A File Offset: 0x00035E5A
	public void AssignRT(RenderTexture texture)
	{
		this.ps.GetComponent<ParticleSystemRenderer>().material.SetTexture("_MainTex", texture);
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x00037C77 File Offset: 0x00035E77
	private void Start()
	{
		this.psr = this.ps.GetComponent<ParticleSystemRenderer>();
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x00037C8A File Offset: 0x00035E8A
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		if (Vector3.Distance(Character.observedCharacter.Center, base.transform.position) < this.fadeDistance && !this.hasFaded)
		{
			this.fadeMirage();
		}
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x00037CCA File Offset: 0x00035ECA
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.fadeDistance);
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x00037D00 File Offset: 0x00035F00
	[CompilerGenerated]
	private IEnumerator <setParticleData>g__particleDataRoutine|11_0()
	{
		yield return new WaitForEndOfFrame();
		this.ps.GetCustomParticleData(this.customData, ParticleSystemCustomData.Custom1);
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[this.ps.particleCount];
		this.ps.GetParticles(array);
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(array[i].position);
			this.customData[i] = new Vector4(vector.x / (float)Screen.width, vector.y / (float)Screen.height, 0f, 1f);
		}
		this.ps.SetCustomParticleData(this.customData, ParticleSystemCustomData.Custom1);
		yield break;
	}

	// Token: 0x040009B0 RID: 2480
	[FormerlySerializedAs("particleSystem")]
	public ParticleSystem ps;

	// Token: 0x040009B1 RID: 2481
	public RenderTexture rt;

	// Token: 0x040009B2 RID: 2482
	private ParticleSystemRenderer psr;

	// Token: 0x040009B3 RID: 2483
	public bool hideObjects;

	// Token: 0x040009B4 RID: 2484
	public GameObject[] objectsToHide;

	// Token: 0x040009B5 RID: 2485
	public float hideDelay;

	// Token: 0x040009B6 RID: 2486
	public float fadeDistance = 10f;

	// Token: 0x040009B7 RID: 2487
	private List<Vector4> customData = new List<Vector4>();

	// Token: 0x040009B8 RID: 2488
	public bool hasFaded;
}
