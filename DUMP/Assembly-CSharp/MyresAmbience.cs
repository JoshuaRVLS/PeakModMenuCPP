using System;
using UnityEngine;

// Token: 0x020002B8 RID: 696
public class MyresAmbience : MonoBehaviour
{
	// Token: 0x060013AC RID: 5036 RVA: 0x00063DA0 File Offset: 0x00061FA0
	private void Update()
	{
		if (this.anim)
		{
			if (this.anim.GetFloat("Myers Distance") > 60f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0f, 1f * Time.deltaTime);
			}
			if (this.anim.GetFloat("Myers Distance") < 50f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0.25f, 1f * Time.deltaTime);
			}
			if (this.anim.GetFloat("Myers Distance") < 25f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0.75f, 1f * Time.deltaTime);
			}
			if (this.anim.GetFloat("Myers Distance") == 0f)
			{
				this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0f, 1f * Time.deltaTime);
			}
		}
	}

	// Token: 0x040011FC RID: 4604
	public Animator anim;

	// Token: 0x040011FD RID: 4605
	public AudioSource fearMusic;
}
