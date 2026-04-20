using System;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000316 RID: 790
public class PutMeInWall : MonoBehaviour
{
	// Token: 0x0600152C RID: 5420 RVA: 0x0006AA93 File Offset: 0x00068C93
	private void Go()
	{
		this.PutInTheWall();
	}

	// Token: 0x0600152D RID: 5421 RVA: 0x0006AA9C File Offset: 0x00068C9C
	public bool PutInTheWall()
	{
		Vector3 vector = base.transform.position - Vector3.forward * 50f;
		RaycastHit[] array = Physics.RaycastAll(vector, Vector3.forward, 500f);
		Debug.DrawLine(vector, vector + Vector3.forward * 100f, Color.red, 10f);
		Debug.Log(string.Format("hits: {0}", array.Length));
		Debug.Log(string.Format("list{0}", array));
		array = (from h in array
		orderby h.distance
		select h).ToArray<RaycastHit>();
		RaycastHit raycastHit = array.First((RaycastHit h) => h.collider.gameObject != this.gameObject);
		Vector3 vector2 = raycastHit.point + Vector3.forward * this.penetrationRnage.PRndRange();
		Collider component = base.GetComponent<Collider>();
		if (this.angle > 0f && Vector2.Angle(raycastHit.normal, Vector2.up) <= this.angle)
		{
			return false;
		}
		if (this.checkBelow)
		{
			RaycastHit[] array2 = Physics.SphereCastAll(vector2, component.bounds.extents.magnitude, Vector3.down, component.bounds.extents.magnitude * this.belowMargin);
			Debug.Log(string.Format("belowHits: {0}", array2.Length));
			array2 = (from hit in array2
			where hit.collider.gameObject != this.gameObject && hit.collider.gameObject != raycastHit.collider.gameObject
			select hit).ToArray<RaycastHit>();
			Debug.Log(string.Format("belowHits2: {0}", array2.Length));
			if (array2.Length != 0)
			{
				foreach (RaycastHit raycastHit2 in array2)
				{
					Debug.Log(string.Format("hit: {0}", raycastHit2.collider.gameObject));
				}
				Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.red, 10f);
				return false;
			}
			Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.green, 10f);
		}
		Debug.Log(raycastHit.collider.gameObject, raycastHit.collider.gameObject);
		base.transform.position = vector2;
		return true;
	}

	// Token: 0x0600152E RID: 5422 RVA: 0x0006AD94 File Offset: 0x00068F94
	public Vector3? GetWallPosition2(Vector3 startCast, float maxDistance = 100f)
	{
		Vector3 vector = startCast - Vector3.forward * 50f;
		maxDistance += 50f;
		RaycastHit[] array = Physics.RaycastAll(vector, Vector3.forward, maxDistance);
		Debug.DrawLine(vector, vector + Vector3.forward * maxDistance, Color.red, 10f);
		Debug.Log(string.Format("hits: {0}", array.Length));
		Debug.Log(string.Format("list{0}", array));
		array = (from h in array
		orderby h.distance
		select h).ToArray<RaycastHit>();
		RaycastHit raycastHit = array.First((RaycastHit h) => h.collider.gameObject != this.gameObject);
		Vector3 vector2 = raycastHit.point + Vector3.forward * this.penetrationRnage.PRndRange();
		Collider component = base.GetComponent<Collider>();
		if (this.angle > 0f && Vector2.Angle(raycastHit.normal, Vector2.up) <= this.angle)
		{
			return null;
		}
		if (this.checkBelow)
		{
			if ((from hit in Physics.SphereCastAll(vector2, component.bounds.extents.magnitude, Vector3.down, component.bounds.extents.magnitude * this.belowMargin)
			where hit.collider.gameObject != this.gameObject && hit.collider.gameObject != raycastHit.collider.gameObject
			select hit).ToArray<RaycastHit>().Length != 0)
			{
				Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.red, 10f);
				return null;
			}
			Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.green, 10f);
		}
		Debug.Log(raycastHit.collider.gameObject, raycastHit.collider.gameObject);
		return new Vector3?(vector2);
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x0006B018 File Offset: 0x00069218
	public Vector3? GetWallPosition(Vector3 startCast, float maxDistance = 100f)
	{
		Vector3 vector = startCast - Vector3.forward * 50f;
		maxDistance += 50f;
		RaycastHit[] array = Physics.RaycastAll(vector, Vector3.forward, maxDistance, HelperFunctions.GetMask(HelperFunctions.LayerType.Terrain));
		if (this.angle > 0f)
		{
			array = (from h in array
			where Vector2.Angle(h.normal, Vector2.up) > this.angle
			select h).ToArray<RaycastHit>();
		}
		array = (from h in array
		orderby h.distance
		select h).ToArray<RaycastHit>();
		Debug.DrawLine(vector, vector + Vector3.up * maxDistance, Color.green, 10f);
		Debug.DrawLine(vector, vector + Vector3.forward * maxDistance, Color.red, 10f);
		Debug.Log(string.Format("hits: {0}", array.Length));
		Debug.Log(string.Format("list{0}", array));
		RaycastHit[] array2 = array;
		int num = 0;
		if (num >= array2.Length)
		{
			return null;
		}
		RaycastHit raycastHit = array2[num];
		return new Vector3?(raycastHit.point + Vector3.forward * this.penetrationRnage.PRndRange());
	}

	// Token: 0x06001530 RID: 5424 RVA: 0x0006B158 File Offset: 0x00069358
	public void RandomRotation()
	{
		base.transform.rotation = Quaternion.Euler((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360));
	}

	// Token: 0x06001531 RID: 5425 RVA: 0x0006B18E File Offset: 0x0006938E
	public void RandomScale()
	{
		base.transform.localScale *= this.scaleRange.PRndRange();
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x0006B1B1 File Offset: 0x000693B1
	private void Start()
	{
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x0006B1B3 File Offset: 0x000693B3
	private void Update()
	{
	}

	// Token: 0x04001346 RID: 4934
	public Vector2 penetrationRnage;

	// Token: 0x04001347 RID: 4935
	public Vector2 scaleRange = new Vector2(1f, 1f);

	// Token: 0x04001348 RID: 4936
	public bool checkBelow;

	// Token: 0x04001349 RID: 4937
	public float belowMargin = 1f;

	// Token: 0x0400134A RID: 4938
	public float angle = -1f;
}
