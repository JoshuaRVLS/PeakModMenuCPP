using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

// Token: 0x02000136 RID: 310
[ExecuteInEditMode]
public class LightVolume : MonoBehaviour
{
	// Token: 0x060009EE RID: 2542 RVA: 0x00034EB8 File Offset: 0x000330B8
	private void SetShaderVars()
	{
		Shader.SetGlobalFloat("brightness", this.brightness);
		Shader.SetGlobalFloat("ambienceStrength", this.ambienceStrength);
		Shader.SetGlobalFloat("ambienceMin", this.ambienceMin);
		Shader.SetGlobalVector("gridRes", this.gridRes);
		Shader.SetGlobalFloat("raySpacing", this.raySpacing);
		Shader.SetGlobalVector("gridOffset", this.gridOffset);
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x00034F34 File Offset: 0x00033134
	public void SetSize()
	{
		Shader.SetGlobalTexture("_LightMap", null);
		Bounds totalBounds = LightVolume.GetTotalBounds((this.sceneParent == null) ? base.gameObject : this.sceneParent);
		this.gridOffset = totalBounds.center;
		this.gridRes = new Vector3Int(Mathf.CeilToInt((totalBounds.size.x + 3f) / this.raySpacing), Mathf.CeilToInt((totalBounds.size.y + 3f) / this.raySpacing), Mathf.CeilToInt((totalBounds.size.z + 3f) / this.raySpacing));
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x00034FE0 File Offset: 0x000331E0
	private static Bounds GetTotalBounds(GameObject gameObject)
	{
		Bounds result = default(Bounds);
		bool flag = true;
		foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
		{
			if (flag)
			{
				result = meshRenderer.bounds;
			}
			else
			{
				result.Encapsulate(meshRenderer.bounds);
			}
			flag = false;
		}
		return result;
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00035030 File Offset: 0x00033230
	private void OnDrawGizmosSelected()
	{
		if (!this.showVolumeGizmos)
		{
			return;
		}
		Gizmos.color = Color.black;
		Gizmos.DrawWireCube(this.gridOffset - Vector3.one * 0.25f, this.gridRes * this.raySpacing);
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(this.gridOffset + Vector3.one * 0.25f, this.gridRes * this.raySpacing);
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x000350C4 File Offset: 0x000332C4
	private void Awake()
	{
		LightVolume.instance = this;
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x000350CC File Offset: 0x000332CC
	private void Start()
	{
		this.SetShaderVars();
		Shader.SetGlobalTexture("_LightMap", this.lightMap);
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x060009F4 RID: 2548 RVA: 0x000350E4 File Offset: 0x000332E4
	private bool RaytracingShaderNotSupported
	{
		get
		{
			return !SystemInfo.supportsRayTracingShaders;
		}
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x000350F0 File Offset: 0x000332F0
	public void Bake(Action onComplete = null)
	{
		if (!this.computeShader)
		{
			Debug.LogError("Cannot bake at runtime (serialize the ComputeShader if you want to do this)");
			return;
		}
		if (!this.rayTracingShader)
		{
			Debug.LogError("Cannot bake at runtime (serialize the RayTracingShader if you want to do this)");
			return;
		}
		this.SetSize();
		RenderTexture inputTex = this.RunBake();
		RenderTexture renderTexture = this.RunBlur(inputTex);
		renderTexture.name = "LightVolumeRenderTexture";
		this.SetShaderVars();
		Shader.SetGlobalTexture("_LightMap", renderTexture);
		this.SaveTex(renderTexture, onComplete);
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00035168 File Offset: 0x00033368
	private RenderTexture RunBake()
	{
		this.rayTracingShader.SetVector("gridRadius", new Vector3((float)this.gridRes.x, (float)this.gridRes.y, (float)this.gridRes.z) * (this.raySpacing / 2f));
		this.rayTracingShader.SetVector("gridOffset", this.gridOffset);
		this.rayTracingShader.SetVector("skyColor", this.skyColor);
		this.rayTracingShader.SetInt("rayCount", this.rayCount);
		ComputeBuffer computeBuffer;
		int num = this.BuildLights(out computeBuffer);
		IDisposable disposable;
		this.BuildMeshes(out disposable, num);
		RenderTexture renderTexture = LightVolume.Create3DTexture(FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, this.gridRes);
		this.rayTracingShader.SetTexture("lightMap", renderTexture);
		for (int i = 0; i < num + 1; i++)
		{
			this.rayTracingShader.SetInt("doLightIndex", i);
			this.rayTracingShader.Dispatch("RaygenShader", this.gridRes.x, this.gridRes.y, this.gridRes.z, null);
		}
		computeBuffer.Dispose();
		disposable.Dispose();
		return renderTexture;
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x000352A4 File Offset: 0x000334A4
	private static RenderTexture Create3DTexture(FilterMode filterMode, RenderTextureFormat format, Vector3Int resolution)
	{
		RenderTexture renderTexture = new RenderTexture(resolution.x, resolution.y, 0);
		renderTexture.enableRandomWrite = true;
		renderTexture.format = format;
		renderTexture.dimension = TextureDimension.Tex3D;
		renderTexture.volumeDepth = resolution.z;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = filterMode;
		renderTexture.hideFlags = HideFlags.DontSave;
		if (!renderTexture.Create())
		{
			throw new Exception("Failed to create texture");
		}
		return renderTexture;
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00035310 File Offset: 0x00033510
	private int BuildLights(out ComputeBuffer toDispose)
	{
		List<LightVolume.GpuLight> list = new List<LightVolume.GpuLight>();
		GameObject gameObject = (this.sceneParent == null) ? base.gameObject : this.sceneParent;
		if (this.allLightsFound == null)
		{
			this.allLightsFound = new List<BakedVolumeLight>();
		}
		this.allLightsFound.Clear();
		foreach (BakedVolumeLight bakedVolumeLight in gameObject.GetComponentsInChildren<BakedVolumeLight>())
		{
			this.allLightsFound.Add(bakedVolumeLight);
			Vector3 a = new Vector3(bakedVolumeLight.color.r, bakedVolumeLight.color.g, bakedVolumeLight.color.b);
			BakedVolumeLight.LightModes mode = bakedVolumeLight.mode;
			float num;
			if (mode != BakedVolumeLight.LightModes.Point)
			{
				if (mode != BakedVolumeLight.LightModes.Spot)
				{
					throw new Exception();
				}
				num = bakedVolumeLight.coneSize * 0.017453292f;
			}
			else
			{
				num = 0f;
			}
			float coneSize = num;
			list.Add(new LightVolume.GpuLight
			{
				Position = bakedVolumeLight.transform.position,
				ConeSize = coneSize,
				Direction = bakedVolumeLight.transform.forward,
				Radius = bakedVolumeLight.GetRadius(),
				Color = a * bakedVolumeLight.intensity,
				Falloff = bakedVolumeLight.falloff,
				ConeFalloff = bakedVolumeLight.coneFalloff
			});
		}
		int count = list.Count;
		if (count == 0)
		{
			list.Add(default(LightVolume.GpuLight));
		}
		ComputeBuffer computeBuffer = new ComputeBuffer(list.Count, 52);
		computeBuffer.SetData<LightVolume.GpuLight>(list);
		this.rayTracingShader.SetBuffer("lightBuffer", computeBuffer);
		this.rayTracingShader.SetInt("lightBufferLength", count);
		toDispose = computeBuffer;
		return count;
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x000354C4 File Offset: 0x000336C4
	private void BuildMeshes(out IDisposable toDispose, int lightCountForDebug)
	{
		int value = this.occluderMask.value;
		GameObject gameObject = (this.sceneParent == null) ? base.gameObject : this.sceneParent;
		if (this.allMeshRenderersFound == null)
		{
			this.allMeshRenderersFound = new List<MeshRenderer>();
		}
		this.allMeshRenderersFound.Clear();
		RayTracingAccelerationStructure rayTracingAccelerationStructure = new RayTracingAccelerationStructure();
		uint num = 0U;
		int num2 = 0;
		foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
		{
			if ((1 << meshRenderer.gameObject.layer & value) != 0 || meshRenderer.GetComponent<LightingCollider>())
			{
				this.allMeshRenderersFound.Add(meshRenderer);
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (component == null)
				{
					Debug.LogError("Mesh renderer without filter: " + meshRenderer.gameObject.name, meshRenderer.gameObject);
				}
				Mesh sharedMesh = component.sharedMesh;
				if (!(sharedMesh == null))
				{
					int subMeshCount = sharedMesh.subMeshCount;
					for (int j = 0; j < subMeshCount; j++)
					{
						num += sharedMesh.GetIndexCount(j);
					}
					num2 += sharedMesh.vertexCount;
					RayTracingSubMeshFlags[] array = new RayTracingSubMeshFlags[subMeshCount];
					for (int k = 0; k < array.Length; k++)
					{
						array[k] = (RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly);
					}
					rayTracingAccelerationStructure.AddInstance(meshRenderer, array, true, false, 255U, uint.MaxValue);
				}
			}
		}
		rayTracingAccelerationStructure.Build();
		Debug.Log(string.Format("Light Volume Baker found: {0} lights, {1} meshes, {2} indices, {3} vertices", new object[]
		{
			lightCountForDebug,
			this.allMeshRenderersFound.Count,
			num,
			num2
		}));
		this.rayTracingShader.SetAccelerationStructure("g_SceneAccelStruct", rayTracingAccelerationStructure);
		toDispose = rayTracingAccelerationStructure;
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x00035680 File Offset: 0x00033880
	private RenderTexture RunBlur(RenderTexture inputTex)
	{
		if (this.blurRadius <= 0)
		{
			return inputTex;
		}
		this.computeShader.SetInt("blurRadius", this.blurRadius);
		Vector3Int resolution = new Vector3Int(inputTex.width, inputTex.height, inputTex.volumeDepth);
		RenderTexture renderTexture = LightVolume.Create3DTexture(inputTex.filterMode, inputTex.format, resolution);
		for (int i = 0; i < 3; i++)
		{
			this.computeShader.SetTexture(1, "blurInputLightMap", inputTex);
			this.computeShader.SetTexture(1, "lightMap", renderTexture);
			this.computeShader.SetInt("blurAxis", i);
			uint num = 4U;
			uint num2 = 4U;
			uint num3 = 4U;
			long num4 = ((long)resolution.x + (long)((ulong)num) - 1L) / (long)((ulong)num);
			long num5 = ((long)resolution.y + (long)((ulong)num2) - 1L) / (long)((ulong)num2);
			long num6 = ((long)resolution.z + (long)((ulong)num3) - 1L) / (long)((ulong)num3);
			this.computeShader.Dispatch(1, (int)num4, (int)num5, (int)num6);
			RenderTexture renderTexture2 = inputTex;
			RenderTexture renderTexture3 = renderTexture;
			renderTexture = renderTexture2;
			inputTex = renderTexture3;
		}
		Object.DestroyImmediate(renderTexture);
		return inputTex;
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x0003578C File Offset: 0x0003398C
	private void SaveTex(RenderTexture renderTexture, Action onComplete = null)
	{
		AsyncGPUReadbackRequest asyncGPUReadbackRequest = AsyncGPUReadback.Request(renderTexture, 0, null);
		asyncGPUReadbackRequest.WaitForCompletion();
		byte[] array = new byte[asyncGPUReadbackRequest.layerDataSize * asyncGPUReadbackRequest.layerCount];
		for (int i = 0; i < asyncGPUReadbackRequest.layerCount; i++)
		{
			NativeArray<byte>.Copy(asyncGPUReadbackRequest.GetData<byte>(i), 0, array, i * asyncGPUReadbackRequest.layerDataSize, asyncGPUReadbackRequest.layerDataSize);
		}
		if (!this.lightMap || this.lightMap.width != renderTexture.width || this.lightMap.height != renderTexture.height || this.lightMap.depth != renderTexture.volumeDepth || this.lightMap.graphicsFormat != renderTexture.graphicsFormat)
		{
			if (this.lightMap)
			{
				Object.DestroyImmediate(this.lightMap);
			}
			this.lightMap = new Texture3D(renderTexture.width, renderTexture.height, renderTexture.volumeDepth, renderTexture.graphicsFormat, TextureCreationFlags.None);
		}
		this.lightMap.name = "LightVolumeBakeTexture";
		this.lightMap.wrapMode = renderTexture.wrapMode;
		this.lightMap.filterMode = renderTexture.filterMode;
		this.lightMap.SetPixelData<byte>(array, 0, 0);
		this.lightMap.Apply();
		Shader.SetGlobalTexture("_LightMap", this.lightMap);
		if (onComplete != null)
		{
			onComplete();
		}
		Object.DestroyImmediate(renderTexture);
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x000358EE File Offset: 0x00033AEE
	public static LightVolume Instance()
	{
		if (LightVolume.instance == null)
		{
			LightVolume.instance = Object.FindAnyObjectByType<LightVolume>();
		}
		return LightVolume.instance;
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x0003590C File Offset: 0x00033B0C
	internal Color SamplePosition(Vector3 worldPos)
	{
		worldPos -= this.gridOffset;
		worldPos += this.raySpacing * this.gridRes * 0.5f;
		worldPos.x /= this.raySpacing;
		worldPos.y /= this.raySpacing;
		worldPos.z /= this.raySpacing;
		return this.lightMap.GetPixel((int)worldPos.x, (int)worldPos.y, (int)worldPos.z);
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x000359A4 File Offset: 0x00033BA4
	public float SamplePositionAlpha(Vector3 worldPos)
	{
		worldPos -= this.gridOffset;
		worldPos += this.raySpacing * this.gridRes * 0.5f;
		worldPos.x /= this.raySpacing;
		worldPos.y /= this.raySpacing;
		worldPos.z /= this.raySpacing;
		return this.lightMap.GetPixel((int)worldPos.x, (int)worldPos.y, (int)worldPos.z).a;
	}

	// Token: 0x0400094E RID: 2382
	public bool showVolumeGizmos = true;

	// Token: 0x0400094F RID: 2383
	public float brightness = 1f;

	// Token: 0x04000950 RID: 2384
	public float ambienceStrength = 1f;

	// Token: 0x04000951 RID: 2385
	public float ambienceMin = 0.05f;

	// Token: 0x04000952 RID: 2386
	public Color skyColor = Color.white;

	// Token: 0x04000953 RID: 2387
	public Vector3Int gridRes;

	// Token: 0x04000954 RID: 2388
	public Vector3 gridOffset;

	// Token: 0x04000955 RID: 2389
	public int rayCount = 128;

	// Token: 0x04000956 RID: 2390
	public float raySpacing = 1.5f;

	// Token: 0x04000957 RID: 2391
	[Tooltip("Colliders matching this mask will be used for light tracing, colliders not matching will be ignored")]
	public LayerMask occluderMask = -1;

	// Token: 0x04000958 RID: 2392
	[Tooltip("Radius (in texels) for how much to box blur the output texture")]
	public int blurRadius;

	// Token: 0x04000959 RID: 2393
	public GameObject sceneParent;

	// Token: 0x0400095A RID: 2394
	public ComputeShader computeShader;

	// Token: 0x0400095B RID: 2395
	public RayTracingShader rayTracingShader;

	// Token: 0x0400095C RID: 2396
	public Texture3D lightMap;

	// Token: 0x0400095D RID: 2397
	public List<BakedVolumeLight> allLightsFound;

	// Token: 0x0400095E RID: 2398
	public List<MeshRenderer> allMeshRenderersFound;

	// Token: 0x0400095F RID: 2399
	internal static LightVolume instance;

	// Token: 0x0200047A RID: 1146
	private struct GpuLight
	{
		// Token: 0x0400199B RID: 6555
		public Vector3 Position;

		// Token: 0x0400199C RID: 6556
		public float ConeSize;

		// Token: 0x0400199D RID: 6557
		public Vector3 Direction;

		// Token: 0x0400199E RID: 6558
		public float Radius;

		// Token: 0x0400199F RID: 6559
		public Vector3 Color;

		// Token: 0x040019A0 RID: 6560
		public float Falloff;

		// Token: 0x040019A1 RID: 6561
		public float ConeFalloff;
	}
}
