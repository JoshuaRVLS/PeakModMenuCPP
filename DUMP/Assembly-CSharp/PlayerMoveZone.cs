using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000B3 RID: 179
public class PlayerMoveZone : MonoBehaviour
{
	// Token: 0x060006BB RID: 1723 RVA: 0x00026B14 File Offset: 0x00024D14
	private void Awake()
	{
		this.zoneBounds.center = base.transform.position;
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x00026B2C File Offset: 0x00024D2C
	private void Start()
	{
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x00026B2E File Offset: 0x00024D2E
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		this.characterInsideBounds = this.zoneBounds.Contains(Character.observedCharacter.Center);
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x00026B59 File Offset: 0x00024D59
	private void FixedUpdate()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (this.characterInsideBounds && Character.observedCharacter == Character.localCharacter)
		{
			this.AddForceToCharacter();
		}
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x00026B88 File Offset: 0x00024D88
	private void AddForceToCharacter()
	{
		Character.localCharacter.AddForce(this.forceDirection * this.Force, 0.5f, 1f);
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00026BB0 File Offset: 0x00024DB0
	private void OnDrawGizmosSelected()
	{
		this.zoneBounds.center = base.transform.position;
		Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
		Gizmos.DrawCube(this.zoneBounds.center, this.zoneBounds.extents * 2f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position, base.transform.position + this.forceDirection * this.Force);
	}

	// Token: 0x040006B3 RID: 1715
	[FormerlySerializedAs("windZoneBounds")]
	public Bounds zoneBounds;

	// Token: 0x040006B4 RID: 1716
	public Vector3 forceDirection;

	// Token: 0x040006B5 RID: 1717
	[FormerlySerializedAs("windForce")]
	[SerializeField]
	private float Force;

	// Token: 0x040006B6 RID: 1718
	public bool characterInsideBounds;
}
