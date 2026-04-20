using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000067 RID: 103
public class Bot : MonoBehaviour
{
	// Token: 0x17000063 RID: 99
	// (get) Token: 0x060004EB RID: 1259 RVA: 0x0001D5CC File Offset: 0x0001B7CC
	// (set) Token: 0x060004EC RID: 1260 RVA: 0x0001D5D4 File Offset: 0x0001B7D4
	public Vector3 LookDirection
	{
		get
		{
			return this.lookDirection;
		}
		set
		{
			this.lookDirection = value;
		}
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x060004ED RID: 1261 RVA: 0x0001D5DD File Offset: 0x0001B7DD
	// (set) Token: 0x060004EE RID: 1262 RVA: 0x0001D5E5 File Offset: 0x0001B7E5
	public Vector2 MovementInput
	{
		get
		{
			return this.movementInput;
		}
		set
		{
			this.movementInput = value;
		}
	}

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x060004EF RID: 1263 RVA: 0x0001D5EE File Offset: 0x0001B7EE
	// (set) Token: 0x060004F0 RID: 1264 RVA: 0x0001D5F6 File Offset: 0x0001B7F6
	public bool IsSprinting
	{
		get
		{
			return this.isSprinting;
		}
		set
		{
			this.isSprinting = value;
		}
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x060004F1 RID: 1265 RVA: 0x0001D5FF File Offset: 0x0001B7FF
	public Vector3 Center
	{
		get
		{
			return this.centerTransform.position;
		}
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060004F2 RID: 1266 RVA: 0x0001D60C File Offset: 0x0001B80C
	// (set) Token: 0x060004F3 RID: 1267 RVA: 0x0001D614 File Offset: 0x0001B814
	[CanBeNull]
	public Character TargetCharacter
	{
		get
		{
			return this.targetCharacter;
		}
		set
		{
			this.targetCharacter = value;
		}
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x060004F4 RID: 1268 RVA: 0x0001D620 File Offset: 0x0001B820
	public Vector3? DistanceToTargetCharacter
	{
		get
		{
			if (!(this.TargetCharacter == null))
			{
				return null;
			}
			return new Vector3?(this.TargetCharacter.Center - this.Center);
		}
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060004F5 RID: 1269 RVA: 0x0001D660 File Offset: 0x0001B860
	public Vector3 HeadPosition
	{
		get
		{
			return this.Center + Vector3.up;
		}
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x0001D672 File Offset: 0x0001B872
	private void Awake()
	{
		this.navigator = base.GetComponentInChildren<Navigator>();
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x0001D680 File Offset: 0x0001B880
	private void Update()
	{
		this.timeSprinting = (this.IsSprinting ? (this.timeSprinting + Time.deltaTime) : 0f);
		this.timeSincePatrolEnded += Time.deltaTime;
		if (this.targetCharacter != null)
		{
			this.timeWithTarget += Time.deltaTime;
			this.timeWithoutTarget = 0f;
		}
		else
		{
			this.timeWithoutTarget += 0f;
			this.timeWithTarget = 0f;
		}
		if (this.timeSincePatrolEnded > 0.2f)
		{
			this.patrolHit = null;
		}
		Debug.DrawLine(this.Center, this.targetPos_Set, Color.cyan);
		Debug.DrawLine(this.Center, this.Center + this.navigationDirection_read, Color.blue);
		Debug.DrawLine(this.Center + Vector3.up, this.Center + Vector3.up + this.lookDirection, Color.yellow);
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x0001D78F File Offset: 0x0001B98F
	private void Start()
	{
		this.LookDirection = base.transform.forward;
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x0001D7A2 File Offset: 0x0001B9A2
	public void ClearTarget()
	{
		this.targetCharacter = null;
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x0001D7AC File Offset: 0x0001B9AC
	public bool CanSee(Vector3 from, Vector3 to, float maxDistance = 70f, float maxAngle = 110f)
	{
		return Vector3.Distance(from, to) <= maxDistance && Vector3.Angle(this.lookDirection, to - from) <= maxAngle && !HelperFunctions.LineCheck(from, to, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform;
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x0001D800 File Offset: 0x0001BA00
	public Rigidbody LookForPlayerHead(Vector3 searcherHeadPos, float maxRange = 70f, float maxAngle = 110f)
	{
		using (IEnumerator<Character> enumerator = (from character in Object.FindObjectsByType<Character>(FindObjectsSortMode.None)
		where !character.isBot
		select character).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Character character2 = enumerator.Current;
				if (character2 == null)
				{
					Debug.Log("No player found");
					return null;
				}
				if (Vector3.Distance(this.Center, character2.TorsoPos()) > maxRange)
				{
					return null;
				}
				if (Vector3.Angle(character2.TorsoPos() - this.Center, this.lookDirection) > maxAngle)
				{
					return null;
				}
				Bodypart bodypart = character2.GetBodypart(BodypartType.Head);
				Debug.DrawLine(searcherHeadPos, bodypart.Rig.position, Color.red);
				if (HelperFunctions.LineCheck(searcherHeadPos, bodypart.Rig.position, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform)
				{
					return null;
				}
				Debug.Log("Found player head", bodypart.Rig);
				return bodypart.Rig;
			}
		}
		return null;
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x0001D930 File Offset: 0x0001BB30
	public void Patrol()
	{
		this.timeSincePatrolEnded = 0f;
		NavMeshHit value;
		if ((this.patrolHit == null || this.remainingNavDistance < 1f) && this.navigator.TryGetPointOnNavMeshCloseTo(PatrolBoss.me.GetPoint(), out value))
		{
			this.patrolHit = new NavMeshHit?(value);
			this.targetPos_Set = this.patrolHit.Value.position;
		}
		this.MoveForward();
		this.LookInDirection(this.navigationDirection_read, 3f);
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x0001D9B7 File Offset: 0x0001BBB7
	public void RotateThenMove(Vector3 dir, float rotationSpeed = 3f)
	{
		if (HelperFunctions.FlatAngle(dir, this.lookDirection) < 5f)
		{
			this.MoveForward();
		}
		else
		{
			this.StandStill();
		}
		this.LookInDirection(dir, rotationSpeed);
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x0001D9E2 File Offset: 0x0001BBE2
	public void StandStill()
	{
		this.MovementInput = new Vector2(0f, 0f);
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0001D9F9 File Offset: 0x0001BBF9
	public void MoveForward()
	{
		this.MovementInput = new Vector2(0f, 1f);
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001DA10 File Offset: 0x0001BC10
	public void Chase()
	{
		if (this.TargetCharacter == null)
		{
			this.StandStill();
			Debug.Log("No target character");
			return;
		}
		this.targetPos_Set = this.TargetCharacter.Center;
		this.MoveForward();
		this.LookInDirection(this.navigationDirection_read, 3f);
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x0001DA64 File Offset: 0x0001BC64
	public void LookAtPoint(Vector3 point, float rotationSpeed = 3f)
	{
		this.LookInDirection((point - this.Center).normalized, rotationSpeed);
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x0001DA8C File Offset: 0x0001BC8C
	public void LookInDirection(Vector3 direction, float rotationSpeed = 3f)
	{
		this.LookDirection = Vector3.RotateTowards(this.LookDirection, direction, Time.deltaTime * rotationSpeed, 0f);
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x0001DAAC File Offset: 0x0001BCAC
	public bool CanSeeTarget(float maxDistance = 20f, float maxAngle = 120f)
	{
		if (this.TargetCharacter != null && this.CanSee(this.HeadPosition, this.TargetCharacter.Center, maxDistance, maxAngle))
		{
			this.timeSinceSawTarget = 0f;
			return true;
		}
		this.timeSinceSawTarget += Time.deltaTime;
		return false;
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x0001DAFC File Offset: 0x0001BCFC
	public void FleeFromPoint(Vector3 point)
	{
		if (this.fleePoint == null || this.remainingNavDistance < 2f)
		{
			Vector3 normalized = (this.Center - point).normalized;
			NavMeshHit value;
			if (this.navigator.TryGetPointOnNavMeshCloseTo(this.Center + normalized * 6f, out value))
			{
				this.fleePoint = new NavMeshHit?(value);
				this.targetPos_Set = this.fleePoint.Value.position;
			}
		}
		this.MoveForward();
		this.LookInDirection(this.navigationDirection_read, 3f);
	}

	// Token: 0x04000549 RID: 1353
	public Vector3 targetPos_Set;

	// Token: 0x0400054A RID: 1354
	public Vector3 navigationDirection_read;

	// Token: 0x0400054B RID: 1355
	public bool targetIsReachable;

	// Token: 0x0400054C RID: 1356
	public float remainingNavDistance;

	// Token: 0x0400054D RID: 1357
	public float timeSincePatrolEnded;

	// Token: 0x0400054E RID: 1358
	public float timeWithTarget;

	// Token: 0x0400054F RID: 1359
	public float timeWithoutTarget;

	// Token: 0x04000550 RID: 1360
	public Navigator navigator;

	// Token: 0x04000551 RID: 1361
	public Transform centerTransform;

	// Token: 0x04000552 RID: 1362
	public float timeSinceSawTarget;

	// Token: 0x04000553 RID: 1363
	public float timeSprinting;

	// Token: 0x04000554 RID: 1364
	private bool isSprinting;

	// Token: 0x04000555 RID: 1365
	private Vector3 lookDirection;

	// Token: 0x04000556 RID: 1366
	private Vector2 movementInput;

	// Token: 0x04000557 RID: 1367
	private Character targetCharacter;

	// Token: 0x04000558 RID: 1368
	private NavMeshHit? fleePoint;

	// Token: 0x04000559 RID: 1369
	private NavMeshHit? patrolHit = new NavMeshHit?(default(NavMeshHit));
}
