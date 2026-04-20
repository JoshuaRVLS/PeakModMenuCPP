using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000029 RID: 41
[DefaultExecutionOrder(500)]
public class MainCameraMovement : Singleton<MainCameraMovement>
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060002F4 RID: 756 RVA: 0x00014A16 File Offset: 0x00012C16
	// (set) Token: 0x060002F5 RID: 757 RVA: 0x00014A1D File Offset: 0x00012C1D
	public static Character specCharacter { get; protected set; }

	// Token: 0x060002F6 RID: 758 RVA: 0x00014A28 File Offset: 0x00012C28
	private void Start()
	{
		this.cam = base.GetComponent<MainCamera>();
		this.currentFov = this.cam.cam.fieldOfView;
		this.fovSetting = GameHandler.Instance.SettingsHandler.GetSetting<FovSetting>();
		this.extraFovSetting = GameHandler.Instance.SettingsHandler.GetSetting<ExtraFovSetting>();
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060002F7 RID: 759 RVA: 0x00014A81 File Offset: 0x00012C81
	public static bool IsSpectating
	{
		get
		{
			return Singleton<MainCameraMovement>.Instance.isSpectating;
		}
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x00014A90 File Offset: 0x00012C90
	private void LateUpdate()
	{
		if (this.isGodCam)
		{
			this.godcam.Update(base.transform, this.cam);
			return;
		}
		this.UpdateVariables();
		if (this.cam.camOverride)
		{
			this.OverrideCam();
			return;
		}
		if (Character.localCharacter && Character.localCharacter.data.fullyPassedOut)
		{
			this.Spectate();
			if (!this.isSpectating)
			{
				this.StartSpectate();
			}
			return;
		}
		if (this.isSpectating)
		{
			this.StopSpectating();
		}
		MainCameraMovement.specCharacter = null;
		this.CharacterCam();
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x00014B28 File Offset: 0x00012D28
	private void StartSpectate()
	{
		this.isSpectating = true;
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00014B31 File Offset: 0x00012D31
	private void StopSpectating()
	{
		this.isSpectating = false;
		MainCameraMovement.specCharacter = null;
		if (Character.localCharacter.Ghost != null)
		{
			PhotonNetwork.Destroy(Character.localCharacter.Ghost.gameObject);
		}
	}

	// Token: 0x060002FB RID: 763 RVA: 0x00014B66 File Offset: 0x00012D66
	private void UpdateVariables()
	{
		this.sinceSwitch += Time.deltaTime;
	}

	// Token: 0x060002FC RID: 764 RVA: 0x00014B7C File Offset: 0x00012D7C
	private void Spectate()
	{
		if (!this.HandleSpecSelection())
		{
			return;
		}
		PlayerGhost playerGhost = Character.localCharacter.Ghost;
		if (playerGhost == null && Character.localCharacter.data.dead)
		{
			playerGhost = PhotonNetwork.Instantiate("PlayerGhost", Vector3.zero, Quaternion.identity, 0, null).GetComponent<PlayerGhost>();
			playerGhost.m_view.RPC("RPCA_InitGhost", RpcTarget.AllBuffered, new object[]
			{
				Character.localCharacter.refs.view,
				MainCameraMovement.specCharacter.refs.view
			});
		}
		if (playerGhost && playerGhost.m_target != MainCameraMovement.specCharacter)
		{
			playerGhost.m_view.RPC("RPCA_SetTarget", RpcTarget.AllBuffered, new object[]
			{
				MainCameraMovement.specCharacter.refs.view
			});
		}
		base.transform.position = MainCameraMovement.specCharacter.GetSpectatePosition();
		Vector3 lookDirection = MainCameraMovement.specCharacter.data.lookDirection;
		if (Character.localCharacter != null)
		{
			lookDirection = Character.localCharacter.data.lookDirection;
		}
		base.transform.rotation = Quaternion.LookRotation(lookDirection);
		this.spectateZoom += Character.localCharacter.input.scrollInput * -0.5f;
		if (Character.localCharacter.input.scrollForwardIsPressed)
		{
			this.spectateZoom -= this.spectateZoomButtonSpeed * Time.deltaTime;
		}
		else if (Character.localCharacter.input.scrollBackwardIsPressed)
		{
			this.spectateZoom += this.spectateZoomButtonSpeed * Time.deltaTime;
		}
		this.spectateZoom = Mathf.Clamp(this.spectateZoom, this.spectateZoomMin, this.spectateZoomMax);
		Character.localCharacter.data.spectateZoom = Mathf.Lerp(Character.localCharacter.data.spectateZoom, this.spectateZoom, Time.deltaTime * 5f);
		base.transform.position += base.transform.TransformDirection(new Vector3(0f, 0.5f, -1f * Character.localCharacter.data.spectateZoom));
	}

	// Token: 0x060002FD RID: 765 RVA: 0x00014DB4 File Offset: 0x00012FB4
	private bool HandleSpecSelection()
	{
		if (MainCameraMovement.specCharacter && !MainCameraMovement.specCharacter.data.canBeSpectated)
		{
			MainCameraMovement.specCharacter = null;
		}
		if (MainCameraMovement.specCharacter == null)
		{
			this.GetSpecPlayer();
		}
		if (MainCameraMovement.specCharacter == null)
		{
			return false;
		}
		if (MainCameraMovement.specCharacter == Character.localCharacter)
		{
			return true;
		}
		if (Character.localCharacter.input.spectateLeftWasPressed && this.sinceSwitch > 0.2f)
		{
			Transitions.instance.PlayTransition(TransitionType.SpectateSwitch, new Action(this.SwapSpecPlayerLeft), 5f, 5f);
			this.sinceSwitch = 0f;
		}
		if (Character.localCharacter.input.spectateRightWasPressed && this.sinceSwitch > 0.2f)
		{
			Transitions.instance.PlayTransition(TransitionType.SpectateSwitch, new Action(this.SwapSpecPlayerRight), 5f, 5f);
			this.sinceSwitch = 0f;
		}
		return !(MainCameraMovement.specCharacter == null);
	}

	// Token: 0x060002FE RID: 766 RVA: 0x00014EB8 File Offset: 0x000130B8
	public void SwapSpecPlayerLeft()
	{
		this.SwapSpecPlayer(-1);
	}

	// Token: 0x060002FF RID: 767 RVA: 0x00014EC1 File Offset: 0x000130C1
	public void SwapSpecPlayerRight()
	{
		this.SwapSpecPlayer(1);
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00014ECC File Offset: 0x000130CC
	private void SwapSpecPlayer(int add)
	{
		List<Character> list = new List<Character>();
		foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
		{
			if (character.data.canBeSpectated && !character.isBot)
			{
				list.Add(character);
			}
		}
		if (list.Count == 0)
		{
			MainCameraMovement.specCharacter = null;
			return;
		}
		if (MainCameraMovement.specCharacter == null)
		{
			Debug.LogError("WE FOUND IT");
			return;
		}
		int num = MainCameraMovement.specCharacter.GetPlayerListID(list);
		num += add;
		if (num < 0)
		{
			num = list.Count - 1;
		}
		if (num >= list.Count)
		{
			num = 0;
		}
		MainCameraMovement.specCharacter = list[num];
	}

	// Token: 0x06000301 RID: 769 RVA: 0x00014F94 File Offset: 0x00013194
	private void GetSpecPlayer()
	{
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		if (allPlayerCharacters.Count == 0)
		{
			return;
		}
		if (Character.localCharacter.data.canBeSpectated)
		{
			MainCameraMovement.specCharacter = Character.localCharacter;
			return;
		}
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			if (allPlayerCharacters[i].data.canBeSpectated && !allPlayerCharacters[i].isBot)
			{
				MainCameraMovement.specCharacter = allPlayerCharacters[i];
				return;
			}
		}
	}

	// Token: 0x06000302 RID: 770 RVA: 0x0001500C File Offset: 0x0001320C
	private void CharacterCam()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.cam.cam.fieldOfView = this.GetFov();
		if (Character.localCharacter == null)
		{
			return;
		}
		if (Character.localCharacter == null)
		{
			return;
		}
		if (Character.localCharacter.data.lookDirection != Vector3.zero)
		{
			base.transform.rotation = Quaternion.LookRotation(Character.localCharacter.data.lookDirection);
			float num = 1f - Character.localCharacter.data.currentRagdollControll;
			if (num > this.ragdollCam)
			{
				this.ragdollCam = Mathf.Lerp(this.ragdollCam, num, Time.deltaTime * 5f);
			}
			else
			{
				this.ragdollCam = Mathf.Lerp(this.ragdollCam, num, Time.deltaTime * 0.5f);
			}
			this.physicsRot = Quaternion.Lerp(this.physicsRot, Character.localCharacter.GetBodypartRig(BodypartType.Head).transform.rotation, Time.deltaTime * 10f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.physicsRot, this.ragdollCam);
			base.transform.Rotate(GamefeelHandler.instance.GetRotation(), Space.World);
		}
		Vector3 cameraPos = Character.localCharacter.GetCameraPos(this.GetHeadOffset());
		Vector3 position = Character.localCharacter.GetBodypart(BodypartType.Torso).transform.position;
		this.targetPlayerPovPosition = Vector3.Lerp(cameraPos, position, this.ragdollCam);
		if (Vector3.Distance(base.transform.position, this.targetPlayerPovPosition) > this.characterPovMaxDistance)
		{
			base.transform.position = this.targetPlayerPovPosition + (base.transform.position - this.targetPlayerPovPosition).normalized * this.characterPovMaxDistance;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPlayerPovPosition, Time.deltaTime * this.characterPovLerpRate);
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00015224 File Offset: 0x00013424
	private void OverrideCam()
	{
		this.cam.cam.fieldOfView = this.cam.camOverride.fov;
		this.cam.transform.position = this.cam.camOverride.transform.position;
		this.cam.transform.rotation = this.cam.camOverride.transform.rotation;
	}

	// Token: 0x06000304 RID: 772 RVA: 0x0001529C File Offset: 0x0001349C
	private float GetHeadOffset()
	{
		if (Character.localCharacter.data.isClimbing)
		{
			this.currentForwardOffset = Mathf.Lerp(this.currentForwardOffset, -0.5f, Time.deltaTime * 5f);
		}
		else
		{
			this.currentForwardOffset = Mathf.Lerp(this.currentForwardOffset, -0.5f, Time.deltaTime * 5f);
		}
		return this.currentForwardOffset;
	}

	// Token: 0x06000305 RID: 773 RVA: 0x00015304 File Offset: 0x00013504
	private float GetFov()
	{
		float num = this.fovSetting.Value;
		if (num < 60f)
		{
			num = 70f;
		}
		if (Character.localCharacter == null)
		{
			return num;
		}
		this.currentFov = Mathf.Lerp(this.currentFov, num + (Character.localCharacter.data.isClimbing ? this.extraFovSetting.Value : 0f), Time.deltaTime * 5f);
		return this.currentFov;
	}

	// Token: 0x040002B7 RID: 695
	private float currentFov;

	// Token: 0x040002B8 RID: 696
	private float currentForwardOffset = 0.5f;

	// Token: 0x040002B9 RID: 697
	private MainCamera cam;

	// Token: 0x040002BA RID: 698
	private FovSetting fovSetting;

	// Token: 0x040002BB RID: 699
	private ExtraFovSetting extraFovSetting;

	// Token: 0x040002BD RID: 701
	public float characterPovLerpRate = 5f;

	// Token: 0x040002BE RID: 702
	public float characterPovMaxDistance = 0.1f;

	// Token: 0x040002BF RID: 703
	private bool isSpectating;

	// Token: 0x040002C0 RID: 704
	internal bool isGodCam;

	// Token: 0x040002C1 RID: 705
	public GodCam godcam;

	// Token: 0x040002C2 RID: 706
	private float spectateZoom = 2f;

	// Token: 0x040002C3 RID: 707
	public float spectateZoomMin = 1f;

	// Token: 0x040002C4 RID: 708
	public float spectateZoomMax = 5f;

	// Token: 0x040002C5 RID: 709
	public float spectateZoomButtonSpeed = 30f;

	// Token: 0x040002C6 RID: 710
	private float sinceSwitch;

	// Token: 0x040002C7 RID: 711
	private float ragdollCam;

	// Token: 0x040002C8 RID: 712
	private Quaternion physicsRot;

	// Token: 0x040002C9 RID: 713
	private Vector3 targetPlayerPovPosition;
}
