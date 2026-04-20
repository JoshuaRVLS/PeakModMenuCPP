using System;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class FogCutoutHandler : MonoBehaviour
{
	// Token: 0x0600070C RID: 1804 RVA: 0x000287BA File Offset: 0x000269BA
	public void debugCurrentCutoutZone()
	{
		this.setFogCutoutZone(this.index);
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x000287C8 File Offset: 0x000269C8
	private void OnEnable()
	{
		this.setFogCutoutZone(0);
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x000287D4 File Offset: 0x000269D4
	private void Update()
	{
		if (Character.localCharacter && Character.localCharacter.Center.z > this.currentCutoutZone.transform.position.z + this.currentCutoutZone.transitionPoint && this.index < this.cutoutZones.Length)
		{
			this.setFogCutoutZone(this.index);
			this.index++;
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x0002884C File Offset: 0x00026A4C
	public void setFogCutoutZone(int zone)
	{
		FogCutoutHandler.<>c__DisplayClass7_0 CS$<>8__locals1 = new FogCutoutHandler.<>c__DisplayClass7_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.zone = zone;
		base.StartCoroutine(CS$<>8__locals1.<setFogCutoutZone>g__changeZoneRoutine|0());
		this.currentCutoutZone = this.cutoutZones[CS$<>8__locals1.zone];
	}

	// Token: 0x0400070E RID: 1806
	public FogCutoutZone[] cutoutZones;

	// Token: 0x0400070F RID: 1807
	private FogCutoutZone currentCutoutZone;

	// Token: 0x04000710 RID: 1808
	public int index;

	// Token: 0x04000711 RID: 1809
	public float fadeTime = 1f;
}
