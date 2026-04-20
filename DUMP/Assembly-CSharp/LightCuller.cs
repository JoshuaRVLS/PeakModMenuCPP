using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200029D RID: 669
public class LightCuller : MonoBehaviour
{
	// Token: 0x06001327 RID: 4903 RVA: 0x00060BE1 File Offset: 0x0005EDE1
	private void Start()
	{
		this.lightToCull = base.GetComponent<Light>();
		this.defaultRange = this.lightToCull.range;
	}

	// Token: 0x06001328 RID: 4904 RVA: 0x00060C00 File Offset: 0x0005EE00
	private void OnEnable()
	{
		base.StartCoroutine(this.LightCullRoutine());
	}

	// Token: 0x06001329 RID: 4905 RVA: 0x00060C0F File Offset: 0x0005EE0F
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.cullDistance);
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x00060C31 File Offset: 0x0005EE31
	private IEnumerator LightCullRoutine()
	{
		yield return new WaitForSeconds(Random.value);
		for (;;)
		{
			if (Character.localCharacter)
			{
				bool shouldEnable = Vector3.Distance(MainCamera.instance.transform.position, base.transform.position) < this.cullDistance;
				if (!this.lightToCull.enabled && shouldEnable)
				{
					this.lightToCull.enabled = true;
					float t = 0f;
					while (t < 1f)
					{
						t += Time.deltaTime;
						this.lightToCull.range = this.defaultRange * t;
						yield return null;
					}
				}
				if (this.lightToCull.enabled && !shouldEnable)
				{
					float t = 0f;
					while (t < 1f)
					{
						t += Time.deltaTime;
						this.lightToCull.range = this.defaultRange * (1f - t);
						yield return null;
					}
					this.lightToCull.enabled = false;
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x04001139 RID: 4409
	private Light lightToCull;

	// Token: 0x0400113A RID: 4410
	public float cullDistance = 50f;

	// Token: 0x0400113B RID: 4411
	public float defaultRange;
}
