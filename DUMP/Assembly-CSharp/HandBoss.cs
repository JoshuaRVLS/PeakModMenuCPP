using System;
using System.Collections.Generic;
using Knot;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200028C RID: 652
public class HandBoss : MonoBehaviour
{
	// Token: 0x060012D3 RID: 4819 RVA: 0x0005EE0F File Offset: 0x0005D00F
	private void Start()
	{
		Cursor.visible = false;
	}

	// Token: 0x060012D4 RID: 4820 RVA: 0x0005EE17 File Offset: 0x0005D017
	private void DisableAll()
	{
		this.grabMaking.SetActive(false);
		this.grabUnmaking.SetActive(false);
		this.idle.SetActive(false);
		this.lr.gameObject.SetActive(false);
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x0005EE50 File Offset: 0x0005D050
	private void Update()
	{
		this.DisableAll();
		base.transform.position = Input.mousePosition;
		if (this.knotMaker.grabbedRope)
		{
			this.grabMaking.SetActive(true);
			return;
		}
		if (this.knotUnmaker.grabbing)
		{
			this.lr.gameObject.SetActive(true);
			LineRenderer lineRenderer = this.lr;
			int index = 0;
			List<TiedKnotVisualizer.KnotPart> knot = this.knotUnmaker.visualizer.knot;
			Vector3 position = knot[knot.Count - 1].position;
			List<TiedKnotVisualizer.KnotPart> knot2 = this.knotUnmaker.visualizer.knot;
			lineRenderer.SetPosition(index, position.xyn(knot2[knot2.Count - 1].position.z - 1f));
			this.lr.startColor = this.knotUnmaker.lineColor;
			this.lr.endColor = this.knotUnmaker.lineColor;
			this.grabUnmaking.SetActive(true);
			Camera main = Camera.main;
			List<TiedKnotVisualizer.KnotPart> knot3 = this.knotUnmaker.visualizer.knot;
			main.WorldToScreenPoint(knot3[knot3.Count - 1].position);
			LineRenderer lineRenderer2 = this.lr;
			int index2 = 1;
			Vector3 me = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			List<TiedKnotVisualizer.KnotPart> knot4 = this.knotUnmaker.visualizer.knot;
			lineRenderer2.SetPosition(index2, me.xyn(knot4[knot4.Count - 1].position.z - 1f));
			return;
		}
		this.idle.SetActive(true);
	}

	// Token: 0x040010E1 RID: 4321
	public GameObject grabMaking;

	// Token: 0x040010E2 RID: 4322
	public GameObject grabUnmaking;

	// Token: 0x040010E3 RID: 4323
	public GameObject idle;

	// Token: 0x040010E4 RID: 4324
	public Image handImage;

	// Token: 0x040010E5 RID: 4325
	public KnotMaker knotMaker;

	// Token: 0x040010E6 RID: 4326
	public KnotUnmaker knotUnmaker;

	// Token: 0x040010E7 RID: 4327
	public LineRenderer lr;
}
