using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003FE RID: 1022
	public class Affliction_AdjustDrowsyOverTime : Affliction
	{
		// Token: 0x06001AF8 RID: 6904 RVA: 0x00084201 File Offset: 0x00082401
		public Affliction_AdjustDrowsyOverTime()
		{
		}

		// Token: 0x06001AF9 RID: 6905 RVA: 0x00084209 File Offset: 0x00082409
		public Affliction_AdjustDrowsyOverTime(float statusPerSecond, float totalTime) : base(totalTime)
		{
			this.statusPerSecond = statusPerSecond;
			this.totalTime = totalTime;
		}

		// Token: 0x06001AFA RID: 6906 RVA: 0x00084220 File Offset: 0x00082420
		protected override void UpdateEffect()
		{
			this.character.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Drowsy, this.statusPerSecond * Time.deltaTime, false);
		}

		// Token: 0x06001AFB RID: 6907 RVA: 0x00084245 File Offset: 0x00082445
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.statusPerSecond);
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001AFC RID: 6908 RVA: 0x0008425F File Offset: 0x0008245F
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.statusPerSecond = serializer.ReadFloat();
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x06001AFD RID: 6909 RVA: 0x00084279 File Offset: 0x00082479
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.DrowsyOverTime;
		}

		// Token: 0x06001AFE RID: 6910 RVA: 0x00084280 File Offset: 0x00082480
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime += incomingAffliction.totalTime;
			Affliction_AdjustDrowsyOverTime affliction_AdjustDrowsyOverTime = incomingAffliction as Affliction_AdjustDrowsyOverTime;
			if (affliction_AdjustDrowsyOverTime != null)
			{
				this.statusPerSecond = Mathf.Max(affliction_AdjustDrowsyOverTime.statusPerSecond, this.statusPerSecond);
			}
		}

		// Token: 0x04001798 RID: 6040
		public float statusPerSecond;
	}
}
