using System;
using UnityEngine;

// Token: 0x020002AB RID: 683
public class MirrorCameraScript : MonoBehaviour
{
	// Token: 0x06001359 RID: 4953 RVA: 0x00062038 File Offset: 0x00060238
	private void Start()
	{
		this.mirrorScript = base.GetComponentInParent<MirrorScript>();
		this.cameraObject = base.GetComponent<Camera>();
		if (this.mirrorScript.AddFlareLayer)
		{
			this.cameraObject.gameObject.AddComponent<FlareLayer>();
		}
		this.mirrorRenderer = this.MirrorObject.GetComponent<Renderer>();
		if (Application.isPlaying)
		{
			foreach (Material material in this.mirrorRenderer.sharedMaterials)
			{
				if (material.name == "MirrorMaterial")
				{
					this.mirrorRenderer.sharedMaterial = material;
					break;
				}
			}
		}
		this.mirrorMaterial = this.mirrorRenderer.sharedMaterial;
		this.CreateRenderTexture();
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x000620E8 File Offset: 0x000602E8
	private void CreateRenderTexture()
	{
		if (this.reflectionTexture == null || this.oldReflectionTextureSize != this.mirrorScript.TextureSize)
		{
			if (this.reflectionTexture)
			{
				Object.DestroyImmediate(this.reflectionTexture);
			}
			this.reflectionTexture = new RenderTexture(this.mirrorScript.TextureSize, this.mirrorScript.TextureSize, 16);
			this.reflectionTexture.filterMode = FilterMode.Bilinear;
			this.reflectionTexture.antiAliasing = 1;
			this.reflectionTexture.name = "MirrorRenderTexture_" + base.GetInstanceID().ToString();
			this.reflectionTexture.hideFlags = HideFlags.HideAndDontSave;
			this.reflectionTexture.autoGenerateMips = false;
			this.reflectionTexture.wrapMode = TextureWrapMode.Clamp;
			this.mirrorMaterial.SetTexture("_MainTex", this.reflectionTexture);
			this.oldReflectionTextureSize = this.mirrorScript.TextureSize;
		}
		if (this.cameraObject.targetTexture != this.reflectionTexture)
		{
			this.cameraObject.targetTexture = this.reflectionTexture;
		}
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x00062204 File Offset: 0x00060404
	private void Update()
	{
		if (this.VRMode && Camera.current == Camera.main)
		{
			return;
		}
		this.CreateRenderTexture();
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x00062228 File Offset: 0x00060428
	private void UpdateCameraProperties(Camera src, Camera dest)
	{
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox component = src.GetComponent<Skybox>();
			Skybox component2 = dest.GetComponent<Skybox>();
			if (!component || !component.material)
			{
				component2.enabled = false;
			}
			else
			{
				component2.enabled = true;
				component2.material = component.material;
			}
		}
		dest.orthographic = src.orthographic;
		dest.orthographicSize = src.orthographicSize;
		if (this.mirrorScript.AspectRatio > 0f)
		{
			dest.aspect = this.mirrorScript.AspectRatio;
		}
		else
		{
			dest.aspect = src.aspect;
		}
		dest.renderingPath = src.renderingPath;
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x000622EC File Offset: 0x000604EC
	internal void RenderMirror()
	{
		Camera current;
		if (MirrorCameraScript.renderingMirror || !base.enabled || (current = Camera.current) == null || this.mirrorRenderer == null || this.mirrorMaterial == null || !this.mirrorRenderer.enabled)
		{
			return;
		}
		MirrorCameraScript.renderingMirror = true;
		int pixelLightCount = QualitySettings.pixelLightCount;
		if (QualitySettings.pixelLightCount != this.mirrorScript.MaximumPerPixelLights)
		{
			QualitySettings.pixelLightCount = this.mirrorScript.MaximumPerPixelLights;
		}
		try
		{
			this.UpdateCameraProperties(current, this.cameraObject);
			if (this.mirrorScript.MirrorRecursion)
			{
				this.mirrorMaterial.EnableKeyword("MIRROR_RECURSION");
				this.cameraObject.ResetWorldToCameraMatrix();
				this.cameraObject.ResetProjectionMatrix();
				this.cameraObject.projectionMatrix = this.cameraObject.projectionMatrix * Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
				this.cameraObject.cullingMask = (-17 & this.mirrorScript.ReflectLayers.value);
				GL.invertCulling = true;
				this.cameraObject.Render();
				GL.invertCulling = false;
			}
			else
			{
				this.mirrorMaterial.DisableKeyword("MIRROR_RECURSION");
				Vector3 position = base.transform.position;
				Vector3 vector = this.mirrorScript.NormalIsForward ? base.transform.forward : base.transform.up;
				float w = -Vector3.Dot(vector, position) - this.mirrorScript.ClipPlaneOffset;
				Vector4 vector2 = new Vector4(vector.x, vector.y, vector.z, w);
				this.CalculateReflectionMatrix(ref vector2);
				Vector3 position2 = this.cameraObject.transform.position;
				float farClipPlane = this.cameraObject.farClipPlane;
				Vector3 position3 = this.reflectionMatrix.MultiplyPoint(position2);
				Matrix4x4 matrix4x = current.worldToCameraMatrix;
				if (this.VRMode)
				{
					if (current.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left)
					{
						ref Matrix4x4 ptr = ref matrix4x;
						ptr[12] = ptr[12] + 0.011f;
					}
					else if (current.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
					{
						ref Matrix4x4 ptr = ref matrix4x;
						ptr[12] = ptr[12] - 0.011f;
					}
				}
				matrix4x *= this.reflectionMatrix;
				this.cameraObject.worldToCameraMatrix = matrix4x;
				Vector4 clipPlane = this.CameraSpacePlane(ref matrix4x, ref position, ref vector, 1f);
				this.cameraObject.projectionMatrix = current.CalculateObliqueMatrix(clipPlane);
				GL.invertCulling = true;
				this.cameraObject.transform.position = position3;
				this.cameraObject.farClipPlane = this.mirrorScript.FarClipPlane;
				this.cameraObject.cullingMask = (-17 & this.mirrorScript.ReflectLayers.value);
				this.cameraObject.Render();
				this.cameraObject.transform.position = position2;
				this.cameraObject.farClipPlane = farClipPlane;
				GL.invertCulling = false;
			}
		}
		finally
		{
			MirrorCameraScript.renderingMirror = false;
			if (QualitySettings.pixelLightCount != pixelLightCount)
			{
				QualitySettings.pixelLightCount = pixelLightCount;
			}
		}
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x00062618 File Offset: 0x00060818
	private void OnDisable()
	{
		if (this.reflectionTexture)
		{
			Object.DestroyImmediate(this.reflectionTexture);
			this.reflectionTexture = null;
		}
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x0006263C File Offset: 0x0006083C
	private Vector4 CameraSpacePlane(ref Matrix4x4 worldToCameraMatrix, ref Vector3 pos, ref Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * this.mirrorScript.ClipPlaneOffset;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 vector = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(vector.x, vector.y, vector.z, -Vector3.Dot(lhs, vector));
	}

	// Token: 0x06001360 RID: 4960 RVA: 0x000626B0 File Offset: 0x000608B0
	private void CalculateReflectionMatrix(ref Vector4 plane)
	{
		this.reflectionMatrix.m00 = 1f - 2f * plane[0] * plane[0];
		this.reflectionMatrix.m01 = -2f * plane[0] * plane[1];
		this.reflectionMatrix.m02 = -2f * plane[0] * plane[2];
		this.reflectionMatrix.m03 = -2f * plane[3] * plane[0];
		this.reflectionMatrix.m10 = -2f * plane[1] * plane[0];
		this.reflectionMatrix.m11 = 1f - 2f * plane[1] * plane[1];
		this.reflectionMatrix.m12 = -2f * plane[1] * plane[2];
		this.reflectionMatrix.m13 = -2f * plane[3] * plane[1];
		this.reflectionMatrix.m20 = -2f * plane[2] * plane[0];
		this.reflectionMatrix.m21 = -2f * plane[2] * plane[1];
		this.reflectionMatrix.m22 = 1f - 2f * plane[2] * plane[2];
		this.reflectionMatrix.m23 = -2f * plane[3] * plane[2];
		this.reflectionMatrix.m30 = 0f;
		this.reflectionMatrix.m31 = 0f;
		this.reflectionMatrix.m32 = 0f;
		this.reflectionMatrix.m33 = 1f;
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x00062890 File Offset: 0x00060A90
	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, ref Vector4 clipPlane)
	{
		Vector4 b = projection.inverse * new Vector4(MirrorCameraScript.Sign(clipPlane.x), MirrorCameraScript.Sign(clipPlane.y), 1f, 1f);
		Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
		projection[2] = vector.x - projection[3];
		projection[6] = vector.y - projection[7];
		projection[10] = vector.z - projection[11];
		projection[14] = vector.w - projection[15];
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x00062944 File Offset: 0x00060B44
	private static float Sign(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	// Token: 0x0400119D RID: 4509
	public GameObject MirrorObject;

	// Token: 0x0400119E RID: 4510
	public bool VRMode;

	// Token: 0x0400119F RID: 4511
	private Renderer mirrorRenderer;

	// Token: 0x040011A0 RID: 4512
	private Material mirrorMaterial;

	// Token: 0x040011A1 RID: 4513
	private MirrorScript mirrorScript;

	// Token: 0x040011A2 RID: 4514
	private Camera cameraObject;

	// Token: 0x040011A3 RID: 4515
	private RenderTexture reflectionTexture;

	// Token: 0x040011A4 RID: 4516
	private Matrix4x4 reflectionMatrix;

	// Token: 0x040011A5 RID: 4517
	private int oldReflectionTextureSize;

	// Token: 0x040011A6 RID: 4518
	private static bool renderingMirror;
}
