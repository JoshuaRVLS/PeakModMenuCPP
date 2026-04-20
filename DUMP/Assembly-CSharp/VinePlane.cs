using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000375 RID: 885
public class VinePlane : MonoBehaviour
{
	// Token: 0x06001734 RID: 5940 RVA: 0x00077628 File Offset: 0x00075828
	private void Start()
	{
		this.UpdateCollider();
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x00077630 File Offset: 0x00075830
	private void OnValidate()
	{
		if (!Application.isPlaying && this.liveEdit)
		{
			this.Blast();
		}
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x00077648 File Offset: 0x00075848
	public void Blast()
	{
		this.meshCollider.enabled = false;
		this.RestoreDefaults();
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			Transform child = this.bonesParent.GetChild(i);
			Vector3 start = child.transform.position + child.transform.up * this.raycastStartLength;
			Vector3 end = child.transform.position - child.transform.up * (this.raycastStartLength + this.raycastEndLength);
			RaycastHit raycastHit;
			if (Physics.Linecast(start, end, out raycastHit, this.mask.value, QueryTriggerInteraction.Ignore))
			{
				if (child.gameObject.activeSelf)
				{
					child.transform.position = raycastHit.point + base.transform.up * this.lift * this.GetDistanceFromCorner(i);
				}
				else
				{
					child.transform.position = raycastHit.point;
				}
				Plane plane = new Plane(base.transform.up, base.transform.position);
				if (child.gameObject.activeSelf)
				{
					float d = Mathf.Pow(Mathf.Clamp01(Mathf.Abs(plane.GetDistanceToPoint(child.transform.position) / this.raycastEndLength)), this.planeLiftPow);
					child.transform.position += base.transform.up * d * this.planeLiftAmount;
				}
			}
		}
		if (!this.liveEdit)
		{
			this.Bake();
			return;
		}
		this.skinnedMeshRenderer.material = this.editingMaterial;
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x00077804 File Offset: 0x00075A04
	public void Bake()
	{
		this.meshCollider.enabled = true;
		this.liveEdit = false;
		this.UpdateCollider();
		if (this.vineType == VinePlane.VineType.Normal)
		{
			this.skinnedMeshRenderer.material = this.vineMatNormal;
		}
		else if (this.vineType == VinePlane.VineType.Thorns)
		{
			this.skinnedMeshRenderer.material = this.vineMatThorns;
		}
		else if (this.vineType == VinePlane.VineType.Poison)
		{
			this.skinnedMeshRenderer.material = this.vineMatPoison;
		}
		this.skinnedMeshRendererLeaves.material = this.skinnedMeshRenderer.material;
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x00077891 File Offset: 0x00075A91
	private float GetDistanceFromCorner(int index)
	{
		return Mathf.InverseLerp(this.distanceToCorner, 0f, Vector3.Distance(this.bonesParent.GetChild(index).position, this.centerBone.position));
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x000778C4 File Offset: 0x00075AC4
	private void RestoreDefaultsButton()
	{
		this.RestoreDefaults();
		this.Bake();
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x000778D4 File Offset: 0x00075AD4
	private void RestoreDefaults()
	{
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			this.bonesParent.GetChild(i).localPosition = this.defaultPositions[i];
			this.bonesParent.GetChild(i).localRotation = this.defaultRotations[i];
		}
		for (int j = 0; j < this.bonesParent.childCount; j++)
		{
			if (Mathf.Abs(this.bonesParent.GetChild(j).localPosition.y) > 3.9f)
			{
				this.bonesParent.GetChild(j).gameObject.SetActive(false);
			}
			else
			{
				this.bonesParent.GetChild(j).gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x0007799C File Offset: 0x00075B9C
	private void SetDefaultsBECAREFUL()
	{
		this.defaultPositions.Clear();
		this.defaultRotations.Clear();
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			this.defaultPositions.Add(this.bonesParent.GetChild(i).localPosition);
			this.defaultRotations.Add(this.bonesParent.GetChild(i).localRotation);
		}
		this.Bake();
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x00077A14 File Offset: 0x00075C14
	private void UpdateCollider()
	{
		this.skinnedMeshRenderer.ResetBounds();
		this.bakedMesh = new Mesh();
		this.skinnedMeshRenderer.BakeMesh(this.bakedMesh, true);
		this.meshCollider.sharedMesh = null;
		this.meshCollider.sharedMesh = this.bakedMesh;
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x00077A68 File Offset: 0x00075C68
	private void OnDrawGizmos()
	{
		Plane plane = new Plane(base.transform.up, base.transform.position);
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			if (this.bonesParent.GetChild(i).gameObject.activeSelf)
			{
				float num = Mathf.Abs(plane.GetDistanceToPoint(this.bonesParent.GetChild(i).transform.position));
				Gizmos.color = new Color(num, num, num);
				Gizmos.DrawSphere(this.bonesParent.GetChild(i).transform.position, 0.1f);
			}
		}
	}

	// Token: 0x040015AA RID: 5546
	public SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x040015AB RID: 5547
	public SkinnedMeshRenderer skinnedMeshRendererLeaves;

	// Token: 0x040015AC RID: 5548
	public MeshCollider meshCollider;

	// Token: 0x040015AD RID: 5549
	private Mesh bakedMesh;

	// Token: 0x040015AE RID: 5550
	public Transform bonesParent;

	// Token: 0x040015AF RID: 5551
	public float raycastStartLength = 1f;

	// Token: 0x040015B0 RID: 5552
	public float raycastEndLength = 5f;

	// Token: 0x040015B1 RID: 5553
	public LayerMask mask;

	// Token: 0x040015B2 RID: 5554
	public float distanceToCorner = 5f;

	// Token: 0x040015B3 RID: 5555
	public Transform centerBone;

	// Token: 0x040015B4 RID: 5556
	public Material vineMatNormal;

	// Token: 0x040015B5 RID: 5557
	public Material vineMatPoison;

	// Token: 0x040015B6 RID: 5558
	public Material vineMatThorns;

	// Token: 0x040015B7 RID: 5559
	public Material editingMaterial;

	// Token: 0x040015B8 RID: 5560
	public VinePlane.VineType vineType;

	// Token: 0x040015B9 RID: 5561
	public float lift = 0.1f;

	// Token: 0x040015BA RID: 5562
	public float planeLiftAmount = 0.5f;

	// Token: 0x040015BB RID: 5563
	public float planeLiftPow = 5f;

	// Token: 0x040015BC RID: 5564
	public bool liveEdit;

	// Token: 0x040015BD RID: 5565
	public List<Vector3> defaultPositions = new List<Vector3>();

	// Token: 0x040015BE RID: 5566
	public List<Quaternion> defaultRotations = new List<Quaternion>();

	// Token: 0x02000550 RID: 1360
	public enum VineType
	{
		// Token: 0x04001D08 RID: 7432
		Normal,
		// Token: 0x04001D09 RID: 7433
		Poison,
		// Token: 0x04001D0A RID: 7434
		Thorns
	}
}
