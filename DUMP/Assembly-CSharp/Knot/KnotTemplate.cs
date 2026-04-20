using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using pworld.Scripts.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Knot
{
	// Token: 0x020003B3 RID: 947
	[ExecuteInEditMode]
	public class KnotTemplate : MonoBehaviour, ISerializationCallbackReceiver
	{
		// Token: 0x06001953 RID: 6483 RVA: 0x0007FDBA File Offset: 0x0007DFBA
		private void Awake()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this.SplineToLineRenderer();
			this.LineRendererToMeshColliders();
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x0007FDD0 File Offset: 0x0007DFD0
		private void Update()
		{
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x0007FDD2 File Offset: 0x0007DFD2
		public void OnBeforeSerialize()
		{
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x0007FDD4 File Offset: 0x0007DFD4
		public void OnAfterDeserialize()
		{
			this.Register();
		}

		// Token: 0x06001957 RID: 6487 RVA: 0x0007FDDC File Offset: 0x0007DFDC
		public void SplineToLineRenderer()
		{
			if (this.splineContainer == null)
			{
				return;
			}
			List<Vector3> list = new List<Vector3>();
			float num = 1f / (float)this.lr.positionCount;
			List<Keyframe> list2 = new List<Keyframe>();
			this.lr.transform.localPosition = Vector3.zero;
			this.lr.transform.localRotation = Quaternion.identity;
			for (int i = 0; i < this.lr.positionCount; i++)
			{
				float num2 = num * (float)i;
				float3 @float = this.splineContainer.Spline.EvaluatePosition(num2);
				float num3 = @float.z + this.splineContainer.transform.localPosition.z;
				num3 *= num3;
				float magnitude = this.splineContainer.transform.TransformVector(Vector3.one).magnitude;
				num3 *= magnitude;
				@float = this.splineContainer.transform.TransformPoint(@float.PToV3());
				@float = this.lr.transform.InverseTransformPoint(@float);
				list.Add(@float.PToV3().xyn(-num2 * 0.1f));
				list2.Add(new Keyframe(num2, Mathf.Max(this.minWidth, num3 * this.widthMul)));
			}
			this.lr.widthCurve = new AnimationCurve
			{
				keys = list2.ToArray()
			};
			this.lr.SetPositions(list.ToArray());
		}

		// Token: 0x06001958 RID: 6488 RVA: 0x0007FF6C File Offset: 0x0007E16C
		private void LineRendererToMeshColliders()
		{
			Debug.Log("LineRendererToMeshColliders");
			this.lineMesh = new Mesh();
			this.lr.BakeMesh(this.lineMesh, Camera.main, true);
			this.colliderRoot.KillAllChildren(true, false, false);
			int num = 0;
			while (num < Mathf.FloorToInt((float)this.lineMesh.triangles.Length / 3f) / 2 && num <= this.lineMesh.triangles.Length)
			{
				GameObject gameObject = new GameObject(string.Format("{0}", num));
				List<int> list = new List<int>();
				List<Vector3> list2 = new List<Vector3>();
				gameObject.transform.parent = this.colliderRoot;
				gameObject.transform.localPosition = 0.ToVec();
				for (int i = 0; i < 2; i++)
				{
					int num2 = num * 2 + i;
					for (int j = 0; j < 3; j++)
					{
						Vector3 item = this.lineMesh.vertices[this.lineMesh.triangles[num2 * 3 + j]];
						list2.Add(item);
						list.Add(list2.Count - 1);
					}
				}
				Mesh mesh = new Mesh();
				mesh.vertices = list2.ToArray();
				mesh.triangles = list.ToArray();
				mesh.RecalculateAll();
				gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
				num++;
			}
		}

		// Token: 0x06001959 RID: 6489 RVA: 0x000800CE File Offset: 0x0007E2CE
		private void Register()
		{
			if (this.registered)
			{
				return;
			}
			this.registered = true;
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x00080100 File Offset: 0x0007E300
		[CompilerGenerated]
		private void <Register>g__EditorSplineUtilityOnAfterSplineWasModified|16_0(Spline spline)
		{
			try
			{
				if (base.gameObject == null)
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
			}
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (spline != this.splineContainer.Spline)
			{
				return;
			}
			this.SplineToLineRenderer();
		}

		// Token: 0x04001717 RID: 5911
		public float widthMul = 0.1f;

		// Token: 0x04001718 RID: 5912
		public float minWidth = 0.001f;

		// Token: 0x04001719 RID: 5913
		public LineRenderer lr;

		// Token: 0x0400171A RID: 5914
		public SplineContainer splineContainer;

		// Token: 0x0400171B RID: 5915
		public Transform colliderRoot;

		// Token: 0x0400171C RID: 5916
		private Mesh lineMesh;

		// Token: 0x0400171D RID: 5917
		private float counter;

		// Token: 0x0400171E RID: 5918
		[NonSerialized]
		private bool registered;

		// Token: 0x0400171F RID: 5919
		public bool editorRefresh;

		// Token: 0x04001720 RID: 5920
		private float timeToRefresh;
	}
}
