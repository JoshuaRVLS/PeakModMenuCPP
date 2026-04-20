using System;
using System.Collections.Generic;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000289 RID: 649
public class JungleVine : CustomSpawnCondition, IInteractible
{
	// Token: 0x17000139 RID: 313
	// (get) Token: 0x060012AD RID: 4781 RVA: 0x0005DDDC File Offset: 0x0005BFDC
	public float Length
	{
		get
		{
			return this.totalLength;
		}
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x0005DDE4 File Offset: 0x0005BFE4
	private void Awake()
	{
		this.totalLength = 0f;
		this.photonView = base.GetComponent<PhotonView>();
		if (this.colliderRoot == null)
		{
			Debug.Log("No collider root, creating on awake.");
			this.colliderRoot = new GameObject("ColliderRoot").transform;
			this.colliderRoot.parent = base.transform;
			this.colliderRoot.localPosition = Vector3.zero;
			this.colliderRoot.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x0005DE66 File Offset: 0x0005C066
	private void Start()
	{
		this.SetRendererBounds();
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x0005DE70 File Offset: 0x0005C070
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position + base.transform.forward * this.maxDist, 0.5f);
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		Gizmos.DrawWireCube(componentInChildren.bounds.center, componentInChildren.bounds.size);
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x0005DEDF File Offset: 0x0005C0DF
	public bool IsInteractible(Character interactor)
	{
		return this.colliderType == JungleVine.ColliderType.Capsule;
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x0005DEEC File Offset: 0x0005C0EC
	public void Interact(Character interactor)
	{
		interactor.refs.items.EquipSlot(Optionable<byte>.None);
		int closestChild = this.GetClosestChild(interactor.Center);
		Debug.Log(string.Format("Grabbing Vine with index: {0}", closestChild));
		interactor.GetComponent<PhotonView>().RPC("GrabVineRpc", RpcTarget.All, new object[]
		{
			base.GetComponent<PhotonView>(),
			closestChild
		});
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x0005DF59 File Offset: 0x0005C159
	public void HoverEnter()
	{
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x0005DF5B File Offset: 0x0005C15B
	public void HoverExit()
	{
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x0005DF5D File Offset: 0x0005C15D
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x060012B6 RID: 4790 RVA: 0x0005DF6A File Offset: 0x0005C16A
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x060012B7 RID: 4791 RVA: 0x0005DF72 File Offset: 0x0005C172
	public string GetInteractionText()
	{
		return LocalizedText.GetText("GRAB", true);
	}

	// Token: 0x060012B8 RID: 4792 RVA: 0x0005DF7F File Offset: 0x0005C17F
	public string GetName()
	{
		return LocalizedText.GetText(this.displayName, true);
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x0005DF90 File Offset: 0x0005C190
	public void PickTreePlatforms(List<TreePlatform> treePlatformList)
	{
		List<TreePlatform> list = new List<TreePlatform>(treePlatformList);
		int num = 50;
		while (list.Count >= 2 && !this.startTreePlatform && num > 0)
		{
			int index = Random.Range(0, list.Count);
			this.startTreePlatform = list[index];
			num--;
		}
		if (num == 0)
		{
			Debug.LogError("Timed out picking a start tree platform for a vine");
		}
		if (this.startTreePlatform == null)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		List<TreePlatform> list2 = new List<TreePlatform>(treePlatformList);
		Debug.LogError("end list count is " + list2.Count.ToString());
		list2.Remove(this.startTreePlatform);
		for (int i = list2.Count - 1; i >= 0; i--)
		{
			float num2 = Vector3.Distance(list2[i].transform.position, this.startTreePlatform.transform.position);
			if (list2[i].connectedPlatforms.Contains(this.startTreePlatform))
			{
				list2.RemoveAt(i);
			}
			else if (num2 < this.minDist || num2 > this.maxDist)
			{
				list2.RemoveAt(i);
			}
			else if (Mathf.Abs(list2[i].transform.position.y - this.startTreePlatform.transform.position.y) > this.maxHeightDifference)
			{
				list2.RemoveAt(i);
			}
		}
		if (list2.Count == 0)
		{
			if (!this.ConfigVine(base.transform.position))
			{
				Object.DestroyImmediate(base.gameObject);
			}
			return;
		}
		int index2 = Random.Range(0, list2.Count);
		this.endTreePlatform = list2[index2];
		this.startTreePlatform.connectedPlatforms.Add(this.endTreePlatform);
		this.endTreePlatform.connectedPlatforms.Add(this.startTreePlatform);
		if (!this.ConfigVine(base.transform.position))
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x0005E194 File Offset: 0x0005C394
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		Vector3 a = data.normal;
		a.y *= this.normalYMult;
		a = a.normalized;
		RaycastHit raycastHit = HelperFunctions.LineCheck(base.transform.position + a * 0.02f, base.transform.position + a * this.maxDist, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (!raycastHit.transform)
		{
			return false;
		}
		if (raycastHit.distance < this.minDist)
		{
			return false;
		}
		if (Mathf.Abs(raycastHit.point.y - base.transform.position.y) > this.maxHeightDifference)
		{
			return false;
		}
		bool flag = this.ConfigVine(raycastHit.point);
		BreakableBridge breakableBridge;
		if (flag && base.TryGetComponent<BreakableBridge>(out breakableBridge))
		{
			breakableBridge.AddCollisionModifiers();
		}
		return flag;
	}

	// Token: 0x060012BB RID: 4795 RVA: 0x0005E274 File Offset: 0x0005C474
	public static bool CheckVinePath(Vector3 from, Vector3 to, float hang, out Vector3 mid, float radius = 0f)
	{
		mid = Vector3.Lerp(from, to, 0.5f);
		mid.y += hang;
		for (int i = 0; i < 50; i++)
		{
			float t = (float)i / 49f;
			Vector3 to2 = BezierCurve.QuadraticBezier(from, mid, to, t);
			if (i < 49 && HelperFunctions.LineCheck(from, to2, HelperFunctions.LayerType.TerrainMap, radius, QueryTriggerInteraction.Ignore).transform)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060012BC RID: 4796 RVA: 0x0005E2E8 File Offset: 0x0005C4E8
	[PunRPC]
	public void ForceBuildVine_RPC(Vector3 from, Vector3 to, float hang, Vector3 mid)
	{
		this.ForceBuildVine(from, to, hang, mid);
	}

	// Token: 0x060012BD RID: 4797 RVA: 0x0005E2F8 File Offset: 0x0005C4F8
	public void ForceBuildVine(Vector3 from, Vector3 to, float hang, Vector3 mid)
	{
		if (this.colliderRoot == null)
		{
			if (Application.isPlaying)
			{
				Debug.LogWarning("colliderRoot was null, creating new one for " + base.gameObject.name);
			}
			this.colliderRoot = new GameObject("ColliderRoot").transform;
			this.colliderRoot.parent = base.transform;
			this.colliderRoot.localPosition = Vector3.zero;
			this.colliderRoot.localRotation = Quaternion.identity;
		}
		this.colliderRoot.KillAllChildren(true, false, false);
		float num = Vector3.Distance(from, to) / this.meshLength;
		Renderer componentInChildren = base.GetComponentInChildren<Renderer>();
		componentInChildren.material.SetFloat("_Hang", hang);
		if (this.hangCenter != null)
		{
			this.hangCenter.position = BezierCurve.QuadraticBezier(from, mid, to, 0.5f);
		}
		Vector3 vector = from;
		for (int i = 0; i < 50; i++)
		{
			float t = (float)i / 49f;
			Vector3 vector2 = BezierCurve.QuadraticBezier(from, mid, to, t);
			GameObject gameObject = new GameObject("Collider");
			if (this.connectsPlatforms)
			{
				gameObject.tag = "Wood";
			}
			gameObject.transform.parent = this.colliderRoot;
			if (this.colliderType == JungleVine.ColliderType.Capsule)
			{
				CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
				capsuleCollider.radius = 0.25f;
				capsuleCollider.height = Vector3.Distance(vector, vector2) + 0.5f;
				capsuleCollider.isTrigger = true;
			}
			else
			{
				gameObject.AddComponent<BoxCollider>().size = new Vector3(this.boxShape.x * num, Vector3.Distance(vector, vector2) + 0.5f, this.boxShape.y);
			}
			gameObject.transform.rotation = HelperFunctions.GetRotationWithUp(Vector3.down, vector2 - vector);
			gameObject.transform.position = vector2 - Vector3.down * this.colliderOffset;
			gameObject.gameObject.layer = 21;
			vector = vector2;
		}
		if (this.connectsPlatforms)
		{
			Object.DestroyImmediate(this.colliderRoot.GetChild(0).gameObject);
			Object.DestroyImmediate(this.colliderRoot.GetChild(this.colliderRoot.childCount - 1).gameObject);
		}
		Transform transform = base.transform.Find("Mesh");
		transform.transform.rotation = Quaternion.LookRotation(to - from);
		transform.transform.localScale = Vector3.one * num;
		if (num < 0.5f)
		{
			componentInChildren.material.SetFloat("_LengthScale", num * 2f);
		}
		this.SetRendererBounds();
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x0005E594 File Offset: 0x0005C794
	public bool ConfigVine(Vector3 to)
	{
		base.GetComponentsInChildren<Collider>().KillAllGameObjects(true);
		float hang = Random.Range(-this.maxDown, -this.minDown);
		float radius = 0f;
		if (this.startTreePlatform && this.endTreePlatform)
		{
			Vector3 b = this.endTreePlatform.transform.position - this.startTreePlatform.transform.position;
			b.y = 0f;
			b = b.normalized * 8f;
			base.transform.position = this.startTreePlatform.transform.position + b;
			to = this.endTreePlatform.transform.position - b;
		}
		else if (this.startTreePlatform)
		{
			Vector3 normalized = Random.onUnitSphere.normalized;
			normalized.y = Random.Range(-0.5f, 0.5f);
			RaycastHit raycastHit = HelperFunctions.LineCheck(this.startTreePlatform.transform.position + normalized * 8f, this.startTreePlatform.transform.position + normalized * this.maxDist, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
			if (!raycastHit.transform || (raycastHit.transform.gameObject.GetComponent<MeshRenderer>() && !(raycastHit.transform.gameObject.GetComponent<MeshRenderer>().sharedMaterial != this.bannedMaterial[0])) || raycastHit.transform.gameObject.CompareTag("Redwood") || Vector3.Distance(raycastHit.point, this.startTreePlatform.transform.position) <= this.minDist)
			{
				return false;
			}
			Vector3 point = raycastHit.point;
			Vector3 b2 = point - this.startTreePlatform.transform.position;
			b2.y = 0f;
			b2 = b2.normalized * 8f;
			base.transform.position = this.startTreePlatform.transform.position + b2;
			to = point;
		}
		Vector3 position = base.transform.position;
		if (this.connectsPlatforms && Vector3.Angle(to - position, Vector3.up) < 45f)
		{
			return false;
		}
		Vector3 mid;
		if (!JungleVine.CheckVinePath(position, to, hang, out mid, radius))
		{
			return false;
		}
		this.ForceBuildVine(position, to, hang, mid);
		return true;
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x0005E831 File Offset: 0x0005CA31
	public void ConnectDebug()
	{
		this.ConfigVine(this.connectTo.transform.position);
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x0005E84C File Offset: 0x0005CA4C
	private void SetRendererBounds()
	{
		Vector3 a = base.transform.position;
		Vector3 vector = base.transform.position;
		if (this.colliderRoot != null)
		{
			this.totalLength = 0f;
			for (int i = 0; i < Mathf.Min(this.colliderRoot.transform.childCount, 50); i++)
			{
				Vector3 position = this.colliderRoot.transform.GetChild(i).transform.position;
				this.totalLength += Vector3.Distance(a, position);
				a = position;
				if (position.y < vector.y)
				{
					vector = position;
				}
			}
		}
		Renderer componentInChildren = base.GetComponentInChildren<Renderer>();
		Bounds localBounds = componentInChildren.localBounds;
		localBounds.Encapsulate(componentInChildren.transform.InverseTransformPoint(vector));
		componentInChildren.localBounds = localBounds;
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x0005E920 File Offset: 0x0005CB20
	public float LengthFactor()
	{
		if (Mathf.Approximately(0f, this.totalLength))
		{
			if (!this._hasInitializedWrong)
			{
				this._hasInitializedWrong = true;
				Debug.LogError(string.Format("We didn't add up the length for our {0} colliders! ", this.colliderRoot.childCount) + "Returning 0 to avoid NaNs", this);
			}
			return 0f;
		}
		return 1f / this.totalLength;
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x0005E98A File Offset: 0x0005CB8A
	public float GetPercentFromSegmentIndex(int segmentIndex)
	{
		return (float)segmentIndex / 49f;
	}

	// Token: 0x060012C3 RID: 4803 RVA: 0x0005E994 File Offset: 0x0005CB94
	public int GetIndexFromPercentage(float percent)
	{
		return Mathf.RoundToInt(Mathf.Lerp(0f, 49f, percent));
	}

	// Token: 0x060012C4 RID: 4804 RVA: 0x0005E9AB File Offset: 0x0005CBAB
	internal Vector3 GetDir(Vector3 facingDirection, Vector3 up)
	{
		if (Vector3.Angle(facingDirection, up) > 90f)
		{
			up *= -1f;
		}
		return up;
	}

	// Token: 0x060012C5 RID: 4805 RVA: 0x0005E9CC File Offset: 0x0005CBCC
	public float GetVineVel(Vector3 vel, float percent)
	{
		int indexFromPercentage = this.GetIndexFromPercentage(percent);
		Vector3 up = this.colliderRoot.transform.GetChild(indexFromPercentage).up;
		Vector3 dir = this.GetDir(vel, up);
		float num = ((Vector3.Angle(vel, up) > 90f) ? -1f : 1f) * Vector3.Project(vel, up).magnitude * Mathf.InverseLerp(0f, -0.5f, dir.y);
		Debug.Log(string.Format("Air speed of {0} converted to vine speed of {1}.", vel.magnitude, num), this);
		return num;
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x0005EA68 File Offset: 0x0005CC68
	public float GetSign(Vector3 dir, float percent)
	{
		Vector3 up = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
		float result = 1f;
		if (Vector3.Angle(dir, up) > 90f)
		{
			result = -1f;
		}
		return result;
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x0005EAB0 File Offset: 0x0005CCB0
	public Vector3 GetUp(float percent)
	{
		Vector3 vector = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
		if (Vector3.Angle(Vector3.up, vector) > 90f)
		{
			vector *= -1f;
		}
		return vector;
	}

	// Token: 0x060012C8 RID: 4808 RVA: 0x0005EAFC File Offset: 0x0005CCFC
	public float UpMult(float percent)
	{
		Vector3 up = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
		return (float)((Vector3.Angle(Vector3.up, up) < 90f) ? 1 : -1);
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x0005EB40 File Offset: 0x0005CD40
	public Vector3 GetPosition(float percent, int clampFromBottom = 0, int clampFromTop = 0)
	{
		int childCount = this.colliderRoot.transform.childCount;
		if (childCount != 50 && !this._hasInitializedWrong)
		{
			this._hasInitializedWrong = true;
			Debug.LogError(string.Format("Expected {0} to have {1} colliders, ", this.colliderRoot.name, 50) + string.Format("but it has only {0}! We'll need to clamp", this.colliderRoot.transform.childCount), this);
		}
		float num = Mathf.Clamp01(percent) * (float)(childCount - 1);
		int value = Mathf.FloorToInt(num);
		int max = childCount - clampFromTop - 2;
		int num2 = Math.Clamp(value, clampFromBottom, max);
		int index = num2 + 1;
		float t = Mathf.Clamp01(num - (float)num2);
		Vector3 position = this.colliderRoot.transform.GetChild(num2).position;
		Vector3 position2 = this.colliderRoot.transform.GetChild(index).position;
		return Vector3.Lerp(position, position2, t);
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x0005EC1C File Offset: 0x0005CE1C
	private int GetClosestChild(Vector3 center)
	{
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < this.colliderRoot.transform.childCount; i++)
		{
			float num3 = Vector3.Distance(center, this.colliderRoot.transform.GetChild(i).position);
			if (num3 < num)
			{
				num = num3;
				num2 = i;
			}
		}
		if (num2 == -1 && !this._hasInitializedWrong)
		{
			Debug.LogError(string.Format("Uh oh! We failed to find a valid collider child on {0}", this), this);
		}
		return num2;
	}

	// Token: 0x040010C8 RID: 4296
	private bool _hasInitializedWrong;

	// Token: 0x040010C9 RID: 4297
	public float minDist = 25f;

	// Token: 0x040010CA RID: 4298
	public float maxDist = 50f;

	// Token: 0x040010CB RID: 4299
	public float maxHeightDifference = 100f;

	// Token: 0x040010CC RID: 4300
	public float normalYMult = 1f;

	// Token: 0x040010CD RID: 4301
	public float minDown = 5f;

	// Token: 0x040010CE RID: 4302
	public float maxDown = 30f;

	// Token: 0x040010CF RID: 4303
	public float meshLength = 40f;

	// Token: 0x040010D0 RID: 4304
	public int vineType;

	// Token: 0x040010D1 RID: 4305
	public float colliderOffset;

	// Token: 0x040010D2 RID: 4306
	public JungleVine.ColliderType colliderType;

	// Token: 0x040010D3 RID: 4307
	public Transform colliderTransform;

	// Token: 0x040010D4 RID: 4308
	public Vector2 boxShape = Vector2.one;

	// Token: 0x040010D5 RID: 4309
	public PhotonView photonView;

	// Token: 0x040010D6 RID: 4310
	public Transform connectTo;

	// Token: 0x040010D7 RID: 4311
	private const int segments = 50;

	// Token: 0x040010D8 RID: 4312
	private float totalLength;

	// Token: 0x040010D9 RID: 4313
	public Transform hangCenter;

	// Token: 0x040010DA RID: 4314
	public Material[] bannedMaterial;

	// Token: 0x040010DB RID: 4315
	public string displayName;

	// Token: 0x040010DC RID: 4316
	public bool connectsPlatforms;

	// Token: 0x040010DD RID: 4317
	public TreePlatform startTreePlatform;

	// Token: 0x040010DE RID: 4318
	public TreePlatform endTreePlatform;

	// Token: 0x040010DF RID: 4319
	public Transform colliderRoot;

	// Token: 0x02000508 RID: 1288
	public enum ColliderType
	{
		// Token: 0x04001C03 RID: 7171
		Capsule,
		// Token: 0x04001C04 RID: 7172
		Box
	}
}
