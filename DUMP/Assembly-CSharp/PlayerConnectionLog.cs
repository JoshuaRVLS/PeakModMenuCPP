using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

// Token: 0x020001E8 RID: 488
public class PlayerConnectionLog : MonoBehaviourPunCallbacks
{
	// Token: 0x06000F82 RID: 3970 RVA: 0x0004C070 File Offset: 0x0004A270
	private void Awake()
	{
		GlobalEvents.OnAchievementThrown = (Action<ACHIEVEMENTTYPE>)Delegate.Combine(GlobalEvents.OnAchievementThrown, new Action<ACHIEVEMENTTYPE>(this.TestAchievementThrown));
		GlobalEvents.OnGemActivated = (Action<bool>)Delegate.Combine(GlobalEvents.OnGemActivated, new Action<bool>(this.TestGemActivated));
	}

	// Token: 0x06000F83 RID: 3971 RVA: 0x0004C0C0 File Offset: 0x0004A2C0
	private void OnDestroy()
	{
		GlobalEvents.OnAchievementThrown = (Action<ACHIEVEMENTTYPE>)Delegate.Remove(GlobalEvents.OnAchievementThrown, new Action<ACHIEVEMENTTYPE>(this.TestAchievementThrown));
		GlobalEvents.OnGemActivated = (Action<bool>)Delegate.Remove(GlobalEvents.OnGemActivated, new Action<bool>(this.TestGemActivated));
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x0004C110 File Offset: 0x0004A310
	private void RebuildString()
	{
		this.sb.Clear();
		foreach (string value in this.currentLog)
		{
			this.sb.Append(value);
			this.sb.Append("\n");
		}
		this.text.text = this.sb.ToString();
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x0004C19C File Offset: 0x0004A39C
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		if (newPlayer.IsLocal)
		{
			return;
		}
		string text = NetworkingUtilities.Sanitize(newPlayer.NickName);
		if (text.Length > 32)
		{
			text = text.Substring(0, 32);
		}
		newPlayer.NickName = text;
		string newValue = this.GetColorTag(this.userColor) + " " + newPlayer.NickName + "</color>";
		string s = this.GetColorTag(this.joinedColor) + LocalizedText.GetText("JOINEDTHEEXPEDITION", true).Replace("#", newValue) + "</color>";
		this.AddMessage(s);
		if (this.sfxJoin)
		{
			this.sfxJoin.Play(default(Vector3));
		}
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x0004C250 File Offset: 0x0004A450
	public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
	{
		if (!newPlayer.IsLocal)
		{
			if (newPlayer.NickName == "Bing Bong")
			{
				return;
			}
			string newValue = this.GetColorTag(this.userColor) + " " + newPlayer.NickName + "</color>";
			string s = this.GetColorTag(this.leftColor) + LocalizedText.GetText("LEFTTHEEXPEDITION", true).Replace("#", newValue) + "</color>";
			this.AddMessage(s);
			if (this.sfxLeave)
			{
				this.sfxLeave.Play(default(Vector3));
			}
		}
	}

	// Token: 0x06000F87 RID: 3975 RVA: 0x0004C2F4 File Offset: 0x0004A4F4
	public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		string newValue = this.GetColorTag(this.userColor) + " " + newMasterClient.NickName + "</color>";
		string text = this.GetColorTag(this.joinedColor) + LocalizedText.GetText("HOSTCHANGED", true).Replace("#", newValue) + "</color>";
		base.StartCoroutine(this.AddMessageDelayed(text));
	}

	// Token: 0x06000F88 RID: 3976 RVA: 0x0004C365 File Offset: 0x0004A565
	private IEnumerator AddMessageDelayed(string text)
	{
		yield return null;
		yield return null;
		this.AddMessage(text);
		yield break;
	}

	// Token: 0x06000F89 RID: 3977 RVA: 0x0004C37C File Offset: 0x0004A57C
	public void TestAddJoin()
	{
		string newValue = this.GetColorTag(this.userColor) + " TESTPLAYER</color>";
		string s = this.GetColorTag(this.joinedColor) + LocalizedText.GetText("JOINEDTHEEXPEDITION", true).Replace("#", newValue) + "</color>";
		this.AddMessage(s);
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x0004C3D4 File Offset: 0x0004A5D4
	public void TestAddLeft()
	{
		string newValue = this.GetColorTag(this.userColor) + " TESTPLAYER</color>";
		string s = this.GetColorTag(this.leftColor) + LocalizedText.GetText("LEFTTHEEXPEDITION", true).Replace("#", newValue) + "</color>";
		this.AddMessage(s);
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x0004C42C File Offset: 0x0004A62C
	private string GetColorTag(Color c)
	{
		return "<color=#" + ColorUtility.ToHtmlStringRGB(c) + ">";
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x0004C443 File Offset: 0x0004A643
	private void AddMessage(string s)
	{
		this.currentLog.Add(s);
		Debug.Log("Message sent to player log: " + s);
		this.RebuildString();
		base.StartCoroutine(this.TimeoutMessageRoutine());
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x0004C474 File Offset: 0x0004A674
	private IEnumerator TimeoutMessageRoutine()
	{
		yield return new WaitForSeconds(8f);
		this.currentLog.RemoveAt(0);
		this.RebuildString();
		yield break;
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x0004C484 File Offset: 0x0004A684
	private void TestAchievementThrown(ACHIEVEMENTTYPE type)
	{
		if (Application.isEditor || Debug.isDebugBuild)
		{
			string str = this.GetColorTag(this.userColor) + " " + type.ToString() + "</color>";
			string s = this.GetColorTag(this.joinedColor) + "Got Badge: </color>" + str;
			this.AddMessage(s);
		}
	}

	// Token: 0x06000F8F RID: 3983 RVA: 0x0004C4E8 File Offset: 0x0004A6E8
	private void TestGemActivated(bool activated)
	{
		string str = LocalizedText.GetText(activated ? "GEM_ACTIVATED" : "GEM_DEACTIVATED", true);
		this.AddMessage(this.GetColorTag(this.gemColor) + " " + str + "</color>");
	}

	// Token: 0x04000D00 RID: 3328
	public TextMeshProUGUI text;

	// Token: 0x04000D01 RID: 3329
	private List<string> currentLog = new List<string>();

	// Token: 0x04000D02 RID: 3330
	private StringBuilder sb = new StringBuilder();

	// Token: 0x04000D03 RID: 3331
	public Color joinedColor;

	// Token: 0x04000D04 RID: 3332
	public Color leftColor;

	// Token: 0x04000D05 RID: 3333
	public Color userColor;

	// Token: 0x04000D06 RID: 3334
	public Color gemColor;

	// Token: 0x04000D07 RID: 3335
	public SFX_Instance sfxJoin;

	// Token: 0x04000D08 RID: 3336
	public SFX_Instance sfxLeave;
}
