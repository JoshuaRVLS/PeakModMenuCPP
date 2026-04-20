using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000215 RID: 533
public class BeachSpawner : MonoBehaviour
{
	// Token: 0x0600105E RID: 4190 RVA: 0x00051554 File Offset: 0x0004F754
	private void Spawn()
	{
		this.Clear();
		int num = Random.Range(this.treeSpawnRange.x, this.treeSpawnRange.y);
		int num2 = 20;
		for (int i = 0; i < num; i++)
		{
			if (!this.TrySpawn(this.palmTrees[Random.Range(0, this.palmTrees.Length)]) && num2 > 0)
			{
				num2--;
				i--;
			}
		}
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x000515C0 File Offset: 0x0004F7C0
	private void Clear()
	{
		foreach (GameObject obj in this.spawned)
		{
			Object.DestroyImmediate(obj);
		}
		this.spawned.Clear();
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x0005161C File Offset: 0x0004F81C
	private bool TrySpawn(GameObject go)
	{
		float f = Random.Range(0f, 360f);
		float d = Random.Range(0f, this.radius);
		Vector3 a = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)) * d + this.treeParent.position;
		RaycastHit raycastHit;
		if (Physics.Linecast(a + Vector3.up * 100f, a - Vector3.up * 100f, out raycastHit, this.layerMask.value, QueryTriggerInteraction.UseGlobal))
		{
			Debug.Log(raycastHit.collider.gameObject.name, raycastHit.collider.gameObject);
			if (raycastHit.collider.gameObject.CompareTag("Sand"))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(go, raycastHit.point, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				gameObject.transform.SetParent(this.treeParent);
				this.spawned.Add(gameObject);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x00051742 File Offset: 0x0004F942
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.73f, 0.57f, 0f);
		Gizmos.DrawWireSphere(this.treeParent.position, this.radius);
	}

	// Token: 0x04000E4F RID: 3663
	public GameObject[] palmTrees;

	// Token: 0x04000E50 RID: 3664
	public float radius;

	// Token: 0x04000E51 RID: 3665
	public Vector2Int treeSpawnRange;

	// Token: 0x04000E52 RID: 3666
	public List<GameObject> spawned;

	// Token: 0x04000E53 RID: 3667
	public Transform treeParent;

	// Token: 0x04000E54 RID: 3668
	public LayerMask layerMask;
}
