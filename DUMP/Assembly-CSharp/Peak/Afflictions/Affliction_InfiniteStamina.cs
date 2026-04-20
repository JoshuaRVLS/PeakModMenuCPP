using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003F9 RID: 1017
	public class Affliction_InfiniteStamina : Affliction
	{
		// Token: 0x06001ACD RID: 6861 RVA: 0x00083D3C File Offset: 0x00081F3C
		public Affliction_InfiniteStamina(float totalTime) : base(totalTime)
		{
		}

		// Token: 0x06001ACE RID: 6862 RVA: 0x00083D45 File Offset: 0x00081F45
		public Affliction_InfiniteStamina()
		{
		}

		// Token: 0x06001ACF RID: 6863 RVA: 0x00083D4D File Offset: 0x00081F4D
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.InfiniteStamina;
		}

		// Token: 0x06001AD0 RID: 6864 RVA: 0x00083D50 File Offset: 0x00081F50
		public override void Stack(Affliction incomingAffliction)
		{
			Affliction_InfiniteStamina affliction_InfiniteStamina = incomingAffliction as Affliction_InfiniteStamina;
			if (affliction_InfiniteStamina != null)
			{
				this.totalTime = incomingAffliction.totalTime;
				this.timeElapsed = 0f;
				if (this.drowsyAffliction != null)
				{
					this.drowsyAffliction.totalTime += affliction_InfiniteStamina.drowsyAffliction.totalTime;
				}
			}
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x00083DA3 File Offset: 0x00081FA3
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				GUIManager.instance.StartSugarRush();
			}
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x00083DBC File Offset: 0x00081FBC
		public override void OnRemoved()
		{
			if (this.character.IsLocal)
			{
				GUIManager.instance.EndSugarRush();
				if (this.drowsyAffliction != null)
				{
					this.character.refs.afflictions.AddAffliction(this.drowsyAffliction, false);
				}
			}
		}

		// Token: 0x06001AD3 RID: 6867 RVA: 0x00083DFC File Offset: 0x00081FFC
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.climbDelay);
			bool flag = this.drowsyAffliction != null;
			serializer.WriteBool(flag);
			if (flag)
			{
				this.drowsyAffliction.Serialize(serializer);
			}
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x00083E44 File Offset: 0x00082044
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.climbDelay = serializer.ReadFloat();
			this.bonusTime = this.climbDelay;
			if (serializer.ReadBool())
			{
				this.drowsyAffliction = new Affliction_AdjustDrowsyOverTime();
				this.drowsyAffliction.Deserialize(serializer);
			}
		}

		// Token: 0x06001AD5 RID: 6869 RVA: 0x00083E94 File Offset: 0x00082094
		protected override void UpdateEffect()
		{
			this.character.AddStamina(1f);
			if (this.character.data.isClimbing)
			{
				this.climbDelay = 0f;
				this.bonusTime = 0f;
			}
		}

		// Token: 0x04001792 RID: 6034
		[SerializeReference]
		public Affliction drowsyAffliction;

		// Token: 0x04001793 RID: 6035
		public float climbDelay;
	}
}
