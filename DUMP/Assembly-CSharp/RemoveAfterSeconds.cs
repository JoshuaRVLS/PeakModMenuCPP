using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200031A RID: 794
public class RemoveAfterSeconds : MonoBehaviour
{
	// Token: 0x0600153C RID: 5436 RVA: 0x0006B2C3 File Offset: 0x000694C3
	private void Start()
	{
		if (this.photonRemove)
		{
			this.view = base.GetComponent<PhotonView>();
		}
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x0006B2D9 File Offset: 0x000694D9
	public void Config(bool setShrink, float setSeconds)
	{
		this.seconds = setSeconds;
		this.shrink = setShrink;
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x0006B2EC File Offset: 0x000694EC
	private void Update()
	{
		if (this.seconds < 0f)
		{
			if (this.shrink && base.transform.localScale.x > 0.01f)
			{
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.zero, Time.deltaTime);
				return;
			}
			if (!this.photonRemove || !this.view)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			if (this.view.IsMine)
			{
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
		}
		else
		{
			this.seconds -= Time.deltaTime;
		}
	}

	// Token: 0x04001351 RID: 4945
	public float seconds = 5f;

	// Token: 0x04001352 RID: 4946
	public bool shrink;

	// Token: 0x04001353 RID: 4947
	public bool photonRemove;

	// Token: 0x04001354 RID: 4948
	private PhotonView view;
}
