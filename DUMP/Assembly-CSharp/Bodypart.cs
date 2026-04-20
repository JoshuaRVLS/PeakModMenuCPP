using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002E RID: 46
[DefaultExecutionOrder(-1)]
public class Bodypart : MonoBehaviour
{
	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000366 RID: 870 RVA: 0x00017883 File Offset: 0x00015A83
	public Rigidbody Rig
	{
		get
		{
			if (this.rig == null)
			{
				this.rig = base.GetComponent<Rigidbody>();
			}
			return this.rig;
		}
	}

	// Token: 0x06000367 RID: 871 RVA: 0x000178A8 File Offset: 0x00015AA8
	private void Awake()
	{
		this.startLocal = base.transform.localRotation;
		this.prevPos = base.transform.position;
		this.prevRot = base.transform.rotation;
		this.rig = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000368 RID: 872 RVA: 0x000178F4 File Offset: 0x00015AF4
	public void Start()
	{
		if (this.started)
		{
			return;
		}
		this.started = true;
		this.character = base.GetComponentInParent<Character>();
		this.joint = base.GetComponent<ConfigurableJoint>();
		if (this.joint)
		{
			this.jointParent = this.joint.connectedBody.GetComponent<Bodypart>();
		}
		if (this.rig)
		{
			this.rig.maxAngularVelocity = 50f;
		}
		this.localCenterOfMass = HelperFunctions.GetCenterOfMass(base.transform);
	}

	// Token: 0x06000369 RID: 873 RVA: 0x0001797A File Offset: 0x00015B7A
	internal void RegisterCollider(RigCreatorCollider rigCreatorCollider)
	{
		this.colliders.Add(rigCreatorCollider);
	}

	// Token: 0x0600036A RID: 874 RVA: 0x00017988 File Offset: 0x00015B88
	internal void InitBodypart(BodypartType setPartType)
	{
		this.partType = setPartType;
	}

	// Token: 0x0600036B RID: 875 RVA: 0x00017991 File Offset: 0x00015B91
	private Vector3 WorldCenterOfMass()
	{
		return base.transform.position;
	}

	// Token: 0x0600036C RID: 876 RVA: 0x000179A0 File Offset: 0x00015BA0
	public void SaveAnimationData()
	{
		if (this != this.character.refs.hip)
		{
			this.targetOffsetRelativeToHip = this.WorldCenterOfMass() - this.character.refs.hip.transform.position;
		}
		this.targetRotation = base.transform.localRotation;
		this.targetForward = base.transform.forward;
		this.targetUp = base.transform.up;
	}

	// Token: 0x0600036D RID: 877 RVA: 0x00017A23 File Offset: 0x00015C23
	public void ResetTransform()
	{
		base.transform.rotation = this.rig.rotation;
		base.transform.position = this.rig.position;
	}

	// Token: 0x0600036E RID: 878 RVA: 0x00017A51 File Offset: 0x00015C51
	internal void Animate(float force, float torque)
	{
		if (this.rig.isKinematic)
		{
			this.SnapToAnim();
			return;
		}
		this.FollowRotation_Joint();
		this.FollowRotation_Rotation(torque);
		this.FollowRotation_Position(force);
	}

	// Token: 0x0600036F RID: 879 RVA: 0x00017A7C File Offset: 0x00015C7C
	public void SnapToAnim()
	{
		this.Start();
		Vector3 b = this.WorldTargetPos() - this.WorldCenterOfMass();
		base.transform.position += b;
		if (this.targetForward != Vector3.zero && this.targetUp != Vector3.zero)
		{
			base.transform.rotation = Quaternion.LookRotation(this.targetForward, this.targetUp);
		}
		if (this.rig.isKinematic)
		{
			return;
		}
		this.rig.linearVelocity *= 0f;
		this.rig.angularVelocity *= 0f;
	}

	// Token: 0x06000370 RID: 880 RVA: 0x00017B3C File Offset: 0x00015D3C
	private void DrawDebug()
	{
	}

	// Token: 0x06000371 RID: 881 RVA: 0x00017B40 File Offset: 0x00015D40
	private void FollowRotation_Joint()
	{
		if (!this.joint)
		{
			return;
		}
		this.joint.SetTargetRotationLocal(this.targetRotation, this.startLocal);
	}

	// Token: 0x06000372 RID: 882 RVA: 0x00017B74 File Offset: 0x00015D74
	private void FollowRotation_Rotation(float torque)
	{
		if (this.rig.isKinematic)
		{
			return;
		}
		Vector3 a = Vector3.Cross(base.transform.forward, this.targetForward).normalized * Vector3.Angle(base.transform.forward, this.targetForward);
		a += Vector3.Cross(base.transform.up, this.targetUp).normalized * Vector3.Angle(base.transform.up, this.targetUp);
		this.rig.AddTorque(a * torque, ForceMode.Acceleration);
	}

	// Token: 0x06000373 RID: 883 RVA: 0x00017C1C File Offset: 0x00015E1C
	private void FollowRotation_Position(float force)
	{
		if (!this.character)
		{
			return;
		}
		if (this == this.character.refs.hip)
		{
			return;
		}
		if (this.targetOffsetRelativeToHip == Vector3.zero)
		{
			return;
		}
		Vector3 vector = (this.WorldTargetPos() - this.WorldCenterOfMass()) * force;
		this.AddForce(vector, ForceMode.Acceleration);
		if (this.jointParent)
		{
			Vector3 a = vector * this.rig.mass;
			this.jointParent.AddForce(-a * 0.5f, ForceMode.Force);
			this.character.refs.hip.AddForce(-a * 0.5f, ForceMode.Force);
		}
	}

	// Token: 0x06000374 RID: 884 RVA: 0x00017CE4 File Offset: 0x00015EE4
	private Vector3 WorldTargetPos()
	{
		return this.character.refs.hip.transform.position + this.targetOffsetRelativeToHip;
	}

	// Token: 0x06000375 RID: 885 RVA: 0x00017D0C File Offset: 0x00015F0C
	internal void Drag(float drag, bool ignoreRagdoll = false)
	{
		if (!ignoreRagdoll)
		{
			drag = Mathf.Lerp(1f, drag, this.character.data.currentRagdollControll);
		}
		if (this.rig.isKinematic)
		{
			return;
		}
		this.rig.linearVelocity *= drag;
		this.rig.angularVelocity *= drag;
	}

	// Token: 0x06000376 RID: 886 RVA: 0x00017D78 File Offset: 0x00015F78
	internal void ParasolDrag(float drag, float xzDrag, bool ignoreRagdoll = false)
	{
		if (!ignoreRagdoll)
		{
			drag = Mathf.Lerp(1f, drag, this.character.data.currentRagdollControll);
		}
		if (this.rig.isKinematic)
		{
			return;
		}
		if (this.rig.linearVelocity.y < 0f)
		{
			this.rig.linearVelocity = new Vector3(this.rig.linearVelocity.x * xzDrag, this.rig.linearVelocity.y * drag, this.rig.linearVelocity.z * xzDrag);
		}
	}

	// Token: 0x06000377 RID: 887 RVA: 0x00017E10 File Offset: 0x00016010
	private void OnCollisionEnter(Collision collision)
	{
		if (this.character == null)
		{
			return;
		}
		if (collision.collider.transform.root == base.transform.root)
		{
			return;
		}
		Action<Collision> action = this.collisionEnterAction;
		if (action != null)
		{
			action(collision);
		}
		this.character.refs.movement.OnCollision(collision, true, this);
	}

	// Token: 0x06000378 RID: 888 RVA: 0x00017E7C File Offset: 0x0001607C
	private void OnCollisionStay(Collision collision)
	{
		if (!this.character)
		{
			return;
		}
		if (collision.collider.transform.root == base.transform.root)
		{
			return;
		}
		Action<Collision> action = this.collisionStayAction;
		if (action != null)
		{
			action(collision);
		}
		this.character.refs.movement.OnCollision(collision, false, this);
	}

	// Token: 0x06000379 RID: 889 RVA: 0x00017EE4 File Offset: 0x000160E4
	internal void Gravity(Vector3 gravity)
	{
		this.AddForce(gravity, ForceMode.Acceleration);
	}

	// Token: 0x0600037A RID: 890 RVA: 0x00017EEE File Offset: 0x000160EE
	public void AddForce(Vector3 force, ForceMode forceMode)
	{
		if (this.rig.isKinematic)
		{
			return;
		}
		if (forceMode == ForceMode.Acceleration)
		{
			force *= this.rig.mass;
		}
		this.forcesToAdd += force;
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00017F27 File Offset: 0x00016127
	internal void ToggleUseGravity(bool useGrav)
	{
		if (this.rig.useGravity != useGrav)
		{
			this.rig.useGravity = useGrav;
		}
	}

	// Token: 0x0600037C RID: 892 RVA: 0x00017F43 File Offset: 0x00016143
	internal void ApplyForces()
	{
		if (!this.rig.isKinematic)
		{
			this.rig.AddForce(this.forcesToAdd, ForceMode.Force);
		}
		this.forcesToAdd *= 0f;
	}

	// Token: 0x0600037D RID: 893 RVA: 0x00017F7C File Offset: 0x0001617C
	internal void AddMovementForce(float movementForce)
	{
		if (!this.character)
		{
			return;
		}
		Vector3 worldMovementInput_Lerp = this.character.data.worldMovementInput_Lerp;
		this.AddForce(movementForce * worldMovementInput_Lerp, ForceMode.Acceleration);
	}

	// Token: 0x0600037E RID: 894 RVA: 0x00017FB8 File Offset: 0x000161B8
	internal void SetPhysicsMaterial(Bodypart.FrictionType setFrictionType, PhysicsMaterial slipperyMat, PhysicsMaterial normalMat)
	{
		foreach (RigCreatorCollider rigCreatorCollider in this.colliders)
		{
			if (this.frictionType == Bodypart.FrictionType.Grippy)
			{
				rigCreatorCollider.col.sharedMaterial = normalMat;
			}
			else if (this.frictionType == Bodypart.FrictionType.Slippery)
			{
				rigCreatorCollider.col.sharedMaterial = slipperyMat;
			}
			else if (setFrictionType == Bodypart.FrictionType.Grippy)
			{
				rigCreatorCollider.col.sharedMaterial = normalMat;
			}
			else
			{
				rigCreatorCollider.col.sharedMaterial = slipperyMat;
			}
		}
	}

	// Token: 0x04000315 RID: 789
	private Character character;

	// Token: 0x04000316 RID: 790
	public BodypartType partType;

	// Token: 0x04000317 RID: 791
	public Bodypart.FrictionType frictionType;

	// Token: 0x04000318 RID: 792
	private Rigidbody rig;

	// Token: 0x04000319 RID: 793
	internal Bodypart jointParent;

	// Token: 0x0400031A RID: 794
	private Quaternion startLocal = Quaternion.identity;

	// Token: 0x0400031B RID: 795
	private Vector3 localCenterOfMass;

	// Token: 0x0400031C RID: 796
	private ConfigurableJoint joint;

	// Token: 0x0400031D RID: 797
	private Quaternion targetRotation = Quaternion.identity;

	// Token: 0x0400031E RID: 798
	private Quaternion lastTargetRotation = Quaternion.identity;

	// Token: 0x0400031F RID: 799
	private Vector3 targetForward;

	// Token: 0x04000320 RID: 800
	private Vector3 targetUp;

	// Token: 0x04000321 RID: 801
	private Vector3 targetOffsetRelativeToHip;

	// Token: 0x04000322 RID: 802
	internal List<RigCreatorCollider> colliders = new List<RigCreatorCollider>();

	// Token: 0x04000323 RID: 803
	public Vector3 forcesToAdd;

	// Token: 0x04000324 RID: 804
	private bool started;

	// Token: 0x04000325 RID: 805
	private Vector3 prevPos;

	// Token: 0x04000326 RID: 806
	private Quaternion prevRot;

	// Token: 0x04000327 RID: 807
	public Action<Collision> collisionEnterAction;

	// Token: 0x04000328 RID: 808
	public Action<Collision> collisionStayAction;

	// Token: 0x02000427 RID: 1063
	public enum FrictionType
	{
		// Token: 0x04001850 RID: 6224
		Unspecified,
		// Token: 0x04001851 RID: 6225
		Grippy,
		// Token: 0x04001852 RID: 6226
		Slippery
	}
}
