using System;
using UnityEngine;

// Token: 0x02000323 RID: 803
public class RockSpawner : MonoBehaviour
{
	// Token: 0x0600156E RID: 5486 RVA: 0x0006C630 File Offset: 0x0006A830
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawLine(base.transform.position - this.area.y * 0.5f * base.transform.forward, base.transform.position + this.area.y * 0.5f * base.transform.forward);
		Gizmos.DrawLine(base.transform.position - this.area.x * 0.5f * base.transform.right, base.transform.position + this.area.x * 0.5f * base.transform.right);
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x0006C70C File Offset: 0x0006A90C
	public void Go()
	{
		this.Clear();
		for (int i = 0; i < this.nrOfSpawns; i++)
		{
			this.DoSpawn();
		}
	}

	// Token: 0x06001570 RID: 5488 RVA: 0x0006C738 File Offset: 0x0006A938
	private void Clear()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x0006C774 File Offset: 0x0006A974
	private void DoSpawn()
	{
		RockSpawner.ReturnData? randomPoint = this.GetRandomPoint();
		if (randomPoint == null)
		{
			return;
		}
		GameObject gameObject = this.rocks[Random.Range(0, this.rocks.Length)];
		Quaternion a = gameObject.transform.rotation;
		if (this.rotation == RockSpawner.OriginalRotation.RaycastNormal)
		{
			a = HelperFunctions.GetRandomRotationWithUp(randomPoint.Value.normal);
		}
		a = Quaternion.Lerp(a, Random.rotation, Mathf.Pow(Random.value, this.rotationPow) * this.maxRotation);
		GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, randomPoint.Value.pos, a, base.transform);
		gameObject2.transform.position += base.transform.up * -this.downMove;
		gameObject2.transform.Rotate(base.transform.eulerAngles, Space.World);
		gameObject2.transform.localScale *= Random.Range(this.minScale, this.maxScale);
		Physics.SyncTransforms();
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x0006C878 File Offset: 0x0006AA78
	private RockSpawner.ReturnData? GetRandomPoint()
	{
		Vector3 vector = base.transform.position;
		vector += base.transform.right * Mathf.Lerp(-this.area.x * 0.5f, this.area.x * 0.5f, Random.value);
		vector += base.transform.forward * Mathf.Lerp(-this.area.y * 0.5f, this.area.y * 0.5f, Random.value);
		if (!this.raycast)
		{
			return new RockSpawner.ReturnData?(new RockSpawner.ReturnData
			{
				pos = vector,
				normal = Vector3.up
			});
		}
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + base.transform.up * -5000f, HelperFunctions.LayerType.Terrain, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			return new RockSpawner.ReturnData?(new RockSpawner.ReturnData
			{
				pos = raycastHit.point,
				normal = raycastHit.normal
			});
		}
		return null;
	}

	// Token: 0x0400138F RID: 5007
	public Vector2 area;

	// Token: 0x04001390 RID: 5008
	public GameObject[] rocks;

	// Token: 0x04001391 RID: 5009
	public int nrOfSpawns = 500;

	// Token: 0x04001392 RID: 5010
	public float downMove;

	// Token: 0x04001393 RID: 5011
	public RockSpawner.OriginalRotation rotation;

	// Token: 0x04001394 RID: 5012
	public bool raycast = true;

	// Token: 0x04001395 RID: 5013
	public float minScale = 1f;

	// Token: 0x04001396 RID: 5014
	public float maxScale = 2f;

	// Token: 0x04001397 RID: 5015
	public float maxRotation = 1f;

	// Token: 0x04001398 RID: 5016
	public float rotationPow;

	// Token: 0x02000533 RID: 1331
	public enum OriginalRotation
	{
		// Token: 0x04001C93 RID: 7315
		PrefabRotation,
		// Token: 0x04001C94 RID: 7316
		RaycastNormal
	}

	// Token: 0x02000534 RID: 1332
	private struct ReturnData
	{
		// Token: 0x04001C95 RID: 7317
		public Vector3 pos;

		// Token: 0x04001C96 RID: 7318
		public Vector3 normal;
	}
}
