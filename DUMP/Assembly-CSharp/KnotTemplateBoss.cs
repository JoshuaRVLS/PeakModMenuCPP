using System;
using System.Collections.Generic;
using Knot;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class KnotTemplateBoss : MonoBehaviour
{
	// Token: 0x060012D7 RID: 4823 RVA: 0x0005EFD5 File Offset: 0x0005D1D5
	private void Awake()
	{
		KnotTemplateBoss.me = this;
	}

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x060012D8 RID: 4824 RVA: 0x0005EFDD File Offset: 0x0005D1DD
	// (set) Token: 0x060012D9 RID: 4825 RVA: 0x0005EFE5 File Offset: 0x0005D1E5
	public LinkedListNode<KnotTemplate> Current
	{
		get
		{
			return this.current;
		}
		set
		{
			this.displayRoot.KillAllChildren(true, false, false);
			this.current = value;
			Object.Instantiate<KnotTemplate>(this.current.Value, this.displayRoot);
		}
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x0005F013 File Offset: 0x0005D213
	private void Start()
	{
		this.templates = new LinkedList<KnotTemplate>(this.startTemplates);
		this.Current = this.templates.First;
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x0005F038 File Offset: 0x0005D238
	public void Next()
	{
		this.Current = ((this.current.Next != null) ? this.Current.Next : this.templates.First);
		Object.FindFirstObjectByType<KnotMaker>().Clear();
		Object.FindFirstObjectByType<KnotUnmaker>().grabbing = false;
		Object.FindFirstObjectByType<TiedKnotVisualizer>().Clear();
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x0005F090 File Offset: 0x0005D290
	public void Previous()
	{
		this.Current = ((this.Current.Previous != null) ? this.current.Previous : this.templates.Last);
		Object.FindFirstObjectByType<KnotMaker>().Clear();
		Object.FindFirstObjectByType<KnotUnmaker>().grabbing = false;
		Object.FindFirstObjectByType<TiedKnotVisualizer>().Clear();
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x0005F0E7 File Offset: 0x0005D2E7
	private void Update()
	{
	}

	// Token: 0x040010E8 RID: 4328
	public Transform displayRoot;

	// Token: 0x040010E9 RID: 4329
	public List<KnotTemplate> startTemplates = new List<KnotTemplate>();

	// Token: 0x040010EA RID: 4330
	public LinkedList<KnotTemplate> templates = new LinkedList<KnotTemplate>();

	// Token: 0x040010EB RID: 4331
	private LinkedListNode<KnotTemplate> current;

	// Token: 0x040010EC RID: 4332
	public static KnotTemplateBoss me;
}
