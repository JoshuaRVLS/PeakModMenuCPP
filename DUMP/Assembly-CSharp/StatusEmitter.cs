using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200034D RID: 845
public class StatusEmitter : MonoBehaviour
{
	// Token: 0x06001670 RID: 5744 RVA: 0x0007246F File Offset: 0x0007066F
	private void Start()
	{
		this.timeSinceLastTick = Random.value * this.randomStart;
	}

	// Token: 0x06001671 RID: 5745 RVA: 0x00072483 File Offset: 0x00070683
	protected virtual bool InRange()
	{
		return !(Character.localCharacter == null) && Vector3.Distance(Character.localCharacter.Center, base.transform.position) < this.radius + this.outerFade;
	}

	// Token: 0x06001672 RID: 5746 RVA: 0x000724C0 File Offset: 0x000706C0
	public void Update()
	{
		bool flag = this.InRange();
		if (flag && this.preventOverlap && this.preventOverlap)
		{
			this.isOverlapPriority = this.TryMakePriorityOverlappedEmitter();
		}
		if (this.inZoneWarning && this.inZone && (!flag || this.emitterDisabledByWind))
		{
			this.inZone = false;
			if (!this.preventOverlap || this.isOverlapPriority)
			{
				GUIManager.instance.sporesWarning.EndFX();
			}
		}
		if (this.inZoneWarning && flag && !this.inZone && !this.emitterDisabledByWind)
		{
			if (!this.preventOverlap || this.isOverlapPriority)
			{
				GUIManager.instance.sporesWarning.StartFX(0.5f);
			}
			this.inZone = true;
			this.timeSinceLastTick = -this.extraWarningTime;
			return;
		}
		if (this.emitterDisabledByWind)
		{
			return;
		}
		if (flag)
		{
			if (this.preventOverlap && !this.isOverlapPriority)
			{
				this.timeSinceLastTick = 0f;
				return;
			}
			this.timeSinceLastTick += Time.deltaTime;
			if (this.timeSinceLastTick < this.tickTime)
			{
				return;
			}
			float num = this.amount;
			float num2 = Vector3.Distance(Character.localCharacter.Center, base.transform.position);
			if (this.outerFade > 0.01f)
			{
				num *= Mathf.InverseLerp(this.radius + this.outerFade, num2, num2);
			}
			float num3 = 0f;
			if (this.innerFade > 0.01f)
			{
				num3 = (num2 - (this.radius - this.innerFade)) / this.innerFade;
			}
			num3 = Mathf.Clamp(1f - num3, this.minAmount, 1f);
			this.debug = num3;
			if (num > 0f)
			{
				Character.localCharacter.refs.afflictions.AddStatus(this.statusType, this.amount * this.tickTime * num3, false, true, true);
			}
			if (num < 0f)
			{
				Character.localCharacter.refs.afflictions.SubtractStatus(this.statusType, Mathf.Abs(this.amount * this.tickTime), false, false);
			}
		}
		this.timeSinceLastTick = 0f;
	}

	// Token: 0x06001673 RID: 5747 RVA: 0x000726D8 File Offset: 0x000708D8
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 0f, 1f, 0.4f);
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
		Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
		Gizmos.DrawWireSphere(base.transform.position, this.radius + this.outerFade);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.radius - this.innerFade);
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x0007277C File Offset: 0x0007097C
	private bool TryMakePriorityOverlappedEmitter()
	{
		if (!StatusEmitter.overlapPreventedStatusEmitters.ContainsKey(this.overlapIdentifier))
		{
			StatusEmitter.overlapPreventedStatusEmitters.Add(this.overlapIdentifier, this);
			return true;
		}
		if (StatusEmitter.overlapPreventedStatusEmitters[this.overlapIdentifier] == this)
		{
			return true;
		}
		if (StatusEmitter.overlapPreventedStatusEmitters[this.overlapIdentifier] == null)
		{
			StatusEmitter.overlapPreventedStatusEmitters[this.overlapIdentifier] = this;
			return true;
		}
		float num = Vector3.Distance(StatusEmitter.overlapPreventedStatusEmitters[this.overlapIdentifier].transform.position, Character.localCharacter.Center);
		if (Vector3.Distance(base.transform.position, Character.localCharacter.Center) < num)
		{
			StatusEmitter.overlapPreventedStatusEmitters[this.overlapIdentifier] = this;
			return true;
		}
		return false;
	}

	// Token: 0x040014A7 RID: 5287
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x040014A8 RID: 5288
	public float amount;

	// Token: 0x040014A9 RID: 5289
	public float radius = 1f;

	// Token: 0x040014AA RID: 5290
	public float outerFade;

	// Token: 0x040014AB RID: 5291
	public float innerFade;

	// Token: 0x040014AC RID: 5292
	public float minAmount = 0.2f;

	// Token: 0x040014AD RID: 5293
	private float timeSinceLastTick;

	// Token: 0x040014AE RID: 5294
	private float tickTime = 0.5f;

	// Token: 0x040014AF RID: 5295
	public bool inZoneWarning;

	// Token: 0x040014B0 RID: 5296
	public float extraWarningTime = 1f;

	// Token: 0x040014B1 RID: 5297
	private bool inZone;

	// Token: 0x040014B2 RID: 5298
	public float randomStart;

	// Token: 0x040014B3 RID: 5299
	public float debug;

	// Token: 0x040014B4 RID: 5300
	public bool preventOverlap;

	// Token: 0x040014B5 RID: 5301
	public static Dictionary<string, StatusEmitter> overlapPreventedStatusEmitters = new Dictionary<string, StatusEmitter>();

	// Token: 0x040014B6 RID: 5302
	public string overlapIdentifier;

	// Token: 0x040014B7 RID: 5303
	public bool isOverlapPriority;

	// Token: 0x040014B8 RID: 5304
	public bool emitterDisabledByWind;
}
