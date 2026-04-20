using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x0200021A RID: 538
[DefaultExecutionOrder(1000000)]
public class BingBongPowers : MonoBehaviour
{
	// Token: 0x0600107B RID: 4219 RVA: 0x00051E9E File Offset: 0x0005009E
	private void Start()
	{
		this.SetGodCamStyle();
		base.GetComponentInChildren<Canvas>().enabled = base.GetComponent<PhotonView>().IsMine;
	}

	// Token: 0x0600107C RID: 4220 RVA: 0x00051EBC File Offset: 0x000500BC
	private void SetGodCamStyle()
	{
		MainCameraMovement component = MainCamera.instance.GetComponent<MainCameraMovement>();
		component.godcam.lookSens = 20f;
		component.godcam.lookDrag = 5f;
		component.godcam.force = 15f;
		component.godcam.drag = 3f;
		component.godcam.canOrbit = false;
	}

	// Token: 0x0600107D RID: 4221 RVA: 0x00051F1E File Offset: 0x0005011E
	private void LateUpdate()
	{
		this.TogglePowers();
		base.transform.position = MainCamera.instance.transform.position;
		base.transform.rotation = MainCamera.instance.transform.rotation;
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x00051F5A File Offset: 0x0005015A
	private void TogglePowers()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			this.ToggleID(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			this.ToggleID(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			this.ToggleID(2);
		}
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x00051F8C File Offset: 0x0005018C
	private void ToggleID(int id)
	{
		base.GetComponent<BingBongPhysics>().enabled = false;
		base.GetComponent<BingBongTimeControl>().enabled = false;
		base.GetComponent<BingBongStatus>().enabled = false;
		if (id == 0)
		{
			base.GetComponent<BingBongPhysics>().enabled = true;
		}
		if (id == 1)
		{
			base.GetComponent<BingBongTimeControl>().enabled = true;
		}
		if (id == 2)
		{
			base.GetComponent<BingBongStatus>().enabled = true;
		}
		for (int i = 0; i < this.tooltipBar.childCount; i++)
		{
			if (i == id)
			{
				this.tooltipBar.GetChild(i).GetComponent<CanvasGroup>().alpha = 1f;
			}
			else
			{
				this.tooltipBar.GetChild(i).GetComponent<CanvasGroup>().alpha = 0.5f;
			}
		}
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x0005203E File Offset: 0x0005023E
	public void SetTexts(string titleDescr, string description)
	{
		this.titleText.text = titleDescr;
		this.descriptionText.text = description;
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x00052058 File Offset: 0x00050258
	public void SetTip(string tip, int toolID)
	{
		this.tooltipBar.GetChild(toolID).Find("Tip").GetComponent<TextMeshProUGUI>().text = tip;
	}

	// Token: 0x04000E6C RID: 3692
	public TextMeshProUGUI titleText;

	// Token: 0x04000E6D RID: 3693
	public TextMeshProUGUI descriptionText;

	// Token: 0x04000E6E RID: 3694
	public Transform tooltipBar;
}
