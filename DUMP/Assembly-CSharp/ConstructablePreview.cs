using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class ConstructablePreview : MonoBehaviour
{
	// Token: 0x06000904 RID: 2308 RVA: 0x00031530 File Offset: 0x0002F730
	public void SetValid(bool valid)
	{
		this.enableIfValid.SetActive(valid);
		this.enableIfInvalid.SetActive(!valid);
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00031550 File Offset: 0x0002F750
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		foreach (ConstructablePreview.ConstructablePreviewAvoidanceSphere constructablePreviewAvoidanceSphere in this.avoidanceSpheres)
		{
			Gizmos.DrawWireSphere(base.transform.TransformPoint(constructablePreviewAvoidanceSphere.position), constructablePreviewAvoidanceSphere.radius);
		}
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x000315C4 File Offset: 0x0002F7C4
	public bool CollisionValid()
	{
		foreach (ConstructablePreview.ConstructablePreviewAvoidanceSphere constructablePreviewAvoidanceSphere in this.avoidanceSpheres)
		{
			if (Physics.CheckSphere(base.transform.TransformPoint(constructablePreviewAvoidanceSphere.position), constructablePreviewAvoidanceSphere.radius, HelperFunctions.GetMask(constructablePreviewAvoidanceSphere.layerType), QueryTriggerInteraction.Ignore))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04000897 RID: 2199
	public GameObject enableIfValid;

	// Token: 0x04000898 RID: 2200
	public GameObject enableIfInvalid;

	// Token: 0x04000899 RID: 2201
	public List<ConstructablePreview.ConstructablePreviewAvoidanceSphere> avoidanceSpheres;

	// Token: 0x0200046E RID: 1134
	[Serializable]
	public class ConstructablePreviewAvoidanceSphere
	{
		// Token: 0x04001975 RID: 6517
		public Vector3 position;

		// Token: 0x04001976 RID: 6518
		public float radius;

		// Token: 0x04001977 RID: 6519
		public HelperFunctions.LayerType layerType;
	}
}
