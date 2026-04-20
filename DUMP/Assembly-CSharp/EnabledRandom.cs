using System;
using UnityEngine;

// Token: 0x0200025D RID: 605
public class EnabledRandom : MonoBehaviour
{
	// Token: 0x06001210 RID: 4624 RVA: 0x0005AAA5 File Offset: 0x00058CA5
	private void Start()
	{
		this.odds = Random.Range(0, 4);
		if (this.odds < 2)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04001013 RID: 4115
	public int odds = 1;
}
