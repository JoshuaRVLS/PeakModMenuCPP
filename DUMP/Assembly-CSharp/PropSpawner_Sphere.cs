using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000314 RID: 788
public class PropSpawner_Sphere : LevelGenStep
{
	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06001517 RID: 5399 RVA: 0x0006A51A File Offset: 0x0006871A
	public override DeferredStepTiming DeferredTiming
	{
		get
		{
			if (this._deferredSteps.Count <= 0)
			{
				return DeferredStepTiming.None;
			}
			return DeferredStepTiming.AfterCurrentGroupTiming;
		}
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x0006A52D File Offset: 0x0006872D
	public override IDeferredStep ConstructDeferred(IMayHaveDeferredStep parent)
	{
		if (this != parent)
		{
			Debug.LogError("What da HECK!!!");
			return null;
		}
		return new ExecuteDeferredStepList(this._deferredSteps);
	}

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06001519 RID: 5401 RVA: 0x0006A54F File Offset: 0x0006874F
	// (set) Token: 0x0600151A RID: 5402 RVA: 0x0006A557 File Offset: 0x00068757
	public List<GameObject> spawnedProps { get; private set; } = new List<GameObject>();

	// Token: 0x0600151B RID: 5403 RVA: 0x0006A560 File Offset: 0x00068760
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.rayLength);
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x0006A578 File Offset: 0x00068778
	public void Go()
	{
		this.Clear();
		this.SpawnNew(true);
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x0006A587 File Offset: 0x00068787
	public override void Execute()
	{
		this.Clear();
		this.SpawnNew(false);
	}

	// Token: 0x0600151E RID: 5406 RVA: 0x0006A596 File Offset: 0x00068796
	public void Add()
	{
		this.SpawnNew(true);
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x0006A5A0 File Offset: 0x000687A0
	public void SpawnNew(bool executeDeferredImmediately = false)
	{
		if (this.spawnChance < Random.value)
		{
			return;
		}
		int num = this.nrOfSpawns;
		if (this.randomSpawns)
		{
			num = Random.Range(this.minSpawnCount, this.nrOfSpawns);
		}
		int num2 = 50000;
		int num3 = 0;
		Physics.SyncTransforms();
		while (num3 < num && num2 > 0)
		{
			num2--;
			if (this.TryToSpawn())
			{
				num3++;
				if (this.syncTransforms)
				{
					Physics.SyncTransforms();
				}
			}
		}
		foreach (PostSpawnBehavior postSpawnBehavior in this.postSpawnBehaviors)
		{
			if (!postSpawnBehavior.mute)
			{
				if (executeDeferredImmediately || postSpawnBehavior.DeferredTiming != DeferredStepTiming.AfterCurrentGroupTiming)
				{
					postSpawnBehavior.RunBehavior(this.spawnedProps);
				}
				else
				{
					this._deferredSteps.Add(postSpawnBehavior.ConstructDeferred(this));
				}
			}
		}
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x0006A688 File Offset: 0x00068888
	public override void Clear()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x0006A6C3 File Offset: 0x000688C3
	public void GoAll()
	{
		base.GetComponentInParent<PropGrouper>().RunAll(true);
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x0006A6D1 File Offset: 0x000688D1
	public void ClearAll()
	{
		base.GetComponentInParent<PropGrouper>().ClearAll();
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x0006A6E0 File Offset: 0x000688E0
	private bool TryToSpawn()
	{
		PropSpawner.SpawnData randomPoint = this.GetRandomPoint();
		if (randomPoint == null)
		{
			return false;
		}
		for (int i = 0; i < this.constraints.Count; i++)
		{
			if (!this.constraints[i].CheckConstraint(randomPoint))
			{
				return false;
			}
		}
		GameObject gameObject = this.Spawn(randomPoint);
		if (gameObject != null)
		{
			this.spawnedProps.Add(gameObject);
		}
		return gameObject != null;
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x0006A744 File Offset: 0x00068944
	private GameObject Spawn(PropSpawner.SpawnData spawnData)
	{
		GameObject gameObject = HelperFunctions.SpawnPrefab(this.props[Random.Range(0, this.props.Length)], spawnData.pos, HelperFunctions.GetRandomRotationWithUp(Vector3.up), base.transform);
		if (gameObject == null)
		{
			Debug.LogError("Failed to spawn prefab", base.gameObject);
		}
		for (int i = 0; i < this.modifiers.Count; i++)
		{
			this.modifiers[i].ModifyObject(gameObject, spawnData);
		}
		for (int j = 0; j < this.postConstraints.Count; j++)
		{
			if (!this.postConstraints[j].CheckConstraint(gameObject, spawnData))
			{
				Object.DestroyImmediate(gameObject);
				return null;
			}
		}
		return gameObject;
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x0006A7F8 File Offset: 0x000689F8
	private PropSpawner.SpawnData GetRandomPoint()
	{
		Vector3 position = base.transform.position;
		Vector3 normalized = Random.onUnitSphere.normalized;
		if (!this.rayCastSpawn)
		{
			return new PropSpawner.SpawnData
			{
				pos = position,
				normal = normalized,
				rayDir = normalized,
				hit = default(RaycastHit),
				spawnerTransform = base.transform
			};
		}
		RaycastHit hit = HelperFunctions.LineCheck(position, position + normalized * this.rayLength, this.layerType, 0f, QueryTriggerInteraction.Ignore);
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

	// Token: 0x04001336 RID: 4918
	private List<IDeferredStep> _deferredSteps = new List<IDeferredStep>();

	// Token: 0x04001337 RID: 4919
	public float spawnChance = 1f;

	// Token: 0x04001338 RID: 4920
	public float rayLength = 5000f;

	// Token: 0x04001339 RID: 4921
	public int nrOfSpawns = 500;

	// Token: 0x0400133A RID: 4922
	public bool randomSpawns;

	// Token: 0x0400133B RID: 4923
	public int minSpawnCount;

	// Token: 0x0400133C RID: 4924
	public bool rayCastSpawn = true;

	// Token: 0x0400133D RID: 4925
	public GameObject[] props;

	// Token: 0x0400133F RID: 4927
	public bool syncTransforms = true;

	// Token: 0x04001340 RID: 4928
	public HelperFunctions.LayerType layerType = HelperFunctions.LayerType.TerrainMap;

	// Token: 0x04001341 RID: 4929
	[SerializeReference]
	public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();

	// Token: 0x04001342 RID: 4930
	[SerializeReference]
	public List<PropSpawnerConstraint> constraints = new List<PropSpawnerConstraint>();

	// Token: 0x04001343 RID: 4931
	[SerializeReference]
	public List<PropSpawnerConstraintPost> postConstraints = new List<PropSpawnerConstraintPost>();

	// Token: 0x04001344 RID: 4932
	[SerializeReference]
	public List<PostSpawnBehavior> postSpawnBehaviors = new List<PostSpawnBehavior>();
}
