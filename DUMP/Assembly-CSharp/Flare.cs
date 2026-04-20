using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000119 RID: 281
public class Flare : ItemComponent
{
	// Token: 0x0600093E RID: 2366 RVA: 0x00031F9D File Offset: 0x0003019D
	public override void Awake()
	{
		base.Awake();
		this.trackable = base.GetComponent<TrackableNetworkObject>();
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x00031FB1 File Offset: 0x000301B1
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.Color))
		{
			this.flareColor = base.GetData<ColorItemData>(DataEntryKey.Color).Value;
		}
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x00031FD0 File Offset: 0x000301D0
	private void Update()
	{
		bool value = base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value;
		this.item.UIData.canPocket = !value;
		this.item.UIData.canBackpack = !value;
		if (value && !this.trackable.hasTracker)
		{
			this.EnableFlareVisuals();
		}
		if (value && this.item.holderCharacter && Singleton<MountainProgressHandler>.Instance.IsAtPeak(this.item.holderCharacter.Center) && !Singleton<PeakHandler>.Instance.summonedHelicopter)
		{
			base.GetComponent<PhotonView>().RPC("TriggerHelicopter", RpcTarget.AllBuffered, Array.Empty<object>());
		}
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x0003207B File Offset: 0x0003027B
	[PunRPC]
	public void TriggerHelicopter()
	{
		Singleton<PeakHandler>.Instance.SummonHelicopter();
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x00032087 File Offset: 0x00030287
	public void LightFlare()
	{
		base.GetComponent<PhotonView>().RPC("SetFlareLitRPC", RpcTarget.AllBuffered, Array.Empty<object>());
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x000320A0 File Offset: 0x000302A0
	[PunRPC]
	public void SetFlareLitRPC()
	{
		if (this.item.holderCharacter)
		{
			this.flareColor = this.item.holderCharacter.refs.customization.PlayerColor;
			this.flareColor.a = 1f;
			base.GetData<ColorItemData>(DataEntryKey.Color).Value = this.flareColor;
			string str = "Set flare color to ";
			Color value = base.GetData<ColorItemData>(DataEntryKey.Color).Value;
			Debug.Log(str + value.ToString());
		}
		base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value = true;
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x0003213C File Offset: 0x0003033C
	public void EnableFlareVisuals()
	{
		Debug.Log(string.Format("Lighting flare with photon ID {0} with instance ID {1}", this.photonView.ViewID, this.trackable.instanceID));
		TrackNetworkedObject component = Object.Instantiate<TrackNetworkedObject>(this.flareVFXPrefab, base.transform.position, base.transform.rotation).GetComponent<TrackNetworkedObject>();
		component.SetObject(this.trackable);
		component.gameObject.GetComponent<ParticleSystem>().main.startColor = this.flareColor;
		string str = "Lit flare with color ";
		Color color = this.flareColor;
		Debug.Log(str + color.ToString());
	}

	// Token: 0x040008AF RID: 2223
	private TrackableNetworkObject trackable;

	// Token: 0x040008B0 RID: 2224
	public TrackNetworkedObject flareVFXPrefab;

	// Token: 0x040008B1 RID: 2225
	public Color flareColor;
}
