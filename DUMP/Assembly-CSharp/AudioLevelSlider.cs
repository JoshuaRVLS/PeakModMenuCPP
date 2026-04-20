using System;
using Peak.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200005E RID: 94
public class AudioLevelSlider : MonoBehaviour
{
	// Token: 0x17000060 RID: 96
	// (get) Token: 0x060004B4 RID: 1204 RVA: 0x0001C84D File Offset: 0x0001AA4D
	public SelectableSlider ParentContainer
	{
		get
		{
			return this._container;
		}
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x060004B5 RID: 1205 RVA: 0x0001C855 File Offset: 0x0001AA55
	public bool isLocal
	{
		get
		{
			return this.player.IsLocal;
		}
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x0001C862 File Offset: 0x0001AA62
	private void Update()
	{
		Photon.Realtime.Player player = this.player;
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0001C86B File Offset: 0x0001AA6B
	private void Awake()
	{
		this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0001C88C File Offset: 0x0001AA8C
	public void Init(Photon.Realtime.Player newPlayer)
	{
		this.player = newPlayer;
		Photon.Realtime.Player player = this.player;
		bool flag = this.player != null && !PhotonNetwork.OfflineMode;
		base.gameObject.SetActive(flag);
		if (flag)
		{
			this._container = base.GetComponent<SelectableSlider>();
			this._container.Init();
			bool isLocal = this.player.IsLocal;
			this.isLocalObject.SetActive(isLocal);
			this.isNotLocalObject.SetActive(!isLocal);
			this.crown.SetActive(this.player.IsMasterClient);
			this.playerName.text = this.player.NickName;
			this.playerName.color = (this.player.IsMasterClient ? this.playerColorGold : this.playerColorDefault);
			this.slider.SetValueWithoutNotify(AudioLevels.GetPlayerLevel(this.player.UserId));
		}
		this.bar.color = this.barGradient.Evaluate(this.slider.value);
		this.percent.text = Mathf.RoundToInt(this.slider.value * 200f).ToString() + "%";
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0001C9CC File Offset: 0x0001ABCC
	private void OnSliderChanged(float newValue)
	{
		if (this.player != null)
		{
			AudioLevels.SetPlayerLevel(this.player.UserId, newValue);
			this.icon.sprite = ((newValue == 0f) ? this.mutedAudioSprite : this.audioSprites[Mathf.FloorToInt(newValue * 2.99f)]);
			this.bar.color = this.barGradient.Evaluate(newValue);
			EventSystem.current.SetSelectedGameObject(null);
			this.percent.text = Mathf.RoundToInt(newValue * 200f).ToString() + "%";
		}
	}

	// Token: 0x0400051D RID: 1309
	private SelectableSlider _container;

	// Token: 0x0400051E RID: 1310
	public TextMeshProUGUI playerName;

	// Token: 0x0400051F RID: 1311
	public TextMeshProUGUI percent;

	// Token: 0x04000520 RID: 1312
	public Photon.Realtime.Player player;

	// Token: 0x04000521 RID: 1313
	public Slider slider;

	// Token: 0x04000522 RID: 1314
	public Image bar;

	// Token: 0x04000523 RID: 1315
	public Gradient barGradient;

	// Token: 0x04000524 RID: 1316
	public Sprite[] audioSprites;

	// Token: 0x04000525 RID: 1317
	public Sprite mutedAudioSprite;

	// Token: 0x04000526 RID: 1318
	public Image icon;

	// Token: 0x04000527 RID: 1319
	public GameObject isLocalObject;

	// Token: 0x04000528 RID: 1320
	public GameObject isNotLocalObject;

	// Token: 0x04000529 RID: 1321
	public GameObject crown;

	// Token: 0x0400052A RID: 1322
	public Color playerColorGold;

	// Token: 0x0400052B RID: 1323
	public Color playerColorDefault;
}
