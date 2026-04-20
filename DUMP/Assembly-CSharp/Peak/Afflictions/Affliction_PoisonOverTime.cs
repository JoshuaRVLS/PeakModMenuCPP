using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003F7 RID: 1015
	public class Affliction_PoisonOverTime : Affliction
	{
		// Token: 0x06001ABD RID: 6845 RVA: 0x00083AD0 File Offset: 0x00081CD0
		public override void OnApplied()
		{
			Debug.Log(string.Format("Added poison to character {0} total time: {1} delay: {2} status per second: {3}", new object[]
			{
				this.character.gameObject.name,
				this.totalTime,
				this.delayBeforeEffect,
				this.statusPerSecond
			}));
		}

		// Token: 0x06001ABE RID: 6846 RVA: 0x00083B2F File Offset: 0x00081D2F
		public Affliction_PoisonOverTime(float totalTime, float delay, float statusPerSecond) : base(totalTime)
		{
			this.totalTime = totalTime + delay;
			this.delayBeforeEffect = delay;
			this.statusPerSecond = statusPerSecond;
		}

		// Token: 0x06001ABF RID: 6847 RVA: 0x00083B4F File Offset: 0x00081D4F
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.delayBeforeEffect);
			serializer.WriteFloat(this.statusPerSecond);
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x00083B75 File Offset: 0x00081D75
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.delayBeforeEffect = serializer.ReadFloat();
			this.statusPerSecond = serializer.ReadFloat();
		}

		// Token: 0x06001AC1 RID: 6849 RVA: 0x00083B9B File Offset: 0x00081D9B
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime += incomingAffliction.totalTime;
		}

		// Token: 0x06001AC2 RID: 6850 RVA: 0x00083BB0 File Offset: 0x00081DB0
		public Affliction_PoisonOverTime()
		{
		}

		// Token: 0x06001AC3 RID: 6851 RVA: 0x00083BB8 File Offset: 0x00081DB8
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.PoisonOverTime;
		}

		// Token: 0x06001AC4 RID: 6852 RVA: 0x00083BBB File Offset: 0x00081DBB
		protected override void UpdateEffect()
		{
			if (this.timeElapsed > this.delayBeforeEffect)
			{
				this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, this.statusPerSecond * Time.deltaTime, false, true, true);
			}
		}

		// Token: 0x0400178E RID: 6030
		public float delayBeforeEffect;

		// Token: 0x0400178F RID: 6031
		public float statusPerSecond;
	}
}
