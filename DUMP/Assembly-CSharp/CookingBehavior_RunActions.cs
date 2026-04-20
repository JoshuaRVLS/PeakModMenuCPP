using System;

// Token: 0x02000050 RID: 80
public class CookingBehavior_RunActions : AdditionalCookingBehavior
{
	// Token: 0x0600044F RID: 1103 RVA: 0x0001B134 File Offset: 0x00019334
	protected override void TriggerBehaviour(int cookedAmount)
	{
		ItemAction[] array = this.actionsToRun;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RunAction();
		}
	}

	// Token: 0x040004BF RID: 1215
	public ItemAction[] actionsToRun;
}
