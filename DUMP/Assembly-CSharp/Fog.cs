using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200026A RID: 618
public class Fog : MonoBehaviour
{
	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06001235 RID: 4661 RVA: 0x0005B309 File Offset: 0x00059509
	private bool IsInFog
	{
		get
		{
			return Character.localCharacter.Center.y < base.transform.position.y;
		}
	}

	// Token: 0x06001236 RID: 4662 RVA: 0x0005B32C File Offset: 0x0005952C
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001237 RID: 4663 RVA: 0x0005B33C File Offset: 0x0005953C
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (this.stops == null)
		{
			Debug.LogError("Disabling fog movement: No stops were found");
			base.enabled = false;
			return;
		}
		this.Movement();
		this.MakePlayerCold();
		this.ApplyVisuals();
		if (this.view.IsMine)
		{
			this.Sync();
		}
		if (this.fogParticles == null)
		{
			return;
		}
		this.fogParticles.transform.position = Character.localCharacter.Center;
		if (this.IsInFog)
		{
			this.fogParticles.Play();
			Character.localCharacter.data.isInFog = true;
			return;
		}
		this.fogParticles.Stop();
		Character.localCharacter.data.isInFog = false;
	}

	// Token: 0x06001238 RID: 4664 RVA: 0x0005B400 File Offset: 0x00059600
	private void Sync()
	{
		this.syncCounter += Time.deltaTime;
		if (this.syncCounter > 5f)
		{
			this.syncCounter = 0f;
			this.view.RPC("RPCA_SyncFog", RpcTarget.Others, new object[]
			{
				this.fogHeight
			});
		}
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x0005B45C File Offset: 0x0005965C
	private void ApplyVisuals()
	{
		base.transform.position = new Vector3(Character.localCharacter.Center.x, this.fogHeight, Mathf.Clamp(Character.localCharacter.Center.z, -10000f, 870f));
		Shader.SetGlobalFloat(Fog.FogHeight, base.transform.position.y);
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x0005B4C8 File Offset: 0x000596C8
	private void MakePlayerCold()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (this.IsInFog)
		{
			if (Character.localCharacter.data.isSkeleton)
			{
				float num = this.amount / 8f * Time.deltaTime;
				Debug.Log(string.Format("Adding {0} injury to skeleton", num));
				Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, num, false, true, true);
				return;
			}
			Debug.Log("Adding cold to player in fog");
			Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, this.amount * Time.deltaTime, false, true, true);
		}
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x0005B571 File Offset: 0x00059771
	private void Movement()
	{
		if (this.waiting)
		{
			this.Wait();
			return;
		}
		this.Move();
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x0005B588 File Offset: 0x00059788
	private void Wait()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.sinceStop += Time.deltaTime;
		if (this.TimeToMove() || this.PlayersHaveMovedOn())
		{
			this.view.RPC("RPCA_Resume", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x0600123D RID: 4669 RVA: 0x0005B5DB File Offset: 0x000597DB
	private bool TimeToMove()
	{
		return this.sinceStop > this.maxWaitTime && this.currentStop > 0;
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x0005B5F8 File Offset: 0x000597F8
	private bool PlayersHaveMovedOn()
	{
		if (Character.AllCharacters.Count == 0)
		{
			return false;
		}
		float num = this.StopHeight() + this.startMoveHeightThreshold;
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (Character.AllCharacters[i].Center.y < num)
			{
				return false;
			}
		}
		Debug.Log("Players have moved on");
		return true;
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x0005B65B File Offset: 0x0005985B
	[PunRPC]
	private void RPCA_Resume()
	{
		this.currentStop++;
		this.waiting = false;
		GUIManager.instance.TheFogRises();
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x0005B67C File Offset: 0x0005987C
	private void Move()
	{
		if (this.currentStop >= this.stops.Length)
		{
			return;
		}
		this.fogHeight += Time.deltaTime * this.fogSpeed;
		if (this.fogHeight > this.StopHeight())
		{
			this.Stop();
		}
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x0005B6BC File Offset: 0x000598BC
	private void Stop()
	{
		this.sinceStop = 0f;
		this.waiting = true;
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x0005B6D0 File Offset: 0x000598D0
	private float StopHeight()
	{
		return this.stops[this.currentStop].transform.position.y;
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x0005B6EE File Offset: 0x000598EE
	[PunRPC]
	public void RPCA_SyncFog(float setHeight)
	{
		this.fogHeight = setHeight;
	}

	// Token: 0x0400103A RID: 4154
	public float fogHeight;

	// Token: 0x0400103B RID: 4155
	public float fogSpeed = 0.4f;

	// Token: 0x0400103C RID: 4156
	public float amount;

	// Token: 0x0400103D RID: 4157
	private static readonly int FogHeight = Shader.PropertyToID("FogHeight");

	// Token: 0x0400103E RID: 4158
	private Transform[] stops;

	// Token: 0x0400103F RID: 4159
	private int currentStop;

	// Token: 0x04001040 RID: 4160
	private float sinceStop;

	// Token: 0x04001041 RID: 4161
	public float maxWaitTime = 180f;

	// Token: 0x04001042 RID: 4162
	public float startMoveHeightThreshold = 60f;

	// Token: 0x04001043 RID: 4163
	private bool waiting;

	// Token: 0x04001044 RID: 4164
	private PhotonView view;

	// Token: 0x04001045 RID: 4165
	public ParticleSystem fogParticles;

	// Token: 0x04001046 RID: 4166
	private float syncCounter;
}
