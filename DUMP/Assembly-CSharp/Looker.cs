using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Settings;

// Token: 0x020002A4 RID: 676
public class Looker : MonoBehaviour
{
	// Token: 0x06001339 RID: 4921 RVA: 0x00060F04 File Offset: 0x0005F104
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.pivot = base.transform.Find("Pivot");
		this.SetRandomSwitch();
		this.view = base.GetComponent<PhotonView>();
		if (GameHandler.Instance.SettingsHandler.GetSetting<LookerSetting>().Value == OffOnMode.OFF)
		{
			this.guy.SetActive(false);
		}
	}

	// Token: 0x0600133A RID: 4922 RVA: 0x00060F68 File Offset: 0x0005F168
	private void ToggleLookers()
	{
		Looker[] componentsInChildren = base.transform.parent.GetComponentsInChildren<Looker>();
		int num = Random.Range(0, componentsInChildren.Length);
		if (Random.value < 0.95f)
		{
			num = -1;
		}
		foreach (Looker looker in componentsInChildren)
		{
			if (looker.transform.GetSiblingIndex() != num)
			{
				looker.view.RPC("RPCA_DisableLooker", RpcTarget.AllBuffered, Array.Empty<object>());
			}
		}
	}

	// Token: 0x0600133B RID: 4923 RVA: 0x00060FD9 File Offset: 0x0005F1D9
	[PunRPC]
	public void RPCA_DisableLooker()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x00060FE8 File Offset: 0x0005F1E8
	private void SetRandomSwitch()
	{
		if (Random.Range(0f, 1f) < 0.1f)
		{
			this.untilSwitch = Random.Range(5f, 20f);
			return;
		}
		this.untilSwitch = Random.Range(1f, 5f);
	}

	// Token: 0x0600133D RID: 4925 RVA: 0x00061038 File Offset: 0x0005F238
	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.hasChecked)
		{
			if (PhotonNetwork.IsMasterClient && base.transform.GetSiblingIndex() == 0)
			{
				this.ToggleLookers();
			}
			this.hasChecked = true;
		}
		Transform transform = MainCamera.instance.transform;
		this.pivot.LookAt(transform.position);
		if (this.neverReturn)
		{
			this.selfDestructCounter -= Time.deltaTime;
			if (this.selfDestructCounter <= 0f)
			{
				Object.Destroy(this.pivot.gameObject);
				Object.Destroy(this);
			}
			return;
		}
		this.untilSwitch -= Time.deltaTime;
		float num = Vector3.Distance(transform.position, this.pivot.position);
		bool flag = Vector3.Dot(transform.forward, (this.pivot.position - transform.position).normalized) > 0.8f && num < 40f;
		if (this.isActive && flag)
		{
			this.untilSwitch -= Time.deltaTime * 5f;
			this.hasLookedAtMeFor += Time.deltaTime;
		}
		bool flag2 = this.hasLookedAtMeFor > 3f;
		bool flag3 = num < 5f;
		if (this.view.IsMine && this.untilSwitch <= 0f)
		{
			this.isActive = !this.isActive;
			this.view.RPC("RPCA_Switch", RpcTarget.All, new object[]
			{
				this.isActive
			});
		}
		if (flag2 || flag3)
		{
			this.view.RPC("RPCA_CodeRed", RpcTarget.AllBuffered, Array.Empty<object>());
		}
	}

	// Token: 0x0600133E RID: 4926 RVA: 0x000611EC File Offset: 0x0005F3EC
	[PunRPC]
	private void RPCA_Switch(bool switchTo)
	{
		if (this.neverReturn)
		{
			return;
		}
		this.anim.SetBool("IsActive", switchTo);
		this.SetRandomSwitch();
	}

	// Token: 0x0600133F RID: 4927 RVA: 0x0006120E File Offset: 0x0005F40E
	[PunRPC]
	private void RPCA_CodeRed()
	{
		this.anim.SetBool("IsActive", false);
		this.neverReturn = true;
	}

	// Token: 0x0400114C RID: 4428
	public GameObject guy;

	// Token: 0x0400114D RID: 4429
	private Animator anim;

	// Token: 0x0400114E RID: 4430
	private Transform pivot;

	// Token: 0x0400114F RID: 4431
	private float untilSwitch;

	// Token: 0x04001150 RID: 4432
	private bool neverReturn;

	// Token: 0x04001151 RID: 4433
	private float selfDestructCounter = 3f;

	// Token: 0x04001152 RID: 4434
	private bool isActive;

	// Token: 0x04001153 RID: 4435
	private PhotonView view;

	// Token: 0x04001154 RID: 4436
	private bool hasChecked;

	// Token: 0x04001155 RID: 4437
	private float hasLookedAtMeFor;
}
