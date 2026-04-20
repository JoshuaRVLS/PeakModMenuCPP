using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003F6 RID: 1014
	public class Affliction_FasterBoi : Affliction
	{
		// Token: 0x06001AB5 RID: 6837 RVA: 0x000837C0 File Offset: 0x000819C0
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.FasterBoi;
		}

		// Token: 0x06001AB6 RID: 6838 RVA: 0x000837C3 File Offset: 0x000819C3
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = Mathf.Max(this.totalTime, incomingAffliction.totalTime);
		}

		// Token: 0x06001AB7 RID: 6839 RVA: 0x000837DC File Offset: 0x000819DC
		protected override void UpdateEffect()
		{
			if (this.character.data.isClimbing)
			{
				this.climbDelay = 0f;
				this.bonusTime = 0f;
			}
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x00083808 File Offset: 0x00081A08
		public override void OnApplied()
		{
			base.OnApplied();
			this.character.refs.movement.movementModifier += this.moveSpeedMod;
			this.character.refs.climbing.climbSpeedMod += this.climbSpeedMod;
			this.character.refs.ropeHandling.climbSpeedMod += this.climbSpeedMod;
			this.character.refs.vineClimbing.climbSpeedMod += this.climbSpeedMod;
			if (this.character.IsLocal)
			{
				GUIManager.instance.StartEnergyDrink();
			}
			this.cachedDrowsy = this.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy);
			this.character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 2f, false, false);
			this.bonusTime = this.climbDelay;
		}

		// Token: 0x06001AB9 RID: 6841 RVA: 0x00083900 File Offset: 0x00081B00
		public override void OnRemoved()
		{
			base.OnRemoved();
			this.character.refs.movement.movementModifier -= this.moveSpeedMod;
			this.character.refs.climbing.climbSpeedMod -= this.climbSpeedMod;
			this.character.refs.ropeHandling.climbSpeedMod -= this.climbSpeedMod;
			this.character.refs.vineClimbing.climbSpeedMod -= this.climbSpeedMod;
			this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, this.cachedDrowsy + this.drowsyOnEnd, false, true, true);
			if (this.character.IsLocal)
			{
				GUIManager.instance.EndEnergyDrink();
			}
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x000839DC File Offset: 0x00081BDC
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.moveSpeedMod);
			serializer.WriteFloat(this.climbSpeedMod);
			serializer.WriteFloat(this.drowsyOnEnd);
			serializer.WriteFloat(this.cachedDrowsy);
			serializer.WriteFloat(this.climbDelay);
		}

		// Token: 0x06001ABB RID: 6843 RVA: 0x00083A34 File Offset: 0x00081C34
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.moveSpeedMod = serializer.ReadFloat();
			this.climbSpeedMod = serializer.ReadFloat();
			this.drowsyOnEnd = serializer.ReadFloat();
			this.cachedDrowsy = serializer.ReadFloat();
			this.climbDelay = serializer.ReadFloat();
			this.bonusTime = this.climbDelay;
			Debug.Log("Bonus time set to " + this.bonusTime.ToString());
		}

		// Token: 0x04001789 RID: 6025
		public float moveSpeedMod = 1f;

		// Token: 0x0400178A RID: 6026
		public float climbSpeedMod = 1f;

		// Token: 0x0400178B RID: 6027
		public float drowsyOnEnd;

		// Token: 0x0400178C RID: 6028
		private float cachedDrowsy;

		// Token: 0x0400178D RID: 6029
		public float climbDelay;
	}
}
