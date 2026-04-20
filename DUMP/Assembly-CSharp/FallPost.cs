using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000268 RID: 616
public class FallPost : MonoBehaviour
{
	// Token: 0x0600122F RID: 4655 RVA: 0x0005B24D File Offset: 0x0005944D
	private void Start()
	{
		this.vol = base.GetComponent<Volume>();
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x0005B25C File Offset: 0x0005945C
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.vol.enabled = (this.vol.weight > 0.0001f);
		if (Character.localCharacter.data.fallSeconds > 0f)
		{
			this.vol.weight = Mathf.Lerp(this.vol.weight, 1f, Time.deltaTime);
			return;
		}
		this.vol.weight = Mathf.Lerp(this.vol.weight, 0f, Time.deltaTime);
	}

	// Token: 0x04001039 RID: 4153
	private Volume vol;
}
