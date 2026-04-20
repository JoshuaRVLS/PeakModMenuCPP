using System;
using UnityEngine;

// Token: 0x0200025F RID: 607
public class EndCutsceneScoutHelper : MonoBehaviour
{
	// Token: 0x06001214 RID: 4628 RVA: 0x0005AAF6 File Offset: 0x00058CF6
	private void OnEnable()
	{
		base.GetComponent<Animator>().SetBool("Alone", this.alone);
	}

	// Token: 0x04001016 RID: 4118
	public bool alone;
}
