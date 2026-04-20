using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200033B RID: 827
public class ShakyIcicleIce : MonoBehaviour
{
	// Token: 0x060015E3 RID: 5603 RVA: 0x0006EBA4 File Offset: 0x0006CDA4
	private void Awake()
	{
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.rig = base.GetComponent<Rigidbody>();
		this.view = base.GetComponent<PhotonView>();
		this.fractureRoot.gameObject.SetActive(false);
		this.rig.useGravity = false;
		this.rig.isKinematic = true;
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x0006EBFE File Offset: 0x0006CDFE
	private void Start()
	{
	}

	// Token: 0x060015E5 RID: 5605 RVA: 0x0006EC00 File Offset: 0x0006CE00
	private void Update()
	{
	}

	// Token: 0x060015E6 RID: 5606 RVA: 0x0006EC04 File Offset: 0x0006CE04
	private void SetIgnoreColliders()
	{
		this.ignoreColliders = new HashSet<Collider>();
		HashSet<Collider> hashSet = (from c in Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation)
		where c != this.meshCollider
		select c).ToHashSet<Collider>();
		Vector3 vector = base.transform.TransformVector(this.innerCheck);
		Vector3 center = base.transform.position + -base.transform.up * vector.y;
		Debug.Log(string.Format("Count: {0}", hashSet.Count));
		foreach (Collider collider in hashSet)
		{
			Vector3 vector2;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector2, out num))
			{
				this.ignoreColliders.Add(collider);
			}
			else if ((from c in Physics.OverlapBox(center, vector, base.transform.rotation)
			where c != this.meshCollider
			select c).ToList<Collider>().Count > 0)
			{
				this.ignoreColliders.Add(collider);
			}
		}
	}

	// Token: 0x060015E7 RID: 5607 RVA: 0x0006ED9C File Offset: 0x0006CF9C
	private bool CheckInTheClear()
	{
		HashSet<Collider> hashSet = (from c in Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation)
		where c != this.meshCollider
		select c).ToHashSet<Collider>();
		if (hashSet.Count == 0 || !hashSet.Any((Collider c) => this.ignoreColliders.Contains(c)))
		{
			this.scaleOnChange = this.meshCollider.transform.lossyScale;
			this.positionOnChange = this.meshCollider.transform.position;
			this.rotationOnChange = this.meshCollider.transform.rotation;
			return true;
		}
		return false;
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x0006EE58 File Offset: 0x0006D058
	private void FixedUpdate()
	{
		if (!this.isFalling)
		{
			return;
		}
		if (this.isFractured)
		{
			return;
		}
		if (!this.once)
		{
			this.once = true;
		}
		if (!this.isInTheClear)
		{
			this.isInTheClear = this.CheckInTheClear();
			if (this.isInTheClear)
			{
				this.ignoreColliders.Clear();
				this.rig.excludeLayers = 0;
			}
		}
		Vector3 vector;
		Vector3 vector2;
		List<Collider> list;
		if (this.CheckBoundingBox(out vector, out vector2, out list))
		{
			this.isFractured = true;
			this.mesh.gameObject.SetActive(false);
			Object.Destroy(this.meshCollider);
			Object.Destroy(base.GetComponent<MeshRenderer>());
			this.fractureRoot.gameObject.SetActive(true);
			Object.Destroy(this.rig);
		}
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x0006EF18 File Offset: 0x0006D118
	private void OnCollisionEnter(Collision other)
	{
		if (this.isShaking || this.isFalling)
		{
			return;
		}
		Character componentInParent = other.gameObject.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		Debug.Log("Before Shake rock");
		this.view.RPC("ShakeRock", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x0006EF74 File Offset: 0x0006D174
	private void OnCollisionStay(Collision other)
	{
		if (!this.isShaking)
		{
			return;
		}
		Character componentInParent = other.gameObject.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		this.tickTime += Time.deltaTime;
		if ((double)this.tickTime > 0.1)
		{
			this.tickTime = 0f;
			GamefeelHandler.instance.AddPerlinShake(this.shakeAmount, 0.2f, 15f);
		}
	}

	// Token: 0x060015EB RID: 5611 RVA: 0x0006EFF4 File Offset: 0x0006D1F4
	private bool CheckInnerBox(out Vector3 halfExtends, out Vector3 innerCheckPosition)
	{
		halfExtends = base.transform.TransformVector(this.innerCheck);
		innerCheckPosition = base.transform.position + -base.transform.up * halfExtends.y;
		return (from c in (from c in Physics.OverlapBox(innerCheckPosition, halfExtends, base.transform.rotation)
		where c != this.meshCollider
		select c).ToList<Collider>()
		where !this.ignoreColliders.Contains(c)
		select c).ToList<Collider>().Count > 0;
	}

	// Token: 0x060015EC RID: 5612 RVA: 0x0006F09C File Offset: 0x0006D29C
	public bool CheckBoundingBox(out Vector3 halfExtends, out Vector3 position, out List<Collider> colliders)
	{
		halfExtends = this.meshCollider.bounds.extents;
		position = this.meshCollider.bounds.center;
		colliders = (from c in Physics.OverlapBox(position, halfExtends, base.transform.rotation)
		where c != this.meshCollider
		select c).ToList<Collider>();
		colliders = (from c in colliders
		where !this.ignoreColliders.Contains(c)
		select c).ToList<Collider>();
		return colliders.Count > 0;
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x0006F134 File Offset: 0x0006D334
	public bool ConvexMeshCollision(List<Collider> colliders)
	{
		foreach (Collider collider in colliders)
		{
			Vector3 vector;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector, out num))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x0006F1CC File Offset: 0x0006D3CC
	private void OnDrawGizmosSelected()
	{
		if (!this.drawGizmos || this.isFractured)
		{
			return;
		}
		this.meshCollider = base.GetComponent<MeshCollider>();
		if (this.isInTheClear)
		{
			Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.positionOnChange, this.rotationOnChange, this.scaleOnChange);
		}
		foreach (Collider collider in this.ignoreColliders)
		{
			Debug.DrawLine(base.transform.position, collider.bounds.center);
		}
		this.CheckInTheClear();
		Vector3 a;
		Vector3 center;
		Gizmos.color = (this.CheckInnerBox(out a, out center) ? Color.red : Color.green);
		Gizmos.DrawCube(center, a * 2f);
		Vector3 a2;
		Vector3 center2;
		List<Collider> colliders;
		Gizmos.color = (this.CheckBoundingBox(out a2, out center2, out colliders) ? Color.red : Color.green);
		Gizmos.DrawWireCube(center2, a2 * 2f);
		Gizmos.color = (this.ConvexMeshCollision(colliders) ? Color.red : Color.green);
		Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x0006F344 File Offset: 0x0006D544
	[PunRPC]
	private void ShakeRock()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		base.StartCoroutine(this.<ShakeRock>g__RockShake|36_0());
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x0006F364 File Offset: 0x0006D564
	private void Go()
	{
		this.isFalling = true;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x0006F45E File Offset: 0x0006D65E
	[CompilerGenerated]
	private IEnumerator <ShakeRock>g__RockShake|36_0()
	{
		Debug.Log("Start shaking");
		float duration = 0f;
		Debug.Log(string.Format("duration: {0}, fallTime: {1}", duration, this.fallTime));
		while (duration < this.fallTime)
		{
			Debug.Log(string.Format("duration: {0}, fallTime: {1}", duration, this.fallTime));
			Vector3 vector = Vector2.zero;
			vector.x += Perlin.Noise(Time.time * this.shakeScale, 0f, 0f) - 0.5f;
			vector.y += Perlin.Noise(0f, Time.time * this.shakeScale, 0f) - 0.5f;
			vector.z += Perlin.Noise(0f, 0f, Time.time * this.shakeScale) - 0.5f;
			vector *= this.amount * Time.deltaTime;
			duration += Time.deltaTime;
			Debug.Log(string.Format("offset: {0}", vector));
			this.mesh.localPosition = vector;
			yield return null;
		}
		Debug.Log("Done shaking");
		this.isShaking = false;
		this.mesh.localPosition = 0.ToVec();
		this.isFalling = true;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
		yield break;
	}

	// Token: 0x040013E4 RID: 5092
	public float fallTime = 5f;

	// Token: 0x040013E5 RID: 5093
	public float amount = 1f;

	// Token: 0x040013E6 RID: 5094
	public float shakeScale = 15f;

	// Token: 0x040013E7 RID: 5095
	public Transform mesh;

	// Token: 0x040013E8 RID: 5096
	public float shakeAmount = 10f;

	// Token: 0x040013E9 RID: 5097
	public bool drawGizmos;

	// Token: 0x040013EA RID: 5098
	public float pushOutForce = 10f;

	// Token: 0x040013EB RID: 5099
	private bool isFalling;

	// Token: 0x040013EC RID: 5100
	private bool isInTheClear;

	// Token: 0x040013ED RID: 5101
	private bool isShaking;

	// Token: 0x040013EE RID: 5102
	private MeshCollider meshCollider;

	// Token: 0x040013EF RID: 5103
	private Transform model;

	// Token: 0x040013F0 RID: 5104
	private bool once;

	// Token: 0x040013F1 RID: 5105
	private Vector3 positionOnChange;

	// Token: 0x040013F2 RID: 5106
	private Rigidbody rig;

	// Token: 0x040013F3 RID: 5107
	private Quaternion rotationOnChange;

	// Token: 0x040013F4 RID: 5108
	private Vector3 scaleOnChange;

	// Token: 0x040013F5 RID: 5109
	private float tickTime;

	// Token: 0x040013F6 RID: 5110
	private Vector3 velocity = Vector3.zero;

	// Token: 0x040013F7 RID: 5111
	private PhotonView view;

	// Token: 0x040013F8 RID: 5112
	public Vector3 innerCheck;

	// Token: 0x040013F9 RID: 5113
	private HashSet<Collider> ignoreColliders = new HashSet<Collider>();

	// Token: 0x040013FA RID: 5114
	public Transform fractureRoot;

	// Token: 0x040013FB RID: 5115
	private bool isFractured;
}
