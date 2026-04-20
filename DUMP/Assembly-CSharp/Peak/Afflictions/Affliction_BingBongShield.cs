using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x02000404 RID: 1028
	public class Affliction_BingBongShield : Affliction
	{
		// Token: 0x06001B1F RID: 6943 RVA: 0x0008469E File Offset: 0x0008289E
		internal override void UpdateEffectNetworked()
		{
			this.character.refs.customization.PulseStatus(Affliction_BingBongShield.gold, 1f);
		}

		// Token: 0x06001B20 RID: 6944 RVA: 0x000846BF File Offset: 0x000828BF
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.BingBongShield;
		}

		// Token: 0x06001B21 RID: 6945 RVA: 0x000846C3 File Offset: 0x000828C3
		public override void Stack(Affliction incomingAffliction)
		{
			this.timeElapsed = 0f;
			this.character.data.RecalculateInvincibility();
		}

		// Token: 0x06001B22 RID: 6946 RVA: 0x000846E0 File Offset: 0x000828E0
		public override void OnApplied()
		{
			this.character.data.RecalculateInvincibility();
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x000846F2 File Offset: 0x000828F2
		public override void OnRemoved()
		{
			this.character.data.RecalculateInvincibility();
		}

		// Token: 0x06001B24 RID: 6948 RVA: 0x00084704 File Offset: 0x00082904
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001B25 RID: 6949 RVA: 0x00084712 File Offset: 0x00082912
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x0400179F RID: 6047
		public static readonly Color gold = new Color(0.9f, 0.59f, 0.035f);
	}
}
