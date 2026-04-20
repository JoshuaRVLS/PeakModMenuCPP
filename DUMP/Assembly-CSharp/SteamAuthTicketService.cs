using System;
using System.Text;
using Steamworks;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001B7 RID: 439
public class SteamAuthTicketService : GameService
{
	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000E11 RID: 3601 RVA: 0x00046E95 File Offset: 0x00045095
	public Optionable<SteamAuthTicketService.GeneratedTicket> CurrentTicket
	{
		get
		{
			return this.m_currentTicket;
		}
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x00046E9D File Offset: 0x0004509D
	public override void Update()
	{
		base.Update();
		this.VerifyHasValidTicket();
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x00046EAC File Offset: 0x000450AC
	public void VerifyHasValidTicket()
	{
		if (this.m_currentTicket.IsNone || (this.m_currentTicket.IsSome && this.m_currentTicket.Value.TimePassed > 60f))
		{
			this.GenerateNewTicket();
		}
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x00046EF4 File Offset: 0x000450F4
	private void GenerateNewTicket()
	{
		if (this.m_currentTicket.IsSome)
		{
			this.CancelSteamTicket(false);
		}
		ValueTuple<string, HAuthTicket> steamAuthTicket = SteamAuthTicketService.GetSteamAuthTicket();
		string item = steamAuthTicket.Item1;
		HAuthTicket item2 = steamAuthTicket.Item2;
		SteamAuthTicketService.GeneratedTicket value = new SteamAuthTicketService.GeneratedTicket(item2, item);
		this.m_currentTicket = Optionable<SteamAuthTicketService.GeneratedTicket>.Some(value);
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x00046F3C File Offset: 0x0004513C
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.CancelSteamTicket(true);
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x00046F4C File Offset: 0x0004514C
	public void CancelSteamTicket(bool immediate)
	{
		if (this.m_currentTicket.IsSome)
		{
			SteamAuthTicketService.<>c__DisplayClass9_0 CS$<>8__locals1 = new SteamAuthTicketService.<>c__DisplayClass9_0();
			CS$<>8__locals1.ticket = this.m_currentTicket.Value.Ticket;
			this.m_currentTicket = Optionable<SteamAuthTicketService.GeneratedTicket>.None;
			if (immediate)
			{
				SteamUser.CancelAuthTicket(CS$<>8__locals1.ticket);
				return;
			}
			GameHandler.Instance.StartCoroutine(CS$<>8__locals1.<CancelSteamTicket>g__CancelOverTime|0());
		}
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x00046FB0 File Offset: 0x000451B0
	public static ValueTuple<string, HAuthTicket> GetSteamAuthTicket()
	{
		byte[] array = new byte[1024];
		CSteamID steamID = SteamUser.GetSteamID();
		SteamNetworkingIdentity steamNetworkingIdentity = default(SteamNetworkingIdentity);
		steamNetworkingIdentity.SetSteamID(steamID);
		uint num;
		HAuthTicket authSessionTicket = SteamUser.GetAuthSessionTicket(array, array.Length, out num, ref steamNetworkingIdentity);
		Array.Resize<byte>(ref array, (int)num);
		StringBuilder stringBuilder = new StringBuilder();
		int num2 = 0;
		while ((long)num2 < (long)((ulong)num))
		{
			stringBuilder.AppendFormat("{0:x2}", array[num2]);
			num2++;
		}
		return new ValueTuple<string, HAuthTicket>(stringBuilder.ToString(), authSessionTicket);
	}

	// Token: 0x04000BE5 RID: 3045
	private Optionable<SteamAuthTicketService.GeneratedTicket> m_currentTicket;

	// Token: 0x04000BE6 RID: 3046
	private const float TICKET_MAX_LIFETIME = 60f;

	// Token: 0x020004CC RID: 1228
	public struct GeneratedTicket
	{
		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06001D8A RID: 7562 RVA: 0x0008A740 File Offset: 0x00088940
		// (set) Token: 0x06001D8B RID: 7563 RVA: 0x0008A748 File Offset: 0x00088948
		public string TicketData { readonly get; private set; }

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06001D8C RID: 7564 RVA: 0x0008A751 File Offset: 0x00088951
		// (set) Token: 0x06001D8D RID: 7565 RVA: 0x0008A759 File Offset: 0x00088959
		public HAuthTicket Ticket { readonly get; private set; }

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06001D8E RID: 7566 RVA: 0x0008A762 File Offset: 0x00088962
		// (set) Token: 0x06001D8F RID: 7567 RVA: 0x0008A76A File Offset: 0x0008896A
		public float TimeCreated { readonly get; private set; }

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06001D90 RID: 7568 RVA: 0x0008A773 File Offset: 0x00088973
		public float TimePassed
		{
			get
			{
				return Time.realtimeSinceStartup - this.TimeCreated;
			}
		}

		// Token: 0x06001D91 RID: 7569 RVA: 0x0008A781 File Offset: 0x00088981
		public GeneratedTicket(HAuthTicket ticket, string data)
		{
			this.TicketData = data;
			this.TimeCreated = Time.realtimeSinceStartup;
			this.Ticket = ticket;
		}
	}
}
