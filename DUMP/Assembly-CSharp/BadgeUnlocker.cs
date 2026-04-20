using System;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class BadgeUnlocker : MonoBehaviour
{
	// Token: 0x060004DB RID: 1243 RVA: 0x0001D0B4 File Offset: 0x0001B2B4
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x0001D0C4 File Offset: 0x0001B2C4
	public void Update()
	{
		if (this.useTestBadge)
		{
			int num = GUIManager.instance.mainBadgeManager.badgeData.Length;
			Texture2D texture2D = new Texture2D(num, 1);
			texture2D.filterMode = FilterMode.Point;
			for (int i = 0; i < num; i++)
			{
				texture2D.SetPixel(i, 1, Color.black);
			}
			texture2D.SetPixel(this.testBadge, 1, Color.white);
			texture2D.Apply();
			this.badgeSashRenderer.materials[0].SetTexture("BadgeUnlockTexture", texture2D);
		}
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x0001D144 File Offset: 0x0001B344
	public static void SetBadges(Character refCharacter, Renderer sashRenderer)
	{
		int num = refCharacter.data.badgeStatus.Length;
		Texture2D texture2D = new Texture2D(num, 1);
		texture2D.filterMode = FilterMode.Point;
		for (int i = 0; i < num; i++)
		{
			if (refCharacter.data.badgeStatus[i])
			{
				if (GUIManager.instance.mainBadgeManager.badgeData[i] != null)
				{
					texture2D.SetPixel(GUIManager.instance.mainBadgeManager.badgeData[i].visualID, 1, Color.white);
				}
				else
				{
					texture2D.SetPixel(i, 1, Color.white);
				}
			}
			else if (GUIManager.instance.mainBadgeManager.badgeData[i] != null)
			{
				texture2D.SetPixel(GUIManager.instance.mainBadgeManager.badgeData[i].visualID, 1, Color.black);
			}
			else
			{
				texture2D.SetPixel(i, 1, Color.black);
			}
		}
		texture2D.Apply();
		if (sashRenderer == null)
		{
			return;
		}
		sashRenderer.materials[0].SetTexture("BadgeUnlockTexture", texture2D);
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x0001D248 File Offset: 0x0001B448
	public void BadgeUnlockVisual()
	{
		if (!this.character)
		{
			this.character = base.GetComponent<Character>();
		}
		BadgeUnlocker.SetBadges(this.character, this.badgeSashRenderer);
	}

	// Token: 0x04000535 RID: 1333
	public int testBadge;

	// Token: 0x04000536 RID: 1334
	public bool useTestBadge;

	// Token: 0x04000537 RID: 1335
	private Character character;

	// Token: 0x04000538 RID: 1336
	public Renderer badgeSashRenderer;
}
