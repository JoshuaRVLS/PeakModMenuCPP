using System;
using System.Collections.Generic;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class AOE : MonoBehaviour
{
	// Token: 0x17000126 RID: 294
	// (get) Token: 0x0600102D RID: 4141 RVA: 0x0004FF62 File Offset: 0x0004E162
	private bool hasStatus
	{
		get
		{
			return Mathf.Abs(this.statusAmount) > 0f;
		}
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x0004FF76 File Offset: 0x0004E176
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.range);
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x0004FF8E File Offset: 0x0004E18E
	private void Start()
	{
		if (this.auto)
		{
			this.Explode();
		}
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x0004FF9E File Offset: 0x0004E19E
	private void OnEnable()
	{
		if (this.onEnable)
		{
			this.Explode();
		}
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x0004FFB0 File Offset: 0x0004E1B0
	public void Explode()
	{
		if (this.range == 0f)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.range, HelperFunctions.GetMask(this.mask));
		List<Character> list = new List<Character>();
		for (int i = 0; i < array.Length; i++)
		{
			Character character;
			CharacterRagdoll.TryGetCharacterFromCollider(array[i], out character);
			if (character != null && !list.Contains(character))
			{
				float num = Vector3.Distance(base.transform.position, character.Center);
				if (num <= this.range)
				{
					float factor = this.GetFactor(num);
					if (factor >= this.minFactor && (!this.requireLineOfSigh || !HelperFunctions.LineCheck(base.transform.position, character.Center, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform))
					{
						list.Add(character);
						Vector3 a = Vector3.zero;
						if (this.useSingleDirection)
						{
							a = this.singleDirectionForwardTF.forward;
						}
						else
						{
							a = (character.Center - base.transform.position).normalized;
						}
						if (Mathf.Abs(this.statusAmount) > 0f)
						{
							if (this.illegalStatus != "")
							{
								if (this.illegalStatus.ToUpperInvariant().Equals("BLIND"))
								{
									Affliction_Blind affliction_Blind = new Affliction_Blind
									{
										totalTime = this.statusAmount * factor
									};
									character.refs.afflictions.AddAffliction(affliction_Blind, false);
								}
								else
								{
									character.AddIllegalStatus(this.illegalStatus, this.statusAmount * factor);
								}
							}
							else
							{
								character.refs.afflictions.AdjustStatus(this.statusType, this.statusAmount * factor, false);
								if (this.addtlStatus.Length != 0)
								{
									for (int j = 0; j < this.addtlStatus.Length; j++)
									{
										character.refs.afflictions.AdjustStatus(this.addtlStatus[j], this.statusAmount * factor, false);
									}
								}
							}
						}
						if (this.hasAffliction)
						{
							character.refs.afflictions.AddAffliction(this.affliction, false);
						}
						character.AddForce(a * factor * this.knockback, 0.7f, 1.3f);
						if (this.fallTime > 0f && character.photonView.IsMine)
						{
							character.Fall(factor * this.fallTime, 0f);
						}
					}
				}
			}
			else if (this.canLaunchItems)
			{
				Item componentInParent = array[i].GetComponentInParent<Item>();
				if (componentInParent != null && componentInParent.photonView.IsMine && componentInParent.itemState == ItemState.Ground)
				{
					float num2 = Vector3.Distance(base.transform.position, componentInParent.Center());
					if (num2 <= this.range)
					{
						float factor2 = this.GetFactor(num2);
						if (factor2 >= this.minFactor && (!this.requireLineOfSigh || !HelperFunctions.LineCheck(base.transform.position, componentInParent.Center(), HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform))
						{
							EventOnItemCollision eventOnItemCollision;
							if (this.procCollisionEvents && componentInParent.TryGetComponent<EventOnItemCollision>(out eventOnItemCollision))
							{
								eventOnItemCollision.TriggerEvent();
							}
							if (this.cooksItems)
							{
								componentInParent.cooking.FinishCooking();
							}
							Vector3 normalized = (componentInParent.Center() - base.transform.position).normalized;
							componentInParent.rig.AddForce(normalized * factor2 * this.knockback * this.itemKnockbackMultiplier, ForceMode.Impulse);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x00050376 File Offset: 0x0004E576
	private float GetFactor(float dist)
	{
		return Mathf.Pow(1f - dist / this.range, this.factorPow);
	}

	// Token: 0x04000E06 RID: 3590
	public HelperFunctions.LayerType mask;

	// Token: 0x04000E07 RID: 3591
	public bool auto = true;

	// Token: 0x04000E08 RID: 3592
	public bool onEnable;

	// Token: 0x04000E09 RID: 3593
	public float range = 5f;

	// Token: 0x04000E0A RID: 3594
	public float fallTime = 0.5f;

	// Token: 0x04000E0B RID: 3595
	public float knockback = 25f;

	// Token: 0x04000E0C RID: 3596
	public float minFactor = 0.2f;

	// Token: 0x04000E0D RID: 3597
	public float factorPow = 1f;

	// Token: 0x04000E0E RID: 3598
	public bool requireLineOfSigh;

	// Token: 0x04000E0F RID: 3599
	public bool canLaunchItems;

	// Token: 0x04000E10 RID: 3600
	public float itemKnockbackMultiplier = 1f;

	// Token: 0x04000E11 RID: 3601
	public float statusAmount;

	// Token: 0x04000E12 RID: 3602
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000E13 RID: 3603
	public CharacterAfflictions.STATUSTYPE[] addtlStatus;

	// Token: 0x04000E14 RID: 3604
	public string illegalStatus = "";

	// Token: 0x04000E15 RID: 3605
	public bool useSingleDirection;

	// Token: 0x04000E16 RID: 3606
	public Transform singleDirectionForwardTF;

	// Token: 0x04000E17 RID: 3607
	public bool hasAffliction;

	// Token: 0x04000E18 RID: 3608
	[SerializeReference]
	public Affliction affliction;

	// Token: 0x04000E19 RID: 3609
	public bool procCollisionEvents;

	// Token: 0x04000E1A RID: 3610
	public bool cooksItems;
}
