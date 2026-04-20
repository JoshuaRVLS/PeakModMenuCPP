using System;

// Token: 0x020000F4 RID: 244
public class Action_Passport : ItemAction
{
	// Token: 0x0600089B RID: 2203 RVA: 0x0002F9F0 File Offset: 0x0002DBF0
	public override void RunAction()
	{
		PassportManager.instance.ToggleOpen();
	}
}
