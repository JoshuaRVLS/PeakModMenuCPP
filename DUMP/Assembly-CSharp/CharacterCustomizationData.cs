using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000079 RID: 121
[Serializable]
public class CharacterCustomizationData : IBinarySerializable
{
	// Token: 0x06000570 RID: 1392 RVA: 0x0002014C File Offset: 0x0001E34C
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteInt(this.currentSkin);
		serializer.WriteInt(this.currentAccessory);
		serializer.WriteInt(this.currentEyes);
		serializer.WriteInt(this.currentMouth);
		serializer.WriteInt(this.currentOutfit);
		serializer.WriteInt(this.currentHat);
		serializer.WriteInt(this.currentSash);
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x000201B0 File Offset: 0x0001E3B0
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.currentSkin = deserializer.ReadInt();
		this.currentAccessory = deserializer.ReadInt();
		this.currentEyes = deserializer.ReadInt();
		this.currentMouth = deserializer.ReadInt();
		this.currentOutfit = deserializer.ReadInt();
		this.currentHat = deserializer.ReadInt();
		this.currentSash = deserializer.ReadInt();
		this.CorrectValues();
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x00020218 File Offset: 0x0001E418
	public void CorrectValues()
	{
		if (Singleton<Customization>.Instance)
		{
			if (this.currentSkin >= Singleton<Customization>.Instance.skins.Length)
			{
				this.currentSkin = 0;
			}
			if (this.currentEyes >= Singleton<Customization>.Instance.eyes.Length)
			{
				this.currentEyes = 0;
			}
			if (this.currentMouth >= Singleton<Customization>.Instance.mouths.Length)
			{
				this.currentMouth = 0;
			}
			if (this.currentAccessory >= Singleton<Customization>.Instance.accessories.Length)
			{
				this.currentAccessory = 0;
			}
			if (this.currentOutfit >= Singleton<Customization>.Instance.fits.Length)
			{
				this.currentOutfit = 0;
			}
			if (this.currentHat >= Singleton<Customization>.Instance.hats.Length)
			{
				this.currentHat = 0;
			}
			if (this.currentSash >= Singleton<Customization>.Instance.sashes.Length)
			{
				this.currentSash = Singleton<Customization>.Instance.sashes.Length - 1;
			}
		}
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x00020300 File Offset: 0x0001E500
	public override string ToString()
	{
		return string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[]
		{
			this.currentSkin,
			this.currentAccessory,
			this.currentEyes,
			this.currentMouth,
			this.currentOutfit,
			this.currentHat,
			this.currentSash
		});
	}

	// Token: 0x040005A4 RID: 1444
	[SerializeField]
	public int currentSkin;

	// Token: 0x040005A5 RID: 1445
	[SerializeField]
	public int currentAccessory;

	// Token: 0x040005A6 RID: 1446
	[SerializeField]
	public int currentEyes;

	// Token: 0x040005A7 RID: 1447
	[SerializeField]
	public int currentMouth;

	// Token: 0x040005A8 RID: 1448
	[FormerlySerializedAs("currentFit")]
	[SerializeField]
	public int currentOutfit;

	// Token: 0x040005A9 RID: 1449
	[SerializeField]
	public int currentHat;

	// Token: 0x040005AA RID: 1450
	[SerializeField]
	public int currentSash;
}
