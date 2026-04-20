using System;
using UnityEngine;

// Token: 0x0200031B RID: 795
public class RemoveMusic : MonoBehaviour
{
	// Token: 0x06001540 RID: 5440 RVA: 0x0006B3AA File Offset: 0x000695AA
	private void Start()
	{
		this.musics = GameObject.FindGameObjectsWithTag("Music");
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x0006B3BC File Offset: 0x000695BC
	private void Update()
	{
		for (int i = 0; i < this.musics.Length; i++)
		{
			if (this.musics[i] != null)
			{
				this.musics[i].GetComponent<AudioSource>().volume /= 1.01f;
			}
		}
	}

	// Token: 0x04001355 RID: 4949
	public GameObject[] musics;
}
