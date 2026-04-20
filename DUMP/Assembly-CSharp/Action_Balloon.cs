using System;

// Token: 0x020000DF RID: 223
public class Action_Balloon : ItemAction
{
	// Token: 0x06000865 RID: 2149 RVA: 0x0002F08C File Offset: 0x0002D28C
	public override void RunAction()
	{
		if (base.character)
		{
			if (this.balloon.isBunch)
			{
				base.character.refs.balloons.TieNewBalloon(0);
				base.character.refs.balloons.TieNewBalloon(2);
				base.character.refs.balloons.TieNewBalloon(4);
				return;
			}
			base.character.refs.balloons.TieNewBalloon(this.balloon.colorIndex);
		}
	}

	// Token: 0x04000815 RID: 2069
	public Balloon balloon;
}
