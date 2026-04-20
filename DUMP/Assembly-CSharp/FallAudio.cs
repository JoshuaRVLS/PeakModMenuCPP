using System;
using UnityEngine;

// Token: 0x02000267 RID: 615
public class FallAudio : MonoBehaviour
{
	// Token: 0x0600122D RID: 4653 RVA: 0x0005B1AC File Offset: 0x000593AC
	private void Update()
	{
		this.yVel = base.transform.position.y - this.prevY;
		this.prevY = base.transform.position.y;
		this.au.volume = Mathf.Lerp(this.au.volume, Mathf.Abs(this.yVel) / 10f, Time.deltaTime * 10f);
		if (this.au.volume > 0.5f)
		{
			this.au.volume = 0.5f;
		}
	}

	// Token: 0x04001036 RID: 4150
	public AudioSource au;

	// Token: 0x04001037 RID: 4151
	private float yVel;

	// Token: 0x04001038 RID: 4152
	private float prevY;
}
