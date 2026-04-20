using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200009A RID: 154
public class DecorSpawner : LevelGenStep
{
	// Token: 0x06000614 RID: 1556 RVA: 0x00022D20 File Offset: 0x00020F20
	public override void Execute()
	{
		this.Clear();
		this.Add();
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x00022D30 File Offset: 0x00020F30
	public void Add()
	{
		if (this.overallSpawnChance < 0.999f && Random.value > this.overallSpawnChance)
		{
			return;
		}
		int num = Random.Range(this.minMaxSpawn.x, this.minMaxSpawn.y);
		if (num > this.spawnPoints.Length)
		{
			num = this.spawnPoints.Length;
		}
		if (num < 0)
		{
			num = 0;
		}
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < this.spawnPoints.Length; i++)
		{
			list.Add(this.spawnPoints[i].position);
		}
		for (int j = 0; j < num; j++)
		{
			int index = Random.Range(0, list.Count);
			GameObject gameObject = HelperFunctions.SpawnPrefab(this.props[Random.Range(0, this.props.Length)], list[index], HelperFunctions.GetRandomRotationWithUp(Vector3.up), base.transform);
			gameObject.transform.localScale *= Random.RandomRange(this.scaleMinMax.x, this.scaleMinMax.y);
			list.RemoveAt(index);
			this.spawnedObjects.Add(gameObject);
		}
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x00022E58 File Offset: 0x00021058
	public override void Clear()
	{
		for (int i = 0; i < this.spawnedObjects.Count; i++)
		{
			Object.DestroyImmediate(this.spawnedObjects[i]);
		}
		this.spawnedObjects.Clear();
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x00022E98 File Offset: 0x00021098
	public void getSpawnSpots()
	{
		LazyGizmo[] componentsInChildren = base.GetComponentsInChildren<LazyGizmo>();
		this.spawnPoints = new Transform[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.spawnPoints[i] = componentsInChildren[i].transform;
		}
	}

	// Token: 0x0400062D RID: 1581
	public GameObject[] props;

	// Token: 0x0400062E RID: 1582
	[Range(0f, 1f)]
	public float overallSpawnChance;

	// Token: 0x0400062F RID: 1583
	public Vector2Int minMaxSpawn;

	// Token: 0x04000630 RID: 1584
	public Transform[] spawnPoints;

	// Token: 0x04000631 RID: 1585
	private List<GameObject> spawnedObjects = new List<GameObject>();

	// Token: 0x04000632 RID: 1586
	[Header("Spawn Customization")]
	public Vector2 scaleMinMax = Vector2.one;
}
