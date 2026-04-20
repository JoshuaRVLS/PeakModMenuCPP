using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200036B RID: 875
public class TumbleWeed : MonoBehaviour
{
	// Token: 0x06001711 RID: 5905 RVA: 0x000766D4 File Offset: 0x000748D4
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.rig = base.GetComponent<Rigidbody>();
		this.maxAngle = Mathf.Lerp(50f, 180f, Mathf.Pow(Random.value, 5f));
		this.rollForce *= Mathf.Lerp(0.5f, 1f, Mathf.Pow(Random.value, 2f));
		this.originalScale = base.transform.localScale.x;
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x00076760 File Offset: 0x00074960
	private void FixedUpdate()
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		Vector3 a = -Vector3.right;
		Character target = this.GetTarget();
		if (target)
		{
			a = (target.Center - base.transform.position).normalized;
		}
		this.rig.AddForce(a * this.rollForce, ForceMode.Acceleration);
	}

	// Token: 0x06001713 RID: 5907 RVA: 0x000767CC File Offset: 0x000749CC
	private Character GetTarget()
	{
		float num = 300f;
		Character result = null;
		foreach (Character character in Character.AllCharacters)
		{
			if (Vector3.Angle(-Vector3.right, character.Center - base.transform.position) <= this.maxAngle)
			{
				float num2 = Vector3.Distance(character.Center, base.transform.position);
				if (num2 < num)
				{
					num = num2;
					result = character;
				}
			}
		}
		return result;
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x00076870 File Offset: 0x00074A70
	public void OnCollisionEnter(Collision collision)
	{
		Character componentInParent = collision.gameObject.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		if (this.ignored.Contains(componentInParent))
		{
			return;
		}
		base.StartCoroutine(this.IgnoreTarget(componentInParent));
		float num = base.transform.localScale.x / this.originalScale;
		if (this.originalScale == 0f)
		{
			num = 1f;
		}
		num = Mathf.Clamp01(num);
		float num2 = Mathf.Clamp01(this.rig.linearVelocity.magnitude * num * this.powerMultiplier);
		if (this.testFullPower)
		{
			num2 = 1f;
		}
		if (num2 < 0.2f)
		{
			return;
		}
		componentInParent.Fall(2f * num2, 0f);
		componentInParent.AddForceAtPosition(this.rig.linearVelocity.normalized * this.collisionForce * num2, collision.contacts[0].point, 2f);
		componentInParent.refs.afflictions.AddThorn(collision.contacts[0].point);
		if (num2 > 0.6f)
		{
			componentInParent.refs.afflictions.AddThorn(collision.contacts[0].point);
		}
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x000769BF File Offset: 0x00074BBF
	public IEnumerator IgnoreTarget(Character target)
	{
		this.ignored.Add(target);
		yield return new WaitForSeconds(1f);
		this.ignored.Remove(target);
		yield break;
	}

	// Token: 0x04001581 RID: 5505
	private Rigidbody rig;

	// Token: 0x04001582 RID: 5506
	public float rollForce;

	// Token: 0x04001583 RID: 5507
	public float collisionForce;

	// Token: 0x04001584 RID: 5508
	private float maxAngle;

	// Token: 0x04001585 RID: 5509
	private PhotonView photonView;

	// Token: 0x04001586 RID: 5510
	private float originalScale = 1f;

	// Token: 0x04001587 RID: 5511
	public float powerMultiplier = 0.035f;

	// Token: 0x04001588 RID: 5512
	public bool testFullPower;

	// Token: 0x04001589 RID: 5513
	private List<Character> ignored = new List<Character>();
}
