using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002CC RID: 716
[RequireComponent(typeof(PhotonView))]
public class ChangeName : MonoBehaviour
{
	// Token: 0x06001425 RID: 5157 RVA: 0x00065DDC File Offset: 0x00063FDC
	private void Start()
	{
		PhotonView component = base.GetComponent<PhotonView>();
		base.name = string.Format("ActorNumber {0}", component.OwnerActorNr);
	}
}
