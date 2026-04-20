using System;
using System.Collections.Generic;
using System.Linq;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000171 RID: 369
[DefaultExecutionOrder(100000)]
public class Rope : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x06000BEF RID: 3055 RVA: 0x0003FF5A File Offset: 0x0003E15A
	// (set) Token: 0x06000BF0 RID: 3056 RVA: 0x0003FF62 File Offset: 0x0003E162
	public float Segments
	{
		get
		{
			return this.segments;
		}
		set
		{
			this.segments = Mathf.Clamp(value, 0f, (float)Rope.MaxSegments);
		}
	}

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x06000BF1 RID: 3057 RVA: 0x0003FF7B File Offset: 0x0003E17B
	public static int MaxSegments
	{
		get
		{
			return 40;
		}
	}

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000BF2 RID: 3058 RVA: 0x0003FF7F File Offset: 0x0003E17F
	public int SegmentCount
	{
		get
		{
			if (base.photonView.IsMine)
			{
				return this.simulationSegments.Count;
			}
			return this.remoteColliderSegments.Count;
		}
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x0003FFA5 File Offset: 0x0003E1A5
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient)
		{
			this.view.RPC("OnRejoinSyncRPC", newPlayer, new object[]
			{
				this.attachmenState
			});
		}
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x0003FFDA File Offset: 0x0003E1DA
	[PunRPC]
	public void OnRejoinSyncRPC(Rope.ATTACHMENT attachmentState)
	{
		this.attachmenState = attachmentState;
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x0003FFE3 File Offset: 0x0003E1E3
	private void Awake()
	{
		this.itemSpool = base.GetComponentInParent<Item>();
		this.climbingAPI = base.GetComponent<RopeClimbingAPI>();
		this.view = base.GetComponent<PhotonView>();
		this.ropeBoneVisualizer = base.GetComponentInChildren<RopeBoneVisualizer>();
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00040018 File Offset: 0x0003E218
	private void Update()
	{
		bool flag;
		switch (this.attachmenState)
		{
		case Rope.ATTACHMENT.unattached:
			flag = false;
			break;
		case Rope.ATTACHMENT.inSpool:
			flag = false;
			break;
		case Rope.ATTACHMENT.anchored:
			flag = true;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this.isClimbable = flag;
		if (!base.photonView.IsMine || this.creatorLeft)
		{
			if (this.simulationSegments.Count > 0)
			{
				this.Clear(false);
			}
			return;
		}
		this.timeSinceRemoved += Time.deltaTime;
		int num = Mathf.Clamp(Mathf.FloorToInt(this.Segments), 1, int.MaxValue);
		if (this.simulationSegments.Count > num)
		{
			if (this.simulationSegments.Count > 1)
			{
				this.RemoveSegment();
			}
		}
		else if (this.simulationSegments.Count < num)
		{
			this.AddSegment();
		}
		if (this.simulationSegments.Count > 1)
		{
			float t = this.Segments % 1f;
			List<Transform> list = this.simulationSegments;
			ConfigurableJoint component = list[list.Count - 1].GetComponent<ConfigurableJoint>();
			Vector3 b = Vector3.Lerp(this.startAnchorOf2ndSegment, -this.spacing.oxo(), Mathf.Clamp01(this.timeSinceRemoved / this.slurpTime));
			component.connectedAnchor = Vector3.Lerp(this.spacing.oxo(), b, t);
			component.GetComponent<Collider>().enabled = true;
		}
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x0004016C File Offset: 0x0003E36C
	private void FixedUpdate()
	{
		if (!base.photonView.IsMine || this.creatorLeft)
		{
			return;
		}
		if (this.antigrav)
		{
			using (List<Transform>.Enumerator enumerator = this.simulationSegments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Transform transform = enumerator.Current;
					transform.GetComponent<Rigidbody>().AddForce(-Physics.gravity * 2f, ForceMode.Acceleration);
				}
				return;
			}
		}
		foreach (Character character in this.charactersClimbing)
		{
			float ropePercent = character.data.ropePercent;
			this.climbingAPI.GetSegmentFromPercent(ropePercent).GetComponent<Rigidbody>().AddForce(Vector3.down * this.climberGravity, ForceMode.Acceleration);
		}
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x00040260 File Offset: 0x0003E460
	public override void OnEnable()
	{
		base.OnEnable();
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x0004026E File Offset: 0x0003E46E
	public override void OnDisable()
	{
		base.OnDisable();
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x0004027C File Offset: 0x0003E47C
	public List<Transform> GetRopeSegments()
	{
		if (base.photonView.IsMine)
		{
			return this.simulationSegments;
		}
		return this.remoteColliderSegments;
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00040298 File Offset: 0x0003E498
	public bool IsActive()
	{
		bool result = true;
		if (this.itemSpool != null && this.itemSpool.itemState != ItemState.Held)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x000402C8 File Offset: 0x0003E4C8
	[PunRPC]
	public void Detach_Rpc(float segmentLength)
	{
		if (this.spool != null)
		{
			Debug.Log(string.Format("Detaching {0} of rope from spool", segmentLength));
			this.spool.ropeInstance = null;
			this.spool.rope = null;
			this.spool.Segments = 0f;
			this.spool.ClearRope();
			if (base.photonView.IsMine && !Mathf.Approximately(segmentLength, this.segments))
			{
				Debug.LogWarning(string.Format("We own this rope and it should be {0} long but RPC says it's {1}", this.segments, segmentLength));
			}
			if (this.HasAuthority())
			{
				this.spool.RopeFuel -= segmentLength;
			}
		}
		if (this.view.IsMine)
		{
			Object.DestroyImmediate(this.simulationSegments.First<Transform>().GetComponent<ConfigurableJoint>());
		}
		this.spool = null;
		this.attachmenState = Rope.ATTACHMENT.unattached;
		Debug.Log(string.Format("Detach_Rpc: {0}", this.attachmenState));
		this.ropeBoneVisualizer.StartTransform = null;
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x000403DA File Offset: 0x0003E5DA
	public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
	{
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x000403DC File Offset: 0x0003E5DC
	public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		this.creatorLeft = true;
		Debug.Log(string.Format("OnMasterClientSwitched: {0}, isMaster: {1}, frame: {2}", newMasterClient, PhotonNetwork.IsMasterClient, Time.frameCount));
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x00040410 File Offset: 0x0003E610
	public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
	{
		if (targetView != base.photonView)
		{
			return;
		}
		Debug.Log("Trasfered ownership to me");
		this.creatorLeft = true;
		if (this.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			Debug.Log(string.Format("attached to spool, deleting rope: {0}", this.view));
			PhotonNetwork.Destroy(this.view);
		}
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x00040466 File Offset: 0x0003E666
	public void OnOwnershipTransferFailed(PhotonView targetView, Photon.Realtime.Player senderOfFailedRequest)
	{
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x00040468 File Offset: 0x0003E668
	[PunRPC]
	public void AttachToAnchor_Rpc(PhotonView anchorView, float ropeLength)
	{
		if (this.ropeBoneVisualizer == null)
		{
			this.ropeBoneVisualizer = base.GetComponentInChildren<RopeBoneVisualizer>();
		}
		if (this.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			this.Detach_Rpc(ropeLength);
		}
		this.attachedToAnchor = anchorView.GetComponent<RopeAnchor>();
		this.attachmenState = Rope.ATTACHMENT.anchored;
		Debug.Log(string.Format("AttachToAnchor_Rpc: {0}", this.attachmenState));
		this.ropeBoneVisualizer.StartTransform = this.attachedToAnchor.anchorPoint;
		if (!base.photonView.IsMine)
		{
			return;
		}
		List<Transform> ropeSegments = this.GetRopeSegments();
		if (ropeSegments.Count > 0)
		{
			ropeSegments[0].GetComponent<RopeSegment>().Tie(this.attachedToAnchor.anchorPoint.position);
		}
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00040521 File Offset: 0x0003E721
	public float GetLengthInMeters()
	{
		return Rope.GetLengthInMeters((float)this.GetRopeSegments().Count);
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x00040534 File Offset: 0x0003E734
	public static float GetLengthInMeters(float segmentCount)
	{
		return segmentCount * 0.25f;
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x00040540 File Offset: 0x0003E740
	[PunRPC]
	public void AttachToSpool_Rpc(PhotonView viewSpool)
	{
		this.spool = viewSpool.GetComponent<RopeSpool>();
		if (this.spool == null)
		{
			Debug.LogError("Spool is null");
			return;
		}
		this.spool.ropeInstance = base.gameObject;
		this.spool.rope = this;
		this.ropeBoneVisualizer.StartTransform = this.spool.ropeStart;
		base.transform.position = this.spool.ropeBase.position;
		base.transform.rotation = this.spool.ropeBase.rotation;
		this.attachmenState = Rope.ATTACHMENT.inSpool;
		Debug.Log(string.Format("AttachToSpool_Rpc: {0}", this.attachmenState));
		this.Segments = 0f;
		Physics.SyncTransforms();
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x0004060C File Offset: 0x0003E80C
	public void AddSegment()
	{
		bool flag = this.simulationSegments.Count == 0;
		Transform transform = null;
		if (!flag)
		{
			transform = this.simulationSegments[0];
		}
		Vector3 position = flag ? base.transform.position : transform.transform.position;
		Quaternion rotation = flag ? base.transform.rotation : transform.transform.rotation;
		GameObject gameObject = Object.Instantiate<GameObject>(this.ropeSegmentPrefab, position, rotation, base.transform);
		gameObject.gameObject.name = "RopeSegment: " + this.simulationSegments.Count.ToString();
		ConfigurableJoint component = gameObject.GetComponent<ConfigurableJoint>();
		if (flag)
		{
			component.autoConfigureConnectedAnchor = true;
			if (this.spool != null)
			{
				component.transform.position = this.spool.ropeBase.position;
				component.transform.rotation = this.spool.ropeBase.rotation;
				component.autoConfigureConnectedAnchor = true;
				component.connectedBody = this.spool.rig;
				component.angularXMotion = ConfigurableJointMotion.Limited;
				component.angularXLimitSpring = new SoftJointLimitSpring
				{
					spring = 35f,
					damper = 45f
				};
				component.angularYZLimitSpring = new SoftJointLimitSpring
				{
					spring = 35f,
					damper = 45f
				};
				component.angularZMotion = ConfigurableJointMotion.Limited;
			}
		}
		else
		{
			component.connectedBody = transform.GetComponent<Rigidbody>();
		}
		this.simulationSegments.Add(gameObject.transform);
		if (this.simulationSegments.Count > 2)
		{
			List<Transform> list = this.simulationSegments;
			Component component2 = list[list.Count - 2];
			Rigidbody component3 = gameObject.GetComponent<Rigidbody>();
			ConfigurableJoint component4 = component2.GetComponent<ConfigurableJoint>();
			component4.connectedBody = component3;
			this.startAnchorOf2ndSegment = new Vector3(0f, -this.spacing, 0f);
			component4.connectedAnchor = this.startAnchorOf2ndSegment;
		}
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x00040808 File Offset: 0x0003EA08
	private void RemoveSegment()
	{
		List<Transform> list = this.simulationSegments;
		Transform transform = list[list.Count - 1];
		List<Transform> list2 = this.simulationSegments;
		Transform transform2 = list2[list2.Count - 2];
		Transform transform3 = this.simulationSegments[0];
		Object.DestroyImmediate(transform.gameObject);
		this.simulationSegments.RemoveLast<Transform>();
		ConfigurableJoint component = transform2.GetComponent<ConfigurableJoint>();
		if (transform2 == transform3)
		{
			Debug.LogError("Attempting to connect joint to itself");
			return;
		}
		this.timeSinceRemoved = 0f;
		component.connectedBody = transform3.GetComponent<Rigidbody>();
		this.startAnchorOf2ndSegment = transform3.InverseTransformPoint(component.transform.position);
		component.connectedAnchor = this.startAnchorOf2ndSegment;
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x000408B4 File Offset: 0x0003EAB4
	public RopeSyncData GetSyncData()
	{
		RopeSyncData ropeSyncData = new RopeSyncData
		{
			isVisible = this.isClimbable,
			segments = new RopeSyncData.SegmentData[this.simulationSegments.Count]
		};
		for (int i = 0; i < this.simulationSegments.Count; i++)
		{
			ropeSyncData.segments[i] = new RopeSyncData.SegmentData
			{
				position = this.simulationSegments[i].position,
				rotation = this.simulationSegments[i].rotation
			};
		}
		return ropeSyncData;
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00040950 File Offset: 0x0003EB50
	public void SetSyncData(RopeSyncData data)
	{
		if (data.updateVisualizerManually)
		{
			this.ropeBoneVisualizer.ManuallyUpdateNextFrame = Optionable<bool>.Some(true);
		}
		if (this.creatorLeft)
		{
			return;
		}
		this.isClimbable = data.isVisible;
		int num = data.segments.Length;
		int count = this.remoteColliderSegments.Count;
		if (num < count)
		{
			int num2 = count - num;
			for (int i = 0; i < num2; i++)
			{
				List<Transform> list = this.remoteColliderSegments;
				Component component = list[list.Count - 1];
				this.remoteColliderSegments.RemoveLast<Transform>();
				Object.Destroy(component.gameObject);
			}
		}
		else if (num > count)
		{
			int num3 = num - count;
			for (int j = 0; j < num3; j++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.remoteSegmentPrefab, Vector3.zero, Quaternion.identity, base.transform);
				gameObject.GetComponent<RopeSegment>().rope = this;
				this.remoteColliderSegments.Add(gameObject.transform);
			}
		}
		if (num != this.remoteColliderSegments.Count)
		{
			Debug.LogError("Remote Segment Logic Failed");
			return;
		}
		for (int k = 0; k < data.segments.Length; k++)
		{
			this.remoteColliderSegments[k].position = data.segments[k].position;
			this.remoteColliderSegments[k].rotation = data.segments[k].rotation;
		}
		this.ropeBoneVisualizer.SetData(data);
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x00040ABD File Offset: 0x0003ECBD
	public float GetTotalLength()
	{
		return (float)this.SegmentCount * this.spacing;
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x00040AD0 File Offset: 0x0003ECD0
	public void Clear(bool alsoRemoveRemote = false)
	{
		Debug.Log("Rope Clear!");
		if (this.simulationSegments.Count > 0)
		{
			for (int i = this.simulationSegments.Count - 1; i >= 0; i--)
			{
				Object.Destroy(this.simulationSegments[i].gameObject);
			}
			this.simulationSegments.Clear();
		}
		if (alsoRemoveRemote)
		{
			for (int j = this.remoteColliderSegments.Count - 1; j >= 0; j--)
			{
				Object.Destroy(this.remoteColliderSegments[j].gameObject);
			}
			this.remoteColliderSegments.Clear();
		}
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x00040B6A File Offset: 0x0003ED6A
	public void AddCharacterClimbing(Character character)
	{
		this.charactersClimbing.Add(character);
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x00040B78 File Offset: 0x0003ED78
	public void RemoveCharacterClimbing(Character character)
	{
		this.charactersClimbing.Remove(character);
	}

	// Token: 0x04000AEE RID: 2798
	public float spacing = 0.75f;

	// Token: 0x04000AEF RID: 2799
	public float climberGravity = 1f;

	// Token: 0x04000AF0 RID: 2800
	public float slurpTime = 10f;

	// Token: 0x04000AF1 RID: 2801
	public bool antigrav;

	// Token: 0x04000AF2 RID: 2802
	public bool isHelicopterRope;

	// Token: 0x04000AF3 RID: 2803
	public GameObject ropeSegmentPrefab;

	// Token: 0x04000AF4 RID: 2804
	public GameObject remoteSegmentPrefab;

	// Token: 0x04000AF5 RID: 2805
	public Rope.ATTACHMENT attachmenState;

	// Token: 0x04000AF6 RID: 2806
	public bool isClimbable;

	// Token: 0x04000AF7 RID: 2807
	public PhotonView view;

	// Token: 0x04000AF8 RID: 2808
	private readonly List<Transform> remoteColliderSegments = new List<Transform>();

	// Token: 0x04000AF9 RID: 2809
	private readonly List<Transform> simulationSegments = new List<Transform>();

	// Token: 0x04000AFA RID: 2810
	[NonSerialized]
	public List<Character> charactersClimbing = new List<Character>();

	// Token: 0x04000AFB RID: 2811
	[NonSerialized]
	public RopeClimbingAPI climbingAPI;

	// Token: 0x04000AFC RID: 2812
	private Item itemSpool;

	// Token: 0x04000AFD RID: 2813
	private RopeBoneVisualizer ropeBoneVisualizer;

	// Token: 0x04000AFE RID: 2814
	private float segments;

	// Token: 0x04000AFF RID: 2815
	private RopeSpool spool;

	// Token: 0x04000B00 RID: 2816
	private Vector3 startAnchorOf2ndSegment;

	// Token: 0x04000B01 RID: 2817
	private float timeSinceRemoved;

	// Token: 0x04000B02 RID: 2818
	public bool creatorLeft;

	// Token: 0x04000B03 RID: 2819
	private RopeAnchor attachedToAnchor;

	// Token: 0x020004A3 RID: 1187
	public enum ATTACHMENT
	{
		// Token: 0x04001A4B RID: 6731
		unattached,
		// Token: 0x04001A4C RID: 6732
		inSpool,
		// Token: 0x04001A4D RID: 6733
		anchored
	}
}
