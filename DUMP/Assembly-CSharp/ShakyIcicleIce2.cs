using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts;
using pworld.Scripts.Extensions;
using Sirenix.Utilities;
using UnityEngine;

// Token: 0x0200033C RID: 828
public class ShakyIcicleIce2 : MonoBehaviour
{
	// Token: 0x17000167 RID: 359
	// (get) Token: 0x060015FB RID: 5627 RVA: 0x0006F46D File Offset: 0x0006D66D
	private bool IsLocalPlayerClimbing
	{
		get
		{
			return Character.localCharacter.data.isClimbing && Character.localCharacter.data.climbHit.collider == this.meshCollider;
		}
	}

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x060015FC RID: 5628 RVA: 0x0006F4A1 File Offset: 0x0006D6A1
	private float DistanceToLocalPlayer
	{
		get
		{
			return Vector3.Distance(Character.localCharacter.Center, base.transform.position);
		}
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x0006F4C0 File Offset: 0x0006D6C0
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		this.photonView = base.GetComponent<PhotonView>();
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.startPeicesCount = this.fracturedRoot.transform.childCount;
		this.fracturedRoot.gameObject.SetActive(false);
		this.source.volume = 0f;
		this.source.Stop();
		if (Random.Range(0f, 1f) > this.fallChance)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060015FE RID: 5630 RVA: 0x0006F554 File Offset: 0x0006D754
	private void Start()
	{
		this.fracturedRoot.gameObject.SetActive(true);
		this.stuckies = this.GetStuckPieces();
		this.fracturedRoot.gameObject.SetActive(false);
		this.fullMesh.gameObject.SetActive(true);
		if (this.fallOnStart)
		{
			this.Fall_Rpc();
		}
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x0006F5B0 File Offset: 0x0006D7B0
	private void Update()
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (!this.isShaking && !this.isFalling)
		{
			if ((from p in (from p in PlayerHandler.GetAllPlayerCharacters()
			where p.data.isClimbing
			select p).ToList<Character>()
			where p.data.climbHit.collider == this.meshCollider
			select p).ToList<Character>().Count > 0)
			{
				this.photonView.RPC("ShakeRock_Rpc", RpcTarget.All, Array.Empty<object>());
			}
		}
		this.timeUntilShake -= Time.deltaTime;
		if (this.isShaking && this.IsLocalPlayerClimbing && this.timeUntilShake <= 0f)
		{
			GamefeelHandler.instance.AddPerlinShake(this.climbingScreenShake, 0.2f, 15f);
			Debug.Log("Clime shake");
			this.timeUntilShake = this.screenShakeTickTime;
		}
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x0006F69C File Offset: 0x0006D89C
	private void FixedUpdate()
	{
		if (this.rig == null)
		{
			return;
		}
		this.lastLinearVelocity = this.rig.linearVelocity;
		this.lastAngularVelocity = this.rig.angularVelocity;
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x0006F6CF File Offset: 0x0006D8CF
	public void OnDestroy()
	{
		Object.DestroyImmediate(this.stuckiesRoot);
		Object.DestroyImmediate(this.shardsRoot);
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x0006F6E8 File Offset: 0x0006D8E8
	private void OnCollisionEnter(Collision other)
	{
		if (!this.isFalling)
		{
			return;
		}
		if ((float)this.fracturedRoot.transform.childCount < (float)this.startPeicesCount * this.maxFracturePercent)
		{
			return;
		}
		bool flag = false;
		HashSet<Collider> hashSet = new HashSet<Collider>();
		foreach (ContactPoint contactPoint in other.contacts)
		{
			Collider[] range = Physics.OverlapSphere(contactPoint.point, this.contactExplosionRadius);
			hashSet.AddRange(range);
		}
		foreach (Collider collider in hashSet)
		{
			if (collider.transform.parent != this.fracturedRoot)
			{
				if (this.shards.Contains(collider.gameObject))
				{
					this.rig.linearVelocity = this.lastLinearVelocity * this.collisionDamp;
					this.rig.angularVelocity = this.lastAngularVelocity;
				}
			}
			else
			{
				flag = true;
				if (this.shardsRoot == null)
				{
					this.shardsRoot = new GameObject("ShardsRoot");
					this.shardsRoot.transform.position = collider.transform.position;
				}
				collider.gameObject.AddComponent<Rigidbody>().mass = this.fracturedMass;
				collider.transform.parent = this.shardsRoot.transform;
				this.shards.Add(collider.gameObject);
			}
		}
		if (flag)
		{
			this.rig.linearVelocity = this.lastLinearVelocity * this.collisionDamp;
			this.rig.angularVelocity = this.lastAngularVelocity;
		}
	}

	// Token: 0x06001603 RID: 5635 RVA: 0x0006F8B0 File Offset: 0x0006DAB0
	private void OnDrawGizmosSelected()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.rig = base.GetComponent<Rigidbody>();
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.startShakeDistance);
		if (this.isFalling)
		{
			return;
		}
		foreach (Collider collider in this.GetStuckPieces())
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireMesh(collider.GetComponent<MeshCollider>().sharedMesh, collider.transform.position, collider.transform.rotation);
		}
	}

	// Token: 0x06001604 RID: 5636 RVA: 0x0006F978 File Offset: 0x0006DB78
	[PunRPC]
	private void ShakeRock_Rpc()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		this.source.Play();
		this.source.volume = 0.7f;
		if (this.DistanceToLocalPlayer < this.startShakeDistance)
		{
			Debug.Log(string.Format("start shake {0}", this.startShakeAmount));
			GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount, 0.2f, 15f);
		}
		base.StartCoroutine(this.<ShakeRock_Rpc>g__RockShake|42_0());
	}

	// Token: 0x06001605 RID: 5637 RVA: 0x0006FA00 File Offset: 0x0006DC00
	[PunRPC]
	private void Fall_Rpc()
	{
		if (Character.localCharacter.data.isClimbing && Character.localCharacter.data.climbHit.collider == this.meshCollider)
		{
			Character.localCharacter.refs.climbing.StopClimbing();
		}
		this.popSound.Play(base.transform.position);
		if (this.DistanceToLocalPlayer < this.startShakeDistance)
		{
			Debug.Log(string.Format("fall shake {0}", this.startShakeAmount));
			GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount, 0.2f, 15f);
		}
		this.fracturedRoot.gameObject.SetActive(true);
		this.fullMesh.gameObject.SetActive(false);
		this.rig = base.gameObject.AddComponent<Rigidbody>();
		this.rig.mass = 1000f;
		this.rig.useGravity = true;
		this.rig.isKinematic = false;
		this.meshCollider.enabled = false;
		Object.DestroyImmediate(this.meshCollider);
		foreach (Collider collider in this.stuckies)
		{
			if (this.stuckiesRoot == null)
			{
				this.stuckiesRoot = new GameObject("StuckiesRoot");
				this.stuckiesRoot.transform.position = collider.transform.position;
			}
			collider.transform.parent = this.stuckiesRoot.transform;
			collider.enabled = true;
		}
		this.startPeicesCount = this.fracturedRoot.transform.childCount;
		Debug.Log("Falling");
		this.isFalling = true;
	}

	// Token: 0x06001606 RID: 5638 RVA: 0x0006FBDC File Offset: 0x0006DDDC
	private List<Collider> GetStuckPieces()
	{
		List<MeshCollider> piecsColliders = this.fracturedRoot.GetComponentsInChildren<MeshCollider>().ToList<MeshCollider>();
		List<Collider> list = (from c in (from c in (from c in Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, base.transform.rotation)
		where c != this.meshCollider
		select c).ToList<Collider>()
		where c.gameObject.IsInLayer(HelperFunctions.LayerType.TerrainMap.ToLayerMask())
		select c).ToList<Collider>()
		where !piecsColliders.Contains(c)
		select c).ToList<Collider>();
		HashSet<Collider> hashSet = new HashSet<Collider>();
		foreach (Collider collider in list)
		{
			foreach (MeshCollider meshCollider in piecsColliders)
			{
				Vector3 vector;
				float num;
				if (Physics.ComputePenetration(meshCollider, meshCollider.transform.position, meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector, out num))
				{
					hashSet.Add(meshCollider);
				}
			}
		}
		HashSet<Collider> hashSet2 = new HashSet<Collider>();
		foreach (MeshCollider meshCollider2 in piecsColliders)
		{
			using (HashSet<Collider>.Enumerator enumerator3 = hashSet.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					if (enumerator3.Current.transform.position.y < meshCollider2.transform.position.y)
					{
						hashSet2.Add(meshCollider2);
					}
				}
			}
		}
		hashSet.AddRange(hashSet2);
		return hashSet.ToList<Collider>();
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x0006FED3 File Offset: 0x0006E0D3
	[CompilerGenerated]
	private IEnumerator <ShakeRock_Rpc>g__RockShake|42_0()
	{
		Debug.Log("Start shaking");
		float duration = 0f;
		while (duration < this.fallTime)
		{
			Vector3 vector = Vector2.zero;
			vector.x += Perlin.Noise(Time.time * this.shakeScale, 0f, 0f) - 0.5f;
			vector.y += Perlin.Noise(0f, Time.time * this.shakeScale, 0f) - 0.5f;
			vector.z += Perlin.Noise(0f, 0f, Time.time * this.shakeScale) - 0.5f;
			vector *= this.amount * Time.deltaTime;
			duration += Time.deltaTime;
			this.fullMesh.localPosition = vector;
			yield return null;
		}
		Debug.Log("Done shaking");
		this.isShaking = false;
		this.fullMesh.localPosition = 0.ToVec();
		this.source.volume = 0f;
		this.source.Stop();
		if (this.photonView.IsMine)
		{
			this.photonView.RPC("Fall_Rpc", RpcTarget.All, Array.Empty<object>());
		}
		yield break;
	}

	// Token: 0x040013FC RID: 5116
	public float fallChance = 0.5f;

	// Token: 0x040013FD RID: 5117
	public float contactExplosionRadius = 0.2f;

	// Token: 0x040013FE RID: 5118
	public float maxFracturePercent = 0.5f;

	// Token: 0x040013FF RID: 5119
	public float fracturedMass = 1f;

	// Token: 0x04001400 RID: 5120
	public float collisionDamp;

	// Token: 0x04001401 RID: 5121
	public float shakeScale = 30f;

	// Token: 0x04001402 RID: 5122
	public float fallTime = 5f;

	// Token: 0x04001403 RID: 5123
	public float amount = 1f;

	// Token: 0x04001404 RID: 5124
	public float startShakeDistance = 10f;

	// Token: 0x04001405 RID: 5125
	public float startShakeAmount = 400f;

	// Token: 0x04001406 RID: 5126
	public float climbingScreenShake = 240f;

	// Token: 0x04001407 RID: 5127
	public float screenShakeTickTime = 0.2f;

	// Token: 0x04001408 RID: 5128
	public bool isFalling;

	// Token: 0x04001409 RID: 5129
	public bool isShaking;

	// Token: 0x0400140A RID: 5130
	public bool fallOnStart;

	// Token: 0x0400140B RID: 5131
	public Transform fullMesh;

	// Token: 0x0400140C RID: 5132
	public Transform fracturedRoot;

	// Token: 0x0400140D RID: 5133
	public SFX_Instance popSound;

	// Token: 0x0400140E RID: 5134
	private readonly List<GameObject> shards = new List<GameObject>();

	// Token: 0x0400140F RID: 5135
	private Vector3 lastAngularVelocity;

	// Token: 0x04001410 RID: 5136
	private Vector3 lastLinearVelocity;

	// Token: 0x04001411 RID: 5137
	private MeshCollider meshCollider;

	// Token: 0x04001412 RID: 5138
	private PhotonView photonView;

	// Token: 0x04001413 RID: 5139
	private Rigidbody rig;

	// Token: 0x04001414 RID: 5140
	private GameObject shardsRoot;

	// Token: 0x04001415 RID: 5141
	private AudioSource source;

	// Token: 0x04001416 RID: 5142
	private int startPeicesCount;

	// Token: 0x04001417 RID: 5143
	private List<Collider> stuckies = new List<Collider>();

	// Token: 0x04001418 RID: 5144
	private GameObject stuckiesRoot;

	// Token: 0x04001419 RID: 5145
	private float timeUntilShake;

	// Token: 0x0400141A RID: 5146
	public bool drawGizmos;
}
