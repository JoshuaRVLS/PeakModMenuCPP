using System;

// Token: 0x020000E4 RID: 228
public class Action_Consume : ItemAction
{
	// Token: 0x06000873 RID: 2163 RVA: 0x0002F3BE File Offset: 0x0002D5BE
	public override void RunAction()
	{
		if (base.character)
		{
			this.item.StartCoroutine(this.item.ConsumeDelayed(false));
		}
	}
}
