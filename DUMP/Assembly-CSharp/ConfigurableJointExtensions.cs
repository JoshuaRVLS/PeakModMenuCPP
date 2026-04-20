using System;
using UnityEngine;

// Token: 0x02000030 RID: 48
public static class ConfigurableJointExtensions
{
	// Token: 0x06000382 RID: 898 RVA: 0x000180FA File Offset: 0x000162FA
	public static void SetTargetRotationLocal(this ConfigurableJoint joint, Quaternion targetLocalRotation, Quaternion startLocalRotation)
	{
		if (joint.configuredInWorldSpace)
		{
			Debug.LogError("SetTargetRotationLocal should not be used with joints that are configured in world space. For world space joints, use SetTargetRotation.", joint);
		}
		ConfigurableJointExtensions.SetTargetRotationInternal(joint, targetLocalRotation, startLocalRotation, Space.Self);
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00018118 File Offset: 0x00016318
	public static void SetTargetRotation(this ConfigurableJoint joint, Quaternion targetWorldRotation, Quaternion startWorldRotation)
	{
		if (!joint.configuredInWorldSpace)
		{
			Debug.LogError("SetTargetRotation must be used with joints that are configured in world space. For local space joints, use SetTargetRotationLocal.", joint);
		}
		ConfigurableJointExtensions.SetTargetRotationInternal(joint, targetWorldRotation, startWorldRotation, Space.World);
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00018138 File Offset: 0x00016338
	private static void SetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
	{
		Vector3 axis = joint.axis;
		Vector3 normalized = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
		Vector3 normalized2 = Vector3.Cross(normalized, axis).normalized;
		Quaternion quaternion = Quaternion.LookRotation(normalized, normalized2);
		Quaternion quaternion2 = Quaternion.Inverse(quaternion);
		if (space == Space.World)
		{
			quaternion2 *= startRotation * Quaternion.Inverse(targetRotation);
		}
		else
		{
			quaternion2 *= Quaternion.Inverse(targetRotation) * startRotation;
		}
		quaternion2 *= quaternion;
		joint.targetRotation = quaternion2;
	}
}
