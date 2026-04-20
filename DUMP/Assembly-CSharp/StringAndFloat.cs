using System;

// Token: 0x02000334 RID: 820
[Serializable]
public class StringAndFloat
{
	// Token: 0x060015CC RID: 5580 RVA: 0x0006E86B File Offset: 0x0006CA6B
	public StringAndFloat(string name, float value)
	{
		this.name = name;
		this.value = value;
	}

	// Token: 0x040013D3 RID: 5075
	public string name;

	// Token: 0x040013D4 RID: 5076
	public float value;
}
