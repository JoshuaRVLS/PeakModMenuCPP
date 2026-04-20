using System;

// Token: 0x020000ED RID: 237
public class Action_LaunchPlayer : ItemAction
{
	// Token: 0x0600088C RID: 2188 RVA: 0x0002F713 File Offset: 0x0002D913
	public override void RunAction()
	{
		base.character.AddForce(MainCamera.instance.transform.forward * this.force, 1f, 1f);
	}

	// Token: 0x04000828 RID: 2088
	public float force;
}
