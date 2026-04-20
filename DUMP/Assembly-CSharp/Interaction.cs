using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000022 RID: 34
[DefaultExecutionOrder(600)]
public class Interaction : MonoBehaviour
{
	// Token: 0x17000031 RID: 49
	// (get) Token: 0x06000270 RID: 624 RVA: 0x000120FE File Offset: 0x000102FE
	// (set) Token: 0x06000271 RID: 625 RVA: 0x00012106 File Offset: 0x00010306
	public float currentInteractableHeldTime
	{
		get
		{
			return this._cihf;
		}
		set
		{
			this._cihf = value;
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x06000272 RID: 626 RVA: 0x0001210F File Offset: 0x0001030F
	public float constantInteractableProgress
	{
		get
		{
			return this.currentInteractableHeldTime / this.currentConstantInteractableTime;
		}
	}

	// Token: 0x06000273 RID: 627 RVA: 0x0001211E File Offset: 0x0001031E
	private void Awake()
	{
		Interaction.instance = this;
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000274 RID: 628 RVA: 0x00012128 File Offset: 0x00010328
	private bool canInteract
	{
		get
		{
			return !Character.localCharacter.data.passedOut && !Character.localCharacter.data.fullyPassedOut && Character.localCharacter.CanDoInput() && !Character.localCharacter.data.currentStickyItem;
		}
	}

	// Token: 0x06000275 RID: 629 RVA: 0x0001217C File Offset: 0x0001037C
	private void LateUpdate()
	{
		this.currentHovered = null;
		if (!Character.localCharacter)
		{
			return;
		}
		if (!this.canInteract)
		{
			this.bestInteractable = null;
			this.bestCharacter = null;
		}
		else
		{
			this.DoInteractableRaycasts(out this.bestInteractable);
			this.bestCharacter = (this.bestInteractable as CharacterInteractible);
			this.DoInteraction(this.bestInteractable);
		}
		this.bestInteractableName = ((this.bestInteractable == null) ? "null" : this.bestInteractable.GetTransform().gameObject.name);
		this.currentHovered = this.bestInteractable;
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000276 RID: 630 RVA: 0x00012214 File Offset: 0x00010414
	public bool hasValidTargetCharacter
	{
		get
		{
			return this.bestCharacter != null;
		}
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00012224 File Offset: 0x00010424
	private void DoInteraction(IInteractible interactable)
	{
		if (Character.localCharacter.input.interactWasReleased && interactable != null && this.currentHeldInteractible == interactable && this.readyToReleaseInteract)
		{
			IInteractibleConstant interactibleConstant = interactable as IInteractibleConstant;
			if (interactibleConstant != null)
			{
				interactibleConstant.ReleaseInteract(Character.localCharacter);
			}
			this.readyToReleaseInteract = false;
		}
		if (!Character.localCharacter.input.interactIsPressed)
		{
			this.readyToInteract = true;
			this.CancelHeldInteract();
		}
		else
		{
			if (this.readyToInteract && interactable != null)
			{
				this.readyToReleaseInteract = true;
				IInteractibleConstant interactibleConstant2 = interactable as IInteractibleConstant;
				if (interactibleConstant2 != null && interactibleConstant2.IsConstantlyInteractable(Character.localCharacter))
				{
					this.currentHeldInteractible = interactibleConstant2;
					this.currentConstantInteractableTime = interactibleConstant2.GetInteractTime(Character.localCharacter);
				}
				interactable.Interact(Character.localCharacter);
				this.readyToInteract = false;
				return;
			}
			if (Character.localCharacter.input.interactIsPressed && this.currentHeldInteractible != null)
			{
				if (interactable != this.currentHeldInteractible)
				{
					this.currentHeldInteractible = null;
				}
				else
				{
					this.currentInteractableHeldTime += Time.deltaTime;
					if (this.currentInteractableHeldTime >= this.currentConstantInteractableTime)
					{
						this.currentHeldInteractible.Interact_CastFinished(Character.localCharacter);
						this.readyToReleaseInteract = false;
						if (!this.currentHeldInteractible.holdOnFinish)
						{
							this.CancelHeldInteract();
						}
					}
				}
			}
		}
		if (this.currentHeldInteractible == null)
		{
			this.CancelHeldInteract();
		}
	}

	// Token: 0x06000278 RID: 632 RVA: 0x0001236C File Offset: 0x0001056C
	private void DoInteractableRaycasts(out IInteractible interactableResult)
	{
		if (Character.localCharacter.data.carriedPlayer != null && Character.localCharacter.refs.items.currentSelectedSlot.IsSome && Character.localCharacter.refs.items.currentSelectedSlot.Value == 3)
		{
			interactableResult = Character.localCharacter.data.carriedPlayer.refs.interactible;
			return;
		}
		float num = Vector3.Angle(Vector3.down, MainCamera.instance.transform.forward);
		if (num <= 10f)
		{
			using (List<StickyItemComponent>.Enumerator enumerator = StickyItemComponent.ALL_STUCK_ITEMS.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					StickyItemComponent stickyItemComponent = enumerator.Current;
					if (stickyItemComponent.stuckToCharacter == Character.localCharacter && stickyItemComponent.item.Center().y <= Character.localCharacter.Center.y)
					{
						interactableResult = stickyItemComponent.item;
						return;
					}
				}
				goto IL_17D;
			}
		}
		if (num >= 170f)
		{
			foreach (StickyItemComponent stickyItemComponent2 in StickyItemComponent.ALL_STUCK_ITEMS)
			{
				if (stickyItemComponent2.stuckToCharacter == Character.localCharacter && stickyItemComponent2.item.Center().y >= Character.localCharacter.Center.y)
				{
					interactableResult = stickyItemComponent2.item;
					return;
				}
			}
		}
		IL_17D:
		Ray ray = new Ray(MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
		int num2 = HelperFunctions.LineCheckAll(ray.origin, ray.origin + ray.direction * this.distance, HelperFunctions.LayerType.AllPhysical, this.rayHitCache, 0f, QueryTriggerInteraction.Collide);
		IInteractible interactible = null;
		RaycastHit raycastHit = default(RaycastHit);
		raycastHit.distance = float.MaxValue;
		for (int i = 0; i < num2; i++)
		{
			RaycastHit raycastHit2 = this.rayHitCache[i];
			Item item;
			if (raycastHit2.distance < this.distance && !Character.localCharacter.refs.ragdoll.colliderList.Contains(raycastHit2.collider) && raycastHit2.transform && (!Item.TryGetItemFromCollider(raycastHit2.collider, out item) || !item || !(item == Character.localCharacter.data.currentItem)))
			{
				raycastHit = raycastHit2;
			}
		}
		if (raycastHit.collider != null)
		{
			IInteractible componentInParent = raycastHit.collider.GetComponentInParent<IInteractible>();
			if (componentInParent != null && componentInParent.IsInteractible(Character.localCharacter))
			{
				interactible = componentInParent;
			}
		}
		if (interactible == null)
		{
			float num3 = float.MaxValue;
			int num4 = Physics.SphereCastNonAlloc(MainCamera.instance.transform.position + MainCamera.instance.transform.forward * (this.area / 2f), this.area, MainCamera.instance.transform.forward, this.sphereCastResults, Mathf.Min(raycastHit.distance, this.distance), HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysical), QueryTriggerInteraction.Collide);
			for (int j = 0; j < num4; j++)
			{
				RaycastHit raycastHit3 = this.sphereCastResults[j];
				Item item2;
				if (!Item.TryGetItemFromCollider(raycastHit3.collider, out item2) || !item2 || !(item2 == Character.localCharacter.data.currentItem))
				{
					float num5 = Vector3.Angle(raycastHit3.point - MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
					if (num5 < num3)
					{
						IInteractible componentInParent2 = raycastHit3.collider.GetComponentInParent<IInteractible>();
						Item item3;
						if (componentInParent2 != null && componentInParent2.IsInteractible(Character.localCharacter) && (!Item.TryGetItemFromCollider(raycastHit3.collider, out item3) || !item3 || !(item3 == Character.localCharacter.data.currentItem)))
						{
							RaycastHit raycastHit4 = HelperFunctions.LineCheck(ray.origin, raycastHit3.point, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Collide);
							if (raycastHit4.collider != null && raycastHit4.collider.GetComponentInParent<IInteractible>() != componentInParent2)
							{
								Debug.DrawLine(ray.origin, raycastHit3.point, Color.red);
							}
							else
							{
								Debug.DrawLine(ray.origin, raycastHit3.point, Color.green);
								num3 = num5;
								interactible = componentInParent2;
							}
						}
					}
				}
			}
		}
		interactableResult = interactible;
	}

	// Token: 0x06000279 RID: 633 RVA: 0x00012838 File Offset: 0x00010A38
	private void CancelHeldInteract()
	{
		if (this.currentHeldInteractible != null)
		{
			this.currentHeldInteractible.CancelCast(Character.localCharacter);
		}
		this.currentInteractableHeldTime = 0f;
		this.currentHeldInteractible = null;
	}

	// Token: 0x04000241 RID: 577
	public float distance = 2f;

	// Token: 0x04000242 RID: 578
	public float area = 0.5f;

	// Token: 0x04000243 RID: 579
	public float maxCharacterInteractAngle = 90f;

	// Token: 0x04000244 RID: 580
	public static Interaction instance;

	// Token: 0x04000245 RID: 581
	public IInteractible currentHovered;

	// Token: 0x04000246 RID: 582
	public IInteractibleConstant currentHeldInteractible;

	// Token: 0x04000247 RID: 583
	public float currentConstantInteractableTime;

	// Token: 0x04000248 RID: 584
	private float _cihf;

	// Token: 0x04000249 RID: 585
	public RaycastHit[] sphereCastResults = new RaycastHit[100];

	// Token: 0x0400024A RID: 586
	internal IInteractible bestInteractable;

	// Token: 0x0400024B RID: 587
	[SerializeField]
	internal CharacterInteractible bestCharacter;

	// Token: 0x0400024C RID: 588
	[HideInInspector]
	public bool readyToInteract = true;

	// Token: 0x0400024D RID: 589
	[HideInInspector]
	public bool readyToReleaseInteract = true;

	// Token: 0x0400024E RID: 590
	[SerializeField]
	private string bestInteractableName;

	// Token: 0x0400024F RID: 591
	private RaycastHit[] rayHitCache = new RaycastHit[64];
}
