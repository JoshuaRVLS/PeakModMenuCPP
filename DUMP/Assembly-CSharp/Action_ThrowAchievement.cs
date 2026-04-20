using System;
using Zorro.Core;

// Token: 0x020000FD RID: 253
public class Action_ThrowAchievement : ItemAction
{
	// Token: 0x060008B5 RID: 2229 RVA: 0x0002FECC File Offset: 0x0002E0CC
	public override void RunAction()
	{
		if (this.item.holderCharacter && this.item.holderCharacter.IsLocal)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(this.achievementType);
		}
	}

	// Token: 0x04000846 RID: 2118
	public ACHIEVEMENTTYPE achievementType;
}
