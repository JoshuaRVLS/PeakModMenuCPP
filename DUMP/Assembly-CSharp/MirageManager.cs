using System;
using UnityEngine;

// Token: 0x02000140 RID: 320
public class MirageManager : MonoBehaviour
{
	// Token: 0x06000A82 RID: 2690 RVA: 0x00037D8C File Offset: 0x00035F8C
	public void sampleCamera()
	{
		this.cam.transform.position = Camera.main.transform.position;
		this.cam.transform.rotation = Camera.main.transform.rotation;
		this.cam.fieldOfView = Camera.main.fieldOfView;
		this.cam.Render();
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x00037DF8 File Offset: 0x00035FF8
	private void Awake()
	{
		this.renderTexture = this.cam.targetTexture;
		this.renderTexture.width = Screen.width / this.rtDownscale;
		this.renderTexture.height = Screen.height / this.rtDownscale;
		MirageManager.instance = this;
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x00037E4A File Offset: 0x0003604A
	private void setParticleData()
	{
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x00037E4C File Offset: 0x0003604C
	private void Update()
	{
	}

	// Token: 0x040009BA RID: 2490
	public static MirageManager instance;

	// Token: 0x040009BB RID: 2491
	public Camera cam;

	// Token: 0x040009BC RID: 2492
	private RenderTexture renderTexture;

	// Token: 0x040009BD RID: 2493
	public int rtDownscale = 2;
}
