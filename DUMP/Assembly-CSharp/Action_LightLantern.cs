using System;

// Token: 0x020000EE RID: 238
public class Action_LightLantern : ItemAction
{
	// Token: 0x0600088E RID: 2190 RVA: 0x0002F74C File Offset: 0x0002D94C
	private void Awake()
	{
		this.lantern = base.GetComponent<Lantern>();
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x0002F75A File Offset: 0x0002D95A
	public override void RunAction()
	{
		this.lantern.ToggleLantern();
	}

	// Token: 0x04000829 RID: 2089
	private Lantern lantern;
}
