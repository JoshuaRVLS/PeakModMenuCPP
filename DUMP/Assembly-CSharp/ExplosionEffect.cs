using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000262 RID: 610
public class ExplosionEffect : MonoBehaviour
{
	// Token: 0x0600121F RID: 4639 RVA: 0x0005AD90 File Offset: 0x00058F90
	private void Start()
	{
		this.GetPoints();
		foreach (ExplosionOrb item in this.explosionPoints)
		{
			base.StartCoroutine(this.<Start>g__IExplode|12_0(item));
		}
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x0005ADD0 File Offset: 0x00058FD0
	private void GetPoints()
	{
		int num = 1 + this.explosionPointCount + this.subExplosionPointCount * this.explosionPointCount;
		this.explosionPoints = new ExplosionOrb[num];
		this.explosionPoints[0] = new ExplosionOrb(base.transform.position, Vector3.up, 0f, 1f, 1f);
		int num2 = 0;
		float size = this.childSizeFactor * this.childSizeFactor;
		for (int i = 0; i < this.explosionPointCount; i++)
		{
			num2++;
			Vector3 vector = Random.onUnitSphere * this.explosionRadius * this.spawnRadiusFactor;
			vector.y = Mathf.Abs(vector.y);
			this.explosionPoints[num2] = new ExplosionOrb(base.transform.position + vector, vector, Random.Range(this.minDelay, this.maxDelay), this.childSizeFactor, Random.Range(this.minSpeed, this.maxSpeed));
			int num3 = num2;
			for (int j = 0; j < this.subExplosionPointCount; j++)
			{
				num2++;
				Vector3 position = this.explosionPoints[num3].position;
				vector = Random.onUnitSphere * this.explosionRadius * this.explosionPoints[num3].size * this.spawnRadiusFactor;
				vector.y = Mathf.Abs(vector.y);
				this.explosionPoints[num2] = new ExplosionOrb(position + vector, vector, this.explosionPoints[num3].delay + Random.Range(this.minDelay, this.maxDelay), size, this.explosionPoints[num3].speed + Random.Range(this.minSpeed, this.maxSpeed));
			}
		}
	}

	// Token: 0x06001221 RID: 4641 RVA: 0x0005AFB8 File Offset: 0x000591B8
	public void OnDrawGizmosSelected()
	{
		foreach (ExplosionOrb explosionOrb in this.explosionPoints)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(explosionOrb.position, this.explosionRadius * explosionOrb.size);
		}
	}

	// Token: 0x06001224 RID: 4644 RVA: 0x0005B099 File Offset: 0x00059299
	[CompilerGenerated]
	private IEnumerator <Start>g__IExplode|12_0(ExplosionOrb item)
	{
		yield return new WaitForSeconds(item.delay / (this.speed * item.speed));
		GameObject gameObject = Object.Instantiate<GameObject>(this.explosionOrb, item.position, HelperFunctions.GetRandomRotationWithUp(item.direction));
		gameObject.GetComponentInChildren<Animator>().speed = this.speed * item.speed;
		gameObject.transform.localScale = Vector3.one * (item.size * this.baseScale);
		MeshRenderer componentInChildren = gameObject.GetComponentInChildren<MeshRenderer>();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		componentInChildren.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetFloat(ExplosionEffect.k_Random, Random.value);
		componentInChildren.SetPropertyBlock(materialPropertyBlock);
		yield break;
	}

	// Token: 0x04001020 RID: 4128
	private static readonly int k_Random = Shader.PropertyToID("_Random");

	// Token: 0x04001021 RID: 4129
	public float speed = 1f;

	// Token: 0x04001022 RID: 4130
	public GameObject explosionOrb;

	// Token: 0x04001023 RID: 4131
	public float baseScale = 1f;

	// Token: 0x04001024 RID: 4132
	public float explosionRadius = 5f;

	// Token: 0x04001025 RID: 4133
	public float spawnRadiusFactor = 0.8f;

	// Token: 0x04001026 RID: 4134
	public float childSizeFactor = 0.9f;

	// Token: 0x04001027 RID: 4135
	public float minDelay = 0.4f;

	// Token: 0x04001028 RID: 4136
	public float maxDelay = 0.5f;

	// Token: 0x04001029 RID: 4137
	public float minSpeed = 0.75f;

	// Token: 0x0400102A RID: 4138
	public float maxSpeed = 1.25f;

	// Token: 0x0400102B RID: 4139
	private ExplosionOrb[] explosionPoints;

	// Token: 0x0400102C RID: 4140
	public int explosionPointCount = 4;

	// Token: 0x0400102D RID: 4141
	public int subExplosionPointCount = 2;
}
