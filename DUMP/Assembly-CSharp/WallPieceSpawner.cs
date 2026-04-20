using System;
using UnityEngine;

// Token: 0x0200037B RID: 891
public class WallPieceSpawner : MonoBehaviour
{
	// Token: 0x0600175F RID: 5983 RVA: 0x00078950 File Offset: 0x00076B50
	private void Go()
	{
		this.wall = base.GetComponent<Wall>();
		this.wall.WallInit();
		this.root = base.transform.Find("Pieces");
		this.Clear();
		for (int i = 0; i < 50; i++)
		{
			this.DoSpawns();
		}
	}

	// Token: 0x06001760 RID: 5984 RVA: 0x000789A4 File Offset: 0x00076BA4
	private void DoSpawns()
	{
		for (int i = 0; i < this.wall.gridSize.x; i++)
		{
			for (int j = 0; j < this.wall.gridSize.y; j++)
			{
				WallPiece randomPiece = this.GetRandomPiece();
				if (this.wall.PieceFits(randomPiece, i, j))
				{
					this.SpawnPiece(randomPiece, i, j);
				}
			}
		}
	}

	// Token: 0x06001761 RID: 5985 RVA: 0x00078A08 File Offset: 0x00076C08
	private void SpawnPiece(WallPiece piece, int x, int y)
	{
		WallPiece component = HelperFunctions.SpawnPrefab(piece.gameObject, this.wall.GetGridPos(x, y), Quaternion.identity, this.root).GetComponent<WallPiece>();
		component.wallPosition = new Vector2Int(x, y);
		this.wall.AddPiece(component);
	}

	// Token: 0x06001762 RID: 5986 RVA: 0x00078A57 File Offset: 0x00076C57
	private WallPiece GetRandomPiece()
	{
		return this.pieces[Random.Range(0, this.pieces.Length)];
	}

	// Token: 0x06001763 RID: 5987 RVA: 0x00078A70 File Offset: 0x00076C70
	private void Clear()
	{
		this.root = base.transform.Find("Pieces");
		for (int i = this.root.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(this.root.GetChild(i).gameObject);
		}
	}

	// Token: 0x040015D5 RID: 5589
	public WallPiece[] pieces;

	// Token: 0x040015D6 RID: 5590
	private Transform root;

	// Token: 0x040015D7 RID: 5591
	private Wall wall;
}
