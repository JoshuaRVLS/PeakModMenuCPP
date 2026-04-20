using System;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class PlayerTrackParticles : MonoBehaviour
{
	// Token: 0x06000BA8 RID: 2984 RVA: 0x0003E8FA File Offset: 0x0003CAFA
	private void Start()
	{
		if (this.bounds.center != base.transform.position)
		{
			this.bounds.center = base.transform.position;
		}
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x0003E930 File Offset: 0x0003CB30
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		this.inBounds = this.bounds.Contains(Character.observedCharacter.Center);
		if (!this.inBounds)
		{
			return;
		}
		if (Vector3.Distance(this.lastPlayerPos, Character.observedCharacter.Center) > this.repositionDelta)
		{
			Vector3 vector = this.fx.transform.position - this.positionOffset;
			if (this.x)
			{
				vector = new Vector3(Character.observedCharacter.Center.x, vector.y, vector.z);
			}
			if (this.y)
			{
				vector = new Vector3(vector.x, Character.observedCharacter.Center.y, vector.z);
			}
			if (this.z)
			{
				vector = new Vector3(vector.x, vector.y, Character.observedCharacter.Center.z);
			}
			this.fx.transform.position = vector + this.positionOffset;
			this.lastPlayerPos = Character.observedCharacter.Center;
		}
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0003EA58 File Offset: 0x0003CC58
	private void OnDrawGizmosSelected()
	{
		if (this.bounds.center != base.transform.position)
		{
			this.bounds.center = base.transform.position;
		}
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x04000AB7 RID: 2743
	public Bounds bounds;

	// Token: 0x04000AB8 RID: 2744
	public GameObject fx;

	// Token: 0x04000AB9 RID: 2745
	[Header("Track Axis")]
	public bool x;

	// Token: 0x04000ABA RID: 2746
	public bool y;

	// Token: 0x04000ABB RID: 2747
	public bool z;

	// Token: 0x04000ABC RID: 2748
	public float repositionDelta = 50f;

	// Token: 0x04000ABD RID: 2749
	private Vector3 lastPlayerPos = Vector3.positiveInfinity;

	// Token: 0x04000ABE RID: 2750
	public bool inBounds;

	// Token: 0x04000ABF RID: 2751
	public Vector3 positionOffset;
}
