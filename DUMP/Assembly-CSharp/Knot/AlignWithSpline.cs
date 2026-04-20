using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Splines;

namespace Knot
{
	// Token: 0x020003B1 RID: 945
	public class AlignWithSpline : MonoBehaviour
	{
		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06001940 RID: 6464 RVA: 0x0007F430 File Offset: 0x0007D630
		public float KnotStepSize
		{
			get
			{
				return this.knotProgressRange * 2f;
			}
		}

		// Token: 0x06001941 RID: 6465 RVA: 0x0007F440 File Offset: 0x0007D640
		public void DistanceToSpline(Vector3 position, out float closest, out float atSplineProgress)
		{
			position = position.xyo();
			int num = 200;
			float num2 = 1f / (float)num;
			closest = float.MaxValue;
			atSplineProgress = 0f;
			for (int i = 0; i < num; i++)
			{
				float num3 = num2 * (float)i;
				Vector3 vector = this.splineContainer.Spline.EvaluatePosition(num3).PToV3().xyo() - position;
				if (vector.magnitude < closest)
				{
					closest = vector.magnitude;
					atSplineProgress = num3;
				}
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06001942 RID: 6466 RVA: 0x0007F4BD File Offset: 0x0007D6BD
		public Vector2 KnotProgressRangeRelation
		{
			get
			{
				return this.knotProgressRangeRelation * this.knotProgressRange;
			}
		}

		// Token: 0x06001943 RID: 6467 RVA: 0x0007F4D0 File Offset: 0x0007D6D0
		private void EvaluateKnot(AlignWithSpline.TiedKnot tiedKnot)
		{
			float templateProgress = tiedKnot.knotPoints[0].templateProgress;
			Vector2 vector = this.KnotProgressRangeRelation;
			vector.x += this.knotProgress;
			vector.y += this.knotProgress;
			Vector2 vector2 = vector;
			vector2.x += this.KnotStepSize;
			vector2.y += this.KnotStepSize;
			if (templateProgress > vector.y && templateProgress > vector2.y)
			{
				Debug.LogError("");
			}
			float x = vector.x;
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x0007F570 File Offset: 0x0007D770
		private void TieRope2()
		{
			Plane plane = new Plane(Camera.main.transform.forward, this.splineContainer.transform.position);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float d;
			if (!plane.Raycast(ray, out d))
			{
				return;
			}
			Vector3 me = ray.direction * d + ray.origin;
			this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint
			{
				position = me.xyo(),
				templateProgress = this.lastKnotPointProgress,
				inside = false
			});
		}

		// Token: 0x06001945 RID: 6469 RVA: 0x0007F610 File Offset: 0x0007D810
		private void TieRope()
		{
			Plane plane = new Plane(Camera.main.transform.forward, this.splineContainer.transform.position);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] array = Physics.RaycastAll(ray);
			if (array.Length != 0)
			{
				IOrderedEnumerable<RaycastHit> orderedEnumerable = from h in array
				orderby Mathf.Abs(h.textureCoord.x - this.lastKnotPointProgress)
				select h;
				foreach (RaycastHit raycastHit in orderedEnumerable)
				{
					Debug.Log(string.Format("{0} Hit: {1}", Time.frameCount, raycastHit.textureCoord.x));
				}
				RaycastHit raycastHit2 = orderedEnumerable.First<RaycastHit>();
				if (this.tiedKnot.knotPoints.Count > 0)
				{
					List<AlignWithSpline.TiedKnot.KnotPoint> knotPoints = this.tiedKnot.knotPoints;
					if (Vector3.Distance(knotPoints[knotPoints.Count - 1].position, raycastHit2.point) < this.minKnotPointDistance)
					{
						return;
					}
				}
				this.lastKnotPointProgress = raycastHit2.textureCoord.x;
				this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint
				{
					position = raycastHit2.point.xyo(),
					templateProgress = this.lastKnotPointProgress,
					inside = true
				});
				Debug.Log(string.Format("Added: {0}", raycastHit2.textureCoord.x));
				return;
			}
			float d;
			if (plane.Raycast(ray, out d))
			{
				Vector3 me = ray.direction * d + ray.origin;
				this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint
				{
					position = me.xyo(),
					templateProgress = this.lastKnotPointProgress,
					inside = false
				});
			}
		}

		// Token: 0x06001946 RID: 6470 RVA: 0x0007F7F4 File Offset: 0x0007D9F4
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				this.tiedKnot = new AlignWithSpline.TiedKnot();
			}
			else if (Input.GetKey(KeyCode.Mouse0))
			{
				this.TieRope();
			}
			Input.GetKeyUp(KeyCode.Mouse0);
		}

		// Token: 0x06001947 RID: 6471 RVA: 0x0007F82C File Offset: 0x0007DA2C
		private void FixedUpdate()
		{
		}

		// Token: 0x04001706 RID: 5894
		public SplineContainer splineContainer;

		// Token: 0x04001707 RID: 5895
		public float knotProgress;

		// Token: 0x04001708 RID: 5896
		public float minKnotPointDistance = 0.001f;

		// Token: 0x04001709 RID: 5897
		public float lastKnotPointProgress;

		// Token: 0x0400170A RID: 5898
		private AlignWithSpline.TiedKnot tiedKnot = new AlignWithSpline.TiedKnot();

		// Token: 0x0400170B RID: 5899
		public TiedKnotVisualizer tiedKnotVisualizer;

		// Token: 0x0400170C RID: 5900
		public float knotProgressRange = 0.025f;

		// Token: 0x0400170D RID: 5901
		public Vector2 knotProgressRangeRelation = new Vector2(-2f, 1f);

		// Token: 0x0400170E RID: 5902
		public float test = -0.3f;

		// Token: 0x02000560 RID: 1376
		public class TiedKnot
		{
			// Token: 0x04001D30 RID: 7472
			public List<AlignWithSpline.TiedKnot.KnotPoint> knotPoints = new List<AlignWithSpline.TiedKnot.KnotPoint>();

			// Token: 0x02000595 RID: 1429
			public class KnotPoint
			{
				// Token: 0x04001DF5 RID: 7669
				public Vector3 position;

				// Token: 0x04001DF6 RID: 7670
				public float templateProgress;

				// Token: 0x04001DF7 RID: 7671
				public bool inside;
			}
		}
	}
}
