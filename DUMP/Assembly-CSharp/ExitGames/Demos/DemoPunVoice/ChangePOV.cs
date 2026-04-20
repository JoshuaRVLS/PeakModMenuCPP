using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000394 RID: 916
	public class ChangePOV : MonoBehaviour, IMatchmakingCallbacks
	{
		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600181E RID: 6174 RVA: 0x0007A71C File Offset: 0x0007891C
		// (remove) Token: 0x0600181F RID: 6175 RVA: 0x0007A750 File Offset: 0x00078950
		public static event ChangePOV.OnCameraChanged CameraChanged;

		// Token: 0x06001820 RID: 6176 RVA: 0x0007A783 File Offset: 0x00078983
		private void OnEnable()
		{
			CharacterInstantiation.CharacterInstantiated += this.OnCharacterInstantiated;
			PhotonNetwork.AddCallbackTarget(this);
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x0007A79C File Offset: 0x0007899C
		private void OnDisable()
		{
			CharacterInstantiation.CharacterInstantiated -= this.OnCharacterInstantiated;
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x0007A7B8 File Offset: 0x000789B8
		private void Start()
		{
			this.defaultCamera = Camera.main;
			this.initialCameraPosition = new Vector3(this.defaultCamera.transform.position.x, this.defaultCamera.transform.position.y, this.defaultCamera.transform.position.z);
			this.initialCameraRotation = new Quaternion(this.defaultCamera.transform.rotation.x, this.defaultCamera.transform.rotation.y, this.defaultCamera.transform.rotation.z, this.defaultCamera.transform.rotation.w);
			this.FirstPersonCamActivator.onClick.AddListener(new UnityAction(this.FirstPersonMode));
			this.ThirdPersonCamActivator.onClick.AddListener(new UnityAction(this.ThirdPersonMode));
			this.OrthographicCamActivator.onClick.AddListener(new UnityAction(this.OrthographicMode));
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x0007A8D0 File Offset: 0x00078AD0
		private void OnCharacterInstantiated(GameObject character)
		{
			this.firstPersonController = character.GetComponent<FirstPersonController>();
			this.firstPersonController.enabled = false;
			this.thirdPersonController = character.GetComponent<ThirdPersonController>();
			this.thirdPersonController.enabled = false;
			this.orthographicController = character.GetComponent<OrthographicController>();
			this.ButtonsHolder.SetActive(true);
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x0007A925 File Offset: 0x00078B25
		private void FirstPersonMode()
		{
			this.ToggleMode(this.firstPersonController);
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x0007A933 File Offset: 0x00078B33
		private void ThirdPersonMode()
		{
			this.ToggleMode(this.thirdPersonController);
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x0007A941 File Offset: 0x00078B41
		private void OrthographicMode()
		{
			this.ToggleMode(this.orthographicController);
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x0007A950 File Offset: 0x00078B50
		private void ToggleMode(BaseController controller)
		{
			if (controller == null)
			{
				return;
			}
			if (controller.ControllerCamera == null)
			{
				return;
			}
			controller.ControllerCamera.gameObject.SetActive(true);
			controller.enabled = true;
			this.FirstPersonCamActivator.interactable = !(controller == this.firstPersonController);
			this.ThirdPersonCamActivator.interactable = !(controller == this.thirdPersonController);
			this.OrthographicCamActivator.interactable = !(controller == this.orthographicController);
			this.BroadcastChange(controller.ControllerCamera);
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x0007A9E8 File Offset: 0x00078BE8
		private void BroadcastChange(Camera camera)
		{
			if (camera == null)
			{
				return;
			}
			if (ChangePOV.CameraChanged != null)
			{
				ChangePOV.CameraChanged(camera);
			}
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x0007AA06 File Offset: 0x00078C06
		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x0007AA08 File Offset: 0x00078C08
		public void OnCreatedRoom()
		{
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x0007AA0A File Offset: 0x00078C0A
		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x0007AA0C File Offset: 0x00078C0C
		public void OnJoinedRoom()
		{
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x0007AA0E File Offset: 0x00078C0E
		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x0007AA10 File Offset: 0x00078C10
		public void OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x0007AA14 File Offset: 0x00078C14
		public void OnLeftRoom()
		{
			if (this.defaultCamera)
			{
				this.defaultCamera.gameObject.SetActive(true);
			}
			this.FirstPersonCamActivator.interactable = true;
			this.ThirdPersonCamActivator.interactable = true;
			this.OrthographicCamActivator.interactable = false;
			this.defaultCamera.transform.position = this.initialCameraPosition;
			this.defaultCamera.transform.rotation = this.initialCameraRotation;
			this.ButtonsHolder.SetActive(false);
		}

		// Token: 0x04001633 RID: 5683
		private FirstPersonController firstPersonController;

		// Token: 0x04001634 RID: 5684
		private ThirdPersonController thirdPersonController;

		// Token: 0x04001635 RID: 5685
		private OrthographicController orthographicController;

		// Token: 0x04001636 RID: 5686
		private Vector3 initialCameraPosition;

		// Token: 0x04001637 RID: 5687
		private Quaternion initialCameraRotation;

		// Token: 0x04001638 RID: 5688
		private Camera defaultCamera;

		// Token: 0x04001639 RID: 5689
		[SerializeField]
		private GameObject ButtonsHolder;

		// Token: 0x0400163A RID: 5690
		[SerializeField]
		private Button FirstPersonCamActivator;

		// Token: 0x0400163B RID: 5691
		[SerializeField]
		private Button ThirdPersonCamActivator;

		// Token: 0x0400163C RID: 5692
		[SerializeField]
		private Button OrthographicCamActivator;

		// Token: 0x02000559 RID: 1369
		// (Invoke) Token: 0x06001FA6 RID: 8102
		public delegate void OnCameraChanged(Camera newCamera);
	}
}
