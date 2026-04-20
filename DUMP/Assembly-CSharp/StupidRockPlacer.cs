using System;
using System.Collections.Generic;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000354 RID: 852
public class StupidRockPlacer : MonoBehaviour
{
	// Token: 0x1700016B RID: 363
	// (get) Token: 0x0600168B RID: 5771 RVA: 0x000736F7 File Offset: 0x000718F7
	public Vector3 size
	{
		get
		{
			return base.transform.localScale.xyz();
		}
	}

	// Token: 0x0600168C RID: 5772 RVA: 0x00073709 File Offset: 0x00071909
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(base.transform.position + this.size / 2f, this.size);
	}

	// Token: 0x0600168D RID: 5773 RVA: 0x00073736 File Offset: 0x00071936
	public void Clear()
	{
		if (this.rockParent)
		{
			this.rockParent.KillAllChildren(true, false, true);
		}
	}

	// Token: 0x0600168E RID: 5774 RVA: 0x00073753 File Offset: 0x00071953
	private void Start()
	{
	}

	// Token: 0x0600168F RID: 5775 RVA: 0x00073758 File Offset: 0x00071958
	private void ValidatePool()
	{
		foreach (Transform transform in (from t in this.pieceRoot.GetComponentsInChildren<Transform>()
		where t != this.pieceRoot
		select t).ToList<Transform>())
		{
			transform.gameObject.GetOrAddComponent<PutMeInWall>();
			transform.gameObject.layer = LayerMask.NameToLayer("Terrain");
			PExt.DirtyObj(transform.gameObject);
		}
	}

	// Token: 0x06001690 RID: 5776 RVA: 0x000737EC File Offset: 0x000719EC
	public void Go()
	{
		this.rockParent = null;
		this.rockParent = base.transform.parent.Find("Rocks: " + base.gameObject.name);
		if (!this.rockParent)
		{
			this.rockParent = new GameObject("Rocks: " + base.gameObject.name).transform;
			this.rockParent.SetParent(base.transform.parent);
		}
		this.rockParent.SetSiblingIndex(base.transform.GetSiblingIndex() + 1);
		this.rocks = (from x in this.pieceRoot.GetComponentsInChildren<PutMeInWall>()
		select x.gameObject).ToList<GameObject>();
		this.lastPlaced = new List<GameObject>();
		int num = 0;
		int num2 = 0;
		while (num2 < this.amount || num > this.amount * 10)
		{
			num++;
			Vector3 startCast = base.transform.position + new Vector3(this.size.x.Rand(), this.size.y.Rand(), 0f);
			GameObject random = this.rocks.GetRandom<GameObject>();
			Vector3? wallPosition = random.GetComponent<PutMeInWall>().GetWallPosition(startCast, base.transform.localScale.z);
			if (wallPosition == null)
			{
				num2--;
			}
			else
			{
				GameObject gameObject = Object.Instantiate<GameObject>(random, wallPosition.Value, ExtQuaternion.RandomRotation());
				gameObject.transform.SetParent(this.rockParent);
				PutMeInWall putMeInWall;
				if (!gameObject.TryGetComponent<PutMeInWall>(out putMeInWall))
				{
					putMeInWall = gameObject.AddComponent<PutMeInWall>();
				}
				putMeInWall.gameObject.SetActive(true);
				this.lastPlaced.Add(gameObject);
				putMeInWall.RandomScale();
				Physics.SyncTransforms();
				PExt.DirtyObj(gameObject);
			}
			num2++;
		}
	}

	// Token: 0x06001691 RID: 5777 RVA: 0x000739D8 File Offset: 0x00071BD8
	public void RemoveLastPlaced()
	{
		foreach (GameObject x in this.lastPlaced)
		{
			x == null;
		}
		this.lastPlaced = new List<GameObject>();
	}

	// Token: 0x06001692 RID: 5778 RVA: 0x00073A38 File Offset: 0x00071C38
	private void Update()
	{
	}

	// Token: 0x040014F0 RID: 5360
	public List<GameObject> rocks;

	// Token: 0x040014F1 RID: 5361
	public Transform pieceRoot;

	// Token: 0x040014F2 RID: 5362
	public int amount = 10;

	// Token: 0x040014F3 RID: 5363
	public Transform rockParent;

	// Token: 0x040014F4 RID: 5364
	public List<GameObject> lastPlaced = new List<GameObject>();
}
