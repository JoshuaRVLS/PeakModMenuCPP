using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200037C RID: 892
public class WarpCompassVFX : ItemVFX
{
	// Token: 0x06001765 RID: 5989 RVA: 0x00078AC9 File Offset: 0x00076CC9
	private new void Start()
	{
		base.Start();
		GameUtils instance = GameUtils.instance;
		instance.OnUpdatedFeedData = (Action)Delegate.Combine(instance.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
	}

	// Token: 0x06001766 RID: 5990 RVA: 0x00078AF7 File Offset: 0x00076CF7
	private void OnDestroy()
	{
		GameUtils instance = GameUtils.instance;
		instance.OnUpdatedFeedData = (Action)Delegate.Remove(instance.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
	}

	// Token: 0x06001767 RID: 5991 RVA: 0x00078B20 File Offset: 0x00076D20
	protected override void Update()
	{
		base.Update();
		float b = this.item.castProgress;
		if ((!this.item.isUsingPrimary || this.item.finishedCast) && this.timeStartedBeingUsedOnMe == 0f)
		{
			b = 0f;
		}
		else if (this.timeStartedBeingUsedOnMe > 0f)
		{
			b = (Time.time - this.timeStartedBeingUsedOnMe) / this.item.totalSecondaryUsingTime;
		}
		this.warpPost.enabled = (this.warpPost.weight > 0.01f);
		this.warpPost2.enabled = (this.warpPost2.weight > 0.01f);
		if (this.warpPost2.weight >= 1f)
		{
			this.warpPost.weight = 0f;
		}
		else
		{
			this.warpPost.weight = Mathf.Lerp(this.warpPost.weight, b, Time.deltaTime * 10f);
		}
		this.warpPost2.weight = this.warpPost2Curve.Evaluate(this.warpPost.weight);
		this.compassPointer.speedMultiplier = 1f + this.warpPost.weight * 4f;
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x00078C5C File Offset: 0x00076E5C
	protected override void Shake()
	{
		GamefeelHandler.instance.AddPerlinShake(this.warpPost.weight * this.shakeAmount * Time.deltaTime * 100f, 0.2f, 15f);
	}

	// Token: 0x06001769 RID: 5993 RVA: 0x00078C90 File Offset: 0x00076E90
	private void OnUpdatedFeedData()
	{
		bool flag = false;
		using (List<FeedData>.Enumerator enumerator = GameUtils.instance.GetFeedDataForReceiver(Character.localCharacter.photonView.ViewID).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.itemID == this.item.itemID)
				{
					flag = true;
					if (this.timeStartedBeingUsedOnMe == 0f)
					{
						this.timeStartedBeingUsedOnMe = Time.time;
					}
				}
			}
		}
		if (!flag)
		{
			this.timeStartedBeingUsedOnMe = 0f;
		}
	}

	// Token: 0x040015D8 RID: 5592
	public Volume warpPost;

	// Token: 0x040015D9 RID: 5593
	public Volume warpPost2;

	// Token: 0x040015DA RID: 5594
	public float maxCastProgress = 1.1f;

	// Token: 0x040015DB RID: 5595
	public AnimationCurve warpPost2Curve;

	// Token: 0x040015DC RID: 5596
	public CompassPointer compassPointer;

	// Token: 0x040015DD RID: 5597
	public float timeStartedBeingUsedOnMe;
}
