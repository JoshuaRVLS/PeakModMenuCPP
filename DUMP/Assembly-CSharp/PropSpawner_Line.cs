using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000313 RID: 787
public class PropSpawner_Line : LevelGenStep
{
	// Token: 0x0600150D RID: 5389 RVA: 0x0006A154 File Offset: 0x00068354
	private void OnDrawGizmosSelected()
	{
		Vector3 to = base.transform.position + this.height * 0.5f * base.transform.up;
		Gizmos.DrawLine(base.transform.position - this.height * 0.5f * base.transform.up, to);
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x0006A1C0 File Offset: 0x000683C0
	public override void Execute()
	{
		this.Clear();
		this.Add();
	}

	// Token: 0x0600150F RID: 5391 RVA: 0x0006A1D0 File Offset: 0x000683D0
	public void Add()
	{
		int num = 50000;
		int num2 = 0;
		Physics.SyncTransforms();
		while (num2 < this.nrOfSpawns && num > 0)
		{
			num--;
			if (this.TryToSpawn())
			{
				num2++;
				if (this.syncTransforms)
				{
					Physics.SyncTransforms();
				}
			}
		}
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x0006A218 File Offset: 0x00068418
	public override void Clear()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06001511 RID: 5393 RVA: 0x0006A253 File Offset: 0x00068453
	public void GoAll()
	{
		base.GetComponentInParent<PropGrouper>().RunAll(true);
	}

	// Token: 0x06001512 RID: 5394 RVA: 0x0006A261 File Offset: 0x00068461
	public void ClearAll()
	{
		base.GetComponentInParent<PropGrouper>().ClearAll();
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x0006A270 File Offset: 0x00068470
	private bool TryToSpawn()
	{
		PropSpawner.SpawnData randomPoint = this.GetRandomPoint();
		if (randomPoint == null)
		{
			return false;
		}
		for (int i = 0; i < this.constraints.Count; i++)
		{
			if (!this.constraints[i].mute && !this.constraints[i].CheckConstraint(randomPoint))
			{
				return false;
			}
		}
		return this.Spawn(randomPoint) != null;
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x0006A2D8 File Offset: 0x000684D8
	private GameObject Spawn(PropSpawner.SpawnData spawnData)
	{
		GameObject gameObject = HelperFunctions.SpawnPrefab(this.props[Random.Range(0, this.props.Length)], spawnData.pos, HelperFunctions.GetRandomRotationWithUp(Vector3.up), base.transform);
		for (int i = 0; i < this.modifiers.Count; i++)
		{
			if (!this.modifiers[i].mute)
			{
				this.modifiers[i].ModifyObject(gameObject, spawnData);
			}
		}
		for (int j = 0; j < this.postConstraints.Count; j++)
		{
			if (!this.postConstraints[j].mute && !this.postConstraints[j].CheckConstraint(gameObject, spawnData))
			{
				Object.DestroyImmediate(gameObject);
				return null;
			}
		}
		return gameObject;
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x0006A39C File Offset: 0x0006859C
	private PropSpawner.SpawnData GetRandomPoint()
	{
		Vector3 vector = base.transform.position + base.transform.up * Mathf.Lerp(-0.5f, 0.5f, Random.value) * this.height;
		Vector3 normalized = Vector3.ProjectOnPlane(Random.onUnitSphere, base.transform.up).normalized;
		if (!this.rayCastSpawn)
		{
			return new PropSpawner.SpawnData
			{
				pos = vector,
				normal = normalized,
				rayDir = normalized,
				hit = default(RaycastHit),
				spawnerTransform = base.transform
			};
		}
		RaycastHit hit = HelperFunctions.LineCheck(vector, vector + normalized * this.rayLength, this.layerType, 0f, QueryTriggerInteraction.Ignore);
		if (hit.transform)
		{
			return new PropSpawner.SpawnData
			{
				pos = hit.point,
				normal = hit.normal,
				rayDir = normalized,
				hit = hit,
				spawnerTransform = base.transform
			};
		}
		return null;
	}

	// Token: 0x0400132C RID: 4908
	public float height = 200f;

	// Token: 0x0400132D RID: 4909
	public float rayLength = 5000f;

	// Token: 0x0400132E RID: 4910
	public int nrOfSpawns = 500;

	// Token: 0x0400132F RID: 4911
	public bool rayCastSpawn = true;

	// Token: 0x04001330 RID: 4912
	public GameObject[] props;

	// Token: 0x04001331 RID: 4913
	public bool syncTransforms = true;

	// Token: 0x04001332 RID: 4914
	public HelperFunctions.LayerType layerType = HelperFunctions.LayerType.TerrainMap;

	// Token: 0x04001333 RID: 4915
	[SerializeReference]
	public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();

	// Token: 0x04001334 RID: 4916
	[SerializeReference]
	public List<PropSpawnerConstraint> constraints = new List<PropSpawnerConstraint>();

	// Token: 0x04001335 RID: 4917
	[SerializeReference]
	public List<PropSpawnerConstraintPost> postConstraints = new List<PropSpawnerConstraintPost>();
}
