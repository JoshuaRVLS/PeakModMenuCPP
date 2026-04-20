using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x02000408 RID: 1032
	public class Affliction_LowGravity : Affliction
	{
		// Token: 0x06001B3E RID: 6974 RVA: 0x00084959 File Offset: 0x00082B59
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.LowGravity;
		}

		// Token: 0x06001B3F RID: 6975 RVA: 0x0008495D File Offset: 0x00082B5D
		public Affliction_LowGravity()
		{
		}

		// Token: 0x06001B40 RID: 6976 RVA: 0x0008496C File Offset: 0x00082B6C
		public Affliction_LowGravity(int lowGravAmount, float totalTime) : base(totalTime)
		{
			this.lowGravAmount = lowGravAmount;
		}

		// Token: 0x06001B41 RID: 6977 RVA: 0x00084984 File Offset: 0x00082B84
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = incomingAffliction.totalTime;
			Affliction_LowGravity affliction_LowGravity = incomingAffliction as Affliction_LowGravity;
			if (affliction_LowGravity != null)
			{
				this.lowGravAmount = Mathf.Max(affliction_LowGravity.lowGravAmount, this.lowGravAmount);
			}
			this.timeElapsed = 0f;
			this.character.data.RecalculateLowGrav();
		}

		// Token: 0x06001B42 RID: 6978 RVA: 0x000849DC File Offset: 0x00082BDC
		public override bool Tick()
		{
			if (this.timeElapsed + 2f > this.totalTime)
			{
				if (!this.warning)
				{
					this.character.refs.afflictions.WarnStopWhirlwind();
					this.warning = true;
				}
			}
			else
			{
				this.warning = false;
			}
			return base.Tick();
		}

		// Token: 0x06001B43 RID: 6979 RVA: 0x00084A30 File Offset: 0x00082C30
		public override void OnApplied()
		{
			this.character.data.RecalculateLowGrav();
			this.character.refs.afflictions.StartWhirlwind();
		}

		// Token: 0x06001B44 RID: 6980 RVA: 0x00084A57 File Offset: 0x00082C57
		public override void OnRemoved()
		{
			this.character.data.RecalculateLowGrav();
			this.character.refs.afflictions.StopWhirlwind();
			this.character.data.sinceGrounded = 0f;
		}

		// Token: 0x06001B45 RID: 6981 RVA: 0x00084A93 File Offset: 0x00082C93
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteInt(this.lowGravAmount);
		}

		// Token: 0x06001B46 RID: 6982 RVA: 0x00084AAD File Offset: 0x00082CAD
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.lowGravAmount = serializer.ReadInt();
		}

		// Token: 0x040017A2 RID: 6050
		public int lowGravAmount = 1;

		// Token: 0x040017A3 RID: 6051
		private bool warning;
	}
}
