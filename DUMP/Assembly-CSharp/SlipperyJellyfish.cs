using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000344 RID: 836
public class SlipperyJellyfish : MonoBehaviour
{
	// Token: 0x06001643 RID: 5699 RVA: 0x000710A5 File Offset: 0x0006F2A5
	private void Start()
	{
		this.relay = base.GetComponentInParent<TriggerRelay>();
		this.lastSlipped = Time.time;
	}

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06001644 RID: 5700 RVA: 0x000710BE File Offset: 0x0006F2BE
	private float timeSinceLastSlipped
	{
		get
		{
			return Time.time - this.lastSlipped;
		}
	}

	// Token: 0x06001645 RID: 5701 RVA: 0x000710CC File Offset: 0x0006F2CC
	public void OnTriggerEnter(Collider other)
	{
		if (this.timeSinceLastSlipped < 3f)
		{
			return;
		}
		Character character;
		if (!CharacterRagdoll.TryGetCharacterFromCollider(other, out character))
		{
			return;
		}
		if (!character.IsLocal)
		{
			return;
		}
		this.lastSlipped = Time.time;
		this.relay.view.RPC("RPCA_TriggerWithTarget", RpcTarget.All, new object[]
		{
			base.transform.GetSiblingIndex(),
			Character.localCharacter.refs.view.ViewID
		});
	}

	// Token: 0x06001646 RID: 5702 RVA: 0x00071154 File Offset: 0x0006F354
	public void Trigger(int targetID)
	{
		Character component = PhotonView.Find(targetID).GetComponent<Character>();
		if (component == null)
		{
			return;
		}
		Rigidbody bodypartRig = component.GetBodypartRig(BodypartType.Foot_R);
		Rigidbody bodypartRig2 = component.GetBodypartRig(BodypartType.Foot_L);
		Rigidbody bodypartRig3 = component.GetBodypartRig(BodypartType.Hip);
		Rigidbody bodypartRig4 = component.GetBodypartRig(BodypartType.Head);
		component.RPCA_Fall(2f);
		bodypartRig.AddForce((component.data.lookDirection_Flat + Vector3.up) * 200f, ForceMode.Impulse);
		bodypartRig2.AddForce((component.data.lookDirection_Flat + Vector3.up) * 200f, ForceMode.Impulse);
		bodypartRig3.AddForce(Vector3.up * 1500f, ForceMode.Impulse);
		bodypartRig4.AddForce(component.data.lookDirection_Flat * -300f, ForceMode.Impulse);
		component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.05f, true, true, true);
		for (int i = 0; i < this.slipSFX.Length; i++)
		{
			this.slipSFX[i].Play(base.transform.position);
		}
	}

	// Token: 0x04001457 RID: 5207
	private TriggerRelay relay;

	// Token: 0x04001458 RID: 5208
	public SFX_Instance[] slipSFX;

	// Token: 0x04001459 RID: 5209
	private float lastSlipped;
}
