using System;
using UnityEngine;

// Token: 0x020002AC RID: 684
public class MirrorReflectionScript : MonoBehaviour
{
	// Token: 0x06001364 RID: 4964 RVA: 0x0006296F File Offset: 0x00060B6F
	private void Start()
	{
		this.childScript = base.gameObject.transform.parent.gameObject.GetComponentInChildren<MirrorCameraScript>();
		if (this.childScript == null)
		{
			Debug.LogError("Child script (MirrorCameraScript) should be in sibling object");
		}
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x000629A9 File Offset: 0x00060BA9
	private void OnWillRenderObject()
	{
		this.childScript.RenderMirror();
	}

	// Token: 0x040011A7 RID: 4519
	private MirrorCameraScript childScript;
}
