using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class RigCreator : MonoBehaviour
{
	// Token: 0x06000385 RID: 901 RVA: 0x000181BF File Offset: 0x000163BF
	public void StartClear()
	{
		this.aboutToClear = true;
	}

	// Token: 0x06000386 RID: 902 RVA: 0x000181C8 File Offset: 0x000163C8
	public void ClearNo()
	{
		this.ClearStates();
	}

	// Token: 0x06000387 RID: 903 RVA: 0x000181D0 File Offset: 0x000163D0
	public void ClearYes()
	{
		this.ClearStates();
		this.ClearDataAndRig();
	}

	// Token: 0x06000388 RID: 904 RVA: 0x000181DE File Offset: 0x000163DE
	public void AutoGenerate()
	{
		this.FindParts();
		this.GenerateData();
	}

	// Token: 0x06000389 RID: 905 RVA: 0x000181EC File Offset: 0x000163EC
	private void ClearStates()
	{
		this.aboutToClear = false;
	}

	// Token: 0x0600038A RID: 906 RVA: 0x000181F8 File Offset: 0x000163F8
	private void GenerateData()
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].justCreated)
			{
				this.InitPart(this.parts[i]);
			}
			else
			{
				this.ApplyPartData(this.parts[i]);
			}
			this.parts[i].justCreated = false;
		}
	}

	// Token: 0x0600038B RID: 907 RVA: 0x00018266 File Offset: 0x00016466
	private void InitPart(RigPart part)
	{
		this.AutoGenerateCollidersForPart(part);
		this.AddRigidbodyToPart(part);
		this.AddJointToPart(part);
		this.AddBodyPartScript(part);
	}

	// Token: 0x0600038C RID: 908 RVA: 0x00018284 File Offset: 0x00016484
	private void ApplyPartData(RigPart rigPart)
	{
		this.SyncCollidersFromData(rigPart);
		this.SyncRigidbodyFromData(rigPart);
		this.SyncJointFromData(rigPart);
		this.SyncBodypartScript(rigPart);
	}

	// Token: 0x0600038D RID: 909 RVA: 0x000182A2 File Offset: 0x000164A2
	private void SyncBodypartScript(RigPart rigPart)
	{
		if (!rigPart.transform.GetComponent<Bodypart>())
		{
			this.AddBodyPartScript(rigPart);
		}
	}

	// Token: 0x0600038E RID: 910 RVA: 0x000182BD File Offset: 0x000164BD
	private void SyncJointFromData(RigPart rigPart)
	{
		if (rigPart.joint == null)
		{
			this.AddJointToPart(rigPart);
		}
	}

	// Token: 0x0600038F RID: 911 RVA: 0x000182D4 File Offset: 0x000164D4
	private void SyncRigidbodyFromData(RigPart rigPart)
	{
		if (rigPart.rig == null)
		{
			this.AddRigidbodyToPart(rigPart);
		}
	}

	// Token: 0x06000390 RID: 912 RVA: 0x000182EC File Offset: 0x000164EC
	private void AddRigidbodyToPart(RigPart rigPart)
	{
		Rigidbody rigidbody = rigPart.transform.gameObject.AddComponent<Rigidbody>();
		rigidbody.mass = rigPart.mass;
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		RigCreatorRigidbody rigCreatorRigidbody = rigPart.transform.gameObject.AddComponent<RigCreatorRigidbody>();
		rigCreatorRigidbody.mass = rigPart.mass;
		rigPart.rig = rigidbody;
		rigPart.rigHandler = rigCreatorRigidbody;
	}

	// Token: 0x06000391 RID: 913 RVA: 0x00018348 File Offset: 0x00016548
	private void SyncCollidersFromData(RigPart rigPart)
	{
		for (int i = 0; i < rigPart.colliders.Count; i++)
		{
			if (rigPart.colliders[i].colliderObject == null)
			{
				rigPart.colliders[i] = this.CreateColliderObject(rigPart.colliders[i].colliderPosition, rigPart.colliders[i].colliderRotation, rigPart.colliders[i].colliderScale, rigPart.transform, rigPart.colliders[i].height, rigPart.colliders[i].radius, false);
			}
		}
	}

	// Token: 0x06000392 RID: 914 RVA: 0x000183F8 File Offset: 0x000165F8
	private RigCreatorColliderData CreateColliderObject(Vector3 position, Quaternion rotation, Vector3 scale, Transform parent, float height, float radius, bool isWorldSpace = true)
	{
		GameObject gameObject = new GameObject("RigCollider");
		if (isWorldSpace)
		{
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}
		gameObject.transform.SetParent(parent);
		if (!isWorldSpace)
		{
			gameObject.transform.localPosition = position;
			gameObject.transform.localRotation = rotation;
			gameObject.transform.localScale = scale;
		}
		CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.direction = 2;
		capsuleCollider.radius = radius;
		capsuleCollider.height = height;
		RigCreatorColliderData rigCreatorColliderData = new RigCreatorColliderData();
		rigCreatorColliderData.colliderPosition = capsuleCollider.transform.position;
		rigCreatorColliderData.colliderRotation = capsuleCollider.transform.rotation;
		rigCreatorColliderData.colliderScale = capsuleCollider.transform.localScale;
		rigCreatorColliderData.radius = capsuleCollider.radius;
		rigCreatorColliderData.height = capsuleCollider.height;
		RigCreatorCollider colliderObject = gameObject.AddComponent<RigCreatorCollider>();
		rigCreatorColliderData.colliderObject = colliderObject;
		return rigCreatorColliderData;
	}

	// Token: 0x06000393 RID: 915 RVA: 0x000184DD File Offset: 0x000166DD
	private void AddBodyPartScript(RigPart rigPart)
	{
		rigPart.transform.gameObject.AddComponent<Bodypart>().InitBodypart(rigPart.partType);
	}

	// Token: 0x06000394 RID: 916 RVA: 0x000184FC File Offset: 0x000166FC
	private void AddJointToPart(RigPart rigPart)
	{
		Rigidbody componentInParent = rigPart.transform.parent.GetComponentInParent<Rigidbody>();
		if (!componentInParent)
		{
			return;
		}
		ConfigurableJoint joint = this.SpawnJoint(rigPart.rig, componentInParent, rigPart.spring);
		rigPart.joint = joint;
		rigPart.jointHandler = rigPart.transform.gameObject.AddComponent<RigCreatorJoint>();
		rigPart.jointHandler.spring = rigPart.spring;
		rigPart.jointHandler.SetSpring(rigPart.spring);
	}

	// Token: 0x06000395 RID: 917 RVA: 0x00018578 File Offset: 0x00016778
	internal ConfigurableJoint SpawnJoint(Rigidbody ownRig, Rigidbody otherRig, float spring)
	{
		ConfigurableJoint configurableJoint = ownRig.gameObject.AddComponent<ConfigurableJoint>();
		SoftJointLimit softJointLimit = configurableJoint.lowAngularXLimit;
		softJointLimit.limit = -177f;
		configurableJoint.lowAngularXLimit = softJointLimit;
		softJointLimit = configurableJoint.highAngularXLimit;
		softJointLimit.limit = 177f;
		configurableJoint.highAngularXLimit = softJointLimit;
		softJointLimit = configurableJoint.angularYLimit;
		softJointLimit.limit = 177f;
		configurableJoint.angularYLimit = softJointLimit;
		softJointLimit = configurableJoint.angularZLimit;
		softJointLimit.limit = 177f;
		configurableJoint.angularZLimit = softJointLimit;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Locked;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		configurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;
		configurableJoint.connectedBody = otherRig;
		return configurableJoint;
	}

	// Token: 0x06000396 RID: 918 RVA: 0x00018630 File Offset: 0x00016830
	private void AutoGenerateCollidersForPart(RigPart rigPart)
	{
		Transform transform = null;
		float num = 0f;
		for (int i = rigPart.transform.childCount - 1; i >= 0; i--)
		{
			float num2 = Vector3.Distance(rigPart.transform.GetChild(i).position, rigPart.transform.position);
			if (num2 > num)
			{
				num = num2;
				transform = rigPart.transform.GetChild(i);
			}
		}
		Vector3 position = Vector3.Lerp(rigPart.transform.position, transform.position, 0.5f);
		Quaternion rotation = Quaternion.LookRotation(transform.position - rigPart.transform.position);
		float height = Vector3.Distance(transform.position, rigPart.transform.position);
		rigPart.colliders.Add(this.CreateColliderObject(position, rotation, Vector3.one, rigPart.transform, height, 0.1f, true));
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00018714 File Offset: 0x00016914
	private void FindParts()
	{
		for (int i = 0; i < 179; i++)
		{
			if (this.Contains((BodypartType)i))
			{
				BodypartType bodypartType = (BodypartType)i;
				Transform transform = HelperFunctions.FindChildRecursive(bodypartType.ToString(), base.transform);
				if (transform)
				{
					this.GetPartFromPartType((BodypartType)i).transform = transform;
				}
			}
			else
			{
				BodypartType bodypartType = (BodypartType)i;
				Transform transform2 = HelperFunctions.FindChildRecursive(bodypartType.ToString(), base.transform);
				if (transform2)
				{
					RigPart rigPart = new RigPart();
					rigPart.transform = transform2;
					rigPart.partType = (BodypartType)i;
					rigPart.justCreated = true;
					this.parts.Add(rigPart);
				}
			}
		}
	}

	// Token: 0x06000398 RID: 920 RVA: 0x000187C4 File Offset: 0x000169C4
	private RigPart GetPartFromPartType(BodypartType partType)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].partType == partType)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x06000399 RID: 921 RVA: 0x0001880C File Offset: 0x00016A0C
	private bool Contains(BodypartType targetType)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].partType == targetType)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00018848 File Offset: 0x00016A48
	private void ClearDataAndRig()
	{
		for (int i = this.parts.Count - 1; i >= 0; i--)
		{
			for (int j = this.parts[i].colliders.Count - 1; j >= 0; j--)
			{
				Object.DestroyImmediate(this.parts[i].colliders[j].colliderObject.gameObject);
			}
			this.parts[i].colliders.Clear();
			Bodypart component = this.parts[i].transform.GetComponent<Bodypart>();
			if (component)
			{
				Object.DestroyImmediate(component);
			}
			Object.DestroyImmediate(this.parts[i].joint);
			if (this.parts[i].jointHandler)
			{
				Object.DestroyImmediate(this.parts[i].jointHandler);
			}
			Object.DestroyImmediate(this.parts[i].rig);
			Object.DestroyImmediate(this.parts[i].rigHandler);
		}
		this.parts.Clear();
	}

	// Token: 0x0600039B RID: 923 RVA: 0x00018974 File Offset: 0x00016B74
	private RigPart GetPartFromJointObject(RigCreatorJoint jointObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].jointHandler == jointObject)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x0600039C RID: 924 RVA: 0x000189C0 File Offset: 0x00016BC0
	private RigPart GetPartFromRigObject(RigCreatorRigidbody rigObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].rigHandler == rigObject)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x0600039D RID: 925 RVA: 0x00018A0C File Offset: 0x00016C0C
	private RigPart GetPartFromColliderObject(RigCreatorCollider colliderObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			for (int j = this.parts[i].colliders.Count - 1; j >= 0; j--)
			{
				if (this.parts[i].colliders[j].colliderObject == colliderObject)
				{
					return this.parts[i];
				}
			}
		}
		return null;
	}

	// Token: 0x0600039E RID: 926 RVA: 0x00018A84 File Offset: 0x00016C84
	private RigCreatorColliderData GetColliderDataFromColliderObject(RigCreatorCollider colliderObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			for (int j = this.parts[i].colliders.Count - 1; j >= 0; j--)
			{
				if (this.parts[i].colliders[j].colliderObject == colliderObject)
				{
					return this.parts[i].colliders[j];
				}
			}
		}
		return null;
	}

	// Token: 0x0600039F RID: 927 RVA: 0x00018B08 File Offset: 0x00016D08
	internal void RemoveCollider(RigCreatorCollider rigCreatorCollider)
	{
		RigCreatorColliderData colliderDataFromColliderObject = this.GetColliderDataFromColliderObject(rigCreatorCollider);
		if (colliderDataFromColliderObject != null)
		{
			RigPart partFromColliderObject = this.GetPartFromColliderObject(rigCreatorCollider);
			if (partFromColliderObject != null)
			{
				partFromColliderObject.colliders.Remove(colliderDataFromColliderObject);
			}
		}
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x00018B38 File Offset: 0x00016D38
	internal void ColliderChanged(RigCreatorCollider rigCreatorCollider, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, float height, float radius)
	{
		RigCreatorColliderData rigCreatorColliderData = this.GetColliderDataFromColliderObject(rigCreatorCollider);
		if (rigCreatorColliderData == null)
		{
			RigPart rigPart = this.GetPartFromColliderObject(rigCreatorCollider);
			if (rigPart == null)
			{
				rigPart = this.FindPartFromName(rigCreatorCollider.transform.parent.name);
			}
			if (rigPart == null)
			{
				return;
			}
			rigCreatorColliderData = new RigCreatorColliderData();
			rigCreatorColliderData.colliderObject = rigCreatorCollider;
			rigPart.colliders.Add(rigCreatorColliderData);
		}
		rigCreatorColliderData.colliderPosition = localPosition;
		rigCreatorColliderData.colliderRotation = localRotation;
		rigCreatorColliderData.colliderScale = localScale;
		rigCreatorColliderData.height = height;
		rigCreatorColliderData.radius = radius;
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x00018BB5 File Offset: 0x00016DB5
	internal void RigidbodyChanged(RigCreatorRigidbody rigObject, float mass)
	{
		this.GetPartFromRigObject(rigObject).mass = mass;
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00018BC4 File Offset: 0x00016DC4
	internal void JointChanged(RigCreatorJoint jointObject, float spring)
	{
		this.GetPartFromJointObject(jointObject).spring = spring;
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x00018BD4 File Offset: 0x00016DD4
	private RigPart FindPartFromName(string targetName)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].partType.ToString() == targetName)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x04000329 RID: 809
	[HideInInspector]
	public bool aboutToClear;

	// Token: 0x0400032A RID: 810
	public float springMultiplier = 1f;

	// Token: 0x0400032B RID: 811
	public List<RigPart> parts;
}
