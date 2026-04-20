using System;
using UnityEngine;

// Token: 0x020002D9 RID: 729
public class PrefabTester : MonoBehaviour
{
	// Token: 0x0600145F RID: 5215 RVA: 0x000673C0 File Offset: 0x000655C0
	private void Awake()
	{
		this.instance = base.transform.GetChild(0).gameObject;
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x000673DC File Offset: 0x000655DC
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			if (this.instance != null)
			{
				Object.Destroy(this.instance);
			}
			this.instance = Object.Instantiate<GameObject>(this.prefab, base.transform.position, base.transform.rotation);
		}
	}

	// Token: 0x0400129D RID: 4765
	public GameObject prefab;

	// Token: 0x0400129E RID: 4766
	public GameObject instance;
}
