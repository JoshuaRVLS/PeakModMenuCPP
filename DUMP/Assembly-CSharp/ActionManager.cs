using System;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class ActionManager : MonoBehaviour
{
	// Token: 0x0600043D RID: 1085 RVA: 0x0001ADC0 File Offset: 0x00018FC0
	private void Start()
	{
		if (base.GetComponent<Animator>())
		{
			this.anim = base.GetComponent<Animator>();
		}
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x0001ADDC File Offset: 0x00018FDC
	private void Update()
	{
		if (this.anim)
		{
			this.anim.SetBool("Jump Cancel", this.jumpCancel);
			this.anim.SetBool("Attack Cancel", this.attackCancel);
			this.anim.SetBool("Continuable", this.continuable);
			this.anim.SetBool("Fall Cancel", this.fallCancel);
			this.anim.SetBool("Dash Cancel", this.dashCancel);
			this.anim.SetBool("Crouch Cancel", this.crouchCancel);
			this.anim.SetBool("Special State", this.specialState);
			if (this.actionTimer <= 0f)
			{
				this.anim.SetBool("Action", false);
			}
			if (this.actionTimer > 0f)
			{
				this.anim.SetBool("Action", true);
			}
			if (this.edgeCaseTimer <= 0f)
			{
				this.anim.SetBool("Edge Case", false);
			}
			if (this.edgeCaseTimer > 0f)
			{
				this.anim.SetBool("Edge Case", true);
			}
		}
		this.actionTimer -= Time.deltaTime;
		this.edgeCaseTimer -= Time.deltaTime;
		if (this.actionTimer <= 0f)
		{
			this.actionTimer = 0f;
		}
		if (this.edgeCaseTimer <= 0f)
		{
			this.edgeCaseTimer = 0f;
		}
	}

	// Token: 0x040004A9 RID: 1193
	public float actionTimer;

	// Token: 0x040004AA RID: 1194
	public float edgeCaseTimer;

	// Token: 0x040004AB RID: 1195
	public Animator anim;

	// Token: 0x040004AC RID: 1196
	public bool fallCancel = true;

	// Token: 0x040004AD RID: 1197
	public bool jumpCancel = true;

	// Token: 0x040004AE RID: 1198
	public bool attackCancel = true;

	// Token: 0x040004AF RID: 1199
	public bool dashCancel = true;

	// Token: 0x040004B0 RID: 1200
	public bool crouchCancel = true;

	// Token: 0x040004B1 RID: 1201
	public bool continuable;

	// Token: 0x040004B2 RID: 1202
	public bool specialState;
}
