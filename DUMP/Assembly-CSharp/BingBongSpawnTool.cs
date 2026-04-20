using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200021B RID: 539
public class BingBongSpawnTool : MonoBehaviour
{
	// Token: 0x06001083 RID: 4227 RVA: 0x00052084 File Offset: 0x00050284
	private void Update()
	{
		this.counter += Time.unscaledDeltaTime;
		if (this.counter < this.spawnRate)
		{
			return;
		}
		if (!this.auto && !Input.GetKeyDown(KeyCode.Mouse0))
		{
			return;
		}
		if (this.auto && !Input.GetKey(KeyCode.Mouse0))
		{
			return;
		}
		this.counter = 0f;
		this.Spawn();
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x000520F0 File Offset: 0x000502F0
	private void Spawn()
	{
		Vector3 position = this.GetPosition();
		Quaternion rotation = this.GetRotation();
		GameObject gameObject = PhotonNetwork.Instantiate(this.folder + this.objectToSpawn.name, position, rotation, 0, null);
		if (this.bingbongInit)
		{
			gameObject.GetComponent<PhotonView>().RPC("RPCA_BingBongInitObj", RpcTarget.AllBuffered, new object[]
			{
				base.GetComponentInParent<PhotonView>().ViewID
			});
		}
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x00052160 File Offset: 0x00050360
	public Vector3 GetPosition()
	{
		Vector3 a = base.transform.position;
		if (this.pos == BingBongSpawnTool.SpawnPos.RaycastPos)
		{
			RaycastHit raycastHit = HelperFunctions.LineCheck(base.transform.position, base.transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.AllPhysical, 0f, QueryTriggerInteraction.Ignore);
			if (raycastHit.transform)
			{
				a = raycastHit.point;
				a += raycastHit.normal * this.normalOffsetPos;
			}
		}
		else if (this.pos == BingBongSpawnTool.SpawnPos.BingBong)
		{
			a = base.transform.TransformPoint(Vector3.forward * 2f);
		}
		return a + Random.insideUnitSphere * this.randomPosRadius;
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x00052220 File Offset: 0x00050420
	public Quaternion GetRotation()
	{
		if (this.rot == BingBongSpawnTool.SpawnRot.BingBongRotation)
		{
			return base.transform.rotation;
		}
		if (this.rot == BingBongSpawnTool.SpawnRot.Random)
		{
			return Random.rotation;
		}
		if (this.rot == BingBongSpawnTool.SpawnRot.RaycastNormal)
		{
			return Quaternion.LookRotation(HelperFunctions.LineCheck(base.transform.position, base.transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.AllPhysical, 0f, QueryTriggerInteraction.Ignore).normal);
		}
		BingBongSpawnTool.SpawnRot spawnRot = this.rot;
		return Quaternion.identity;
	}

	// Token: 0x04000E6F RID: 3695
	public float spawnRate = 0.1f;

	// Token: 0x04000E70 RID: 3696
	public bool auto = true;

	// Token: 0x04000E71 RID: 3697
	public string folder = "0_Items/";

	// Token: 0x04000E72 RID: 3698
	public GameObject objectToSpawn;

	// Token: 0x04000E73 RID: 3699
	public bool bingbongInit;

	// Token: 0x04000E74 RID: 3700
	public BingBongSpawnTool.SpawnPos pos;

	// Token: 0x04000E75 RID: 3701
	public BingBongSpawnTool.SpawnRot rot;

	// Token: 0x04000E76 RID: 3702
	public float randomPosRadius;

	// Token: 0x04000E77 RID: 3703
	public float normalOffsetPos;

	// Token: 0x04000E78 RID: 3704
	private float counter;

	// Token: 0x020004F7 RID: 1271
	public enum SpawnPos
	{
		// Token: 0x04001BC8 RID: 7112
		BingBong,
		// Token: 0x04001BC9 RID: 7113
		RaycastPos
	}

	// Token: 0x020004F8 RID: 1272
	public enum SpawnRot
	{
		// Token: 0x04001BCB RID: 7115
		BingBongRotation,
		// Token: 0x04001BCC RID: 7116
		Random,
		// Token: 0x04001BCD RID: 7117
		RaycastNormal,
		// Token: 0x04001BCE RID: 7118
		Identity
	}
}
