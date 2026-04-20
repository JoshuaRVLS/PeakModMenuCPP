using System;
using UnityEngine;

// Token: 0x02000258 RID: 600
public class DisableIKBehavior : StateMachineBehaviour
{
	// Token: 0x06001201 RID: 4609 RVA: 0x0005A7B4 File Offset: 0x000589B4
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.character == null)
		{
			this.character = animator.GetComponentInParent<Character>();
		}
		this.character.data.overrideIKForSeconds = 0.1f;
	}

	// Token: 0x04001005 RID: 4101
	private Character character;
}
