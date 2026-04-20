using System;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace Peak.Afflictions
{
	// Token: 0x020003F5 RID: 1013
	public class Affliction_Glowing : Affliction
	{
		// Token: 0x06001AAE RID: 6830 RVA: 0x00083663 File Offset: 0x00081863
		public override Affliction.AfflictionType GetAfflictionType()
		{
			return Affliction.AfflictionType.Glowing;
		}

		// Token: 0x06001AAF RID: 6831 RVA: 0x00083668 File Offset: 0x00081868
		public override void OnApplied()
		{
			base.OnApplied();
			Material material = this.character.refs.mainRenderer.materials[0];
			float @float = material.GetFloat("_Glow");
			Debug.Log(string.Format("Appling Glow to character {0}, amount {1}", this.character.gameObject.name, @float));
			material.SetFloat("_Glow", @float + 1f);
			this.pointLightInstance = Object.Instantiate<GameObject>(this.pointLightPref, this.character.GetBodypart(BodypartType.Head).transform);
			this.pointLightInstance.transform.localPosition = Vector3.zero;
		}

		// Token: 0x06001AB0 RID: 6832 RVA: 0x0008370C File Offset: 0x0008190C
		public override void OnRemoved()
		{
			base.OnRemoved();
			Material material = this.character.refs.mainRenderer.materials[0];
			float @float = material.GetFloat("_Glow");
			Debug.Log(string.Format("Removing Glow from character {0}, amount {1}", this.character.gameObject.name, @float));
			material.SetFloat("_Glow", @float - 1f);
			Object.DestroyImmediate(this.pointLightInstance);
		}

		// Token: 0x06001AB1 RID: 6833 RVA: 0x00083783 File Offset: 0x00081983
		public override void Stack(Affliction incomingAffliction)
		{
			this.totalTime = Mathf.Max(this.totalTime, incomingAffliction.totalTime);
		}

		// Token: 0x06001AB2 RID: 6834 RVA: 0x0008379C File Offset: 0x0008199C
		public override void Serialize(BinarySerializer serializer)
		{
			serializer.WriteFloat(this.totalTime);
		}

		// Token: 0x06001AB3 RID: 6835 RVA: 0x000837AA File Offset: 0x000819AA
		public override void Deserialize(BinaryDeserializer serializer)
		{
			this.totalTime = serializer.ReadFloat();
		}

		// Token: 0x04001787 RID: 6023
		public GameObject pointLightPref;

		// Token: 0x04001788 RID: 6024
		private GameObject pointLightInstance;
	}
}
