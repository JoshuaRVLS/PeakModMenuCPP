using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200033D RID: 829
public class ShakyRock : MonoBehaviour
{
	// Token: 0x0600160A RID: 5642 RVA: 0x0006FEE2 File Offset: 0x0006E0E2
	private void Awake()
	{
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.rig = base.GetComponent<Rigidbody>();
		this.view = base.GetComponent<PhotonView>();
		this.rig.useGravity = false;
		this.rig.isKinematic = true;
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x0006FF20 File Offset: 0x0006E120
	private void Start()
	{
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x0006FF22 File Offset: 0x0006E122
	private void Update()
	{
	}

	// Token: 0x0600160D RID: 5645 RVA: 0x0006FF24 File Offset: 0x0006E124
	private void FixedUpdate()
	{
		if (this.isFinished)
		{
			return;
		}
		if (!this.isFalling)
		{
			return;
		}
		if (!this.once)
		{
			this.rig.AddForce(Vector3.back * this.pushOutForce, ForceMode.VelocityChange);
			this.once = true;
		}
		if ((from c in Physics.OverlapSphere(this.meshCollider.bounds.center, this.meshCollider.bounds.extents.magnitude / 2f)
		where c != this.meshCollider
		select c).ToList<Collider>().Count > 0)
		{
			return;
		}
		List<Collider> list = (from c in Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation)
		where c != this.meshCollider
		select c).ToList<Collider>();
		Debug.Log(string.Format("Count: {0}", list.Count));
		foreach (Collider collider in list)
		{
			Vector3 vector;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector, out num))
			{
				Debug.Log("colliding with " + collider.name);
				return;
			}
			Debug.Log("Not colliding with " + collider.name);
		}
		this.scaleOnChange = this.meshCollider.transform.lossyScale;
		this.positionOnChange = this.meshCollider.transform.position;
		this.rotationOnChange = this.meshCollider.transform.rotation;
		this.isFinished = true;
		this.rig.excludeLayers = 0;
	}

	// Token: 0x0600160E RID: 5646 RVA: 0x00070144 File Offset: 0x0006E344
	private void OnCollisionEnter(Collision other)
	{
		if (this.isShaking || this.isFalling || this.isFinished)
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

	// Token: 0x0600160F RID: 5647 RVA: 0x000701A8 File Offset: 0x0006E3A8
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

	// Token: 0x06001610 RID: 5648 RVA: 0x00070228 File Offset: 0x0006E428
	private void OnDrawGizmosSelected()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		if (this.isFinished)
		{
			Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.positionOnChange, this.rotationOnChange, this.scaleOnChange);
		}
		this.meshCollider = base.GetComponent<MeshCollider>();
		List<Collider> list = (from c in Physics.OverlapSphere(this.meshCollider.bounds.center, this.meshCollider.bounds.extents.magnitude / 2f)
		where c != this.meshCollider
		select c).ToList<Collider>();
		Gizmos.color = ((list.Count > 0) ? Color.red : Color.green);
		Gizmos.DrawWireSphere(this.meshCollider.bounds.center, this.meshCollider.bounds.extents.magnitude / 2f);
		if (list.Count > 0)
		{
			return;
		}
		List<Collider> list2 = (from c in Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation)
		where c != this.meshCollider
		select c).ToList<Collider>();
		Gizmos.color = ((list2.Count > 0) ? Color.red : Color.green);
		Gizmos.DrawWireCube(this.meshCollider.bounds.center, this.meshCollider.bounds.size);
		foreach (Collider collider in list2)
		{
			Vector3 vector;
			float num;
			if (Physics.ComputePenetration(this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector, out num))
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
				return;
			}
			Debug.Log("Not colliding with " + collider.name);
		}
		Gizmos.color = Color.green;
		Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
	}

	// Token: 0x06001611 RID: 5649 RVA: 0x000704E4 File Offset: 0x0006E6E4
	private void Go2()
	{
		GamefeelHandler.instance.AddPerlinShake(this.shakeAmount, 0.2f, 15f);
	}

	// Token: 0x06001612 RID: 5650 RVA: 0x00070500 File Offset: 0x0006E700
	[PunRPC]
	private void ShakeRock()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		base.StartCoroutine(this.<ShakeRock>g__RockShake|28_0());
	}

	// Token: 0x06001613 RID: 5651 RVA: 0x00070520 File Offset: 0x0006E720
	private void Go()
	{
		this.isFalling = true;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x000705D1 File Offset: 0x0006E7D1
	[CompilerGenerated]
	private IEnumerator <ShakeRock>g__RockShake|28_0()
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
		this.rig.AddForce(Vector3.back * this.pushOutForce, ForceMode.VelocityChange);
		yield break;
	}

	// Token: 0x0400141B RID: 5147
	public float fallTime = 5f;

	// Token: 0x0400141C RID: 5148
	public float amount = 1f;

	// Token: 0x0400141D RID: 5149
	public float shakeScale = 15f;

	// Token: 0x0400141E RID: 5150
	public Transform mesh;

	// Token: 0x0400141F RID: 5151
	public float shakeAmount = 10f;

	// Token: 0x04001420 RID: 5152
	public bool drawGizmos;

	// Token: 0x04001421 RID: 5153
	public float pushOutForce = 10f;

	// Token: 0x04001422 RID: 5154
	private bool isFalling;

	// Token: 0x04001423 RID: 5155
	private bool isFinished;

	// Token: 0x04001424 RID: 5156
	private bool isShaking;

	// Token: 0x04001425 RID: 5157
	private MeshCollider meshCollider;

	// Token: 0x04001426 RID: 5158
	private Transform model;

	// Token: 0x04001427 RID: 5159
	private bool once;

	// Token: 0x04001428 RID: 5160
	private Vector3 positionOnChange;

	// Token: 0x04001429 RID: 5161
	private Rigidbody rig;

	// Token: 0x0400142A RID: 5162
	private Quaternion rotationOnChange;

	// Token: 0x0400142B RID: 5163
	private Vector3 scaleOnChange;

	// Token: 0x0400142C RID: 5164
	private float tickTime;

	// Token: 0x0400142D RID: 5165
	private Vector3 velocity = Vector3.zero;

	// Token: 0x0400142E RID: 5166
	private PhotonView view;
}
