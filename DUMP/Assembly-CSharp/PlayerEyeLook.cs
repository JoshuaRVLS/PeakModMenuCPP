using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200015E RID: 350
public class PlayerEyeLook : MonoBehaviour
{
	// Token: 0x06000B8A RID: 2954 RVA: 0x0003DD63 File Offset: 0x0003BF63
	private void Start()
	{
		this.localCharacter = base.GetComponent<Character>();
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x0003DD74 File Offset: 0x0003BF74
	private void Update()
	{
		this.characters = Character.AllCharacters;
		this.distance = float.PositiveInfinity;
		for (int i = 0; i < this.characters.Count; i++)
		{
			float num = Vector3.Distance(this.characters[i].Center, this.localCharacter.Center);
			if (num < this.distance && this.characters[i] != this.localCharacter)
			{
				this.distance = num;
				this.character = this.characters[i];
			}
			AnimatedMouth component = this.characters[i].GetComponent<AnimatedMouth>();
			if (num < this.listenRange && component.isSpeaking && this.characters[i] != this.localCharacter)
			{
				this.distance = num;
				this.character = this.characters[i];
			}
		}
		if (this.character != null)
		{
			this.lookDir = (this.character.Head - this.localCharacter.Head).normalized;
			this.lookDelta = this.localCharacter.GetBodypart(BodypartType.Head).transform.forward - this.lookDir;
			base.transform.InverseTransformDirection(this.lookDelta);
			this.UpDelta = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.up, this.lookDelta);
			this.RightDelta = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.right, this.lookDelta);
			this.lookAngle = Vector3.Angle(this.localCharacter.data.lookDirection, this.lookDir);
		}
		if (this.character != null && this.distance < this.lookRange && this.lookAngle < this.lookAngleMax)
		{
			this.eyeTarget = new Vector2(this.RightDelta * -this.XMax, this.UpDelta * this.YMax);
			this.lookingAtCharacter = true;
		}
		else
		{
			this.lookingAtCharacter = false;
			Vector3 forward = this.localCharacter.GetBodypart(BodypartType.Hip).transform.forward;
			forward.y = 0f;
			Vector3 rhs = this.localCharacter.data.lookDirection - forward;
			float num2 = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.right, rhs);
			float num3 = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.up, rhs);
			this.eyeTarget = new Vector2(num2 * this.XMax, num3 * -this.YMax);
		}
		float num4 = 1f;
		if (this.character != this.lastCharacter)
		{
			num4 = 0.3f;
		}
		this.eyePos = Vector2.Lerp(this.eyePos, this.eyeTarget, Time.deltaTime * this.lookSmoothing * num4);
		for (int j = 0; j < this.eyeRenderers.Length; j++)
		{
			this.eyeRenderers[j].material.SetVector("_EyePosition", this.eyePos);
		}
		if (Vector3.Distance(this.lastViewDir, this.localCharacter.GetBodypart(BodypartType.Head).transform.forward) > this.xLookThreshold)
		{
			this.lastViewDir = this.localCharacter.GetBodypart(BodypartType.Head).transform.forward;
		}
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x0003E108 File Offset: 0x0003C308
	private void OnDrawGizmosSelected()
	{
		if (this.localCharacter == null)
		{
			return;
		}
		if (this.lookingAtCharacter)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(this.localCharacter.Head, this.lookDir * this.lookRange);
		}
		else
		{
			Gizmos.color = Color.yellow;
			Vector3 forward = this.localCharacter.GetBodypart(BodypartType.Head).transform.forward;
			forward.y = 0f;
			Gizmos.DrawRay(this.localCharacter.Head, forward * this.lookRange);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawRay(this.localCharacter.Head, this.localCharacter.GetBodypart(BodypartType.Head).transform.forward * this.lookRange);
	}

	// Token: 0x04000A97 RID: 2711
	public bool lookingAtCharacter;

	// Token: 0x04000A98 RID: 2712
	private List<Character> characters = new List<Character>();

	// Token: 0x04000A99 RID: 2713
	public float distance;

	// Token: 0x04000A9A RID: 2714
	public float lookRange;

	// Token: 0x04000A9B RID: 2715
	public float listenRange;

	// Token: 0x04000A9C RID: 2716
	private Character lastCharacter;

	// Token: 0x04000A9D RID: 2717
	public float lookSmoothing;

	// Token: 0x04000A9E RID: 2718
	public Character character;

	// Token: 0x04000A9F RID: 2719
	public Renderer[] eyeRenderers;

	// Token: 0x04000AA0 RID: 2720
	private Vector3 lookDir;

	// Token: 0x04000AA1 RID: 2721
	public float lookAngleMax;

	// Token: 0x04000AA2 RID: 2722
	private Vector3 lookDelta;

	// Token: 0x04000AA3 RID: 2723
	private float RightDelta;

	// Token: 0x04000AA4 RID: 2724
	private float UpDelta;

	// Token: 0x04000AA5 RID: 2725
	public float lookAngle;

	// Token: 0x04000AA6 RID: 2726
	public float xLookThreshold;

	// Token: 0x04000AA7 RID: 2727
	[FormerlySerializedAs("leftRightMax")]
	public float XMax;

	// Token: 0x04000AA8 RID: 2728
	[FormerlySerializedAs("upDownMax")]
	public float YMax;

	// Token: 0x04000AA9 RID: 2729
	private Character localCharacter;

	// Token: 0x04000AAA RID: 2730
	private Vector2 eyePos = Vector2.zero;

	// Token: 0x04000AAB RID: 2731
	private Vector2 eyeTarget = Vector2.zero;

	// Token: 0x04000AAC RID: 2732
	private Vector3 lastViewDir;
}
