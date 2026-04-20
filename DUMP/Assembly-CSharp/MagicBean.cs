using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000124 RID: 292
public class MagicBean : ItemComponent
{
	// Token: 0x06000994 RID: 2452 RVA: 0x00032F88 File Offset: 0x00031188
	public void Update()
	{
		if (this.photonView.IsMine)
		{
			if (this.item.itemState == ItemState.Held)
			{
				base.GetData<OptionableBoolItemData>(DataEntryKey.Used).HasData = true;
				return;
			}
			if (PhotonNetwork.IsMasterClient && this.isPlanted)
			{
				this.timeToPlant -= Time.deltaTime;
				if (this.timeToPlant <= 0f)
				{
					float vineDistance = this.GetVineDistance(base.transform.position, this.averageNormal);
					this.photonView.RPC("GrowVineRPC", RpcTarget.All, new object[]
					{
						base.transform.position,
						this.averageNormal,
						vineDistance
					});
					this.GrowVineRPC(base.transform.position, this.averageNormal, vineDistance);
					PhotonNetwork.Destroy(base.gameObject);
				}
			}
		}
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x00033071 File Offset: 0x00031271
	private void DebugValue()
	{
		if (base.HasData(DataEntryKey.Used))
		{
			Debug.Log(base.GetData<BoolItemData>(DataEntryKey.Used).Value);
			return;
		}
		Debug.Log("No data");
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x0003309D File Offset: 0x0003129D
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x000330A0 File Offset: 0x000312A0
	private void OnCollisionEnter(Collision collision)
	{
		if (this.photonView.IsMine && this.item.itemState == ItemState.Ground && base.GetData<OptionableBoolItemData>(DataEntryKey.Used).HasData && HelperFunctions.IsLayerInLayerMask(HelperFunctions.LayerType.TerrainMap, collision.gameObject.layer))
		{
			this.item.SetKinematicNetworked(true, this.item.transform.position, this.item.transform.rotation);
			this.DoNormalRaycasts(collision.contacts[0].point, collision.contacts[0].normal);
			this.isPlanted = true;
		}
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x00033148 File Offset: 0x00031348
	private float GetVineDistance(Vector3 startPos, Vector3 direction)
	{
		RaycastHit[] array = new RaycastHit[128];
		HelperFunctions.LineCheckAll(startPos, startPos + direction * this.plantPrefab.maxLength, HelperFunctions.LayerType.TerrainMap, array, 0f, QueryTriggerInteraction.Ignore);
		float num = this.plantPrefab.maxLength;
		foreach (RaycastHit raycastHit in array)
		{
			if (raycastHit.collider && raycastHit.distance > 0.7f && raycastHit.distance < num)
			{
				num = raycastHit.distance;
			}
		}
		return num;
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x000331DA File Offset: 0x000313DA
	[PunRPC]
	protected void GrowVineRPC(Vector3 pos, Vector3 direction, float maxLength)
	{
		MagicBeanVine magicBeanVine = Object.Instantiate<MagicBeanVine>(this.plantPrefab, pos, Quaternion.identity);
		magicBeanVine.transform.up = direction;
		magicBeanVine.maxLength = maxLength;
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x00033200 File Offset: 0x00031400
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		foreach (Vector3 center in this.raycastSpotsTest)
		{
			Gizmos.DrawSphere(center, 0.1f);
			Gizmos.DrawLine(base.transform.position, base.transform.position + this.averageNormal);
		}
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x00033288 File Offset: 0x00031488
	private void TestRaycast()
	{
		this.DoNormalRaycasts(base.transform.position, Vector3.up);
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x000332A0 File Offset: 0x000314A0
	private void DoNormalRaycasts(Vector3 centralHit, Vector3 centralNormal)
	{
		this.raycastSpotsTest.Clear();
		List<Vector3> list = new List<Vector3>();
		float d = 0.2f;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i != 0 || j != 0)
				{
					Vector3 b = Vector3.ProjectOnPlane(new Vector3((float)i, 0f, (float)j), centralNormal).normalized * d;
					Vector3 vector = centralHit + b + centralNormal;
					this.raycastSpotsTest.Add(vector);
					this.raycastResult = HelperFunctions.LineCheck(vector, vector - centralNormal * 2f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
					if (this.raycastResult.collider != null)
					{
						list.Add(this.raycastResult.normal);
					}
				}
			}
			Vector3 a = centralNormal;
			foreach (Vector3 b2 in list)
			{
				a += b2;
			}
			this.averageNormal = a.normalized;
			if (Vector3.Angle(this.averageNormal, Vector3.up) < this.snapToVerticalAngle)
			{
				this.averageNormal = Vector3.up;
			}
		}
	}

	// Token: 0x040008D7 RID: 2263
	private bool isPlanted;

	// Token: 0x040008D8 RID: 2264
	public float timeToPlant;

	// Token: 0x040008D9 RID: 2265
	public MagicBeanVine plantPrefab;

	// Token: 0x040008DA RID: 2266
	public float snapToVerticalAngle = 15f;

	// Token: 0x040008DB RID: 2267
	private List<Vector3> raycastSpotsTest = new List<Vector3>();

	// Token: 0x040008DC RID: 2268
	private RaycastHit raycastResult;

	// Token: 0x040008DD RID: 2269
	private Vector3 averageNormal;
}
