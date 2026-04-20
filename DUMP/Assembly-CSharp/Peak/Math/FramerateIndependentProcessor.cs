using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Peak.Math
{
	// Token: 0x020003E5 RID: 997
	public class FramerateIndependentProcessor : InputProcessor<Vector2>
	{
		// Token: 0x06001A88 RID: 6792 RVA: 0x00083150 File Offset: 0x00081350
		public override Vector2 Process(Vector2 value, InputControl control)
		{
			float num = this.UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
			if (this.DivideInstead)
			{
				num = 1f / num;
			}
			return num * value;
		}

		// Token: 0x04001776 RID: 6006
		public bool UseScaledTime;

		// Token: 0x04001777 RID: 6007
		public bool DivideInstead;
	}
}
