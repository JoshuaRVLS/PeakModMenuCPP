using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020002CE RID: 718
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ProximityVoiceTrigger : VoiceComponent
{
	// Token: 0x1700014A RID: 330
	// (get) Token: 0x0600142B RID: 5163 RVA: 0x00065E8E File Offset: 0x0006408E
	public byte TargetInterestGroup
	{
		get
		{
			if (this.photonView != null)
			{
				return (byte)this.photonView.OwnerActorNr;
			}
			return 0;
		}
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x00065EAC File Offset: 0x000640AC
	protected override void Awake()
	{
		this.photonVoiceView = base.GetComponentInParent<PhotonVoiceView>();
		this.photonView = base.GetComponentInParent<PhotonView>();
		base.GetComponent<Collider>().isTrigger = true;
		this.IsLocalCheck();
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x00065EDC File Offset: 0x000640DC
	private void ToggleTransmission()
	{
		if (this.photonVoiceView.RecorderInUse != null)
		{
			byte targetInterestGroup = this.TargetInterestGroup;
			if (this.photonVoiceView.RecorderInUse.InterestGroup != targetInterestGroup)
			{
				base.Logger.Log(LogLevel.Info, "Setting RecorderInUse's InterestGroup to {0}", new object[]
				{
					targetInterestGroup
				});
				this.photonVoiceView.RecorderInUse.InterestGroup = targetInterestGroup;
			}
			this.photonVoiceView.RecorderInUse.RecordingEnabled = true;
		}
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x00065F58 File Offset: 0x00064158
	private void OnTriggerEnter(Collider other)
	{
		if (this.IsLocalCheck())
		{
			ProximityVoiceTrigger component = other.GetComponent<ProximityVoiceTrigger>();
			if (component != null)
			{
				byte targetInterestGroup = component.TargetInterestGroup;
				base.Logger.Log(LogLevel.Debug, "OnTriggerEnter {0}", new object[]
				{
					targetInterestGroup
				});
				if (targetInterestGroup == this.TargetInterestGroup)
				{
					return;
				}
				if (targetInterestGroup == 0)
				{
					return;
				}
				if (!this.groupsToAdd.Contains(targetInterestGroup))
				{
					this.groupsToAdd.Add(targetInterestGroup);
				}
			}
		}
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x00065FCC File Offset: 0x000641CC
	private void OnTriggerExit(Collider other)
	{
		if (this.IsLocalCheck())
		{
			ProximityVoiceTrigger component = other.GetComponent<ProximityVoiceTrigger>();
			if (component != null)
			{
				byte targetInterestGroup = component.TargetInterestGroup;
				base.Logger.Log(LogLevel.Debug, "OnTriggerExit {0}", new object[]
				{
					targetInterestGroup
				});
				if (targetInterestGroup == this.TargetInterestGroup)
				{
					return;
				}
				if (targetInterestGroup == 0)
				{
					return;
				}
				if (this.groupsToAdd.Contains(targetInterestGroup))
				{
					this.groupsToAdd.Remove(targetInterestGroup);
				}
				if (!this.groupsToRemove.Contains(targetInterestGroup))
				{
					this.groupsToRemove.Add(targetInterestGroup);
				}
			}
		}
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x0006605C File Offset: 0x0006425C
	protected void Update()
	{
		if (!PunVoiceClient.Instance.Client.InRoom)
		{
			this.subscribedGroups = null;
			return;
		}
		if (this.IsLocalCheck())
		{
			if (this.groupsToAdd.Count > 0 || this.groupsToRemove.Count > 0)
			{
				byte[] array = null;
				byte[] array2 = null;
				if (this.groupsToAdd.Count > 0)
				{
					array = this.groupsToAdd.ToArray();
				}
				if (this.groupsToRemove.Count > 0)
				{
					array2 = this.groupsToRemove.ToArray();
				}
				base.Logger.Log(LogLevel.Info, "client of actor number {0} trying to change groups, to_be_removed#={1} to_be_added#={2}", new object[]
				{
					this.TargetInterestGroup,
					this.groupsToRemove.Count,
					this.groupsToAdd.Count
				});
				if (PunVoiceClient.Instance.Client.OpChangeGroups(array2, array))
				{
					if (this.subscribedGroups != null)
					{
						List<byte> list = new List<byte>();
						for (int i = 0; i < this.subscribedGroups.Length; i++)
						{
							list.Add(this.subscribedGroups[i]);
						}
						for (int j = 0; j < this.groupsToRemove.Count; j++)
						{
							if (list.Contains(this.groupsToRemove[j]))
							{
								list.Remove(this.groupsToRemove[j]);
							}
						}
						for (int k = 0; k < this.groupsToAdd.Count; k++)
						{
							if (!list.Contains(this.groupsToAdd[k]))
							{
								list.Add(this.groupsToAdd[k]);
							}
						}
						this.subscribedGroups = list.ToArray();
					}
					else
					{
						this.subscribedGroups = array;
					}
					this.groupsToAdd.Clear();
					this.groupsToRemove.Clear();
				}
				else
				{
					base.Logger.Log(LogLevel.Error, "Error changing groups", Array.Empty<object>());
				}
			}
			this.ToggleTransmission();
		}
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x00066248 File Offset: 0x00064448
	private bool IsLocalCheck()
	{
		if (this.photonView.IsMine)
		{
			return true;
		}
		if (base.enabled)
		{
			base.Logger.Log(LogLevel.Info, "Disabling ProximityVoiceTrigger as does not belong to local player, actor number {0}", new object[]
			{
				this.TargetInterestGroup
			});
			base.enabled = false;
		}
		return false;
	}

	// Token: 0x0400125F RID: 4703
	private List<byte> groupsToAdd = new List<byte>();

	// Token: 0x04001260 RID: 4704
	private List<byte> groupsToRemove = new List<byte>();

	// Token: 0x04001261 RID: 4705
	[SerializeField]
	private byte[] subscribedGroups;

	// Token: 0x04001262 RID: 4706
	private PhotonVoiceView photonVoiceView;

	// Token: 0x04001263 RID: 4707
	private PhotonView photonView;
}
