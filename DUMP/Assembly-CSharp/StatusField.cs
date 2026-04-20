using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200034E RID: 846
public class StatusField : MonoBehaviour
{
	// Token: 0x06001677 RID: 5751 RVA: 0x0007288E File Offset: 0x00070A8E
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x06001678 RID: 5752 RVA: 0x000728B0 File Offset: 0x00070AB0
	public void Update()
	{
		if (!Character.localCharacter || Vector3.Distance(Character.localCharacter.Center, base.transform.position) > this.radius)
		{
			this.inflicting = false;
			return;
		}
		if (this.doNotApplyIfStatusesMaxed && Character.localCharacter.refs.afflictions.statusSum >= 1f)
		{
			this.inflicting = false;
			return;
		}
		Character.localCharacter.refs.afflictions.AdjustStatus(this.statusType, this.statusAmountPerSecond * Time.deltaTime, false);
		foreach (StatusField.StatusFieldStatus statusFieldStatus in this.additionalStatuses)
		{
			Character.localCharacter.refs.afflictions.AdjustStatus(statusFieldStatus.statusType, statusFieldStatus.statusAmountPerSecond * Time.deltaTime, false);
		}
		if (!this.inflicting && this.statusAmountOnEntry != 0f && Time.time - this.lastEnteredTime > this.entryCooldown)
		{
			Character.localCharacter.refs.afflictions.AdjustStatus(this.statusType, this.statusAmountOnEntry, false);
			this.lastEnteredTime = Time.time;
		}
		this.inflicting = true;
	}

	// Token: 0x040014B9 RID: 5305
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x040014BA RID: 5306
	public float statusAmountPerSecond;

	// Token: 0x040014BB RID: 5307
	public float statusAmountOnEntry;

	// Token: 0x040014BC RID: 5308
	public float radius;

	// Token: 0x040014BD RID: 5309
	private float lastEnteredTime;

	// Token: 0x040014BE RID: 5310
	public float entryCooldown = 1f;

	// Token: 0x040014BF RID: 5311
	public bool doNotApplyIfStatusesMaxed;

	// Token: 0x040014C0 RID: 5312
	public List<StatusField.StatusFieldStatus> additionalStatuses;

	// Token: 0x040014C1 RID: 5313
	private bool inflicting;

	// Token: 0x0200053F RID: 1343
	[Serializable]
	public class StatusFieldStatus
	{
		// Token: 0x04001CBB RID: 7355
		public CharacterAfflictions.STATUSTYPE statusType;

		// Token: 0x04001CBC RID: 7356
		public float statusAmountPerSecond;
	}
}
