using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200035C RID: 860
public class TiedBalloon : MonoBehaviourPunCallbacks
{
	// Token: 0x060016C7 RID: 5831 RVA: 0x0007516B File Offset: 0x0007336B
	public void Init(CharacterBalloons characterBalloons, float height, int colorID)
	{
		base.photonView.RPC("RPC_Init", RpcTarget.All, new object[]
		{
			characterBalloons.photonView.ViewID,
			height,
			colorID
		});
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x000751AC File Offset: 0x000733AC
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("RPC_Init", newPlayer, new object[]
			{
				this.characterBalloons.photonView.ViewID,
				this.characterBalloons.character.Center.y,
				this.colorIndex
			});
		}
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x00075220 File Offset: 0x00073420
	[PunRPC]
	public void RPC_Init(int characterID, float height, int colorID)
	{
		base.StartCoroutine(this.InitRoutine(characterID, height, colorID));
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x00075232 File Offset: 0x00073432
	private IEnumerator InitRoutine(int characterID, float height, int colorID)
	{
		while (!Character.localCharacter)
		{
			yield return null;
		}
		PhotonView photonView = PhotonView.Find(characterID);
		if (photonView == null)
		{
			Debug.LogError("Tried to assign balloon to nonexistent photon ID.");
			yield break;
		}
		CharacterBalloons component = photonView.GetComponent<CharacterBalloons>();
		Debug.Log(string.Format("Init Balloon for view {0} with color {1}", characterID, colorID));
		this.balloonRenderer.material = Character.localCharacter.refs.balloons.balloonColors[colorID];
		this.colorIndex = colorID;
		this.initialHeight = height;
		this.initialTime = Time.time;
		this.characterBalloons = component;
		component.tiedBalloons.Add(this);
		yield break;
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x00075256 File Offset: 0x00073456
	private void LateUpdate()
	{
		this.UpdateLineRenderer();
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x00075260 File Offset: 0x00073460
	private void FixedUpdate()
	{
		this.rb.AddForce(Vector3.up * this.floatForce, ForceMode.Acceleration);
		this.UpdateLineRenderer();
		if (base.photonView.IsMine && (this.rb.transform.position.y > this.initialHeight + this.popHeight || Time.time > this.initialTime + this.popTime))
		{
			this.Pop();
		}
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x000752DA File Offset: 0x000734DA
	public void Pop()
	{
		if (base.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x000752F4 File Offset: 0x000734F4
	private void OnDestroy()
	{
		this.characterBalloons.RemoveBalloon(this);
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x00075304 File Offset: 0x00073504
	private void UpdateLineRenderer()
	{
		this.positions[0] = this.start.position;
		this.positions[1] = this.end.position;
		this.lr.SetPositions(this.positions);
	}

	// Token: 0x0400152F RID: 5423
	public LineRenderer lr;

	// Token: 0x04001530 RID: 5424
	public Transform anchor;

	// Token: 0x04001531 RID: 5425
	public Transform start;

	// Token: 0x04001532 RID: 5426
	public Transform end;

	// Token: 0x04001533 RID: 5427
	public Rigidbody rb;

	// Token: 0x04001534 RID: 5428
	public MeshRenderer balloonRenderer;

	// Token: 0x04001535 RID: 5429
	public float floatForce = 10f;

	// Token: 0x04001536 RID: 5430
	public int colorIndex;

	// Token: 0x04001537 RID: 5431
	private float initialHeight;

	// Token: 0x04001538 RID: 5432
	public float popHeight = 100f;

	// Token: 0x04001539 RID: 5433
	public float popTime = 10f;

	// Token: 0x0400153A RID: 5434
	private CharacterBalloons characterBalloons;

	// Token: 0x0400153B RID: 5435
	private float initialTime;

	// Token: 0x0400153C RID: 5436
	private Vector3[] positions = new Vector3[2];
}
