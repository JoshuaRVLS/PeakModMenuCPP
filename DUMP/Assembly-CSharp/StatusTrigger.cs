using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x0200034F RID: 847
public class StatusTrigger : MonoBehaviour
{
	// Token: 0x0600167A RID: 5754 RVA: 0x00072A23 File Offset: 0x00070C23
	private void Update()
	{
		this.counter += Time.deltaTime;
	}

	// Token: 0x0600167B RID: 5755 RVA: 0x00072A38 File Offset: 0x00070C38
	private void OnTriggerEnter(Collider other)
	{
		Character character;
		if (!CharacterRagdoll.TryGetCharacterFromCollider(other, out character))
		{
			return;
		}
		if (!character.IsLocal)
		{
			return;
		}
		if (this.counter < this.cooldown)
		{
			return;
		}
		this.counter = 0f;
		if (this.addStatus)
		{
			character.refs.afflictions.AddStatus(this.statusType, this.statusAmount, false, true, true);
		}
		if (this.poisonOverTime)
		{
			character.refs.afflictions.AddAffliction(new Affliction_PoisonOverTime(this.poisonOverTimeDuration, this.poisonOverTimeDelay, this.poisonOverTimeAmountPerSecond), false);
		}
	}

	// Token: 0x040014C2 RID: 5314
	public float cooldown = 1f;

	// Token: 0x040014C3 RID: 5315
	public bool addStatus;

	// Token: 0x040014C4 RID: 5316
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x040014C5 RID: 5317
	public float statusAmount = 0.05f;

	// Token: 0x040014C6 RID: 5318
	public bool poisonOverTime;

	// Token: 0x040014C7 RID: 5319
	public float poisonOverTimeDuration = 5f;

	// Token: 0x040014C8 RID: 5320
	public float poisonOverTimeDelay = 1f;

	// Token: 0x040014C9 RID: 5321
	public float poisonOverTimeAmountPerSecond = 0.01f;

	// Token: 0x040014CA RID: 5322
	private float counter;
}
