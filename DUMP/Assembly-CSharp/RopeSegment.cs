using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000178 RID: 376
public class RopeSegment : MonoBehaviour, IInteractible
{
	// Token: 0x06000C37 RID: 3127 RVA: 0x00041C24 File Offset: 0x0003FE24
	private void Awake()
	{
		this.rope = base.GetComponentInParent<Rope>();
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x00041C32 File Offset: 0x0003FE32
	private void Update()
	{
		this.angle = this.GetAngle();
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x00041C40 File Offset: 0x0003FE40
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x00041C4D File Offset: 0x0003FE4D
	public string GetInteractionText()
	{
		return LocalizedText.GetText("GRAB", true);
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x00041C5A File Offset: 0x0003FE5A
	public string GetName()
	{
		return LocalizedText.GetText(this.displayName, true);
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x00041C68 File Offset: 0x0003FE68
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x00041C70 File Offset: 0x0003FE70
	public void Interact(Character interactor)
	{
		interactor.refs.items.EquipSlot(Optionable<byte>.None);
		int num = base.transform.GetSiblingIndex() - 2;
		Debug.Log(string.Format("Grabbing Rope: {0} with index {1}", base.gameObject.name, num));
		interactor.GetComponent<PhotonView>().RPC("GrabRopeRpc", RpcTarget.All, new object[]
		{
			this.rope.GetComponentInParent<PhotonView>(),
			num
		});
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x00041CF0 File Offset: 0x0003FEF0
	public bool IsInteractible(Character interactor)
	{
		float num = this.GetAngle();
		bool flag = num < interactor.refs.ropeHandling.maxRopeAngle * 0.6f || 180f - num < interactor.refs.ropeHandling.maxRopeAngle * 0.6f;
		flag = (flag && this.rope.isClimbable);
		if (interactor.data.isRopeClimbing)
		{
			flag = (flag && interactor.data.heldRope != this.rope);
		}
		return flag;
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x00041D7D File Offset: 0x0003FF7D
	public float GetAngle()
	{
		return Vector3.Angle(Vector3.up, base.transform.up);
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x00041D94 File Offset: 0x0003FF94
	internal Vector3 GetClimbNormal(Vector3 charPos)
	{
		Vector3 vector = charPos - base.transform.position;
		vector = Vector3.ProjectOnPlane(vector, base.transform.up);
		return base.transform.InverseTransformDirection(vector);
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x00041DD4 File Offset: 0x0003FFD4
	internal void Tie(Vector3 tiePos)
	{
		RopeSegment.<>c__DisplayClass13_0 CS$<>8__locals1 = new RopeSegment.<>c__DisplayClass13_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.joint = base.gameObject.AddComponent<ConfigurableJoint>();
		CS$<>8__locals1.joint.xMotion = ConfigurableJointMotion.Locked;
		CS$<>8__locals1.joint.yMotion = ConfigurableJointMotion.Locked;
		CS$<>8__locals1.joint.zMotion = ConfigurableJointMotion.Locked;
		CS$<>8__locals1.joint.anchor = Vector3.zero;
		base.StartCoroutine(CS$<>8__locals1.<Tie>g__ITighten|0(tiePos));
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x00041E41 File Offset: 0x00040041
	public void HoverEnter()
	{
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x00041E43 File Offset: 0x00040043
	public void HoverExit()
	{
	}

	// Token: 0x04000B2F RID: 2863
	public Rope rope;

	// Token: 0x04000B30 RID: 2864
	public float angle;

	// Token: 0x04000B31 RID: 2865
	public string displayName;
}
