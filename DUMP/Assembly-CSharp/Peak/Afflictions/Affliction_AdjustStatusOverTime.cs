using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003FF RID: 1023
	public class Affliction_AdjustStatusOverTime : Affliction
	{
		// Token: 0x06001B00 RID: 6912 RVA: 0x000842C9 File Offset: 0x000824C9
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.AdjustStatusOverTime;
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x000842D0 File Offset: 0x000824D0
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime += incomingAffliction.totalTime;
			Affliction_AdjustStatusOverTime affliction_AdjustStatusOverTime = incomingAffliction as Affliction_AdjustStatusOverTime;
			if (affliction_AdjustStatusOverTime != null)
			{
				this.statusPerSecond = Mathf.Max(affliction_AdjustStatusOverTime.statusPerSecond, this.statusPerSecond);
			}
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x00084311 File Offset: 0x00082511
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.statusPerSecond);
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001B03 RID: 6915 RVA: 0x0008432B File Offset: 0x0008252B
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.statusPerSecond = serializer.ReadFloat();
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x04001799 RID: 6041
		public float statusPerSecond;
	}
}
