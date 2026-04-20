using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x02000405 RID: 1029
	public class Affliction_Invincibility : Affliction
	{
		// Token: 0x06001B28 RID: 6952 RVA: 0x00084743 File Offset: 0x00082943
		internal override void UpdateEffectNetworked()
		{
			this.character.refs.customization.PulseStatus(this.gold, Mathf.Clamp01(this.totalTime - this.timeElapsed));
		}

		// Token: 0x06001B29 RID: 6953 RVA: 0x00084772 File Offset: 0x00082972
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Invincibility;
		}

		// Token: 0x06001B2A RID: 6954 RVA: 0x00084778 File Offset: 0x00082978
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = incomingAffliction.totalTime;
			this.timeElapsed = 0f;
			this.character.data.RecalculateInvincibility();
			this.isFromMilk = (this.isFromMilk || (incomingAffliction as Affliction_Invincibility).isFromMilk);
		}

		// Token: 0x06001B2B RID: 6955 RVA: 0x000847C8 File Offset: 0x000829C8
		public override void OnApplied()
		{
			this.character.data.RecalculateInvincibility();
		}

		// Token: 0x06001B2C RID: 6956 RVA: 0x000847DA File Offset: 0x000829DA
		public override void OnRemoved()
		{
			this.character.data.RecalculateInvincibility();
		}

		// Token: 0x06001B2D RID: 6957 RVA: 0x000847EC File Offset: 0x000829EC
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteBool(this.isFromMilk);
		}

		// Token: 0x06001B2E RID: 6958 RVA: 0x00084806 File Offset: 0x00082A06
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.isFromMilk = serializer.ReadBool();
		}

		// Token: 0x040017A0 RID: 6048
		private Color gold = new Color(0.9f, 0.59f, 0.035f);

		// Token: 0x040017A1 RID: 6049
		public bool isFromMilk;
	}
}
