using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x0200028B RID: 651
public class AddPointAtEndOfSpline : MonoBehaviour
{
	// Token: 0x060012CE RID: 4814 RVA: 0x0005ED10 File Offset: 0x0005CF10
	public void SetAllZ(float v)
	{
		SplineContainer component = base.GetComponent<SplineContainer>();
		List<BezierKnot> list = component.Spline.Knots.ToList<BezierKnot>();
		for (int i = 0; i < list.Count; i++)
		{
			BezierKnot bezierKnot = list[i];
			bezierKnot.Position = bezierKnot.Position.xyn(v);
			list[i] = bezierKnot;
		}
		component.Spline.Knots = list;
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x0005ED78 File Offset: 0x0005CF78
	private void GO()
	{
		SplineContainer component = base.GetComponent<SplineContainer>();
		BezierKnot bezierKnot = component.Spline.Knots.Last<BezierKnot>();
		List<BezierKnot> list = component.Spline.Knots.ToList<BezierKnot>();
		BezierKnot bezierKnot2 = list[list.Count - 2];
		component.Spline.Add(bezierKnot.Position.PToV3() + (bezierKnot.Position.PToV3() - bezierKnot2.Position.PToV3()).normalized, TangentMode.AutoSmooth);
		PExt.SaveObj(component);
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x0005EE03 File Offset: 0x0005D003
	private void Start()
	{
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x0005EE05 File Offset: 0x0005D005
	private void Update()
	{
	}
}
