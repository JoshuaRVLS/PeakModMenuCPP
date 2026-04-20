using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200034A RID: 842
public class SpiderManager : MonoBehaviour
{
	// Token: 0x06001666 RID: 5734 RVA: 0x000722C9 File Offset: 0x000704C9
	private void Awake()
	{
		SpiderManager.instance = this;
	}

	// Token: 0x06001667 RID: 5735 RVA: 0x000722D4 File Offset: 0x000704D4
	private void Update()
	{
		if (this.spiders.Count == 0)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			this.spiders[this.currentIndex].Scan();
			this.currentIndex = (this.currentIndex + 1) % this.spiders.Count;
		}
	}

	// Token: 0x06001668 RID: 5736 RVA: 0x0007232B File Offset: 0x0007052B
	public void Register(Spider spider)
	{
		if (!this.spiders.Contains(spider))
		{
			this.spiders.Add(spider);
		}
	}

	// Token: 0x06001669 RID: 5737 RVA: 0x00072347 File Offset: 0x00070547
	public void Unregister(Spider spider)
	{
		this.spiders.Remove(spider);
		if (this.currentIndex >= this.spiders.Count)
		{
			this.currentIndex = 0;
		}
	}

	// Token: 0x040014A0 RID: 5280
	public static SpiderManager instance;

	// Token: 0x040014A1 RID: 5281
	public List<Spider> spiders = new List<Spider>();

	// Token: 0x040014A2 RID: 5282
	private int currentIndex;

	// Token: 0x040014A3 RID: 5283
	private const int spidersPerFrame = 3;
}
