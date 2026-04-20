using System;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003FB RID: 1019
	public class Affliction_ClearAllStatus : Affliction
	{
		// Token: 0x06001ADE RID: 6878 RVA: 0x00083FD3 File Offset: 0x000821D3
		public Affliction_ClearAllStatus()
		{
		}

		// Token: 0x06001ADF RID: 6879 RVA: 0x00083FDB File Offset: 0x000821DB
		public Affliction_ClearAllStatus(bool excludeCurse, float totalTime) : base(totalTime)
		{
			this.excludeCurse = excludeCurse;
		}

		// Token: 0x06001AE0 RID: 6880 RVA: 0x00083FEB File Offset: 0x000821EB
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.ClearAllStatus;
		}

		// Token: 0x06001AE1 RID: 6881 RVA: 0x00083FEE File Offset: 0x000821EE
		public override void Stack(Affliction incomingAffliction)
		{
			this.OnApplied();
		}

		// Token: 0x06001AE2 RID: 6882 RVA: 0x00083FF6 File Offset: 0x000821F6
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				this.character.refs.afflictions.ClearAllStatus(this.excludeCurse);
			}
		}

		// Token: 0x06001AE3 RID: 6883 RVA: 0x00084020 File Offset: 0x00082220
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteBool(this.excludeCurse);
		}

		// Token: 0x06001AE4 RID: 6884 RVA: 0x0008402E File Offset: 0x0008222E
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.excludeCurse = serializer.ReadBool();
		}

		// Token: 0x04001796 RID: 6038
		public bool excludeCurse;
	}
}
