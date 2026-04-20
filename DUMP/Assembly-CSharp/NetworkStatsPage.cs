using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UIElements;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x0200014D RID: 333
public class NetworkStatsPage : DebugPage
{
	// Token: 0x06000B1C RID: 2844 RVA: 0x0003BC84 File Offset: 0x00039E84
	public NetworkStatsPage(NetworkStats networkStats)
	{
		this.stats = networkStats;
		base.styleSheets.Add(SingletonAsset<CoreGlobalDependencies>.Instance.DebugPageStyleSheets);
		PhotonNetwork.NetworkStatisticsEnabled = true;
		this.m_label = new Label();
		this.m_label.AddToClassList("info");
		base.Add(this.m_label);
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0003BCE4 File Offset: 0x00039EE4
	public override void Update()
	{
		base.Update();
		string text = string.Concat(new string[]
		{
			"bytes in: ",
			NetworkStatsPage.ToPrettySize(this.stats.m_lastRecievedDelta * 8L, 0),
			"/s, out: ",
			NetworkStatsPage.ToPrettySize(this.stats.m_lastSentDelta * 8L, 0),
			"/s"
		});
		List<ValueTuple<string, ulong>> bytesDeltaSent = this.stats.GetBytesDeltaSent();
		bytesDeltaSent.Sort((ValueTuple<string, ulong> t1, ValueTuple<string, ulong> t2) => t2.Item2.CompareTo(t1.Item2));
		foreach (ValueTuple<string, ulong> valueTuple in bytesDeltaSent)
		{
			string item = valueTuple.Item1;
			text = string.Concat(new string[]
			{
				text,
				Environment.NewLine,
				item,
				": ",
				NetworkStatsPage.ToPrettySize((long)valueTuple.Item2, 0),
				"/s"
			});
		}
		this.m_label.text = text;
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x0003BE00 File Offset: 0x0003A000
	public static string ToPrettySize(long value, int decimalPlaces = 0)
	{
		double num = Math.Round((double)value / 1099511627776.0, decimalPlaces);
		double num2 = Math.Round((double)value / 1073741824.0, decimalPlaces);
		double num3 = Math.Round((double)value / 1048576.0, decimalPlaces);
		double num4 = Math.Round((double)value / 1024.0, decimalPlaces);
		if (num > 1.0)
		{
			return string.Format("{0}Tb", num);
		}
		if (num2 > 1.0)
		{
			return string.Format("{0}Gb", num2);
		}
		if (num3 > 1.0)
		{
			return string.Format("{0}Mb", num3);
		}
		if (num4 <= 1.0)
		{
			return string.Format("{0}B", Math.Round((double)value, decimalPlaces));
		}
		return string.Format("{0}Kb", num4);
	}

	// Token: 0x04000A39 RID: 2617
	private Label m_label;

	// Token: 0x04000A3A RID: 2618
	private NetworkStats stats;

	// Token: 0x04000A3B RID: 2619
	private const long OneKb = 1024L;

	// Token: 0x04000A3C RID: 2620
	private const long OneMb = 1048576L;

	// Token: 0x04000A3D RID: 2621
	private const long OneGb = 1073741824L;

	// Token: 0x04000A3E RID: 2622
	private const long OneTb = 1099511627776L;
}
