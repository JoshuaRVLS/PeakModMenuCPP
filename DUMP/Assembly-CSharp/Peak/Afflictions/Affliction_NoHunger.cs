using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x02000403 RID: 1027
	public class Affliction_NoHunger : Affliction
	{
		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06001B18 RID: 6936 RVA: 0x00084660 File Offset: 0x00082860
		private Color pulseColor
		{
			get
			{
				return Affliction_BingBongShield.gold;
			}
		}

		// Token: 0x06001B19 RID: 6937 RVA: 0x00084667 File Offset: 0x00082867
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.NoHunger;
		}

		// Token: 0x06001B1A RID: 6938 RVA: 0x0008466B File Offset: 0x0008286B
		internal override void UpdateEffectNetworked()
		{
		}

		// Token: 0x06001B1B RID: 6939 RVA: 0x0008466D File Offset: 0x0008286D
		public override void Stack(Affliction incomingAffliction)
		{
			this.timeElapsed = 0f;
		}

		// Token: 0x06001B1C RID: 6940 RVA: 0x0008467A File Offset: 0x0008287A
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001B1D RID: 6941 RVA: 0x00084688 File Offset: 0x00082888
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}
	}
}
