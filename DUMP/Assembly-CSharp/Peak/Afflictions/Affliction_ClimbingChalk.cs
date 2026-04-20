using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x02000409 RID: 1033
	public class Affliction_ClimbingChalk : Affliction
	{
		// Token: 0x06001B47 RID: 6983 RVA: 0x00084AC7 File Offset: 0x00082CC7
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.ClimbingChalk;
		}

		// Token: 0x06001B48 RID: 6984 RVA: 0x00084ACC File Offset: 0x00082CCC
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = Mathf.Max(this.totalTime, incomingAffliction.totalTime);
			Affliction_ClimbingChalk affliction_ClimbingChalk = incomingAffliction as Affliction_ClimbingChalk;
			if (affliction_ClimbingChalk != null)
			{
				this.climbStaminaMultiplier = Mathf.Min(affliction_ClimbingChalk.climbStaminaMultiplier, this.climbStaminaMultiplier);
			}
			this.timeElapsed = 0f;
		}

		// Token: 0x06001B49 RID: 6985 RVA: 0x00084B1C File Offset: 0x00082D1C
		public override void OnApplied()
		{
		}

		// Token: 0x06001B4A RID: 6986 RVA: 0x00084B1E File Offset: 0x00082D1E
		public override void OnRemoved()
		{
		}

		// Token: 0x06001B4B RID: 6987 RVA: 0x00084B20 File Offset: 0x00082D20
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.climbStaminaMultiplier);
		}

		// Token: 0x06001B4C RID: 6988 RVA: 0x00084B3A File Offset: 0x00082D3A
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.climbStaminaMultiplier = serializer.ReadFloat();
		}

		// Token: 0x040017A4 RID: 6052
		public float climbStaminaMultiplier;
	}
}
