using System;

// Token: 0x02000085 RID: 133
public abstract class GameService
{
	// Token: 0x060005B1 RID: 1457 RVA: 0x00020D2C File Offset: 0x0001EF2C
	public virtual void ClearSessionData()
	{
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x00020D2E File Offset: 0x0001EF2E
	public virtual void ClearSceneData()
	{
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x00020D30 File Offset: 0x0001EF30
	public virtual void Update()
	{
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x00020D32 File Offset: 0x0001EF32
	public virtual void OnDestroy()
	{
	}
}
