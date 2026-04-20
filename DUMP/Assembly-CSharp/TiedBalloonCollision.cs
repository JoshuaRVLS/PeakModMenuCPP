using System;
using UnityEngine;

// Token: 0x0200035D RID: 861
public class TiedBalloonCollision : MonoBehaviour
{
	// Token: 0x060016D1 RID: 5841 RVA: 0x00075385 File Offset: 0x00073585
	private void OnCollisionEnter(Collision collision)
	{
		if (this.tiedBalloon.photonView.IsMine && collision.collider.GetComponent<StickyCactus>())
		{
			this.tiedBalloon.Pop();
		}
	}

	// Token: 0x0400153D RID: 5437
	public TiedBalloon tiedBalloon;
}
