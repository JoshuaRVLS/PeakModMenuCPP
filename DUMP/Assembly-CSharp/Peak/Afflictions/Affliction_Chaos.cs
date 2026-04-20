using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x02000400 RID: 1024
	public class Affliction_Chaos : Affliction
	{
		// Token: 0x06001B04 RID: 6916 RVA: 0x00084345 File Offset: 0x00082545
		public Affliction_Chaos()
		{
		}

		// Token: 0x06001B05 RID: 6917 RVA: 0x0008434D File Offset: 0x0008254D
		public Affliction_Chaos(float statusAmountAverage, float statusAmountStandardDeviation, float averageBonusStamina, float standardDeviationBonusStamina)
		{
			this.statusAmountAverage = statusAmountAverage;
			this.statusAmountStandardDeviation = statusAmountStandardDeviation;
			this.averageBonusStamina = averageBonusStamina;
			this.standardDeviationBonusStamina = standardDeviationBonusStamina;
		}

		// Token: 0x06001B06 RID: 6918 RVA: 0x00084374 File Offset: 0x00082574
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				List<CharacterAfflictions.STATUSTYPE> list = new List<CharacterAfflictions.STATUSTYPE>
				{
					CharacterAfflictions.STATUSTYPE.Cold,
					CharacterAfflictions.STATUSTYPE.Hot,
					CharacterAfflictions.STATUSTYPE.Poison,
					CharacterAfflictions.STATUSTYPE.Drowsy,
					CharacterAfflictions.STATUSTYPE.Injury,
					CharacterAfflictions.STATUSTYPE.Hunger,
					CharacterAfflictions.STATUSTYPE.Spores
				};
				this.character.refs.afflictions.ClearAllStatus(false);
				float num = Mathf.Clamp(Util.GenerateNormalDistribution(this.statusAmountAverage, this.statusAmountStandardDeviation), 0f, 1f);
				Debug.Log(string.Format("total status: {0}", num));
				float num2 = num;
				while (num2 > 0.05f && list.Count != 0)
				{
					float num3;
					if (list.Count == 1)
					{
						num3 = num2;
					}
					else
					{
						num3 = num * Util.GenerateNormalDistribution(0.3f, 0.5f);
					}
					Debug.Log(string.Format("Next status: {0}", num3));
					num3 = Mathf.Min(num3, num2);
					if (num3 >= 0.025f)
					{
						int index = Random.Range(0, list.Count);
						CharacterAfflictions.STATUSTYPE statustype = list[index];
						this.character.refs.afflictions.AddStatus(statustype, num3, false, true, true);
						list.RemoveAt(index);
						if (statustype == CharacterAfflictions.STATUSTYPE.Hot)
						{
							list.Remove(CharacterAfflictions.STATUSTYPE.Cold);
						}
						else if (statustype == CharacterAfflictions.STATUSTYPE.Cold)
						{
							list.Remove(CharacterAfflictions.STATUSTYPE.Hot);
						}
						num2 -= num3;
					}
				}
				float extraStamina = Mathf.Clamp(Util.GenerateNormalDistribution(this.averageBonusStamina, this.standardDeviationBonusStamina), 0f, 1f);
				this.character.SetExtraStamina(extraStamina);
				this.character.refs.afflictions.RemoveAffliction(this, false, true);
			}
		}

		// Token: 0x06001B07 RID: 6919 RVA: 0x00084526 File Offset: 0x00082726
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Chaos;
		}

		// Token: 0x06001B08 RID: 6920 RVA: 0x00084529 File Offset: 0x00082729
		public override void Stack(Affliction incomingAffliction)
		{
		}

		// Token: 0x06001B09 RID: 6921 RVA: 0x0008452B File Offset: 0x0008272B
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.statusAmountAverage);
			serializer.WriteFloat(this.statusAmountStandardDeviation);
			serializer.WriteFloat(this.averageBonusStamina);
			serializer.WriteFloat(this.standardDeviationBonusStamina);
		}

		// Token: 0x06001B0A RID: 6922 RVA: 0x0008455D File Offset: 0x0008275D
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.statusAmountAverage = serializer.ReadFloat();
			this.statusAmountStandardDeviation = serializer.ReadFloat();
			this.averageBonusStamina = serializer.ReadFloat();
			this.standardDeviationBonusStamina = serializer.ReadFloat();
		}

		// Token: 0x0400179A RID: 6042
		public float statusAmountAverage;

		// Token: 0x0400179B RID: 6043
		public float statusAmountStandardDeviation;

		// Token: 0x0400179C RID: 6044
		public float averageBonusStamina;

		// Token: 0x0400179D RID: 6045
		public float standardDeviationBonusStamina;
	}
}
