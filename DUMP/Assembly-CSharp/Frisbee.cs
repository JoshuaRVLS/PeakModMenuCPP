using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000BC RID: 188
public class Frisbee : MonoBehaviour
{
	// Token: 0x06000711 RID: 1809 RVA: 0x000288A0 File Offset: 0x00026AA0
	private void OnEnable()
	{
		GlobalEvents.OnItemThrown = (Action<Item>)Delegate.Combine(GlobalEvents.OnItemThrown, new Action<Item>(this.OnItemThrown));
		GlobalEvents.OnItemRequested = (Action<Item, Character>)Delegate.Combine(GlobalEvents.OnItemRequested, new Action<Item, Character>(this.TestRequestedItem));
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x000288F0 File Offset: 0x00026AF0
	private void OnDisable()
	{
		GlobalEvents.OnItemThrown = (Action<Item>)Delegate.Remove(GlobalEvents.OnItemThrown, new Action<Item>(this.OnItemThrown));
		GlobalEvents.OnItemRequested = (Action<Item, Character>)Delegate.Remove(GlobalEvents.OnItemRequested, new Action<Item, Character>(this.TestRequestedItem));
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x0002893D File Offset: 0x00026B3D
	private void OnItemThrown(Item obj)
	{
		if (obj == this.item)
		{
			this.startedThrowPosition = obj.Center();
			this.throwValidForAchievement = true;
		}
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x00028960 File Offset: 0x00026B60
	private void FixedUpdate()
	{
		if (this.item.holderCharacter == null)
		{
			float d = Mathf.InverseLerp(0f, this.velocityForLift, this.item.rig.linearVelocity.sqrMagnitude);
			Vector3 up = base.transform.up;
			float d2 = Mathf.Clamp01(Vector3.Dot(base.transform.up, Vector3.up));
			this.item.rig.AddForce(up * d2 * this.liftForce * d);
		}
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x000289F8 File Offset: 0x00026BF8
	private void OnCollisionEnter(Collision collision)
	{
		this.item.rig.linearVelocity *= 0.5f;
		if (this.throwValidForAchievement)
		{
			int layer = collision.gameObject.layer;
			if ((HelperFunctions.terrainMapMask & 1 << layer) != 0)
			{
				this.throwValidForAchievement = false;
				return;
			}
			if ((LayerMask.GetMask(new string[]
			{
				"Water"
			}) & 1 << layer) != 0)
			{
				this.throwValidForAchievement = false;
			}
		}
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000716 RID: 1814 RVA: 0x00028A77 File Offset: 0x00026C77
	private float throwDistance
	{
		get
		{
			return Vector3.Distance(this.startedThrowPosition, this.item.Center()) * CharacterStats.unitsToMeters;
		}
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00028A98 File Offset: 0x00026C98
	private void TestRequestedItem(Item requestedItem, Character character)
	{
		if (!this.throwValidForAchievement)
		{
			return;
		}
		if (character.IsLocal && requestedItem == this.item)
		{
			Debug.Log("Frisbee grabbed at distance " + this.throwDistance.ToString());
			if (this.throwDistance >= 100f)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.UltimateBadge);
			}
		}
	}

	// Token: 0x04000712 RID: 1810
	public Item item;

	// Token: 0x04000713 RID: 1811
	public float liftForce = 10f;

	// Token: 0x04000714 RID: 1812
	public float velocityForLift = 10f;

	// Token: 0x04000715 RID: 1813
	private Vector3 startedThrowPosition;

	// Token: 0x04000716 RID: 1814
	private bool throwValidForAchievement;
}
