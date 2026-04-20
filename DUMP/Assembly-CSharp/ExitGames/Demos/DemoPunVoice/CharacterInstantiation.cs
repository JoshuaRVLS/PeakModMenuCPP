using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000395 RID: 917
	public class CharacterInstantiation : MonoBehaviourPunCallbacks, IOnEventCallback
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06001831 RID: 6193 RVA: 0x0007AAA4 File Offset: 0x00078CA4
		// (remove) Token: 0x06001832 RID: 6194 RVA: 0x0007AAD8 File Offset: 0x00078CD8
		public static event CharacterInstantiation.OnCharacterInstantiated CharacterInstantiated;

		// Token: 0x06001833 RID: 6195 RVA: 0x0007AB0C File Offset: 0x00078D0C
		public override void OnJoinedRoom()
		{
			if (!this.AutoSpawn)
			{
				return;
			}
			if (this.PrefabsToInstantiate != null)
			{
				int num = PhotonNetwork.LocalPlayer.ActorNumber;
				if (num < 1)
				{
					num = 1;
				}
				int num2 = (num - 1) % this.PrefabsToInstantiate.Length;
				Vector3 vector;
				Quaternion rotation;
				this.GetSpawnPoint(out vector, out rotation);
				Camera.main.transform.position += vector;
				if (this.manualInstantiation)
				{
					this.ManualInstantiation(num2, vector, rotation);
					return;
				}
				GameObject gameObject = this.PrefabsToInstantiate[num2];
				gameObject = PhotonNetwork.Instantiate(gameObject.name, vector, rotation, 0, null);
				if (CharacterInstantiation.CharacterInstantiated != null)
				{
					CharacterInstantiation.CharacterInstantiated(gameObject);
				}
			}
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x0007ABB4 File Offset: 0x00078DB4
		private void ManualInstantiation(int index, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = this.PrefabsToInstantiate[index];
			GameObject gameObject2;
			if (this.differentPrefabs)
			{
				gameObject2 = Object.Instantiate<GameObject>(Resources.Load(string.Format("{0}{1}", gameObject.name, this.localPrefabSuffix)) as GameObject, position, rotation);
			}
			else
			{
				gameObject2 = Object.Instantiate<GameObject>(gameObject, position, rotation);
			}
			PhotonView component = gameObject2.GetComponent<PhotonView>();
			if (PhotonNetwork.AllocateViewID(component))
			{
				object[] eventContent = new object[]
				{
					index,
					gameObject2.transform.position,
					gameObject2.transform.rotation,
					component.ViewID
				};
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					Receivers = ReceiverGroup.Others,
					CachingOption = EventCaching.AddToRoomCache
				};
				PhotonNetwork.RaiseEvent(this.manualInstantiationEventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
				if (CharacterInstantiation.CharacterInstantiated != null)
				{
					CharacterInstantiation.CharacterInstantiated(gameObject2);
					return;
				}
			}
			else
			{
				Debug.LogError("Failed to allocate a ViewId.");
				Object.Destroy(gameObject2);
			}
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x0007ACA8 File Offset: 0x00078EA8
		public void OnEvent(EventData photonEvent)
		{
			if (photonEvent.Code == this.manualInstantiationEventCode)
			{
				object[] array = photonEvent.CustomData as object[];
				int num = (int)array[0];
				GameObject gameObject = this.PrefabsToInstantiate[num];
				Vector3 position = (Vector3)array[1];
				Quaternion rotation = (Quaternion)array[2];
				GameObject gameObject2;
				if (this.differentPrefabs)
				{
					gameObject2 = Object.Instantiate<GameObject>(Resources.Load(string.Format("{0}{1}", gameObject.name, this.remotePrefabSuffix)) as GameObject, position, rotation);
				}
				else
				{
					gameObject2 = Object.Instantiate<GameObject>(gameObject, position, Quaternion.identity);
				}
				gameObject2.GetComponent<PhotonView>().ViewID = (int)array[3];
			}
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x0007AD50 File Offset: 0x00078F50
		protected virtual void GetSpawnPoint(out Vector3 spawnPos, out Quaternion spawnRot)
		{
			Transform spawnPoint = this.GetSpawnPoint();
			if (spawnPoint != null)
			{
				spawnPos = spawnPoint.position;
				spawnRot = spawnPoint.rotation;
			}
			else
			{
				spawnPos = new Vector3(0f, 0f, 0f);
				spawnRot = new Quaternion(0f, 0f, 0f, 1f);
			}
			if (this.UseRandomOffset)
			{
				Debug.Log("Set Seed");
				Random.InitState((int)(Time.time * 10000f));
				Vector3 a = Random.insideUnitSphere;
				a.y = 0f;
				a = a.normalized;
				spawnPos += this.PositionOffset * a;
			}
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x0007AE1C File Offset: 0x0007901C
		protected virtual Transform GetSpawnPoint()
		{
			if (this.SpawnPoints == null || this.SpawnPoints.Count == 0)
			{
				return null;
			}
			switch (this.Sequence)
			{
			case CharacterInstantiation.SpawnSequence.Connection:
			{
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				return this.SpawnPoints[(actorNumber == -1) ? 0 : (actorNumber % this.SpawnPoints.Count)];
			}
			case CharacterInstantiation.SpawnSequence.Random:
				return this.SpawnPoints[Random.Range(0, this.SpawnPoints.Count)];
			case CharacterInstantiation.SpawnSequence.RoundRobin:
				this.lastUsedSpawnPointIndex++;
				if (this.lastUsedSpawnPointIndex >= this.SpawnPoints.Count)
				{
					this.lastUsedSpawnPointIndex = 0;
				}
				return this.SpawnPoints[this.lastUsedSpawnPointIndex];
			default:
				return null;
			}
		}

		// Token: 0x0400163E RID: 5694
		public Transform SpawnPosition;

		// Token: 0x0400163F RID: 5695
		public float PositionOffset = 2f;

		// Token: 0x04001640 RID: 5696
		public GameObject[] PrefabsToInstantiate;

		// Token: 0x04001641 RID: 5697
		public List<Transform> SpawnPoints;

		// Token: 0x04001642 RID: 5698
		public bool AutoSpawn = true;

		// Token: 0x04001643 RID: 5699
		public bool UseRandomOffset = true;

		// Token: 0x04001644 RID: 5700
		public CharacterInstantiation.SpawnSequence Sequence;

		// Token: 0x04001646 RID: 5702
		[SerializeField]
		private byte manualInstantiationEventCode = 1;

		// Token: 0x04001647 RID: 5703
		protected int lastUsedSpawnPointIndex = -1;

		// Token: 0x04001648 RID: 5704
		[SerializeField]
		private bool manualInstantiation;

		// Token: 0x04001649 RID: 5705
		[SerializeField]
		private bool differentPrefabs;

		// Token: 0x0400164A RID: 5706
		[SerializeField]
		private string localPrefabSuffix;

		// Token: 0x0400164B RID: 5707
		[SerializeField]
		private string remotePrefabSuffix;

		// Token: 0x0200055A RID: 1370
		public enum SpawnSequence
		{
			// Token: 0x04001D28 RID: 7464
			Connection,
			// Token: 0x04001D29 RID: 7465
			Random,
			// Token: 0x04001D2A RID: 7466
			RoundRobin
		}

		// Token: 0x0200055B RID: 1371
		// (Invoke) Token: 0x06001FAA RID: 8106
		public delegate void OnCharacterInstantiated(GameObject character);
	}
}
