using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003FC RID: 1020
	public class Affliction_AdjustColdOverTime : Affliction
	{
		// Token: 0x06001AE5 RID: 6885 RVA: 0x0008403C File Offset: 0x0008223C
		public Affliction_AdjustColdOverTime()
		{
		}

		// Token: 0x06001AE6 RID: 6886 RVA: 0x00084044 File Offset: 0x00082244
		public Affliction_AdjustColdOverTime(float statusPerSecond, float totalTime) : base(totalTime)
		{
			this.statusPerSecond = statusPerSecond;
			this.totalTime = totalTime;
		}

		// Token: 0x06001AE7 RID: 6887 RVA: 0x0008405B File Offset: 0x0008225B
		protected override void UpdateEffect()
		{
			this.character.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Cold, this.statusPerSecond * Time.deltaTime, false);
		}

		// Token: 0x06001AE8 RID: 6888 RVA: 0x00084080 File Offset: 0x00082280
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.statusPerSecond);
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001AE9 RID: 6889 RVA: 0x0008409A File Offset: 0x0008229A
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.statusPerSecond = serializer.ReadFloat();
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x06001AEA RID: 6890 RVA: 0x000840B4 File Offset: 0x000822B4
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.ColdOverTime;
		}

		// Token: 0x06001AEB RID: 6891 RVA: 0x000840B7 File Offset: 0x000822B7
		public override void OnApplied()
		{
			if (this.character.IsLocal && this.statusPerSecond < 0f)
			{
				GUIManager.instance.StartHeat();
			}
		}

		// Token: 0x06001AEC RID: 6892 RVA: 0x000840DD File Offset: 0x000822DD
		public override void OnRemoved()
		{
			if (this.character.IsLocal && this.statusPerSecond < 0f)
			{
				GUIManager.instance.EndHeat();
			}
		}

		// Token: 0x06001AED RID: 6893 RVA: 0x00084104 File Offset: 0x00082304
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime += incomingAffliction.totalTime;
			Affliction_AdjustColdOverTime affliction_AdjustColdOverTime = incomingAffliction as Affliction_AdjustColdOverTime;
			if (affliction_AdjustColdOverTime != null)
			{
				this.statusPerSecond = Mathf.Max(affliction_AdjustColdOverTime.statusPerSecond, this.statusPerSecond);
			}
		}

		// Token: 0x04001797 RID: 6039
		public float statusPerSecond;
	}
}
