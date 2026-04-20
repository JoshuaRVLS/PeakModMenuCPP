using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Token: 0x020001B0 RID: 432
public class SingleBufferFeature : ScriptableRendererFeature
{
	// Token: 0x06000DFA RID: 3578 RVA: 0x000465EC File Offset: 0x000447EC
	public override void Create()
	{
		this.m_ScriptablePass = new SingleBufferFeature.CustomRenderPass(this.settings, base.name);
		this.m_ScriptablePass.renderPassEvent = this.settings._event;
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x0004661C File Offset: 0x0004481C
	public unsafe override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		CameraType cameraType = *renderingData.cameraData.cameraType;
		if (cameraType == CameraType.Preview)
		{
			return;
		}
		if (!this.settings.showInSceneView && cameraType == CameraType.SceneView)
		{
			return;
		}
		renderer.EnqueuePass(this.m_ScriptablePass);
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x00046659 File Offset: 0x00044859
	protected override void Dispose(bool disposing)
	{
		this.m_ScriptablePass.Dispose();
	}

	// Token: 0x04000BD0 RID: 3024
	public SingleBufferFeature.Settings settings = new SingleBufferFeature.Settings();

	// Token: 0x04000BD1 RID: 3025
	private SingleBufferFeature.CustomRenderPass m_ScriptablePass;

	// Token: 0x020004C8 RID: 1224
	public class CustomRenderPass : ScriptableRenderPass
	{
		// Token: 0x06001D78 RID: 7544 RVA: 0x0008A1C0 File Offset: 0x000883C0
		public CustomRenderPass(SingleBufferFeature.Settings settings, string name)
		{
			this.settings = settings;
			this.filteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.transparent), settings.layerMask, uint.MaxValue, 0);
			this.shaderTagsList.Add(new ShaderTagId("SRPDefaultUnlit"));
			this.shaderTagsList.Add(new ShaderTagId("UniversalForward"));
			this.shaderTagsList.Add(new ShaderTagId("UniversalForwardOnly"));
			this._profilingSampler = new ProfilingSampler(name);
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x0008A254 File Offset: 0x00088454
		public unsafe override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			RenderTextureDescriptor renderTextureDescriptor = *renderingData.cameraData.cameraTargetDescriptor;
			renderTextureDescriptor.depthBufferBits = 0;
			RenderingUtils.ReAllocateIfNeeded(ref this.rtTempColor, renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Repeat, false, 1, 0f, "_TemporaryColorTexture");
			if (this.settings.colorTargetDestinationID != "")
			{
				RenderingUtils.ReAllocateIfNeeded(ref this.rtCustomColor, renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Repeat, false, 1, 0f, this.settings.colorTargetDestinationID);
			}
			else
			{
				this.rtCustomColor = renderingData.cameraData.renderer->cameraColorTargetHandle;
			}
			RTHandle cameraDepthTargetHandle = renderingData.cameraData.renderer->cameraDepthTargetHandle;
			base.ConfigureTarget(this.rtCustomColor, cameraDepthTargetHandle);
			base.ConfigureClear(ClearFlag.Color, new Color(0f, 0f, 0f, 0f));
		}

		// Token: 0x06001D7A RID: 7546 RVA: 0x0008A328 File Offset: 0x00088528
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer commandBuffer = CommandBufferPool.Get();
			using (new ProfilingScope(commandBuffer, this._profilingSampler))
			{
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
				SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
				DrawingSettings drawingSettings = base.CreateDrawingSettings(this.shaderTagsList, ref renderingData, sortingCriteria);
				if (this.settings.overrideMaterial != null)
				{
					drawingSettings.overrideMaterialPassIndex = this.settings.overrideMaterialPass;
					drawingSettings.overrideMaterial = this.settings.overrideMaterial;
				}
				context.DrawRenderers(*renderingData.cullResults, ref drawingSettings, ref this.filteringSettings);
				if (this.settings.colorTargetDestinationID != "")
				{
					commandBuffer.SetGlobalTexture(this.settings.colorTargetDestinationID, this.rtCustomColor);
				}
				if (this.settings.blitMaterial != null)
				{
					RTHandle cameraColorTargetHandle = renderingData.cameraData.renderer->cameraColorTargetHandle;
					if (cameraColorTargetHandle != null && this.rtTempColor != null)
					{
						Blitter.BlitCameraTexture(commandBuffer, cameraColorTargetHandle, this.rtTempColor, this.settings.blitMaterial, 0);
						Blitter.BlitCameraTexture(commandBuffer, this.rtTempColor, cameraColorTargetHandle, 0f, false);
					}
				}
			}
			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
			CommandBufferPool.Release(commandBuffer);
		}

		// Token: 0x06001D7B RID: 7547 RVA: 0x0008A490 File Offset: 0x00088690
		public override void OnCameraCleanup(CommandBuffer cmd)
		{
		}

		// Token: 0x06001D7C RID: 7548 RVA: 0x0008A492 File Offset: 0x00088692
		public void Dispose()
		{
			if (this.settings.colorTargetDestinationID != "")
			{
				RTHandle rthandle = this.rtCustomColor;
				if (rthandle != null)
				{
					rthandle.Release();
				}
			}
			RTHandle rthandle2 = this.rtTempColor;
			if (rthandle2 == null)
			{
				return;
			}
			rthandle2.Release();
		}

		// Token: 0x04001B0A RID: 6922
		private SingleBufferFeature.Settings settings;

		// Token: 0x04001B0B RID: 6923
		private FilteringSettings filteringSettings;

		// Token: 0x04001B0C RID: 6924
		private ProfilingSampler _profilingSampler;

		// Token: 0x04001B0D RID: 6925
		private List<ShaderTagId> shaderTagsList = new List<ShaderTagId>();

		// Token: 0x04001B0E RID: 6926
		private RTHandle rtCustomColor;

		// Token: 0x04001B0F RID: 6927
		private RTHandle rtTempColor;
	}

	// Token: 0x020004C9 RID: 1225
	[Serializable]
	public class Settings
	{
		// Token: 0x04001B10 RID: 6928
		public bool showInSceneView = true;

		// Token: 0x04001B11 RID: 6929
		public RenderPassEvent _event = RenderPassEvent.AfterRenderingOpaques;

		// Token: 0x04001B12 RID: 6930
		[Header("Draw Renderers Settings")]
		public LayerMask layerMask = 1;

		// Token: 0x04001B13 RID: 6931
		public Material overrideMaterial;

		// Token: 0x04001B14 RID: 6932
		public int overrideMaterialPass;

		// Token: 0x04001B15 RID: 6933
		public string colorTargetDestinationID = "";

		// Token: 0x04001B16 RID: 6934
		[Header("Blit Settings")]
		public Material blitMaterial;
	}
}
