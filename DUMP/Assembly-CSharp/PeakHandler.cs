using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.Core;

// Token: 0x020002C7 RID: 711
public class PeakHandler : Singleton<PeakHandler>
{
	// Token: 0x0600140C RID: 5132 RVA: 0x000653FA File Offset: 0x000635FA
	public void SummonHelicopter()
	{
		this.peakSequence.SetActive(true);
		this.summonedHelicopter = true;
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x00065410 File Offset: 0x00063610
	public void EndCutscene()
	{
		this.isPlayingCinematic = true;
		List<Character> allCharacters = Character.AllCharacters;
		foreach (Character character in allCharacters)
		{
			character.refs.animator.gameObject.SetActive(false);
		}
		MainCamera.instance.gameObject.SetActive(false);
		MenuWindow.CloseAllWindows();
		this.peakSequence.SetActive(false);
		GUIManager.instance.letterboxCanvas.gameObject.SetActive(true);
		GUIManager.instance.hudCanvas.enabled = false;
		this.endCutscene.SetActive(true);
		this.SetCosmetics(allCharacters);
		base.StartCoroutine(this.<EndCutscene>g__OpenEndscreen|12_0());
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x000654E0 File Offset: 0x000636E0
	private void SetCosmetics(List<Character> characters)
	{
		Singleton<MicrophoneRelay>.Instance.RegisterMicListener(new Action<float[]>(this.OnGetLocalMic));
		characters = (from character in characters
		where character.refs.stats.won
		select character).ToList<Character>();
		characters.Sort((Character c1, Character c2) => c1.photonView.ViewID.CompareTo(c2.photonView.ViewID));
		characters[0].refs.customization.SetCustomizationForRef(this.firstCutsceneScout);
		if (characters[0].data.isSkeleton)
		{
			this.firstCutsceneScout.SetSkeleton(true, false);
		}
		this.firstCutsceneScout.GetComponent<AnimatedMouth>().audioSource = characters[0].GetComponent<AnimatedMouth>().audioSource;
		this.localMouths.Add(this.firstCutsceneScout.GetComponent<AnimatedMouth>());
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			if (i >= characters.Count)
			{
				this.cutsceneScoutRefs[i].gameObject.SetActive(false);
			}
			else
			{
				characters[i].refs.customization.SetCustomizationForRef(this.cutsceneScoutRefs[num]);
				if (characters[i].data.isSkeleton)
				{
					this.cutsceneScoutRefs[i].SetSkeleton(true, false);
				}
				BadgeUnlocker.SetBadges(characters[i], this.cutsceneScoutRefs[num].sashRenderer);
				this.cutsceneScoutRefs[num].GetComponent<AnimatedMouth>().audioSource = characters[i].GetComponent<AnimatedMouth>().audioSource;
				if (characters[i].IsLocal)
				{
					this.localMouths.Add(this.cutsceneScoutRefs[num].GetComponent<AnimatedMouth>());
				}
				num++;
			}
		}
		if (characters.Count <= 1)
		{
			this.cutsceneScoutAnims[0].alone = true;
		}
		if (characters.Count <= 2)
		{
			this.cutsceneScoutAnims[1].alone = true;
		}
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x000656D0 File Offset: 0x000638D0
	private void OnGetLocalMic(float[] buffer)
	{
		foreach (AnimatedMouth animatedMouth in this.localMouths)
		{
			animatedMouth.OnGetMic(buffer);
		}
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x00065724 File Offset: 0x00063924
	public void EndScreenComplete()
	{
		Singleton<GameOverHandler>.Instance.ForceEveryPlayerDoneWithEndScreen();
		this.endScreenComplete = true;
		base.StartCoroutine(PeakHandler.<EndScreenComplete>g__CreditsLogic|15_0());
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x00065743 File Offset: 0x00063943
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (this.isPlayingCinematic && Singleton<MicrophoneRelay>.Instance)
		{
			Singleton<MicrophoneRelay>.Instance.UnregisterMicListener(new Action<float[]>(this.OnGetLocalMic));
		}
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x00065793 File Offset: 0x00063993
	[CompilerGenerated]
	private IEnumerator <EndCutscene>g__OpenEndscreen|12_0()
	{
		yield return new WaitForSeconds(this.secondsUntilEndscreen);
		GUIManager.instance.endScreen.Open();
		while (!this.endScreenComplete)
		{
			yield return null;
		}
		this.endCutsceneAnimator.SetBool("Next", true);
		GUIManager.instance.endScreen.Close();
		yield break;
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x000657A2 File Offset: 0x000639A2
	[CompilerGenerated]
	internal static IEnumerator <EndScreenComplete>g__CreditsLogic|15_0()
	{
		yield return new WaitForSecondsRealtime(20f);
		InputAction anyKeyAction = InputSystem.actions.FindAction("AnyKey", false);
		float timeCreditsStarted = Time.unscaledTime;
		float t = 0f;
		float timeAnyKeyLastPressed = float.NegativeInfinity;
		bool anyKeyPressed = false;
		while (Time.unscaledTime - timeCreditsStarted < 60f)
		{
			if (anyKeyAction != null && anyKeyAction.WasPerformedThisFrame() && !anyKeyPressed)
			{
				anyKeyPressed = true;
				timeAnyKeyLastPressed = Time.unscaledTime;
			}
			if (anyKeyPressed)
			{
				if (GUIManager.TimeLastPaused + 0.5f > timeAnyKeyLastPressed)
				{
					Debug.Log("'Any key' consumed by pause menu");
					anyKeyPressed = false;
				}
				else if (Time.unscaledTime - timeAnyKeyLastPressed > 0.5f)
				{
					break;
				}
			}
			t += Time.unscaledDeltaTime;
			yield return null;
		}
		Debug.Log("Local player is done with credits!");
		Singleton<GameOverHandler>.Instance.LoadAirport();
		yield break;
	}

	// Token: 0x0400123D RID: 4669
	public bool summonedHelicopter;

	// Token: 0x0400123E RID: 4670
	public GameObject peakSequence;

	// Token: 0x0400123F RID: 4671
	public GameObject endCutscene;

	// Token: 0x04001240 RID: 4672
	public Animator endCutsceneAnimator;

	// Token: 0x04001241 RID: 4673
	public float secondsUntilEndscreen = 13f;

	// Token: 0x04001242 RID: 4674
	public CustomizationRefs firstCutsceneScout;

	// Token: 0x04001243 RID: 4675
	public CustomizationRefs[] cutsceneScoutRefs;

	// Token: 0x04001244 RID: 4676
	public EndCutsceneScoutHelper[] cutsceneScoutAnims;

	// Token: 0x04001245 RID: 4677
	private List<AnimatedMouth> localMouths = new List<AnimatedMouth>();

	// Token: 0x04001246 RID: 4678
	public bool isPlayingCinematic;

	// Token: 0x04001247 RID: 4679
	private bool endScreenComplete;
}
