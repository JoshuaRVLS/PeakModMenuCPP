using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200022F RID: 559
public class CharacterInteractible : MonoBehaviour, IInteractibleConstant, IInteractible
{
	// Token: 0x0600112C RID: 4396 RVA: 0x00056841 File Offset: 0x00054A41
	private void Start()
	{
		this.character = base.GetComponent<Character>();
		this.localCannibalismSetting = GameHandler.Instance.SettingsHandler.GetSetting<CannibalismSetting>();
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x00056864 File Offset: 0x00054A64
	public Vector3 Center()
	{
		return this.character.Center;
	}

	// Token: 0x0600112E RID: 4398 RVA: 0x00056874 File Offset: 0x00054A74
	public string GetInteractionText()
	{
		if (this.CarriedByLocalCharacter())
		{
			return LocalizedText.GetText("DROP", true).Replace("#", this.GetName());
		}
		if (this.IsCannibal())
		{
			return LocalizedText.GetText("EAT", true);
		}
		if (this.CanBeCarried())
		{
			return LocalizedText.GetText("CARRY", true).Replace("#", this.GetName());
		}
		return "";
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x000568E2 File Offset: 0x00054AE2
	private bool IsCannibal()
	{
		return !this.character.isBot && this.character.refs.customization.isCannibalizable;
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x00056908 File Offset: 0x00054B08
	public string GetSecondaryInteractionText()
	{
		if (this.HasItemCanUseOnFriend())
		{
			return this.GetItemPrompt(Character.localCharacter.data.currentItem);
		}
		return "";
	}

	// Token: 0x06001131 RID: 4401 RVA: 0x0005692D File Offset: 0x00054B2D
	public string GetItemPrompt(Item item)
	{
		return LocalizedText.GetText(item.UIData.secondaryInteractPrompt, true).Replace("#targetchar", this.GetName());
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x00056950 File Offset: 0x00054B50
	public string GetName()
	{
		return this.character.characterName;
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x0005695D File Offset: 0x00054B5D
	private bool CarriedByLocalCharacter()
	{
		return this.character.data.carrier && this.character.data.carrier == Character.localCharacter;
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x00056994 File Offset: 0x00054B94
	private bool CanBeCarried()
	{
		return !this.character.isBot && (this.character.data.fullyPassedOut && !this.character.data.dead) && !this.character.data.carrier;
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x000569F0 File Offset: 0x00054BF0
	private bool HasItemCanUseOnFriend()
	{
		return !this.character.data.dead && this.character != Character.localCharacter && Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.canUseOnFriend;
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x00056A4D File Offset: 0x00054C4D
	public Transform GetTransform()
	{
		return this.character.GetBodypart(BodypartType.Torso).transform;
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x00056A60 File Offset: 0x00054C60
	public void HoverEnter()
	{
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x00056A62 File Offset: 0x00054C62
	public void HoverExit()
	{
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x00056A64 File Offset: 0x00054C64
	public void Interact(Character interactor)
	{
		if (this.CarriedByLocalCharacter())
		{
			interactor.refs.carriying.Drop(this.character);
			return;
		}
		if (this.IsCannibal())
		{
			return;
		}
		if (this.CanBeCarried())
		{
			interactor.refs.carriying.StartCarry(this.character);
			return;
		}
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x00056AB8 File Offset: 0x00054CB8
	public bool IsInteractible(Character interactor)
	{
		return !this.character.isBot && (this.IsPrimaryInteractible(interactor) || this.IsSecondaryInteractible(interactor));
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x00056ADB File Offset: 0x00054CDB
	public bool IsPrimaryInteractible(Character interactor)
	{
		return this.character.refs.customization.isCannibalizable || this.CarriedByLocalCharacter() || this.CanBeCarried();
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x00056B0C File Offset: 0x00054D0C
	public bool IsSecondaryInteractible(Character interactor)
	{
		if (!this.HasItemCanUseOnFriend())
		{
			return false;
		}
		if (this.character.data.fullyPassedOut)
		{
			return true;
		}
		Vector3 from = HelperFunctions.ZeroY(this.character.data.lookDirection);
		Vector3 a = HelperFunctions.ZeroY(interactor.data.lookDirection);
		return Vector3.Angle(from, -a) <= Interaction.instance.maxCharacterInteractAngle;
	}

	// Token: 0x0600113D RID: 4413 RVA: 0x00056B78 File Offset: 0x00054D78
	private void GetEaten(Character eater)
	{
		if (eater.IsLocal)
		{
			this.character.DieInstantly();
			eater.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f, false, false);
			eater.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 0.1f, false, true, true);
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.ResourcefulnessBadge);
		}
	}

	// Token: 0x0600113E RID: 4414 RVA: 0x00056BD6 File Offset: 0x00054DD6
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.character.refs.customization.isCannibalizable;
	}

	// Token: 0x0600113F RID: 4415 RVA: 0x00056BED File Offset: 0x00054DED
	public float GetInteractTime(Character interactor)
	{
		return 3f;
	}

	// Token: 0x06001140 RID: 4416 RVA: 0x00056BF4 File Offset: 0x00054DF4
	public void Interact_CastFinished(Character interactor)
	{
		if (interactor.IsLocal && this.character.refs.customization.isCannibalizable)
		{
			this.GetEaten(interactor);
		}
	}

	// Token: 0x06001141 RID: 4417 RVA: 0x00056C1C File Offset: 0x00054E1C
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x06001142 RID: 4418 RVA: 0x00056C1E File Offset: 0x00054E1E
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06001143 RID: 4419 RVA: 0x00056C20 File Offset: 0x00054E20
	public bool holdOnFinish { get; }

	// Token: 0x04000F10 RID: 3856
	public Character character;

	// Token: 0x04000F11 RID: 3857
	private CannibalismSetting localCannibalismSetting;
}
