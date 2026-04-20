using System;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class CharacterHeatEmission : MonoBehaviour
{
	// Token: 0x0600018C RID: 396 RVA: 0x0000B261 File Offset: 0x00009461
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
	}

	// Token: 0x0600018D RID: 397 RVA: 0x0000B270 File Offset: 0x00009470
	public void Update()
	{
		base.transform.position = this.character.refs.hip.transform.position;
		if (this.character.data.sinceAddedCold < 3f)
		{
			return;
		}
		this.counter += Time.deltaTime;
		if (this.counter < this.rate)
		{
			return;
		}
		this.counter = 0f;
		foreach (Character character in Character.AllCharacters)
		{
			if (Vector3.Distance(base.transform.position, character.Center) < this.radius)
			{
				character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, this.heatAmount, false, false);
			}
		}
	}

	// Token: 0x0600018E RID: 398 RVA: 0x0000B35C File Offset: 0x0000955C
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x04000151 RID: 337
	public float radius = 1f;

	// Token: 0x04000152 RID: 338
	public float heatAmount = 0.05f;

	// Token: 0x04000153 RID: 339
	public float rate = 0.5f;

	// Token: 0x04000154 RID: 340
	private float counter;

	// Token: 0x04000155 RID: 341
	private Character character;
}
