using System;
using System.Collections.Generic;
using System.Linq;
using Peak.ProcGen;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public class PropSpawner : LevelGenStep, IValidatable
{
	// Token: 0x17000154 RID: 340
	// (get) Token: 0x0600148C RID: 5260 RVA: 0x00067D77 File Offset: 0x00065F77
	public override DeferredStepTiming DeferredTiming
	{
		get
		{
			if (this._deferredSteps.Count <= 0)
			{
				return DeferredStepTiming.None;
			}
			return DeferredStepTiming.AfterCurrentGroupTiming;
		}
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x00067D8A File Offset: 0x00065F8A
	public override IDeferredStep ConstructDeferred(IMayHaveDeferredStep parent)
	{
		if (this != parent)
		{
			Debug.LogError("What da HECK!!!");
			return null;
		}
		return new ExecuteDeferredStepList(this._deferredSteps);
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x00067DAC File Offset: 0x00065FAC
	public void Validate()
	{
		this.ValidationState = this.DoValidation();
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x00067DBA File Offset: 0x00065FBA
	public Color GetValidationColor()
	{
		return this.GetValidationColorImpl();
	}

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06001490 RID: 5264 RVA: 0x00067DC2 File Offset: 0x00065FC2
	// (set) Token: 0x06001491 RID: 5265 RVA: 0x00067DCA File Offset: 0x00065FCA
	public ValidationState ValidationState { get; private set; }

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06001492 RID: 5266 RVA: 0x00067DD3 File Offset: 0x00065FD3
	private Dictionary<GameObject, PropSpawner.SpawnData> AllSpawnData
	{
		get
		{
			return this._propSpawnData.Dict;
		}
	}

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06001493 RID: 5267 RVA: 0x00067DE0 File Offset: 0x00065FE0
	public IEnumerable<GameObject> SpawnedProps
	{
		get
		{
			return this.AllSpawnData.Keys;
		}
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x00067DF0 File Offset: 0x00065FF0
	private void OnDrawGizmosSelected()
	{
		Vector3 vector = base.transform.position + this.area.y * 0.5f * base.transform.up;
		Vector3 vector2 = base.transform.position - this.area.y * 0.5f * base.transform.up;
		Vector3 vector3 = base.transform.position - this.area.x * 0.5f * base.transform.right;
		Vector3 vector4 = base.transform.position + this.area.x * 0.5f * base.transform.right;
		Gizmos.DrawLine(vector2, vector);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(vector2, vector2 + base.transform.forward * this.rayLength + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector, vector + base.transform.forward * this.rayLength + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector3, vector3 + base.transform.forward * this.rayLength + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector4, vector4 + base.transform.forward * this.rayLength + base.transform.forward * this.rayNearCutoff);
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position + base.transform.forward * this.rayLength / 2f, base.transform.rotation, Vector3.one);
		Gizmos.DrawWireCube(Vector3.zero, this.area.xyn(this.rayLength));
		Gizmos.matrix = matrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(vector2, vector2 + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector3, vector3 + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector, vector + base.transform.forward * this.rayNearCutoff);
		Gizmos.DrawLine(vector4, vector4 + base.transform.forward * this.rayNearCutoff);
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x000680D0 File Offset: 0x000662D0
	public void Go()
	{
		this.Clear();
		this.SpawnNew(true);
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x000680DF File Offset: 0x000662DF
	public override void Execute()
	{
		this.Clear();
		this.SpawnNew(false);
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x000680EE File Offset: 0x000662EE
	public void Add()
	{
		this.SpawnNew(true);
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x000680F8 File Offset: 0x000662F8
	private void OnValidate()
	{
		this.validationConstraints = (from c in this.constraints
		where c is IValidationConstraint
		select c).Cast<IValidationConstraint>().Concat((from c in this.postConstraints
		where c is IValidationConstraint
		select c).Cast<IValidationConstraint>()).ToList<IValidationConstraint>();
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x00068174 File Offset: 0x00066374
	public ValidationState DoValidation()
	{
		int num = 0;
		if (this.AllSpawnData.Count == 0 && base.transform.childCount != 0)
		{
			this._madeDummyData = true;
			Debug.Log("Attempted validation without cached prop data. Doing our best to create dummy data.");
			for (int i = 0; i < base.transform.childCount; i++)
			{
				GameObject gameObject = base.transform.GetChild(i).gameObject;
				this.AllSpawnData.Add(gameObject, new PropSpawner.SpawnData
				{
					spawnerTransform = base.transform,
					pos = gameObject.transform.position,
					normal = gameObject.transform.up,
					spawnCount = 1,
					hit = new RaycastHit
					{
						normal = gameObject.transform.up,
						point = gameObject.transform.position
					}
				});
			}
		}
		foreach (KeyValuePair<GameObject, PropSpawner.SpawnData> keyValuePair in this.AllSpawnData)
		{
			GameObject gameObject2;
			PropSpawner.SpawnData spawnData;
			keyValuePair.Deconstruct(out gameObject2, out spawnData);
			GameObject gameObject3 = gameObject2;
			PropSpawner.SpawnData spawnData2 = spawnData;
			bool flag = true;
			foreach (IValidationConstraint validationConstraint in this.validationConstraints)
			{
				if (!validationConstraint.Muted && !validationConstraint.Validate(gameObject3, spawnData2))
				{
					Debug.LogWarning(string.Format("Failed validation constraint {0}: {1}/{2}", validationConstraint.GetType(), base.name, gameObject3.name), gameObject3);
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				num++;
			}
		}
		if (num > 0)
		{
			Debug.LogWarning(string.Format("{0} failed validation on {1} out of {2} props.", base.name, num, this.AllSpawnData.Count), this);
			return ValidationState.Failed;
		}
		if (this.AllSpawnData.Count > 0)
		{
			Debug.Log(string.Format("{0} passed validation on all {1} props!", base.name, this.AllSpawnData.Count), this);
			return ValidationState.Passed;
		}
		Debug.LogWarning(base.name + " didn't have any spawned props to validate", this);
		return ValidationState.Unknown;
	}

	// Token: 0x0600149A RID: 5274 RVA: 0x000683BC File Offset: 0x000665BC
	public void SpawnNew(bool executeDeferredImmediately = false)
	{
		if (this.chanceToUseSpawner < 0.999f && Random.value > this.chanceToUseSpawner)
		{
			return;
		}
		int num = this.nrOfSpawns;
		if (this.randomSpawns)
		{
			num = Random.Range(this.minSpawnCount, this.nrOfSpawns);
		}
		int num2 = 25000;
		int num3 = 5000;
		int num4 = 0;
		while (num4 < num && num2 > 0 && num3 > 0)
		{
			num2--;
			num3--;
			if (this.TryToSpawn(num4))
			{
				num4++;
				num3 = 5000;
				if (this.syncTransforms)
				{
					Physics.SyncTransforms();
				}
			}
		}
		if (num2 == 0)
		{
			Debug.LogWarning("Max attempts reached in PropSpawner, could not spawn all props!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", base.gameObject);
		}
		if (num3 == 0)
		{
			Debug.LogWarning("Max attempts IN A ROW reached in PropSpawner, could not spawn all props!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", base.gameObject);
		}
		this.currentSpawns = base.transform.childCount;
		this.SpawnDecor();
		foreach (PostSpawnBehavior postSpawnBehavior in this.postSpawnBehaviors)
		{
			if (!postSpawnBehavior.mute)
			{
				if (executeDeferredImmediately || postSpawnBehavior.DeferredTiming != DeferredStepTiming.AfterCurrentGroupTiming)
				{
					postSpawnBehavior.RunBehavior(this.SpawnedProps);
				}
				else
				{
					this._deferredSteps.Add(postSpawnBehavior.ConstructDeferred(this));
				}
			}
		}
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x00068500 File Offset: 0x00066700
	private void SpawnDecor()
	{
		DecorSpawner[] componentsInChildren = base.GetComponentsInChildren<DecorSpawner>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Execute();
		}
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x0006852C File Offset: 0x0006672C
	public override void Clear()
	{
		this.ValidationState = ValidationState.Unknown;
		this.AllSpawnData.Clear();
		this._madeDummyData = false;
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
		this._deferredSteps.Clear();
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x0006858B File Offset: 0x0006678B
	public void GoAll()
	{
		base.GetComponentInParent<PropGrouper>().RunAll(true);
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x00068599 File Offset: 0x00066799
	public void ClearAll()
	{
		base.GetComponentInParent<PropGrouper>().ClearAll();
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x000685A8 File Offset: 0x000667A8
	private static bool AllConstraintsPass(IEnumerable<PropSpawnerConstraint> currentConstraints, PropSpawner.SpawnData spawnData)
	{
		foreach (PropSpawnerConstraint propSpawnerConstraint in currentConstraints)
		{
			if (!propSpawnerConstraint.mute && !propSpawnerConstraint.CheckConstraint(spawnData))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x00068604 File Offset: 0x00066804
	private bool AllValidationConstraintsPass(IEnumerable<IValidationConstraint> currentConstraints, GameObject prop, PropSpawner.SpawnData spawnData)
	{
		foreach (IValidationConstraint validationConstraint in currentConstraints)
		{
			if (!validationConstraint.Muted && !validationConstraint.Validate(prop, spawnData))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x00068660 File Offset: 0x00066860
	private bool TryToSpawn(int currentSpawnCount)
	{
		PropSpawner.SpawnData randomPoint = this.GetRandomPoint();
		if (randomPoint == null)
		{
			return false;
		}
		if (!PropSpawner.AllConstraintsPass(this.constraints, randomPoint))
		{
			return false;
		}
		randomPoint.spawnCount = currentSpawnCount;
		GameObject gameObject = this.Spawn(randomPoint);
		if (gameObject != null)
		{
			this.AllSpawnData[gameObject] = randomPoint;
		}
		return gameObject != null;
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x000686B8 File Offset: 0x000668B8
	private GameObject Spawn(PropSpawner.SpawnData spawnData)
	{
		GameObject gameObject = HelperFunctions.SpawnPrefab(this.props[Random.Range(0, this.props.Length)], spawnData.pos, HelperFunctions.GetRandomRotationWithUp(Vector3.up), base.transform);
		for (int i = 0; i < this.modifiers.Count; i++)
		{
			if (!this.modifiers[i].mute)
			{
				this.modifiers[i].ModifyObject(gameObject, spawnData);
			}
		}
		for (int j = 0; j < this.postConstraints.Count; j++)
		{
			if (!this.postConstraints[j].mute && !this.postConstraints[j].CheckConstraint(gameObject, spawnData))
			{
				Object.DestroyImmediate(gameObject);
				return null;
			}
		}
		return gameObject;
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x0006877C File Offset: 0x0006697C
	private PropSpawner.SpawnData GetRandomPoint()
	{
		Vector3 vector = base.transform.position;
		Vector2 vector2 = new Vector2(Random.value, Random.value);
		vector += base.transform.right * Mathf.Lerp(-this.area.x * 0.5f, this.area.x * 0.5f, vector2.x);
		vector += base.transform.up * Mathf.Lerp(-this.area.y * 0.5f, this.area.y * 0.5f, vector2.y);
		if (!this.raycastPosition)
		{
			return new PropSpawner.SpawnData
			{
				pos = vector,
				normal = -base.transform.forward,
				rayDir = base.transform.forward,
				hit = default(RaycastHit),
				spawnerTransform = base.transform,
				placement = vector2
			};
		}
		Vector3 b = base.transform.forward * this.rayNearCutoff;
		RaycastHit hit = HelperFunctions.LineCheck(vector, vector + (base.transform.forward + b + this.rayDirectionOffset).normalized * (this.rayLength - this.rayNearCutoff), this.layerType, 0f, QueryTriggerInteraction.Ignore);
		if (hit.transform)
		{
			return new PropSpawner.SpawnData
			{
				pos = hit.point,
				normal = hit.normal,
				rayDir = base.transform.forward,
				hit = hit,
				spawnerTransform = base.transform,
				placement = vector2
			};
		}
		return null;
	}

	// Token: 0x040012AE RID: 4782
	private List<IDeferredStep> _deferredSteps = new List<IDeferredStep>();

	// Token: 0x040012AF RID: 4783
	public Vector2 area;

	// Token: 0x040012B0 RID: 4784
	public Vector3 rayDirectionOffset;

	// Token: 0x040012B1 RID: 4785
	public float rayLength = 5000f;

	// Token: 0x040012B2 RID: 4786
	public float rayNearCutoff;

	// Token: 0x040012B3 RID: 4787
	public bool raycastPosition = true;

	// Token: 0x040012B4 RID: 4788
	public int nrOfSpawns = 500;

	// Token: 0x040012B5 RID: 4789
	public bool randomSpawns;

	// Token: 0x040012B6 RID: 4790
	public int minSpawnCount;

	// Token: 0x040012B7 RID: 4791
	[Range(0f, 1f)]
	public float chanceToUseSpawner = 1f;

	// Token: 0x040012B8 RID: 4792
	public int currentSpawns;

	// Token: 0x040012B9 RID: 4793
	public GameObject[] props;

	// Token: 0x040012BA RID: 4794
	public bool syncTransforms = true;

	// Token: 0x040012BB RID: 4795
	public HelperFunctions.LayerType layerType = HelperFunctions.LayerType.TerrainMap;

	// Token: 0x040012BC RID: 4796
	[SerializeReference]
	public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();

	// Token: 0x040012BD RID: 4797
	[SerializeReference]
	public List<PropSpawnerConstraint> constraints = new List<PropSpawnerConstraint>();

	// Token: 0x040012BE RID: 4798
	[SerializeReference]
	public List<PropSpawnerConstraintPost> postConstraints = new List<PropSpawnerConstraintPost>();

	// Token: 0x040012BF RID: 4799
	[SerializeReference]
	public List<PostSpawnBehavior> postSpawnBehaviors = new List<PostSpawnBehavior>();

	// Token: 0x040012C0 RID: 4800
	public const string k_ValidationGroupColor = "#91DDF2";

	// Token: 0x040012C1 RID: 4801
	[SerializeReference]
	[Tooltip("Constraints that do NOT apply when spawning props, but may be useful for debugging in Editor.")]
	private List<IValidationConstraint> validationConstraints = new List<IValidationConstraint>();

	// Token: 0x040012C3 RID: 4803
	[SerializeField]
	[HideInInspector]
	private bool _madeDummyData;

	// Token: 0x040012C4 RID: 4804
	[SerializeReference]
	private PropSpawnData _propSpawnData = new PropSpawnData();

	// Token: 0x02000529 RID: 1321
	[Serializable]
	public class SpawnData
	{
		// Token: 0x04001C78 RID: 7288
		public Transform spawnerTransform;

		// Token: 0x04001C79 RID: 7289
		public Vector3 pos;

		// Token: 0x04001C7A RID: 7290
		public Vector3 normal;

		// Token: 0x04001C7B RID: 7291
		public Vector3 rayDir;

		// Token: 0x04001C7C RID: 7292
		public RaycastHit hit;

		// Token: 0x04001C7D RID: 7293
		public Vector2 placement;

		// Token: 0x04001C7E RID: 7294
		public int spawnCount;
	}
}
