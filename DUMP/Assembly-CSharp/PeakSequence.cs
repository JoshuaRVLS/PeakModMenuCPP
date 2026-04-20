using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002C8 RID: 712
public class PeakSequence : MonoBehaviour
{
	// Token: 0x06001415 RID: 5141 RVA: 0x000657AA File Offset: 0x000639AA
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x000657B8 File Offset: 0x000639B8
	private void OnDisable()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.Log("Destroying ropes");
			if (this.ropeAnchorInstance != null)
			{
				PhotonNetwork.Destroy(this.ropeAnchorInstance.photonView);
			}
			if (this.ropeInstance != null)
			{
				PhotonNetwork.Destroy(this.ropeInstance.photonView);
				return;
			}
		}
		else
		{
			if (this.ropeAnchorInstance != null)
			{
				this.ropeAnchorInstance.gameObject.SetActive(false);
			}
			if (this.ropeInstance != null)
			{
				this.ropeInstance.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x00065854 File Offset: 0x00063A54
	private void Update()
	{
		if (this.waitTime > this.timeToWait)
		{
			if (!this.spawnedRope)
			{
				if (PhotonNetwork.IsMasterClient)
				{
					this.spawnedRope = true;
					GameObject gameObject = PhotonNetwork.Instantiate(this.ropeAnchorWithRopePref.name, this.ropeSpawnPoint.position, Quaternion.identity, 0, null);
					this.ropeAnchorInstance = gameObject.GetComponent<RopeAnchorWithRope>();
					this.ropeAnchorInstance.ropeSegmentLength = 40f;
					Rope rope = this.ropeAnchorInstance.SpawnRope();
					this.view.RPC("SetRopeToClients", RpcTarget.All, new object[]
					{
						rope.GetComponent<PhotonView>()
					});
				}
			}
			else
			{
				this.CheckGameComplete();
			}
		}
		this.waitTime += Time.deltaTime;
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x00065910 File Offset: 0x00063B10
	private void CheckGameComplete()
	{
		if (this.endingGame)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			int num = 0;
			List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
			for (int i = allPlayerCharacters.Count - 1; i >= 0; i--)
			{
				if (allPlayerCharacters[i].data.dead)
				{
					allPlayerCharacters.RemoveAt(i);
				}
			}
			List<Character> list = new List<Character>();
			foreach (Character character in allPlayerCharacters)
			{
				if (character.data.fullyConscious)
				{
					list.Add(character);
				}
			}
			for (int j = 0; j < allPlayerCharacters.Count; j++)
			{
				if (Character.CheckWinCondition(allPlayerCharacters[j]))
				{
					num++;
				}
			}
			if (num > 0)
			{
				this.timerElapsed += Time.deltaTime;
				if (this.timerElapsed >= this.lengthOfASecond)
				{
					if (num >= list.Count && this.secondsElapsed < this.totalSeconds - this.totalWinningSeconds)
					{
						this.secondsElapsed = this.totalSeconds - this.totalWinningSeconds;
					}
					this.timerElapsed = 0f;
					this.view.RPC("RPCUpdateTimer", RpcTarget.All, new object[]
					{
						this.secondsElapsed
					});
					this.secondsElapsed++;
					if (this.secondsElapsed > this.totalSeconds)
					{
						this.endingGame = true;
						Character.localCharacter.EndGame();
						return;
					}
				}
			}
			else
			{
				this.secondsElapsed = 0;
				this.timerElapsed = 0f;
				this.view.RPC("RPCUpdateTimer", RpcTarget.All, new object[]
				{
					-1
				});
			}
		}
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x00065AD0 File Offset: 0x00063CD0
	[PunRPC]
	public void SetRopeToClients(PhotonView v)
	{
		this.ropeInstance = v.GetComponent<Rope>();
		Debug.Log(string.Format("ROPE AS BEEN SET TO {0}", this.ropeInstance));
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x00065AF3 File Offset: 0x00063CF3
	[PunRPC]
	private void RPCUpdateTimer(int seconds)
	{
		if (seconds == -1)
		{
			GUIManager.instance.endgame.Disable();
			return;
		}
		GUIManager.instance.endgame.UpdateCounter(this.totalSeconds - seconds);
	}

	// Token: 0x04001248 RID: 4680
	private PhotonView view;

	// Token: 0x04001249 RID: 4681
	public GameObject ropeAnchorWithRopePref;

	// Token: 0x0400124A RID: 4682
	public Transform ropeSpawnPoint;

	// Token: 0x0400124B RID: 4683
	private float waitTime;

	// Token: 0x0400124C RID: 4684
	public float timeToWait = 5f;

	// Token: 0x0400124D RID: 4685
	public int totalSeconds = 30;

	// Token: 0x0400124E RID: 4686
	public int totalWinningSeconds = 5;

	// Token: 0x0400124F RID: 4687
	public float lengthOfASecond = 1.5f;

	// Token: 0x04001250 RID: 4688
	private bool spawnedRope;

	// Token: 0x04001251 RID: 4689
	public RopeAnchorWithRope ropeAnchorInstance;

	// Token: 0x04001252 RID: 4690
	public Rope ropeInstance;

	// Token: 0x04001253 RID: 4691
	private float timerElapsed;

	// Token: 0x04001254 RID: 4692
	private int secondsElapsed;

	// Token: 0x04001255 RID: 4693
	private bool endingGame;
}
