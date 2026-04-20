using System;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class Action_AddOrRemoveThorns : ItemAction
{
	// Token: 0x06000856 RID: 2134 RVA: 0x0002EDDC File Offset: 0x0002CFDC
	public override void RunAction()
	{
		int i = this.thornCount;
		if (i > 0)
		{
			while (i > 0)
			{
				if (this.specificBodyPart)
				{
					Vector3 vector = Vector3.Lerp(this.minOffset, this.maxOffset, Random.Range(0f, 1f));
					Transform transform = base.character.GetBodypart(this.location).transform;
					Vector3 position = transform.position + transform.TransformVector(vector);
					base.character.refs.afflictions.AddThorn(position);
				}
				else
				{
					base.character.refs.afflictions.AddThorn(999);
				}
				i--;
			}
			return;
		}
		while (i < 0)
		{
			base.character.refs.afflictions.RemoveRandomThornLinq();
			i++;
		}
	}

	// Token: 0x0400080A RID: 2058
	public int thornCount;

	// Token: 0x0400080B RID: 2059
	public bool specificBodyPart;

	// Token: 0x0400080C RID: 2060
	public BodypartType location;

	// Token: 0x0400080D RID: 2061
	public Vector3 minOffset;

	// Token: 0x0400080E RID: 2062
	public Vector3 maxOffset;
}
