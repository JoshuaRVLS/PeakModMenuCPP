using System;
using Unity.Collections;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003F3 RID: 1011
	[Serializable]
	public abstract class Affliction
	{
		// Token: 0x06001A9A RID: 6810 RVA: 0x0008340C File Offset: 0x0008160C
		public Affliction()
		{
		}

		// Token: 0x06001A9B RID: 6811 RVA: 0x00083414 File Offset: 0x00081614
		public Affliction(float totalTime)
		{
			this.totalTime = totalTime;
		}

		// Token: 0x06001A9C RID: 6812 RVA: 0x00083424 File Offset: 0x00081624
		public static Affliction CreateBlankAffliction(Affliction.AfflictionType afflictionType)
		{
			switch (afflictionType)
			{
			case Affliction.AfflictionType.PoisonOverTime:
				return new Affliction_PoisonOverTime();
			case Affliction.AfflictionType.InfiniteStamina:
				return new Affliction_InfiniteStamina();
			case Affliction.AfflictionType.FasterBoi:
				return new Affliction_FasterBoi();
			case Affliction.AfflictionType.Exhausted:
				return new Affliction_Exhaustion();
			case Affliction.AfflictionType.Glowing:
				return new Affliction_Glowing();
			case Affliction.AfflictionType.ColdOverTime:
				return new Affliction_AdjustColdOverTime();
			case Affliction.AfflictionType.Chaos:
				return new Affliction_Chaos();
			case Affliction.AfflictionType.AdjustStatus:
				return new Affliction_AdjustStatus();
			case Affliction.AfflictionType.ClearAllStatus:
				return new Affliction_ClearAllStatus();
			case Affliction.AfflictionType.PreventPoisonHealing:
				return new Affliction_PreventPoisonHealing();
			case Affliction.AfflictionType.AddBonusStamina:
				return new Affliction_AddBonusStamina();
			case Affliction.AfflictionType.DrowsyOverTime:
				return new Affliction_AdjustDrowsyOverTime();
			case Affliction.AfflictionType.AdjustStatusOverTime:
				return new Affliction_AdjustStatusOverTime();
			case Affliction.AfflictionType.Sunscreen:
				return new Affliction_Sunscreen();
			case Affliction.AfflictionType.BingBongShield:
				return new Affliction_BingBongShield();
			case Affliction.AfflictionType.ZombieBite:
				return new Affliction_ZombieBite();
			case Affliction.AfflictionType.Invincibility:
				return new Affliction_Invincibility();
			case Affliction.AfflictionType.LowGravity:
				return new Affliction_LowGravity();
			case Affliction.AfflictionType.Blind:
				return new Affliction_Blind();
			case Affliction.AfflictionType.Numb:
				return new Affliction_Numb();
			case Affliction.AfflictionType.ClimbingChalk:
				return new Affliction_ClimbingChalk();
			case Affliction.AfflictionType.NoHunger:
				return new Affliction_NoHunger();
			default:
				return null;
			}
		}

		// Token: 0x06001A9D RID: 6813
		public abstract Affliction.AfflictionType GetAfflictionType();

		// Token: 0x06001A9E RID: 6814 RVA: 0x00083519 File Offset: 0x00081719
		public virtual void OnApplied()
		{
		}

		// Token: 0x06001A9F RID: 6815 RVA: 0x0008351B File Offset: 0x0008171B
		public virtual void OnRemoved()
		{
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06001AA0 RID: 6816 RVA: 0x0008351D File Offset: 0x0008171D
		public virtual bool worksOnBot
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001AA1 RID: 6817
		public abstract void Stack(Affliction incomingAffliction);

		// Token: 0x06001AA2 RID: 6818 RVA: 0x00083520 File Offset: 0x00081720
		public virtual bool Tick()
		{
			if (this.bonusTime > 0f)
			{
				this.bonusTime -= Time.deltaTime;
			}
			else
			{
				this.timeElapsed += Time.deltaTime;
			}
			if (this.timeElapsed >= this.totalTime)
			{
				return true;
			}
			this.UpdateEffect();
			return false;
		}

		// Token: 0x06001AA3 RID: 6819 RVA: 0x00083577 File Offset: 0x00081777
		protected virtual void UpdateEffect()
		{
		}

		// Token: 0x06001AA4 RID: 6820 RVA: 0x00083579 File Offset: 0x00081779
		internal virtual void UpdateEffectNetworked()
		{
		}

		// Token: 0x06001AA5 RID: 6821
		public abstract void Serialize(BinarySerializer serializer);

		// Token: 0x06001AA6 RID: 6822
		public abstract void Deserialize(BinaryDeserializer serializer);

		// Token: 0x06001AA7 RID: 6823 RVA: 0x0008357C File Offset: 0x0008177C
		public Affliction Copy()
		{
			BinarySerializer binarySerializer = new BinarySerializer(100, Allocator.Temp);
			Affliction affliction = Affliction.CreateBlankAffliction(this.GetAfflictionType());
			this.Serialize(binarySerializer);
			BinaryDeserializer binaryDeserializer = new BinaryDeserializer(binarySerializer);
			affliction.Deserialize(binaryDeserializer);
			binarySerializer.Dispose();
			binaryDeserializer.Dispose();
			return affliction;
		}

		// Token: 0x04001782 RID: 6018
		public float timeElapsed;

		// Token: 0x04001783 RID: 6019
		public float totalTime;

		// Token: 0x04001784 RID: 6020
		protected float bonusTime;

		// Token: 0x04001785 RID: 6021
		[HideInInspector]
		public Character character;

		// Token: 0x02000572 RID: 1394
		public enum AfflictionType
		{
			// Token: 0x04001D6D RID: 7533
			PoisonOverTime,
			// Token: 0x04001D6E RID: 7534
			InfiniteStamina,
			// Token: 0x04001D6F RID: 7535
			FasterBoi,
			// Token: 0x04001D70 RID: 7536
			Exhausted,
			// Token: 0x04001D71 RID: 7537
			Glowing,
			// Token: 0x04001D72 RID: 7538
			ColdOverTime,
			// Token: 0x04001D73 RID: 7539
			Chaos,
			// Token: 0x04001D74 RID: 7540
			AdjustStatus,
			// Token: 0x04001D75 RID: 7541
			ClearAllStatus,
			// Token: 0x04001D76 RID: 7542
			PreventPoisonHealing,
			// Token: 0x04001D77 RID: 7543
			AddBonusStamina,
			// Token: 0x04001D78 RID: 7544
			DrowsyOverTime,
			// Token: 0x04001D79 RID: 7545
			AdjustStatusOverTime,
			// Token: 0x04001D7A RID: 7546
			Sunscreen,
			// Token: 0x04001D7B RID: 7547
			BingBongShield,
			// Token: 0x04001D7C RID: 7548
			ZombieBite,
			// Token: 0x04001D7D RID: 7549
			Invincibility,
			// Token: 0x04001D7E RID: 7550
			LowGravity,
			// Token: 0x04001D7F RID: 7551
			Blind,
			// Token: 0x04001D80 RID: 7552
			Numb,
			// Token: 0x04001D81 RID: 7553
			ClimbingChalk,
			// Token: 0x04001D82 RID: 7554
			NoHunger
		}
	}
}
