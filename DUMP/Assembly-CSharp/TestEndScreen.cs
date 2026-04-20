using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000095 RID: 149
public class TestEndScreen : MonoBehaviour
{
	// Token: 0x060005FB RID: 1531 RVA: 0x000223EA File Offset: 0x000205EA
	private void RunTest()
	{
		base.gameObject.SetActive(true);
		base.StartCoroutine(this.EndSequenceRoutine((Random.value + 0.75f) * this.runTimeInSeconds, this.didLocalWin, this.didOthersWin));
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x00022423 File Offset: 0x00020623
	private IEnumerator EndSequenceRoutine(float runTime, bool localWon, bool othersWon)
	{
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.DOFade(1f, 1f);
		for (int i = 0; i < this.scoutWindows.Length; i++)
		{
			if (i < 4)
			{
				this.scoutWindows[i].gameObject.SetActive(true);
				this.scoutWindows[i].Init(i == 0, string.Format("Scout{0}", i + 1), this.scoutColors[i]);
			}
			else
			{
				this.scoutWindows[i].gameObject.SetActive(false);
			}
		}
		this.endTime.gameObject.SetActive(false);
		this.buttons.SetActive(false);
		this.peakBanner.SetActive(localWon);
		this.yourFriendsWonBanner.SetActive(!localWon && othersWon);
		this.deadBanner.SetActive(!localWon && !othersWon);
		yield return new WaitForSeconds(2f);
		try
		{
			this.endTime.text = this.GetTimeString(runTime);
			this.endTime.gameObject.SetActive(true);
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
		}
		yield return new WaitForSeconds(1f);
		yield return base.StartCoroutine(this.TimelineRoutine(this.GenerateMockTimelines()));
		yield return new WaitForSeconds(0.5f);
		this.buttons.SetActive(true);
		yield break;
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x00022448 File Offset: 0x00020648
	private List<List<EndScreen.TimelineInfo>> GenerateMockTimelines()
	{
		float num = Random.value * 100f;
		float num2 = this.climbIntervalVariance * 1200f / (float)this.numTimelinePoints;
		List<List<EndScreen.TimelineInfo>> list = new List<List<EndScreen.TimelineInfo>>();
		for (int i = 0; i < 1; i++)
		{
			float num3 = num;
			float num4 = 0f;
			List<EndScreen.TimelineInfo> list2 = new List<EndScreen.TimelineInfo>();
			int num5 = 10;
			float num6 = 1f;
			for (int j = 0; j < this.numTimelinePoints; j++)
			{
				if (j == num5)
				{
					num6 = Mathf.Sign(Random.value * 2f - 1f);
					num5 += Mathf.FloorToInt(Random.value * (float)this.numTimelinePoints / 100f);
				}
				list2.Add(new EndScreen.TimelineInfo(num4, num3, EndScreen.TimelineNote.None));
				num3 += 10f;
				num4 += Mathf.Clamp(num6 * (3f * Random.value * num2 - Random.value * num2), 0f, 1200f);
			}
			list.Add(list2);
		}
		for (int k = 1; k < 4; k++)
		{
			int num7 = 64;
			for (int l = 1; l < k; l++)
			{
				num7 /= 2;
			}
			list.Add(CharacterStats.Downsample(list[0], num7));
		}
		return list;
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00022590 File Offset: 0x00020790
	private string GetTimeString(float totalSeconds)
	{
		int num = Mathf.FloorToInt(totalSeconds);
		int num2 = num / 3600;
		int num3 = num % 3600 / 60;
		int num4 = num % 60;
		return string.Format("{0}:{1:00}:{2:00}", num2, num3, num4);
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x000225D6 File Offset: 0x000207D6
	private IEnumerator TimelineRoutine(List<List<EndScreen.TimelineInfo>> timelineInfos)
	{
		for (int j = 0; j < this.scouts.Length; j++)
		{
			this.scouts[j].gameObject.SetActive(false);
			this.scoutsAtPeak[j].gameObject.SetActive(false);
		}
		for (int k = 0; k < 4; k++)
		{
			if (k < this.scouts.Length)
			{
				Color color = this.scoutColors[k];
				color.a = 1f;
				this.scouts[k].color = color;
				this.scoutsAtPeak[k].color = this.scouts[k].color;
			}
		}
		yield return new WaitForSeconds(0.1f);
		for (int l = 0; l < timelineInfos.Count; l++)
		{
			if (l < this.scouts.Length)
			{
				this.scouts[l].gameObject.SetActive(true);
			}
		}
		int num = 1;
		for (int m = 0; m < timelineInfos.Count; m++)
		{
			if (timelineInfos[m].Count > num)
			{
				num = timelineInfos[m].Count;
			}
		}
		float startTime = 100000f;
		float maxTime = 0f;
		List<EndScreen.TimelineInfo> list = timelineInfos[0];
		maxTime = list[list.Count - 1].time;
		startTime = timelineInfos[0][0].time;
		maxTime -= startTime;
		if (maxTime == 0f)
		{
			maxTime = 1f;
		}
		float yieldTime = Mathf.Min(this.waitTime * Time.deltaTime / (float)num, 0.2f);
		int count = timelineInfos.Count;
		List<int> timelineIndices = new List<int>(count);
		for (int n = 0; n < count; n++)
		{
			timelineIndices.Add(0);
		}
		List<int> timelinesFinished = new List<int>();
		int num6;
		while (timelinesFinished.Count < timelineInfos.Count)
		{
			float num2 = float.MaxValue;
			int num3 = 0;
			for (int num4 = timelineIndices.Count - 1; num4 >= 0; num4--)
			{
				int num5 = timelineIndices[num4];
				if (num5 >= timelineInfos[num4].Count)
				{
					if (!timelinesFinished.Contains(num4))
					{
						timelinesFinished.Add(num4);
					}
				}
				else
				{
					float time = timelineInfos[num4][num5].time;
					if (time < num2)
					{
						num2 = time;
						num3 = num4;
					}
				}
			}
			List<EndScreen.TimelineInfo> list2 = timelineInfos[num3];
			int index = timelineIndices[num3];
			this.DrawPip(num3, list2[index], maxTime, startTime, this.scouts[num3].color);
			if (!list2[index].dead && !list2[index].died)
			{
				this.scoutWindows[num3].UpdateAltitude(CharacterStats.UnitsToMeters(list2[index].height));
			}
			List<int> list3 = timelineIndices;
			int index2 = num3;
			num6 = list3[index2];
			list3[index2] = num6 + 1;
			yield return new WaitForSeconds(yieldTime * 0.33f);
		}
		for (int i = 0; i < timelineInfos.Count; i = num6 + 1)
		{
			Debug.Log(string.Format("Checking timeline info {0}, has infos: {1}", i, timelineInfos[i].Count));
			if (timelineInfos[i].Count > 0)
			{
				this.CheckPeak(i, timelineInfos[i][timelineInfos[i].Count - 1]);
				yield return new WaitForSeconds(0.25f);
			}
			num6 = i;
		}
		yield break;
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x000225EC File Offset: 0x000207EC
	public void PopupNext()
	{
		this.inPopupView = false;
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x000225F8 File Offset: 0x000207F8
	public void DrawPip(int playerIndex, EndScreen.TimelineInfo heightTime, float maxTime, float startTime, Color color)
	{
		if (heightTime.dead)
		{
			return;
		}
		Image image = Object.Instantiate<Image>(heightTime.revived ? this.revivedPip : (heightTime.justPassedOut ? this.passedOutPip : (heightTime.died ? this.deadPip : this.pip)), this.scoutLines[playerIndex]);
		image.color = color;
		image.transform.GetChild(0).GetComponent<Image>().color = image.color;
		float num = 1200f;
		image.transform.localPosition = new Vector3(this.timelinePanel.sizeDelta.x * Mathf.Clamp01((heightTime.time - startTime) / maxTime), this.timelinePanel.sizeDelta.y * heightTime.height / num, 0f);
		image.transform.localPosition += Vector3.up * (float)playerIndex * 2f;
		this.scouts[playerIndex].transform.localPosition = image.transform.localPosition;
		if (this.oldPip[playerIndex])
		{
			image.transform.right = this.oldPip[playerIndex].transform.position - image.transform.position;
			image.rectTransform.sizeDelta = new Vector2(Vector3.Distance(image.transform.position, this.oldPip[playerIndex].transform.position) / this.timelinePanel.lossyScale.x, 1.5f);
		}
		if (heightTime.died)
		{
			this.scouts[playerIndex].gameObject.SetActive(false);
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
		}
		if (heightTime.justPassedOut)
		{
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
		}
		else if (heightTime.passedOut)
		{
			image.transform.GetChild(0).GetComponent<Image>().material = this.passedOutMaterial;
		}
		if (heightTime.revived)
		{
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
			image.transform.GetChild(0).gameObject.SetActive(false);
			this.scouts[playerIndex].gameObject.SetActive(true);
		}
		this.oldPip[playerIndex] = image;
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x000228CC File Offset: 0x00020ACC
	public void CheckPeak(int playerIndex, EndScreen.TimelineInfo timelineInfo)
	{
		if (playerIndex < this.scouts.Length && timelineInfo.time >= 0.99f && timelineInfo.height >= 1f && !this.scoutsAtPeak[playerIndex].gameObject.activeSelf && !timelineInfo.dead && timelineInfo.won)
		{
			this.scouts[playerIndex].gameObject.SetActive(false);
			this.scoutsAtPeak[playerIndex].gameObject.SetActive(true);
			this.scoutsAtPeak[playerIndex].transform.SetSiblingIndex(1);
			this.scoutsAtPeak[playerIndex].rectTransform.sizeDelta = Vector3.zero;
			this.scoutsAtPeak[playerIndex].rectTransform.DOSizeDelta(Vector3.one * 15f, 0.25f, false).SetEase(Ease.OutBack);
		}
	}

	// Token: 0x04000607 RID: 1543
	public CanvasGroup canvasGroup;

	// Token: 0x04000608 RID: 1544
	public EndScreenScoutWindow[] scoutWindows;

	// Token: 0x04000609 RID: 1545
	public TMP_Text endTime;

	// Token: 0x0400060A RID: 1546
	public GameObject buttons;

	// Token: 0x0400060B RID: 1547
	public GameObject peakBanner;

	// Token: 0x0400060C RID: 1548
	public GameObject deadBanner;

	// Token: 0x0400060D RID: 1549
	public GameObject yourFriendsWonBanner;

	// Token: 0x0400060E RID: 1550
	public Image[] scouts;

	// Token: 0x0400060F RID: 1551
	public Image[] scoutsAtPeak;

	// Token: 0x04000610 RID: 1552
	public RectTransform timelinePanel;

	// Token: 0x04000611 RID: 1553
	public float waitTime = 5f;

	// Token: 0x04000612 RID: 1554
	public Image pip;

	// Token: 0x04000613 RID: 1555
	public Image deadPip;

	// Token: 0x04000614 RID: 1556
	public Image passedOutPip;

	// Token: 0x04000615 RID: 1557
	public Image revivedPip;

	// Token: 0x04000616 RID: 1558
	public Transform[] scoutLines;

	// Token: 0x04000617 RID: 1559
	public Material passedOutMaterial;

	// Token: 0x04000618 RID: 1560
	[Header("Debug Settings")]
	public Color[] scoutColors;

	// Token: 0x04000619 RID: 1561
	public float runTimeInSeconds = 3600f;

	// Token: 0x0400061A RID: 1562
	public bool didLocalWin = true;

	// Token: 0x0400061B RID: 1563
	public bool didOthersWin = true;

	// Token: 0x0400061C RID: 1564
	public int numTimelinePoints = 400;

	// Token: 0x0400061D RID: 1565
	[Range(1f, 2f)]
	public float climbIntervalVariance = 1.5f;

	// Token: 0x0400061E RID: 1566
	private bool inPopupView;

	// Token: 0x0400061F RID: 1567
	private Image[] oldPip = new Image[4];
}
