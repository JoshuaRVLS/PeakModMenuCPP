using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x02000293 RID: 659
public class LavaRiver : CustomSpawnCondition
{
	// Token: 0x06001304 RID: 4868 RVA: 0x0005FFE3 File Offset: 0x0005E1E3
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		this.Spawn();
		return true;
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x0005FFEC File Offset: 0x0005E1EC
	private void OnDrawGizmosSelected()
	{
		for (int i = 0; i < this.frames.Count; i++)
		{
			Gizmos.color = Color.Lerp(Color.blue, Color.red, (float)i / (float)this.frames.Count);
			Gizmos.DrawSphere(this.frames[i].position, 0.1f);
			Gizmos.DrawLine(this.frames[i].position, this.frames[i].position + this.frames[i].up * 0.5f);
		}
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x0006009A File Offset: 0x0005E29A
	public void Spawn()
	{
		this.GenerateData();
		this.Apply();
		this.AddLights();
		this.addAlongSpline();
		if (this.spawnAlongSpline != null)
		{
			this.SpawnItemsAlongSpline();
		}
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x000600C8 File Offset: 0x0005E2C8
	private void AddLights()
	{
		Transform transform = base.transform.Find("BakedLight");
		if (transform == null)
		{
			return;
		}
		GameObject gameObject = transform.gameObject;
		Transform transform2 = base.transform.Find("BakedLights");
		for (int i = transform2.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(transform2.GetChild(i).gameObject);
		}
		if (this.spawnLights)
		{
			for (int j = 0; j < this.frames.Count; j += 3)
			{
				Object.Instantiate<GameObject>(gameObject, this.frames[j].position + this.frames[j].up * 4f, Quaternion.identity, transform2).SetActive(true);
			}
		}
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x00060194 File Offset: 0x0005E394
	private void addAlongSpline()
	{
		if (this.detailsAlongSpline.Length == 0)
		{
			return;
		}
		Transform transform = base.transform.Find("DetailObjects");
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < this.frames.Count; j += this.stepsAlongSpline)
		{
			for (int k = 0; k < this.detailsAlongSpline.Length; k++)
			{
				Object.Instantiate<GameObject>(this.detailsAlongSpline[k], this.frames[j].position, quaternion.identity, transform).SetActive(true);
			}
		}
		for (int l = 0; l < this.detailsAlongSpline.Length; l++)
		{
			this.detailsAlongSpline[l].SetActive(false);
		}
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x00060264 File Offset: 0x0005E464
	private void SpawnItemsAlongSpline()
	{
		this.splineObjectParent = base.transform.Find("SpawnedObjects");
		if (this.splineObjectParent == null)
		{
			this.splineObjectParent = new GameObject("SpawnedObjects").transform;
			this.splineObjectParent.SetParent(base.transform);
		}
		for (int i = this.splineObjectParent.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(this.splineObjectParent.GetChild(i).gameObject);
		}
		for (int j = 0; j < this.frames.Count; j++)
		{
			Object.Instantiate<GameObject>(this.spawnAlongSpline, this.frames[j].position, Quaternion.LookRotation(this.frames[j].forward), this.splineObjectParent).SetActive(true);
		}
		this.spawnAlongSpline.SetActive(false);
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x00060349 File Offset: 0x0005E549
	private void GenerateData()
	{
		this.Simulate();
		this.Simplify();
		this.SmoothUps();
		this.SmoothUps();
		this.SmoothUps();
		this.SmoothUps();
		this.SmoothUps();
	}

	// Token: 0x0600130B RID: 4875 RVA: 0x00060378 File Offset: 0x0005E578
	public void Apply()
	{
		SplineContainer componentInChildren = base.GetComponentInChildren<SplineContainer>();
		componentInChildren.transform.position = Vector3.zero;
		componentInChildren.transform.rotation = Quaternion.identity;
		for (int i = componentInChildren.Splines.Count - 1; i >= 0; i--)
		{
			componentInChildren.RemoveSplineAt(i);
		}
		Spline spline = new Spline();
		foreach (LavaRiver.LavaRiverFrame lavaRiverFrame in this.frames)
		{
			Quaternion rotationWithUp = HelperFunctions.GetRotationWithUp(lavaRiverFrame.forward, lavaRiverFrame.up);
			spline.Add(new BezierKnot(lavaRiverFrame.position, Vector3.zero, Vector3.zero, rotationWithUp), TangentMode.AutoSmooth);
		}
		for (int j = 0; j < spline.Count; j++)
		{
			spline.SetKnot(j, new BezierKnot(spline[j].Position, spline[j].TangentIn, spline[j].TangentOut, Quaternion.Euler(Vector3.left)), BezierTangent.Out);
		}
		componentInChildren.AddSpline(spline);
		SplineExtrude component = componentInChildren.GetComponent<SplineExtrude>();
		component.GetComponent<MeshFilter>().mesh = new Mesh();
		component.Capped = true;
		component.Rebuild();
		if (this.splash != null)
		{
			this.splash.transform.position = this.frames[this.frames.Count - 1].position;
		}
		if (this.endRock != null)
		{
			this.endRock.transform.position = this.frames[this.frames.Count - 1].position;
			this.endRock.transform.rotation = UnityEngine.Random.rotation;
		}
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x00060568 File Offset: 0x0005E768
	private void Simulate()
	{
		LavaRiver.<>c__DisplayClass25_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this.frames = new List<LavaRiver.LavaRiverFrame>();
		this.steps = this.maxSteps;
		CS$<>8__locals1.vel = base.transform.forward * this.spawnVel;
		CS$<>8__locals1.pos = base.transform.position + base.transform.up * 0.1f + base.transform.forward * 0.1f;
		CS$<>8__locals1.up = base.transform.up;
		CS$<>8__locals1.lastPos = CS$<>8__locals1.pos;
		while (this.<Simulate>g__SimulationStep|25_0(ref CS$<>8__locals1))
		{
		}
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x00060620 File Offset: 0x0005E820
	public void SmoothUps()
	{
		for (int i = 1; i < this.frames.Count - 1; i++)
		{
			LavaRiver.LavaRiverFrame lavaRiverFrame = this.frames[i - 1];
			LavaRiver.LavaRiverFrame lavaRiverFrame2 = this.frames[i];
			LavaRiver.LavaRiverFrame lavaRiverFrame3 = this.frames[i + 1];
			Vector3 normalized = (lavaRiverFrame.up + lavaRiverFrame2.up + lavaRiverFrame3.up).normalized;
			lavaRiverFrame2.up = normalized;
		}
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x0006069C File Offset: 0x0005E89C
	public void Simplify()
	{
		for (int i = 1; i < this.frames.Count; i++)
		{
			LavaRiver.LavaRiverFrame lavaRiverFrame = this.frames[i - 1];
			LavaRiver.LavaRiverFrame lavaRiverFrame2 = this.frames[i];
			if (Vector3.Distance(lavaRiverFrame.position, lavaRiverFrame2.position) < this.prefDistancePerFrame)
			{
				this.frames.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x00060704 File Offset: 0x0005E904
	public void Clear()
	{
		this.frames.Clear();
		SplineContainer componentInChildren = base.GetComponentInChildren<SplineContainer>();
		componentInChildren.transform.position = Vector3.zero;
		componentInChildren.transform.rotation = Quaternion.identity;
		for (int i = componentInChildren.Splines.Count - 1; i >= 0; i--)
		{
			componentInChildren.RemoveSplineAt(i);
		}
		componentInChildren.GetComponent<SplineExtrude>().Rebuild();
		this.endRock.transform.position = base.transform.position;
		if (this.splash != null)
		{
			this.splash.transform.position = base.transform.position;
		}
		if (this.splineObjectParent != null)
		{
			for (int j = this.splineObjectParent.childCount - 1; j >= 0; j--)
			{
				Object.DestroyImmediate(this.splineObjectParent.GetChild(j).gameObject);
			}
		}
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x00060864 File Offset: 0x0005EA64
	[CompilerGenerated]
	private bool <Simulate>g__SimulationStep|25_0(ref LavaRiver.<>c__DisplayClass25_0 A_1)
	{
		this.steps--;
		if (this.steps < 0)
		{
			return false;
		}
		if (A_1.vel.magnitude < 0.01f)
		{
			return false;
		}
		if (Vector3.Distance(base.transform.position, A_1.pos) > this.maxLength)
		{
			return false;
		}
		A_1.vel += Vector3.down * this.gravity * this.timeStep;
		A_1.vel += A_1.up * -this.wallStick * this.timeStep;
		A_1.vel *= this.drag;
		Vector3 vector = A_1.pos + A_1.vel * this.timeStep;
		RaycastHit raycastHit = HelperFunctions.LineCheck(A_1.lastPos, vector, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			A_1.up = raycastHit.normal;
			vector = raycastHit.point + raycastHit.normal * 0.05f;
			A_1.vel = Vector3.ProjectOnPlane(A_1.vel, raycastHit.normal);
		}
		A_1.pos = vector;
		this.frames.Add(new LavaRiver.LavaRiverFrame
		{
			position = A_1.pos,
			up = A_1.up,
			forward = A_1.vel.normalized
		});
		A_1.lastPos = A_1.pos;
		return true;
	}

	// Token: 0x04001120 RID: 4384
	public float spawnVel = 5f;

	// Token: 0x04001121 RID: 4385
	public float gravity = 10f;

	// Token: 0x04001122 RID: 4386
	public float wallStick;

	// Token: 0x04001123 RID: 4387
	public float drag = 0.8f;

	// Token: 0x04001124 RID: 4388
	public float timeStep = 0.02f;

	// Token: 0x04001125 RID: 4389
	public int maxSteps = 1000;

	// Token: 0x04001126 RID: 4390
	public float maxLength = 500f;

	// Token: 0x04001127 RID: 4391
	public bool spawnLights = true;

	// Token: 0x04001128 RID: 4392
	private int steps;

	// Token: 0x04001129 RID: 4393
	public float prefDistancePerFrame = 0.3f;

	// Token: 0x0400112A RID: 4394
	public GameObject endRock;

	// Token: 0x0400112B RID: 4395
	public GameObject splash;

	// Token: 0x0400112C RID: 4396
	public GameObject spawnAlongSpline;

	// Token: 0x0400112D RID: 4397
	public GameObject[] detailsAlongSpline;

	// Token: 0x0400112E RID: 4398
	public int stepsAlongSpline;

	// Token: 0x0400112F RID: 4399
	private Transform splineObjectParent;

	// Token: 0x04001130 RID: 4400
	public List<LavaRiver.LavaRiverFrame> frames = new List<LavaRiver.LavaRiverFrame>();

	// Token: 0x0200050B RID: 1291
	[Serializable]
	public class LavaRiverFrame
	{
		// Token: 0x04001C0E RID: 7182
		public Vector3 position;

		// Token: 0x04001C0F RID: 7183
		public Vector3 up;

		// Token: 0x04001C10 RID: 7184
		public Vector3 forward;
	}
}
