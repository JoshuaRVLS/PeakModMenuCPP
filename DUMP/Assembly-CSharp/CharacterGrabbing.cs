using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200022E RID: 558
public class CharacterGrabbing : MonoBehaviour
{
	// Token: 0x0600111A RID: 4378 RVA: 0x00055DF5 File Offset: 0x00053FF5
	private void Start()
	{
		this.character = base.GetComponent<Character>();
		Bodypart bodypart = this.character.GetBodypart(BodypartType.Hand_R);
		bodypart.collisionStayAction = (Action<Collision>)Delegate.Combine(bodypart.collisionStayAction, new Action<Collision>(this.GrabAction));
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x00055E34 File Offset: 0x00054034
	private void GrabAction(Collision collision)
	{
		if (!this.character.data.isScoutmaster)
		{
			return;
		}
		if (!this.character.photonView.IsMine)
		{
			return;
		}
		if (this.character.data.grabJoint)
		{
			return;
		}
		if (!this.character.data.isReaching)
		{
			return;
		}
		if (this.character.data.sinceLetGoOfFriend < 0.35f)
		{
			return;
		}
		if (!collision.rigidbody)
		{
			return;
		}
		Character character;
		if (!CharacterRagdoll.TryGetCharacterFromCollider(collision.collider, out character))
		{
			return;
		}
		if (!character)
		{
			return;
		}
		if (character == this.character)
		{
			return;
		}
		BodypartType partType = character.GetPartType(collision.rigidbody);
		if (partType == (BodypartType)(-1))
		{
			return;
		}
		this.character.photonView.RPC("RPCA_GrabAttach", RpcTarget.All, new object[]
		{
			character.photonView,
			(int)partType,
			collision.rigidbody.transform.InverseTransformPoint(this.character.GetBodypart(BodypartType.Hand_R).Rig.transform.position)
		});
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x00055F54 File Offset: 0x00054154
	[PunRPC]
	public void RPCA_GrabAttach(PhotonView view, int bodyPartID, Vector3 relativePos)
	{
		Character component = view.GetComponent<Character>();
		if (!component)
		{
			return;
		}
		Rigidbody rig = component.GetBodypart((BodypartType)bodyPartID).Rig;
		Rigidbody rig2 = this.character.GetBodypart(BodypartType.Hand_R).Rig;
		rig2.transform.position = rig.transform.TransformPoint(relativePos);
		this.character.data.grabJoint = rig2.gameObject.AddComponent<FixedJoint>();
		this.character.data.grabJoint.connectedBody = rig;
		component.BreakCharacterCarrying(false);
		this.character.data.grabbedPlayer = component;
		component.data.grabbingPlayer = this.character;
		Debug.Log(string.Format("Grab Attaching {0} to {1}", component, rig));
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x00056018 File Offset: 0x00054218
	[PunRPC]
	public void RPCA_GrabUnattach()
	{
		if (this.character.data.grabbedPlayer)
		{
			this.character.data.grabbedPlayer.data.grabbingPlayer = null;
		}
		this.character.data.grabbedPlayer = null;
		Object.Destroy(this.character.data.grabJoint);
		this.character.data.sinceLetGoOfFriend = 0f;
		Debug.Log("Grab unattaching");
	}

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x0600111E RID: 4382 RVA: 0x0005609C File Offset: 0x0005429C
	public bool isKickMode
	{
		get
		{
			return RunSettings.GetValue(RunSettings.SETTINGTYPE.HelpingHand, false) == 1;
		}
	}

	// Token: 0x0600111F RID: 4383 RVA: 0x000560AC File Offset: 0x000542AC
	private void Update()
	{
		if (!this.character.refs.view.IsMine)
		{
			return;
		}
		if (this.character.data.grabbingPlayer && this.character.input.jumpWasPressed && !this.character.data.grabbingPlayer.isBot)
		{
			this.character.data.grabbingPlayer.refs.view.RPC("RPCA_GrabUnattach", RpcTarget.All, Array.Empty<object>());
		}
		if (this.isKickMode)
		{
			if (this.character.data.isKicking)
			{
				this._kickTime += Time.deltaTime;
				if (this._kickTime > 1f)
				{
					this.character.data.isKicking = false;
					this._kickTime = 0f;
				}
			}
			if (this.character.data.sincePressReach < 0.2f && !this.character.data.isKicking && !this.character.data.isClimbingAnything && this.character.data.isGrounded && !this.character.OutOfRegularStamina())
			{
				this.character.refs.view.RPC("RPCA_Kick", RpcTarget.All, Array.Empty<object>());
			}
			return;
		}
		if (!this.CanGrab())
		{
			if (this.character.data.grabJoint || this.character.data.isReaching)
			{
				this.character.refs.view.RPC("RPCA_StopReaching", RpcTarget.All, Array.Empty<object>());
			}
			return;
		}
		if (this.character.data.sincePressReach < 0.2f)
		{
			if (!this.character.data.isReaching)
			{
				this.character.refs.view.RPC("RPCA_StartReaching", RpcTarget.All, Array.Empty<object>());
			}
		}
		else if (this.character.data.isReaching)
		{
			this.character.refs.view.RPC("RPCA_StopReaching", RpcTarget.All, Array.Empty<object>());
		}
		if (this.character.data.grabJoint)
		{
			if (this.character.data.grabbedPlayer)
			{
				this.character.data.grabbedPlayer.LimitFalling();
			}
			if (!this.character.data.isReaching)
			{
				this.character.refs.view.RPC("RPCA_GrabUnattach", RpcTarget.All, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06001120 RID: 4384 RVA: 0x00056352 File Offset: 0x00054552
	private void FixedUpdate()
	{
		this.character.data.grabFriendDistance = 1000f;
		if (this.character.data.isReaching)
		{
			this.Reach();
		}
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x00056381 File Offset: 0x00054581
	[PunRPC]
	private void RPCA_StopReaching()
	{
		this.character.data.isReaching = false;
		if (this.character.data.grabJoint)
		{
			Object.Destroy(this.character.data.grabJoint);
		}
	}

	// Token: 0x06001122 RID: 4386 RVA: 0x000563C0 File Offset: 0x000545C0
	[PunRPC]
	private void RPCA_StartGrabbing()
	{
		this.character.data.isReaching = false;
	}

	// Token: 0x06001123 RID: 4387 RVA: 0x000563D3 File Offset: 0x000545D3
	[PunRPC]
	private void RPCA_StartReaching()
	{
		this.character.data.isReaching = true;
	}

	// Token: 0x06001124 RID: 4388 RVA: 0x000563E8 File Offset: 0x000545E8
	[PunRPC]
	private void RPCA_Kick()
	{
		this.character.refs.animator.SetTrigger("Kick");
		this.character.data.isKicking = true;
		if (this.character.refs.view.IsMine)
		{
			this.KickCast();
		}
	}

	// Token: 0x06001125 RID: 4389 RVA: 0x0005643D File Offset: 0x0005463D
	[PunRPC]
	private void RPCA_KickImpact(Vector3 position)
	{
		this.kickImpactSFX.Play(position);
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x0005644C File Offset: 0x0005464C
	public void KickCast()
	{
		CharacterGrabbing.<>c__DisplayClass24_0 CS$<>8__locals1 = new CharacterGrabbing.<>c__DisplayClass24_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.characters = new List<Character>(Character.AllCharacters);
		if (ZombieManager.Instance)
		{
			foreach (MushroomZombie mushroomZombie in ZombieManager.Instance.zombies)
			{
				CS$<>8__locals1.characters.Add(mushroomZombie.character);
			}
		}
		float currentStamina = this.character.data.currentStamina;
		this.character.UseStamina(1f, true);
		base.StartCoroutine(CS$<>8__locals1.<KickCast>g__KickRoutine|1(currentStamina));
		base.StartCoroutine(CS$<>8__locals1.<KickCast>g__ParticleRoutineIDK|0());
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x00056514 File Offset: 0x00054714
	private void Reach()
	{
		foreach (Character character in Character.AllCharacters)
		{
			float num = Vector3.Distance(this.character.Center, character.Center);
			if (num <= 4f && Vector3.Angle(this.character.data.lookDirection, character.Center - this.character.Center) <= 60f && this.TargetCanBeHelped(character))
			{
				if (character.IsStuck() && character.IsLocal)
				{
					character.UnStick();
				}
				character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Web, 1f, false, false);
				if (num < this.character.data.grabFriendDistance)
				{
					this.character.data.grabFriendDistance = num;
					this.character.data.sinceGrabFriend = 0f;
				}
				if (this.character.refs.view.IsMine)
				{
					GUIManager.instance.Grasp();
				}
				if (character.refs.view.IsMine)
				{
					character.DragTowards(this.character.Center, 70f);
					character.LimitFalling();
					GUIManager.instance.Grasp();
				}
			}
		}
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x00056698 File Offset: 0x00054898
	private bool TargetCanBeHelped(Character item)
	{
		if (item != this.character)
		{
			if (item.IsStuck() || item.data.sinceUnstuck < 1f)
			{
				return true;
			}
			if (item.refs.afflictions.isWebbed)
			{
				return true;
			}
			if (item.data.isClimbing && item.Center.y < this.character.Center.y + 1f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x00056718 File Offset: 0x00054918
	private bool CanGrab()
	{
		return !(this.character.data.currentItem != null) && Time.time - this.character.data.lastConsumedItem >= 0.5f && !this.character.data.isClimbing && !this.character.data.isRopeClimbing && !this.character.data.isVineClimbing;
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x0005679C File Offset: 0x0005499C
	internal void Throw(Vector3 force, float fallSeconds)
	{
		this.character.data.grabbedPlayer.RPCA_Fall(1f);
		this.character.data.grabbedPlayer.AddForce(force, 0.7f, 1f);
		this.RPCA_GrabUnattach();
	}

	// Token: 0x04000F05 RID: 3845
	public ParticleSystem kickParticle;

	// Token: 0x04000F06 RID: 3846
	private const float _maxKickTime = 1f;

	// Token: 0x04000F07 RID: 3847
	private float _kickTime;

	// Token: 0x04000F08 RID: 3848
	private Character character;

	// Token: 0x04000F09 RID: 3849
	public float kickForce = 10f;

	// Token: 0x04000F0A RID: 3850
	public float kickRange = 3f;

	// Token: 0x04000F0B RID: 3851
	public float kickRagdollTime = 1f;

	// Token: 0x04000F0C RID: 3852
	public float kickDistance = 1f;

	// Token: 0x04000F0D RID: 3853
	public float kickAngle = 45f;

	// Token: 0x04000F0E RID: 3854
	public float kickDelay = 0.6f;

	// Token: 0x04000F0F RID: 3855
	public SFX_Instance kickImpactSFX;
}
