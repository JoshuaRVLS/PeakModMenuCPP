using System;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class AntlionSandRumbleSFX : MonoBehaviour
{
	// Token: 0x0600102A RID: 4138 RVA: 0x0004FEC1 File Offset: 0x0004E0C1
	private void Start()
	{
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x0004FED0 File Offset: 0x0004E0D0
	private void Update()
	{
		if (this.refObj)
		{
			if (this.refObj.active)
			{
				this.source.volume = Mathf.Lerp(this.source.volume, this.vol, 5f * Time.deltaTime);
				return;
			}
			this.source.volume = Mathf.Lerp(this.source.volume, 0f, 5f * Time.deltaTime);
		}
	}

	// Token: 0x04000E03 RID: 3587
	public GameObject refObj;

	// Token: 0x04000E04 RID: 3588
	public float vol = 0.3f;

	// Token: 0x04000E05 RID: 3589
	private AudioSource source;
}
