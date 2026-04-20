using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class FakeItemSticky : FakeItem
{
	// Token: 0x06000242 RID: 578 RVA: 0x00011242 File Offset: 0x0000F442
	private void Start()
	{
		if (Application.isPlaying)
		{
			CollisionModifier component = this.physicalCollider.GetComponent<CollisionModifier>();
			component.onCollide = (Action<Character, CollisionModifier, Collision, Bodypart>)Delegate.Combine(component.onCollide, new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnCollide));
		}
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00011278 File Offset: 0x0000F478
	private void OnCollide(Character character, CollisionModifier modifier, Collision collision, Bodypart bodyPart)
	{
		if (!character.IsLocal)
		{
			return;
		}
		if (character.data.isInvincible)
		{
			return;
		}
		if (this.collided)
		{
			return;
		}
		this.collided = true;
		FakeItemManager.Instance.photonView.RPC("RPC_RequestStickFakeItemToPlayer", RpcTarget.MasterClient, new object[]
		{
			character.photonView.ViewID,
			this.index,
			(int)bodyPart.partType,
			base.transform.position - bodyPart.transform.position
		});
	}

	// Token: 0x06000244 RID: 580 RVA: 0x0001131B File Offset: 0x0000F51B
	public override void UnPickUpVisibly()
	{
		base.UnPickUpVisibly();
		this.collided = false;
	}

	// Token: 0x04000215 RID: 533
	public bool stickOnCollision = true;

	// Token: 0x04000216 RID: 534
	public Collider physicalCollider;

	// Token: 0x04000217 RID: 535
	private bool collided;
}
