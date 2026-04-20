using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020002D8 RID: 728
public class PointPinger : MonoBehaviour
{
	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06001456 RID: 5206 RVA: 0x0006715A File Offset: 0x0006535A
	private bool inCooldown
	{
		get
		{
			return Time.time - this._timeLastPinged < this.coolDown;
		}
	}

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06001457 RID: 5207 RVA: 0x00067170 File Offset: 0x00065370
	private bool shouldPing
	{
		get
		{
			return this.photonView.IsMine && this.character.input.pingWasPressed;
		}
	}

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06001458 RID: 5208 RVA: 0x00067191 File Offset: 0x00065391
	private bool canPing
	{
		get
		{
			return this.character.data.fullyConscious && !this.inCooldown;
		}
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x000671B0 File Offset: 0x000653B0
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600145A RID: 5210 RVA: 0x000671CA File Offset: 0x000653CA
	private static bool TryGetPingHit(out RaycastHit hit)
	{
		return Camera.main.ScreenPointToRay(Input.mousePosition).Raycast(out hit, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), -1f);
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x000671EC File Offset: 0x000653EC
	private void DoPing()
	{
		RaycastHit raycastHit;
		if (!PointPinger.TryGetPingHit(out raycastHit))
		{
			return;
		}
		this._timeLastPinged = Time.time;
		this.photonView.RPC("ReceivePoint_Rpc", RpcTarget.All, new object[]
		{
			raycastHit.point,
			raycastHit.normal
		});
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x00067243 File Offset: 0x00065443
	private void Update()
	{
		if (this.shouldPing && this.canPing)
		{
			this.DoPing();
		}
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x0006725C File Offset: 0x0006545C
	[PunRPC]
	private void ReceivePoint_Rpc(Vector3 point, Vector3 hitNormal)
	{
		RaycastHit raycastHit;
		bool flag = PExt.LineCast(this.character.Head, Character.localCharacter.Head, out raycastHit, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), true);
		float value = Vector3.Distance(this.character.Head, Character.localCharacter.Head);
		PointPing component = this.pointPrefab.GetComponent<PointPing>();
		Vector2 visibilityFullNoneNoLos = component.visibilityFullNoneNoLos;
		float num = 1f - Mathf.InverseLerp(visibilityFullNoneNoLos.x, visibilityFullNoneNoLos.x + (visibilityFullNoneNoLos.y - visibilityFullNoneNoLos.x) * (flag ? component.NoLosVisibilityMul : 1f), value);
		if (num <= 0f)
		{
			return;
		}
		if (this.pingInstance != null)
		{
			Object.DestroyImmediate(this.pingInstance);
		}
		this.pingInstance = Object.Instantiate<GameObject>(this.pointPrefab, point, Quaternion.LookRotation((point - this.character.Head).normalized, Vector3.up));
		PointPing component2 = this.pingInstance.GetComponent<PointPing>();
		component2.hitNormal = hitNormal;
		component2.Init(this.character);
		component2.pointPinger = this;
		component2.renderer.material = Object.Instantiate<Material>(this.character.refs.mainRenderer.sharedMaterial);
		component2.material.SetFloat("_Opacity", num);
		Object.Destroy(this.pingInstance, 2f);
	}

	// Token: 0x04001297 RID: 4759
	public GameObject pointPrefab;

	// Token: 0x04001298 RID: 4760
	public float coolDown;

	// Token: 0x04001299 RID: 4761
	public Character character;

	// Token: 0x0400129A RID: 4762
	private GameObject pingInstance;

	// Token: 0x0400129B RID: 4763
	private PhotonView photonView;

	// Token: 0x0400129C RID: 4764
	private float _timeLastPinged;
}
