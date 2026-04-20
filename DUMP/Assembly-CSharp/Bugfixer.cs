using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class Bugfixer : MonoBehaviour
{
	// Token: 0x060010D1 RID: 4305 RVA: 0x00053939 File Offset: 0x00051B39
	private void Start()
	{
	}

	// Token: 0x060010D2 RID: 4306 RVA: 0x0005393C File Offset: 0x00051B3C
	private void Update()
	{
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Period))
		{
			Character target = this.GetTarget();
			if (target != null)
			{
				PhotonNetwork.Instantiate("BugfixOnYou", Vector3.zero, Quaternion.identity, 0, null).GetComponent<PhotonView>().RPC("AttachBug", RpcTarget.All, new object[]
				{
					target.photonView.ViewID
				});
			}
		}
	}

	// Token: 0x060010D3 RID: 4307 RVA: 0x000539B0 File Offset: 0x00051BB0
	private Character GetTarget()
	{
		if (this.useLocalCharacter)
		{
			return Character.localCharacter;
		}
		Character result = null;
		float num = float.MaxValue;
		foreach (Character character in Character.AllCharacters)
		{
			float num2 = Vector3.Angle(MainCamera.instance.transform.forward, character.Center - MainCamera.instance.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = character;
			}
		}
		return result;
	}

	// Token: 0x04000ED0 RID: 3792
	public bool useLocalCharacter;
}
