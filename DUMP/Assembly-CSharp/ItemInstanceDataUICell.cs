using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

// Token: 0x020000D6 RID: 214
public class ItemInstanceDataUICell : VisualElement
{
	// Token: 0x06000847 RID: 2119 RVA: 0x0002E9A0 File Offset: 0x0002CBA0
	public ItemInstanceDataUICell(ItemInstanceData data)
	{
		this.data = data;
		this.label = new Label();
		this.label.AddToClassList("info");
		base.Add(this.label);
		this.label.text = data.guid.ToString();
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x0002EA00 File Offset: 0x0002CC00
	public void Update()
	{
		string text = this.data.guid.ToString();
		text += string.Format(" - enteries: {0}", this.data.data.Count);
		foreach (KeyValuePair<DataEntryKey, DataEntryValue> keyValuePair in this.data.data)
		{
			text += string.Format("\n{0} : {1}", keyValuePair.Key, keyValuePair.Value.GetType().Name);
			text += "\n---";
			text = text + "\n" + keyValuePair.Value.ToString();
			text += "\n---";
		}
		this.label.text = text;
	}

	// Token: 0x04000803 RID: 2051
	private ItemInstanceData data;

	// Token: 0x04000804 RID: 2052
	private Label label;
}
