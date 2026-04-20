using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000225 RID: 549
public class BugleSFX : MonoBehaviourPun
{
	// Token: 0x060010D5 RID: 4309 RVA: 0x00053A54 File Offset: 0x00051C54
	private void Start()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x00053A64 File Offset: 0x00051C64
	private void UpdateTooting()
	{
		if (base.photonView.IsMine)
		{
			bool flag = this.item.isUsingPrimary;
			if (this.magicBugle && this.magicBugle.currentFuel <= 0.02f)
			{
				flag = false;
			}
			if (flag != this.hold)
			{
				if (flag)
				{
					int num = Random.Range(0, this.bugle.Length);
					float num2 = Vector3.Dot(this.item.holderCharacter.data.lookDirection, Vector3.up);
					num2 = (num2 + 1f) / 2f;
					base.photonView.RPC("RPC_StartToot", RpcTarget.All, new object[]
					{
						num,
						num2
					});
				}
				else
				{
					base.photonView.RPC("RPC_EndToot", RpcTarget.All, Array.Empty<object>());
				}
				this.hold = flag;
			}
		}
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x00053B44 File Offset: 0x00051D44
	[PunRPC]
	private void RPC_StartToot(int clip, float pitch)
	{
		this.currentPitch = pitch;
		this.currentClip = clip;
		this.hold = true;
		if (this.particle1 && this.particle2)
		{
			if (!this.particle1.isPlaying)
			{
				this.particle1.Play();
			}
			if (!this.particle2.isPlaying)
			{
				this.particle2.Play();
			}
			ParticleSystem.EmissionModule emission = this.particle1.emission;
			ParticleSystem.EmissionModule emission2 = this.particle2.emission;
			emission.enabled = true;
			emission2.enabled = true;
		}
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x00053BD8 File Offset: 0x00051DD8
	[PunRPC]
	private void RPC_EndToot()
	{
		this.hold = false;
		if (this.particle1 && this.particle2)
		{
			ParticleSystem.EmissionModule emission = this.particle1.emission;
			ParticleSystem.EmissionModule emission2 = this.particle2.emission;
			emission.enabled = false;
			emission2.enabled = false;
		}
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x00053C30 File Offset: 0x00051E30
	private void Update()
	{
		this.UpdateTooting();
		if (this.hold && !this.t && !this.isProp)
		{
			this.buglePlayer.clip = this.bugle[this.currentClip];
			this.buglePlayer.Play();
			this.buglePlayer.volume = 0f;
			this.t = true;
		}
		this.item.defaultPos = new Vector3(this.item.defaultPos.x, this.hold ? 0.5f : 0f, this.item.defaultPos.z);
		if (this.hold)
		{
			this.buglePlayer.volume = Mathf.Lerp(this.buglePlayer.volume, this.volume, 10f * Time.deltaTime);
			float value = this.currentPitch * (1f + Mathf.Sin(Time.time * (1f + this.pitchWobble)) * this.pitchWobble);
			value = Mathf.Clamp(value, 0.01f, 0.99f);
			this.buglePlayer.pitch = Mathf.Lerp(this.pitchMin, this.pitchMax, value);
		}
		if (!this.hold)
		{
			this.buglePlayer.volume = Mathf.Lerp(this.buglePlayer.volume, 0f, 10f * Time.deltaTime);
		}
		if (!this.hold && this.t)
		{
			this.t = false;
		}
	}

	// Token: 0x04000ED1 RID: 3793
	private Item item;

	// Token: 0x04000ED2 RID: 3794
	public bool hold;

	// Token: 0x04000ED3 RID: 3795
	private bool t;

	// Token: 0x04000ED4 RID: 3796
	private int currentClip;

	// Token: 0x04000ED5 RID: 3797
	public AudioClip[] bugle;

	// Token: 0x04000ED6 RID: 3798
	public AudioSource buglePlayer;

	// Token: 0x04000ED7 RID: 3799
	public AudioSource bugleEnd;

	// Token: 0x04000ED8 RID: 3800
	public MagicBugle magicBugle;

	// Token: 0x04000ED9 RID: 3801
	public ParticleSystem particle1;

	// Token: 0x04000EDA RID: 3802
	public ParticleSystem particle2;

	// Token: 0x04000EDB RID: 3803
	public float currentPitch;

	// Token: 0x04000EDC RID: 3804
	public float pitchMin = 0.7f;

	// Token: 0x04000EDD RID: 3805
	public float pitchMax = 1.3f;

	// Token: 0x04000EDE RID: 3806
	public float volume = 0.35f;

	// Token: 0x04000EDF RID: 3807
	public float pitchWobble;

	// Token: 0x04000EE0 RID: 3808
	public bool isProp;
}
