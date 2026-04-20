using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class ClimbHandle : MonoBehaviour, IInteractible
{
	// Token: 0x0600117B RID: 4475 RVA: 0x00058346 File Offset: 0x00056546
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x00058354 File Offset: 0x00056554
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x00058361 File Offset: 0x00056561
	public string GetInteractionText()
	{
		return LocalizedText.GetText("GRAB", true);
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x0005836E File Offset: 0x0005656E
	public string GetName()
	{
		if (this.isPickaxe)
		{
			return LocalizedText.GetText("PICKAXE", true);
		}
		return LocalizedText.GetText("PITONPROMPT", true);
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0005838F File Offset: 0x0005658F
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x00058397 File Offset: 0x00056597
	public void HoverEnter()
	{
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x00058399 File Offset: 0x00056599
	public void HoverExit()
	{
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x0005839C File Offset: 0x0005659C
	public void Interact(Character interactor)
	{
		if (this.hanger)
		{
			return;
		}
		if (interactor.IsLocal)
		{
			interactor.refs.climbing.StopAnyClimbing();
		}
		else
		{
			Debug.LogWarning("How come someone who is not local is Interacting???");
		}
		this.view.RPC("RPCA_Hang", RpcTarget.All, new object[]
		{
			interactor.photonView
		});
	}

	// Token: 0x06001183 RID: 4483 RVA: 0x000583FC File Offset: 0x000565FC
	[PunRPC]
	public void RPCA_Hang(PhotonView view)
	{
		if (view == null)
		{
			return;
		}
		Character component = view.GetComponent<Character>();
		if (component == null)
		{
			return;
		}
		this.hanger = component;
		component.refs.climbing.StartHang(this);
		Action<Character> action = this.onHangStart;
		if (action == null)
		{
			return;
		}
		action(component);
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x00058450 File Offset: 0x00056650
	[PunRPC]
	public void RPCA_UnHang(PhotonView view)
	{
		this.hanger = null;
		if (view == null)
		{
			return;
		}
		Character component = view.GetComponent<Character>();
		if (component == null)
		{
			return;
		}
		component.data.currentClimbHandle = null;
		Action action = this.onHangStop;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x0005849B File Offset: 0x0005669B
	public bool IsInteractible(Character interactor)
	{
		return this.hanger == null;
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x000584AC File Offset: 0x000566AC
	internal void Break()
	{
		if (this.hanger != null)
		{
			this.hanger.refs.climbing.CancelHandle(true);
		}
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
	}

	// Token: 0x04000F4B RID: 3915
	public Character hanger;

	// Token: 0x04000F4C RID: 3916
	internal PhotonView view;

	// Token: 0x04000F4D RID: 3917
	public bool isPickaxe;

	// Token: 0x04000F4E RID: 3918
	public Action<Character> onHangStart;

	// Token: 0x04000F4F RID: 3919
	public Action onHangStop;
}
