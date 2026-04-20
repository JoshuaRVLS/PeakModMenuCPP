using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003FD RID: 1021
	public class Affliction_Sunscreen : Affliction
	{
		// Token: 0x06001AEE RID: 6894 RVA: 0x00084145 File Offset: 0x00082345
		public Affliction_Sunscreen()
		{
		}

		// Token: 0x06001AEF RID: 6895 RVA: 0x0008414D File Offset: 0x0008234D
		public Affliction_Sunscreen(float totalTime) : base(totalTime)
		{
			this.totalTime = totalTime;
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x0008415D File Offset: 0x0008235D
		protected override void UpdateEffect()
		{
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x0008415F File Offset: 0x0008235F
		internal override void UpdateEffectNetworked()
		{
			this.character.refs.customization.PulseStatus(Color.white, 1f);
		}

		// Token: 0x06001AF2 RID: 6898 RVA: 0x00084180 File Offset: 0x00082380
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001AF3 RID: 6899 RVA: 0x0008418E File Offset: 0x0008238E
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x06001AF4 RID: 6900 RVA: 0x0008419C File Offset: 0x0008239C
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Sunscreen;
		}

		// Token: 0x06001AF5 RID: 6901 RVA: 0x000841A0 File Offset: 0x000823A0
		public override void OnApplied()
		{
			if (this.character.IsLocal)
			{
				this.character.data.wearingSunscreen = true;
				GUIManager.instance.StartSunscreen();
			}
		}

		// Token: 0x06001AF6 RID: 6902 RVA: 0x000841CA File Offset: 0x000823CA
		public override void OnRemoved()
		{
			if (this.character.IsLocal)
			{
				this.character.data.wearingSunscreen = false;
				GUIManager.instance.EndSunscreen();
			}
		}

		// Token: 0x06001AF7 RID: 6903 RVA: 0x000841F4 File Offset: 0x000823F4
		public override void Stack(Affliction incomingAffliction)
		{
			this.timeElapsed = 0f;
		}
	}
}
