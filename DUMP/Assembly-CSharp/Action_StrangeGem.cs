using System;

// Token: 0x020000FC RID: 252
public class Action_StrangeGem : ItemAction
{
	// Token: 0x060008B3 RID: 2227 RVA: 0x0002FEAB File Offset: 0x0002E0AB
	public override void RunAction()
	{
		Action_StrangeGem.gemActive = !Action_StrangeGem.gemActive;
		GlobalEvents.TriggerGemActivated(Action_StrangeGem.gemActive);
	}

	// Token: 0x04000845 RID: 2117
	public static bool gemActive;
}
