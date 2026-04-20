using System;
using UnityEngine;

// Token: 0x02000276 RID: 630
public class GoToURL : MonoBehaviour
{
	// Token: 0x06001279 RID: 4729 RVA: 0x0005C963 File Offset: 0x0005AB63
	public void Click()
	{
		Application.OpenURL(this.URL);
	}

	// Token: 0x04001089 RID: 4233
	public string URL;
}
