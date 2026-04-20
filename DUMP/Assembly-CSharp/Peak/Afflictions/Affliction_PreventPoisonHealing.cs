using System;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x02000401 RID: 1025
	public class Affliction_PreventPoisonHealing : Affliction
	{
		// Token: 0x06001B0B RID: 6923 RVA: 0x0008458F File Offset: 0x0008278F
		public Affliction_PreventPoisonHealing()
		{
		}

		// Token: 0x06001B0C RID: 6924 RVA: 0x00084597 File Offset: 0x00082797
		public Affliction_PreventPoisonHealing(float totalTime) : base(totalTime)
		{
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x000845A0 File Offset: 0x000827A0
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.PreventPoisonHealing;
		}

		// Token: 0x06001B0E RID: 6926 RVA: 0x000845A4 File Offset: 0x000827A4
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x000845B2 File Offset: 0x000827B2
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x06001B10 RID: 6928 RVA: 0x000845C0 File Offset: 0x000827C0
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = incomingAffliction.totalTime;
		}
	}
}
