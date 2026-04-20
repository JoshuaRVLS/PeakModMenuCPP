using System;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200027C RID: 636
public class IcicleCheck : CustomSpawnCondition
{
	// Token: 0x0600128B RID: 4747 RVA: 0x0005D0FC File Offset: 0x0005B2FC
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		PropSpawner comp = base.GetComponentInParent<PropSpawner>();
		base.transform.localScale = this.minMaxScale.PRndRange().xxx();
		Vector3 vector = this.boxCollider.transform.TransformPoint(this.boxCollider.center);
		Vector3 halfExtents = Vector3.Scale(this.boxCollider.transform.lossyScale, this.boxCollider.size) / 2f;
		if (!this.LineCheck())
		{
			return false;
		}
		Collider[] array = (from c in Physics.OverlapBox(vector, halfExtents, this.boxCollider.transform.rotation)
		where c.GetComponentInParent<PropSpawner>() != comp
		select c).ToArray<Collider>();
		foreach (Collider collider in array)
		{
			Debug.DrawLine(vector, collider.transform.position, Color.red);
		}
		base.transform.position += Vector2.Scale(base.transform.lossyScale, this.minMax).PRndRange().oxo();
		return array.Length == 0;
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x0005D22C File Offset: 0x0005B42C
	public bool LineCheck()
	{
		Vector3 vector = base.transform.TransformPoint(this.localStart);
		Vector3 vector2 = base.transform.TransformPoint(this.localEnd);
		bool flag = !HelperFunctions.LineCheck(vector, vector2, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform;
		Debug.DrawLine(vector, vector2, flag ? Color.green : Color.red, 10f);
		return flag;
	}

	// Token: 0x0400109E RID: 4254
	public BoxCollider boxCollider;

	// Token: 0x0400109F RID: 4255
	public Vector2 minMax;

	// Token: 0x040010A0 RID: 4256
	public Vector2 minMaxScale = new Vector2(1f, 1f);

	// Token: 0x040010A1 RID: 4257
	public Vector3 localStart = new Vector3(0f, 0f, 0f);

	// Token: 0x040010A2 RID: 4258
	public Vector3 localEnd = new Vector3(0f, 5f, 0f);
}
