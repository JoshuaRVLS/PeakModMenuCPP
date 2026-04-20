using System;
using UnityEngine;

// Token: 0x02000038 RID: 56
[ExecuteInEditMode]
public class RigCreatorJoint : MonoBehaviour
{
	// Token: 0x060003B2 RID: 946 RVA: 0x00018E82 File Offset: 0x00017082
	private void Awake()
	{
		if (!Application.isEditor || Application.isPlaying)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x00018E98 File Offset: 0x00017098
	private ConfigurableJoint Joint()
	{
		if (!this.joint)
		{
			this.joint = base.GetComponentInParent<ConfigurableJoint>();
		}
		return this.joint;
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x00018EB9 File Offset: 0x000170B9
	private Rigidbody Rig()
	{
		if (!this.rig)
		{
			this.rig = base.GetComponentInParent<Rigidbody>();
		}
		return this.rig;
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x00018EDA File Offset: 0x000170DA
	private RigCreator RigCreator()
	{
		if (!this.rigCreator)
		{
			this.rigCreator = base.GetComponentInParent<RigCreator>();
		}
		return this.rigCreator;
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x00018EFB File Offset: 0x000170FB
	private void Update()
	{
		if (this.spring != this.CurrentSpring())
		{
			this.SetSpring(this.spring);
			this.RigCreator().JointChanged(this, this.CurrentSpring());
		}
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x00018F2C File Offset: 0x0001712C
	private float CurrentSpring()
	{
		return this.Joint().angularXDrive.positionSpring / (this.Rig().mass * this.RigCreator().springMultiplier);
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x00018F64 File Offset: 0x00017164
	internal void SetSpring(float spring)
	{
		JointDrive angularXDrive = this.Joint().angularXDrive;
		angularXDrive.positionSpring = this.Rig().mass * spring * this.RigCreator().springMultiplier;
		angularXDrive.positionDamper = this.Rig().mass * spring * 0.1f * this.RigCreator().springMultiplier;
		this.Joint().angularXDrive = angularXDrive;
		this.Joint().angularYZDrive = angularXDrive;
	}

	// Token: 0x04000400 RID: 1024
	public float spring;

	// Token: 0x04000401 RID: 1025
	internal ConfigurableJoint joint;

	// Token: 0x04000402 RID: 1026
	internal Rigidbody rig;

	// Token: 0x04000403 RID: 1027
	internal RigCreator rigCreator;
}
