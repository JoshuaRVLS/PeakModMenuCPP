using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class MapGenerationStage : MonoBehaviour
{
	// Token: 0x17000085 RID: 133
	// (get) Token: 0x060006A7 RID: 1703 RVA: 0x000260D5 File Offset: 0x000242D5
	private bool singleObject
	{
		get
		{
			return this.spawnMode == MapGenerationStage.SpawnMode.SingleObject;
		}
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x000260E0 File Offset: 0x000242E0
	private void OnDrawGizmosSelected()
	{
		if (this.useMinimumHeightLimit)
		{
			Gizmos.color = new Color(1f, 0.21f, 0f, 0.49f);
			Gizmos.DrawCube(base.transform.position + new Vector3(0f, this.minimumHeightLimit, 0f), new Vector3(1000f, 0.01f, 1000f));
		}
		if (this.useMaximumHeightLimit)
		{
			Gizmos.color = new Color(0f, 1f, 0.96f, 0.49f);
			Gizmos.DrawCube(base.transform.position + new Vector3(0f, this.maximumHeightLimit, 0f), new Vector3(1000f, 0.01f, 1000f));
		}
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x000261B5 File Offset: 0x000243B5
	public void Generate(int seed = 0)
	{
		this.ClearSpawnedObjects();
		this.GenerateNodeMap();
		this.RunProximityPasses();
		this.SpawnObjectsFromNodeMap();
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x000261D0 File Offset: 0x000243D0
	public void ClearSpawnedObjects()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
		this.spawnedObjects.Clear();
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00026218 File Offset: 0x00024418
	private void GenerateNodeMap()
	{
		if (this.nodeSpacing == 0f)
		{
			Debug.LogError("NODE SPACING IS ZERO! THIS WOULD RESULT IN INFINITE SPAWNING!");
			return;
		}
		Vector2 vector = new Vector2(this.spawnRange.bounds.min.x, this.spawnRange.bounds.min.z);
		Vector2 vector2 = new Vector2(this.spawnRange.bounds.max.x, this.spawnRange.bounds.max.z);
		Vector2 vector3 = new Vector2(vector.x, vector.y);
		this.nodeMap.Clear();
		while (vector3.y <= vector2.y)
		{
			List<MapGenerationStage.GenerationNode> list = new List<MapGenerationStage.GenerationNode>();
			this.nodeMap.Add(list);
			while (vector3.x <= vector2.x)
			{
				Vector2 vector4 = new Vector2(vector3.x, vector3.y);
				if (this.randomizedNodeOffset > 0f)
				{
					vector4 += new Vector2(Random.Range(-this.randomizedNodeOffset, this.randomizedNodeOffset), Random.Range(-this.randomizedNodeOffset, this.randomizedNodeOffset));
				}
				list.Add(new MapGenerationStage.GenerationNode(new Vector2(vector4.x, vector4.y), this.defaultDensity));
				vector3.x += this.nodeSpacing;
			}
			vector3.x = vector.x;
			vector3.y += this.nodeSpacing;
		}
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x000263AF File Offset: 0x000245AF
	private void SpawnObjectsFromNodeMap()
	{
		this.RunActionOnAllNodes(new Action<MapGenerationStage.GenerationNode>(this.TrySpawnObject));
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x000263C4 File Offset: 0x000245C4
	private void SpawnObject(Vector3 spot, Vector3 normal)
	{
		GameObject gameObject;
		if (this.singleObject)
		{
			if (this.objectPrefab)
			{
				gameObject = Object.Instantiate<GameObject>(this.objectPrefab);
			}
			else
			{
				gameObject = new GameObject();
			}
		}
		else if (!this.singleObject && this.spawnList)
		{
			gameObject = Object.Instantiate<GameObject>(this.spawnList.GetSingleSpawn());
		}
		else
		{
			gameObject = new GameObject();
		}
		if (this.randomizeRotation)
		{
			if (this.randomizeRotationOnNormalPlane)
			{
				gameObject.transform.rotation = HelperFunctions.GetRandomRotationWithUp(normal);
			}
			else
			{
				gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, (float)Random.Range(0, 360), gameObject.transform.eulerAngles.z);
			}
		}
		if (this.heightVariation != Vector2.zero)
		{
			spot += Vector3.up * Random.Range(this.heightVariation.x, this.heightVariation.y);
		}
		if (this.scaleVariation != Vector2.zero)
		{
			float num = Random.Range(this.scaleVariation.x, this.scaleVariation.y);
			gameObject.transform.localScale += new Vector3(num, num, num);
		}
		gameObject.transform.position = spot;
		gameObject.transform.SetParent(base.transform, true);
		LazyGizmo lazyGizmo = gameObject.AddComponent<LazyGizmo>();
		lazyGizmo.onSelected = false;
		lazyGizmo.color = this.testGizmoColor;
		lazyGizmo.radius = this.testGizmoSize;
		this.spawnedObjects.Add(gameObject);
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x00026564 File Offset: 0x00024764
	private void TrySpawnObject(MapGenerationStage.GenerationNode node)
	{
		Vector3 point = new Vector3(node.position.x, base.transform.position.y, node.position.y);
		Vector3 vector = Vector3.up;
		if ((this.raycastDownward || this.allowedTags.Count > 0) && Physics.Raycast(point + Vector3.up * 50f, Vector3.down, out this.hit, 100f))
		{
			if (this.useMinimumHeightLimit && this.hit.point.y < base.transform.position.y + this.minimumHeightLimit)
			{
				node.valid = false;
				return;
			}
			if (this.useMaximumHeightLimit && this.hit.point.y > base.transform.position.y + this.maximumHeightLimit)
			{
				node.valid = false;
				return;
			}
			if (this.allowedTags.Count > 0 && !this.allowedTags.Contains(this.hit.collider.gameObject.tag))
			{
				node.valid = false;
				return;
			}
			if (this.raycastDownward)
			{
				point = this.hit.point;
				vector = this.hit.normal;
				Debug.DrawLine(point, point + vector * 10f, Color.red, 10f);
			}
		}
		if (!node.valid)
		{
			return;
		}
		if (Random.Range(0f, 1f) < node.probability)
		{
			this.SpawnObject(point, vector);
		}
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x000266FF File Offset: 0x000248FF
	private void RunProximityPasses()
	{
		this.RunActionOnAllNodes(new Action<MapGenerationStage.GenerationNode>(this.RunProximityPassesOnNode));
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x00026714 File Offset: 0x00024914
	private void RunProximityPassesOnNode(MapGenerationStage.GenerationNode node)
	{
		this.RunPositionGradientPass(node);
		for (int i = 0; i < this.proximityPassData.Count; i++)
		{
			MapGenerationStage.GenerationProximityPassData generationProximityPassData = this.proximityPassData[i];
			List<GameObject> list = generationProximityPassData.previousStage.spawnedObjects;
			for (int j = 0; j < list.Count; j++)
			{
				float num = Vector3.Distance(node.position, Util.FlattenVector3(list[j].transform.position));
				if (num < generationProximityPassData.hardAvoidanceRadius * list[j].transform.localScale.x)
				{
					node.valid = false;
				}
				else if (num <= generationProximityPassData.minMaxProximity.y)
				{
					float num2 = Util.RangeLerp(generationProximityPassData.correlation, 0f, generationProximityPassData.minMaxProximity.x, generationProximityPassData.minMaxProximity.y, num, true, null);
					node.probability = Mathf.Clamp(node.probability + num2, this.minMaxDensity.x, this.minMaxDensity.y);
				}
			}
		}
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x00026830 File Offset: 0x00024A30
	private void RunPositionGradientPass(MapGenerationStage.GenerationNode node)
	{
		float time = (node.position.x - this.spawnRange.bounds.min.x) / (this.spawnRange.bounds.max.x - this.spawnRange.bounds.min.x);
		float time2 = (node.position.y - this.spawnRange.bounds.min.z) / (this.spawnRange.bounds.max.z - this.spawnRange.bounds.min.z);
		float num = 0f;
		float num2 = 0f;
		if (this.useCurveX)
		{
			num = this.curveX.Evaluate(time);
		}
		if (this.useCurveZ)
		{
			num2 = this.curveZ.Evaluate(time2);
		}
		node.probability = Mathf.Clamp(node.probability + num + num2, this.minMaxDensity.x, this.minMaxDensity.y);
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x00026954 File Offset: 0x00024B54
	private void RunActionOnAllNodes(Action<MapGenerationStage.GenerationNode> Action)
	{
		for (int i = 0; i < this.nodeMap.Count; i++)
		{
			List<MapGenerationStage.GenerationNode> list = this.nodeMap[i];
			for (int j = 0; j < list.Count; j++)
			{
				MapGenerationStage.GenerationNode obj = list[j];
				Action(obj);
			}
		}
	}

	// Token: 0x04000695 RID: 1685
	public BoxCollider spawnRange;

	// Token: 0x04000696 RID: 1686
	public float nodeSpacing = 1f;

	// Token: 0x04000697 RID: 1687
	[Range(0f, 1f)]
	public float defaultDensity;

	// Token: 0x04000698 RID: 1688
	public Vector2 minMaxDensity = new Vector2(0f, 1f);

	// Token: 0x04000699 RID: 1689
	public float randomizedNodeOffset;

	// Token: 0x0400069A RID: 1690
	public bool useCurveX;

	// Token: 0x0400069B RID: 1691
	public AnimationCurve curveX;

	// Token: 0x0400069C RID: 1692
	public bool useCurveZ;

	// Token: 0x0400069D RID: 1693
	public AnimationCurve curveZ;

	// Token: 0x0400069E RID: 1694
	public List<MapGenerationStage.GenerationProximityPassData> proximityPassData;

	// Token: 0x0400069F RID: 1695
	public bool useMinimumHeightLimit;

	// Token: 0x040006A0 RID: 1696
	public float minimumHeightLimit;

	// Token: 0x040006A1 RID: 1697
	public bool useMaximumHeightLimit;

	// Token: 0x040006A2 RID: 1698
	public float maximumHeightLimit;

	// Token: 0x040006A3 RID: 1699
	public MapGenerationStage.SpawnMode spawnMode;

	// Token: 0x040006A4 RID: 1700
	public GameObject objectPrefab;

	// Token: 0x040006A5 RID: 1701
	public SpawnList spawnList;

	// Token: 0x040006A6 RID: 1702
	public bool randomizeRotation = true;

	// Token: 0x040006A7 RID: 1703
	public bool randomizeRotationOnNormalPlane = true;

	// Token: 0x040006A8 RID: 1704
	public bool raycastDownward = true;

	// Token: 0x040006A9 RID: 1705
	public List<string> allowedTags;

	// Token: 0x040006AA RID: 1706
	public Vector2 heightVariation;

	// Token: 0x040006AB RID: 1707
	public Vector2 scaleVariation;

	// Token: 0x040006AC RID: 1708
	public Color testGizmoColor = Color.red;

	// Token: 0x040006AD RID: 1709
	public float testGizmoSize = 0.5f;

	// Token: 0x040006AE RID: 1710
	public List<List<MapGenerationStage.GenerationNode>> nodeMap = new List<List<MapGenerationStage.GenerationNode>>();

	// Token: 0x040006AF RID: 1711
	public List<GameObject> spawnedObjects;

	// Token: 0x040006B0 RID: 1712
	private RaycastHit hit;

	// Token: 0x0200044B RID: 1099
	public enum SpawnMode
	{
		// Token: 0x040018E0 RID: 6368
		SingleObject,
		// Token: 0x040018E1 RID: 6369
		SpawnList
	}

	// Token: 0x0200044C RID: 1100
	public class GenerationNode
	{
		// Token: 0x06001C05 RID: 7173 RVA: 0x00086DA8 File Offset: 0x00084FA8
		public GenerationNode(Vector2 pos, float defaultProbability)
		{
			this.position = pos;
			this.probability = defaultProbability;
			this.valid = true;
		}

		// Token: 0x040018E2 RID: 6370
		public Vector2 position;

		// Token: 0x040018E3 RID: 6371
		public float probability;

		// Token: 0x040018E4 RID: 6372
		public bool valid;
	}

	// Token: 0x0200044D RID: 1101
	[Serializable]
	public class GenerationProximityPassData
	{
		// Token: 0x040018E5 RID: 6373
		public MapGenerationStage previousStage;

		// Token: 0x040018E6 RID: 6374
		public float hardAvoidanceRadius;

		// Token: 0x040018E7 RID: 6375
		public Vector2 minMaxProximity;

		// Token: 0x040018E8 RID: 6376
		public float correlation;
	}
}
