using System;
using Photon.Pun;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Settings;

// Token: 0x0200022D RID: 557
public class CharacterClimbing : MonoBehaviour
{
	// Token: 0x060010FB RID: 4347 RVA: 0x00054688 File Offset: 0x00052888
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		this.character = base.GetComponent<Character>();
		Character character = this.character;
		character.dragTowardsAction = (Action<Vector3, float>)Delegate.Combine(character.dragTowardsAction, new Action<Vector3, float>(this.GetDragged));
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x000546D4 File Offset: 0x000528D4
	private void FixedUpdate()
	{
		if (this.character.data.currentClimbHandle)
		{
			this.HandleClimbHandle();
		}
		if (this.character.data.isClimbing)
		{
			this.Climbing();
		}
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x0005470C File Offset: 0x0005290C
	private void Update()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.ClimbHandleUpdate();
		this.sinceLastClimbStarted += Time.deltaTime;
		if (!this.character.data.isClimbing)
		{
			this.sprintHasBeenPressedSinceClimb = false;
			this.climbToggledOn = false;
			if (this.character.data.currentClimbHandle == null)
			{
				this.TryToStartWallClimb(false, default(Vector3), false, 1.25f);
			}
			return;
		}
		if (this.character.input.sprintWasPressed || this.character.input.sprintToggleWasPressed)
		{
			this.sprintHasBeenPressedSinceClimb = true;
		}
		if (this.sprintHasBeenPressedSinceClimb && (this.character.input.sprintIsPressed || this.character.input.sprintToggleIsPressed) && this.character.data.sinceClimbJump > 1f && this.character.data.outOfStaminaFor < 0.5f && this.character.input.movementInput.magnitude > 0.1f && this.character.input.movementInput.normalized.y > -0.9f)
		{
			this.character.refs.view.RPC("RPCA_ClimbJump", RpcTarget.All, Array.Empty<object>());
		}
		this.sinceShake += Time.deltaTime;
		if (this.character.OutOfStamina() && this.sinceShake > 0.1f && this.character.refs.view.IsMine)
		{
			GamefeelHandler.instance.AddPerlinShake(3f * Mathf.Clamp01(this.character.data.outOfStaminaFor * 1f), 0.2f, 10f);
			this.sinceShake = 0f;
		}
		float num = this.maxStaminaUsage * Mathf.Clamp(this.character.input.movementInput.magnitude, 0f, 1f);
		float min = this.minStaminaUsage * this.climbingStamMinimumMultiplier;
		num = Mathf.Clamp(num, min, this.maxStaminaUsage);
		if (!this.character.data.staticClimbCost)
		{
			num *= this.GetAngleUsage();
		}
		num *= this.character.data.staminaMod;
		this.character.UseStamina(num * Time.deltaTime, true);
		this.TestAchievement();
		if (this.character.input.jumpWasPressed || (this.character.input.usePrimaryWasReleased && !this.climbToggledOn) || this.character.data.currentRagdollControll < 0.25f)
		{
			this.view.RPC("StopClimbingRpc", RpcTarget.All, new object[]
			{
				this.GetFallSpeed()
			});
		}
		this.climbingStamMinimumMultiplier = 1f;
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x000549FC File Offset: 0x00052BFC
	private float GetAngleUsage()
	{
		float value = Vector3.Angle(Vector3.up, this.character.data.climbNormal);
		float t = Mathf.InverseLerp(40f, 60f, value);
		return Mathf.Lerp(0.2f, 1f, t);
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x00054A48 File Offset: 0x00052C48
	private void ClimbHandleUpdate()
	{
		if (this.character.data.currentClimbHandle && this.view.IsMine)
		{
			if (this.character.data.fullyPassedOut || this.character.data.dead)
			{
				this.CancelHandle(false);
				return;
			}
			if (this.character.input.jumpWasPressed)
			{
				if (GameHandler.Instance.SettingsHandler.GetSetting<JumpToClimbSetting>().Value == OffOnMode.ON)
				{
					this.CancelHandle(true);
					return;
				}
				this.CancelHandle(false);
				return;
			}
			else
			{
				if (this.character.data.isRopeClimbing)
				{
					this.CancelHandle(false);
					return;
				}
				if (this.character.data.isVineClimbing)
				{
					this.CancelHandle(false);
					return;
				}
			}
		}
		else
		{
			this.handleOffset = Vector2.zero;
		}
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x00054B24 File Offset: 0x00052D24
	public void CancelHandle(bool grabWall = true)
	{
		if (grabWall && this.character.IsLocal)
		{
			this.TryToStartWallClimb(true, this.character.data.currentClimbHandle.transform.forward, false, 1.25f);
		}
		this.character.data.currentClimbHandle.view.RPC("RPCA_UnHang", RpcTarget.All, new object[]
		{
			this.view
		});
		this.handleOffset = Vector2.zero;
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x00054BA4 File Offset: 0x00052DA4
	private void HandleClimbHandle()
	{
		this.handleOffset = Vector2.Lerp(this.handleOffset, this.character.input.movementInput, Time.fixedDeltaTime);
		if (this.handleOffset.magnitude > 0.3f && this.view.IsMine)
		{
			this.CancelHandle(true);
			return;
		}
		this.character.data.sinceGrounded = 0f;
		Vector3 b = (this.character.GetBodypartRig(BodypartType.Hand_R).position + this.character.GetBodypartRig(BodypartType.Hand_L).position) * 0.5f;
		Vector3 vector = this.character.data.currentClimbHandle.transform.TransformPoint(new Vector3(0f, -0.7f, -0.3f));
		this.character.MoveBodypartTowardsPoint(BodypartType.Hand_L, vector, 100f, 1f);
		this.character.MoveBodypartTowardsPoint(BodypartType.Hand_R, vector, 100f, 1f);
		Vector3 b2 = this.character.TorsoPos() - b;
		Vector3 a = vector + b2 - this.character.TorsoPos();
		a += this.character.data.currentClimbHandle.transform.up * this.handleOffset.y;
		a += this.character.data.currentClimbHandle.transform.right * this.handleOffset.x;
		this.character.AddForce(a * 50f, 1f, 1f);
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x00054D50 File Offset: 0x00052F50
	public void StopClimbing()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		Debug.Log("StopClimbing");
		this.view.RPC("StopClimbingRpc", RpcTarget.All, new object[]
		{
			this.GetFallSpeed()
		});
	}

	// Token: 0x06001103 RID: 4355 RVA: 0x00054D90 File Offset: 0x00052F90
	[PunRPC]
	public void RPCA_ClimbJump()
	{
		this.character.data.sinceClimbJump = 0f;
		this.character.UseStamina(0.2f, true);
		this.playerSlide += this.character.input.movementInput.normalized * 8f;
		if (this.view.IsMine && !this.character.isBot)
		{
			GamefeelHandler.instance.AddPerlinShake(10f, 0.5f, 10f);
			GUIManager.instance.ClimbJump();
		}
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x00054E34 File Offset: 0x00053034
	private void GetDragged(Vector3 targetPos, float force)
	{
		this.character.data.climbPos += Vector3.ClampMagnitude(targetPos - this.character.Center, 1f) * (force * Time.fixedDeltaTime * 0.1f);
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x00054E8C File Offset: 0x0005308C
	private void Climbing()
	{
		if (!this.character.OutOfStamina())
		{
			this.character.data.sinceGrounded = 0f;
		}
		if (this.character.data.sinceClimbJump > 0.5f)
		{
			this.playerSlide += Vector2.down * 200f * Mathf.Clamp01(Mathf.Pow(this.character.data.outOfStaminaFor * 0.15f, 2f)) * Time.fixedDeltaTime;
		}
		if (!this.SampleWall(this.GetRequestedPostition()).transform)
		{
			if (this.view.IsMine)
			{
				this.view.RPC("StopClimbingRpc", RpcTarget.All, new object[]
				{
					this.GetFallSpeed()
				});
			}
			return;
		}
		this.character.refs.movement.ApplyExtraDrag(this.climbDrag, false);
		this.character.AddForce(this.GetClimbDirection(), 1f, 1f);
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x00054FAC File Offset: 0x000531AC
	private float GetFallSpeed()
	{
		float a = Mathf.InverseLerp(-5f, -60f, this.playerSlide.y) * 5f;
		float b = 0f;
		return Mathf.Max(a, b);
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x00054FE8 File Offset: 0x000531E8
	private Vector3 GetRequestedPostition()
	{
		Vector3 normalized = Vector3.ProjectOnPlane(Vector3.up, this.character.data.climbNormal).normalized;
		Vector3 normalized2 = Vector3.Cross(normalized, this.character.data.climbNormal).normalized;
		Vector3 a = Vector3.zero;
		ClimbModifierSurface climbMod = this.character.data.climbMod;
		float num = 1f;
		if (climbMod)
		{
			num = climbMod.speedMultiplier;
		}
		if (climbMod && climbMod.onlySlideDown)
		{
			a += normalized * -3f;
		}
		else if (this.character.data.sinceClimbJump > 0.5f && !this.character.OutOfStamina())
		{
			if (this.character.input.movementInput.y < 0f)
			{
				a += normalized * -3f;
			}
			else
			{
				a += normalized * (this.character.input.movementInput.y * this.character.data.staminaMod * num);
			}
		}
		a += this.playerSlide.y * normalized * num;
		a += this.playerSlide.x * -normalized2 * num;
		a += normalized * -0.5f * Mathf.Clamp01(this.character.data.slippy);
		this.playerSlide *= 0.97f;
		this.playerSlide = Vector2.MoveTowards(this.playerSlide, Vector2.zero, Time.fixedDeltaTime * 15f);
		a += -normalized2 * (this.character.input.movementInput.x * this.character.data.staminaMod * num);
		if (this.character.data.currentClimbHandle)
		{
			Vector3 b = Vector3.ClampMagnitude(this.HandlePos() - this.character.data.climbPos, 1f) * 5f;
			float t = 1f;
			if (this.character.data.sinceClimbHandle > 0.5f)
			{
				t = Mathf.Lerp(1f, 0.15f, this.character.input.movementInput.magnitude);
			}
			a = Vector3.Lerp(a, b, t);
		}
		return this.character.data.climbPos + a * (this.climbSpeed * Time.fixedDeltaTime * this.climbSpeedMod);
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x000552C1 File Offset: 0x000534C1
	private Vector3 HandlePos()
	{
		return this.character.data.currentClimbHandle.transform.position + Vector3.down * 1.5f;
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x000552F1 File Offset: 0x000534F1
	private Vector3 GetClimbDirection()
	{
		return (this.VisualClimberPos() - this.character.TorsoPos()) * this.climbForce;
	}

	// Token: 0x0600110A RID: 4362 RVA: 0x00055314 File Offset: 0x00053514
	private Vector3 VisualClimberPos()
	{
		return this.GetVisualClimberPos(this.character.data.climbPos, this.character.data.climbNormal);
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x0005533C File Offset: 0x0005353C
	private Vector3 GetVisualClimberPos(Vector3 samplePos, Vector3 sampleNormal)
	{
		return samplePos + sampleNormal * 0.4f;
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x00055350 File Offset: 0x00053550
	private RaycastHit SampleWall(Vector3 samplePos)
	{
		this.character.data.staticClimbCost = false;
		Vector3 from = this.RaycastPos();
		Vector3 to = samplePos + this.character.data.climbNormal * 0.5f;
		Vector3 to2 = samplePos + this.character.data.climbNormal * -1f;
		RaycastHit raycastHit = HelperFunctions.LineCheck(from, to, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(from, to2, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(from, to2, HelperFunctions.LayerType.TerrainMap, 0.1f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(from, to2, HelperFunctions.LayerType.TerrainMap, 0.2f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(from, to2, HelperFunctions.LayerType.TerrainMap, 0.3f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform == null)
		{
			raycastHit = HelperFunctions.LineCheck(from, to2, HelperFunctions.LayerType.TerrainMap, 0.4f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform == null)
		{
			return default(RaycastHit);
		}
		if (raycastHit.transform)
		{
			this.character.data.climbMod = raycastHit.collider.GetComponent<ClimbModifierSurface>();
			float num = Vector3.Angle(raycastHit.normal, Vector3.up);
			if (this.character.data.climbMod)
			{
				num = this.character.data.climbMod.OverrideClimbAngle(this.character, num);
				this.character.data.staticClimbCost = this.character.data.climbMod.staticClimbCost;
			}
			float num2 = num - 90f;
			if (num2 > 0f)
			{
				if (Mathf.Abs(num2) > (float)(this.character.OutOfStamina() ? 60 : 80))
				{
					return default(RaycastHit);
				}
			}
			else if (this.character.data.sinceClimbJump > 0.3f)
			{
				if (this.character.input.movementInput.magnitude < 0.1f)
				{
					if (Mathf.Abs(num2) > 60f)
					{
						this.CheckFallDamage(raycastHit);
						return default(RaycastHit);
					}
				}
				else if (Mathf.Abs(num2) > 40f)
				{
					this.CheckFallDamage(raycastHit);
					return default(RaycastHit);
				}
			}
			if (this.character.data.climbMod != null)
			{
				this.character.data.climbMod.OnClimb(this.character);
			}
			this.character.data.climbPos = raycastHit.point;
			this.character.data.climbNormal = raycastHit.normal;
			this.character.data.climbHit = raycastHit;
		}
		return raycastHit;
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x00055634 File Offset: 0x00053834
	private void CheckFallDamage(RaycastHit hit)
	{
		if (this.playerSlide.y > 0f)
		{
			return;
		}
		float num = (Mathf.Abs(this.playerSlide.y) - 15f) * 0.035f;
		if (num < 0.15f)
		{
			return;
		}
		num -= 0.05f;
		this.character.data.sinceGrounded = 0f;
		this.playerSlide = Vector2.zero;
		if (num > 0.3f && this.character.IsLocal)
		{
			this.character.Fall(num * 5f, 0f);
		}
		Debug.Log("Damage: " + num.ToString());
		num *= Ascents.fallDamageMultiplier;
		if (this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, num, false, true, true))
		{
			Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.FallDamageTaken, num);
		}
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x00055714 File Offset: 0x00053914
	private bool AcceptableGrabAngle(Vector3 normal, Collider collider)
	{
		float num = Vector3.Angle(normal, Vector3.up);
		ClimbModifierSurface component = collider.GetComponent<ClimbModifierSurface>();
		if (component)
		{
			num = component.OverrideClimbAngle(this.character, num);
		}
		float num2 = num - 90f;
		if (num2 > 0f)
		{
			if (Mathf.Abs(num2) > 80f)
			{
				return false;
			}
		}
		else if (Mathf.Abs(num2) > 40f)
		{
			return false;
		}
		return true;
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x0005577C File Offset: 0x0005397C
	private void TryToStartWallClimb(bool forceAttempt = false, Vector3 overide = default(Vector3), bool botGrab = false, float raycastDistance = 1.25f)
	{
		string str = "Trying to start wall climb.";
		if (!this.CanClimb())
		{
			return;
		}
		if (this.character.isBot && !botGrab)
		{
			return;
		}
		if (!this.view.IsMine)
		{
			return;
		}
		Vector3 vector = MainCamera.instance.transform.position;
		Vector3 a = this.character.data.lookDirection;
		if (botGrab)
		{
			vector = this.character.Center;
			a = this.character.data.lookDirection_Flat.normalized;
		}
		if (forceAttempt)
		{
			a = overide;
		}
		Vector3 to = vector + a * raycastDistance;
		RaycastHit raycastHit = HelperFunctions.LineCheck(vector, to, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (!raycastHit.transform)
		{
			raycastHit = HelperFunctions.LineCheck(vector, to, HelperFunctions.LayerType.TerrainMap, 0.05f, QueryTriggerInteraction.Ignore);
		}
		if (!raycastHit.transform)
		{
			raycastHit = HelperFunctions.LineCheck(vector, to, HelperFunctions.LayerType.TerrainMap, 0.1f, QueryTriggerInteraction.Ignore);
		}
		if (raycastHit.transform && this.AcceptableGrabAngle(raycastHit.normal, raycastHit.collider) && (this.sinceLastClimbStarted > 1f || !this.character.OutOfStamina()))
		{
			this.character.data.sinceCanClimb = 0f;
			if (this.character.data.sincePressClimb < 0.1f || this.validJumpToClimb || forceAttempt || botGrab)
			{
				this.character.refs.items.EquipSlot(Optionable<byte>.None);
				if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.Gamepad)
				{
					this.climbToggledOn = true;
				}
				this.sinceLastClimbStarted = 0f;
				this.view.RPC("StartClimbRpc", RpcTarget.All, new object[]
				{
					raycastHit.point,
					raycastHit.normal
				});
				str += "\nClimb started.";
			}
		}
	}

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06001110 RID: 4368 RVA: 0x00055954 File Offset: 0x00053B54
	private bool validJumpToClimb
	{
		get
		{
			return this.character.input.jumpWasPressed && this.character.data.sinceGrounded > 0.1f && GameHandler.Instance.SettingsHandler.GetSetting<JumpToClimbSetting>().Value == OffOnMode.ON;
		}
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x000559A4 File Offset: 0x00053BA4
	public bool CanClimb()
	{
		return this.character.data.sinceClimb >= 0.2f && !this.character.data.isRopeClimbing && !this.character.data.isVineClimbing;
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x000559F3 File Offset: 0x00053BF3
	private Vector3 RaycastPos()
	{
		return this.character.data.climbPos + this.character.data.climbNormal * 0.4f;
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x00055A24 File Offset: 0x00053C24
	[PunRPC]
	private void StartClimbRpc(Vector3 climbPos, Vector3 climbNormal)
	{
		float num = 0f;
		if (this.character.data.hasClimbedSinceGrounded)
		{
			Vector3 vector = this.GetVisualClimberPos(climbPos, climbNormal) - (this.character.Center + Vector3.up * 0.5f);
			vector = Vector3.ProjectOnPlane(vector * 1.5f, climbNormal);
			float num2 = vector.magnitude;
			if (Vector3.Dot(vector, Vector3.up) < 0f)
			{
				num2 = 0f;
			}
			num2 = Mathf.Max(num2, 0.1f);
			this.character.UseStamina(0.15f * num2, true);
			if (this.character.OutOfStamina())
			{
				num += -num2 * this.outOfStamAttachSlide;
			}
		}
		if (this.character.data.avarageVelocity.y < 0f)
		{
			num += this.character.data.avarageVelocity.y * 1.5f;
		}
		this.character.OutOfStamina();
		this.playerSlide = new Vector2(this.playerSlide.x, num);
		this.character.data.climbPos = climbPos;
		this.character.data.climbNormal = climbNormal;
		this.character.data.hasClimbedSinceGrounded = true;
		this.character.data.isClimbing = true;
		this.character.data.isGrounded = false;
		this.character.data.sinceStartClimb = 0f;
		this.character.OnStartClimb();
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x00055BB8 File Offset: 0x00053DB8
	public void StopAnyClimbing()
	{
		if (this.character.data.isVineClimbing)
		{
			this.character.refs.vineClimbing.Stop();
			return;
		}
		if (this.character.data.isRopeClimbing)
		{
			this.character.refs.ropeHandling.Stop();
			return;
		}
		if (this.character.data.isClimbing)
		{
			this.view.RPC("StopClimbingRpc", RpcTarget.All, new object[]
			{
				0f
			});
		}
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x00055C4C File Offset: 0x00053E4C
	[PunRPC]
	public void StopClimbingRpc(float setFall)
	{
		this.character.data.isClimbing = false;
		this.character.data.isJumping = false;
		this.character.data.sinceGrounded = setFall;
		if (this.character.OutOfStamina())
		{
			this.character.data.sinceGrounded = Mathf.Clamp(this.character.data.sinceGrounded, 0.5f, 1000f);
		}
		this.playerSlide = Vector2.zero;
		this.climbToggledOn = false;
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x00055CDA File Offset: 0x00053EDA
	internal void StartHang(ClimbHandle climbHandle)
	{
		this.character.data.currentClimbHandle = climbHandle;
		this.character.data.sinceClimbHandle = 0f;
		this.character.data.sinceGrounded = 0f;
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x00055D18 File Offset: 0x00053F18
	internal void TryClimb(float raycastDistance = 1.25f)
	{
		this.TryToStartWallClimb(false, default(Vector3), true, raycastDistance);
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x00055D38 File Offset: 0x00053F38
	internal void TestAchievement()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		if (this.character.data.isClimbing && (this.character.Center.y - this.character.data.lastGroundedHeight) * CharacterStats.unitsToMeters >= 50f)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.EnduranceBadge);
		}
	}

	// Token: 0x04000EF5 RID: 3829
	private Character character;

	// Token: 0x04000EF6 RID: 3830
	public float outOfStamAttachSlide = 1f;

	// Token: 0x04000EF7 RID: 3831
	public float climbForce;

	// Token: 0x04000EF8 RID: 3832
	public float climbSpeed;

	// Token: 0x04000EF9 RID: 3833
	public float climbSpeedMod = 1f;

	// Token: 0x04000EFA RID: 3834
	public float climbDrag = 0.85f;

	// Token: 0x04000EFB RID: 3835
	public float maxStaminaUsage = 0.2f;

	// Token: 0x04000EFC RID: 3836
	public float minStaminaUsage = 0.02f;

	// Token: 0x04000EFD RID: 3837
	[HideInInspector]
	public float climbingStamMinimumMultiplier = 1f;

	// Token: 0x04000EFE RID: 3838
	private PhotonView view;

	// Token: 0x04000EFF RID: 3839
	public Vector2 playerSlide;

	// Token: 0x04000F00 RID: 3840
	private float sinceShake;

	// Token: 0x04000F01 RID: 3841
	private Vector2 handleOffset;

	// Token: 0x04000F02 RID: 3842
	private bool sprintHasBeenPressedSinceClimb;

	// Token: 0x04000F03 RID: 3843
	private bool climbToggledOn;

	// Token: 0x04000F04 RID: 3844
	private float sinceLastClimbStarted;
}
