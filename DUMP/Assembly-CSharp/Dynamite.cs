using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Zorro.Settings;

// Token: 0x02000118 RID: 280
public class Dynamite : ItemComponent
{
	// Token: 0x06000932 RID: 2354 RVA: 0x00031BF6 File Offset: 0x0002FDF6
	public override void Awake()
	{
		base.Awake();
		this.trackable = base.GetComponent<TrackableNetworkObject>();
		this.item.UIData.canPocket = false;
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x00031C1B File Offset: 0x0002FE1B
	private void Start()
	{
		this.setting = GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>();
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00031C32 File Offset: 0x0002FE32
	public override void OnInstanceDataSet()
	{
		this.fuseTime = base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value;
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x00031C54 File Offset: 0x0002FE54
	private void Update()
	{
		if (!base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value)
		{
			this.TestLightWick();
			return;
		}
		if (!this.trackable.hasTracker)
		{
			this.EnableFlareVisuals();
		}
		if (this.setting.Value != OffOnMode.ON)
		{
			this.sparks.gameObject.SetActive(true);
		}
		else
		{
			this.sparksPhotosensitive.gameObject.SetActive(true);
		}
		this.fuseTime = base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value;
		this.item.SetUseRemainingPercentage(this.fuseTime / this.startingFuseTime);
		if (this.photonView.IsMine)
		{
			this.fuseTime -= Time.deltaTime;
			if (this.fuseTime <= 0f)
			{
				if (this._hasExploded)
				{
					Debug.LogError("Attempting to explode an already exploded object!");
				}
				if (Character.localCharacter.data.currentItem == this.item)
				{
					Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.25f, false, true, true);
					Player.localPlayer.EmptySlot(Character.localCharacter.refs.items.currentSelectedSlot);
					Character.localCharacter.refs.afflictions.UpdateWeight();
				}
				this.photonView.RPC("RPC_Explode", RpcTarget.All, Array.Empty<object>());
				Debug.Log("<color=Red>Exploded</color>");
				PhotonNetwork.Destroy(base.gameObject);
				this.item.ClearDataFromBackpack();
				this.fuseTime = 0f;
			}
			base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value = this.fuseTime;
		}
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00031E04 File Offset: 0x00030004
	[PunRPC]
	private void RPC_Explode()
	{
		if (this.DEBUG_PauseOnExplode)
		{
			Debug.Break();
		}
		Object.Instantiate<GameObject>(this.explosionPrefab, base.transform.position, base.transform.rotation);
		base.gameObject.SetActive(false);
		this._hasExploded = true;
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00031E54 File Offset: 0x00030054
	private void TestLightWick()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value)
		{
			return;
		}
		using (List<Character>.Enumerator enumerator = Character.AllCharacters.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.Center, base.transform.position) < this.lightFuseRadius)
				{
					this.LightFlare();
				}
			}
		}
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00031ED8 File Offset: 0x000300D8
	private FloatItemData SetupDefaultFuel()
	{
		return new FloatItemData
		{
			Value = this.startingFuseTime
		};
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00031EEB File Offset: 0x000300EB
	[PunRPC]
	public void TriggerHelicopter()
	{
		Singleton<PeakHandler>.Instance.SummonHelicopter();
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x00031EF7 File Offset: 0x000300F7
	public void LightFlare()
	{
		Debug.Log("Lighting dynamite");
		base.GetComponent<PhotonView>().RPC("SetFlareLitRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00031F19 File Offset: 0x00030119
	[PunRPC]
	public void SetFlareLitRPC()
	{
		base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value = true;
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x00031F28 File Offset: 0x00030128
	public void EnableFlareVisuals()
	{
		Debug.Log(string.Format("Lighting flare with photon ID {0} with instance ID {1}", this.photonView.ViewID, this.trackable.instanceID));
		Object.Instantiate<TrackNetworkedObject>(this.smokeVFXPrefab, base.transform.position, base.transform.rotation).GetComponent<TrackNetworkedObject>().SetObject(this.trackable);
	}

	// Token: 0x040008A4 RID: 2212
	private bool _hasExploded;

	// Token: 0x040008A5 RID: 2213
	private TrackableNetworkObject trackable;

	// Token: 0x040008A6 RID: 2214
	public TrackNetworkedObject smokeVFXPrefab;

	// Token: 0x040008A7 RID: 2215
	public GameObject explosionPrefab;

	// Token: 0x040008A8 RID: 2216
	public float startingFuseTime;

	// Token: 0x040008A9 RID: 2217
	public float lightFuseRadius;

	// Token: 0x040008AA RID: 2218
	[SerializeField]
	private float fuseTime;

	// Token: 0x040008AB RID: 2219
	public Transform sparks;

	// Token: 0x040008AC RID: 2220
	public Transform sparksPhotosensitive;

	// Token: 0x040008AD RID: 2221
	private PhotosensitiveSetting setting;

	// Token: 0x040008AE RID: 2222
	public bool DEBUG_PauseOnExplode;
}
