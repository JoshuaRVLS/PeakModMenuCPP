using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class ItemActionBase : MonoBehaviourPun
{
	// Token: 0x1700009B RID: 155
	// (get) Token: 0x0600094B RID: 2379 RVA: 0x00032333 File Offset: 0x00030533
	[SerializeField]
	protected Character character
	{
		get
		{
			return this.item.holderCharacter;
		}
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x00032340 File Offset: 0x00030540
	public virtual void RunAction()
	{
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x00032342 File Offset: 0x00030542
	protected virtual void OnEnable()
	{
		this.Init();
		this.Subscribe();
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x00032350 File Offset: 0x00030550
	protected virtual void Start()
	{
		this.Unsubscribe();
		this.Subscribe();
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x0003235E File Offset: 0x0003055E
	public void OnDisable()
	{
		this.Unsubscribe();
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x00032366 File Offset: 0x00030566
	protected virtual void Subscribe()
	{
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x00032368 File Offset: 0x00030568
	protected virtual void Unsubscribe()
	{
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0003236A File Offset: 0x0003056A
	private void Init()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x040008BB RID: 2235
	protected Item item;
}
