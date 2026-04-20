using System;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x0200028E RID: 654
public class SplineKnotPosition : MonoBehaviour
{
	// Token: 0x060012DF RID: 4831 RVA: 0x0005F108 File Offset: 0x0005D308
	private void Start()
	{
		if (this.splineContainer == null || this.splineContainer.Splines.Count == 0)
		{
			Debug.LogError("SplineContainer is missing or empty.");
			return;
		}
		Spline spline = this.splineContainer.Splines[0];
		float normalizedInterpolation = SplineUtility.GetNormalizedInterpolation<Spline>(this.splineContainer.Spline, this.f, PathIndexUnit.Knot);
		Debug.Log(string.Format("Knot {0} is at {1}% along the spline.", this.knotIndex, normalizedInterpolation * 100f));
	}

	// Token: 0x040010ED RID: 4333
	public SplineContainer splineContainer;

	// Token: 0x040010EE RID: 4334
	public int knotIndex;

	// Token: 0x040010EF RID: 4335
	public float f;
}
