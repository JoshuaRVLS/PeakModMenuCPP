using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000311 RID: 785
[Serializable]
public abstract class PostSpawnBehavior : IMayHaveDeferredStep
{
	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06001505 RID: 5381 RVA: 0x00069FEF File Offset: 0x000681EF
	protected virtual DeferredStepTiming DefaultTiming
	{
		get
		{
			return DeferredStepTiming.None;
		}
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06001506 RID: 5382 RVA: 0x00069FF2 File Offset: 0x000681F2
	public virtual DeferredStepTiming DeferredTiming
	{
		get
		{
			if (this._timing != DeferredStepTiming.None)
			{
				return this._timing;
			}
			return this.DefaultTiming;
		}
	}

	// Token: 0x06001507 RID: 5383
	public abstract void RunBehavior(IEnumerable<GameObject> spawned);

	// Token: 0x06001508 RID: 5384 RVA: 0x0006A00C File Offset: 0x0006820C
	public IDeferredStep ConstructDeferred(IMayHaveDeferredStep parent)
	{
		if (this.DeferredTiming == DeferredStepTiming.None)
		{
			Debug.LogError(string.Format("Can't construct a deferred execution if timing is {0}", DeferredStepTiming.None));
			return null;
		}
		if (this.mute)
		{
			Debug.LogError("Can't construct a deferred execution of a muted step");
			return null;
		}
		PropSpawner propSpawner = parent as PropSpawner;
		if (propSpawner == null)
		{
			Debug.LogError("Assumed we could only get here from a prop spawning parent. Did something change?");
			return null;
		}
		return new PostSpawnBehavior.PostSpawnBehaviorDeferredRunner(this, propSpawner.syncTransforms, propSpawner.SpawnedProps);
	}

	// Token: 0x0400132A RID: 4906
	[SerializeField]
	protected DeferredStepTiming _timing;

	// Token: 0x0400132B RID: 4907
	public bool mute;

	// Token: 0x0200052E RID: 1326
	public readonly struct PostSpawnBehaviorDeferredRunner : IDeferredStep
	{
		// Token: 0x06001F1C RID: 7964 RVA: 0x0008EC7F File Offset: 0x0008CE7F
		public PostSpawnBehaviorDeferredRunner(PostSpawnBehavior behavior, bool syncTransforms, IEnumerable<GameObject> spawnedObjects)
		{
			this._syncTransforms = syncTransforms;
			this._spawnedObjects = spawnedObjects.ToList<GameObject>();
			this._behavior = behavior;
		}

		// Token: 0x06001F1D RID: 7965 RVA: 0x0008EC9B File Offset: 0x0008CE9B
		public void DeferredGo()
		{
			if (this._syncTransforms)
			{
				Physics.SyncTransforms();
			}
			this._behavior.RunBehavior(this._spawnedObjects);
		}

		// Token: 0x04001C86 RID: 7302
		private readonly bool _syncTransforms;

		// Token: 0x04001C87 RID: 7303
		private readonly List<GameObject> _spawnedObjects;

		// Token: 0x04001C88 RID: 7304
		private readonly PostSpawnBehavior _behavior;
	}
}
