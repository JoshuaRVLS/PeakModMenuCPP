using System;

// Token: 0x02000075 RID: 117
[Serializable]
public class LoginResponse
{
	// Token: 0x0400059D RID: 1437
	public bool VersionOkay;

	// Token: 0x0400059E RID: 1438
	public int HoursUntilLevel;

	// Token: 0x0400059F RID: 1439
	public int MinutesUntilLevel;

	// Token: 0x040005A0 RID: 1440
	public int SecondsUntilLevel;

	// Token: 0x040005A1 RID: 1441
	public int LevelIndex;

	// Token: 0x040005A2 RID: 1442
	public string Message;
}
