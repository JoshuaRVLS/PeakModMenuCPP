using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Peak.Math;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000175 RID: 373
public class RopeBoneVisualizer : MonoBehaviour
{
	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000C1C RID: 3100 RVA: 0x00040E78 File Offset: 0x0003F078
	// (set) Token: 0x06000C1D RID: 3101 RVA: 0x00040E80 File Offset: 0x0003F080
	public Transform StartTransform { get; set; }

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000C1E RID: 3102 RVA: 0x00040E89 File Offset: 0x0003F089
	// (set) Token: 0x06000C1F RID: 3103 RVA: 0x00040E91 File Offset: 0x0003F091
	public Optionable<bool> ManuallyUpdateNextFrame { get; set; }

	// Token: 0x06000C20 RID: 3104 RVA: 0x00040E9C File Offset: 0x0003F09C
	private void Awake()
	{
		this.view = base.GetComponentInParent<PhotonView>();
		this.rope = base.GetComponent<Rope>();
		this.bones = this.boneRoot.PGetComponentsInChildrenButNotMe(false).ToList<Transform>();
		this.bones.Reverse();
		this.meshRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>();
		this.CheckVisible();
		this.ghostMaterial = Object.Instantiate<Material>(this.ghostMaterial);
		this.ropeMaterial = Object.Instantiate<Material>(this.ropeMaterial);
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x00040F17 File Offset: 0x0003F117
	private void OnDestroy()
	{
		Object.Destroy(this.ropeMaterial);
		Object.Destroy(this.ghostMaterial);
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x00040F30 File Offset: 0x0003F130
	private void LateUpdate()
	{
		RopeBoneVisualizer.<>c__DisplayClass22_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this.CheckVisible();
		if (this.targetPoints.Count != this.remoteRenderingPoints.Count)
		{
			Debug.LogError("Target points count mismatch");
			return;
		}
		float num = 1f / (float)PhotonNetwork.SerializationRate;
		this.sinceLastPackage += Time.deltaTime;
		if (this.ManuallyUpdateNextFrame.IsSome)
		{
			if (!this.ManuallyUpdateNextFrame.Value)
			{
				return;
			}
			this.ManuallyUpdateNextFrame = Optionable<bool>.Some(false);
		}
		float num2 = this.sinceLastPackage / num;
		for (int i = 0; i < this.remoteRenderingPoints.Count; i++)
		{
			Vector3 v = Vector3.Lerp(this.remoteRenderingPoints[i].position, this.targetPoints[i].position, num2 * 0.5f);
			Quaternion rotation = Quaternion.Lerp(this.remoteRenderingPoints[i].rotation, this.targetPoints[i].rotation, num2 * 0.5f);
			this.remoteRenderingPoints[i] = new RopeSyncData.SegmentData
			{
				position = v,
				rotation = rotation
			};
		}
		List<RopeSyncData.SegmentData> list;
		if (!this.view.IsMine)
		{
			list = this.remoteRenderingPoints;
		}
		else
		{
			list = (from transform1 in this.rope.GetRopeSegments()
			select new RopeSyncData.SegmentData
			{
				position = transform1.position,
				rotation = transform1.rotation
			}).ToList<RopeSyncData.SegmentData>();
		}
		List<RopeSyncData.SegmentData> list2 = list;
		CS$<>8__locals1.positions = new List<RopeSyncData.SegmentData>();
		if (this.StartTransform != null && this.rope.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			CS$<>8__locals1.positions.Add(new RopeSyncData.SegmentData
			{
				position = this.StartTransform.position,
				rotation = this.StartTransform.rotation
			});
		}
		if (list2.Count > 0)
		{
			CS$<>8__locals1.positions.Add(list2[0]);
		}
		for (int j = list2.Count - 1; j >= 1; j--)
		{
			CS$<>8__locals1.positions.Add(list2[j]);
		}
		CS$<>8__locals1.positions.Reverse();
		this.meshRenderer.sharedMaterial.SetFloat("_RopeCutoff", (Mathf.Floor((float)CS$<>8__locals1.positions.Count) - this.segmentMod) * (1f / (float)(this.bones.Count - 1)));
		if (CS$<>8__locals1.positions.Count == 0)
		{
			return;
		}
		if (this.rope.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			this.<LateUpdate>g__RenderInSpool|22_2(ref CS$<>8__locals1);
		}
		else
		{
			this.<LateUpdate>g__RenderInNotSpool|22_1(ref CS$<>8__locals1);
		}
		this.<LateUpdate>g__RenderInSpool|22_2(ref CS$<>8__locals1);
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x000411FC File Offset: 0x0003F3FC
	public void OnDrawGizmosSelected()
	{
		foreach (Transform transform in this.bones)
		{
			DrawArrow.ForGizmo(transform.position, transform.up, Color.green, 0.25f);
			DrawArrow.ForGizmo(transform.position, transform.forward, Color.blue, 0.25f);
			DrawArrow.ForGizmo(transform.position, transform.right, Color.red, 0.25f);
		}
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x0004129C File Offset: 0x0003F49C
	private void CheckVisible()
	{
		this.meshRenderer.sharedMaterial = ((this.rope.attachmenState == Rope.ATTACHMENT.inSpool) ? this.ghostMaterial : this.ropeMaterial);
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x000412C8 File Offset: 0x0003F4C8
	public void SetData(RopeSyncData data)
	{
		if (this.rope.creatorLeft)
		{
			return;
		}
		this.sinceLastPackage = 0f;
		this.targetPoints = data.segments.ToList<RopeSyncData.SegmentData>();
		int num = data.segments.Length;
		int count = this.remoteRenderingPoints.Count;
		if (num < count)
		{
			int num2 = count - num;
			for (int i = 0; i < num2; i++)
			{
				this.remoteRenderingPoints.RemoveLast<RopeSyncData.SegmentData>();
			}
			return;
		}
		if (num > count)
		{
			int num3 = num - count;
			for (int j = 0; j < num3; j++)
			{
				int num4 = count + j;
				this.remoteRenderingPoints.Add(data.segments[num4]);
			}
		}
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x00041398 File Offset: 0x0003F598
	[CompilerGenerated]
	private void <LateUpdate>g__RenderInNotSpool|22_1(ref RopeBoneVisualizer.<>c__DisplayClass22_0 A_1)
	{
		int num = 0;
		for (int i = 0; i < this.bones.Count; i++)
		{
			Transform transform = this.bones[i];
			if (i > A_1.positions.Count - 1 && i > 0)
			{
				transform.gameObject.name = num.ToString();
				if (num == 0 && this.StartTransform != null)
				{
					transform.position = this.StartTransform.position;
					Vector3 vector = transform.position - this.bones[i - 1].position;
					if (!vector.NearZero())
					{
						transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up, -vector);
					}
					num++;
				}
				else
				{
					transform.position = this.bones[i - 1].position;
					transform.rotation = this.bones[i - 1].rotation;
					num++;
					transform.localScale = Vector3.zero;
				}
			}
			else
			{
				transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up + Vector3.forward * 0.05f, -A_1.positions[i].rotation.GetUp());
				transform.localScale = 1f.xxx();
				transform.position = A_1.positions[i].position.PToV3();
				transform.gameObject.name = i.ToString();
			}
		}
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x00041524 File Offset: 0x0003F724
	[CompilerGenerated]
	private void <LateUpdate>g__RenderInSpool|22_2(ref RopeBoneVisualizer.<>c__DisplayClass22_0 A_1)
	{
		int num = 0;
		for (int i = 0; i < this.bones.Count; i++)
		{
			Transform transform = this.bones[i];
			if (i > A_1.positions.Count - 3 && i > 0)
			{
				transform.gameObject.name = num.ToString();
				if (num == 0 && this.StartTransform != null)
				{
					transform.position = this.StartTransform.position;
					if (this.withRotOfStartPos)
					{
						transform.rotation = this.StartTransform.rotation;
					}
					else
					{
						Vector3 a = transform.position - this.bones[i - 1].position;
						transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up, -a);
					}
					num++;
				}
				else
				{
					transform.position = this.bones[i - 1].position;
					transform.rotation = this.bones[i - 1].rotation;
					num++;
					transform.localScale = Vector3.zero;
				}
			}
			else
			{
				transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up + Vector3.forward * 0.05f, -A_1.positions[i].rotation.GetUp());
				transform.localScale = 1f.xxx();
				transform.position = A_1.positions[i].position.PToV3();
				transform.gameObject.name = i.ToString();
			}
		}
	}

	// Token: 0x04000B17 RID: 2839
	public Material ghostMaterial;

	// Token: 0x04000B18 RID: 2840
	public Material ropeMaterial;

	// Token: 0x04000B19 RID: 2841
	public GameObject boneRoot;

	// Token: 0x04000B1A RID: 2842
	public List<Transform> bones;

	// Token: 0x04000B1B RID: 2843
	public float segmentMod = 1f;

	// Token: 0x04000B1C RID: 2844
	public bool withRotOfStartPos;

	// Token: 0x04000B1D RID: 2845
	private readonly List<RopeSyncData.SegmentData> remoteRenderingPoints = new List<RopeSyncData.SegmentData>();

	// Token: 0x04000B1E RID: 2846
	private SkinnedMeshRenderer meshRenderer;

	// Token: 0x04000B1F RID: 2847
	private Rope rope;

	// Token: 0x04000B20 RID: 2848
	private float sinceLastPackage;

	// Token: 0x04000B21 RID: 2849
	[NonSerialized]
	private List<RopeSyncData.SegmentData> targetPoints = new List<RopeSyncData.SegmentData>();

	// Token: 0x04000B22 RID: 2850
	private PhotonView view;
}
