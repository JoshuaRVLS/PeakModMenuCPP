using System;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x02000407 RID: 1031
	public class Affliction_Numb : Affliction
	{
		// Token: 0x06001B37 RID: 6967 RVA: 0x000848E6 File Offset: 0x00082AE6
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Numb;
		}

		// Token: 0x06001B38 RID: 6968 RVA: 0x000848EA File Offset: 0x00082AEA
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = incomingAffliction.totalTime;
			this.timeElapsed = 0f;
		}

		// Token: 0x06001B39 RID: 6969 RVA: 0x00084903 File Offset: 0x00082B03
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				GUIManager.instance.StartNumb();
			}
		}

		// Token: 0x06001B3A RID: 6970 RVA: 0x0008491C File Offset: 0x00082B1C
		public override void OnRemoved()
		{
			if (this.character.IsLocal)
			{
				GUIManager.instance.StopNumb();
			}
		}

		// Token: 0x06001B3B RID: 6971 RVA: 0x00084935 File Offset: 0x00082B35
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001B3C RID: 6972 RVA: 0x00084943 File Offset: 0x00082B43
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}
	}
}
