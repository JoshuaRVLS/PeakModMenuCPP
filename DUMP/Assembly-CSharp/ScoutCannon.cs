using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class ScoutCannon : MonoBehaviour
{
	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000CBC RID: 3260 RVA: 0x000440BF File Offset: 0x000422BF
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x000440C4 File Offset: 0x000422C4
	private void FixedUpdate()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		float num = 1f;
		float num2 = num;
		Character character = null;
		this.characters.Clear();
		this.characters.AddRange(Character.AllCharacters);
		this.characters.AddRange(Character.AllBotCharacters);
		foreach (Character character2 in this.characters)
		{
			float num3 = Vector3.Distance(character2.Center, this.entry.position);
			if (num3 < num2)
			{
				num2 = num3;
				character = character2;
			}
			if (num3 < num)
			{
				if (character2 == this.target)
				{
					if (character2.data.sinceJump < 0.5f)
					{
						continue;
					}
					List<Bodypart> partList = character2.refs.ragdoll.partList;
					if (character2.isBot && character2.data.fallSeconds == 0f)
					{
						character2.Fall(2f, 0f);
					}
					using (List<Bodypart>.Enumerator enumerator2 = partList.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Bodypart bodypart = enumerator2.Current;
							BodypartType partType = bodypart.partType;
							if (partType != BodypartType.Hand_L && partType != BodypartType.Hand_R && partType != BodypartType.Elbow_L && partType != BodypartType.Elbow_R && partType != BodypartType.Arm_L && partType != BodypartType.Arm_R)
							{
								Vector3 vector = bodypart.transform.position - this.tube.position;
								vector = Vector3.Project(vector, this.tube.forward);
								Vector3 vector2 = this.tube.position + vector - bodypart.transform.position;
								vector2 = Vector3.ClampMagnitude(vector2, 1f);
								bodypart.Rig.AddForce(vector2 * this.pullForce, ForceMode.Acceleration);
								if (this.entry.InverseTransformPoint(bodypart.transform.position).z < 0f && HelperFunctions.LineCheck(this.entry.position, bodypart.transform.position, HelperFunctions.LayerType.Map, 0f, QueryTriggerInteraction.Ignore).transform)
								{
									vector2 = this.entry.forward;
									bodypart.Rig.AddForce(vector2 * this.pullForce, ForceMode.Acceleration);
								}
							}
						}
						continue;
					}
				}
				if (HelperFunctions.LineCheck(this.tube.position, character2.Center, HelperFunctions.LayerType.Map, 0f, QueryTriggerInteraction.Ignore).transform == null)
				{
					Vector3 a = this.tube.position - character2.Center;
					a.Normalize();
					character2.AddForce(-a * this.pushForce, 1f, 1f);
				}
			}
		}
		if (this.target != character)
		{
			if (character == null)
			{
				this.view.RPC("RPCA_SetTarget", RpcTarget.All, new object[]
				{
					-1
				});
				return;
			}
			this.view.RPC("RPCA_SetTarget", RpcTarget.All, new object[]
			{
				character.refs.view.ViewID
			});
		}
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x00044450 File Offset: 0x00042650
	[PunRPC]
	private void RPCA_SetTarget(int setTargetID)
	{
		this.targetID = setTargetID;
		if (this.targetID == -1)
		{
			this.target = null;
			return;
		}
		this.target = PhotonNetwork.GetPhotonView(this.targetID).GetComponent<Character>();
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x00044480 File Offset: 0x00042680
	private void Awake()
	{
		this.tube = base.transform.Find("Cannon");
		this.entry = this.tube.Find("Entry");
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000CC0 RID: 3264 RVA: 0x000444BA File Offset: 0x000426BA
	// (set) Token: 0x06000CC1 RID: 3265 RVA: 0x000444DB File Offset: 0x000426DB
	private MeshRenderer[] meshRenderers
	{
		get
		{
			if (this._mr == null)
			{
				this._mr = this.tube.GetComponentsInChildren<MeshRenderer>();
			}
			return this._mr;
		}
		set
		{
			this._mr = value;
		}
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x000444E4 File Offset: 0x000426E4
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x000444E6 File Offset: 0x000426E6
	public void Light()
	{
		this.view.RPC("RPCA_Light", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x000444FE File Offset: 0x000426FE
	[PunRPC]
	public void RPCA_Light()
	{
		base.StartCoroutine(this.<RPCA_Light>g__LightRoutine|30_0());
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x0004450D File Offset: 0x0004270D
	private void FireTargets()
	{
		this.LaunchPlayers();
		this.LaunchItems();
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x0004451C File Offset: 0x0004271C
	private void LaunchPlayers()
	{
		List<Character> list = new List<Character>();
		if (this.target)
		{
			list.Add(this.target);
		}
		foreach (Character character in Character.AllCharacters)
		{
			if (Vector3.Distance(character.Center, this.entry.position) <= 0.75f && !(character == this.target))
			{
				list.Add(character);
			}
		}
		foreach (Character character2 in Character.AllBotCharacters)
		{
			if (Vector3.Distance(character2.Center, this.entry.position) <= 0.75f && !(character2 == this.target))
			{
				list.Add(character2);
			}
		}
		foreach (Character character3 in list)
		{
			this.view.RPC("RPCA_LaunchTarget", RpcTarget.All, new object[]
			{
				character3.refs.view.ViewID
			});
		}
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x0004468C File Offset: 0x0004288C
	private void LaunchItems()
	{
		Collider[] array = Physics.OverlapSphere(this.tube.position, 1f, HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysical));
		List<Item> list = new List<Item>();
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Item componentInParent = array2[i].GetComponentInParent<Item>();
			if (componentInParent && componentInParent.itemState == ItemState.Ground && !HelperFunctions.LineCheck(this.tube.position, componentInParent.Center(), HelperFunctions.LayerType.Map, 0f, QueryTriggerInteraction.Ignore).transform && !list.Contains(componentInParent))
			{
				list.Add(componentInParent);
			}
		}
		foreach (Item item in list)
		{
			this.view.RPC("RPCA_LaunchItem", RpcTarget.All, new object[]
			{
				item.photonView.ViewID
			});
		}
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x0004478C File Offset: 0x0004298C
	[PunRPC]
	public void RPCA_LaunchItem(int targetID)
	{
		PhotonView photonView = PhotonNetwork.GetPhotonView(targetID);
		if (photonView == null)
		{
			return;
		}
		Item component = photonView.GetComponent<Item>();
		if (component == null)
		{
			return;
		}
		if (component is Backpack)
		{
			component.rig.AddForce(this.tube.forward * this.backpackLaunchForce, ForceMode.VelocityChange);
			return;
		}
		component.rig.AddForce(this.tube.forward * this.itemLaunchForce, ForceMode.VelocityChange);
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00044808 File Offset: 0x00042A08
	[PunRPC]
	public void RPCA_LaunchTarget(int targetID)
	{
		ScoutCannon.<>c__DisplayClass35_0 CS$<>8__locals1 = new ScoutCannon.<>c__DisplayClass35_0();
		CS$<>8__locals1.<>4__this = this;
		PhotonView photonView = PhotonNetwork.GetPhotonView(targetID);
		if (photonView == null)
		{
			return;
		}
		CS$<>8__locals1.t = photonView.GetComponent<Character>();
		if (CS$<>8__locals1.t == null)
		{
			return;
		}
		CS$<>8__locals1.t.data.launchedByCannon = true;
		CS$<>8__locals1.t.RPCA_Fall(this.fallFor);
		CS$<>8__locals1.t.AddForce(this.tube.forward * this.launchForce, 1f, 1f);
		base.StartCoroutine(CS$<>8__locals1.<RPCA_LaunchTarget>g__ILaunch|0());
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x0004491A File Offset: 0x00042B1A
	[CompilerGenerated]
	private IEnumerator <RPCA_Light>g__LightRoutine|30_0()
	{
		this.lit = true;
		this.litParticle.Play();
		this.anim.Play("Light");
		yield return new WaitForSeconds(this.fireTime);
		this.anim.Play("Fire");
		this.fireParticle.Play();
		this.fireSFX.SetActive(true);
		if (this.view.IsMine)
		{
			this.FireTargets();
		}
		yield return new WaitForSeconds(this.fallFor);
		this.lit = false;
		yield break;
	}

	// Token: 0x04000B81 RID: 2945
	public float launchForce = 500f;

	// Token: 0x04000B82 RID: 2946
	public float itemLaunchForce = 500f;

	// Token: 0x04000B83 RID: 2947
	public float backpackLaunchForce = 5000f;

	// Token: 0x04000B84 RID: 2948
	public float fallFor = 1f;

	// Token: 0x04000B85 RID: 2949
	public float pullForce = 10f;

	// Token: 0x04000B86 RID: 2950
	public float pushForce = 10f;

	// Token: 0x04000B87 RID: 2951
	public bool lit;

	// Token: 0x04000B88 RID: 2952
	public float fireTime = 3f;

	// Token: 0x04000B89 RID: 2953
	public ParticleSystem litParticle;

	// Token: 0x04000B8A RID: 2954
	public ParticleSystem fireParticle;

	// Token: 0x04000B8B RID: 2955
	public GameObject fireSFX;

	// Token: 0x04000B8C RID: 2956
	public Animator anim;

	// Token: 0x04000B8D RID: 2957
	private MaterialPropertyBlock mpb;

	// Token: 0x04000B8E RID: 2958
	private PhotonView view;

	// Token: 0x04000B8F RID: 2959
	private Transform tube;

	// Token: 0x04000B90 RID: 2960
	private Transform entry;

	// Token: 0x04000B91 RID: 2961
	private Character target;

	// Token: 0x04000B92 RID: 2962
	private int targetID = -1;

	// Token: 0x04000B93 RID: 2963
	private List<Character> characters = new List<Character>();

	// Token: 0x04000B94 RID: 2964
	private MeshRenderer[] _mr;
}
