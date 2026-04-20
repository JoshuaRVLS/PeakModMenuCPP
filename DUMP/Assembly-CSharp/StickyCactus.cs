using System;
using UnityEngine;

// Token: 0x02000351 RID: 849
public class StickyCactus : MonoBehaviour
{
	// Token: 0x0600167F RID: 5759 RVA: 0x00072DB1 File Offset: 0x00070FB1
	private void Start()
	{
		CollisionModifier component = base.GetComponent<CollisionModifier>();
		component.onCollide = (Action<Character, CollisionModifier, Collision, Bodypart>)Delegate.Combine(component.onCollide, new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnCollide));
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x00072DDC File Offset: 0x00070FDC
	private void OnCollide(Character character, CollisionModifier modifier, Collision collision, Bodypart bodypart)
	{
		if (!character.IsLocal)
		{
			return;
		}
		if (RunSettings.GetValue(RunSettings.SETTINGTYPE.Hazard_Thorns, false) == 0)
		{
			return;
		}
		if (character.warping)
		{
			return;
		}
		if (character.data.isInvincible)
		{
			return;
		}
		if (character.data.isSkeleton)
		{
			return;
		}
		if (bodypart.partType == BodypartType.Head)
		{
			return;
		}
		if (bodypart.partType == BodypartType.Torso)
		{
			return;
		}
		if (bodypart.partType == BodypartType.Hip)
		{
			return;
		}
		if (character.TryStickBodypart(bodypart, collision.contacts[0].point, CharacterAfflictions.STATUSTYPE.Thorns, 0f) && this.applyThorn)
		{
			character.refs.afflictions.AddThorn(collision.contacts[0].point);
		}
	}

	// Token: 0x040014D5 RID: 5333
	public bool applyThorn = true;
}
