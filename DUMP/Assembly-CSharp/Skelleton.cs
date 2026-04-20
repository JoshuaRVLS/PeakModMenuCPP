using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000342 RID: 834
public class Skelleton : MonoBehaviour, IInteractibleConstant, IInteractible
{
	// Token: 0x0600162B RID: 5675 RVA: 0x00070BF8 File Offset: 0x0006EDF8
	public void SpawnSkelly(Character target)
	{
		this.spawnedFromCharacter = target;
		foreach (Bodypart bodypart in base.transform.GetComponentsInChildren<Bodypart>())
		{
			Bodypart bodypart2 = target.GetBodypart(bodypart.partType);
			if (!(bodypart2 == null))
			{
				bodypart.transform.position = bodypart2.transform.position;
				bodypart.transform.rotation = bodypart2.transform.rotation;
			}
		}
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x00070C6C File Offset: 0x0006EE6C
	private void Update()
	{
		if (this.spawnedFromCharacter && !this.spawnedFromCharacter.data.dead && this.spawnedFromCharacter.data.isSkeleton)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x00070CAA File Offset: 0x0006EEAA
	public void SetCharacter(Character character)
	{
		this.spawnedFromCharacter = character;
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x00070CB4 File Offset: 0x0006EEB4
	private bool CharacterHasBook(Character character)
	{
		Item currentItem = character.data.currentItem;
		return !(currentItem == null) && (currentItem.gameObject.CompareTag("BookOfBones") && currentItem.GetData<OptionableIntItemData>(DataEntryKey.ItemUses).Value > 0);
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x00070CFC File Offset: 0x0006EEFC
	public bool IsInteractible(Character interactor)
	{
		return this.CharacterHasBook(interactor);
	}

	// Token: 0x06001630 RID: 5680 RVA: 0x00070D08 File Offset: 0x0006EF08
	public void Interact(Character interactor)
	{
		Item currentItem = interactor.data.currentItem;
		if (currentItem == null)
		{
			return;
		}
		if (currentItem.gameObject.CompareTag("BookOfBones"))
		{
			Action_BookOfBonesAnim component = currentItem.GetComponent<Action_BookOfBonesAnim>();
			if (component == null)
			{
				return;
			}
			component.Open(true);
		}
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x00070D4E File Offset: 0x0006EF4E
	public void HoverEnter()
	{
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x00070D50 File Offset: 0x0006EF50
	public void HoverExit()
	{
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x00070D52 File Offset: 0x0006EF52
	public Vector3 Center()
	{
		return this.hip.transform.position;
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x00070D64 File Offset: 0x0006EF64
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x00070D6C File Offset: 0x0006EF6C
	public string GetInteractionText()
	{
		return LocalizedText.GetText("REVIVE_BOOKOFBONES", true);
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x00070D7C File Offset: 0x0006EF7C
	public string GetName()
	{
		if (this.spawnedFromCharacter != null)
		{
			return LocalizedText.GetText("NAME_SKELETON_PLAYER", true).Replace("#", this.spawnedFromCharacter.characterName.ToUpper());
		}
		return LocalizedText.GetText("NAME_SKELETON", true);
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x00070DC8 File Offset: 0x0006EFC8
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.CharacterHasBook(interactor);
	}

	// Token: 0x06001638 RID: 5688 RVA: 0x00070DD1 File Offset: 0x0006EFD1
	public float GetInteractTime(Character interactor)
	{
		return this.revivalTime;
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x00070DDC File Offset: 0x0006EFDC
	public void Interact_CastFinished(Character interactor)
	{
		if (this.spawnedFromCharacter)
		{
			this.spawnedFromCharacter.data.SetSkeleton(true);
			if (interactor.IsLocal)
			{
				Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.ScoutsResurrected, 1);
			}
			this.spawnedFromCharacter.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[]
			{
				this.hip.transform.position + base.transform.up * 1f,
				true,
				-1
			});
			this.ReleaseInteract(interactor);
			Action_ReduceUses action_ReduceUses;
			if (interactor.data.currentItem && interactor.data.currentItem.TryGetComponent<Action_ReduceUses>(out action_ReduceUses))
			{
				action_ReduceUses.RunAction();
			}
			if (interactor.IsLocal)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.AppliedEsotericaBadge);
			}
		}
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x00070EC5 File Offset: 0x0006F0C5
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x0600163B RID: 5691 RVA: 0x00070EC8 File Offset: 0x0006F0C8
	public void ReleaseInteract(Character interactor)
	{
		Item currentItem = interactor.data.currentItem;
		if (currentItem == null)
		{
			return;
		}
		if (currentItem.gameObject.CompareTag("BookOfBones"))
		{
			Action_BookOfBonesAnim component = currentItem.GetComponent<Action_BookOfBonesAnim>();
			if (component == null)
			{
				return;
			}
			component.Close();
		}
	}

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x0600163C RID: 5692 RVA: 0x00070F0D File Offset: 0x0006F10D
	public bool holdOnFinish { get; }

	// Token: 0x04001450 RID: 5200
	[SerializeField]
	private Character spawnedFromCharacter;

	// Token: 0x04001451 RID: 5201
	public Transform hip;

	// Token: 0x04001452 RID: 5202
	public float revivalTime;
}
