using System;
using Peak.Network;
using UnityEngine;
using Zorro.Settings;

// Token: 0x0200036C RID: 876
public class UIPlayerNames : MonoBehaviour
{
	// Token: 0x06001717 RID: 5911 RVA: 0x00076A00 File Offset: 0x00074C00
	public int Init(CharacterInteractible characterInteractable)
	{
		this.indexCounter++;
		this.playerNameText[this.indexCounter - 1].characterInteractable = characterInteractable;
		this.playerNameText[this.indexCounter - 1].text.text = characterInteractable.GetName();
		for (int i = 0; i < this.playerNameText.Length; i++)
		{
			this.playerNameText[i].gameObject.SetActive(false);
		}
		this.localCannibalismSetting = GameHandler.Instance.SettingsHandler.GetSetting<CannibalismSetting>();
		return this.indexCounter - 1;
	}

	// Token: 0x06001718 RID: 5912 RVA: 0x00076A94 File Offset: 0x00074C94
	public void UpdateName(int index, Vector3 position, bool visible, int speakingAmplitude)
	{
		if (!Character.localCharacter)
		{
			return;
		}
		if (index >= this.playerNameText.Length)
		{
			return;
		}
		this.playerNameText[index].transform.position = MainCamera.instance.cam.WorldToScreenPoint(position);
		if (visible)
		{
			if (this.CanCannibalize(this.playerNameText[index].characterInteractable.character))
			{
				this.playerNameText[index].characterInteractable.character.refs.customization.BecomeChicken();
			}
			this.playerNameText[index].gameObject.SetActive(true);
			this.playerNameText[index].group.alpha = Mathf.MoveTowards(this.playerNameText[index].group.alpha, 1f, Time.deltaTime * 5f);
			if (this.playerNameText[index].characterInteractable && AudioLevels.GetPlayerLevel(this.playerNameText[index].characterInteractable.character.player.GetUserId()) == 0f)
			{
				this.playerNameText[index].audioImage.sprite = this.mutedAudioSprite;
				return;
			}
			if (speakingAmplitude <= 0)
			{
				this.playerNameText[index].audioImageTimeout -= Time.deltaTime;
				if (this.playerNameText[index].audioImageTimeout <= 0f)
				{
					this.playerNameText[index].audioImage.sprite = this.audioSprites[0];
				}
			}
			else
			{
				this.playerNameText[index].audioImage.sprite = this.audioSprites[Mathf.Clamp(speakingAmplitude * 2, 0, this.audioSprites.Length - 1)];
				this.playerNameText[index].audioImageTimeout = this.audioImageTimeoutMax;
			}
			this.playerNameText[index].hostStar.SetActive(this.playerNameText[index].characterInteractable.character.photonView.Owner.IsMasterClient);
		}
		else
		{
			this.playerNameText[index].group.alpha = Mathf.MoveTowards(this.playerNameText[index].group.alpha, 0f, Time.deltaTime * 5f);
			if (this.playerNameText[index].group.alpha < 0.01f && this.playerNameText[index].gameObject.activeSelf)
			{
				this.playerNameText[index].characterInteractable.character.refs.customization.BecomeHuman();
				this.playerNameText[index].gameObject.SetActive(false);
			}
		}
		if ((Character.localCharacter.data.fullyPassedOut || Character.localCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hunger) < UIPlayerNames.CANNIBAL_HUNGER_THRESHOLD) && this.playerNameText[index].gameObject.activeSelf)
		{
			this.playerNameText[index].characterInteractable.character.refs.customization.BecomeHuman();
		}
	}

	// Token: 0x06001719 RID: 5913 RVA: 0x00076D80 File Offset: 0x00074F80
	private bool CanCannibalize(Character otherCharacter)
	{
		return !otherCharacter.isBot && (!otherCharacter.refs.customization.isCannibalizable && Character.localCharacter.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hunger) >= UIPlayerNames.CANNIBAL_HUNGER_THRESHOLD && Character.localCharacter.data.fullyConscious && this.localCannibalismSetting.Value == OffOnMode.ON && otherCharacter.data.cannibalismPermitted);
	}

	// Token: 0x0600171A RID: 5914 RVA: 0x00076DF4 File Offset: 0x00074FF4
	public void DisableName(int index)
	{
		if (this.playerNameText[index])
		{
			this.playerNameText[index].gameObject.SetActive(false);
		}
	}

	// Token: 0x0400158A RID: 5514
	private int indexCounter;

	// Token: 0x0400158B RID: 5515
	public PlayerName[] playerNameText;

	// Token: 0x0400158C RID: 5516
	public Sprite[] audioSprites;

	// Token: 0x0400158D RID: 5517
	public Sprite mutedAudioSprite;

	// Token: 0x0400158E RID: 5518
	public CannibalismSetting localCannibalismSetting;

	// Token: 0x0400158F RID: 5519
	public float audioImageTimeoutMax = 1f;

	// Token: 0x04001590 RID: 5520
	public static float CANNIBAL_HUNGER_THRESHOLD = 0.7f;
}
