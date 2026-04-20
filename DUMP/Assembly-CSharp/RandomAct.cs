using System;
using UnityEngine;

// Token: 0x02000318 RID: 792
public class RandomAct : MonoBehaviour
{
	// Token: 0x06001538 RID: 5432 RVA: 0x0006B25B File Offset: 0x0006945B
	private void Start()
	{
		base.GetComponent<Animator>().SetInteger("Act", this.act);
	}

	// Token: 0x0400134D RID: 4941
	public int act;
}
