using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003FA RID: 1018
	public class Affliction_AdjustStatus : Affliction
	{
		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06001AD6 RID: 6870 RVA: 0x00083ECE File Offset: 0x000820CE
		public override bool worksOnBot
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x00083ED1 File Offset: 0x000820D1
		public Affliction_AdjustStatus()
		{
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x00083ED9 File Offset: 0x000820D9
		public Affliction_AdjustStatus(CharacterAfflictions.STATUSTYPE statusType, float statusAmount, float totalTime) : base(totalTime)
		{
			this.statusType = statusType;
			this.statusAmount = statusAmount;
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x00083EF0 File Offset: 0x000820F0
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.AdjustStatus;
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x00083EF3 File Offset: 0x000820F3
		public override void Stack(Affliction incomingAffliction)
		{
			this.OnApplied();
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x00083EFB File Offset: 0x000820FB
		public override void OnApplied()
		{
			if (this.character.photonView.IsMine)
			{
				this.character.refs.afflictions.AdjustStatus(this.statusType, this.statusAmount, false);
			}
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x00083F34 File Offset: 0x00082134
		public override void Serialize(BinarySerializer serializer)
		{
			Debug.Log("Serializing int");
			serializer.WriteInt((int)this.statusType);
			Debug.Log("Serializing float");
			serializer.WriteFloat(this.statusAmount);
			Debug.Log("Serializing float");
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001ADD RID: 6877 RVA: 0x00083F84 File Offset: 0x00082184
		public override void Deserialize(BinaryDeserializer serializer)
		{
			Debug.Log("Deserializing int");
			this.statusType = (CharacterAfflictions.STATUSTYPE)serializer.ReadInt();
			Debug.Log("Deserializing float");
			this.statusAmount = serializer.ReadFloat();
			Debug.Log("Deserializing float");
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x04001794 RID: 6036
		public CharacterAfflictions.STATUSTYPE statusType;

		// Token: 0x04001795 RID: 6037
		public float statusAmount;
	}
}
