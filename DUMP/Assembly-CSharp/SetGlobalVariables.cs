using System;
using UnityEngine;

// Token: 0x02000333 RID: 819
public class SetGlobalVariables : MonoBehaviour
{
	// Token: 0x060015CA RID: 5578 RVA: 0x0006E82C File Offset: 0x0006CA2C
	private void Start()
	{
		foreach (StringAndFloat stringAndFloat in this.globalVariables)
		{
			PlayerPrefs.SetFloat(stringAndFloat.name, stringAndFloat.value);
		}
	}

	// Token: 0x040013D2 RID: 5074
	public StringAndFloat[] globalVariables;
}
