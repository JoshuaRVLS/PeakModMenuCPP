using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020002B9 RID: 697
public class NavJumper : MonoBehaviour
{
	// Token: 0x060013AE RID: 5038 RVA: 0x00063ECD File Offset: 0x000620CD
	private void Start()
	{
	}

	// Token: 0x060013AF RID: 5039 RVA: 0x00063ED0 File Offset: 0x000620D0
	private void Jump()
	{
		List<RaycastHit> list = new List<RaycastHit>();
		for (int i = 0; i < this.castsPerJump; i++)
		{
			RaycastHit item;
			if (Physics.Raycast(base.transform.position + (ExtMath.RandInsideUnitCircle() * this.castRadius).xny(this.castHeight), Vector3.down * this.castHeight, out item))
			{
				list.Add(item);
			}
		}
		Debug.Log(string.Format("Total: {0}", list.Count));
		list = (from hit in list
		where Vector3.Angle(hit.normal, Vector3.up) < 50f
		select hit).ToList<RaycastHit>();
		Debug.Log(string.Format("After angle: {0}", list.Count));
		list = (from hit in list
		where Vector3.Distance(hit.point, base.transform.position) < this.maxDistance
		select hit).ToList<RaycastHit>();
		Debug.Log(string.Format("After distance: {0}", list.Count));
		list = (from hit in list
		where hit.point.z > base.transform.position.z && hit.point.y > base.transform.position.y
		select hit).ToList<RaycastHit>();
		list = (from hit in list
		where hit.point.y > base.transform.position.y
		select hit).ToList<RaycastHit>();
		Debug.Log(string.Format("After Z: {0}", list.Count));
		if (list.Count == 0)
		{
			return;
		}
		RaycastHit raycastHit = (from hit in list
		orderby hit.point.z descending
		select hit).First<RaycastHit>();
		Debug.DrawLine(base.transform.position + Vector3.up, raycastHit.point + Vector3.up, Color.green, 10f);
		base.transform.position = raycastHit.point;
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x00064095 File Offset: 0x00062295
	private void Update()
	{
	}

	// Token: 0x040011FE RID: 4606
	public int castsPerJump = 100;

	// Token: 0x040011FF RID: 4607
	public float maxDistance = 3f;

	// Token: 0x04001200 RID: 4608
	public float castRadius = 1f;

	// Token: 0x04001201 RID: 4609
	public float castHeight = 100f;

	// Token: 0x04001202 RID: 4610
	private int fails;
}
