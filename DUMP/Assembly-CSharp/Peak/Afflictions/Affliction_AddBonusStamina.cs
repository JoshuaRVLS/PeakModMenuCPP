using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x02000402 RID: 1026
	public class Affliction_AddBonusStamina : Affliction
	{
		// Token: 0x06001B11 RID: 6929 RVA: 0x000845CE File Offset: 0x000827CE
		public Affliction_AddBonusStamina()
		{
		}

		// Token: 0x06001B12 RID: 6930 RVA: 0x000845D6 File Offset: 0x000827D6
		public Affliction_AddBonusStamina(float staminaAmount, float totalTime) : base(totalTime)
		{
			this.staminaAmount = staminaAmount;
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x000845E6 File Offset: 0x000827E6
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.AddBonusStamina;
		}

		// Token: 0x06001B14 RID: 6932 RVA: 0x000845EA File Offset: 0x000827EA
		public override void Stack(Affliction incomingAffliction)
		{
			this.OnApplied();
		}

		// Token: 0x06001B15 RID: 6933 RVA: 0x000845F2 File Offset: 0x000827F2
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				Debug.Log("Adding extra stamina: " + this.staminaAmount.ToString());
				this.character.AddExtraStamina(this.staminaAmount);
			}
		}

		// Token: 0x06001B16 RID: 6934 RVA: 0x0008462C File Offset: 0x0008282C
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.staminaAmount);
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001B17 RID: 6935 RVA: 0x00084646 File Offset: 0x00082846
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.staminaAmount = serializer.ReadFloat();
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x0400179E RID: 6046
		public float staminaAmount;
	}
}
