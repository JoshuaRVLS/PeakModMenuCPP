using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003F8 RID: 1016
	public class Affliction_ZombieBite : Affliction
	{
		// Token: 0x06001AC5 RID: 6853 RVA: 0x00083BF4 File Offset: 0x00081DF4
		public override void OnApplied()
		{
			Debug.Log(string.Format("Added spores to character {0} total time: {1} delay: {2} status per second: {3}", new object[]
			{
				this.character.gameObject.name,
				this.totalTime,
				this.delayBeforeEffect,
				this.statusPerSecond
			}));
		}

		// Token: 0x06001AC6 RID: 6854 RVA: 0x00083C53 File Offset: 0x00081E53
		public Affliction_ZombieBite(float totalTime, float delay, float statusPerSecond) : base(totalTime)
		{
			this.totalTime = totalTime + delay;
			this.delayBeforeEffect = delay;
			this.statusPerSecond = statusPerSecond;
		}

		// Token: 0x06001AC7 RID: 6855 RVA: 0x00083C73 File Offset: 0x00081E73
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.delayBeforeEffect);
			serializer.WriteFloat(this.statusPerSecond);
		}

		// Token: 0x06001AC8 RID: 6856 RVA: 0x00083C99 File Offset: 0x00081E99
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.delayBeforeEffect = serializer.ReadFloat();
			this.statusPerSecond = serializer.ReadFloat();
		}

		// Token: 0x06001AC9 RID: 6857 RVA: 0x00083CBF File Offset: 0x00081EBF
		public override void Stack(Affliction incomingAffliction)
		{
		}

		// Token: 0x06001ACA RID: 6858 RVA: 0x00083CC1 File Offset: 0x00081EC1
		public Affliction_ZombieBite()
		{
		}

		// Token: 0x06001ACB RID: 6859 RVA: 0x00083CC9 File Offset: 0x00081EC9
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.ZombieBite;
		}

		// Token: 0x06001ACC RID: 6860 RVA: 0x00083CD0 File Offset: 0x00081ED0
		protected override void UpdateEffect()
		{
			if (this.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Spores) < 0.025f)
			{
				this.totalTime = 0f;
				return;
			}
			if (this.timeElapsed > this.delayBeforeEffect)
			{
				this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Spores, this.statusPerSecond * Time.deltaTime, false, true, true);
			}
		}

		// Token: 0x04001790 RID: 6032
		public float delayBeforeEffect;

		// Token: 0x04001791 RID: 6033
		public float statusPerSecond;
	}
}
