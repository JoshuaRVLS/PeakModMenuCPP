using System;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000332 RID: 818
public class SetCharacterPosition : MonoBehaviour
{
	// Token: 0x060015C6 RID: 5574 RVA: 0x0006E7F8 File Offset: 0x0006C9F8
	private void Go()
	{
		this.characterPrefab.transform.position = base.transform.position;
		PExt.SaveObj(this.characterPrefab);
	}

	// Token: 0x060015C7 RID: 5575 RVA: 0x0006E820 File Offset: 0x0006CA20
	private void Start()
	{
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x0006E822 File Offset: 0x0006CA22
	private void Update()
	{
	}

	// Token: 0x040013D1 RID: 5073
	public GameObject characterPrefab;
}
