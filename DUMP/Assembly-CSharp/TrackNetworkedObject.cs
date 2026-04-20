using System;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class TrackNetworkedObject : MonoBehaviour
{
	// Token: 0x06000E38 RID: 3640 RVA: 0x000475E4 File Offset: 0x000457E4
	private void OnEnable()
	{
		TrackableNetworkObject.OnTrackableObjectCreated = (Action<int>)Delegate.Combine(TrackableNetworkObject.OnTrackableObjectCreated, new Action<int>(this.TryReattachToTrackedObject));
		TrackableNetworkObject.OnTrackableObjectConsumed = (Action<int>)Delegate.Combine(TrackableNetworkObject.OnTrackableObjectConsumed, new Action<int>(this.TryConsumeTrackedObject));
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x00047634 File Offset: 0x00045834
	private void OnDisable()
	{
		TrackableNetworkObject.OnTrackableObjectCreated = (Action<int>)Delegate.Remove(TrackableNetworkObject.OnTrackableObjectCreated, new Action<int>(this.TryReattachToTrackedObject));
		TrackableNetworkObject.OnTrackableObjectConsumed = (Action<int>)Delegate.Remove(TrackableNetworkObject.OnTrackableObjectConsumed, new Action<int>(this.TryConsumeTrackedObject));
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x00047681 File Offset: 0x00045881
	private void TryReattachToTrackedObject(int ID)
	{
		this.TryGetTrackedObject();
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x00047689 File Offset: 0x00045889
	private void TryConsumeTrackedObject(int ID)
	{
		if (this.trackedObjectID == ID)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x000476A0 File Offset: 0x000458A0
	private void TryGetTrackedObject()
	{
		if (this.trackedObjectID == -1)
		{
			Debug.LogError("TrackNetworkObject has a value of -1. This should never happen.");
			base.enabled = false;
			return;
		}
		TrackableNetworkObject trackableObject = TrackableNetworkObject.GetTrackableObject(this.trackedObjectID);
		if (trackableObject != null)
		{
			this.SetObject(trackableObject);
			this.lostTrackableTick = 0f;
			return;
		}
		this.lostTrackableTick += Time.deltaTime;
		if (this.lostTrackableTick > 0.3f)
		{
			Debug.Log(string.Format("Object {0} timed out. Destroying...", base.gameObject.GetHashCode()));
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x0004773C File Offset: 0x0004593C
	public void SetObject(TrackableNetworkObject trackableObject)
	{
		this.trackedObject = trackableObject;
		this.trackedObjectID = trackableObject.instanceID;
		this.trackedObject.currentTracker = this;
		Debug.Log(string.Format("Object {0} Reconnected to trackable object {1} with photon ID {2}", base.gameObject.GetHashCode(), this.trackedObjectID, trackableObject.photonView.ViewID));
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x000477A4 File Offset: 0x000459A4
	private void LateUpdate()
	{
		if (this.trackedObject == null)
		{
			this.TryGetTrackedObject();
		}
		if (this.trackedObject != null)
		{
			base.transform.rotation = this.trackedObject.transform.rotation;
			base.transform.position = this.trackedObject.transform.TransformPoint(this.offset);
		}
	}

	// Token: 0x04000BF9 RID: 3065
	public int trackedObjectID = -1;

	// Token: 0x04000BFA RID: 3066
	public TrackableNetworkObject trackedObject;

	// Token: 0x04000BFB RID: 3067
	public Vector3 offset;

	// Token: 0x04000BFC RID: 3068
	private float lostTrackableTick;
}
