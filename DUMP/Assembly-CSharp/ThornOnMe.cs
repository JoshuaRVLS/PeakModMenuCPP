using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class ThornOnMe : MonoBehaviour, IInteractibleConstant, IInteractible
{
	// Token: 0x060003D7 RID: 983 RVA: 0x000195AE File Offset: 0x000177AE
	private void OnEnable()
	{
		if (this.mainRenderer == null)
		{
			this.AddPropertyBlock();
		}
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x000195C4 File Offset: 0x000177C4
	private float GetPopOutTime(bool solo)
	{
		if (!solo)
		{
			return 120f;
		}
		return 30f;
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x000195D4 File Offset: 0x000177D4
	public bool ShouldPopOut()
	{
		return this.stuckIn && Time.time > this.popOutTime;
	}

	// Token: 0x060003DA RID: 986 RVA: 0x000195F0 File Offset: 0x000177F0
	public void EnableThorn()
	{
		if (!this.character.IsLocal || this.visibleLocally)
		{
			base.gameObject.SetActive(true);
		}
		this.stuckIn = true;
		this.popOutTime = Time.time + this.GetPopOutTime(Character.AllCharacters.Count == 1);
	}

	// Token: 0x060003DB RID: 987 RVA: 0x00019644 File Offset: 0x00017844
	public void DisableThorn()
	{
		base.gameObject.SetActive(false);
		this.stuckIn = false;
	}

	// Token: 0x060003DC RID: 988 RVA: 0x0001965C File Offset: 0x0001785C
	public bool IsInteractible(Character interactor)
	{
		if (interactor.IsStuck())
		{
			return false;
		}
		float num = Vector3.Angle(base.transform.position - MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
		float num2 = (Character.AllCharacters.Count == 1 || interactor != this.character) ? 15f : 0f;
		return num <= 5f + num2;
	}

	// Token: 0x060003DD RID: 989 RVA: 0x000196DA File Offset: 0x000178DA
	public void Interact(Character interactor)
	{
	}

	// Token: 0x060003DE RID: 990 RVA: 0x000196DC File Offset: 0x000178DC
	private void AddPropertyBlock()
	{
		this.mpb = new MaterialPropertyBlock();
		this.mainRenderer = base.GetComponentInChildren<MeshRenderer>();
		if (!this.mainRenderer)
		{
			this.mainRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>();
		}
		this.mainRenderer.GetPropertyBlock(this.mpb);
	}

	// Token: 0x060003DF RID: 991 RVA: 0x0001972A File Offset: 0x0001792A
	public void HoverEnter()
	{
		this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
		this.mainRenderer.SetPropertyBlock(this.mpb);
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00019752 File Offset: 0x00017952
	public void HoverExit()
	{
		this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
		this.mainRenderer.SetPropertyBlock(this.mpb);
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x0001977A File Offset: 0x0001797A
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00019787 File Offset: 0x00017987
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x0001978F File Offset: 0x0001798F
	public string GetInteractionText()
	{
		return LocalizedText.GetText("REMOVE", true);
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x0001979C File Offset: 0x0001799C
	public string GetName()
	{
		return LocalizedText.GetText("Name_Thorn", true);
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x000197A9 File Offset: 0x000179A9
	public bool IsConstantlyInteractable(Character interactor)
	{
		return true;
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x000197AC File Offset: 0x000179AC
	public float GetInteractTime(Character interactor)
	{
		if (Character.AllCharacters.Count != 1 && !(interactor != this.character))
		{
			return 3f;
		}
		return 1f;
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x000197D4 File Offset: 0x000179D4
	public void Interact_CastFinished(Character interactor)
	{
		this.character.refs.afflictions.RemoveThorn(this);
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x000197EC File Offset: 0x000179EC
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x000197EE File Offset: 0x000179EE
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x060003EA RID: 1002 RVA: 0x000197F0 File Offset: 0x000179F0
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0400041B RID: 1051
	[HideInInspector]
	public Character character;

	// Token: 0x0400041C RID: 1052
	public int thornDamage;

	// Token: 0x0400041D RID: 1053
	public bool stuckIn;

	// Token: 0x0400041E RID: 1054
	public bool visibleLocally;

	// Token: 0x0400041F RID: 1055
	private float popOutTime;

	// Token: 0x04000420 RID: 1056
	private MaterialPropertyBlock mpb;

	// Token: 0x04000421 RID: 1057
	public Renderer mainRenderer;
}
