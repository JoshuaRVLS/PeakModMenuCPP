using System;
using UnityEngine;

// Token: 0x020002E0 RID: 736
public class PropDeleter : LevelGenStep
{
	// Token: 0x0600147A RID: 5242 RVA: 0x00067718 File Offset: 0x00065918
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x0600147B RID: 5243 RVA: 0x00067730 File Offset: 0x00065930
	public override void Execute()
	{
		foreach (Collider collider in Physics.OverlapSphere(base.transform.position, this.radius, HelperFunctions.GetMask(this.layerType)))
		{
			if (!(collider == null) && !(collider.gameObject == null))
			{
				int j = 0;
				Transform transform = collider.transform;
				while (j < 5)
				{
					j++;
					Transform parent = transform.parent;
					if (parent == null)
					{
						break;
					}
					PropGrouper componentInParent = transform.GetComponentInParent<PropGrouper>();
					if (!(componentInParent == null))
					{
						Transform transform2 = componentInParent.transform;
						bool flag = false;
						for (int k = 0; k < this.requiredParents.Length; k++)
						{
							if (transform2 == this.requiredParents[k])
							{
								flag = true;
							}
						}
						if (!flag && this.requiredParents.Length != 0)
						{
							break;
						}
						if (parent.GetComponent<PropSpawner>() || parent.GetComponent<PropSpawner_Line>())
						{
							Object.DestroyImmediate(transform.gameObject);
							break;
						}
						transform = parent;
					}
				}
			}
		}
	}

	// Token: 0x0600147C RID: 5244 RVA: 0x0006784F File Offset: 0x00065A4F
	public override void Clear()
	{
	}

	// Token: 0x040012A7 RID: 4775
	public HelperFunctions.LayerType layerType;

	// Token: 0x040012A8 RID: 4776
	public float radius = 10f;

	// Token: 0x040012A9 RID: 4777
	public Transform[] requiredParents;
}
