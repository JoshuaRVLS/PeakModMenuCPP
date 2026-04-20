using System;
using System.Collections.Generic;
using Peak.Network;
using Peak.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200005D RID: 93
public class AudioLevels : MonoBehaviour
{
	// Token: 0x060004A7 RID: 1191 RVA: 0x0001C3F8 File Offset: 0x0001A5F8
	public static void ResetSliders()
	{
		AudioLevels.PlayerAudioLevels.Clear();
		GlobalEvents.TriggerCharacterAudioLevelsUpdated();
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001C409 File Offset: 0x0001A609
	public static float GetPlayerLevel(string playerID)
	{
		if (!AudioLevels.PlayerAudioLevels.ContainsKey(playerID))
		{
			return 0.5f;
		}
		return AudioLevels.PlayerAudioLevels[playerID];
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0001C429 File Offset: 0x0001A629
	public static void SetPlayerLevel(string playerID, float f)
	{
		if (!AudioLevels.PlayerAudioLevels.ContainsKey(playerID))
		{
			AudioLevels.PlayerAudioLevels.Add(playerID, 1f);
		}
		AudioLevels.PlayerAudioLevels[playerID] = f;
		GlobalEvents.TriggerCharacterAudioLevelsUpdated();
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x0001C459 File Offset: 0x0001A659
	private void Update()
	{
		if (this._dirty)
		{
			this.UpdateSliders();
		}
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x0001C46C File Offset: 0x0001A66C
	public void OnEnable()
	{
		if (PhotonNetwork.OfflineMode && Application.isPlaying)
		{
			base.gameObject.SetActive(false);
		}
		this._dirty = true;
		PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Combine(PlayerHandler.CharacterRegistered, new Action<Character>(this.OnPlayerListChanged));
		NetCode.RoomEvents.PlayerLeft += this.OnPlayerListChanged;
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x0001C4D0 File Offset: 0x0001A6D0
	public void OnDisable()
	{
		NetCode.RoomEvents.PlayerLeft -= this.OnPlayerListChanged;
		PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Remove(PlayerHandler.CharacterRegistered, new Action<Character>(this.OnPlayerListChanged));
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x0001C508 File Offset: 0x0001A708
	private void OnPlayerListChanged(Character _)
	{
		this.OnPlayerListChanged();
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x0001C510 File Offset: 0x0001A710
	public void OnPlayerListChanged()
	{
		this._dirty = true;
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0001C51C File Offset: 0x0001A71C
	public void UpdateSliders()
	{
		this._dirty = false;
		Photon.Realtime.Player[] playerList = PhotonNetwork.PlayerList;
		int i = 0;
		for (int j = 0; j < playerList.Length; j++)
		{
			if (this.sliders.Count > j)
			{
				this.sliders[j].Init(playerList[j]);
			}
			i = j + 1;
		}
		while (i < this.sliders.Count)
		{
			this.sliders[i].Init(null);
			i++;
		}
		this.InitNavigation();
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x0001C598 File Offset: 0x0001A798
	private void InitNavigation()
	{
		if (!this.mainPage)
		{
			Debug.LogWarning("Couldn't init because no pause menu main page!");
		}
		Debug.Log("initializing audio sliders.");
		bool flag = false;
		for (int i = 0; i < this.sliders.Count; i++)
		{
			if (this.sliders[i].isActiveAndEnabled && !this.sliders[i].isLocal)
			{
				object obj = i == this.sliders.Count - 1 || !this.sliders[i + 1].gameObject.activeInHierarchy;
				SelectableSlider prev = flag ? this.sliders[i - 1].ParentContainer : null;
				object obj2 = obj;
				SelectableSlider next = (obj2 != null) ? null : this.sliders[i + 1].ParentContainer;
				this.SetSliderSelection(this.sliders[i].ParentContainer, prev, next);
				flag = true;
				if (obj2 != null)
				{
					Debug.Log("Pointing resume button at " + this.sliders[i].name);
					Navigation navigation = this.mainPage.resumeButton.navigation;
					navigation.mode = Navigation.Mode.Explicit;
					navigation.selectOnRight = this.sliders[i].ParentContainer.MySelectable;
					navigation.selectOnUp = this.sliders[i].ParentContainer.MySelectable;
					this.mainPage.resumeButton.navigation = navigation;
				}
			}
		}
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0001C718 File Offset: 0x0001A918
	private void SetSliderSelection(SelectableSlider current, SelectableSlider prev, SelectableSlider next)
	{
		Selectable selectable = (prev != null) ? prev.MySelectable : null;
		Selectable selectable2 = ((next != null) ? next.MySelectable : null) ?? this.mainPage.resumeButton;
		Debug.Log(string.Concat(new string[]
		{
			"Setting up nav for ",
			current.name,
			". Up: ",
			(selectable != null) ? selectable.name : null,
			" | Down: ",
			(selectable2 != null) ? selectable2.name : null
		}), this);
		Selectable mySelectable = current.MySelectable;
		Navigation navigation = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnUp = selectable,
			selectOnDown = selectable2,
			selectOnLeft = this.mainPage.resumeButton,
			selectOnRight = (NetCode.Session.IsHost ? current.GetComponentInChildren<KickButton>().MyButton : null)
		};
		mySelectable.navigation = navigation;
		Selectable childSlider = current.ChildSlider;
		navigation = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnUp = selectable,
			selectOnDown = selectable2,
			selectOnLeft = null,
			selectOnRight = null
		};
		childSlider.navigation = navigation;
	}

	// Token: 0x04000519 RID: 1305
	private bool _dirty;

	// Token: 0x0400051A RID: 1306
	public static Dictionary<string, float> PlayerAudioLevels = new Dictionary<string, float>();

	// Token: 0x0400051B RID: 1307
	public List<AudioLevelSlider> sliders;

	// Token: 0x0400051C RID: 1308
	[SerializeField]
	private PauseMenuMainPage mainPage;
}
