using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000211 RID: 529
public class Balloon : ItemComponent
{
	// Token: 0x0600104A RID: 4170 RVA: 0x00050F08 File Offset: 0x0004F108
	public void Start()
	{
		base.StartCoroutine(this.InitColorYieldRoutine());
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x00050F18 File Offset: 0x0004F118
	private void InitColor()
	{
		if (base.HasData(DataEntryKey.Color))
		{
			this.colorIndex = base.GetData<IntItemData>(DataEntryKey.Color).Value;
			this.SetColor(this.colorIndex);
			return;
		}
		if (this.photonView.IsMine)
		{
			this.RandomizeColor();
			this.photonView.RPC("RPC_SyncColor", RpcTarget.All, new object[]
			{
				this.colorIndex
			});
		}
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x00050F88 File Offset: 0x0004F188
	private void SetColor(int index)
	{
		this.r.sharedMaterial = Character.localCharacter.refs.balloons.balloonColors[this.colorIndex];
		this.item.UIData.icon = this.icons[this.colorIndex];
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x00050FD8 File Offset: 0x0004F1D8
	private void RandomizeColor()
	{
		Material[] balloonColors = Character.localCharacter.refs.balloons.balloonColors;
		this.colorIndex = Random.Range(0, balloonColors.Length);
		this.SetColor(this.colorIndex);
		base.GetData<IntItemData>(DataEntryKey.Color).Value = this.colorIndex;
	}

	// Token: 0x0600104E RID: 4174 RVA: 0x00051028 File Offset: 0x0004F228
	[PunRPC]
	public void RPC_SyncColor(int colorIndex)
	{
		this.colorIndex = colorIndex;
		this.SetColor(this.colorIndex);
		base.GetData<IntItemData>(DataEntryKey.Color).Value = colorIndex;
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x0005104B File Offset: 0x0004F24B
	public override void OnInstanceDataSet()
	{
		base.StartCoroutine(this.InitColorYieldRoutine());
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x0005105A File Offset: 0x0004F25A
	private IEnumerator InitColorYieldRoutine()
	{
		while (!Character.localCharacter)
		{
			yield return null;
		}
		if (!this.isBunch)
		{
			this.InitColor();
		}
		yield break;
	}

	// Token: 0x04000E36 RID: 3638
	public new Item item;

	// Token: 0x04000E37 RID: 3639
	public Renderer r;

	// Token: 0x04000E38 RID: 3640
	public Texture2D[] icons;

	// Token: 0x04000E39 RID: 3641
	public int colorIndex;

	// Token: 0x04000E3A RID: 3642
	public bool isBunch;

	// Token: 0x04000E3B RID: 3643
	public GameObject popParticle;
}
