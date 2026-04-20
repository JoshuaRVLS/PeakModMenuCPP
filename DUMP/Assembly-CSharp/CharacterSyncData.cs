using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000148 RID: 328
public struct CharacterSyncData : IBinarySerializable
{
	// Token: 0x06000AE5 RID: 2789 RVA: 0x0003A450 File Offset: 0x00038650
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteFloat3(this.hipLocation);
		serializer.WriteHalf2(new half2((half)this.lookValues.x, (half)this.lookValues.y));
		CharacterSyncData.Flags flags = CharacterSyncData.Flags.NONE;
		if (this.sprintIsPressed)
		{
			flags |= CharacterSyncData.Flags.SPRINT;
		}
		if (this.movementInput.x > 0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_RIGHT;
		}
		if (this.movementInput.x < -0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_LEFT;
		}
		if (this.movementInput.y > 0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_FORWARD;
		}
		if (this.movementInput.y < -0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_BACKWARD;
		}
		if (this.ropeClimbing)
		{
			flags |= CharacterSyncData.Flags.ROPE_CLIMBING;
		}
		if (this.isClimbing)
		{
			flags |= CharacterSyncData.Flags.CLIMBING;
		}
		if (this.isGrounded)
		{
			flags |= CharacterSyncData.Flags.IS_GROUNDED;
		}
		serializer.WriteByte((byte)flags);
		serializer.WriteHalf((half)this.sinceGrounded);
		if (this.ropeClimbing)
		{
			serializer.WriteHalf((half)this.ropePercent);
		}
		serializer.WriteHalf3((half3)this.averageVelocity);
		if (this.isClimbing)
		{
			serializer.WriteHalf3((half3)this.climbPos);
		}
		serializer.WriteHalf((half)this.stammina);
		serializer.WriteHalf((half)this.extraStammina);
		serializer.WriteHalf((half)this.spectateZoom);
		serializer.WriteHalf((half)this.isChargingThrow);
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0003A5C4 File Offset: 0x000387C4
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.hipLocation = deserializer.ReadFloat3();
		this.lookValues = new Vector2(deserializer.ReadHalf(), deserializer.ReadHalf());
		CharacterSyncData.Flags lhs = (CharacterSyncData.Flags)deserializer.ReadByte();
		Vector2 zero = Vector2.zero;
		this.sprintIsPressed = lhs.HasFlagUnsafe(CharacterSyncData.Flags.SPRINT);
		if (lhs.HasFlagUnsafe(CharacterSyncData.Flags.WALK_RIGHT))
		{
			zero.x += 1f;
		}
		if (lhs.HasFlagUnsafe(CharacterSyncData.Flags.WALK_LEFT))
		{
			zero.x -= 1f;
		}
		if (lhs.HasFlagUnsafe(CharacterSyncData.Flags.WALK_FORWARD))
		{
			zero.y += 1f;
		}
		if (lhs.HasFlagUnsafe(CharacterSyncData.Flags.WALK_BACKWARD))
		{
			zero.y -= 1f;
		}
		this.movementInput = zero;
		this.sinceGrounded = deserializer.ReadHalf();
		this.ropeClimbing = lhs.HasFlagUnsafe(CharacterSyncData.Flags.ROPE_CLIMBING);
		if (this.ropeClimbing)
		{
			this.ropePercent = deserializer.ReadHalf();
		}
		this.averageVelocity = deserializer.ReadHalf3();
		this.isClimbing = lhs.HasFlagUnsafe(CharacterSyncData.Flags.CLIMBING);
		this.isGrounded = lhs.HasFlagUnsafe(CharacterSyncData.Flags.IS_GROUNDED);
		if (this.isClimbing)
		{
			this.climbPos = deserializer.ReadHalf3();
		}
		this.stammina = deserializer.ReadHalf();
		this.extraStammina = deserializer.ReadHalf();
		this.spectateZoom = deserializer.ReadHalf();
		this.isChargingThrow = deserializer.ReadHalf();
	}

	// Token: 0x04000A1B RID: 2587
	public float3 hipLocation;

	// Token: 0x04000A1C RID: 2588
	public float2 lookValues;

	// Token: 0x04000A1D RID: 2589
	public Vector2 movementInput;

	// Token: 0x04000A1E RID: 2590
	public bool sprintIsPressed;

	// Token: 0x04000A1F RID: 2591
	public float sinceGrounded;

	// Token: 0x04000A20 RID: 2592
	public bool ropeClimbing;

	// Token: 0x04000A21 RID: 2593
	public float ropePercent;

	// Token: 0x04000A22 RID: 2594
	public float3 averageVelocity;

	// Token: 0x04000A23 RID: 2595
	public bool isClimbing;

	// Token: 0x04000A24 RID: 2596
	public bool isGrounded;

	// Token: 0x04000A25 RID: 2597
	public float3 climbPos;

	// Token: 0x04000A26 RID: 2598
	public float stammina;

	// Token: 0x04000A27 RID: 2599
	public float extraStammina;

	// Token: 0x04000A28 RID: 2600
	public float spectateZoom;

	// Token: 0x04000A29 RID: 2601
	public float isChargingThrow;

	// Token: 0x02000493 RID: 1171
	[Flags]
	public enum Flags : byte
	{
		// Token: 0x04001A18 RID: 6680
		NONE = 0,
		// Token: 0x04001A19 RID: 6681
		SPRINT = 1,
		// Token: 0x04001A1A RID: 6682
		ROPE_CLIMBING = 2,
		// Token: 0x04001A1B RID: 6683
		WALK_RIGHT = 4,
		// Token: 0x04001A1C RID: 6684
		WALK_LEFT = 8,
		// Token: 0x04001A1D RID: 6685
		WALK_FORWARD = 16,
		// Token: 0x04001A1E RID: 6686
		WALK_BACKWARD = 32,
		// Token: 0x04001A1F RID: 6687
		CLIMBING = 64,
		// Token: 0x04001A20 RID: 6688
		IS_GROUNDED = 128
	}
}
