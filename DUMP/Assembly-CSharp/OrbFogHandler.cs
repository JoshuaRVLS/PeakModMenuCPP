using System;
using System.Collections;
using ExitGames.Client.Photon;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002BF RID: 703
public class OrbFogHandler : Singleton<OrbFogHandler>, IInRoomCallbacks
{
	// Token: 0x17000143 RID: 323
	// (get) Token: 0x060013CB RID: 5067 RVA: 0x00064802 File Offset: 0x00062A02
	// (set) Token: 0x060013CC RID: 5068 RVA: 0x0006480A File Offset: 0x00062A0A
	public bool PlayersAreResting { get; set; }

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x060013CD RID: 5069 RVA: 0x00064813 File Offset: 0x00062A13
	// (set) Token: 0x060013CE RID: 5070 RVA: 0x0006481B File Offset: 0x00062A1B
	public int currentID { get; private set; }

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x060013CF RID: 5071 RVA: 0x00064824 File Offset: 0x00062A24
	public static bool IsFoggingCurrentSegment
	{
		get
		{
			return Singleton<OrbFogHandler>.Instance != null && (Singleton<OrbFogHandler>.Instance.isMoving || Singleton<OrbFogHandler>.Instance.hasArrived);
		}
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x0006484D File Offset: 0x00062A4D
	protected override void Awake()
	{
		base.Awake();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x00064861 File Offset: 0x00062A61
	private void Start()
	{
		this.sphere = base.GetComponentInChildren<FogSphere>();
		this.origins = base.transform.root.GetComponentsInChildren<FogSphereOrigin>();
		this.InitNewSphere(this.origins[this.currentID]);
	}

	// Token: 0x060013D2 RID: 5074 RVA: 0x00064898 File Offset: 0x00062A98
	private void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x000648A0 File Offset: 0x00062AA0
	private void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
		Shader.SetGlobalFloat("FakeMountainEnabled", 1f);
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x000648B8 File Offset: 0x00062AB8
	private void Update()
	{
		this.sphere != null;
		if (!this.hasArrived)
		{
			bool flag = this.currentID >= this.origins.Length || this.origins[this.currentID].disableFog;
			if (Ascents.fogEnabled && !flag)
			{
				if (this.isMoving)
				{
					this.Move();
				}
				else
				{
					this.WaitToMove();
				}
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.Sync();
		}
		this.ApplyMeshEffects();
		float b = Mathf.Lerp(1f, 5f, this.dispelFogAmount);
		this.currentCloseFog = Mathf.Lerp(this.currentCloseFog, b, Time.deltaTime * 1f);
		Shader.SetGlobalFloat("CloseDistanceMod", this.currentCloseFog);
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x0006497C File Offset: 0x00062B7C
	private void Sync()
	{
		this.syncCounter += Time.deltaTime;
		if (this.syncCounter > 5f)
		{
			this.syncCounter = 0f;
			this.photonView.RPC("RPCA_SyncFog", RpcTarget.Others, new object[]
			{
				this.currentSize,
				this.isMoving
			});
		}
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x000649E6 File Offset: 0x00062BE6
	[PunRPC]
	public void RPCA_SyncFog(float s, bool moving)
	{
		this.currentSize = s;
		this.isMoving = moving;
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x000649F6 File Offset: 0x00062BF6
	[PunRPC]
	public void RPC_InitFog(int originId, float s, bool arrived, bool moving, PhotonMessageInfo info)
	{
		if (info.Sender.ActorNumber != NetCode.Session.HostId)
		{
			return;
		}
		this.SetFogOrigin(originId);
		this.currentSize = s;
		this.hasArrived = arrived;
		this.isMoving = moving;
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x00064A2E File Offset: 0x00062C2E
	public IEnumerator WaitForFogCatchUp()
	{
		this.isMoving = true;
		while (this.currentSize > 30f && this.isMoving && !this.hasArrived)
		{
			this.currentSize = Mathf.Lerp(this.currentSize, 29.5f, Time.deltaTime);
			this.currentSize = Mathf.MoveTowards(this.currentSize, 29.5f, Time.deltaTime);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x00064A3D File Offset: 0x00062C3D
	public IEnumerator WaitForReveal()
	{
		float c = 0f;
		float t = 5f;
		this.sphere.ENABLE = 1f;
		while (c < t)
		{
			c += Time.deltaTime;
			this.sphere.REVEAL_AMOUNT = this.fogRevealCurve.Evaluate(c / t);
			this.sphere.ENABLE = this.fogFadeCurve.Evaluate(c / t);
			yield return null;
		}
		this.sphere.REVEAL_AMOUNT = 1f;
		this.sphere.ENABLE = 0f;
		this.currentSize = 800f;
		yield break;
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x00064A4C File Offset: 0x00062C4C
	public IEnumerator DisableFog()
	{
		float c = 0f;
		float t = 1f;
		while (c < t)
		{
			c += Time.deltaTime;
			this.sphere.ENABLE = 1f - c / t;
			yield return null;
		}
		this.sphere.ENABLE = 0f;
		this.sphere.REVEAL_AMOUNT = 0f;
		this.currentSize = 800f;
		yield break;
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x00064A5C File Offset: 0x00062C5C
	private void Move()
	{
		this.sphere.REVEAL_AMOUNT = 0f;
		this.sphere.ENABLE = Mathf.MoveTowards(this.sphere.ENABLE, 1f, Time.deltaTime * 0.1f);
		this.currentSize -= this.speed * Time.deltaTime;
		if (this.currentSize <= 30f)
		{
			this.Stop();
		}
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x00064AD0 File Offset: 0x00062CD0
	private void Stop()
	{
		this.hasArrived = true;
		this.isMoving = false;
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x00064AE0 File Offset: 0x00062CE0
	private void WaitToMove()
	{
		if (!this.PlayersAreResting)
		{
			this.currentWaitTime += Time.deltaTime;
		}
		if (!NetCode.Session.IsHost)
		{
			return;
		}
		if (this.PlayersHaveMovedOn() || this.TimeToMove())
		{
			this.photonView.RPC("StartMovingRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x00064B3A File Offset: 0x00062D3A
	private bool TimeToMove()
	{
		return Ascents.currentAscent >= 0 && this.currentWaitTime > this.maxWaitTime && this.currentID > 0;
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x00064B60 File Offset: 0x00062D60
	private bool PlayersHaveMovedOn()
	{
		if (Character.AllCharacters.Count == 0)
		{
			return false;
		}
		if (Ascents.currentAscent < 0)
		{
			return false;
		}
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (!Character.AllCharacters[i].data.dead && (Character.AllCharacters[i].Center.y < this.currentStartHeight || Character.AllCharacters[i].Center.z < this.currentStartForward))
			{
				return false;
			}
		}
		Debug.Log("Players have moved on");
		return true;
	}

	// Token: 0x060013E0 RID: 5088 RVA: 0x00064BF8 File Offset: 0x00062DF8
	private void ApplyMeshEffects()
	{
		this.sphere.currentSize = this.currentSize;
	}

	// Token: 0x060013E1 RID: 5089 RVA: 0x00064C0B File Offset: 0x00062E0B
	public void InitNewSphere(FogSphereOrigin newOrigin)
	{
		this.sphere.fogPoint = newOrigin.transform.position;
		this.currentSize = newOrigin.size;
		this.currentStartHeight = newOrigin.moveOnHeight;
		this.currentStartForward = newOrigin.moveOnForward;
	}

	// Token: 0x060013E2 RID: 5090 RVA: 0x00064C47 File Offset: 0x00062E47
	[PunRPC]
	public void StartMovingRPC()
	{
		this.currentWaitTime = 0f;
		this.hasArrived = false;
		this.isMoving = true;
		GUIManager.instance.TheFogRises();
	}

	// Token: 0x060013E3 RID: 5091 RVA: 0x00064C6C File Offset: 0x00062E6C
	public void SetFogOrigin(int id)
	{
		this.currentID = id;
		if (this.currentID < this.origins.Length)
		{
			this.hasArrived = false;
			this.sphere.gameObject.SetActive(true);
			this.InitNewSphere(this.origins[this.currentID]);
			return;
		}
		this.hasArrived = true;
		Debug.Log("Last section, disabling fog sphere");
		this.sphere.gameObject.SetActive(false);
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x00064CDE File Offset: 0x00062EDE
	public static void InitFogIfExists(Photon.Realtime.Player newPlayer)
	{
		if (!Singleton<OrbFogHandler>.Instance)
		{
			return;
		}
		Singleton<OrbFogHandler>.Instance.InitFogForPlayer(newPlayer);
	}

	// Token: 0x060013E5 RID: 5093 RVA: 0x00064CF8 File Offset: 0x00062EF8
	private void InitFogForPlayer(Photon.Realtime.Player newPlayer)
	{
		this.photonView.RPC("RPC_InitFog", newPlayer, new object[]
		{
			this.currentID,
			this.currentSize,
			this.hasArrived,
			this.isMoving
		});
	}

	// Token: 0x060013E6 RID: 5094 RVA: 0x00064D54 File Offset: 0x00062F54
	public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x00064D56 File Offset: 0x00062F56
	public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
	}

	// Token: 0x060013E8 RID: 5096 RVA: 0x00064D58 File Offset: 0x00062F58
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x060013E9 RID: 5097 RVA: 0x00064D5A File Offset: 0x00062F5A
	public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x00064D5C File Offset: 0x00062F5C
	public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
	{
	}

	// Token: 0x04001210 RID: 4624
	public float speed = 0.3f;

	// Token: 0x04001211 RID: 4625
	public float maxWaitTime = 500f;

	// Token: 0x04001213 RID: 4627
	public float currentWaitTime;

	// Token: 0x04001214 RID: 4628
	public bool hasArrived;

	// Token: 0x04001215 RID: 4629
	public bool isMoving;

	// Token: 0x04001216 RID: 4630
	public float currentSize;

	// Token: 0x04001217 RID: 4631
	public float currentStartHeight;

	// Token: 0x04001218 RID: 4632
	public float currentStartForward;

	// Token: 0x04001219 RID: 4633
	public float dispelFogAmount;

	// Token: 0x0400121A RID: 4634
	private FogSphere sphere;

	// Token: 0x0400121B RID: 4635
	private FogSphereOrigin[] origins;

	// Token: 0x0400121D RID: 4637
	private float syncCounter;

	// Token: 0x0400121E RID: 4638
	private PhotonView photonView;

	// Token: 0x0400121F RID: 4639
	public AnimationCurve fogRevealCurve;

	// Token: 0x04001220 RID: 4640
	public AnimationCurve fogFadeCurve;

	// Token: 0x04001221 RID: 4641
	public float currentCloseFog = 1f;
}
