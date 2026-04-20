using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200006C RID: 108
public class Navigator : MonoBehaviour
{
	// Token: 0x06000516 RID: 1302 RVA: 0x0001E1F0 File Offset: 0x0001C3F0
	private void Awake()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.agent.updatePosition = false;
		this.agent.updateRotation = false;
		this.bot = base.GetComponentInParent<Bot>();
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0001E222 File Offset: 0x0001C422
	private void Start()
	{
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0001E224 File Offset: 0x0001C424
	public bool TryGetPointOnNavMeshCloseTo(Vector3 position, out NavMeshHit hit)
	{
		return NavMesh.SamplePosition(position, out hit, 2f, 1 << NavMesh.GetAreaFromName("Walkable"));
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x0001E244 File Offset: 0x0001C444
	private void Update()
	{
		this.agent.nextPosition = this.bot.Center;
		this.bot.navigationDirection_read = this.agent.desiredVelocity.normalized;
		if (this.agent.isOnNavMesh)
		{
			this.bot.remainingNavDistance = this.agent.remainingDistance;
		}
		if (this.lastReadTargetPosition == this.bot.targetPos_Set)
		{
			return;
		}
		if (this.agent.isOnNavMesh)
		{
			this.lastReadTargetPosition = this.bot.targetPos_Set;
			this.agent.SetDestination(this.lastReadTargetPosition);
		}
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x0001E2F1 File Offset: 0x0001C4F1
	public void SetAgentVelocity(Vector3 velocity)
	{
		this.agent.velocity = velocity;
	}

	// Token: 0x0400056D RID: 1389
	[HideInInspector]
	public NavMeshAgent agent;

	// Token: 0x0400056E RID: 1390
	private Bot bot;

	// Token: 0x0400056F RID: 1391
	private Vector3 lastReadTargetPosition;
}
