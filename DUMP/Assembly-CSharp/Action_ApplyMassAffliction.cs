using System;
using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class Action_ApplyMassAffliction : Action_ApplyAffliction
{
	// Token: 0x0600085F RID: 2143 RVA: 0x0002EF7C File Offset: 0x0002D17C
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0002EF9E File Offset: 0x0002D19E
	public override void RunAction()
	{
		if (this.affliction == null)
		{
			Debug.LogError("Your affliction is null bro");
			return;
		}
		Debug.Log("Running RPC");
		this.item.photonView.RPC("TryAddAfflictionToLocalCharacter", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0002EFD8 File Offset: 0x0002D1D8
	[PunRPC]
	public void TryAddAfflictionToLocalCharacter()
	{
		if (this.ignoreCaster && this.item.holderCharacter == Character.localCharacter)
		{
			return;
		}
		if (Vector3.Distance(Character.localCharacter.Center, base.transform.position) <= this.radius)
		{
			Character.localCharacter.refs.afflictions.AddAffliction(this.affliction, false);
			if (this.extraAfflictions != null)
			{
				foreach (Affliction affliction in this.extraAfflictions)
				{
					Character.localCharacter.refs.afflictions.AddAffliction(affliction, false);
				}
			}
		}
	}

	// Token: 0x04000813 RID: 2067
	public float radius;

	// Token: 0x04000814 RID: 2068
	public bool ignoreCaster;
}
