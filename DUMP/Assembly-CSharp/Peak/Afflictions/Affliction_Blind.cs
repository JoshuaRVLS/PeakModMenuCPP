using System;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x02000406 RID: 1030
	public class Affliction_Blind : Affliction
	{
		// Token: 0x06001B30 RID: 6960 RVA: 0x00084842 File Offset: 0x00082A42
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Blind;
		}

		// Token: 0x06001B31 RID: 6961 RVA: 0x00084846 File Offset: 0x00082A46
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = incomingAffliction.totalTime;
			this.timeElapsed = 0f;
		}

		// Token: 0x06001B32 RID: 6962 RVA: 0x0008485F File Offset: 0x00082A5F
		public override void OnApplied()
		{
			this.character.AddIllegalStatus("BLIND", 60f);
			this.character.refs.customization.refs.blindRenderer.gameObject.SetActive(true);
		}

		// Token: 0x06001B33 RID: 6963 RVA: 0x0008489B File Offset: 0x00082A9B
		public override void OnRemoved()
		{
			this.character.refs.customization.refs.blindRenderer.gameObject.SetActive(false);
		}

		// Token: 0x06001B34 RID: 6964 RVA: 0x000848C2 File Offset: 0x00082AC2
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001B35 RID: 6965 RVA: 0x000848D0 File Offset: 0x00082AD0
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}
	}
}
