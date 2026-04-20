using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

// Token: 0x02000078 RID: 120
public class ControllerManager
{
	// Token: 0x0600056B RID: 1387 RVA: 0x000200AB File Offset: 0x0001E2AB
	public void Init()
	{
		InputSystem.onDeviceChange += this.OnDeviceChange;
		this.UpdateGamepadUsage();
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x000200C4 File Offset: 0x0001E2C4
	public void Destroy()
	{
		InputSystem.onDeviceChange -= this.OnDeviceChange;
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x000200D7 File Offset: 0x0001E2D7
	private void OnDeviceChange(InputDevice device, InputDeviceChange change)
	{
		this.UpdateGamepadUsage();
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x000200E0 File Offset: 0x0001E2E0
	private void UpdateGamepadUsage()
	{
		using (ReadOnlyArray<InputDevice>.Enumerator enumerator = InputSystem.devices.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is Gamepad)
				{
					this.gamepadAttached = true;
					return;
				}
			}
		}
		this.gamepadAttached = false;
	}

	// Token: 0x040005A3 RID: 1443
	public bool gamepadAttached;
}
