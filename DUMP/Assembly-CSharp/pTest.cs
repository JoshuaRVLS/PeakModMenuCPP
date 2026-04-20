using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000315 RID: 789
public class pTest : MonoBehaviour
{
	// Token: 0x06001527 RID: 5415 RVA: 0x0006A953 File Offset: 0x00068B53
	private void Awake()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.agent.updatePosition = false;
		this.agent.updateRotation = false;
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x0006A979 File Offset: 0x00068B79
	private void Start()
	{
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x0006A97B File Offset: 0x00068B7B
	private void Update()
	{
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x0006A980 File Offset: 0x00068B80
	private void OnDrawGizmosSelected()
	{
		BoxCollider boxCollider = base.GetComponent<BoxCollider>();
		Vector3 center = boxCollider.bounds.center;
		Collider[] array = (from c in Physics.OverlapBox(center, boxCollider.bounds.extents, boxCollider.transform.rotation)
		where c != boxCollider
		select c).ToArray<Collider>();
		Debug.Log(string.Format("position: {0}, extents: {1}", center, boxCollider.bounds.extents));
		foreach (Collider collider in array)
		{
			Debug.Log("Collider: " + collider.name);
		}
		Gizmos.color = ((array.Length != 0) ? Color.red : Color.green);
		Gizmos.DrawWireCube(center, boxCollider.bounds.extents * 2f);
	}

	// Token: 0x04001345 RID: 4933
	private NavMeshAgent agent;
}
