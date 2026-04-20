using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000FA RID: 250
public class Action_ShowBinocularOverlay : ItemAction
{
	// Token: 0x060008AA RID: 2218 RVA: 0x0002FC23 File Offset: 0x0002DE23
	private void Update()
	{
		if (this.binocularsActive && !this.isProp)
		{
			this.TestLookAtSun();
		}
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x0002FC3C File Offset: 0x0002DE3C
	public override void RunAction()
	{
		this.binocularsActive = !this.binocularsActive;
		if (!this.isProp)
		{
			this.featureManager.setFeatureActive(this.binocularsActive);
			MainCamera.instance.SetCameraOverride(this.binocularsActive ? this.cameraOverride : null);
		}
		this.item.photonView.RPC("ToggleUseRPC", RpcTarget.All, new object[]
		{
			this.binocularsActive
		});
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0002FCB6 File Offset: 0x0002DEB6
	[PunRPC]
	private void ToggleUseRPC(bool open)
	{
		this.item.defaultPos = new Vector3(this.item.defaultPos.x, (float)(open ? 1 : 0), this.item.defaultPos.z);
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0002FCF0 File Offset: 0x0002DEF0
	private void TestLookAtSun()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (Singleton<MapHandler>.Instance.GetCurrentBiome() != Biome.BiomeType.Mesa)
		{
			return;
		}
		if (DayNightManager.instance.sun.intensity < 5f)
		{
			return;
		}
		Transform transform = DayNightManager.instance.sun.transform;
		if (Vector3.Angle(MainCamera.instance.transform.forward, -transform.forward) > 10f)
		{
			return;
		}
		RaycastHit raycastHit = HelperFunctions.LineCheck(Character.localCharacter.Center + transform.forward * -1000f, Character.localCharacter.Center, HelperFunctions.LayerType.AllPhysical, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform == null || raycastHit.transform.root == Character.localCharacter.transform.root)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.AstronomyBadge);
		}
	}

	// Token: 0x04000840 RID: 2112
	public bool binocularsActive;

	// Token: 0x04000841 RID: 2113
	public CameraOverride cameraOverride;

	// Token: 0x04000842 RID: 2114
	public ItemRenderFeatureManager featureManager;

	// Token: 0x04000843 RID: 2115
	public bool isProp;
}
