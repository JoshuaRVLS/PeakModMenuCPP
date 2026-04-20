using System;
using Photon.Pun;
using Sirenix.Utilities;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002D1 RID: 721
public class PlayerGhost : MonoBehaviour
{
	// Token: 0x0600143D RID: 5181 RVA: 0x000669AB File Offset: 0x00064BAB
	private void Awake()
	{
		this.m_view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x000669BC File Offset: 0x00064BBC
	[PunRPC]
	public void RPCA_InitGhost(PhotonView character, PhotonView t)
	{
		this.m_owner = character.GetComponent<Character>();
		this.m_owner.Ghost = this;
		this.RPCA_SetTarget(t);
		PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(this.m_owner.photonView.Owner);
		this.animatedMouth.audioSource = character.GetComponent<AnimatedMouth>().audioSource;
		this.CustomizeGhost(playerData);
		if (character.IsMine)
		{
			this.PlayerRenderers.ForEach(delegate(Renderer r)
			{
				r.enabled = false;
			});
			this.EyeRenderers.ForEach(delegate(Renderer r)
			{
				r.enabled = false;
			});
			this.mouthRenderer.enabled = false;
			this.accessoryRenderer.enabled = false;
			this.thirdEye.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x00066AA8 File Offset: 0x00064CA8
	private void CustomizeGhost(PersistentPlayerData data)
	{
		int skinIndex = CharacterCustomization.GetSkinIndex(data);
		for (int i = 0; i < this.PlayerRenderers.Length; i++)
		{
			this.PlayerRenderers[i].material.SetColor("_PlayerColor", Singleton<Customization>.Instance.skins[skinIndex].color);
		}
		for (int j = 0; j < this.EyeRenderers.Length; j++)
		{
			this.EyeRenderers[j].material.SetColor(PlayerGhost.SkinColor, Singleton<Customization>.Instance.skins[skinIndex].color);
		}
		int eyesIndex = CharacterCustomization.GetEyesIndex(data);
		for (int k = 0; k < this.EyeRenderers.Length; k++)
		{
			this.EyeRenderers[k].material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.eyes[eyesIndex].texture);
		}
		int accessoryIndex = CharacterCustomization.GetAccessoryIndex(data);
		this.accessoryRenderer.material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.accessories[accessoryIndex].texture);
		this.accessoryRenderer.gameObject.SetActive(accessoryIndex != 20);
		this.thirdEye.gameObject.SetActive(accessoryIndex == 20);
		int mouthIndex = CharacterCustomization.GetMouthIndex(data);
		this.mouthRenderer.material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.mouths[mouthIndex].texture);
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x00066C0A File Offset: 0x00064E0A
	[PunRPC]
	public void RPCA_SetTarget(PhotonView t)
	{
		this.m_target = t.GetComponent<Character>();
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x00066C18 File Offset: 0x00064E18
	private void Update()
	{
		if (!this.m_target)
		{
			return;
		}
		Vector3 vector = this.m_target.Center;
		base.transform.rotation = Quaternion.LookRotation(this.m_owner.data.lookDirection);
		vector += base.transform.forward * -1f * this.m_owner.data.spectateZoom;
		vector += base.transform.up * 0.5f;
		base.transform.position = Vector3.Lerp(base.transform.position, vector, Time.deltaTime * 3f);
		base.transform.rotation = Quaternion.LookRotation(MainCamera.instance.cam.transform.position - base.transform.position);
	}

	// Token: 0x04001270 RID: 4720
	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	// Token: 0x04001271 RID: 4721
	private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");

	// Token: 0x04001272 RID: 4722
	public Character m_target;

	// Token: 0x04001273 RID: 4723
	public Character m_owner;

	// Token: 0x04001274 RID: 4724
	public PhotonView m_view;

	// Token: 0x04001275 RID: 4725
	[Header("Customization Refrences")]
	public Renderer[] PlayerRenderers;

	// Token: 0x04001276 RID: 4726
	public Renderer[] EyeRenderers;

	// Token: 0x04001277 RID: 4727
	public Renderer mouthRenderer;

	// Token: 0x04001278 RID: 4728
	public Renderer accessoryRenderer;

	// Token: 0x04001279 RID: 4729
	public AnimatedMouth animatedMouth;

	// Token: 0x0400127A RID: 4730
	public GameObject thirdEye;
}
