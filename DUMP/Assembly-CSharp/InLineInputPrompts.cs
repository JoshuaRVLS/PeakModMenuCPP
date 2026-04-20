using System;
using TMPro;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020001CC RID: 460
[RequireComponent(typeof(TMP_Text))]
public class InLineInputPrompts : MonoBehaviour
{
	// Token: 0x06000EB7 RID: 3767 RVA: 0x00049678 File Offset: 0x00047878
	private void Awake()
	{
		this.text = base.GetComponent<TMP_Text>();
		this.loc = base.GetComponent<LocalizedText>();
		this.originalText = this.text.text;
		this.setting = GameHandler.Instance.SettingsHandler.GetSetting<ControllerIconSetting>();
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x000496B8 File Offset: 0x000478B8
	private void OnEnable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Combine(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
		this.OnDeviceChange(InputHandler.GetCurrentUsedInputScheme());
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x000496EB File Offset: 0x000478EB
	private void OnDisable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Remove(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x00049713 File Offset: 0x00047913
	private void OnDeviceChange(InputScheme scheme)
	{
		this.UpdateText(scheme);
		this.UpdateSprites(scheme);
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x00049724 File Offset: 0x00047924
	private void UpdateText(InputScheme scheme)
	{
		string text = this.originalText;
		if (this.loc)
		{
			text = this.loc.GetText();
			this.loc.enabled = false;
		}
		if (text.Contains("[") && text.Contains("]"))
		{
			foreach (object obj in Enum.GetValues(typeof(InputSpriteData.InputAction)))
			{
				if (text.ToUpperInvariant().Contains(obj.ToString().ToUpperInvariant()))
				{
					string spriteTag = SingletonAsset<InputSpriteData>.Instance.GetSpriteTag((InputSpriteData.InputAction)obj, scheme);
					if (!string.IsNullOrEmpty(spriteTag))
					{
						string oldValue = string.Format("[{0}]", obj).ToUpperInvariant();
						text = text.Replace(oldValue, spriteTag);
					}
				}
			}
		}
		this.text.text = text;
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x00049824 File Offset: 0x00047A24
	private void UpdateSprites(InputScheme scheme)
	{
		if (scheme == InputScheme.KeyboardMouse || this.setting.Value == ControllerIconSetting.IconMode.KBM)
		{
			this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.keyboardSprites;
			return;
		}
		if (scheme == InputScheme.Gamepad)
		{
			if (this.setting.Value == ControllerIconSetting.IconMode.Auto)
			{
				GamepadType gamepadType = InputHandler.GetGamepadType();
				if (gamepadType == GamepadType.Xbox)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
					return;
				}
				if (gamepadType == GamepadType.Dualshock)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.ps5Sprites;
					return;
				}
				if (gamepadType == GamepadType.Dualsense)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.ps5Sprites;
					return;
				}
				if (gamepadType == GamepadType.SteamDeck)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
					return;
				}
				if (gamepadType == GamepadType.Unkown)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
					return;
				}
			}
			else
			{
				if (this.setting.Value == ControllerIconSetting.IconMode.Style1)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.xboxSprites;
					return;
				}
				if (this.setting.Value == ControllerIconSetting.IconMode.Style2)
				{
					this.text.spriteAsset = SingletonAsset<InputSpriteData>.Instance.ps5Sprites;
				}
			}
		}
	}

	// Token: 0x04000C76 RID: 3190
	private TMP_Text text;

	// Token: 0x04000C77 RID: 3191
	private LocalizedText loc;

	// Token: 0x04000C78 RID: 3192
	private string originalText;

	// Token: 0x04000C79 RID: 3193
	private ControllerIconSetting setting;
}
