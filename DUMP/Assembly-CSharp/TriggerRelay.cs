using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200036A RID: 874
public class TriggerRelay : MonoBehaviour
{
	// Token: 0x0600170D RID: 5901 RVA: 0x00076672 File Offset: 0x00074872
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x00076680 File Offset: 0x00074880
	[PunRPC]
	public void RPCA_Trigger(int childID)
	{
		Transform child = base.transform.GetChild(childID);
		TriggerEvent triggerEvent;
		if (child && child.TryGetComponent<TriggerEvent>(out triggerEvent))
		{
			triggerEvent.Trigger();
		}
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x000766B2 File Offset: 0x000748B2
	[PunRPC]
	public void RPCA_TriggerWithTarget(int childID, int targetID)
	{
		base.transform.GetChild(childID).GetComponent<SlipperyJellyfish>().Trigger(targetID);
	}

	// Token: 0x04001580 RID: 5504
	internal PhotonView view;
}
