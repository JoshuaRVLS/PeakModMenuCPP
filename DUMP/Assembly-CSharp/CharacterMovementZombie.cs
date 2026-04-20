using System;

// Token: 0x02000015 RID: 21
public class CharacterMovementZombie : CharacterMovement
{
	// Token: 0x060001FA RID: 506 RVA: 0x0000F8E5 File Offset: 0x0000DAE5
	private void Awake()
	{
		this.zombie = base.GetComponent<MushroomZombie>();
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000F8F3 File Offset: 0x0000DAF3
	protected override void EvaluateGroundChecks()
	{
		base.EvaluateGroundChecks();
		if (this.zombie.currentState == MushroomZombie.State.Lunging)
		{
			this.zombie.character.data.isGrounded = false;
		}
	}

	// Token: 0x040001D8 RID: 472
	private MushroomZombie zombie;
}
