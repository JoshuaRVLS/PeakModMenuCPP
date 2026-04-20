using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000059 RID: 89
public class ArrowShooter : MonoBehaviourPunCallbacks
{
	// Token: 0x06000488 RID: 1160 RVA: 0x0001BAB9 File Offset: 0x00019CB9
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x0001BAC7 File Offset: 0x00019CC7
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.view.IsMine)
		{
			this.view.RPC("WarningArrows_RPC", RpcTarget.AllBuffered, new object[]
			{
				UnityEngine.Random.Range(1, 5)
			});
		}
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0001BB02 File Offset: 0x00019D02
	private void Start()
	{
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0001BB04 File Offset: 0x00019D04
	private void Update()
	{
		if (this.empty)
		{
			return;
		}
		if (!this.reloading)
		{
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.castRadius, this.shooter.forward, out raycastHit, this.range))
			{
				bool flag = false;
				if (raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
				{
					this.targetCharacter = raycastHit.collider.gameObject.GetComponentInParent<Character>();
					this.target = raycastHit.collider.transform;
					this.hitTarget = raycastHit.point;
					flag = true;
				}
				if (!flag)
				{
					this.target = raycastHit.collider.transform;
					this.hitTarget = raycastHit.point;
				}
			}
			if (this.target != null)
			{
				this.moveAcumulator += Vector3.Distance(this.target.position, this.targetLastPosition);
				if (this.moveAcumulator > 0f)
				{
					this.moveAcumulator -= this.movementCooldown * Time.deltaTime;
				}
				this.targetLastPosition = this.target.position;
			}
			else
			{
				this.moveAcumulator = 0f;
			}
			if (this.moveAcumulator > this.movementThreshold)
			{
				this.view.RPC("FireArrow_RPC", RpcTarget.AllBuffered, Array.Empty<object>());
			}
		}
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x0001BC64 File Offset: 0x00019E64
	public void testFire()
	{
		if (this.view.IsMine)
		{
			this.view.RPC("FireArrow_RPC", RpcTarget.AllBuffered, new object[]
			{
				base.transform.position + base.transform.forward
			});
		}
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x0001BCB8 File Offset: 0x00019EB8
	[PunRPC]
	public void FireArrow_RPC()
	{
		this.firedParticles.Play();
		Vector3 vector = this.hitTarget - base.transform.position;
		Vector3 position = base.transform.position + vector * 0.5f;
		ParticleSystem particleSystem = Object.Instantiate<ParticleSystem>(this.trailParticles, position, Quaternion.identity);
		particleSystem.shape.radius = Vector3.Distance(this.hitTarget, base.transform.position) / 2f;
		particleSystem.transform.rotation = Quaternion.LookRotation(vector, base.transform.up);
		if (this.targetCharacter != null)
		{
			this.targetCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, (float)this.damagePips * 0.025f, false, true, true);
		}
		Arrow arrow = Object.Instantiate<Arrow>(this.arrowPrefab, this.hitTarget, Quaternion.identity);
		arrow.transform.rotation = quaternion.LookRotation(vector, Vector3.up);
		arrow.transform.parent = this.target;
		arrow.stuckArrow(true);
		Rigidbody rigidbody;
		if (this.target.gameObject.TryGetComponent<Rigidbody>(out rigidbody))
		{
			rigidbody.AddForce(vector.normalized * this.force, ForceMode.Impulse);
		}
		this.arrows.Add(arrow);
		this.checkMaxArrows();
		base.StartCoroutine(this.<FireArrow_RPC>g__Reload|28_0());
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x0001BE30 File Offset: 0x0001A030
	[PunRPC]
	public void WarningArrows_RPC(int count)
	{
		for (int i = 0; i < count; i++)
		{
			Vector3 b = base.transform.up * UnityEngine.Random.Range(-1f, 1f) + base.transform.right * UnityEngine.Random.Range(-1f, 1f);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + b, base.transform.forward, out raycastHit, this.range))
			{
				MonoBehaviour.print(raycastHit.collider.gameObject.name);
				Arrow arrow = Object.Instantiate<Arrow>(this.arrowPrefab, raycastHit.point, Quaternion.identity);
				arrow.stuckArrow(true);
				arrow.transform.rotation = quaternion.LookRotation(raycastHit.point - base.transform.position, Vector3.up);
				arrow.transform.Rotate(new Vector3((float)UnityEngine.Random.Range(-10, 10), (float)UnityEngine.Random.Range(-10, 10), (float)UnityEngine.Random.Range(-10, 10)));
				arrow.transform.parent = raycastHit.transform;
			}
		}
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x0001BF6F File Offset: 0x0001A16F
	public void checkMaxArrows()
	{
		if (this.arrows.Count >= this.maxArrows)
		{
			this.emptyParticles.Play();
			this.empty = true;
		}
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0001BF98 File Offset: 0x0001A198
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position + this.shooter.forward * this.range, this.castRadius);
		Gizmos.DrawLine(base.transform.position, base.transform.position + this.shooter.forward * this.range);
		Gizmos.DrawRay(base.transform.position, this.hitTarget - base.transform.position);
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0001C057 File Offset: 0x0001A257
	[CompilerGenerated]
	private IEnumerator <FireArrow_RPC>g__Reload|28_0()
	{
		this.target = null;
		this.moveAcumulator = 0f;
		this.targetCharacter = null;
		this.reloading = true;
		yield return new WaitForSeconds(this.reloadTime);
		this.reloading = false;
		yield break;
	}

	// Token: 0x040004FA RID: 1274
	[FormerlySerializedAs("damage")]
	public int damagePips;

	// Token: 0x040004FB RID: 1275
	public float force;

	// Token: 0x040004FC RID: 1276
	public float range;

	// Token: 0x040004FD RID: 1277
	public float castRadius;

	// Token: 0x040004FE RID: 1278
	public float movementThreshold;

	// Token: 0x040004FF RID: 1279
	public float movementCooldown;

	// Token: 0x04000500 RID: 1280
	public Arrow arrowPrefab;

	// Token: 0x04000501 RID: 1281
	public List<Arrow> arrows = new List<Arrow>();

	// Token: 0x04000502 RID: 1282
	public int maxArrows = 100;

	// Token: 0x04000503 RID: 1283
	private PhotonView view;

	// Token: 0x04000504 RID: 1284
	public float reloadTime;

	// Token: 0x04000505 RID: 1285
	private bool reloading;

	// Token: 0x04000506 RID: 1286
	public Transform shooter;

	// Token: 0x04000507 RID: 1287
	public Transform target;

	// Token: 0x04000508 RID: 1288
	private Vector3 hitTarget;

	// Token: 0x04000509 RID: 1289
	private Vector3 targetLastPosition;

	// Token: 0x0400050A RID: 1290
	private float moveAcumulator;

	// Token: 0x0400050B RID: 1291
	public Character targetCharacter;

	// Token: 0x0400050C RID: 1292
	public ParticleSystem trailParticles;

	// Token: 0x0400050D RID: 1293
	public ParticleSystem firedParticles;

	// Token: 0x0400050E RID: 1294
	public ParticleSystem emptyParticles;

	// Token: 0x0400050F RID: 1295
	public bool empty;

	// Token: 0x04000510 RID: 1296
	private bool initialized;
}
