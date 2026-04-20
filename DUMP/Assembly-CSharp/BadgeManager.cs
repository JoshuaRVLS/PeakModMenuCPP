using System;
using TMPro;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class BadgeManager : MonoBehaviour
{
	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000E61 RID: 3681 RVA: 0x0004851E File Offset: 0x0004671E
	// (set) Token: 0x06000E62 RID: 3682 RVA: 0x00048528 File Offset: 0x00046728
	public BadgeUI selectedBadge
	{
		get
		{
			return this._selectedBadge;
		}
		set
		{
			this._selectedBadge = value;
			if (this._selectedBadge != null && this._selectedBadge.data != null)
			{
				if (this._selectedBadge.data.IsLocked)
				{
					this.badgePopupName.text = "???";
					this.badgePopupDescription.text = LocalizedText.GetText(LocalizedText.GetDescriptionIndex(this._selectedBadge.data.displayName), true);
				}
				else
				{
					this.badgePopupName.text = LocalizedText.GetText(LocalizedText.GetNameIndex(this._selectedBadge.data.displayName), true);
					this.badgePopupDescription.text = LocalizedText.GetText(LocalizedText.GetDescriptionIndex(this._selectedBadge.data.displayName), true);
				}
			}
			else
			{
				this.badgePopupName.text = "???";
				this.badgePopupDescription.text = LocalizedText.GetText("BADGELOCKED", true);
			}
			this.badgePopupAnim.Play("Popup", 0, 0f);
		}
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x00048637 File Offset: 0x00046837
	public void InheritData(BadgeManager other)
	{
		this.badgeData = new BadgeData[other.badgeData.Length];
		other.badgeData.CopyTo(this.badgeData, 0);
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x0004865E File Offset: 0x0004685E
	private void OnEnable()
	{
		this.selectedBadge = null;
		if (this.initBadgesOnEnable)
		{
			this.InitBadges();
		}
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x00048678 File Offset: 0x00046878
	public BadgeData GetBadgeData(ACHIEVEMENTTYPE achievementType)
	{
		foreach (BadgeData badgeData in this.badgeData)
		{
			if (!(badgeData == null) && badgeData.linkedAchievement == achievementType)
			{
				return badgeData;
			}
		}
		return null;
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x000486B4 File Offset: 0x000468B4
	private void InitBadges()
	{
		this.badges = base.GetComponentsInChildren<BadgeUI>();
		for (int i = 0; i < this.badges.Length; i++)
		{
			if (i < this.badgeData.Length)
			{
				this.badges[i].Init(this.badgeData[i]);
			}
			else
			{
				this.badges[i].Init(null);
			}
		}
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x00048710 File Offset: 0x00046910
	private void Update()
	{
		this.badgePopup.SetActive(this.selectedBadge != null);
		if (this.selectedBadge)
		{
			this.badgePopup.transform.position = this.selectedBadge.transform.position;
		}
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x00048764 File Offset: 0x00046964
	public void AddAllToCSV()
	{
		for (int i = 0; i < this.badgeData.Length; i++)
		{
			this.badgeData[i].AddToCSV();
		}
	}

	// Token: 0x04000C18 RID: 3096
	private BadgeUI _selectedBadge;

	// Token: 0x04000C19 RID: 3097
	public GameObject badgePopup;

	// Token: 0x04000C1A RID: 3098
	public Animator badgePopupAnim;

	// Token: 0x04000C1B RID: 3099
	public TextMeshProUGUI badgePopupName;

	// Token: 0x04000C1C RID: 3100
	public TextMeshProUGUI badgePopupDescription;

	// Token: 0x04000C1D RID: 3101
	public BadgeData[] badgeData;

	// Token: 0x04000C1E RID: 3102
	private BadgeUI[] badges;

	// Token: 0x04000C1F RID: 3103
	public bool initBadgesOnEnable;
}
