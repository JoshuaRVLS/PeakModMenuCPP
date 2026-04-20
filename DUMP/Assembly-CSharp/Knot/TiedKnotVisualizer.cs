using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Knot
{
	// Token: 0x020003B6 RID: 950
	public class TiedKnotVisualizer : MonoBehaviour
	{
		// Token: 0x06001963 RID: 6499 RVA: 0x00080652 File Offset: 0x0007E852
		private void Awake()
		{
			this.lr = base.GetComponent<LineRenderer>();
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x00080660 File Offset: 0x0007E860
		public void Refresh()
		{
			this.Visualize(this.knot);
		}

		// Token: 0x06001965 RID: 6501 RVA: 0x00080670 File Offset: 0x0007E870
		public void Go()
		{
			foreach (TiedKnotVisualizer.KnotPart knotPart in this.knot)
			{
				Debug.Log(string.Format("Quality: {0}, Position: {1}", knotPart.quality, knotPart.position));
			}
		}

		// Token: 0x06001966 RID: 6502 RVA: 0x000806E4 File Offset: 0x0007E8E4
		public void Visualize(List<TiedKnotVisualizer.KnotPart> knot)
		{
			this.knot = knot;
			List<Vector3> list = (from knotPoint in knot
			select knotPoint.position).ToList<Vector3>();
			if (!this.splineIt)
			{
				this.lr.positionCount = list.Count;
				this.lr.SetPositions(list.ToArray());
				return;
			}
			Spline spline = new Spline();
			spline.Knots = (from knotPoint in list
			select new BezierKnot(knotPoint)).ToArray<BezierKnot>();
			List<Vector3> list2 = new List<Vector3>();
			float num = 1f / (float)this.count;
			for (int i = 0; i < this.count; i++)
			{
				float t = num * (float)i;
				float3 me = spline.EvaluatePosition(t);
				list2.Add(me.PToV3());
			}
			this.lr.positionCount = this.count;
			this.lr.SetPositions(list2.ToArray());
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x000807ED File Offset: 0x0007E9ED
		private void Start()
		{
		}

		// Token: 0x06001968 RID: 6504 RVA: 0x000807EF File Offset: 0x0007E9EF
		private void Update()
		{
		}

		// Token: 0x06001969 RID: 6505 RVA: 0x000807F1 File Offset: 0x0007E9F1
		public void Clear()
		{
			this.knot.Clear();
			this.Refresh();
		}

		// Token: 0x04001730 RID: 5936
		private LineRenderer lr;

		// Token: 0x04001731 RID: 5937
		public int count;

		// Token: 0x04001732 RID: 5938
		public bool splineIt;

		// Token: 0x04001733 RID: 5939
		public List<TiedKnotVisualizer.KnotPart> knot = new List<TiedKnotVisualizer.KnotPart>();

		// Token: 0x02000564 RID: 1380
		public struct KnotPart
		{
			// Token: 0x06001FC0 RID: 8128 RVA: 0x00090034 File Offset: 0x0008E234
			public KnotPart(bool quality, Vector3 position, int part)
			{
				this.quality = quality;
				this.position = position;
				this.part = part;
			}

			// Token: 0x04001D37 RID: 7479
			public bool quality;

			// Token: 0x04001D38 RID: 7480
			public Vector3 position;

			// Token: 0x04001D39 RID: 7481
			public int part;
		}
	}
}
