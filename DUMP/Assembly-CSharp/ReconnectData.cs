using System;
using System.Linq;
using Peak.Dev;
using Unity.Collections;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x0200014F RID: 335
public struct ReconnectData : IPrettyPrintable
{
	// Token: 0x06000B25 RID: 2853 RVA: 0x0003C00C File Offset: 0x0003A20C
	public string ToPrettyString()
	{
		return string.Concat(new string[]
		{
			Pretty.Print(this.currentStatuses),
			string.Format("\nPosition: {0}, Dead: {1}, FullyPassedOut: {2}, DeathTimer: {3}", new object[]
			{
				this.position,
				this.dead,
				this.fullyPassedOut,
				this.deathTimer
			}),
			string.Format("\nLastRevived: {0}, LastSegment: {1}", this.lastRevivedSegment, this.mapSegment),
			"\nScout Report: ",
			Pretty.Print(this.scoutReport)
		});
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x0003C0BC File Offset: 0x0003A2BC
	public static ReconnectData CreateFromCharacter(Character character)
	{
		ReconnectData reconnectData = default(ReconnectData);
		reconnectData.isValid = true;
		reconnectData.position = character.VirtualCenter;
		reconnectData.dead = character.data.dead;
		reconnectData.fullyPassedOut = character.data.fullyPassedOut;
		reconnectData.isSkeleton = character.data.isSkeleton;
		reconnectData.deathTimer = character.data.deathTimer;
		reconnectData.currentStatuses = character.refs.afflictions.currentStatuses;
		reconnectData.inventorySyncData = new InventorySyncData(character.player.itemSlots, character.player.backpackSlot, character.player.tempFullSlot);
		reconnectData.mapSegment = Singleton<MapHandler>.Instance.GetCurrentSegment();
		reconnectData.lastRevivedSegment = character.data.lastRevivedSegment;
		reconnectData.scoutReport = new CharacterStats.SyncData(character.refs.stats.timelineInfo);
		if (Time.time - character.timeLastWarped < 1f || Vector3.Magnitude(character.data.avarageVelocity) > 100f || Vector3.Magnitude(character.data.avarageLastFrameVelocity) > 100f)
		{
			reconnectData.isValid = false;
		}
		else if (reconnectData.position.z < -2000f || reconnectData.position.y > 1300f)
		{
			Debug.LogWarning("Bad position data tried to sneak into our reconnect record again!");
			reconnectData.isValid = false;
		}
		return reconnectData;
	}

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x06000B27 RID: 2855 RVA: 0x0003C234 File Offset: 0x0003A434
	public static ReconnectData Invalid
	{
		get
		{
			return new ReconnectData
			{
				isValid = false,
				inventorySyncData = default(InventorySyncData)
			};
		}
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x0003C264 File Offset: 0x0003A464
	public byte[] Serialize()
	{
		BinarySerializer binarySerializer = new BinarySerializer(3000, Allocator.Temp);
		binarySerializer.WriteBool(this.isValid);
		if (this.isValid)
		{
			binarySerializer.WriteFloat3(this.position);
			binarySerializer.WriteBool(this.dead);
			binarySerializer.WriteBool(this.fullyPassedOut);
			binarySerializer.WriteBool(this.isSkeleton);
			binarySerializer.WriteFloat(this.deathTimer);
			new StatusSyncData
			{
				statusList = this.currentStatuses.ToList<float>()
			}.Serialize(binarySerializer);
			this.inventorySyncData.Serialize(binarySerializer);
			binarySerializer.WriteByte((byte)this.mapSegment);
			binarySerializer.WriteByte((byte)this.lastRevivedSegment);
			this.scoutReport.Serialize(binarySerializer);
		}
		byte[] result = binarySerializer.buffer.ToByteArray();
		binarySerializer.Dispose();
		return result;
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0003C33C File Offset: 0x0003A53C
	public static ReconnectData Deserialize(byte[] data)
	{
		ReconnectData reconnectData = default(ReconnectData);
		BinaryDeserializer binaryDeserializer = new BinaryDeserializer(data, Allocator.Temp);
		reconnectData.isValid = binaryDeserializer.ReadBool();
		if (reconnectData.isValid)
		{
			reconnectData.position = binaryDeserializer.ReadFloat3();
			reconnectData.dead = binaryDeserializer.ReadBool();
			reconnectData.fullyPassedOut = binaryDeserializer.ReadBool();
			reconnectData.isSkeleton = binaryDeserializer.ReadBool();
			reconnectData.deathTimer = binaryDeserializer.ReadFloat();
			reconnectData.currentStatuses = IBinarySerializable.Deserialize<StatusSyncData>(binaryDeserializer).statusList.ToArray();
			reconnectData.inventorySyncData = IBinarySerializable.Deserialize<InventorySyncData>(binaryDeserializer);
			reconnectData.mapSegment = (Segment)binaryDeserializer.ReadByte();
			reconnectData.lastRevivedSegment = (Segment)binaryDeserializer.ReadByte();
			reconnectData.scoutReport = IBinarySerializable.Deserialize<CharacterStats.SyncData>(binaryDeserializer);
		}
		binaryDeserializer.Dispose();
		return reconnectData;
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0003C40C File Offset: 0x0003A60C
	public override string ToString()
	{
		string newLine = Environment.NewLine;
		return string.Format("IsValid: {0}{1}Position: {2}{3}Dead: {4}{5}FullyPassedOut: {6}{7}DeathTimer: {8}", new object[]
		{
			this.isValid,
			newLine,
			this.position,
			newLine,
			this.dead,
			newLine,
			this.fullyPassedOut,
			newLine,
			this.deathTimer
		});
	}

	// Token: 0x04000A42 RID: 2626
	public bool isValid;

	// Token: 0x04000A43 RID: 2627
	public Vector3 position;

	// Token: 0x04000A44 RID: 2628
	public bool dead;

	// Token: 0x04000A45 RID: 2629
	public bool fullyPassedOut;

	// Token: 0x04000A46 RID: 2630
	public bool isSkeleton;

	// Token: 0x04000A47 RID: 2631
	public float deathTimer;

	// Token: 0x04000A48 RID: 2632
	public Segment mapSegment;

	// Token: 0x04000A49 RID: 2633
	public Segment lastRevivedSegment;

	// Token: 0x04000A4A RID: 2634
	public float[] currentStatuses;

	// Token: 0x04000A4B RID: 2635
	public InventorySyncData inventorySyncData;

	// Token: 0x04000A4C RID: 2636
	public CharacterStats.SyncData scoutReport;

	// Token: 0x04000A4D RID: 2637
	private const float maxAcceptedVelocity = 100f;
}
