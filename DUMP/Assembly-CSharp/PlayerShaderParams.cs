using System;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class PlayerShaderParams : MonoBehaviour
{
	// Token: 0x06000BA6 RID: 2982 RVA: 0x0003E8BE File Offset: 0x0003CABE
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		Shader.SetGlobalVector("PlayerPos", Character.localCharacter.Center + this.playerCenterOffset);
	}

	// Token: 0x04000AB6 RID: 2742
	public Vector3 playerCenterOffset;
}
