using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200012E RID: 302
public class Snowball : ItemComponent
{
	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x060009C6 RID: 2502 RVA: 0x00034084 File Offset: 0x00032284
	private float ScaledAngularDrag
	{
		get
		{
			return this.defaultAngularDrag;
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060009C7 RID: 2503 RVA: 0x0003408C File Offset: 0x0003228C
	private bool CanGrow
	{
		get
		{
			return this.item.rig.linearVelocity.magnitude > this.minimumSpeedToGrow;
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060009C8 RID: 2504 RVA: 0x000340B9 File Offset: 0x000322B9
	private float TimeSpentBraking
	{
		get
		{
			return Time.time - this._timeBrakingStarted;
		}
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x000340C7 File Offset: 0x000322C7
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x000340CF File Offset: 0x000322CF
	private void Start()
	{
		this.Init();
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x000340D8 File Offset: 0x000322D8
	private void Init()
	{
		if (this._isInitialized)
		{
			return;
		}
		this._isInitialized = true;
		this._defaultPhysicsMaterial = this.physicalCollider.sharedMaterial;
		this.defaultAngularDrag = this.item.rig.angularDamping;
		this._leftHandPosition = this.LeftHand.transform.localPosition;
		this._rightHandPosition = this.RightHand.transform.localPosition;
		this._startTime = Time.time;
		this._timeLastTouchedScout = Time.time + 0.2f;
		this._timeLastCollided = Time.time + 0.2f;
		this._timeBrakingStarted = Time.time + 0.2f;
		this._previousPosition = null;
		this.scaleSyncer = base.GetComponent<ItemScaleSyncer>();
		this.scaleSyncer.InitScale();
		this.terrainLayer = LayerMask.NameToLayer("Terrain");
		this.characterLayer = LayerMask.NameToLayer("Character");
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x000341C9 File Offset: 0x000323C9
	public override void OnEnable()
	{
		base.OnEnable();
		Item item = this.item;
		item.OnStateChange = (Action<ItemState>)Delegate.Combine(item.OnStateChange, new Action<ItemState>(this.HandleStateChange));
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x000341F8 File Offset: 0x000323F8
	public override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x00034200 File Offset: 0x00032400
	private void Update()
	{
		if (Time.time - this._startTime < 0.2f)
		{
			return;
		}
		if (this.item.itemState != ItemState.Ground)
		{
			this._timeLastTouchedScout = Time.time;
			this._timeBrakingStarted = Time.time;
			this._previousPosition = null;
			return;
		}
		this.ProcessLastCollision();
		bool flag = this.scaleSyncer.currentScale < this.maxScaleToPickUp;
		this.item.blockInteraction = !flag;
		if (this.item.itemState != ItemState.Ground || Time.time - this._timeLastCollided > 1f)
		{
			this.item.rig.angularDamping = this.ScaledAngularDrag;
			this.physicalCollider.material.dynamicFriction = this._defaultPhysicsMaterial.dynamicFriction;
			return;
		}
		if (this.item.rig.isKinematic)
		{
			return;
		}
		if (this.item.rig.linearVelocity.magnitude >= this.minimumSpeedToGrow)
		{
			this._timeBrakingStarted = Time.time;
		}
		float num = Time.time - this._timeBrakingStarted;
		float t = Mathf.Clamp01(num / this.timeBeforeFullStop);
		this.physicalCollider.material.dynamicFriction = Mathf.Lerp(this._defaultPhysicsMaterial.dynamicFriction, 1000f, t);
		this.item.rig.angularDamping = Mathf.Lerp(this.ScaledAngularDrag, this.BrakingAngularDrag, t);
		if (num > this.timeBeforeFullStop && this.item.rig.linearVelocity.sqrMagnitude < 0.5f)
		{
			this.item.rig.linearVelocity = Vector3.zero;
			this.item.rig.angularVelocity = Vector3.zero;
			this.item.SetKinematic(this.scaleSyncer.currentScale > 2f);
		}
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x000343E4 File Offset: 0x000325E4
	private void FixedUpdate()
	{
		if (Time.time - this._startTime < 0.2f)
		{
			return;
		}
		if (this.flatCollider.enabled)
		{
			this.flatCollider.transform.parent.rotation = Quaternion.identity;
		}
		bool isKinematic = this.item.rig.isKinematic;
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x0003443D File Offset: 0x0003263D
	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer != this.characterLayer)
		{
			return;
		}
		this._timeLastTouchedScout = Time.time;
		this.item.SetKinematic(false);
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x0003446A File Offset: 0x0003266A
	private void OnCollisionStay(Collision other)
	{
		if (other.gameObject.layer != this.characterLayer)
		{
			this._unprocessedContact = other;
			return;
		}
		this._timeLastTouchedScout = Time.time;
		this.item.SetKinematic(false);
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x000344A0 File Offset: 0x000326A0
	private void HandleStateChange(ItemState state)
	{
		this.Init();
		if (state == ItemState.Held)
		{
			this.UpdateForBeingHeld();
		}
		else
		{
			this.UpdateMass();
		}
		Item item = this.item;
		item.OnStateChange = (Action<ItemState>)Delegate.Remove(item.OnStateChange, new Action<ItemState>(this.HandleStateChange));
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x000344EC File Offset: 0x000326EC
	private void UpdateForBeingHeld()
	{
		this.flatCollider.enabled = false;
		float currentScale = this.scaleSyncer.currentScale;
		float d = Mathf.Min(this.maxCarriedScale, currentScale);
		this.LeftHand.transform.localPosition = this._leftHandPosition * d;
		this.RightHand.transform.localPosition = this._rightHandPosition * d;
		this.physicalCollider.transform.parent.localScale = ((currentScale > this.maxCarriedScale) ? (this.maxCarriedScale / currentScale * Vector3.one) : Vector3.one);
		this.UpdateMass();
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00034594 File Offset: 0x00032794
	private void UpdateMass()
	{
		float num = this.scaleSyncer.currentScale * this.physicalCollider.radius;
		this.item.rig.mass = 4.1887903f * num * num * num * this.Density;
		this.item.rig.angularDamping = this.ScaledAngularDrag;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x000345F4 File Offset: 0x000327F4
	private void ProcessLastCollision()
	{
		if (this._unprocessedContact == null)
		{
			return;
		}
		this._lastCollision = this._unprocessedContact.collider;
		Snowball.ContactType lastContactType = this.ClassifyCurrentContact();
		switch (lastContactType)
		{
		case Snowball.ContactType.Unknown:
		case Snowball.ContactType.RegularGround:
		case Snowball.ContactType.Snowball:
			break;
		case Snowball.ContactType.SnowyGround:
			if (this.CanGrow)
			{
				this.Scale(1f);
			}
			this._previousPosition = new Vector3?(base.transform.position);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this._timeLastCollided = Time.time;
		this._lastContactType = lastContactType;
		this._unprocessedContact = null;
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x00034684 File Offset: 0x00032884
	private void Scale(float modifier = 1f)
	{
		if (this._previousPosition == null)
		{
			return;
		}
		Vector3 vector = base.transform.position - this._previousPosition.Value;
		vector = Vector3.ProjectOnPlane(vector, this._unprocessedContact.contacts[0].normal);
		float num = Mathf.Clamp(modifier * this.scaleRate * vector.magnitude, 0f, 0.1f);
		this.scaleSyncer.currentScale += num / this.scaleSyncer.currentScale;
		this.UpdateMass();
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x00034720 File Offset: 0x00032920
	private Snowball.ContactType ClassifyCurrentContact()
	{
		GameObject gameObject = this._unprocessedContact.gameObject;
		Renderer renderer;
		if ((gameObject.layer == this.terrainLayer || gameObject.CompareTag("Stone1")) && gameObject.TryGetComponent<Renderer>(out renderer))
		{
			string text = renderer.sharedMaterial.name.ToLowerInvariant();
			if (!text.Contains("ice") && !text.Contains("snow"))
			{
				return Snowball.ContactType.RegularGround;
			}
			return Snowball.ContactType.SnowyGround;
		}
		else
		{
			if (!gameObject.CompareTag("Snowball"))
			{
				return Snowball.ContactType.Unknown;
			}
			return Snowball.ContactType.Snowball;
		}
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x000347A0 File Offset: 0x000329A0
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x04000909 RID: 2313
	private const float k_StartGracePeriod = 0.2f;

	// Token: 0x0400090A RID: 2314
	private const float k_HardBrakeFriction = 1000f;

	// Token: 0x0400090B RID: 2315
	private const string k_SnowName = "snow";

	// Token: 0x0400090C RID: 2316
	private const string k_IceName = "ice";

	// Token: 0x0400090D RID: 2317
	private const string k_StoneTag = "Stone1";

	// Token: 0x0400090E RID: 2318
	private const string k_SnowballTag = "Snowball";

	// Token: 0x0400090F RID: 2319
	private bool _isInitialized;

	// Token: 0x04000910 RID: 2320
	private PhysicsMaterial _defaultPhysicsMaterial;

	// Token: 0x04000911 RID: 2321
	private float _originalScaleMagnitude;

	// Token: 0x04000912 RID: 2322
	private Vector3? _previousPosition;

	// Token: 0x04000913 RID: 2323
	private Vector3 _leftHandPosition;

	// Token: 0x04000914 RID: 2324
	private Vector3 _rightHandPosition;

	// Token: 0x04000915 RID: 2325
	private float _startTime;

	// Token: 0x04000916 RID: 2326
	private float _timeLastCollided;

	// Token: 0x04000917 RID: 2327
	private float _timeBrakingStarted;

	// Token: 0x04000918 RID: 2328
	private float _timeLastTouchedScout;

	// Token: 0x04000919 RID: 2329
	public float scaleRate;

	// Token: 0x0400091A RID: 2330
	private ItemScaleSyncer scaleSyncer;

	// Token: 0x0400091B RID: 2331
	[SerializeField]
	[Range(0f, 10f)]
	public float minimumSpeedToGrow = 0.1f;

	// Token: 0x0400091C RID: 2332
	private const float k_MinTimeOnGroundBeforeGrow = 1f;

	// Token: 0x0400091D RID: 2333
	private const string k_SettingsName = "Settings";

	// Token: 0x0400091E RID: 2334
	[FormerlySerializedAs("_density")]
	[SerializeField]
	[Range(0.01f, 10f)]
	private float Density = 1f;

	// Token: 0x0400091F RID: 2335
	[FormerlySerializedAs("timeBeforeHardBrake")]
	[SerializeField]
	[Range(0f, 10f)]
	private float timeBeforeFullStop = 2f;

	// Token: 0x04000920 RID: 2336
	[SerializeField]
	private float maxScaleToPickUp = 3f;

	// Token: 0x04000921 RID: 2337
	[SerializeField]
	private float maxCarriedScale = 3f;

	// Token: 0x04000922 RID: 2338
	private float defaultAngularDrag;

	// Token: 0x04000923 RID: 2339
	[FormerlySerializedAs("brakingDrag")]
	[SerializeField]
	private float BrakingAngularDrag = 100f;

	// Token: 0x04000924 RID: 2340
	private const string k_ReferencesName = "References";

	// Token: 0x04000925 RID: 2341
	[SerializeField]
	private Transform LeftHand;

	// Token: 0x04000926 RID: 2342
	[SerializeField]
	private Transform RightHand;

	// Token: 0x04000927 RID: 2343
	[SerializeField]
	private SphereCollider physicalCollider;

	// Token: 0x04000928 RID: 2344
	[SerializeField]
	private PhysicsMaterial HardBrakeMaterial;

	// Token: 0x04000929 RID: 2345
	[SerializeField]
	private Collider flatCollider;

	// Token: 0x0400092A RID: 2346
	[SerializeField]
	private GameObject snowballParticles;

	// Token: 0x0400092B RID: 2347
	private int terrainLayer;

	// Token: 0x0400092C RID: 2348
	private int characterLayer;

	// Token: 0x0400092D RID: 2349
	private Collision _unprocessedContact;

	// Token: 0x0400092E RID: 2350
	private Snowball.ContactType _lastContactType;

	// Token: 0x0400092F RID: 2351
	private Collider _lastCollision;

	// Token: 0x04000930 RID: 2352
	private bool _couldGrowLastFrame;

	// Token: 0x02000477 RID: 1143
	private enum ContactType
	{
		// Token: 0x04001990 RID: 6544
		Unknown,
		// Token: 0x04001991 RID: 6545
		RegularGround,
		// Token: 0x04001992 RID: 6546
		SnowyGround,
		// Token: 0x04001993 RID: 6547
		Snowball
	}
}
