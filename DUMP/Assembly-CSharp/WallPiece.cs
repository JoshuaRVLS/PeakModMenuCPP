using System;
using UnityEngine;

// Token: 0x0200037A RID: 890
public class WallPiece : MonoBehaviour
{
	// Token: 0x0600175B RID: 5979 RVA: 0x00078914 File Offset: 0x00076B14
	public void SnapToGrid()
	{
		base.transform.position = base.GetComponentInParent<Wall>().SnapToPosition(base.transform.position);
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x00078937 File Offset: 0x00076B37
	private void Start()
	{
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x00078939 File Offset: 0x00076B39
	private void Update()
	{
	}

	// Token: 0x040015D3 RID: 5587
	public Vector2Int dimention = Vector2Int.one;

	// Token: 0x040015D4 RID: 5588
	internal Vector2Int wallPosition;
}
