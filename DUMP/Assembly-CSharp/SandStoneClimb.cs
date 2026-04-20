using System;
using UnityEngine;

// Token: 0x0200032A RID: 810
public class SandStoneClimb : MonoBehaviour
{
	// Token: 0x06001583 RID: 5507 RVA: 0x0006CE54 File Offset: 0x0006B054
	private void Start()
	{
		ClimbModifierSurface component = base.GetComponent<ClimbModifierSurface>();
		component.onClimbAction = (Action<Character>)Delegate.Combine(component.onClimbAction, new Action<Character>(this.OnClimb));
		CollisionModifier component2 = base.GetComponent<CollisionModifier>();
		component2.onCollide = (Action<Character, CollisionModifier, Collision, Bodypart>)Delegate.Combine(component2.onCollide, new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnCollide));
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x0006CEAF File Offset: 0x0006B0AF
	private void OnCollide(Character character, CollisionModifier modifier, Collision collision, Bodypart bodypart)
	{
		base.transform.localScale = Vector3.MoveTowards(base.transform.localScale, Vector3.zero, Time.deltaTime * 0.05f);
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x0006CEDC File Offset: 0x0006B0DC
	private void OnClimb(Character character)
	{
		base.transform.localScale = Vector3.MoveTowards(base.transform.localScale, Vector3.zero, Time.deltaTime * 0.1f);
	}
}
