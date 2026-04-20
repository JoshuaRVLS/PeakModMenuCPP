using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000376 RID: 886
public class VineShooter : ItemComponent
{
	// Token: 0x0600173F RID: 5951 RVA: 0x00077B7B File Offset: 0x00075D7B
	public override void Awake()
	{
		this.actionReduceUses = base.GetComponent<Action_ReduceUses>();
		base.Awake();
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Combine(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x00077BB6 File Offset: 0x00075DB6
	private void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Remove(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x00077BE0 File Offset: 0x00075DE0
	public void Update()
	{
		RaycastHit raycastHit;
		this.item.overrideUsability = Optionable<bool>.Some(this.WillAttach(out raycastHit));
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x00077C08 File Offset: 0x00075E08
	private void OnPrimaryFinishedCast()
	{
		Debug.Log("VineShooter shoot");
		RaycastHit raycastHit;
		if (!this.WillAttach(out raycastHit))
		{
			return;
		}
		if (this.disableOnFire != null)
		{
			this.disableOnFire.SetActive(false);
		}
		JungleVine component = this.vinePrefab.GetComponent<JungleVine>();
		Vector2 a = new Vector2(component.minDown, component.maxDown);
		int num = 10;
		for (int i = 0; i < num; i++)
		{
			float num2 = -Vector2.Lerp(a, Vector2.zero, (float)i / ((float)num - 1f)).PRndRange();
			Transform transform = MainCamera.instance.transform;
			Vector3 origin = transform.position + transform.forward;
			Vector3 vector = transform.position - Vector3.up * 0.2f;
			RaycastHit raycastHit2;
			if (Physics.Raycast(origin, Vector3.down, out raycastHit2, 4f, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), QueryTriggerInteraction.UseGlobal))
			{
				vector = raycastHit2.point + Vector3.up * 1.5f;
			}
			Debug.Log(string.Format("from: {0}, to: {1}, hang: {2}", vector, raycastHit.point, num2));
			Vector3 vector2;
			if (JungleVine.CheckVinePath(vector, raycastHit.point, num2, out vector2, 0f))
			{
				PhotonNetwork.Instantiate(this.vinePrefab.name, vector, Quaternion.identity, 0, null).GetComponent<JungleVine>().photonView.RPC("ForceBuildVine_RPC", RpcTarget.AllBuffered, new object[]
				{
					vector,
					raycastHit.point,
					num2,
					vector2
				});
				this.actionReduceUses.RunAction();
				Debug.DrawLine(vector, raycastHit.point, Color.green, 5f);
				GameUtils.instance.IncrementPermanentItemsPlaced();
				return;
			}
			Debug.DrawLine(vector, raycastHit.point, Color.red, 5f);
		}
	}

	// Token: 0x06001743 RID: 5955 RVA: 0x00077E00 File Offset: 0x00076000
	public bool WillAttach(out RaycastHit hit)
	{
		hit = default(RaycastHit);
		if (!Character.localCharacter.data.isGrounded)
		{
			return false;
		}
		if (!Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, out hit, this.maxLength, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), QueryTriggerInteraction.UseGlobal))
		{
			return false;
		}
		float num = Vector3.Dot(Vector3.up, (hit.point - MainCamera.instance.transform.position).normalized);
		return num <= 0.6f && num >= -0.7f && Vector3.Dot(Vector3.up, (hit.point - MainCamera.instance.transform.position).normalized) <= 0.5f;
	}

	// Token: 0x06001744 RID: 5956 RVA: 0x00077ED6 File Offset: 0x000760D6
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x040015BF RID: 5567
	public GameObject vinePrefab;

	// Token: 0x040015C0 RID: 5568
	public GameObject disableOnFire;

	// Token: 0x040015C1 RID: 5569
	public float maxLength = 40f;

	// Token: 0x040015C2 RID: 5570
	private Action_ReduceUses actionReduceUses;
}
