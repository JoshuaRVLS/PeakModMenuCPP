using System;
using System.Collections.Generic;
using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class Bugfix : MonoBehaviour, IInteractible
{
	// Token: 0x060010C2 RID: 4290 RVA: 0x0005357E File Offset: 0x0005177E
	private void Start()
	{
		base.transform.localScale = Vector3.zero;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x0005359C File Offset: 0x0005179C
	private void LateUpdate()
	{
		this.counter += Time.deltaTime;
		this.lifeTime += Time.deltaTime;
		if (this.targetCharacter && !this.targetCharacter.data.dead)
		{
			if (this.targetCharacter.IsLocal && this.counter > 29f)
			{
				this.targetCharacter.refs.afflictions.AddAffliction(new Affliction_PreventPoisonHealing(30f), false);
				if (this.totalStatusApplied < this.maxStatus || this.targetCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison) < 0.5f)
				{
					this.targetCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.05f, false, true, true);
					this.totalStatusApplied += 0.05f;
				}
				this.counter = 0f;
			}
			Vector3 position = this.leg.TransformPoint(this.localPos);
			base.transform.position = position;
			Quaternion rotation = Quaternion.LookRotation(this.leg.TransformDirection(this.forward), this.leg.TransformDirection(this.up));
			base.transform.rotation = rotation;
			base.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, this.lifeTime / 300f);
			return;
		}
		if (this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x0005372C File Offset: 0x0005192C
	[PunRPC]
	public void AttachBug(int targetID)
	{
		PhotonView photonView = PhotonView.Find(targetID);
		this.targetCharacter = photonView.GetComponent<Character>();
		Rigidbody bodypartRig = this.targetCharacter.GetBodypartRig(BodypartType.Knee_R);
		this.leg = bodypartRig.transform;
		this.localPos = new Vector3(-0.27054f, 0f, -0.17134f);
		Vector3 position = bodypartRig.transform.TransformPoint(this.localPos);
		Vector3 euler = new Vector3(0f, 55f, 0f);
		Quaternion rotation = bodypartRig.transform.rotation * Quaternion.Euler(euler);
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.forward = this.leg.InverseTransformDirection(base.transform.forward);
		this.up = this.leg.InverseTransformDirection(base.transform.up);
		Bugfix.AllAttachedBugs.Add(this, this.targetCharacter);
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x00053822 File Offset: 0x00051A22
	private void OnDestroy()
	{
		Bugfix.AllAttachedBugs.Remove(this);
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x00053830 File Offset: 0x00051A30
	public bool IsInteractible(Character interactor)
	{
		float num = Vector3.Angle(base.transform.position - MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
		float num2 = (Character.AllCharacters.Count == 1) ? 15f : 0f;
		return num <= 2f + num2 + this.lifeTime / 60f;
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x000538A3 File Offset: 0x00051AA3
	public void Interact(Character interactor)
	{
		GameUtils.instance.InstantiateAndGrab(this.bugItem, interactor, 0);
		this.photonView.RPC("RPCA_Remove", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x000538CD File Offset: 0x00051ACD
	[PunRPC]
	public void RPCA_Remove()
	{
		if (this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x000538E7 File Offset: 0x00051AE7
	public void HoverEnter()
	{
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x000538E9 File Offset: 0x00051AE9
	public void HoverExit()
	{
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x000538EB File Offset: 0x00051AEB
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x000538F8 File Offset: 0x00051AF8
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x060010CD RID: 4301 RVA: 0x00053900 File Offset: 0x00051B00
	public string GetInteractionText()
	{
		return LocalizedText.GetText("TICKINTERACT", true);
	}

	// Token: 0x060010CE RID: 4302 RVA: 0x0005390D File Offset: 0x00051B0D
	public string GetName()
	{
		return LocalizedText.GetText("TICK", true);
	}

	// Token: 0x04000EC4 RID: 3780
	public Item bugItem;

	// Token: 0x04000EC5 RID: 3781
	private Transform leg;

	// Token: 0x04000EC6 RID: 3782
	private Vector3 localPos;

	// Token: 0x04000EC7 RID: 3783
	private Vector3 forward;

	// Token: 0x04000EC8 RID: 3784
	private Vector3 up;

	// Token: 0x04000EC9 RID: 3785
	public float maxStatus = 0.5f;

	// Token: 0x04000ECA RID: 3786
	private float totalStatusApplied;

	// Token: 0x04000ECB RID: 3787
	private float lifeTime;

	// Token: 0x04000ECC RID: 3788
	private PhotonView photonView;

	// Token: 0x04000ECD RID: 3789
	private Character targetCharacter;

	// Token: 0x04000ECE RID: 3790
	public static Dictionary<Bugfix, Character> AllAttachedBugs = new Dictionary<Bugfix, Character>();

	// Token: 0x04000ECF RID: 3791
	private float counter;
}
