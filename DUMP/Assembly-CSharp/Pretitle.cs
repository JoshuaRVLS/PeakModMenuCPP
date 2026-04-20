using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Token: 0x020002DA RID: 730
public class Pretitle : MonoBehaviour
{
	// Token: 0x06001462 RID: 5218 RVA: 0x0006743A File Offset: 0x0006563A
	private void Start()
	{
		base.StartCoroutine(this.PreloadScene());
		base.StartCoroutine(this.LoadTitle());
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x00067456 File Offset: 0x00065656
	private IEnumerator PreloadScene()
	{
		AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Title", LoadSceneMode.Single);
		loadSceneAsync.allowSceneActivation = false;
		while (!this.allowedToSwitch)
		{
			yield return null;
		}
		loadSceneAsync.allowSceneActivation = true;
		yield break;
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x00067465 File Offset: 0x00065665
	private IEnumerator LoadTitle()
	{
		yield return new WaitForSecondsRealtime(this.loadWait);
		this.allowedToSwitch = true;
		yield break;
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x00067474 File Offset: 0x00065674
	private void Update()
	{
		bool flag = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape);
		if (!flag)
		{
			InputActionReference[] array = this.skipKeys;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].action.WasPressedThisFrame())
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			this.allowedToSwitch = true;
		}
	}

	// Token: 0x0400129F RID: 4767
	public InputActionReference[] skipKeys;

	// Token: 0x040012A0 RID: 4768
	public float loadWait = 11f;

	// Token: 0x040012A1 RID: 4769
	private bool allowedToSwitch;
}
