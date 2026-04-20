using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000378 RID: 888
public class Wall : MonoBehaviour
{
	// Token: 0x06001749 RID: 5961 RVA: 0x00078493 File Offset: 0x00076693
	internal void WallInit()
	{
		this.pieces = new List<WallPiece>();
	}

	// Token: 0x0600174A RID: 5962 RVA: 0x000784A0 File Offset: 0x000766A0
	internal void AddPiece(WallPiece piece)
	{
		this.pieces.Add(piece);
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x000784B0 File Offset: 0x000766B0
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
		for (int i = 0; i < this.gridSize.x; i++)
		{
			for (int j = 0; j < this.gridSize.y; j++)
			{
				Gizmos.DrawWireCube(this.GetGridPos(i, j), new Vector3(this.gridCellSize, this.gridCellSize, 0.25f));
			}
		}
	}

	// Token: 0x0600174C RID: 5964 RVA: 0x0007852C File Offset: 0x0007672C
	internal Vector3 GetGridPos(int x, int y)
	{
		Vector2 a = (this.gridSize - Vector2.one) * this.gridCellSize;
		Vector2 vector = base.transform.position - a * 0.5f;
		Vector2 vector2 = base.transform.position + a * 0.5f;
		float t = (float)x / ((float)this.gridSize.x - 1f);
		float t2 = (float)y / ((float)this.gridSize.y - 1f);
		return new Vector3(Mathf.Lerp(vector.x, vector2.x, t), Mathf.Lerp(vector.y, vector2.y, t2), base.transform.position.z);
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x00078604 File Offset: 0x00076804
	internal Vector3 SnapToPosition(Vector3 position)
	{
		Vector2 a = (this.gridSize - Vector2.one) * this.gridCellSize;
		Vector2 vector = base.transform.position - a * 0.5f;
		Vector2 vector2 = base.transform.position + a * 0.5f;
		float num = Mathf.InverseLerp(vector.x, vector2.x, position.x);
		float num2 = Mathf.InverseLerp(vector.y, vector2.y, position.y);
		int x = Mathf.RoundToInt(num * ((float)this.gridSize.x - 1f));
		int y = Mathf.RoundToInt(num2 * ((float)this.gridSize.y - 1f));
		return this.GetGridPos(x, y);
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x000786E0 File Offset: 0x000768E0
	internal bool PieceFits(WallPiece piece, int x, int y)
	{
		foreach (WallPiece existing in this.pieces)
		{
			if (this.CollisionCheck(piece, x, y, existing))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x00078740 File Offset: 0x00076940
	private bool CollisionCheck(WallPiece newPiece, int newPosX, int newPosY, WallPiece existing)
	{
		for (int i = 0; i < newPiece.dimention.x; i++)
		{
			for (int j = 0; j < newPiece.dimention.y; j++)
			{
				Vector2Int checkPos = new Vector2Int(newPosX + i, newPosY + j);
				if (this.CollisionCheckSpot(checkPos, existing))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001750 RID: 5968 RVA: 0x00078794 File Offset: 0x00076994
	private bool CollisionCheckSpot(Vector2Int checkPos, WallPiece existing)
	{
		for (int i = 0; i < existing.dimention.x; i++)
		{
			for (int j = 0; j < existing.dimention.y; j++)
			{
				if (new Vector2Int(existing.wallPosition.x + i, existing.wallPosition.y + j) == checkPos)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x040015CB RID: 5579
	public Vector2Int gridSize;

	// Token: 0x040015CC RID: 5580
	public float gridCellSize;

	// Token: 0x040015CD RID: 5581
	public List<WallPiece> pieces = new List<WallPiece>();
}
