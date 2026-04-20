using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200009F RID: 159
[AddComponentMenu("Dynamic Bone/Dynamic Bone")]
public class DynamicBone : MonoBehaviour
{
	// Token: 0x06000625 RID: 1573 RVA: 0x00022FAD File Offset: 0x000211AD
	private void Start()
	{
		if (!this.m_Root)
		{
			this.m_Root = base.transform;
		}
		this.SetupParticles();
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x00022FCE File Offset: 0x000211CE
	private void FixedUpdate()
	{
		if (this.m_UpdateMode == DynamicBone.UpdateMode.AnimatePhysics)
		{
			this.PreUpdate();
		}
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x00022FDF File Offset: 0x000211DF
	private void Update()
	{
		if (this.m_UpdateMode != DynamicBone.UpdateMode.AnimatePhysics)
		{
			this.PreUpdate();
		}
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x00022FF0 File Offset: 0x000211F0
	private void LateUpdate()
	{
		if (this.m_DistantDisable)
		{
			this.CheckDistance();
		}
		if (this.m_Weight > 0f && (!this.m_DistantDisable || !this.m_DistantDisabled))
		{
			float t = (this.m_UpdateMode == DynamicBone.UpdateMode.UnscaledTime) ? Time.unscaledDeltaTime : Time.deltaTime;
			this.UpdateDynamicBones(t);
		}
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x00023045 File Offset: 0x00021245
	private void PreUpdate()
	{
		if (this.m_Weight > 0f && (!this.m_DistantDisable || !this.m_DistantDisabled))
		{
			this.InitTransforms();
		}
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x0002306C File Offset: 0x0002126C
	private void CheckDistance()
	{
		Transform transform = this.m_ReferenceObject;
		if (transform == null && Camera.main != null)
		{
			transform = Camera.main.transform;
		}
		if (transform != null)
		{
			bool flag = (transform.position - base.transform.position).sqrMagnitude > this.m_DistanceToObject * this.m_DistanceToObject;
			if (flag != this.m_DistantDisabled)
			{
				if (!flag)
				{
					this.ResetParticlesPosition();
				}
				this.m_DistantDisabled = flag;
			}
		}
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x000230F1 File Offset: 0x000212F1
	private void OnEnable()
	{
		this.ResetParticlesPosition();
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x000230F9 File Offset: 0x000212F9
	private void OnDisable()
	{
		this.InitTransforms();
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x00023104 File Offset: 0x00021304
	private void OnValidate()
	{
		this.m_UpdateRate = Mathf.Max(this.m_UpdateRate, 0f);
		this.m_Damping = Mathf.Clamp01(this.m_Damping);
		this.m_Elasticity = Mathf.Clamp01(this.m_Elasticity);
		this.m_Stiffness = Mathf.Clamp01(this.m_Stiffness);
		this.m_Inert = Mathf.Clamp01(this.m_Inert);
		this.m_Friction = Mathf.Clamp01(this.m_Friction);
		this.m_Radius = Mathf.Max(this.m_Radius, 0f);
		if (Application.isEditor && Application.isPlaying)
		{
			this.InitTransforms();
			this.SetupParticles();
		}
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x000231AC File Offset: 0x000213AC
	private void OnDrawGizmosSelected()
	{
		if (!base.enabled || this.m_Root == null)
		{
			return;
		}
		if (Application.isEditor && !Application.isPlaying && base.transform.hasChanged)
		{
			this.InitTransforms();
			this.SetupParticles();
		}
		Gizmos.color = Color.white;
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
				Gizmos.DrawLine(particle.m_Position, particle2.m_Position);
			}
			if (particle.m_Radius > 0f)
			{
				Gizmos.DrawWireSphere(particle.m_Position, particle.m_Radius * this.m_ObjectScale);
			}
		}
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x00023275 File Offset: 0x00021475
	public void SetWeight(float w)
	{
		if (this.m_Weight != w)
		{
			if (w == 0f)
			{
				this.InitTransforms();
			}
			else if (this.m_Weight == 0f)
			{
				this.ResetParticlesPosition();
			}
			this.m_Weight = w;
		}
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x000232AA File Offset: 0x000214AA
	public float GetWeight()
	{
		return this.m_Weight;
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x000232B4 File Offset: 0x000214B4
	private void UpdateDynamicBones(float t)
	{
		if (this.m_Root == null)
		{
			return;
		}
		this.m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
		this.m_ObjectMove = base.transform.position - this.m_ObjectPrevPosition;
		this.m_ObjectPrevPosition = base.transform.position;
		int num = 1;
		float timeVar = 1f;
		if (this.m_UpdateMode == DynamicBone.UpdateMode.Default)
		{
			if (this.m_UpdateRate > 0f)
			{
				timeVar = Time.deltaTime * this.m_UpdateRate;
			}
			else
			{
				timeVar = Time.deltaTime;
			}
		}
		else if (this.m_UpdateRate > 0f)
		{
			float num2 = 1f / this.m_UpdateRate;
			this.m_Time += t;
			num = 0;
			while (this.m_Time >= num2)
			{
				this.m_Time -= num2;
				if (++num >= 3)
				{
					this.m_Time = 0f;
					break;
				}
			}
		}
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				this.UpdateParticles1(timeVar);
				this.UpdateParticles2(timeVar);
				this.m_ObjectMove = Vector3.zero;
			}
		}
		else
		{
			this.SkipUpdateParticles();
		}
		this.ApplyParticlesToTransforms();
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x000233DC File Offset: 0x000215DC
	public void SetupParticles()
	{
		this.m_Particles.Clear();
		if (this.m_Root == null)
		{
			return;
		}
		this.m_LocalGravity = this.m_Root.InverseTransformDirection(this.m_Gravity);
		this.m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
		this.m_ObjectPrevPosition = base.transform.position;
		this.m_ObjectMove = Vector3.zero;
		this.m_BoneTotalLength = 0f;
		this.AppendParticles(this.m_Root, -1, 0f);
		this.UpdateParameters();
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00023474 File Offset: 0x00021674
	private void AppendParticles(Transform b, int parentIndex, float boneLength)
	{
		DynamicBone.Particle particle = new DynamicBone.Particle();
		particle.m_Transform = b;
		particle.m_ParentIndex = parentIndex;
		if (b != null)
		{
			particle.m_Position = (particle.m_PrevPosition = b.position);
			particle.m_InitLocalPosition = b.localPosition;
			particle.m_InitLocalRotation = b.localRotation;
		}
		else
		{
			Transform transform = this.m_Particles[parentIndex].m_Transform;
			if (this.m_EndLength > 0f)
			{
				Transform parent = transform.parent;
				if (parent != null)
				{
					particle.m_EndOffset = transform.InverseTransformPoint(transform.position * 2f - parent.position) * this.m_EndLength;
				}
				else
				{
					particle.m_EndOffset = new Vector3(this.m_EndLength, 0f, 0f);
				}
			}
			else
			{
				particle.m_EndOffset = transform.InverseTransformPoint(base.transform.TransformDirection(this.m_EndOffset) + transform.position);
			}
			particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
		}
		if (parentIndex >= 0)
		{
			boneLength += (this.m_Particles[parentIndex].m_Transform.position - particle.m_Position).magnitude;
			particle.m_BoneLength = boneLength;
			this.m_BoneTotalLength = Mathf.Max(this.m_BoneTotalLength, boneLength);
		}
		int count = this.m_Particles.Count;
		this.m_Particles.Add(particle);
		if (b != null)
		{
			for (int i = 0; i < b.childCount; i++)
			{
				Transform child = b.GetChild(i);
				bool flag = false;
				if (this.m_Exclusions != null)
				{
					flag = this.m_Exclusions.Contains(child);
				}
				if (!flag)
				{
					this.AppendParticles(child, count, boneLength);
				}
				else if (this.m_EndLength > 0f || this.m_EndOffset != Vector3.zero)
				{
					this.AppendParticles(null, count, boneLength);
				}
			}
			if (b.childCount == 0 && (this.m_EndLength > 0f || this.m_EndOffset != Vector3.zero))
			{
				this.AppendParticles(null, count, boneLength);
			}
		}
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x000236A8 File Offset: 0x000218A8
	public void UpdateParameters()
	{
		if (this.m_Root == null)
		{
			return;
		}
		this.m_LocalGravity = this.m_Root.InverseTransformDirection(this.m_Gravity);
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			particle.m_Damping = this.m_Damping;
			particle.m_Elasticity = this.m_Elasticity;
			particle.m_Stiffness = this.m_Stiffness;
			particle.m_Inert = this.m_Inert;
			particle.m_Friction = this.m_Friction;
			particle.m_Radius = this.m_Radius;
			if (this.m_BoneTotalLength > 0f)
			{
				float time = particle.m_BoneLength / this.m_BoneTotalLength;
				if (this.m_DampingDistrib != null && this.m_DampingDistrib.keys.Length != 0)
				{
					particle.m_Damping *= this.m_DampingDistrib.Evaluate(time);
				}
				if (this.m_ElasticityDistrib != null && this.m_ElasticityDistrib.keys.Length != 0)
				{
					particle.m_Elasticity *= this.m_ElasticityDistrib.Evaluate(time);
				}
				if (this.m_StiffnessDistrib != null && this.m_StiffnessDistrib.keys.Length != 0)
				{
					particle.m_Stiffness *= this.m_StiffnessDistrib.Evaluate(time);
				}
				if (this.m_InertDistrib != null && this.m_InertDistrib.keys.Length != 0)
				{
					particle.m_Inert *= this.m_InertDistrib.Evaluate(time);
				}
				if (this.m_FrictionDistrib != null && this.m_FrictionDistrib.keys.Length != 0)
				{
					particle.m_Friction *= this.m_FrictionDistrib.Evaluate(time);
				}
				if (this.m_RadiusDistrib != null && this.m_RadiusDistrib.keys.Length != 0)
				{
					particle.m_Radius *= this.m_RadiusDistrib.Evaluate(time);
				}
			}
			particle.m_Damping = Mathf.Clamp01(particle.m_Damping);
			particle.m_Elasticity = Mathf.Clamp01(particle.m_Elasticity);
			particle.m_Stiffness = Mathf.Clamp01(particle.m_Stiffness);
			particle.m_Inert = Mathf.Clamp01(particle.m_Inert);
			particle.m_Friction = Mathf.Clamp01(particle.m_Friction);
			particle.m_Radius = Mathf.Max(particle.m_Radius, 0f);
		}
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x000238F0 File Offset: 0x00021AF0
	private void InitTransforms()
	{
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_Transform != null)
			{
				particle.m_Transform.localPosition = particle.m_InitLocalPosition;
				particle.m_Transform.localRotation = particle.m_InitLocalRotation;
			}
		}
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x00023950 File Offset: 0x00021B50
	private void ResetParticlesPosition()
	{
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_Transform != null)
			{
				particle.m_Position = (particle.m_PrevPosition = particle.m_Transform.position);
			}
			else
			{
				Transform transform = this.m_Particles[particle.m_ParentIndex].m_Transform;
				particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
			}
			particle.m_isCollide = false;
		}
		this.m_ObjectPrevPosition = base.transform.position;
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x000239F8 File Offset: 0x00021BF8
	private void UpdateParticles1(float timeVar)
	{
		Vector3 vector = this.m_Gravity;
		Vector3 normalized = this.m_Gravity.normalized;
		Vector3 lhs = this.m_Root.TransformDirection(this.m_LocalGravity);
		Vector3 b = normalized * Mathf.Max(Vector3.Dot(lhs, normalized), 0f);
		vector -= b;
		vector = (vector + this.m_Force) * (this.m_ObjectScale * timeVar);
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				Vector3 a = particle.m_Position - particle.m_PrevPosition;
				Vector3 b2 = this.m_ObjectMove * particle.m_Inert;
				particle.m_PrevPosition = particle.m_Position + b2;
				float num = particle.m_Damping;
				if (particle.m_isCollide)
				{
					num += particle.m_Friction;
					if (num > 1f)
					{
						num = 1f;
					}
					particle.m_isCollide = false;
				}
				particle.m_Position += a * (1f - num) + vector + b2;
			}
			else
			{
				particle.m_PrevPosition = particle.m_Position;
				particle.m_Position = particle.m_Transform.position;
			}
		}
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x00023B68 File Offset: 0x00021D68
	private void UpdateParticles2(float timeVar)
	{
		Plane plane = default(Plane);
		for (int i = 1; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
			float magnitude;
			if (particle.m_Transform != null)
			{
				magnitude = (particle2.m_Transform.position - particle.m_Transform.position).magnitude;
			}
			else
			{
				magnitude = particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude;
			}
			float num = Mathf.Lerp(1f, particle.m_Stiffness, this.m_Weight);
			if (num > 0f || particle.m_Elasticity > 0f)
			{
				Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
				localToWorldMatrix.SetColumn(3, particle2.m_Position);
				Vector3 a;
				if (particle.m_Transform != null)
				{
					a = localToWorldMatrix.MultiplyPoint3x4(particle.m_Transform.localPosition);
				}
				else
				{
					a = localToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset);
				}
				Vector3 a2 = a - particle.m_Position;
				particle.m_Position += a2 * (particle.m_Elasticity * timeVar);
				if (num > 0f)
				{
					a2 = a - particle.m_Position;
					float magnitude2 = a2.magnitude;
					float num2 = magnitude * (1f - num) * 2f;
					if (magnitude2 > num2)
					{
						particle.m_Position += a2 * ((magnitude2 - num2) / magnitude2);
					}
				}
			}
			if (this.m_Colliders != null)
			{
				float particleRadius = particle.m_Radius * this.m_ObjectScale;
				for (int j = 0; j < this.m_Colliders.Count; j++)
				{
					DynamicBoneColliderBase dynamicBoneColliderBase = this.m_Colliders[j];
					if (dynamicBoneColliderBase != null && dynamicBoneColliderBase.enabled)
					{
						particle.m_isCollide |= dynamicBoneColliderBase.Collide(ref particle.m_Position, particleRadius);
					}
				}
			}
			if (this.m_FreezeAxis != DynamicBone.FreezeAxis.None)
			{
				switch (this.m_FreezeAxis)
				{
				case DynamicBone.FreezeAxis.X:
					plane.SetNormalAndPosition(particle2.m_Transform.right, particle2.m_Position);
					break;
				case DynamicBone.FreezeAxis.Y:
					plane.SetNormalAndPosition(particle2.m_Transform.up, particle2.m_Position);
					break;
				case DynamicBone.FreezeAxis.Z:
					plane.SetNormalAndPosition(particle2.m_Transform.forward, particle2.m_Position);
					break;
				}
				particle.m_Position -= plane.normal * plane.GetDistanceToPoint(particle.m_Position);
			}
			Vector3 a3 = particle2.m_Position - particle.m_Position;
			float magnitude3 = a3.magnitude;
			if (magnitude3 > 0f)
			{
				particle.m_Position += a3 * ((magnitude3 - magnitude) / magnitude3);
			}
		}
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x00023E74 File Offset: 0x00022074
	private void SkipUpdateParticles()
	{
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				particle.m_PrevPosition += this.m_ObjectMove;
				particle.m_Position += this.m_ObjectMove;
				DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
				float magnitude;
				if (particle.m_Transform != null)
				{
					magnitude = (particle2.m_Transform.position - particle.m_Transform.position).magnitude;
				}
				else
				{
					magnitude = particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude;
				}
				float num = Mathf.Lerp(1f, particle.m_Stiffness, this.m_Weight);
				if (num > 0f)
				{
					Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
					localToWorldMatrix.SetColumn(3, particle2.m_Position);
					Vector3 a;
					if (particle.m_Transform != null)
					{
						a = localToWorldMatrix.MultiplyPoint3x4(particle.m_Transform.localPosition);
					}
					else
					{
						a = localToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset);
					}
					Vector3 a2 = a - particle.m_Position;
					float magnitude2 = a2.magnitude;
					float num2 = magnitude * (1f - num) * 2f;
					if (magnitude2 > num2)
					{
						particle.m_Position += a2 * ((magnitude2 - num2) / magnitude2);
					}
				}
				Vector3 a3 = particle2.m_Position - particle.m_Position;
				float magnitude3 = a3.magnitude;
				if (magnitude3 > 0f)
				{
					particle.m_Position += a3 * ((magnitude3 - magnitude) / magnitude3);
				}
			}
			else
			{
				particle.m_PrevPosition = particle.m_Position;
				particle.m_Position = particle.m_Transform.position;
			}
		}
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00024079 File Offset: 0x00022279
	private static Vector3 MirrorVector(Vector3 v, Vector3 axis)
	{
		return v - axis * (Vector3.Dot(v, axis) * 2f);
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x00024094 File Offset: 0x00022294
	private void ApplyParticlesToTransforms()
	{
		for (int i = 1; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
			if (particle2.m_Transform.childCount <= 1)
			{
				Vector3 direction;
				if (particle.m_Transform != null)
				{
					direction = particle.m_Transform.localPosition;
				}
				else
				{
					direction = particle.m_EndOffset;
				}
				Vector3 toDirection = particle.m_Position - particle2.m_Position;
				Quaternion lhs = Quaternion.FromToRotation(particle2.m_Transform.TransformDirection(direction), toDirection);
				particle2.m_Transform.rotation = lhs * particle2.m_Transform.rotation;
			}
			if (particle.m_Transform != null)
			{
				particle.m_Transform.position = particle.m_Position;
			}
		}
	}

	// Token: 0x04000634 RID: 1588
	[Tooltip("The root of the transform hierarchy to apply physics.")]
	public Transform m_Root;

	// Token: 0x04000635 RID: 1589
	[Tooltip("Internal physics simulation rate.")]
	public float m_UpdateRate = 60f;

	// Token: 0x04000636 RID: 1590
	public DynamicBone.UpdateMode m_UpdateMode = DynamicBone.UpdateMode.Default;

	// Token: 0x04000637 RID: 1591
	[Tooltip("How much the bones slowed down.")]
	[Range(0f, 1f)]
	public float m_Damping = 0.1f;

	// Token: 0x04000638 RID: 1592
	public AnimationCurve m_DampingDistrib;

	// Token: 0x04000639 RID: 1593
	[Tooltip("How much the force applied to return each bone to original orientation.")]
	[Range(0f, 1f)]
	public float m_Elasticity = 0.1f;

	// Token: 0x0400063A RID: 1594
	public AnimationCurve m_ElasticityDistrib;

	// Token: 0x0400063B RID: 1595
	[Tooltip("How much bone's original orientation are preserved.")]
	[Range(0f, 1f)]
	public float m_Stiffness = 0.1f;

	// Token: 0x0400063C RID: 1596
	public AnimationCurve m_StiffnessDistrib;

	// Token: 0x0400063D RID: 1597
	[Tooltip("How much character's position change is ignored in physics simulation.")]
	[Range(0f, 1f)]
	public float m_Inert;

	// Token: 0x0400063E RID: 1598
	public AnimationCurve m_InertDistrib;

	// Token: 0x0400063F RID: 1599
	[Tooltip("How much the bones slowed down when collide.")]
	public float m_Friction;

	// Token: 0x04000640 RID: 1600
	public AnimationCurve m_FrictionDistrib;

	// Token: 0x04000641 RID: 1601
	[Tooltip("Each bone can be a sphere to collide with colliders. Radius describe sphere's size.")]
	public float m_Radius;

	// Token: 0x04000642 RID: 1602
	public AnimationCurve m_RadiusDistrib;

	// Token: 0x04000643 RID: 1603
	[Tooltip("If End Length is not zero, an extra bone is generated at the end of transform hierarchy.")]
	public float m_EndLength;

	// Token: 0x04000644 RID: 1604
	[Tooltip("If End Offset is not zero, an extra bone is generated at the end of transform hierarchy.")]
	public Vector3 m_EndOffset = Vector3.zero;

	// Token: 0x04000645 RID: 1605
	[Tooltip("The force apply to bones. Partial force apply to character's initial pose is cancelled out.")]
	public Vector3 m_Gravity = Vector3.zero;

	// Token: 0x04000646 RID: 1606
	[Tooltip("The force apply to bones.")]
	public Vector3 m_Force = Vector3.zero;

	// Token: 0x04000647 RID: 1607
	[Tooltip("Collider objects interact with the bones.")]
	public List<DynamicBoneColliderBase> m_Colliders;

	// Token: 0x04000648 RID: 1608
	[Tooltip("Bones exclude from physics simulation.")]
	public List<Transform> m_Exclusions;

	// Token: 0x04000649 RID: 1609
	[Tooltip("Constrain bones to move on specified plane.")]
	public DynamicBone.FreezeAxis m_FreezeAxis;

	// Token: 0x0400064A RID: 1610
	[Tooltip("Disable physics simulation automatically if character is far from camera or player.")]
	public bool m_DistantDisable;

	// Token: 0x0400064B RID: 1611
	public Transform m_ReferenceObject;

	// Token: 0x0400064C RID: 1612
	public float m_DistanceToObject = 20f;

	// Token: 0x0400064D RID: 1613
	private Vector3 m_LocalGravity = Vector3.zero;

	// Token: 0x0400064E RID: 1614
	private Vector3 m_ObjectMove = Vector3.zero;

	// Token: 0x0400064F RID: 1615
	private Vector3 m_ObjectPrevPosition = Vector3.zero;

	// Token: 0x04000650 RID: 1616
	private float m_BoneTotalLength;

	// Token: 0x04000651 RID: 1617
	private float m_ObjectScale = 1f;

	// Token: 0x04000652 RID: 1618
	private float m_Time;

	// Token: 0x04000653 RID: 1619
	private float m_Weight = 1f;

	// Token: 0x04000654 RID: 1620
	private bool m_DistantDisabled;

	// Token: 0x04000655 RID: 1621
	private List<DynamicBone.Particle> m_Particles = new List<DynamicBone.Particle>();

	// Token: 0x02000444 RID: 1092
	public enum UpdateMode
	{
		// Token: 0x040018BA RID: 6330
		Normal,
		// Token: 0x040018BB RID: 6331
		AnimatePhysics,
		// Token: 0x040018BC RID: 6332
		UnscaledTime,
		// Token: 0x040018BD RID: 6333
		Default
	}

	// Token: 0x02000445 RID: 1093
	public enum FreezeAxis
	{
		// Token: 0x040018BF RID: 6335
		None,
		// Token: 0x040018C0 RID: 6336
		X,
		// Token: 0x040018C1 RID: 6337
		Y,
		// Token: 0x040018C2 RID: 6338
		Z
	}

	// Token: 0x02000446 RID: 1094
	private class Particle
	{
		// Token: 0x040018C3 RID: 6339
		public Transform m_Transform;

		// Token: 0x040018C4 RID: 6340
		public int m_ParentIndex = -1;

		// Token: 0x040018C5 RID: 6341
		public float m_Damping;

		// Token: 0x040018C6 RID: 6342
		public float m_Elasticity;

		// Token: 0x040018C7 RID: 6343
		public float m_Stiffness;

		// Token: 0x040018C8 RID: 6344
		public float m_Inert;

		// Token: 0x040018C9 RID: 6345
		public float m_Friction;

		// Token: 0x040018CA RID: 6346
		public float m_Radius;

		// Token: 0x040018CB RID: 6347
		public float m_BoneLength;

		// Token: 0x040018CC RID: 6348
		public bool m_isCollide;

		// Token: 0x040018CD RID: 6349
		public Vector3 m_Position = Vector3.zero;

		// Token: 0x040018CE RID: 6350
		public Vector3 m_PrevPosition = Vector3.zero;

		// Token: 0x040018CF RID: 6351
		public Vector3 m_EndOffset = Vector3.zero;

		// Token: 0x040018D0 RID: 6352
		public Vector3 m_InitLocalPosition = Vector3.zero;

		// Token: 0x040018D1 RID: 6353
		public Quaternion m_InitLocalRotation = Quaternion.identity;
	}
}
