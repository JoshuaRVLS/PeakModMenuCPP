using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class Action_AskBingBong : ItemAction
{
	// Token: 0x14000004 RID: 4
	// (add) Token: 0x06000FF1 RID: 4081 RVA: 0x0004E158 File Offset: 0x0004C358
	// (remove) Token: 0x06000FF2 RID: 4082 RVA: 0x0004E190 File Offset: 0x0004C390
	public event Action_AskBingBong.AskEvent OnAsk;

	// Token: 0x06000FF3 RID: 4083 RVA: 0x0004E1C8 File Offset: 0x0004C3C8
	public override void RunAction()
	{
		int num = Random.Range(0, this.responses.Length);
		if (this.debugCycle)
		{
			num = this.debug;
			this.debug++;
			if (this.debug >= this.responses.Length)
			{
				this.debug = 0;
			}
		}
		this.item.photonView.RPC("Ask", RpcTarget.All, new object[]
		{
			num,
			Time.time < this.lastAsked + 1f
		});
		if (Time.time > this.lastAsked + 1f)
		{
			this.lastAsked = Time.time;
		}
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x0004E278 File Offset: 0x0004C478
	[PunRPC]
	public void Ask(int index, bool spamming)
	{
		if (this.item.holderCharacter != null)
		{
			this.squishAnim.SetTrigger("Squish");
			SFX_Player.instance.PlaySFX(this.squeak, base.transform.position, base.transform, null, 1f, false);
			this.subtitles.gameObject.SetActive(false);
			if (this.askRoutine != null)
			{
				base.StopCoroutine(this.askRoutine);
			}
			base.StartCoroutine(this.AskRoutine(index, spamming));
		}
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x0004E305 File Offset: 0x0004C505
	private IEnumerator SubtitleRoutine(string subtitleID)
	{
		yield return new WaitForSeconds(0.19f);
		string text = LocalizedText.GetText(subtitleID, true);
		this.subtitles.gameObject.SetActive(true);
		this.subtitles.text = text;
		float t = 0f;
		while (t < 1.8f / this.source.pitch)
		{
			t += Time.deltaTime;
			this.subtitles.alpha = Mathf.Clamp01(t * 12f);
			this.subtitles.transform.localScale = Vector3.one * this.scaleCurve.Evaluate(Vector3.Distance(this.subtitles.transform.position, MainCamera.instance.cam.transform.position));
			this.subtitles.transform.forward = MainCamera.instance.cam.transform.forward;
			yield return null;
		}
		this.subtitles.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x0004E31B File Offset: 0x0004C51B
	private IEnumerator AskRoutine(int index, bool spamming)
	{
		float t = 0f;
		while (t < 0.5f)
		{
			this.item.holderCharacter.refs.items.UpdateAttachedItem();
			t += Time.deltaTime;
			yield return null;
		}
		if (spamming)
		{
			yield break;
		}
		this.source.Stop();
		this.subtitles.gameObject.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		if (this.subtitleRoutine != null)
		{
			base.StopCoroutine(this.subtitleRoutine);
		}
		this.subtitleRoutine = base.StartCoroutine(this.SubtitleRoutine(this.responses[index].subtitleID));
		if (this.responses[index].sfx != null)
		{
			this.source.PlayOneShot(this.responses[index].sfx.clips[0]);
			if (this.OnAsk != null)
			{
				this.OnAsk(this.responses[index].sfx.clips[0]);
			}
		}
		yield break;
	}

	// Token: 0x04000D86 RID: 3462
	public AnimationCurve animationCurve;

	// Token: 0x04000D87 RID: 3463
	public SFX_Instance shake;

	// Token: 0x04000D88 RID: 3464
	public SFX_Instance squeak;

	// Token: 0x04000D89 RID: 3465
	public Action_AskBingBong.BingBongResponse[] responses;

	// Token: 0x04000D8A RID: 3466
	public Animator squishAnim;

	// Token: 0x04000D8B RID: 3467
	public Animator anim;

	// Token: 0x04000D8C RID: 3468
	public bool debugCycle;

	// Token: 0x04000D8D RID: 3469
	private int debug;

	// Token: 0x04000D8E RID: 3470
	public TextMeshPro subtitles;

	// Token: 0x04000D8F RID: 3471
	private float lastAsked;

	// Token: 0x04000D90 RID: 3472
	public AnimationCurve scaleCurve;

	// Token: 0x04000D91 RID: 3473
	public AudioSource source;

	// Token: 0x04000D93 RID: 3475
	public static int MOUTHBLENDID = Animator.StringToHash("Mouth Blend");

	// Token: 0x04000D94 RID: 3476
	private Coroutine askRoutine;

	// Token: 0x04000D95 RID: 3477
	private Coroutine subtitleRoutine;

	// Token: 0x020004EE RID: 1262
	// (Invoke) Token: 0x06001E39 RID: 7737
	public delegate void AskEvent(AudioClip clip);

	// Token: 0x020004EF RID: 1263
	[Serializable]
	public class BingBongResponse
	{
		// Token: 0x04001BA3 RID: 7075
		public string subtitleID;

		// Token: 0x04001BA4 RID: 7076
		public SFX_Instance sfx;

		// Token: 0x04001BA5 RID: 7077
		public AnimationCurve mouthCurve;

		// Token: 0x04001BA6 RID: 7078
		public float mouthCurveTime = 1f;
	}
}
