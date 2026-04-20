using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200017C RID: 380
public class RopeSyncer : PhotonBinaryStreamSerializer<RopeSyncData>
{
	// Token: 0x06000C60 RID: 3168 RVA: 0x0004288E File Offset: 0x00040A8E
	protected override void Awake()
	{
		if (!this.rope)
		{
			this.rope = base.GetComponent<Rope>();
		}
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x000428AC File Offset: 0x00040AAC
	public override RopeSyncData GetDataToWrite()
	{
		RopeSyncData syncData = this.rope.GetSyncData();
		syncData.updateVisualizerManually = this.updateVisualizerManually;
		return syncData;
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x000428D3 File Offset: 0x00040AD3
	public override void OnDataReceived(RopeSyncData data)
	{
		base.OnDataReceived(data);
		this.rope.SetSyncData(data);
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x000428E8 File Offset: 0x00040AE8
	public override bool ShouldSendData()
	{
		if (this.rope.isClimbable && this.startSyncTime.IsNone)
		{
			this.startSyncTime = Optionable<float>.Some(Time.realtimeSinceStartup);
		}
		List<Transform> ropeSegments = this.rope.GetRopeSegments();
		if (ropeSegments.Count == 0)
		{
			return false;
		}
		Vector3 position = ropeSegments[0].position;
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		if (allPlayerCharacters.Count == 0)
		{
			return false;
		}
		float num = float.MaxValue;
		foreach (Character character in allPlayerCharacters)
		{
			float b = Vector3.Distance(character.Center, position);
			num = Mathf.Min(num, b);
		}
		if (num > 100f)
		{
			return false;
		}
		if (this.startSyncTime.IsSome && Time.realtimeSinceStartup - this.startSyncTime.Value > 60f)
		{
			this.updateVisualizerManually = true;
			this.syncIndex++;
			if (this.syncIndex < 600)
			{
				return false;
			}
			this.syncIndex = 0;
		}
		return !this.rope.creatorLeft;
	}

	// Token: 0x04000B50 RID: 2896
	public Rope rope;

	// Token: 0x04000B51 RID: 2897
	public Optionable<float> startSyncTime = Optionable<float>.None;

	// Token: 0x04000B52 RID: 2898
	private int syncIndex;

	// Token: 0x04000B53 RID: 2899
	private bool updateVisualizerManually;
}
