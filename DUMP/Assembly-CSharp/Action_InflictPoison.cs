using System;
using Peak.Afflictions;

// Token: 0x020000EC RID: 236
public class Action_InflictPoison : ItemAction
{
	// Token: 0x0600088A RID: 2186 RVA: 0x0002F6DC File Offset: 0x0002D8DC
	public override void RunAction()
	{
		base.character.refs.afflictions.AddAffliction(new Affliction_PoisonOverTime(this.inflictionTime, this.delay, this.poisonPerSecond), false);
	}

	// Token: 0x04000825 RID: 2085
	public float inflictionTime;

	// Token: 0x04000826 RID: 2086
	public float poisonPerSecond;

	// Token: 0x04000827 RID: 2087
	public float delay;
}
