using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003F4 RID: 1012
	public class Affliction_Exhaustion : Affliction
	{
		// Token: 0x06001AA8 RID: 6824 RVA: 0x000835BE File Offset: 0x000817BE
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Exhausted;
		}

		// Token: 0x06001AA9 RID: 6825 RVA: 0x000835C4 File Offset: 0x000817C4
		protected override void UpdateEffect()
		{
			base.UpdateEffect();
			float num = this.drainAmount / this.totalTime * Time.deltaTime;
			this.character.UseStamina(num, true);
			Debug.Log(string.Format("Exhausterd: {0}", num));
		}

		// Token: 0x06001AAA RID: 6826 RVA: 0x0008360E File Offset: 0x0008180E
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = Mathf.Max(this.timeElapsed, incomingAffliction.totalTime);
		}

		// Token: 0x06001AAB RID: 6827 RVA: 0x00083627 File Offset: 0x00081827
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
			serializer.WriteFloat(this.drainAmount);
		}

		// Token: 0x06001AAC RID: 6828 RVA: 0x00083641 File Offset: 0x00081841
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
			this.drainAmount = serializer.ReadFloat();
		}

		// Token: 0x04001786 RID: 6022
		public float drainAmount;
	}
}
