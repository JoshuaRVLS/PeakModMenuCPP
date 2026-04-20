using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001F4 RID: 500
public class UI_UseItemProgressFriend : MonoBehaviour
{
	// Token: 0x06000FC4 RID: 4036 RVA: 0x0004D630 File Offset: 0x0004B830
	public void Init(FeedData feedData)
	{
		this.giverID = feedData.giverID;
		this._maxTime = feedData.totalItemTime;
		Item item;
		if (ItemDatabase.TryGetItem(feedData.itemID, out item))
		{
			this.icon.texture = item.UIData.GetIcon();
		}
		Vector2 sizeDelta = this.rect.sizeDelta;
		this.rect.sizeDelta = Vector2.zero;
		this.rect.DOSizeDelta(sizeDelta, 0.5f, false).SetEase(Ease.OutBack);
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x0004D6B0 File Offset: 0x0004B8B0
	private void Update()
	{
		if (!this._dead)
		{
			this._currentTime += Time.deltaTime;
			this.fill.fillAmount = this._currentTime / this._maxTime;
		}
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x0004D6E4 File Offset: 0x0004B8E4
	public void Kill()
	{
		this._dead = true;
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04000D61 RID: 3425
	public RectTransform rect;

	// Token: 0x04000D62 RID: 3426
	public Image fill;

	// Token: 0x04000D63 RID: 3427
	public RawImage icon;

	// Token: 0x04000D64 RID: 3428
	public int giverID;

	// Token: 0x04000D65 RID: 3429
	private float _maxTime;

	// Token: 0x04000D66 RID: 3430
	private float _currentTime;

	// Token: 0x04000D67 RID: 3431
	private bool _dead;
}
