using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Zorro.PhotonUtility;

// Token: 0x02000292 RID: 658
public class LavaRising : Singleton<LavaRising>
{
	// Token: 0x1700013B RID: 315
	// (get) Token: 0x060012F4 RID: 4852 RVA: 0x0005FC39 File Offset: 0x0005DE39
	// (set) Token: 0x060012F5 RID: 4853 RVA: 0x0005FC41 File Offset: 0x0005DE41
	public float timeTraveled { get; set; }

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x060012F6 RID: 4854 RVA: 0x0005FC4A File Offset: 0x0005DE4A
	// (set) Token: 0x060012F7 RID: 4855 RVA: 0x0005FC52 File Offset: 0x0005DE52
	public bool started { get; set; }

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x060012F8 RID: 4856 RVA: 0x0005FC5B File Offset: 0x0005DE5B
	// (set) Token: 0x060012F9 RID: 4857 RVA: 0x0005FC63 File Offset: 0x0005DE63
	public bool ended { get; set; }

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x060012FA RID: 4858 RVA: 0x0005FC6C File Offset: 0x0005DE6C
	// (set) Token: 0x060012FB RID: 4859 RVA: 0x0005FC74 File Offset: 0x0005DE74
	public float secondsWaitedToStart { get; set; }

	// Token: 0x060012FC RID: 4860 RVA: 0x0005FC7D File Offset: 0x0005DE7D
	protected override void Awake()
	{
		base.Awake();
		if (this.debug)
		{
			this.initialWaitTime = this.debugInitialWaitTime;
			this.travelTime = this.debugTravelTime;
		}
	}

	// Token: 0x060012FD RID: 4861 RVA: 0x0005FCA5 File Offset: 0x0005DEA5
	private void Start()
	{
		this.startHeight = this.lava.transform.position.y;
		Debug.Log("Initialized lava height: " + this.startHeight.ToString());
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x0005FCDC File Offset: 0x0005DEDC
	public override void OnDestroy()
	{
		base.OnDestroy();
		CustomCommands<CustomCommandType>.UnregisterListener(this.debugCommandHandle);
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x0005FCF0 File Offset: 0x0005DEF0
	private void Update()
	{
		if (RunSettings.GetValue(RunSettings.SETTINGTYPE.Hazard_TheLavaRises, false) == 0)
		{
			base.enabled = false;
			return;
		}
		if ((PhotonNetwork.IsMasterClient && Singleton<MapHandler>.Instance.GetCurrentSegment() == Segment.TheKiln) || Singleton<MapHandler>.Instance.GetCurrentSegment() == Segment.Peak)
		{
			bool flag = false;
			this.syncTime += Time.deltaTime;
			if (!this.started && this.secondsWaitedToStart <= 0f && Ascents.fogEnabled)
			{
				this.StartWaiting();
			}
			if (!this.started && this.secondsWaitedToStart > 0f)
			{
				if (!Singleton<OrbFogHandler>.Instance.PlayersAreResting)
				{
					this.secondsWaitedToStart += Time.deltaTime;
				}
				if (this.secondsWaitedToStart > this.initialWaitTime && Ascents.fogEnabled)
				{
					flag = true;
					this.started = true;
				}
			}
			if (this.syncTime > 15f)
			{
				this.syncTime = 0f;
				flag = true;
			}
			if (flag)
			{
				Debug.Log("Syncing Lava Rising to others...");
				GameUtils.instance.SyncLava(this.started, this.ended, this.timeTraveled, this.secondsWaitedToStart);
			}
		}
		if (this.started && !this.ended)
		{
			if (!this.shownLavaRisingMessage)
			{
				GUIManager.instance.TheLavaRises();
				GamefeelHandler.instance.AddPerlinShake(5f, 3f, 15f);
				this.shownLavaRisingMessage = true;
				Debug.Log("Lava rising started.");
			}
			this.timeTraveled += Time.deltaTime;
			float y = Mathf.Lerp(this.startHeight, this.topTransform.position.y, this.timeTraveled / this.travelTime);
			this.lava.MovePosition(new Vector3(this.lava.transform.position.x, y, this.lava.transform.position.z));
			if (this.timeTraveled > this.travelTime)
			{
				this.EndRising();
			}
		}
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x0005FEE4 File Offset: 0x0005E0E4
	public void RecieveLavaData(bool started, bool ended, float time, float timeWaited)
	{
		this.started = started;
		this.ended = ended;
		this.timeTraveled = time;
		this.secondsWaitedToStart = timeWaited;
		Debug.Log(string.Format("Handle Lava Rising package: started: {0}, ended: {1}, seconds waited: {2}, time traveled: {3} starting position: {4} total time: {5}", new object[]
		{
			started,
			ended,
			this.secondsWaitedToStart,
			this.timeTraveled,
			this.startHeight,
			this.travelTime
		}));
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x0005FF6D File Offset: 0x0005E16D
	public void StartWaiting()
	{
		if (this.secondsWaitedToStart > 0f)
		{
			Debug.LogError("Tried to start waiting for lava rising but already rising!");
			return;
		}
		Debug.Log("Starting wait for lava rising");
		this.secondsWaitedToStart = Time.deltaTime;
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x0005FF9C File Offset: 0x0005E19C
	private void EndRising()
	{
		Debug.Log("Ending lava rising.");
		this.ended = true;
	}

	// Token: 0x04001111 RID: 4369
	public Rigidbody lava;

	// Token: 0x04001112 RID: 4370
	public Transform topTransform;

	// Token: 0x04001113 RID: 4371
	public float initialWaitTime = 1f;

	// Token: 0x04001114 RID: 4372
	public float travelTime = 60f;

	// Token: 0x04001119 RID: 4377
	public bool debug;

	// Token: 0x0400111A RID: 4378
	public float debugInitialWaitTime = 1f;

	// Token: 0x0400111B RID: 4379
	public float debugTravelTime = 60f;

	// Token: 0x0400111C RID: 4380
	private bool shownLavaRisingMessage;

	// Token: 0x0400111D RID: 4381
	private ListenerHandle debugCommandHandle;

	// Token: 0x0400111E RID: 4382
	private float syncTime;

	// Token: 0x0400111F RID: 4383
	[SerializeField]
	private float startHeight;
}
