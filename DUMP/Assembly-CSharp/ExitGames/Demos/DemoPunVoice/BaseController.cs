using System;
using Photon.Pun;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000392 RID: 914
	[RequireComponent(typeof(PhotonView))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]
	public abstract class BaseController : MonoBehaviour
	{
		// Token: 0x0600180C RID: 6156 RVA: 0x0007A4C0 File Offset: 0x000786C0
		protected virtual void OnEnable()
		{
			ChangePOV.CameraChanged += this.ChangePOV_CameraChanged;
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x0007A4D4 File Offset: 0x000786D4
		protected virtual void OnDisable()
		{
			ChangePOV.CameraChanged -= this.ChangePOV_CameraChanged;
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x0007A4E8 File Offset: 0x000786E8
		protected virtual void ChangePOV_CameraChanged(Camera camera)
		{
			if (camera != this.ControllerCamera)
			{
				base.enabled = false;
				this.HideCamera(this.ControllerCamera);
				return;
			}
			this.ShowCamera(this.ControllerCamera);
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x0007A518 File Offset: 0x00078718
		protected virtual void Start()
		{
			if (base.GetComponent<PhotonView>().IsMine)
			{
				this.Init();
				this.SetCamera();
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x0007A53B File Offset: 0x0007873B
		protected virtual void Init()
		{
			this.rigidBody = base.GetComponent<Rigidbody>();
			this.animator = base.GetComponent<Animator>();
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x0007A555 File Offset: 0x00078755
		protected virtual void SetCamera()
		{
			this.camTrans = this.ControllerCamera.transform;
			this.camTrans.position += this.cameraDistance * base.transform.forward;
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x0007A594 File Offset: 0x00078794
		protected virtual void UpdateAnimator(float h, float v)
		{
			bool value = h != 0f || v != 0f;
			this.animator.SetBool("IsWalking", value);
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x0007A5CC File Offset: 0x000787CC
		protected virtual void FixedUpdate()
		{
			this.h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
			this.v = CrossPlatformInputManager.GetAxisRaw("Vertical");
			this.UpdateAnimator(this.h, this.v);
			this.Move(this.h, this.v);
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x0007A61D File Offset: 0x0007881D
		protected virtual void ShowCamera(Camera camera)
		{
			if (camera != null)
			{
				camera.gameObject.SetActive(true);
			}
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x0007A634 File Offset: 0x00078834
		protected virtual void HideCamera(Camera camera)
		{
			if (camera != null)
			{
				camera.gameObject.SetActive(false);
			}
		}

		// Token: 0x06001816 RID: 6166
		protected abstract void Move(float h, float v);

		// Token: 0x04001629 RID: 5673
		public Camera ControllerCamera;

		// Token: 0x0400162A RID: 5674
		protected Rigidbody rigidBody;

		// Token: 0x0400162B RID: 5675
		protected Animator animator;

		// Token: 0x0400162C RID: 5676
		protected Transform camTrans;

		// Token: 0x0400162D RID: 5677
		private float h;

		// Token: 0x0400162E RID: 5678
		private float v;

		// Token: 0x0400162F RID: 5679
		[SerializeField]
		protected float speed = 5f;

		// Token: 0x04001630 RID: 5680
		[SerializeField]
		private float cameraDistance;
	}
}
