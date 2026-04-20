using System;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class BotToCharacterTranslator : MonoBehaviour
{
	// Token: 0x06000513 RID: 1299 RVA: 0x0001E164 File Offset: 0x0001C364
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
		this.bot = base.GetComponentInParent<Bot>();
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0001E180 File Offset: 0x0001C380
	private void Update()
	{
		this.character.input.movementInput = this.bot.MovementInput;
		this.character.input.sprintIsPressed = this.bot.IsSprinting;
		this.character.data.lookValues = HelperFunctions.DirectionToLook(this.bot.LookDirection);
	}

	// Token: 0x0400056B RID: 1387
	private Character character;

	// Token: 0x0400056C RID: 1388
	private Bot bot;
}
