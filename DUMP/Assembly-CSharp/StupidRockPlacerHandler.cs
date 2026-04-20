using System;
using UnityEngine;

// Token: 0x02000355 RID: 853
public class StupidRockPlacerHandler : MonoBehaviour
{
	// Token: 0x06001695 RID: 5781 RVA: 0x00073A63 File Offset: 0x00071C63
	private void Start()
	{
	}

	// Token: 0x06001696 RID: 5782 RVA: 0x00073A68 File Offset: 0x00071C68
	private void ReDo()
	{
		StupidRockPlacer[] componentsInChildren = base.GetComponentsInChildren<StupidRockPlacer>();
		StupidRockPlacer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
		foreach (StupidRockPlacer stupidRockPlacer in componentsInChildren)
		{
			int num = stupidRockPlacer.amount;
			stupidRockPlacer.amount = (int)(this.amount * (float)stupidRockPlacer.amount);
			stupidRockPlacer.Go();
			stupidRockPlacer.amount = num;
		}
	}

	// Token: 0x06001697 RID: 5783 RVA: 0x00073AD4 File Offset: 0x00071CD4
	private void Clear()
	{
		StupidRockPlacer[] componentsInChildren = base.GetComponentsInChildren<StupidRockPlacer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Clear();
		}
	}

	// Token: 0x06001698 RID: 5784 RVA: 0x00073AFE File Offset: 0x00071CFE
	private void Update()
	{
	}

	// Token: 0x040014F5 RID: 5365
	public float amount = 1f;
}
