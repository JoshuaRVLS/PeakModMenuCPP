using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200032D RID: 813
public class Scoutmaster : MonoBehaviour
{
	// Token: 0x17000164 RID: 356
	// (get) Token: 0x0600158C RID: 5516 RVA: 0x0006D05D File Offset: 0x0006B25D
	private bool targetForced
	{
		get
		{
			return Time.time < this.targetForcedUntil;
		}
	}

	// Token: 0x0600158D RID: 5517 RVA: 0x0006D06C File Offset: 0x0006B26C
	public static bool GetPrimaryScoutmaster(out Scoutmaster scoutmaster)
	{
		if (Scoutmaster.AllScoutmasters.Count == 0)
		{
			scoutmaster = null;
			return false;
		}
		scoutmaster = Scoutmaster.AllScoutmasters[0];
		return true;
	}

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x0600158E RID: 5518 RVA: 0x0006D08D File Offset: 0x0006B28D
	// (set) Token: 0x0600158F RID: 5519 RVA: 0x0006D095 File Offset: 0x0006B295
	public Character currentTarget
	{
		get
		{
			return this._currentTarget;
		}
		set
		{
			if (this.targetForced)
			{
				return;
			}
			this._currentTarget = value;
		}
	}

	// Token: 0x06001590 RID: 5520 RVA: 0x0006D0A7 File Offset: 0x0006B2A7
	private void OnEnable()
	{
		Scoutmaster.AllScoutmasters.Add(this);
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x0006D0B4 File Offset: 0x0006B2B4
	internal void SetCurrentTarget(Character setCurrentTarget, float forceForTime = 0f)
	{
		if (setCurrentTarget != null && this.preventSpawning)
		{
			return;
		}
		if (setCurrentTarget != this.currentTarget)
		{
			this.view.RPC("RPCA_SetCurrentTarget", RpcTarget.All, new object[]
			{
				(setCurrentTarget == null) ? -1 : setCurrentTarget.photonView.ViewID,
				forceForTime
			});
		}
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x0006D120 File Offset: 0x0006B320
	[PunRPC]
	private void RPCA_SetCurrentTarget(int targetViewID, float forceForTime)
	{
		if (targetViewID == -1)
		{
			this.currentTarget = null;
		}
		else
		{
			this.currentTarget = PhotonNetwork.GetPhotonView(targetViewID).GetComponent<Character>();
		}
		if (forceForTime > 0f)
		{
			this.targetForcedUntil = Time.time + forceForTime;
		}
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x0006D155 File Offset: 0x0006B355
	private void OnDestroy()
	{
		this.mat.SetFloat(this.STRENGTHID, 0f);
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x0006D16D File Offset: 0x0006B36D
	private void OnDisable()
	{
		this.mat.SetFloat(this.STRENGTHID, 0f);
		Scoutmaster.AllScoutmasters.Remove(this);
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x0006D194 File Offset: 0x0006B394
	private void Start()
	{
		this.animVars = base.GetComponentInChildren<ScoutmasterAnimVars>();
		this.character = base.GetComponent<Character>();
		this.view = base.GetComponent<PhotonView>();
		this.character.data.isScoutmaster = true;
		this.mat.SetFloat(this.STRENGTHID, 0f);
		this.mat.SetFloat(this.GRAINMULTID, (float)(GUIManager.instance.photosensitivity ? 0 : 1));
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x0006D210 File Offset: 0x0006B410
	private void CalcVars()
	{
		this.sinceLookForTarget += Time.deltaTime;
		bool flag = this.currentTarget && this.CanSeeTarget(this.currentTarget);
		if (this.currentTarget)
		{
			if (!flag)
			{
				this.sinceSeenTarget += Time.deltaTime;
			}
			else
			{
				this.sinceSeenTarget = 0f;
			}
		}
		else
		{
			this.sinceSeenTarget = 0f;
		}
		if (this.currentTarget)
		{
			this.distanceToTarget = Vector3.Distance(this.character.Center, this.currentTarget.Center);
		}
		this.sinceAnyoneCanSeeMe += Time.deltaTime;
		if (this.AnyoneCanSeeMe())
		{
			this.sinceAnyoneCanSeeMe = 0f;
		}
		if (!this.currentTarget)
		{
			this.targetHasSeenMeCounter = 0f;
			return;
		}
		bool flag2 = Vector3.Distance(this.character.Center, this.currentTarget.Center) < 10f;
		bool flag3 = HelperFunctions.LineCheck(this.character.Center, this.currentTarget.Head, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null;
		if (Vector3.Angle(this.currentTarget.data.lookDirection, this.character.Center - this.currentTarget.Head) > 70f)
		{
			flag3 = false;
		}
		if (flag2 && flag3)
		{
			this.targetHasSeenMeCounter += Time.deltaTime * 1f;
			return;
		}
		if (flag3)
		{
			this.targetHasSeenMeCounter += Time.deltaTime * 0.3f;
			return;
		}
		if (flag2 && flag)
		{
			this.targetHasSeenMeCounter += Time.deltaTime * 0.15f;
			return;
		}
		this.targetHasSeenMeCounter = Mathf.MoveTowards(this.targetHasSeenMeCounter, 0f, Time.deltaTime * 0.1f);
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x0006D400 File Offset: 0x0006B600
	private bool CanSeeTarget(Character currentTarget)
	{
		return HelperFunctions.LineCheck(this.character.Head, currentTarget.Center + Random.insideUnitSphere * 0.5f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null;
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x0006D44C File Offset: 0x0006B64C
	private void DoVisuals()
	{
		float b = 0f;
		if (this.currentTarget)
		{
			this.currentTarget.data.myersDistance = Vector3.Distance(this.character.Center, this.currentTarget.Center);
		}
		if (this.currentTarget && this.currentTarget.IsLocal)
		{
			b = Mathf.InverseLerp(50f, 5f, this.distanceToTarget);
			this.mat.SetFloat(this.GRAINMULTID, (float)(GUIManager.instance.photosensitivity ? 0 : 1));
		}
		this.mat.SetFloat(this.STRENGTHID, Mathf.Lerp(this.mat.GetFloat(this.STRENGTHID), b, Time.deltaTime * 0.5f));
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x0006D51C File Offset: 0x0006B71C
	private void FixedUpdate()
	{
		if (this.animVars.reaching && this.character.data.grabbedPlayer == null && this.currentTarget)
		{
			Rigidbody bodypartRig = this.character.GetBodypartRig(BodypartType.Hand_R);
			Vector3 normalized = (this.currentTarget.Center - bodypartRig.transform.position).normalized;
			bodypartRig.AddForce(normalized * this.reachForce, ForceMode.Acceleration);
		}
	}

	// Token: 0x0600159A RID: 5530 RVA: 0x0006D5A0 File Offset: 0x0006B7A0
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.achievementDistance);
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x0006D5C4 File Offset: 0x0006B7C4
	private void Update()
	{
		if (!Player.localPlayer)
		{
			return;
		}
		this.UpdateAchievement();
		this.DoVisuals();
		if (!this.view.IsMine)
		{
			return;
		}
		this.tpCounter += Time.deltaTime;
		this.ResetInput();
		this.CalcVars();
		if (this.chillForSeconds > 0f)
		{
			this.chillForSeconds -= Time.deltaTime;
			return;
		}
		if (this.currentTarget == null)
		{
			this.EvasiveBehaviour();
			this.LookForTarget();
			return;
		}
		if (this.distanceToTarget > 80f)
		{
			this.TeleportCloseToTarget();
		}
		else
		{
			this.Chase();
		}
		this.VerifyTarget();
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x0006D672 File Offset: 0x0006B872
	private void UpdateAchievement()
	{
		this.achievementTestTick += Time.deltaTime;
		if (this.achievementTestTick > 1f)
		{
			this.achievementTestTick = 0f;
			this.TestAchievement();
		}
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x0006D6A4 File Offset: 0x0006B8A4
	private void TestAchievement()
	{
		if (Vector3.Distance(this.character.Center, Character.localCharacter.Center) <= this.achievementDistance)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.MentorshipBadge);
		}
	}

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x0600159E RID: 5534 RVA: 0x0006D6D4 File Offset: 0x0006B8D4
	private bool preventSpawning
	{
		get
		{
			if (RunSettings.IsCustomRun)
			{
				return RunSettings.GetValue(RunSettings.SETTINGTYPE.Hazard_Scoutmaster, false) == 0;
			}
			return Ascents.currentAscent < 0;
		}
	}

	// Token: 0x0600159F RID: 5535 RVA: 0x0006D6F4 File Offset: 0x0006B8F4
	private void VerifyTarget()
	{
		if (this.ViableTargets() < 2 || this.preventSpawning)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		Character closestOther = this.GetClosestOther(this.currentTarget);
		Character highestCharacter = this.GetHighestCharacter(null);
		Character highestCharacter2 = this.GetHighestCharacter(highestCharacter);
		if (highestCharacter.Center.y > this.maxAggroHeight)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		if (this.currentTarget != highestCharacter)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		if (highestCharacter.Center.y < highestCharacter2.Center.y + this.attackHeightDelta - 20f)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
		if (Vector3.Distance(closestOther.Center, this.currentTarget.Center) < 15f)
		{
			this.SetCurrentTarget(null, 0f);
			return;
		}
	}

	// Token: 0x060015A0 RID: 5536 RVA: 0x0006D7D4 File Offset: 0x0006B9D4
	private Character GetClosestOther(Character currentTarget)
	{
		List<Character> allCharacters = Character.AllCharacters;
		float num = float.MaxValue;
		Character result = null;
		foreach (Character character in allCharacters)
		{
			if (!character.isBot && !(character == currentTarget))
			{
				float num2 = Vector3.Distance(character.Center, currentTarget.Center);
				if (num2 < num)
				{
					num = num2;
					result = character;
				}
			}
		}
		return result;
	}

	// Token: 0x060015A1 RID: 5537 RVA: 0x0006D858 File Offset: 0x0006BA58
	private void EvasiveBehaviour()
	{
		if (!this.discovered)
		{
			this.discovered = this.GetPlayerWhoSeesMe();
		}
		if (this.discovered)
		{
			this.Flee();
			if (this.sinceAnyoneCanSeeMe > 0.5f)
			{
				this.TeleportFarAway();
			}
		}
	}

	// Token: 0x060015A2 RID: 5538 RVA: 0x0006D8A4 File Offset: 0x0006BAA4
	public void TeleportFarAway()
	{
		if (this.tpCounter < 5f)
		{
			return;
		}
		this.tpCounter = 0f;
		this.view.RPC("WarpPlayerRPC", RpcTarget.All, new object[]
		{
			new Vector3(0f, 0f, 5000f),
			false
		});
		this.view.RPC("StopClimbingRpc", RpcTarget.All, new object[]
		{
			0f
		});
		this.discovered = null;
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x0006D934 File Offset: 0x0006BB34
	private Character GetPlayerWhoSeesMe()
	{
		Vector3 vector = this.character.Center + Vector3.up * Random.Range(-0.5f, 0.5f);
		foreach (Character character in Character.AllCharacters)
		{
			if (!character.isBot && Vector3.Angle(vector - character.Head, character.data.lookDirection) <= 80f && HelperFunctions.LineCheck(character.Head, vector, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null)
			{
				return character;
			}
		}
		return null;
	}

	// Token: 0x060015A4 RID: 5540 RVA: 0x0006DA00 File Offset: 0x0006BC00
	private void Flee()
	{
		Vector3 normalized = (this.character.Center - this.discovered.Center).normalized;
		Vector3 targetPos = this.character.Center + normalized * 10f;
		if (this.character.data.isClimbing)
		{
			this.ClimbTowards(targetPos, 1f);
			return;
		}
		this.WalkTowards(targetPos, 1f);
		this.character.input.sprintIsPressed = true;
	}

	// Token: 0x060015A5 RID: 5541 RVA: 0x0006DA8C File Offset: 0x0006BC8C
	private bool AnyoneCanSeeMe()
	{
		Vector3 pos = this.character.Head + Vector3.up * 0.3f + Random.insideUnitSphere * 0.5f;
		Vector3 pos2 = this.character.HipPos() - Vector3.up * 0.3f + Random.insideUnitSphere * 0.5f;
		return this.AnyoneCanSeePos(pos) || this.AnyoneCanSeePos(pos2);
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x0006DB18 File Offset: 0x0006BD18
	private bool AnyoneCanSeePos(Vector3 pos)
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (!character.isBot && Vector3.Angle(pos - character.Head, character.data.lookDirection) <= 80f)
			{
				if (HelperFunctions.LineCheck(character.Head, pos, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null)
				{
					Debug.DrawLine(character.Head, pos, Color.blue);
					return true;
				}
				Debug.DrawLine(character.Head, pos, Color.red);
			}
		}
		return false;
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x0006DBE0 File Offset: 0x0006BDE0
	private void TeleportCloseToTarget()
	{
		this.Teleport(this.currentTarget, 50f, 70f, 15f);
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x0006DC00 File Offset: 0x0006BE00
	private void Teleport(Character target, float minDistanceToTarget = 35f, float maxDistanceToTarget = 45f, float maxHeightDifference = 15f)
	{
		if (this.tpCounter < 5f)
		{
			return;
		}
		this.tpCounter = 0f;
		Debug.Log("Trying to Teleport");
		if (target == null)
		{
			target = this.GetHighestCharacter(null);
		}
		Vector3 center = this.character.Center;
		int i = 50;
		while (i > 0)
		{
			i--;
			Vector3 onUnitSphere = Random.onUnitSphere;
			Vector3 vector = target.Center + Vector3.up * 500f + onUnitSphere * 95f;
			Vector3 a = Vector3.down;
			if (i < 25)
			{
				vector = target.Center + Vector3.up;
				a = Random.onUnitSphere;
			}
			RaycastHit raycastHit = HelperFunctions.LineCheck(vector, vector + a * 1000f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
			if (raycastHit.transform)
			{
				float num = Vector3.Distance(raycastHit.point, target.Center);
				float num2 = Mathf.Abs(raycastHit.point.y - target.Center.y);
				if (num < maxDistanceToTarget && num2 < maxHeightDifference && num > minDistanceToTarget && !this.AnyoneCanSeePos(raycastHit.point + Vector3.up))
				{
					Debug.Log("Teleporting");
					this.view.RPC("WarpPlayerRPC", RpcTarget.All, new object[]
					{
						raycastHit.point + Vector3.up,
						false
					});
					this.view.RPC("StopClimbingRpc", RpcTarget.All, new object[]
					{
						0f
					});
					this.discovered = null;
					return;
				}
			}
		}
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x0006DDBC File Offset: 0x0006BFBC
	private void Chase()
	{
		if (this.sinceSeenTarget > 30f && !this.AnyoneCanSeeMe())
		{
			this.sinceSeenTarget = 0f;
			this.TeleportCloseToTarget();
			if (Random.value < 0.1f)
			{
				this.currentTarget = null;
			}
			return;
		}
		if (this.character.data.isClimbing)
		{
			this.ClimbTowards(this.currentTarget.Head, 1f);
			if (this.currentTarget.Center.y < this.character.Center.y && !HelperFunctions.LineCheck(this.character.Center, this.currentTarget.Center, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform)
			{
				this.character.refs.climbing.StopClimbing();
				return;
			}
		}
		else
		{
			if (this.character.data.grabbedPlayer)
			{
				this.HoldPlayer();
				return;
			}
			this.LookAt(this.currentTarget.Head);
			float num = Vector3.Distance(this.character.Center, this.currentTarget.Center);
			if (num > 5f || this.targetHasSeenMeCounter > 1f)
			{
				this.WalkTowards(this.currentTarget.Head, 1f);
			}
			if (this.targetHasSeenMeCounter > 1f)
			{
				this.character.input.sprintIsPressed = (num < 15f);
				if (Vector3.Distance(this.character.Center, this.currentTarget.Center) < 3f && this.character.data.sinceClimb > 1f && this.character.data.isGrounded)
				{
					this.character.input.useSecondaryIsPressed = true;
				}
			}
		}
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x0006DF94 File Offset: 0x0006C194
	private void StandStill()
	{
	}

	// Token: 0x060015AB RID: 5547 RVA: 0x0006DF96 File Offset: 0x0006C196
	private void ResetInput()
	{
		this.character.input.ResetInput();
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x0006DFA8 File Offset: 0x0006C1A8
	private void HoldPlayer()
	{
		this.currentTarget.data.sinceGrounded = 0f;
		this.character.input.useSecondaryIsPressed = true;
		Vector3 lookDirection = this.character.data.lookDirection;
		lookDirection.y = 0.6f;
		lookDirection.Normalize();
		this.character.data.lookValues = HelperFunctions.DirectionToLook(lookDirection);
		if (!this.isThrowing)
		{
			this.view.RPC("RPCA_Throw", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x0006E038 File Offset: 0x0006C238
	[PunRPC]
	public void RPCA_Throw()
	{
		base.StartCoroutine(this.IThrow());
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x0006E047 File Offset: 0x0006C247
	private IEnumerator IThrow()
	{
		this.isThrowing = true;
		if (this.view.IsMine)
		{
			this.RotateToMostEvilThrowDirection();
		}
		if (this.currentTarget.IsLocal)
		{
			GamefeelHandler.instance.AddPerlinShake(15f, 0.5f, 15f);
		}
		GamefeelHandler.instance.AddPerlinShake(3f, 3f, 15f);
		float c = 0f;
		while (c < 3.2f)
		{
			this.currentTarget.data.lookValues = HelperFunctions.DirectionToLook(this.character.Head - this.currentTarget.Head);
			c += Time.deltaTime;
			yield return null;
		}
		Vector3 a = -this.character.data.lookDirection;
		a.y = 0f;
		a.Normalize();
		a.y = 0.3f;
		this.character.refs.grabbing.Throw(a * 1500f, 3f);
		this.currentTarget.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.25f, true, true, true);
		this.isThrowing = false;
		this.chillForSeconds = 2f;
		yield break;
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x0006E058 File Offset: 0x0006C258
	private void RotateToMostEvilThrowDirection()
	{
		Vector3[] circularDirections = HelperFunctions.GetCircularDirections(10);
		float d = 10f;
		float d2 = 1000f;
		Vector3 center = this.character.Center;
		Vector3 a = this.character.data.lookDirection_Flat;
		float num = 0f;
		foreach (Vector3 vector in circularDirections)
		{
			Vector3 vector2 = center + vector * d;
			if (!HelperFunctions.LineCheck(center, vector2, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform)
			{
				RaycastHit raycastHit = HelperFunctions.LineCheck(vector2, center + vector2 + Vector3.down * d2, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
				if (raycastHit.transform && raycastHit.distance > num)
				{
					a = vector;
					num = raycastHit.distance;
				}
			}
		}
		this.character.data.lookValues = HelperFunctions.DirectionToLook(-a);
	}

	// Token: 0x060015B0 RID: 5552 RVA: 0x0006E160 File Offset: 0x0006C360
	private void ClimbTowards(Vector3 targetPos, float mult)
	{
		this.LookAt(targetPos);
		float x = Mathf.Clamp(this.character.GetBodypart(BodypartType.Torso).transform.InverseTransformPoint(targetPos).x * 0.25f, -1f, 1f);
		this.character.input.movementInput = new Vector2(x, mult);
		this.character.data.currentStamina = 1f;
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x0006E1D4 File Offset: 0x0006C3D4
	private void WalkTowards(Vector3 targetPos, float mult)
	{
		this.LookAt(targetPos);
		float num = HelperFunctions.FlatDistance(this.character.Center, targetPos);
		if (Vector3.Distance(this.character.Center, targetPos) < 5f)
		{
			if (num < 2.5f)
			{
				mult *= 0f;
			}
			else if (num < 1.5f)
			{
				mult *= -1f;
			}
		}
		this.character.input.movementInput = new Vector2(0f, mult);
		this.character.refs.climbing.TryClimb(1.25f);
		if (HelperFunctions.LineCheck(this.character.Center, this.character.Center + Vector3.down * 3f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform == null)
		{
			this.character.input.jumpWasPressed = true;
		}
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x0006E2C2 File Offset: 0x0006C4C2
	private void LookAt(Vector3 lookAtPos)
	{
		this.character.data.lookValues = HelperFunctions.DirectionToLook(lookAtPos - this.character.Head);
	}

	// Token: 0x060015B3 RID: 5555 RVA: 0x0006E2F0 File Offset: 0x0006C4F0
	private int ViableTargets()
	{
		List<Character> allCharacters = Character.AllCharacters;
		int num = 0;
		foreach (Character character in allCharacters)
		{
			if (!character.isBot && !character.data.dead && !character.data.fullyPassedOut)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060015B4 RID: 5556 RVA: 0x0006E364 File Offset: 0x0006C564
	private void LookForTarget()
	{
		if (this.ViableTargets() < 2)
		{
			return;
		}
		if (this.sinceLookForTarget < 30f)
		{
			return;
		}
		this.sinceLookForTarget = 0f;
		if (Random.value > 0.1f)
		{
			return;
		}
		Character highestCharacter = this.GetHighestCharacter(null);
		Character highestCharacter2 = this.GetHighestCharacter(highestCharacter);
		if (highestCharacter.Center.y > highestCharacter2.Center.y + this.attackHeightDelta && highestCharacter.Center.y < this.maxAggroHeight)
		{
			this.SetCurrentTarget(highestCharacter, 0f);
		}
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x0006E3F0 File Offset: 0x0006C5F0
	private Character GetHighestCharacter(Character ignoredCharacter)
	{
		List<Character> allCharacters = Character.AllCharacters;
		Character character = null;
		foreach (Character character2 in allCharacters)
		{
			if (!character2.isBot && !character2.data.dead && !character2.data.fullyPassedOut && !(character2 == ignoredCharacter) && (character == null || character2.Center.y > character.Center.y))
			{
				character = character2;
			}
		}
		return character;
	}

	// Token: 0x040013AE RID: 5038
	public float reachForce;

	// Token: 0x040013AF RID: 5039
	private float targetForcedUntil;

	// Token: 0x040013B0 RID: 5040
	private Character _currentTarget;

	// Token: 0x040013B1 RID: 5041
	internal static List<Scoutmaster> AllScoutmasters = new List<Scoutmaster>();

	// Token: 0x040013B2 RID: 5042
	public Character discovered;

	// Token: 0x040013B3 RID: 5043
	private ScoutmasterAnimVars animVars;

	// Token: 0x040013B4 RID: 5044
	public float achievementDistance;

	// Token: 0x040013B5 RID: 5045
	private int STRENGTHID = Shader.PropertyToID("_Strength");

	// Token: 0x040013B6 RID: 5046
	private int GRAINMULTID = Shader.PropertyToID("_GrainMult");

	// Token: 0x040013B7 RID: 5047
	private Character character;

	// Token: 0x040013B8 RID: 5048
	private PhotonView view;

	// Token: 0x040013B9 RID: 5049
	public Material mat;

	// Token: 0x040013BA RID: 5050
	private float sinceLookForTarget;

	// Token: 0x040013BB RID: 5051
	private float distanceToTarget;

	// Token: 0x040013BC RID: 5052
	private float sinceAnyoneCanSeeMe = 10f;

	// Token: 0x040013BD RID: 5053
	private float achievementTestTick;

	// Token: 0x040013BE RID: 5054
	private float attackHeightDelta = 100f;

	// Token: 0x040013BF RID: 5055
	private float tpCounter;

	// Token: 0x040013C0 RID: 5056
	public float targetHasSeenMeCounter;

	// Token: 0x040013C1 RID: 5057
	private float sinceSeenTarget;

	// Token: 0x040013C2 RID: 5058
	private bool isThrowing;

	// Token: 0x040013C3 RID: 5059
	private float chillForSeconds;

	// Token: 0x040013C4 RID: 5060
	private float maxAggroHeight = 825f;
}
