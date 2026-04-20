using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002D2 RID: 722
public class PlayerGizmos : MonoBehaviour
{
	// Token: 0x06001444 RID: 5188 RVA: 0x00066D2F File Offset: 0x00064F2F
	private void Start()
	{
		PlayerGizmos.instance = this;
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x00066D38 File Offset: 0x00064F38
	private void Update()
	{
		for (int i = this.gizmos.Count - 1; i >= 0; i--)
		{
			GizmoInstance gizmoInstance = this.gizmos[i];
			if (gizmoInstance == null)
			{
				this.gizmos.RemoveAt(i);
			}
			else
			{
				gizmoInstance.framesSinceActivated++;
				if (gizmoInstance.framesSinceActivated > 5)
				{
					gizmoInstance.giz.SetActive(false);
					this.gizmos.Remove(gizmoInstance);
				}
			}
		}
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x00066DAC File Offset: 0x00064FAC
	public void DisplayGizmo(PlayerGizmos.GizmoType gizmoType, Vector3 pos, Vector3 direction)
	{
		GameObject gizmo = this.GetGizmo(gizmoType);
		GizmoInstance gizmoInstance = this.Contains(gizmo);
		if (gizmoInstance != null)
		{
			gizmoInstance.framesSinceActivated = 0;
		}
		else
		{
			this.gizmos.Add(new GizmoInstance
			{
				giz = gizmo,
				framesSinceActivated = 0
			});
		}
		gizmo.SetActive(true);
		gizmo.transform.position = pos;
		gizmo.transform.rotation = Quaternion.LookRotation(direction);
	}

	// Token: 0x06001447 RID: 5191 RVA: 0x00066E17 File Offset: 0x00065017
	private GameObject GetGizmo(PlayerGizmos.GizmoType gizmoType)
	{
		if (gizmoType == PlayerGizmos.GizmoType.Pointer)
		{
			return this.pointer;
		}
		return null;
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x00066E24 File Offset: 0x00065024
	private GizmoInstance Contains(GameObject gizmo)
	{
		foreach (GizmoInstance gizmoInstance in this.gizmos)
		{
			if (gizmoInstance.giz == gizmo)
			{
				return gizmoInstance;
			}
		}
		return null;
	}

	// Token: 0x0400127B RID: 4731
	public List<GizmoInstance> gizmos = new List<GizmoInstance>();

	// Token: 0x0400127C RID: 4732
	public static PlayerGizmos instance;

	// Token: 0x0400127D RID: 4733
	public GameObject pointer;

	// Token: 0x02000521 RID: 1313
	public enum GizmoType
	{
		// Token: 0x04001C60 RID: 7264
		Pointer
	}
}
