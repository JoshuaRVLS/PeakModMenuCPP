using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class BingBongTimeControl : MonoBehaviour
{
	// Token: 0x0600108E RID: 4238 RVA: 0x00052717 File Offset: 0x00050917
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x00052728 File Offset: 0x00050928
	private void Update()
	{
		this.syncCounter += Time.unscaledDeltaTime;
		if (Input.GetKeyDown(KeyCode.R))
		{
			this.currentTimeScale = 1f;
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			this.currentTimeScale = 0f;
		}
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.currentTimeScale += Mathf.Clamp(0.1f, this.currentTimeScale * 0.3f, 0.5f);
		}
		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			this.currentTimeScale -= Mathf.Clamp(0.1f, this.currentTimeScale * 0.3f, 0.5f);
		}
		this.currentTimeScale = Mathf.Clamp(this.currentTimeScale, 0.02f, 10f);
		if (Time.timeScale != this.currentTimeScale)
		{
			this.bingBongPowers.SetTip(string.Format("Time Scale: {0:P0}", this.currentTimeScale), 1);
			if (this.syncCounter > 0.1f)
			{
				this.view.RPC("RPCA_SyncTimeScale", RpcTarget.All, new object[]
				{
					this.currentTimeScale
				});
			}
		}
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x00052852 File Offset: 0x00050A52
	[PunRPC]
	public void RPCA_SyncTimeScale(float newTimeScale)
	{
		Time.timeScale = newTimeScale;
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x0005285A File Offset: 0x00050A5A
	private void OnDestroy()
	{
		Time.timeScale = 1f;
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x00052866 File Offset: 0x00050A66
	private void OnEnable()
	{
		this.bingBongPowers = base.GetComponent<BingBongPowers>();
		this.bingBongPowers.SetTexts("TIME", this.descr);
	}

	// Token: 0x04000E7E RID: 3710
	private PhotonView view;

	// Token: 0x04000E7F RID: 3711
	public float currentTimeScale = 1f;

	// Token: 0x04000E80 RID: 3712
	private float syncCounter;

	// Token: 0x04000E81 RID: 3713
	private BingBongPowers bingBongPowers;

	// Token: 0x04000E82 RID: 3714
	private string descr = "Reset time: [R]\n\nFreeze: [F]\n\nFaster: [LMB]\n\nSlower: [RMB]";
}
