using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000149 RID: 329
[DefaultExecutionOrder(-100)]
public class CharacterSyncer : PhotonBinaryStreamSerializer<CharacterSyncData>
{
	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x06000AE7 RID: 2791 RVA: 0x0003A74E File Offset: 0x0003894E
	public Vector3 LastPosition
	{
		get
		{
			return this.lastPosition.Value;
		}
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0003A760 File Offset: 0x00038960
	protected override void Awake()
	{
		base.Awake();
		this.m_character = base.GetComponent<Character>();
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0003A774 File Offset: 0x00038974
	public override CharacterSyncData GetDataToWrite()
	{
		return new CharacterSyncData
		{
			hipLocation = this.m_character.GetBodypart(BodypartType.Hip).Rig.position,
			lookValues = this.m_character.data.lookValues,
			movementInput = this.m_character.input.movementInput,
			sprintIsPressed = this.m_character.input.sprintIsPressed,
			sinceGrounded = this.m_character.data.sinceGrounded,
			ropePercent = this.m_character.data.ropePercent,
			ropeClimbing = this.m_character.data.isRopeClimbing,
			averageVelocity = this.GetAverageVelocity(),
			isClimbing = this.m_character.data.isClimbing,
			isGrounded = this.m_character.data.isGrounded,
			climbPos = this.m_character.data.climbPos,
			stammina = this.m_character.data.currentStamina,
			extraStammina = this.m_character.data.extraStamina,
			spectateZoom = this.m_character.data.spectateZoom,
			isChargingThrow = (float)(this.m_character.refs.items.isChargingThrow ? 1 : 0)
		};
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0003A900 File Offset: 0x00038B00
	public Vector3 GetAverageVelocity()
	{
		if (this.m_character.warping)
		{
			return Vector3.zero;
		}
		Vector3 vector = Vector3.zero;
		foreach (Bodypart bodypart in this.m_character.refs.ragdoll.partList)
		{
			vector += bodypart.Rig.linearVelocity;
		}
		vector /= (float)this.m_character.refs.ragdoll.partList.Count;
		return vector;
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0003A9AC File Offset: 0x00038BAC
	private float ComputeScalar(float incoming, float current)
	{
		if (Mathf.Abs(incoming) >= Mathf.Abs(current))
		{
			return 1f;
		}
		return incoming / current;
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0003A9C5 File Offset: 0x00038BC5
	private float ComputeDelta(float incoming, float current)
	{
		if (Mathf.Abs(incoming) < Mathf.Abs(current))
		{
			return 0f;
		}
		return incoming - current;
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0003A9E0 File Offset: 0x00038BE0
	public override void OnDataReceived(CharacterSyncData data)
	{
		this.sinceLastPackageUpdate = 0f;
		base.OnDataReceived(data);
		if (this.m_character.warping && !this.wasWarping)
		{
			this.lastPreWarpPosition = this.lastPosition;
		}
		this.wasWarping = this.m_character.warping;
		this.lastLook = Optionable<float2>.Some(this.m_character.data.lookValues);
		this.lastPosition = Optionable<float3>.Some(this.m_character.warping ? this.RemoteValue.Value.hipLocation : this.m_character.GetBodypart(BodypartType.Hip).Rig.position);
		Vector3 averageVelocity = this.GetAverageVelocity();
		Vector3 vector = data.averageVelocity;
		Vector3 b = new Vector3(this.ComputeScalar(vector.x, averageVelocity.x), this.ComputeScalar(vector.y, averageVelocity.y), this.ComputeScalar(vector.z, averageVelocity.z));
		Vector3 b2 = new Vector3(this.ComputeDelta(vector.x, averageVelocity.x), this.ComputeDelta(vector.y, averageVelocity.y), this.ComputeDelta(vector.z, averageVelocity.z));
		foreach (Bodypart bodypart in this.m_character.refs.ragdoll.partList)
		{
			if (!bodypart.Rig.isKinematic && !this.m_character.warping)
			{
				bodypart.Rig.linearVelocity = Vector3.Scale(bodypart.Rig.linearVelocity, b) + b2;
			}
		}
		this.m_character.input.movementInput = data.movementInput;
		this.m_character.input.sprintIsPressed = data.sprintIsPressed;
		if (Mathf.Abs(this.m_character.data.sinceGrounded - data.sinceGrounded) > 2f)
		{
			this.m_character.data.sinceGrounded = data.sinceGrounded;
		}
		if (data.ropeClimbing)
		{
			this.m_character.data.ropePercent = data.ropePercent;
		}
		if (data.isClimbing)
		{
			this.m_character.data.climbPos = data.climbPos;
		}
		this.m_character.data.currentStamina = data.stammina;
		this.m_character.data.extraStamina = data.extraStammina;
		this.m_character.data.spectateZoom = data.spectateZoom;
		this.m_character.refs.items.isChargingThrow = (data.isChargingThrow > 0.5f);
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0003ACBC File Offset: 0x00038EBC
	private void Update()
	{
		if (this.photonView.IsMine)
		{
			return;
		}
		if (this.RemoteValue.IsNone)
		{
			return;
		}
		if (this.lastLook.IsNone)
		{
			return;
		}
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackageUpdate += Time.deltaTime;
		float t = (float)((double)this.sinceLastPackageUpdate / num);
		Vector2 b = this.RemoteValue.Value.lookValues;
		Vector2 lookValues = Vector2.Lerp(this.lastLook.Value, b, t);
		this.m_character.data.lookValues = lookValues;
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0003AD60 File Offset: 0x00038F60
	private void FixedUpdate()
	{
		if (this.photonView.IsMine)
		{
			return;
		}
		if (this.RemoteValue.IsNone)
		{
			return;
		}
		if (this.lastPosition.IsNone)
		{
			return;
		}
		if (this.m_character.data.carrier)
		{
			return;
		}
		this.InterpolateRigPositions();
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x0003ADB8 File Offset: 0x00038FB8
	private void OnDrawGizmos()
	{
		if (this.RemoteValue.IsNone)
		{
			return;
		}
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.RemoteValue.Value.hipLocation, 0.2f);
		float t = Mathf.Clamp01(Vector3.Distance(this.tPos, this.LastPosition) / Vector3.Distance(this.LastPosition, this.RemoteValue.Value.hipLocation));
		Gizmos.color = Color.Lerp(Color.red, Color.green, t);
		Gizmos.DrawLine(this.LastPosition, this.tPos);
		Gizmos.DrawCube(this.tPos, 0.1f * Vector3.one);
	}

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x06000AF1 RID: 2801 RVA: 0x0003AE74 File Offset: 0x00039074
	// (set) Token: 0x06000AF2 RID: 2802 RVA: 0x0003AE7C File Offset: 0x0003907C
	public Vector3 tPos { get; private set; }

	// Token: 0x06000AF3 RID: 2803 RVA: 0x0003AE88 File Offset: 0x00039088
	private void InterpolateRigPositions()
	{
		if (this.m_character.warping)
		{
			return;
		}
		Vector3 b = this.RemoteValue.Value.hipLocation;
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackage += Time.fixedDeltaTime * 0.6f;
		float t = (float)((double)this.sinceLastPackage / num);
		this.tPos = Vector3.Lerp(this.lastPosition.Value, b, t);
		Vector3 position = this.m_character.GetBodypart(BodypartType.Hip).Rig.position;
		Vector3 vector = this.tPos - position;
		if (vector.magnitude > 10f)
		{
			this.m_character.refs.ragdoll.MoveAllRigsInDirection(vector);
			this.m_character.refs.ragdoll.HaltBodyVelocity();
			return;
		}
		vector.y *= 0f;
		float f = this.tPos.y - position.y;
		float value = Mathf.Abs(f);
		float d = Mathf.InverseLerp(0.3f, 0.6f, value) * Mathf.Sign(f);
		vector += Vector3.up * d;
		this.m_character.refs.ragdoll.MoveAllRigsInDirection(vector * 0.1f);
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x0003AFE8 File Offset: 0x000391E8
	[ConsoleCommand]
	public static void SnapToRemoteValues()
	{
		foreach (CharacterSyncer characterSyncer in Object.FindObjectsByType<CharacterSyncer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			if (!characterSyncer.m_character.IsLocal)
			{
				characterSyncer.lastPosition = (characterSyncer.RemoteValue.IsSome ? Optionable<float3>.Some(characterSyncer.RemoteValue.Value.hipLocation) : Optionable<float3>.None);
			}
		}
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x0003B04B File Offset: 0x0003924B
	public void BreakIt()
	{
		this.lastPosition = this.lastPreWarpPosition;
	}

	// Token: 0x04000A2A RID: 2602
	private Character m_character;

	// Token: 0x04000A2B RID: 2603
	private Optionable<float3> lastPosition;

	// Token: 0x04000A2C RID: 2604
	private Optionable<float3> lastPreWarpPosition;

	// Token: 0x04000A2D RID: 2605
	private bool wasWarping;

	// Token: 0x04000A2E RID: 2606
	private Optionable<float2> lastLook;

	// Token: 0x04000A2F RID: 2607
	private float sinceLastPackageUpdate;
}
