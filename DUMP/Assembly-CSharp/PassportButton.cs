using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001DD RID: 477
public class PassportButton : MonoBehaviour
{
	// Token: 0x06000F3A RID: 3898 RVA: 0x0004B174 File Offset: 0x00049374
	public void SetButton(CustomizationOption option, int index)
	{
		if (option != null)
		{
			base.gameObject.SetActive(true);
			if (option.IsLocked && !this.manager.testUnlockAll)
			{
				this.lockedIcon.gameObject.SetActive(true);
				this.icon.gameObject.SetActive(false);
			}
			else
			{
				this.lockedIcon.gameObject.SetActive(false);
				this.icon.gameObject.SetActive(true);
				this.icon.texture = option.texture;
				if (option.type == Customization.Type.Skin)
				{
					this.icon.color = option.color;
				}
				else
				{
					this.icon.color = Color.white;
				}
				if (option.type == Customization.Type.Eyes)
				{
					this.icon.material = this.eyeMaterial;
				}
				else
				{
					this.icon.material = null;
				}
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		this.currentOption = option;
		this.currentIndex = index;
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x0004B278 File Offset: 0x00049478
	public void Click()
	{
		if (!this.currentOption.IsLocked || this.manager.testUnlockAll)
		{
			this.manager.SetOption(this.currentOption, this.currentIndex);
		}
	}

	// Token: 0x04000CC9 RID: 3273
	public Button button;

	// Token: 0x04000CCA RID: 3274
	public PassportManager manager;

	// Token: 0x04000CCB RID: 3275
	public RawImage icon;

	// Token: 0x04000CCC RID: 3276
	public RawImage lockedIcon;

	// Token: 0x04000CCD RID: 3277
	public Image border;

	// Token: 0x04000CCE RID: 3278
	private CustomizationOption currentOption;

	// Token: 0x04000CCF RID: 3279
	private int currentIndex;

	// Token: 0x04000CD0 RID: 3280
	public Material eyeMaterial;
}
