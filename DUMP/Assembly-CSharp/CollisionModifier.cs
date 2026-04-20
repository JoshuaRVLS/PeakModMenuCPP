using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200023D RID: 573
public class CollisionModifier : MonoBehaviour
{
	// Token: 0x06001195 RID: 4501 RVA: 0x0005889D File Offset: 0x00056A9D
	private void Awake()
	{
		this.collisionEvents = base.GetComponents<ICollisionModifierEvent>();
		if (this.bounceAnimator != null)
		{
			this.bounceAnimator.enabled = false;
		}
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x000588C8 File Offset: 0x00056AC8
	public void Collide(Character character, ContactPoint contactPoint, Collision collision, Bodypart bodypart)
	{
		CollisionModifier.<>c__DisplayClass20_0 CS$<>8__locals1 = new CollisionModifier.<>c__DisplayClass20_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.character = character;
		Action<Character, CollisionModifier, Collision, Bodypart> action = this.onCollide;
		if (action != null)
		{
			action(CS$<>8__locals1.character, this, collision, bodypart);
		}
		if (!this.applyEffects)
		{
			return;
		}
		if (this.characterList.Contains(CS$<>8__locals1.character))
		{
			return;
		}
		foreach (CollisionMod collisionMod in this.additionalMods)
		{
			CS$<>8__locals1.character.refs.afflictions.AddStatus(collisionMod.statusType, collisionMod.amount, false, true, true);
			CS$<>8__locals1.character.AddForce((CS$<>8__locals1.character.Center - contactPoint.point).normalized * collisionMod.knockback, 1f, 1f);
		}
		CS$<>8__locals1.character.refs.afflictions.AddStatus(this.statusType, this.damage, false, true, true);
		if (this.knockback > 0f && Vector3.Dot(contactPoint.normal, base.transform.forward) > this.bounceDotMinimum && CS$<>8__locals1.character.data.lastBouncedTime + 0.2f < Time.time)
		{
			float num = 1f;
			num *= Mathf.Clamp(Mathf.Pow(Mathf.Clamp(CS$<>8__locals1.character.data.sinceGrounded + this.sinceGroundedOffset, 1f, 5f), this.superBouncePow), 1f, 5f);
			Parasol parasol;
			if (CS$<>8__locals1.character.data.currentItem && CS$<>8__locals1.character.data.currentItem.TryGetComponent<Parasol>(out parasol) && parasol.isOpen)
			{
				num = 1f;
			}
			if (CS$<>8__locals1.character.data.fallSeconds > 0f || CS$<>8__locals1.character.refs.afflictions.shouldPassOut)
			{
				num = 1f;
			}
			CS$<>8__locals1.character.AddStamina(this.staminaBackOnBounce);
			CS$<>8__locals1.character.data.lastBouncedTime = Time.time;
			CS$<>8__locals1.character.data.sinceJump = 0f;
			if (this.useBounceCoroutine)
			{
				base.StartCoroutine(CS$<>8__locals1.<Collide>g__BounceRoutine|1(Vector3.Lerp((CS$<>8__locals1.character.Center - contactPoint.point).normalized, base.transform.forward, this.knockbackTowardsFwdVector) * num));
			}
			else
			{
				float d = this.knockback;
				if (CS$<>8__locals1.character.data.fallSeconds > 0f || CS$<>8__locals1.character.refs.afflictions.shouldPassOut)
				{
					d = this.ragdolledKnockback;
				}
				CS$<>8__locals1.character.AddForce(Vector3.Lerp((CS$<>8__locals1.character.Center - contactPoint.point).normalized, base.transform.forward, this.knockbackTowardsFwdVector) * num * d, 1f, 1f);
			}
			if (this.bounceAnimator)
			{
				if (this.bounceAnimatorRoutine != null)
				{
					base.StopCoroutine(this.bounceAnimatorRoutine);
				}
				this.bounceAnimator.enabled = true;
				this.bounceAnimator.SetTrigger("Bounce");
				this.bounceAnimatorRoutine = base.StartCoroutine(this.DisableBounceAnimator());
			}
			SFX_Player.instance.PlaySFX(this.bounceSFX, contactPoint.point, null, null, 1f, false);
			this.TriggerCharacterBouncedEvents(CS$<>8__locals1.character);
		}
		base.StartCoroutine(CS$<>8__locals1.<Collide>g__IHoldPlayer|0());
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x00058CAC File Offset: 0x00056EAC
	private IEnumerator DisableBounceAnimator()
	{
		yield return new WaitForSeconds(1f);
		this.bounceAnimator.enabled = false;
		yield break;
	}

	// Token: 0x06001198 RID: 4504 RVA: 0x00058CBC File Offset: 0x00056EBC
	private void TriggerCharacterBouncedEvents(Character bouncedCharacter)
	{
		ICollisionModifierEvent[] array = this.collisionEvents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnBouncedCharacter(bouncedCharacter);
		}
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x00058CE7 File Offset: 0x00056EE7
	internal bool CanStand(Character character)
	{
		return !this.hasStandableRange || Vector3.Distance(base.transform.position, character.Center) >= this.standableRange;
	}

	// Token: 0x04000F6D RID: 3949
	private List<Character> characterList = new List<Character>();

	// Token: 0x04000F6E RID: 3950
	private ICollisionModifierEvent[] collisionEvents;

	// Token: 0x04000F6F RID: 3951
	public bool applyEffects = true;

	// Token: 0x04000F70 RID: 3952
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000F71 RID: 3953
	public float damage = 0.15f;

	// Token: 0x04000F72 RID: 3954
	public float cooldown = 1f;

	// Token: 0x04000F73 RID: 3955
	public float knockback = 20f;

	// Token: 0x04000F74 RID: 3956
	public float ragdolledKnockback = 100f;

	// Token: 0x04000F75 RID: 3957
	public float knockbackTowardsFwdVector;

	// Token: 0x04000F76 RID: 3958
	public List<CollisionMod> additionalMods = new List<CollisionMod>();

	// Token: 0x04000F77 RID: 3959
	public Action<Character, CollisionModifier, Collision, Bodypart> onCollide;

	// Token: 0x04000F78 RID: 3960
	public bool useBounceCoroutine;

	// Token: 0x04000F79 RID: 3961
	public bool standable = true;

	// Token: 0x04000F7A RID: 3962
	public Animator bounceAnimator;

	// Token: 0x04000F7B RID: 3963
	public float superBouncePow = 2f;

	// Token: 0x04000F7C RID: 3964
	public float sinceGroundedOffset = -0.5f;

	// Token: 0x04000F7D RID: 3965
	public float bounceDotMinimum = -1f;

	// Token: 0x04000F7E RID: 3966
	public float staminaBackOnBounce = 0.1f;

	// Token: 0x04000F7F RID: 3967
	public SFX_Instance bounceSFX;

	// Token: 0x04000F80 RID: 3968
	private Coroutine bounceAnimatorRoutine;

	// Token: 0x04000F81 RID: 3969
	internal bool hasStandableRange;

	// Token: 0x04000F82 RID: 3970
	internal float standableRange;
}
