using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Peak.Math
{
	// Token: 0x020003E6 RID: 998
	public class Clamp01Processor : InputProcessor<Vector2>
	{
		// Token: 0x06001A8A RID: 6794 RVA: 0x00083191 File Offset: 0x00081391
		public override Vector2 Process(Vector2 value, InputControl control)
		{
			return Vector2.ClampMagnitude(value, 1f);
		}
	}
}
