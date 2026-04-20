using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using Unity.Collections;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class HelperFunctions : MonoBehaviour
{
	// Token: 0x060007CA RID: 1994 RVA: 0x0002BCFC File Offset: 0x00029EFC
	internal static Terrain GetTerrain(Vector3 center)
	{
		RaycastHit raycastHit = HelperFunctions.LineCheck(center + Vector3.up * 1000f, center - Vector3.up * 1000f, HelperFunctions.LayerType.Terrain, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			return raycastHit.transform.GetComponent<Terrain>();
		}
		return null;
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0002BD5C File Offset: 0x00029F5C
	public static LayerMask GetMask(HelperFunctions.LayerType layerType)
	{
		if (layerType == HelperFunctions.LayerType.AllPhysical)
		{
			return HelperFunctions.AllPhysical;
		}
		if (layerType == HelperFunctions.LayerType.TerrainMap)
		{
			return HelperFunctions.terrainMapMask;
		}
		if (layerType == HelperFunctions.LayerType.Terrain)
		{
			return HelperFunctions.terrainMask;
		}
		if (layerType == HelperFunctions.LayerType.Default)
		{
			return HelperFunctions.DefaultMask;
		}
		if (layerType == HelperFunctions.LayerType.AllPhysicalExceptCharacter)
		{
			return HelperFunctions.AllPhysicalExceptCharacter;
		}
		if (layerType == HelperFunctions.LayerType.Map)
		{
			return HelperFunctions.MapMask;
		}
		if (layerType == HelperFunctions.LayerType.CharacterAndDefault)
		{
			return HelperFunctions.CharacterAndDefaultMask;
		}
		if (layerType == HelperFunctions.LayerType.AllPhysicalExceptDefault)
		{
			return HelperFunctions.AllPhysicalExceptDefault;
		}
		return HelperFunctions.MapMask;
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0002BDC0 File Offset: 0x00029FC0
	public static Vector3 GetGroundPos(Vector3 from, HelperFunctions.LayerType layerType, float radius = 0f)
	{
		Vector3 result = from;
		RaycastHit raycastHit = HelperFunctions.LineCheck(from, from + Vector3.down * 10000f, layerType, radius, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			result = raycastHit.point;
		}
		return result;
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0002BE05 File Offset: 0x0002A005
	public static RaycastHit GetGroundPosRaycast(Vector3 from, HelperFunctions.LayerType layerType, float radius = 0f)
	{
		return HelperFunctions.LineCheck(from, from + Vector3.down * 10000f, layerType, radius, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0002BE25 File Offset: 0x0002A025
	internal static GameObject InstantiatePrefab(GameObject sourceObj, Vector3 pos, Quaternion rot, Transform parent)
	{
		GameObject gameObject = HelperFunctions.InstantiatePrefab(sourceObj, parent);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rot;
		return gameObject;
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x0002BE48 File Offset: 0x0002A048
	internal static GameObject InstantiatePrefab(GameObject sourceObj, Transform parent)
	{
		GameObject result = null;
		if (!Application.isEditor)
		{
			result = Object.Instantiate<GameObject>(sourceObj, parent);
		}
		return result;
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0002BE67 File Offset: 0x0002A067
	public static RaycastHit GetGroundPosRaycast(Vector3 from, HelperFunctions.LayerType layerType, Vector3 gravityDir, float radius = 0f)
	{
		return HelperFunctions.LineCheck(from, from + gravityDir * 10000f, layerType, radius, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0002BE84 File Offset: 0x0002A084
	public static RaycastHit LineCheck(Vector3 from, Vector3 to, HelperFunctions.LayerType layerType, float radius = 0f, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		RaycastHit result = default(RaycastHit);
		Ray ray = new Ray(from, to - from);
		if (radius == 0f)
		{
			Physics.Raycast(ray, out result, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType));
		}
		else
		{
			Physics.SphereCast(ray, radius, out result, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType));
		}
		return result;
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0002BEEC File Offset: 0x0002A0EC
	public static int LineCheckAll(Vector3 from, Vector3 to, HelperFunctions.LayerType layerType, RaycastHit[] hitCache, float radius = 0f, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		if (radius == 0f)
		{
			return Physics.RaycastNonAlloc(from, to - from, hitCache, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType), triggerInteraction);
		}
		return Physics.SphereCastNonAlloc(from, radius, to - from, hitCache, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType), triggerInteraction);
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0002BF4C File Offset: 0x0002A14C
	public static RaycastHit LineCheckIgnoreItem(Vector3 from, Vector3 to, HelperFunctions.LayerType layerType, RaycastHit[] hitCache, Item ignoreItem)
	{
		RaycastHit result = default(RaycastHit);
		Physics.RaycastNonAlloc(from, to - from, hitCache, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType));
		foreach (RaycastHit raycastHit in hitCache)
		{
			Item item;
			if (raycastHit.collider && (!Item.TryGetItemFromCollider(raycastHit.collider, out item) || !item || !(item == ignoreItem)) && (result.collider == null || result.distance > raycastHit.distance))
			{
				result = raycastHit;
			}
		}
		return result;
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0002BFF0 File Offset: 0x0002A1F0
	internal static ConfigurableJoint AttachPositionJoint(Rigidbody rig1, Rigidbody rig2, bool useCustomConnection = false, Vector3 customConnectionPoint = default(Vector3))
	{
		ConfigurableJoint configurableJoint = rig1.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Locked;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		configurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;
		configurableJoint.anchor = ((!useCustomConnection) ? rig1.transform.InverseTransformPoint(rig2.position) : rig1.transform.InverseTransformPoint(customConnectionPoint));
		configurableJoint.enableCollision = false;
		configurableJoint.connectedBody = rig2;
		return configurableJoint;
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x0002C05A File Offset: 0x0002A25A
	internal static Joint AttachFixedJoint(Rigidbody rig1, Rigidbody rig2)
	{
		FixedJoint fixedJoint = rig1.gameObject.AddComponent<FixedJoint>();
		fixedJoint.enableCollision = false;
		fixedJoint.connectedBody = rig2;
		return fixedJoint;
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x0002C078 File Offset: 0x0002A278
	internal static Vector3 RandomOnFlatCircle()
	{
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		return new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x0002C0A4 File Offset: 0x0002A2A4
	internal static void DestroyAll(Object[] objects)
	{
		for (int i = objects.Length - 1; i >= 0; i--)
		{
			Object.Destroy(objects[i]);
		}
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0002C0C9 File Offset: 0x0002A2C9
	internal static Vector3 EulerToLook(Vector2 euler)
	{
		return new Vector3(euler.y, -euler.x, 0f);
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x0002C0E2 File Offset: 0x0002A2E2
	internal static Vector3 LookToEuler(Vector2 lookRotationValues)
	{
		return new Vector3(-lookRotationValues.y, lookRotationValues.x, 0f);
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x0002C0FB File Offset: 0x0002A2FB
	internal static Vector3 LookToDirection(Vector3 look, Vector3 targetDir)
	{
		return HelperFunctions.EulerToDirection(HelperFunctions.LookToEuler(look), targetDir);
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x0002C10E File Offset: 0x0002A30E
	internal static Vector3 EulerToDirection(Vector3 euler, Vector3 targetDir)
	{
		return Quaternion.Euler(euler) * targetDir;
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0002C11C File Offset: 0x0002A31C
	internal static Vector3 DirectionToEuler(Vector3 dir)
	{
		return Quaternion.LookRotation(dir, Vector3.up).eulerAngles;
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x0002C13C File Offset: 0x0002A33C
	internal static Vector3 DirectionToLook(Vector3 dir)
	{
		Vector3 vector = HelperFunctions.DirectionToEuler(dir);
		while (vector.x > 180f)
		{
			vector.x -= 360f;
		}
		return HelperFunctions.EulerToLook(vector);
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0002C17A File Offset: 0x0002A37A
	internal static Vector3 GroundDirection(Vector3 planeNormal, Vector3 sideDirection)
	{
		return -Vector3.Cross(sideDirection, planeNormal);
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0002C188 File Offset: 0x0002A388
	internal static Vector3 SeparateClamps(Vector3 rotationError, float clamp)
	{
		rotationError.x = Mathf.Clamp(rotationError.x, -clamp, clamp);
		rotationError.y = Mathf.Clamp(rotationError.y, -clamp, clamp);
		rotationError.z = Mathf.Clamp(rotationError.z, -clamp, clamp);
		return rotationError;
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0002C1D5 File Offset: 0x0002A3D5
	internal static float FlatDistance(Vector3 from, Vector3 to)
	{
		return Vector2.Distance(from.XZ(), to.XZ());
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0002C1E8 File Offset: 0x0002A3E8
	internal static void IgnoreConnect(Rigidbody rig1, Rigidbody rig2)
	{
		rig1.gameObject.AddComponent<ConfigurableJoint>().connectedBody = rig2;
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0002C1FB File Offset: 0x0002A3FB
	internal static RaycastHit[] SortRaycastResults(RaycastHit[] hitsToSort)
	{
		hitsToSort.Sort(new Comparison<RaycastHit>(HelperFunctions.RaycastHitComparer));
		return hitsToSort;
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0002C210 File Offset: 0x0002A410
	public static Vector3[] GetCircularDirections(int count)
	{
		Vector3[] array = new Vector3[count];
		float num = 360f / (float)count;
		for (int i = 0; i < count; i++)
		{
			float num2 = (float)i * num;
			float f = 0.017453292f * num2;
			array[i] = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)).normalized;
		}
		return array;
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0002C270 File Offset: 0x0002A470
	private static int RaycastHitComparer(RaycastHit x, RaycastHit y)
	{
		if (x.distance < y.distance)
		{
			return -1;
		}
		return 1;
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0002C288 File Offset: 0x0002A488
	internal static Quaternion GetRandomRotationWithUp(Vector3 normal)
	{
		Vector3 vector = Random.onUnitSphere;
		vector.y = 0f;
		vector = Vector3.Cross(normal, Vector3.Cross(normal, vector));
		return Quaternion.LookRotation(vector, normal);
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0002C2BC File Offset: 0x0002A4BC
	public static Bounds GetTotalBounds(GameObject gameObject)
	{
		return HelperFunctions.GetTotalBounds(gameObject.GetComponentsInChildren<MeshRenderer>());
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0002C2CC File Offset: 0x0002A4CC
	internal static Vector3 GetCenterOfMass(Transform transform)
	{
		Vector3 vector = Vector3.zero;
		float num = 0f;
		for (int i = 0; i < transform.childCount; i++)
		{
			Collider component = transform.GetChild(i).GetComponent<Collider>();
			if (component)
			{
				vector += component.transform.position;
				num += 1f;
			}
		}
		vector /= num;
		return transform.InverseTransformPoint(vector);
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0002C334 File Offset: 0x0002A534
	public static Bounds GetTotalBounds(IEnumerable<Renderer> rends)
	{
		Bounds result = default(Bounds);
		bool flag = true;
		foreach (Renderer renderer in rends)
		{
			if (flag)
			{
				result = renderer.bounds;
				flag = false;
			}
			else
			{
				result.Encapsulate(renderer.bounds);
			}
		}
		return result;
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0002C39C File Offset: 0x0002A59C
	public static List<Tout> GetComponentListFromComponentArray<Tin, Tout>(IEnumerable<Tin> inComponents) where Tin : Component where Tout : Component
	{
		List<Tout> list = new List<Tout>();
		foreach (Tin tin in inComponents)
		{
			Tout component = tin.GetComponent<Tout>();
			if (component)
			{
				list.Add(component);
			}
		}
		return list;
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0002C404 File Offset: 0x0002A604
	internal static IEnumerable<T> SortBySiblingIndex<T>(IEnumerable<T> componentsToSort) where T : Component
	{
		List<T> list = new List<T>();
		list.AddRange(componentsToSort);
		list.Sort((T p1, T p2) => p1.transform.GetSiblingIndex().CompareTo(p2.transform.GetSiblingIndex()));
		return list;
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0002C437 File Offset: 0x0002A637
	internal static float FlatAngle(Vector3 dir1, Vector3 dir2)
	{
		return Vector3.Angle(dir1.Flat(), dir2.Flat());
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0002C44C File Offset: 0x0002A64C
	internal static void SetChildCollidersLayer(Transform root, int layerID)
	{
		Collider[] componentsInChildren = root.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = layerID;
		}
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0002C47C File Offset: 0x0002A67C
	internal static void SetJointDrive(ConfigurableJoint joint, float spring, float damper, Rigidbody rig)
	{
		JointDrive angularXDrive = joint.angularXDrive;
		angularXDrive.positionSpring = spring * rig.mass;
		angularXDrive.positionDamper = damper * rig.mass;
		joint.angularXDrive = angularXDrive;
		joint.angularYZDrive = angularXDrive;
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0002C4BC File Offset: 0x0002A6BC
	internal static Transform FindChildRecursive(string targetName, Transform root)
	{
		if (root.gameObject.name.ToUpper() == targetName.ToUpper())
		{
			return root;
		}
		for (int i = 0; i < root.childCount; i++)
		{
			Transform transform = HelperFunctions.FindChildRecursive(targetName, root.GetChild(i));
			if (!(transform == null) && transform.gameObject.name.ToUpper() == targetName.ToUpper())
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0002C530 File Offset: 0x0002A730
	internal static void PhysicsRotateTowards(Rigidbody rig, Vector3 from, Vector3 to, float force)
	{
		Vector3 a = Vector3.Cross(from, to).normalized * Vector3.Angle(from, to);
		rig.AddTorque(a * force, ForceMode.Acceleration);
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0002C567 File Offset: 0x0002A767
	internal static Vector3 MultiplyVectors(Vector3 v1, Vector3 v2)
	{
		v1.x *= v2.x;
		v1.y *= v2.y;
		v1.z *= v2.z;
		return v1;
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0002C59D File Offset: 0x0002A79D
	public static Vector3 CubicBezier(Vector3 Start, Vector3 _P1, Vector3 _P2, Vector3 end, float _t)
	{
		return (1f - _t) * HelperFunctions.QuadraticBezier(Start, _P1, _P2, _t) + _t * HelperFunctions.QuadraticBezier(_P1, _P2, end, _t);
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0002C5CC File Offset: 0x0002A7CC
	public static Vector3 QuadraticBezier(Vector3 start, Vector3 _P1, Vector3 end, float _t)
	{
		return (1f - _t) * HelperFunctions.LinearBezier(start, _P1, _t) + _t * HelperFunctions.LinearBezier(_P1, end, _t);
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0002C5F5 File Offset: 0x0002A7F5
	public static Vector3 LinearBezier(Vector3 start, Vector3 end, float _t)
	{
		return (1f - _t) * start + _t * end;
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0002C610 File Offset: 0x0002A810
	internal static Vector3 GetRandomPositionInBounds(Bounds bounds)
	{
		return new Vector3(Mathf.Lerp(bounds.min.x, bounds.max.x, Random.value), Mathf.Lerp(bounds.min.y, bounds.max.y, Random.value), Mathf.Lerp(bounds.min.z, bounds.max.z, Random.value));
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0002C688 File Offset: 0x0002A888
	internal static GameObject SpawnPrefab(GameObject gameObject, Vector3 position, Quaternion rotation, Transform transform)
	{
		GameObject gameObject2 = null;
		if (!Application.isEditor)
		{
			gameObject2 = Object.Instantiate<GameObject>(gameObject);
		}
		if (gameObject2 == null)
		{
			Debug.LogError("Failed to spawn prefab: " + gameObject.name, gameObject);
			return null;
		}
		gameObject2.transform.SetParent(transform);
		gameObject2.transform.rotation = rotation;
		gameObject2.transform.position = position;
		return gameObject2;
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0002C6EB File Offset: 0x0002A8EB
	internal static Quaternion GetRotationWithUp(Vector3 forward, Vector3 up)
	{
		return Quaternion.LookRotation(Vector3.ProjectOnPlane(forward, up), up);
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0002C6FC File Offset: 0x0002A8FC
	internal static float BoxDistance(Vector3 pos1, Vector3 pos2)
	{
		return Mathf.Max(Mathf.Max(Mathf.Max(0f, Mathf.Abs(pos1.x - pos2.x)), Mathf.Abs(pos1.y - pos2.y)), Mathf.Abs(pos1.z - pos2.z));
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0002C754 File Offset: 0x0002A954
	internal static bool CanSee(Transform looker, Vector3 pos, float maxAngle = 70f)
	{
		return Vector3.Angle(looker.forward, pos - looker.position) <= maxAngle && !HelperFunctions.LineCheck(looker.transform.position, pos, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0002C7A8 File Offset: 0x0002A9A8
	internal static bool InBoxRange(Vector3 position1, Vector3 position2, int range)
	{
		return Mathf.Abs(position1.x - position2.x) <= (float)range && Mathf.Abs(position1.y - position2.y) <= (float)range && Mathf.Abs(position1.z - position2.z) <= (float)range;
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0002C800 File Offset: 0x0002AA00
	internal static Random.State SetRandomSeedFromWorldPos(Vector3 position, int seed)
	{
		position.x = (float)Mathf.RoundToInt(position.x);
		position.y = (float)Mathf.RoundToInt(position.y);
		position.z = (float)Mathf.RoundToInt(position.z);
		Random.State state = Random.state;
		Debug.Log("Set Seed");
		Random.InitState(Mathf.RoundToInt((float)seed + position.x + position.y * 100f + position.z * 10000f));
		return state;
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0002C884 File Offset: 0x0002AA84
	public static List<Transform> FindAllChildrenWithTag(string targetTag, Transform target)
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < target.childCount; i++)
		{
			Transform child = target.GetChild(i);
			if (child.name.Contains(targetTag))
			{
				list.Add(child);
			}
			list.AddRange(HelperFunctions.FindAllChildrenWithTag(targetTag, child));
		}
		return list;
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0002C8D4 File Offset: 0x0002AAD4
	internal static T[] GridToFlatArray<T>(T[,] grid)
	{
		T[] array = new T[grid.GetLength(0) * grid.GetLength(1)];
		int length = grid.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
			{
				int num = i * length + j;
				array[num] = grid[j, i];
			}
		}
		return array;
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x0002C930 File Offset: 0x0002AB30
	internal static NativeArray<float> FloatGridToNativeArray(float[,] floats)
	{
		NativeArray<float> result = new NativeArray<float>(floats.GetLength(0) * floats.GetLength(1), Allocator.TempJob, NativeArrayOptions.ClearMemory);
		int length = floats.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
			{
				int index = i * length + j;
				result[index] = floats[i, j];
			}
		}
		return result;
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0002C990 File Offset: 0x0002AB90
	internal static float[,] NativeArrayToFloatGrid(NativeArray<float> array, int arrayLength)
	{
		float[,] array2 = new float[arrayLength, arrayLength];
		int length = array.Length;
		for (int i = 0; i < length; i++)
		{
			int num = Mathf.FloorToInt((float)(i / arrayLength));
			int num2 = i - num * arrayLength;
			array2[num, num2] = array[i];
		}
		return array2;
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0002C9DC File Offset: 0x0002ABDC
	public static Vector2Int GetIndex_FlatToGrid(int flatIndex, int arrayLength)
	{
		int num = Mathf.FloorToInt((float)(flatIndex / arrayLength));
		int y = flatIndex - num * arrayLength;
		return new Vector2Int(num, y);
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0002CA00 File Offset: 0x0002AC00
	public static int GetIndex_GridToFlat(Vector2Int gridIndex, int arrayLength)
	{
		return gridIndex.x * arrayLength + gridIndex.y;
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0002CA14 File Offset: 0x0002AC14
	internal static List<Vector2Int> GetIndexesInBounds(int xRess, int yRess, Bounds selectionBounds, Bounds totalBounds)
	{
		int num = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.x, totalBounds.max.x, selectionBounds.min.x) * (float)xRess);
		int num2 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.x, totalBounds.max.x, selectionBounds.max.x) * (float)xRess);
		int num3 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.z, totalBounds.max.z, selectionBounds.min.z) * (float)xRess);
		int num4 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.z, totalBounds.max.z, selectionBounds.max.z) * (float)yRess);
		List<Vector2Int> list = new List<Vector2Int>();
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				list.Add(new Vector2Int(i, j));
				HelperFunctions.IDToWorldPos(i, j, xRess, yRess, totalBounds);
			}
		}
		return list;
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0002CB2C File Offset: 0x0002AD2C
	public static Vector3 IDToWorldPos(int x, int y, int xRess, int yRess, Bounds totalBounds)
	{
		float t = (float)x / ((float)xRess - 1f);
		float t2 = (float)y / ((float)yRess - 1f);
		return new Vector3(Mathf.Lerp(totalBounds.min.x, totalBounds.max.x, t), 0f, Mathf.Lerp(totalBounds.min.z, totalBounds.max.z, t2));
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0002CB98 File Offset: 0x0002AD98
	internal static Vector3 GetRadomPointInBounds(Bounds b)
	{
		Vector3 min = b.min;
		Vector3 max = b.max;
		return new Vector3(Mathf.Lerp(min.x, max.x, Random.value), Mathf.Lerp(min.y, max.y, Random.value), Mathf.Lerp(min.z, max.z, Random.value));
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0002CBFC File Offset: 0x0002ADFC
	internal static Camera GetMainCamera()
	{
		if (MainCamera.instance == null)
		{
			MainCamera.instance = Object.FindAnyObjectByType<MainCamera>();
			MainCamera.instance.cam = MainCamera.instance.GetComponent<Camera>();
		}
		return MainCamera.instance.cam;
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x0002CC34 File Offset: 0x0002AE34
	internal static Color GetVertexColorAtPoint(Vector3[] verts, Color[] colors, Transform transform, Vector3 point)
	{
		if (colors.Length == 0)
		{
			return Color.black;
		}
		Color result = Color.black;
		float num = 10000000f;
		for (int i = 0; i < verts.Length; i++)
		{
			Vector3 b = transform.TransformPoint(verts[i]);
			float num2 = Vector3.Distance(point, b);
			if (num2 < num)
			{
				num = num2;
				result = colors[i];
			}
		}
		return result;
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x0002CC8D File Offset: 0x0002AE8D
	internal static float GetValue(Color color)
	{
		return Mathf.Max(new float[]
		{
			color.r,
			color.g,
			color.b
		});
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x0002CCB8 File Offset: 0x0002AEB8
	public static T RandomSelection<T>(List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		return list[Random.Range(0, list.Count)];
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x0002CCEC File Offset: 0x0002AEEC
	public static bool IsLayerInLayerMask(LayerMask layerMask, int layer)
	{
		return (layerMask.value & 1 << layer) != 0;
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x0002CCFF File Offset: 0x0002AEFF
	public static bool IsLayerInLayerMask(HelperFunctions.LayerType layerType, int layer)
	{
		return HelperFunctions.IsLayerInLayerMask(HelperFunctions.GetMask(layerType), layer);
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x0002CD0D File Offset: 0x0002AF0D
	public static Vector3 ZeroY(Vector3 original)
	{
		return new Vector3(original.x, 0f, original.z);
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x0002CD28 File Offset: 0x0002AF28
	internal static bool AnyPlayerInZRange(float min, float max)
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (!character.isBot && character.Center.z >= min && character.Center.z <= max)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040007C8 RID: 1992
	public static LayerMask AllPhysical = LayerMask.GetMask(new string[]
	{
		"Terrain",
		"Map",
		"Default",
		"Character",
		"Rope"
	});

	// Token: 0x040007C9 RID: 1993
	public static LayerMask AllPhysicalExceptCharacter = LayerMask.GetMask(new string[]
	{
		"Terrain",
		"Map",
		"Default",
		"Rope"
	});

	// Token: 0x040007CA RID: 1994
	public static LayerMask AllPhysicalExceptDefault = LayerMask.GetMask(new string[]
	{
		"Terrain",
		"Map",
		"Character",
		"Rope"
	});

	// Token: 0x040007CB RID: 1995
	public static LayerMask terrainMapMask = LayerMask.GetMask(new string[]
	{
		"Terrain",
		"Map"
	});

	// Token: 0x040007CC RID: 1996
	public static LayerMask terrainMask = LayerMask.GetMask(new string[]
	{
		"Terrain"
	});

	// Token: 0x040007CD RID: 1997
	public static LayerMask MapMask = LayerMask.GetMask(new string[]
	{
		"Map"
	});

	// Token: 0x040007CE RID: 1998
	public static LayerMask DefaultMask = LayerMask.GetMask(new string[]
	{
		"Default"
	});

	// Token: 0x040007CF RID: 1999
	public static LayerMask CharacterAndDefaultMask = LayerMask.GetMask(new string[]
	{
		"Character",
		"Default"
	});

	// Token: 0x02000464 RID: 1124
	public enum LayerType
	{
		// Token: 0x0400192A RID: 6442
		AllPhysical,
		// Token: 0x0400192B RID: 6443
		TerrainMap,
		// Token: 0x0400192C RID: 6444
		Terrain,
		// Token: 0x0400192D RID: 6445
		Map,
		// Token: 0x0400192E RID: 6446
		Default,
		// Token: 0x0400192F RID: 6447
		AllPhysicalExceptCharacter,
		// Token: 0x04001930 RID: 6448
		CharacterAndDefault,
		// Token: 0x04001931 RID: 6449
		AllPhysicalExceptDefault
	}
}
