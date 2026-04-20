using System;
using UnityEngine;

// Token: 0x0200027A RID: 634
[ExecuteAlways]
public class HotSun : MonoBehaviour
{
	// Token: 0x06001286 RID: 4742 RVA: 0x0005CF61 File Offset: 0x0005B161
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x0005CF88 File Offset: 0x0005B188
	private void Start()
	{
	}

	// Token: 0x06001288 RID: 4744 RVA: 0x0005CF8C File Offset: 0x0005B18C
	private void Update()
	{
		this.bounds.center = base.transform.position;
		this.bounds.size = base.transform.localScale;
		if (!Application.isPlaying)
		{
			return;
		}
		if (Character.localCharacter == null)
		{
			return;
		}
		if (!this.bounds.Contains(Character.localCharacter.Center))
		{
			return;
		}
		if (DayNightManager.instance.sun.intensity < 5f)
		{
			return;
		}
		Transform transform = DayNightManager.instance.sun.transform;
		RaycastHit raycastHit = HelperFunctions.LineCheck(Character.localCharacter.Center + transform.forward * -1000f, Character.localCharacter.Center, HelperFunctions.LayerType.AllPhysical, 0f, QueryTriggerInteraction.Ignore);
		bool flag = false;
		if (raycastHit.transform == null || raycastHit.transform.root == Character.localCharacter.transform.root)
		{
			flag = true;
		}
		if (flag)
		{
			this.counter += Time.deltaTime;
			if (this.counter > this.rate)
			{
				this.counter = 0f;
				Character.localCharacter.refs.afflictions.AddSunHeat(this.amount);
			}
		}
	}

	// Token: 0x04001099 RID: 4249
	public Bounds bounds;

	// Token: 0x0400109A RID: 4250
	public float rate = 0.5f;

	// Token: 0x0400109B RID: 4251
	public float amount = 0.05f;

	// Token: 0x0400109C RID: 4252
	private float counter;
}
