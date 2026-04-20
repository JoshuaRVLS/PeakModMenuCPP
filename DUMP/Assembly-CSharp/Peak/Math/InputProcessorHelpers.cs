using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Peak.Math
{
	// Token: 0x020003E4 RID: 996
	internal static class InputProcessorHelpers
	{
		// Token: 0x06001A87 RID: 6791 RVA: 0x00083142 File Offset: 0x00081342
		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			InputSystem.RegisterProcessor<FramerateIndependentProcessor>(null);
			InputSystem.RegisterProcessor<Clamp01Processor>(null);
		}
	}
}
