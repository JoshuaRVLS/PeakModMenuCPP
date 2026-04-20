using System;
using System.Collections;
using System.Runtime.CompilerServices;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020001B3 RID: 435
public class SpawnedVine : MonoBehaviour
{
	// Token: 0x06000E04 RID: 3588 RVA: 0x000468D5 File Offset: 0x00044AD5
	private void Start()
	{
		this.vine = base.GetComponent<JungleVine>();
		this.SpawnVine();
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x000468EC File Offset: 0x00044AEC
	public void SpawnVine()
	{
		if (this.startObject != null)
		{
			this.startObject.transform.position = this.vine.colliderRoot.GetChild(0).transform.position;
			Vector3 position = this.vine.GetPosition(1f, 0, 0);
			position = new Vector3(position.x, this.startObject.transform.position.y, position.z);
			this.startObject.transform.LookAt(position);
			if (this.endObject != null)
			{
				this.endObject.transform.forward = this.vine.colliderRoot.GetLastChild().transform.up;
			}
		}
		if (this.endObject != null)
		{
			this.endObject.transform.position = this.vine.GetPosition(1f, 0, 0);
		}
		base.StartCoroutine(this.<SpawnVine>g__waveFX|7_0());
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x000469F6 File Offset: 0x00044BF6
	private void Update()
	{
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x00046A1C File Offset: 0x00044C1C
	[CompilerGenerated]
	private IEnumerator <SpawnVine>g__waveFX|7_0()
	{
		float normalizedTime = 0f;
		while (normalizedTime < 1f)
		{
			normalizedTime += Time.deltaTime / this.vineWaveDecay;
			float value = Mathf.Lerp(100f, 0f, normalizedTime);
			this.vineRenderer.material.SetFloat(SpawnedVine.JitterAmount, value);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000BD8 RID: 3032
	private static readonly int JitterAmount = Shader.PropertyToID("_JitterAmount");

	// Token: 0x04000BD9 RID: 3033
	private JungleVine vine;

	// Token: 0x04000BDA RID: 3034
	public MeshRenderer vineRenderer;

	// Token: 0x04000BDB RID: 3035
	public float vineWaveDecay = 0.5f;

	// Token: 0x04000BDC RID: 3036
	public GameObject startObject;

	// Token: 0x04000BDD RID: 3037
	public GameObject endObject;
}
