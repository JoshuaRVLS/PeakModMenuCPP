using System;

// Token: 0x020000F3 RID: 243
public class Action_Parasol : ItemAction
{
	// Token: 0x06000899 RID: 2201 RVA: 0x0002F9DB File Offset: 0x0002DBDB
	public override void RunAction()
	{
		this.parasol.ToggleOpen();
	}

	// Token: 0x04000832 RID: 2098
	public Parasol parasol;
}
