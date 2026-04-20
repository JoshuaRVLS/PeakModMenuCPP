using System;
using System.Collections.Generic;
using Peak.Dev;
using UnityEngine;
using UnityEngine.UIElements;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000166 RID: 358
public class ReconnectDataDebugPage : DebugPage
{
	// Token: 0x06000BB7 RID: 2999 RVA: 0x0003EC3B File Offset: 0x0003CE3B
	public ReconnectDataDebugPage()
	{
		this.label = new Label();
		this.label.AddToClassList("info");
		base.Add(this.label);
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x0003EC6C File Offset: 0x0003CE6C
	public override void Update()
	{
		base.Update();
		Object instance = Singleton<ReconnectHandler>.Instance;
		string text = "";
		if (instance != null)
		{
			text = string.Format("Reconnect Data found: {0}", Singleton<ReconnectHandler>.Instance.Records.Count);
			using (IEnumerator<KeyValuePair<string, ReconnectHandler.ReconnectDataRecord>> enumerator = Singleton<ReconnectHandler>.Instance.Records.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, ReconnectHandler.ReconnectDataRecord> keyValuePair = enumerator.Current;
					string text2;
					ReconnectHandler.ReconnectDataRecord reconnectDataRecord;
					keyValuePair.Deconstruct(out text2, out reconnectDataRecord);
					string text3 = text2;
					ReconnectHandler.ReconnectDataRecord reconnectDataRecord2 = reconnectDataRecord;
					text = string.Concat(new string[]
					{
						text,
						Environment.NewLine,
						text3,
						" : ",
						Pretty.Print(reconnectDataRecord2.Data.currentStatuses)
					});
				}
				goto IL_B7;
			}
		}
		text = "No reconnect handler found";
		IL_B7:
		this.label.text = text;
	}

	// Token: 0x04000AC3 RID: 2755
	private Label label;
}
