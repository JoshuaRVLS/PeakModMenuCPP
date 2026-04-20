using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class BreakableBridge : OnNetworkStart
{
	// Token: 0x1700012E RID: 302
	// (get) Token: 0x060010AC RID: 4268 RVA: 0x00052EB3 File Offset: 0x000510B3
	public bool LocalCharacterOnBridge
	{
		get
		{
			return Time.time - this.localTouchStamp < 0.2f;
		}
	}

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x060010AD RID: 4269 RVA: 0x00052EC8 File Offset: 0x000510C8
	private float DistanceToLocalPlayer
	{
		get
		{
			return Vector3.Distance(Character.localCharacter.Center, base.transform.position);
		}
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x00052EE4 File Offset: 0x000510E4
	private void Awake()
	{
		this.jungleVine = base.GetComponent<JungleVine>();
		this.photonView = base.GetComponent<PhotonView>();
		this.source = base.GetComponent<AudioSource>();
		foreach (CollisionModifier collisionModifier in base.GetComponentsInChildren<CollisionModifier>())
		{
			collisionModifier.applyEffects = false;
			collisionModifier.onCollide = (Action<Character, CollisionModifier, Collision, Bodypart>)Delegate.Combine(collisionModifier.onCollide, new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnBridgeCollision));
		}
		this.rend = base.GetComponentInChildren<Renderer>();
		this.rend.material.SetFloat(BreakableBridge.JitterAmount, 0f);
		this.rend.material.SetFloat(BreakableBridge.AlphaClip, 0.01f);
		if (this.holdsPeople == 0)
		{
			this.holdsPeople = 5;
		}
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x00052FA4 File Offset: 0x000511A4
	public override void NetworkStart()
	{
		this.holdsPeople = Random.Range(1, this.maxPeople);
		this.photonView.RPC("SyncHoldsPeopleRPC", RpcTarget.All, new object[]
		{
			this.holdsPeople
		});
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x00052FE0 File Offset: 0x000511E0
	private void Update()
	{
		if (this.isShaking)
		{
			this.source.pitch += 0.1f * Time.deltaTime;
			this.source.volume += 0.1f * Time.deltaTime;
			this.source.enabled = true;
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (this.isBreaking && !this.isShaking && !this.isFallen)
		{
			this.timeUntilBreak -= Time.deltaTime;
			if (this.timeUntilBreak < 0f)
			{
				this.photonView.RPC("ShakeBridge_Rpc", RpcTarget.All, Array.Empty<object>());
			}
		}
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x00053098 File Offset: 0x00051298
	private void FixedUpdate()
	{
		this.peopleOnBridge = 0;
		if (this.debug)
		{
			Debug.Log(string.Format("FixedUpdate: {0}, peopleOnBridge: {1}", Time.frameCount, this.peopleOnBridge));
		}
		if (this.peopleOnBridgeDict.Keys.Count > 0)
		{
			this.cachedPeopleOnBridgeList = this.peopleOnBridgeDict.Keys.ToList<Character>();
			foreach (Character character in this.cachedPeopleOnBridgeList)
			{
				Dictionary<Character, float> dictionary = this.peopleOnBridgeDict;
				Character key = character;
				dictionary[key] += Time.deltaTime;
				if (this.peopleOnBridgeDict[character] < 0.25f)
				{
					this.peopleOnBridge++;
				}
			}
		}
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x00053184 File Offset: 0x00051384
	private void OnDestroy()
	{
	}

	// Token: 0x060010B3 RID: 4275 RVA: 0x00053186 File Offset: 0x00051386
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient || newPlayer == PhotonNetwork.LocalPlayer)
		{
			return;
		}
		this.photonView.RPC("SyncHoldsPeopleRPC", newPlayer, new object[]
		{
			this.holdsPeople
		});
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x000531C4 File Offset: 0x000513C4
	[PunRPC]
	public void SyncHoldsPeopleRPC(int holdsPeople)
	{
		this.holdsPeople = holdsPeople;
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x000531D0 File Offset: 0x000513D0
	public void AddCollisionModifiers()
	{
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.AddComponent<CollisionModifier>();
		}
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x00053200 File Offset: 0x00051400
	private void OnBridgeCollision(Character character, CollisionModifier collider, Collision collision, Bodypart bodypart)
	{
		if (this.isBreaking)
		{
			return;
		}
		if (character == Character.localCharacter)
		{
			this.localTouchStamp = Time.time;
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (!this.peopleOnBridgeDict.TryAdd(character, 0f))
		{
			this.peopleOnBridgeDict[character] = 0f;
		}
		if (this.peopleOnBridge < this.holdsPeople)
		{
			return;
		}
		if (this.isShaking)
		{
			return;
		}
		if (this.holdsPeople >= this.peopleOnBridge)
		{
			return;
		}
		this.isBreaking = true;
		this.timeUntilBreak = Random.Range(2.5f, 7.5f);
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x000532A4 File Offset: 0x000514A4
	[PunRPC]
	private void ShakeBridge_Rpc()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		this.source.enabled = true;
		this.source.Play();
		if (!this.isShaking)
		{
			this.source.volume = 0.125f;
		}
		if (this.DistanceToLocalPlayer < this.startShakeDistance)
		{
			Debug.Log(string.Format("start shake {0}", this.startShakeAmount));
			GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount, 0.2f, 15f);
		}
		base.StartCoroutine(this.<ShakeBridge_Rpc>g__RockShake|44_0());
	}

	// Token: 0x060010B8 RID: 4280 RVA: 0x00053340 File Offset: 0x00051540
	[PunRPC]
	private void Fall_Rpc()
	{
		base.StartCoroutine(this.<Fall_Rpc>g__DestroyRoutine|45_0());
	}

	// Token: 0x060010BB RID: 4283 RVA: 0x0005342C File Offset: 0x0005162C
	[CompilerGenerated]
	private IEnumerator <ShakeBridge_Rpc>g__RockShake|44_0()
	{
		Debug.Log("Start shaking");
		float duration = 0f;
		float timeUntilShake = 0f;
		this.rend.material.SetFloat(BreakableBridge.JitterAmount, 1f);
		while (duration < this.fallTime)
		{
			timeUntilShake -= Time.deltaTime;
			if (this.LocalCharacterOnBridge && timeUntilShake <= 0f)
			{
				GamefeelHandler.instance.AddPerlinShake(this.climbingScreenShake, 0.2f, 15f);
				Debug.Log("Clime shake");
				timeUntilShake = this.screenShakeTickTime;
			}
			Vector3 a = Vector2.zero;
			a.x += Mathf.PerlinNoise1D(100f + duration * this.shakeScale) * this.axisMul.x;
			a.y += Mathf.PerlinNoise1D(10300f + duration * this.shakeScale) * this.axisMul.y;
			a.z += Mathf.PerlinNoise1D(1340f + duration * this.shakeScale) * this.axisMul.z;
			a *= this.amount;
			duration += Time.deltaTime;
			yield return null;
		}
		this.rend.material.SetFloat(BreakableBridge.JitterAmount, 0f);
		Debug.Log("Done shaking");
		if (this.isShaking)
		{
			for (int i = 0; i < this.breakSfx.Length; i++)
			{
				this.breakSfx[i].Play(base.transform.position);
			}
		}
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

	// Token: 0x060010BC RID: 4284 RVA: 0x0005343B File Offset: 0x0005163B
	[CompilerGenerated]
	private IEnumerator <Fall_Rpc>g__DestroyRoutine|45_0()
	{
		this.isFallen = true;
		Object.DestroyImmediate(this.jungleVine.colliderRoot.gameObject);
		if (this.breakParticles != null)
		{
			this.breakParticles.Play();
		}
		float normalizedTime = 0f;
		while (normalizedTime < 1f)
		{
			normalizedTime += Time.deltaTime * 0.7f;
			this.rend.material.SetFloat(BreakableBridge.BreakAmount, normalizedTime);
			yield return null;
		}
		Debug.Log(string.Format("Destroy: {0}", base.gameObject), base.gameObject);
		yield return null;
		yield break;
	}

	// Token: 0x04000E9E RID: 3742
	private static readonly int JitterAmount = Shader.PropertyToID("_JitterAmount");

	// Token: 0x04000E9F RID: 3743
	private static readonly int BreakAmount = Shader.PropertyToID("_BreakAmount");

	// Token: 0x04000EA0 RID: 3744
	private static readonly int AlphaClip = Shader.PropertyToID("_AlphaClip");

	// Token: 0x04000EA1 RID: 3745
	public int maxPeople = 5;

	// Token: 0x04000EA2 RID: 3746
	public SFX_Instance[] breakSfx;

	// Token: 0x04000EA3 RID: 3747
	[Range(0f, 1f)]
	public float breakPoint = 0.4f;

	// Token: 0x04000EA4 RID: 3748
	[Range(0f, 1f)]
	public float breakChance = 0.5f;

	// Token: 0x04000EA5 RID: 3749
	public Vector3 axisMul = new Vector3(1f, 1f, 1f);

	// Token: 0x04000EA6 RID: 3750
	public float shakeScale = 30f;

	// Token: 0x04000EA7 RID: 3751
	public float fallTime = 5f;

	// Token: 0x04000EA8 RID: 3752
	public float amount = 1f;

	// Token: 0x04000EA9 RID: 3753
	public float startShakeDistance = 10f;

	// Token: 0x04000EAA RID: 3754
	public float startShakeAmount = 400f;

	// Token: 0x04000EAB RID: 3755
	public float climbingScreenShake = 240f;

	// Token: 0x04000EAC RID: 3756
	public float screenShakeTickTime = 0.2f;

	// Token: 0x04000EAD RID: 3757
	public bool debug;

	// Token: 0x04000EAE RID: 3758
	public bool isShaking;

	// Token: 0x04000EAF RID: 3759
	public float localTouchStamp;

	// Token: 0x04000EB0 RID: 3760
	public int holdsPeople;

	// Token: 0x04000EB1 RID: 3761
	public int peopleOnBridge;

	// Token: 0x04000EB2 RID: 3762
	public Transform fullMesh;

	// Token: 0x04000EB3 RID: 3763
	public ParticleSystem breakParticles;

	// Token: 0x04000EB4 RID: 3764
	private readonly Dictionary<Character, float> peopleOnBridgeDict = new Dictionary<Character, float>();

	// Token: 0x04000EB5 RID: 3765
	private new PhotonView photonView;

	// Token: 0x04000EB6 RID: 3766
	private Renderer rend;

	// Token: 0x04000EB7 RID: 3767
	private AudioSource source;

	// Token: 0x04000EB8 RID: 3768
	private JungleVine jungleVine;

	// Token: 0x04000EB9 RID: 3769
	private List<Character> cachedPeopleOnBridgeList = new List<Character>();

	// Token: 0x04000EBA RID: 3770
	private float timeUntilBreak;

	// Token: 0x04000EBB RID: 3771
	private bool isBreaking;

	// Token: 0x04000EBC RID: 3772
	private bool isFallen;
}
