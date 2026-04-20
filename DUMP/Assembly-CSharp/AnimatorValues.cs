using System;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class AnimatorValues : MonoBehaviour
{
	// Token: 0x06001019 RID: 4121 RVA: 0x0004F7D4 File Offset: 0x0004D9D4
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.cD = base.GetComponentInParent<CharacterData>();
		this.cI = base.GetComponentInParent<CharacterInput>();
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x0004F7FC File Offset: 0x0004D9FC
	private void Update()
	{
		this.anim.SetFloat("Input X", this.cI.movementInput.x);
		this.anim.SetFloat("Input Y", this.cI.movementInput.y);
		this.anim.SetBool("Is Grounded", this.cD.isGrounded);
		if (this.cI.sprintIsPressed)
		{
			this.anim.SetFloat("Sprint", 1f, 0.125f, Time.deltaTime);
		}
		if (!this.cI.sprintIsPressed)
		{
			this.anim.SetFloat("Sprint", 0f, 0.125f, Time.deltaTime);
		}
		this.anim.SetFloat("Velocity Y", this.cD.avarageVelocity.y);
	}

	// Token: 0x04000DF0 RID: 3568
	private Animator anim;

	// Token: 0x04000DF1 RID: 3569
	private CharacterData cD;

	// Token: 0x04000DF2 RID: 3570
	private CharacterInput cI;
}
