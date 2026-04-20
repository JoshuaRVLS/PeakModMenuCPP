using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000213 RID: 531
public class Basketball : MonoBehaviour
{
	// Token: 0x06001056 RID: 4182 RVA: 0x0005128C File Offset: 0x0004F48C
	public void OnCollisionEnter(Collision collision)
	{
		if (!collision.rigidbody && Mathf.Abs(Vector3.Dot(collision.contacts[0].normal, Vector3.up)) < 0.2f)
		{
			this.item.rig.linearVelocity = new Vector3(this.item.rig.linearVelocity.x * this.xzBounceLoss, this.item.rig.linearVelocity.y - this.yFall, this.item.rig.linearVelocity.z * this.xzBounceLoss);
		}
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x0005133C File Offset: 0x0004F53C
	public void Update()
	{
		if (this.item.itemState == ItemState.Held && this.item.holderCharacter.input.movementInput.magnitude > 0.5f && this.item.holderCharacter.data.isGrounded && !this.item.holderCharacter.refs.items.isChargingThrow)
		{
			this.dribbling = true;
			return;
		}
		this.dribbling = false;
	}

	// Token: 0x06001058 RID: 4184 RVA: 0x000513BA File Offset: 0x0004F5BA
	private void Start()
	{
		base.StartCoroutine(this.DribbleRoutine());
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x000513C9 File Offset: 0x0004F5C9
	private IEnumerator DribbleRoutine()
	{
		for (;;)
		{
			if (this.dribbling)
			{
				Vector3 endPosWorldSpace = Vector3.zero;
				RaycastHit raycastHit = HelperFunctions.LineCheck(this.basketballMesh.position, this.basketballMesh.position - Vector3.up * 100f, HelperFunctions.LayerType.AllPhysicalExceptCharacter, 0.1f, QueryTriggerInteraction.Ignore);
				if (raycastHit.collider != null)
				{
					endPosWorldSpace = raycastHit.point;
					bool playedSFX = false;
					float t = 0f;
					Vector3 avarageVelocity = this.item.holderCharacter.data.avarageVelocity;
					avarageVelocity.y = 0f;
					float dribSpeed = Mathf.Clamp(avarageVelocity.magnitude, 1f, 3f);
					while (t < 1f)
					{
						t += Time.deltaTime * dribSpeed;
						if (t > 0.5f && !playedSFX)
						{
							this.impact.Play(this.basketballMesh.position);
							playedSFX = true;
						}
						endPosWorldSpace = new Vector3(this.basketballMesh.parent.position.x, endPosWorldSpace.y, this.basketballMesh.parent.position.z);
						this.basketballMesh.position = Vector3.Lerp(this.basketballMesh.parent.TransformPoint(Vector3.zero), endPosWorldSpace + Vector3.up * this.dribbleFloorOffset, this.dribbleCurve.Evaluate(t));
						this.item.defaultPos = new Vector3(this.item.defaultPos.x, this.handsPositionCurve.Evaluate(t), this.item.defaultPos.z);
						yield return null;
					}
				}
				Vector3 avarageVelocity2 = this.item.holderCharacter.data.avarageVelocity;
				avarageVelocity2.y = 0f;
				yield return new WaitForSeconds((avarageVelocity2.magnitude > 3f) ? this.dribbleWaitSprint : this.dribbleWait);
				endPosWorldSpace = default(Vector3);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000E3F RID: 3647
	public Item item;

	// Token: 0x04000E40 RID: 3648
	public float xzBounceLoss = 0.5f;

	// Token: 0x04000E41 RID: 3649
	public float yFall = 5f;

	// Token: 0x04000E42 RID: 3650
	public Transform basketballMesh;

	// Token: 0x04000E43 RID: 3651
	public SFX_Instance impact;

	// Token: 0x04000E44 RID: 3652
	public float dribbleWait = 0.25f;

	// Token: 0x04000E45 RID: 3653
	public float dribbleWaitSprint = 0.05f;

	// Token: 0x04000E46 RID: 3654
	public float dribbleFloorOffset = 0.3f;

	// Token: 0x04000E47 RID: 3655
	public AnimationCurve dribbleCurve;

	// Token: 0x04000E48 RID: 3656
	public AnimationCurve handsPositionCurve;

	// Token: 0x04000E49 RID: 3657
	private bool dribbling;
}
