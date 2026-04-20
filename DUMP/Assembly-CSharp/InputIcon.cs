using System;
using TMPro;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020001CD RID: 461
public class InputIcon : MonoBehaviour
{
	// Token: 0x06000EBE RID: 3774 RVA: 0x00049945 File Offset: 0x00047B45
	private void Awake()
	{
		this.text = base.GetComponent<TMP_Text>();
	}

	// Token: 0x06000EBF RID: 3775 RVA: 0x00049953 File Offset: 0x00047B53
	private void Start()
	{
		this.setting = GameHandler.Instance.SettingsHandler.GetSetting<ControllerIconSetting>();
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x0004996A File Offset: 0x00047B6A
	private void OnEnable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Combine(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
		this.OnDeviceChange(InputHandler.GetCurrentUsedInputScheme());
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x0004999D File Offset: 0x00047B9D
	private void OnDisable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Remove(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x000499C8 File Offset: 0x00047BC8
	private void OnDeviceChange(InputScheme scheme)
	{
		if (this.setting == null)
		{
			if (GameHandler.Instance == null)
			{
				return;
			}
			this.setting = GameHandler.Instance.SettingsHandler.GetSetting<ControllerIconSetting>();
		}
		if (scheme == InputScheme.KeyboardMouse || this.setting.Value == ControllerIconSetting.IconMode.KBM)
		{
			this.text.spriteAsset = this.keyboardSprites;
		}
		else if (scheme == InputScheme.Gamepad)
		{
			if (this.setting.Value == ControllerIconSetting.IconMode.Auto)
			{
				GamepadType gamepadType = InputHandler.GetGamepadType();
				if (gamepadType == GamepadType.Xbox)
				{
					this.text.spriteAsset = this.xboxSprites;
				}
				else if (gamepadType == GamepadType.Dualshock)
				{
					this.text.spriteAsset = this.ps5Sprites;
				}
				else if (gamepadType == GamepadType.Dualsense)
				{
					this.text.spriteAsset = this.ps5Sprites;
				}
				else if (gamepadType == GamepadType.SteamDeck)
				{
					this.text.spriteAsset = this.xboxSprites;
				}
				else if (gamepadType == GamepadType.Unkown)
				{
					this.text.spriteAsset = this.xboxSprites;
				}
			}
			else if (this.setting.Value == ControllerIconSetting.IconMode.Style1)
			{
				this.text.spriteAsset = this.xboxSprites;
			}
			else if (this.setting.Value == ControllerIconSetting.IconMode.Style2)
			{
				this.text.spriteAsset = this.ps5Sprites;
			}
		}
		this.SetText(scheme);
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x00049B00 File Offset: 0x00047D00
	private void SetText(InputScheme scheme)
	{
		if (scheme == InputScheme.Gamepad)
		{
			this.text.enabled = !this.disableIfController;
		}
		else if (scheme == InputScheme.KeyboardMouse)
		{
			this.text.enabled = !this.disableIfKeyboard;
		}
		string value;
		if (scheme == InputScheme.Gamepad && this.action == InputSpriteData.InputAction.Scroll)
		{
			value = SingletonAsset<InputSpriteData>.Instance.GetSpriteTag(InputSpriteData.InputAction.ScrollForward, scheme) + SingletonAsset<InputSpriteData>.Instance.GetSpriteTag(InputSpriteData.InputAction.ScrollBackward, scheme);
		}
		else
		{
			value = SingletonAsset<InputSpriteData>.Instance.GetSpriteTag(this.action, scheme);
		}
		if (!string.IsNullOrEmpty(value))
		{
			this.text.text = value;
		}
		if (scheme == InputScheme.Gamepad && this.hold != null)
		{
			this.hold.SetActive(this.action == InputSpriteData.InputAction.Throw || this.action == InputSpriteData.InputAction.HoldInteract);
		}
	}

	// Token: 0x04000C7A RID: 3194
	private TMP_Text text;

	// Token: 0x04000C7B RID: 3195
	public GameObject hold;

	// Token: 0x04000C7C RID: 3196
	public InputSpriteData.InputAction action;

	// Token: 0x04000C7D RID: 3197
	public TMP_SpriteAsset keyboardSprites;

	// Token: 0x04000C7E RID: 3198
	public TMP_SpriteAsset xboxSprites;

	// Token: 0x04000C7F RID: 3199
	public TMP_SpriteAsset switchSprites;

	// Token: 0x04000C80 RID: 3200
	public TMP_SpriteAsset ps5Sprites;

	// Token: 0x04000C81 RID: 3201
	public TMP_SpriteAsset ps4Sprites;

	// Token: 0x04000C82 RID: 3202
	public bool disableIfController;

	// Token: 0x04000C83 RID: 3203
	public bool disableIfKeyboard;

	// Token: 0x04000C84 RID: 3204
	private ControllerIconSetting setting;
}
