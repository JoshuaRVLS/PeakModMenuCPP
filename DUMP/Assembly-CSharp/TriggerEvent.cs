using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000369 RID: 873
public class TriggerEvent : MonoBehaviour
{
	// Token: 0x06001707 RID: 5895 RVA: 0x00076533 File Offset: 0x00074733
	private void Start()
	{
		this.view = base.GetComponentInParent<PhotonView>();
	}

	// Token: 0x06001708 RID: 5896 RVA: 0x00076544 File Offset: 0x00074744
	private void OnTriggerEnter(Collider other)
	{
		TriggerEvent.<>c__DisplayClass9_0 CS$<>8__locals1 = new TriggerEvent.<>c__DisplayClass9_0();
		CS$<>8__locals1.<>4__this = this;
		if (this.onlyOnce && (this.hasActivated || this.hasTriggered))
		{
			return;
		}
		if (other.isTrigger)
		{
			return;
		}
		if (!CharacterRagdoll.TryGetCharacterFromCollider(other, out CS$<>8__locals1.player))
		{
			return;
		}
		if (this.hits.Contains(CS$<>8__locals1.player))
		{
			return;
		}
		base.StartCoroutine(CS$<>8__locals1.<OnTriggerEnter>g__IHoldHit|0());
		this.TriggerEntered();
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x000765B8 File Offset: 0x000747B8
	public void TriggerEntered()
	{
		if (this.onlyOnce && (this.hasActivated || this.hasTriggered))
		{
			return;
		}
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.triggerChance < Random.value)
		{
			return;
		}
		this.hasActivated = true;
		this.view.RPC("RPCA_Trigger", RpcTarget.All, new object[]
		{
			base.transform.GetSiblingIndex()
		});
	}

	// Token: 0x0600170A RID: 5898 RVA: 0x0007662B File Offset: 0x0007482B
	public void Trigger()
	{
		GameUtils.instance.StartCoroutine(this.TriggerRoutine());
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x0007663E File Offset: 0x0007483E
	private IEnumerator TriggerRoutine()
	{
		if (this.onlyOnce && this.hasTriggered)
		{
			yield break;
		}
		if (this.waitForRenderFrame)
		{
			yield return new WaitForEndOfFrame();
		}
		this.triggerEvent.Invoke();
		this.hasActivated = true;
		this.hasTriggered = true;
		yield break;
	}

	// Token: 0x04001578 RID: 5496
	[Range(0f, 1f)]
	public float triggerChance = 1f;

	// Token: 0x04001579 RID: 5497
	public bool onlyOnce;

	// Token: 0x0400157A RID: 5498
	public bool waitForRenderFrame = true;

	// Token: 0x0400157B RID: 5499
	public UnityEvent triggerEvent;

	// Token: 0x0400157C RID: 5500
	private PhotonView view;

	// Token: 0x0400157D RID: 5501
	private bool hasActivated;

	// Token: 0x0400157E RID: 5502
	private bool hasTriggered;

	// Token: 0x0400157F RID: 5503
	private List<Character> hits = new List<Character>();
}
