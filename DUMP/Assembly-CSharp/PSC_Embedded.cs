using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000301 RID: 769
public class PSC_Embedded : PropSpawnerConstraint, IValidationConstraint
{
	// Token: 0x060014E0 RID: 5344 RVA: 0x000696D0 File Offset: 0x000678D0
	private bool IsEmbeddedInCollider(PropSpawner.SpawnData spawnData, Collider[] ignoredColliders)
	{
		bool queriesHitBackfaces = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		Vector3 pos = spawnData.pos;
		RaycastHit[] array = new RaycastHit[100];
		bool flag = false;
		List<Collider> list = new List<Collider>();
		foreach (Vector3 a in PSC_Embedded.LeftRight)
		{
			foreach (Vector3 b in PSC_Embedded.UpDown)
			{
				foreach (Vector3 b2 in PSC_Embedded.ForwardBack)
				{
					Vector3 from = a + b + b2;
					if (from.sqrMagnitude >= 0.1f)
					{
						list.Clear();
						Vector3 normalized = from.normalized;
						int num = Physics.RaycastNonAlloc(pos - this.Deadzone * normalized, normalized, array, this.RaycastDistance, HelperFunctions.GetMask(this.LayerType));
						for (int l = 0; l < num; l++)
						{
							if (!ignoredColliders.Contains(array[l].collider) && !list.Contains(array[l].collider))
							{
								if (Mathf.Abs(Vector3.Angle(from, array[l].normal)) < 90f && array[l].distance > 2f * this.Deadzone)
								{
									flag = true;
									break;
								}
								list.Add(array[l].collider);
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
		}
		Physics.queriesHitBackfaces = queriesHitBackfaces;
		return flag;
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x0006988E File Offset: 0x00067A8E
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		return this.DesiredResult == this.IsEmbeddedInCollider(spawnData, new Collider[]
		{
			spawnData.hit.collider
		});
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x000698B4 File Offset: 0x00067AB4
	public override bool Validate(GameObject spawnedProp, PropSpawner.SpawnData spawnData)
	{
		Collider[] ignoredColliders = spawnedProp.GetComponentsInChildren<Collider>().Append(spawnData.hit.collider).ToArray<Collider>();
		return this.DesiredResult == this.IsEmbeddedInCollider(spawnData, ignoredColliders);
	}

	// Token: 0x04001308 RID: 4872
	public HelperFunctions.LayerType LayerType = HelperFunctions.LayerType.TerrainMap;

	// Token: 0x04001309 RID: 4873
	[SerializeField]
	[Tooltip("Radius around spawn transform where it's ok to be embedded")]
	private float Deadzone = 0.01f;

	// Token: 0x0400130A RID: 4874
	[Tooltip("Should be at least as big as the largest object you think this prop might be embedded in")]
	public float RaycastDistance = 50f;

	// Token: 0x0400130B RID: 4875
	public bool DesiredResult;

	// Token: 0x0400130C RID: 4876
	private static readonly Vector3[] UpDown = new Vector3[]
	{
		Vector3.up,
		Vector3.zero,
		Vector3.down
	};

	// Token: 0x0400130D RID: 4877
	private static readonly Vector3[] ForwardBack = new Vector3[]
	{
		Vector3.forward,
		Vector3.zero,
		Vector3.back
	};

	// Token: 0x0400130E RID: 4878
	private static readonly Vector3[] LeftRight = new Vector3[]
	{
		Vector3.left,
		Vector3.zero,
		Vector3.right
	};
}
