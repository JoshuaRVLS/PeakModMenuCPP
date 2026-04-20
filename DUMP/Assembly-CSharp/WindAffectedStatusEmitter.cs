using System;

// Token: 0x02000381 RID: 897
public class WindAffectedStatusEmitter : StatusEmitter
{
	// Token: 0x06001773 RID: 6003 RVA: 0x00079019 File Offset: 0x00077219
	private void FixedUpdate()
	{
		if (RootsWind.instance)
		{
			this.emitterDisabledByWind = RootsWind.instance.windZone.windActive;
			return;
		}
		this.emitterDisabledByWind = false;
	}
}
