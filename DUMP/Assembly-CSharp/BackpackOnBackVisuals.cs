using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class BackpackOnBackVisuals : BackpackVisuals, IInteractibleConstant, IInteractible
{
	// Token: 0x060004BB RID: 1211 RVA: 0x0001CA76 File Offset: 0x0001AC76
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
		this.InitRenderers();
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0001CA8A File Offset: 0x0001AC8A
	private void OnEnable()
	{
		this.RefreshCooking();
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0001CA94 File Offset: 0x0001AC94
	private void InitRenderers()
	{
		this.renderers = base.GetComponentsInChildren<MeshRenderer>();
		this.defaultTints = new Color[this.renderers.Length];
		for (int i = 0; i < this.renderers.Length; i++)
		{
			this.defaultTints[i] = this.renderers[i].material.GetColor("_Tint");
		}
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0001CAF8 File Offset: 0x0001ACF8
	private void RefreshCooking()
	{
		IntItemData intItemData;
		if (this.character.player.backpackSlot.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
		{
			this.CookVisually(intItemData.Value);
		}
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0001CB30 File Offset: 0x0001AD30
	protected virtual void CookVisually(int cookedAmount)
	{
		Debug.Log("Cooking backpack visually");
		if (this.renderers == null)
		{
			this.InitRenderers();
		}
		for (int i = 0; i < this.renderers.Length; i++)
		{
			if (cookedAmount > 0)
			{
				Debug.Log(string.Format("Cooked amount is {0}", cookedAmount));
				this.renderers[i].material.SetColor("_Tint", this.defaultTints[i] * ItemCooking.GetCookColor(cookedAmount));
			}
		}
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0001CBB0 File Offset: 0x0001ADB0
	public override BackpackData GetBackpackData()
	{
		BackpackData result;
		if (!this.character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out result))
		{
			this.character.player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
		}
		return result;
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0001CBF9 File Offset: 0x0001ADF9
	protected override void PutItemInBackpack(GameObject visual, byte slotID)
	{
		visual.GetComponent<PhotonView>().RPC("PutInBackpackRPC", RpcTarget.All, new object[]
		{
			slotID,
			BackpackReference.GetFromEquippedBackpack(this.character)
		});
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0001CC30 File Offset: 0x0001AE30
	public bool IsInteractible(Character interactor)
	{
		Vector3 from = HelperFunctions.ZeroY(interactor.data.lookDirection);
		Vector3 to = HelperFunctions.ZeroY(base.transform.forward);
		return Vector3.Angle(from, to) < 110f;
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0001CC6B File Offset: 0x0001AE6B
	public void Interact(Character interactor)
	{
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x0001CC70 File Offset: 0x0001AE70
	public void HoverEnter()
	{
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren)
		{
			componentInChildren.material.SetFloat(BackpackOnBackVisuals.Interactable, 1f);
		}
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x0001CCA4 File Offset: 0x0001AEA4
	public void HoverExit()
	{
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren)
		{
			componentInChildren.material.SetFloat(BackpackOnBackVisuals.Interactable, 0f);
		}
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x0001CCD5 File Offset: 0x0001AED5
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0001CCE2 File Offset: 0x0001AEE2
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x0001CCEA File Offset: 0x0001AEEA
	public string GetInteractionText()
	{
		return LocalizedText.GetText("open", true);
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x0001CCF7 File Offset: 0x0001AEF7
	public string GetName()
	{
		return LocalizedText.GetText("SOMEONESBACKPACK", true).Replace("#", this.character.characterName);
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0001CD19 File Offset: 0x0001AF19
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.IsInteractible(interactor);
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x0001CD22 File Offset: 0x0001AF22
	public float GetInteractTime(Character interactor)
	{
		return this.openRadialMenuTime;
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x0001CD2A File Offset: 0x0001AF2A
	public void Interact_CastFinished(Character interactor)
	{
		GUIManager.instance.OpenBackpackWheel(BackpackReference.GetFromEquippedBackpack(this.character));
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0001CD41 File Offset: 0x0001AF41
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0001CD43 File Offset: 0x0001AF43
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x060004CF RID: 1231 RVA: 0x0001CD45 File Offset: 0x0001AF45
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0400052C RID: 1324
	private static readonly int Interactable = Shader.PropertyToID("_Interactable");

	// Token: 0x0400052D RID: 1325
	public Character character;

	// Token: 0x0400052E RID: 1326
	public float openRadialMenuTime = 0.25f;

	// Token: 0x0400052F RID: 1327
	private MeshRenderer[] renderers;

	// Token: 0x04000530 RID: 1328
	private Color[] defaultTints;
}
