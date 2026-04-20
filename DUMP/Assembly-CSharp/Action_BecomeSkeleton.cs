using System;

// Token: 0x020000E0 RID: 224
public class Action_BecomeSkeleton : ItemAction
{
	// Token: 0x06000867 RID: 2151 RVA: 0x0002F11E File Offset: 0x0002D31E
	public override void RunAction()
	{
		if (base.character.IsLocal)
		{
			base.character.data.SetSkeleton(!base.character.data.isSkeleton);
		}
	}
}
