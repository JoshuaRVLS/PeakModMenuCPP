using System;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Peak
{
	// Token: 0x020003C3 RID: 963
	public static class IntegratedGraphicsHelper
	{
		// Token: 0x17000181 RID: 385
		// (get) Token: 0x0600197B RID: 6523 RVA: 0x00081310 File Offset: 0x0007F510
		public static bool IsUsingIntegratedGraphics
		{
			get
			{
				string graphicsDeviceName = UnityEngine.Device.SystemInfo.graphicsDeviceName;
				return graphicsDeviceName.Contains("Integrated") || (graphicsDeviceName.Contains("Intel") && !graphicsDeviceName.Contains("Arc")) || (graphicsDeviceName.Contains("Radeon") && graphicsDeviceName.Contains("Vega")) || graphicsDeviceName.Contains("Look I know this is dumb but it's the best I can do right now ok? Don't @ me.");
			}
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x00081374 File Offset: 0x0007F574
		public static void DisableGPUCullingIfNecessary()
		{
			bool flag2;
			bool flag = CLI.TryGetArg("gpuCulling", out flag2);
			if (!IntegratedGraphicsHelper.IsUsingIntegratedGraphics && (!flag || flag2))
			{
				return;
			}
			if (IntegratedGraphicsHelper.IsUsingIntegratedGraphics && flag && flag2)
			{
				Debug.Log("Using integrated graphics, but GPU culling forced to remain on with override.");
				return;
			}
			if (IntegratedGraphicsHelper.IsUsingIntegratedGraphics)
			{
				Debug.Log("Integrated graphics detected! (" + UnityEngine.Device.SystemInfo.graphicsDeviceName + "). Disabling GPU culling. ");
			}
			else
			{
				Debug.Log("GPU culling forced off by command-line override!");
			}
			UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (universalRenderPipelineAsset == null)
			{
				Debug.LogError("uhhh... where is our URP asset?");
				return;
			}
			universalRenderPipelineAsset.gpuResidentDrawerEnableOcclusionCullingInCameras = false;
		}
	}
}
