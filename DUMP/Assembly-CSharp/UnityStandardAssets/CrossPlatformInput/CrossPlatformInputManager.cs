using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput.PlatformSpecific;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200038A RID: 906
	public static class CrossPlatformInputManager
	{
		// Token: 0x060017A7 RID: 6055 RVA: 0x000798FD File Offset: 0x00077AFD
		static CrossPlatformInputManager()
		{
			CrossPlatformInputManager.activeInput = CrossPlatformInputManager.s_HardwareInput;
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x0007991D File Offset: 0x00077B1D
		public static void SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod activeInputMethod)
		{
			if (activeInputMethod == CrossPlatformInputManager.ActiveInputMethod.Hardware)
			{
				CrossPlatformInputManager.activeInput = CrossPlatformInputManager.s_HardwareInput;
				return;
			}
			if (activeInputMethod != CrossPlatformInputManager.ActiveInputMethod.Touch)
			{
				return;
			}
			CrossPlatformInputManager.activeInput = CrossPlatformInputManager.s_TouchInput;
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x0007993C File Offset: 0x00077B3C
		public static bool AxisExists(string name)
		{
			return CrossPlatformInputManager.activeInput.AxisExists(name);
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x00079949 File Offset: 0x00077B49
		public static bool ButtonExists(string name)
		{
			return CrossPlatformInputManager.activeInput.ButtonExists(name);
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x00079956 File Offset: 0x00077B56
		public static void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
		{
			CrossPlatformInputManager.activeInput.RegisterVirtualAxis(axis);
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x00079963 File Offset: 0x00077B63
		public static void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
		{
			CrossPlatformInputManager.activeInput.RegisterVirtualButton(button);
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x00079970 File Offset: 0x00077B70
		public static void UnRegisterVirtualAxis(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			CrossPlatformInputManager.activeInput.UnRegisterVirtualAxis(name);
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x0007998B File Offset: 0x00077B8B
		public static void UnRegisterVirtualButton(string name)
		{
			CrossPlatformInputManager.activeInput.UnRegisterVirtualButton(name);
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x00079998 File Offset: 0x00077B98
		public static CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
		{
			return CrossPlatformInputManager.activeInput.VirtualAxisReference(name);
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x000799A5 File Offset: 0x00077BA5
		public static float GetAxis(string name)
		{
			return CrossPlatformInputManager.GetAxis(name, false);
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x000799AE File Offset: 0x00077BAE
		public static float GetAxisRaw(string name)
		{
			return CrossPlatformInputManager.GetAxis(name, true);
		}

		// Token: 0x060017B2 RID: 6066 RVA: 0x000799B7 File Offset: 0x00077BB7
		private static float GetAxis(string name, bool raw)
		{
			return CrossPlatformInputManager.activeInput.GetAxis(name, raw);
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x000799C5 File Offset: 0x00077BC5
		public static bool GetButton(string name)
		{
			return CrossPlatformInputManager.activeInput.GetButton(name);
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x000799D2 File Offset: 0x00077BD2
		public static bool GetButtonDown(string name)
		{
			return CrossPlatformInputManager.activeInput.GetButtonDown(name);
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x000799DF File Offset: 0x00077BDF
		public static bool GetButtonUp(string name)
		{
			return CrossPlatformInputManager.activeInput.GetButtonUp(name);
		}

		// Token: 0x060017B6 RID: 6070 RVA: 0x000799EC File Offset: 0x00077BEC
		public static void SetButtonDown(string name)
		{
			CrossPlatformInputManager.activeInput.SetButtonDown(name);
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x000799F9 File Offset: 0x00077BF9
		public static void SetButtonUp(string name)
		{
			CrossPlatformInputManager.activeInput.SetButtonUp(name);
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x00079A06 File Offset: 0x00077C06
		public static void SetAxisPositive(string name)
		{
			CrossPlatformInputManager.activeInput.SetAxisPositive(name);
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x00079A13 File Offset: 0x00077C13
		public static void SetAxisNegative(string name)
		{
			CrossPlatformInputManager.activeInput.SetAxisNegative(name);
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x00079A20 File Offset: 0x00077C20
		public static void SetAxisZero(string name)
		{
			CrossPlatformInputManager.activeInput.SetAxisZero(name);
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x00079A2D File Offset: 0x00077C2D
		public static void SetAxis(string name, float value)
		{
			CrossPlatformInputManager.activeInput.SetAxis(name, value);
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060017BC RID: 6076 RVA: 0x00079A3B File Offset: 0x00077C3B
		public static Vector3 mousePosition
		{
			get
			{
				return CrossPlatformInputManager.activeInput.MousePosition();
			}
		}

		// Token: 0x060017BD RID: 6077 RVA: 0x00079A47 File Offset: 0x00077C47
		public static void SetVirtualMousePositionX(float f)
		{
			CrossPlatformInputManager.activeInput.SetVirtualMousePositionX(f);
		}

		// Token: 0x060017BE RID: 6078 RVA: 0x00079A54 File Offset: 0x00077C54
		public static void SetVirtualMousePositionY(float f)
		{
			CrossPlatformInputManager.activeInput.SetVirtualMousePositionY(f);
		}

		// Token: 0x060017BF RID: 6079 RVA: 0x00079A61 File Offset: 0x00077C61
		public static void SetVirtualMousePositionZ(float f)
		{
			CrossPlatformInputManager.activeInput.SetVirtualMousePositionZ(f);
		}

		// Token: 0x04001606 RID: 5638
		private static VirtualInput activeInput;

		// Token: 0x04001607 RID: 5639
		private static VirtualInput s_TouchInput = new MobileInput();

		// Token: 0x04001608 RID: 5640
		private static VirtualInput s_HardwareInput = new StandaloneInput();

		// Token: 0x02000552 RID: 1362
		public enum ActiveInputMethod
		{
			// Token: 0x04001D11 RID: 7441
			Hardware,
			// Token: 0x04001D12 RID: 7442
			Touch
		}

		// Token: 0x02000553 RID: 1363
		public class VirtualAxis
		{
			// Token: 0x170002C0 RID: 704
			// (get) Token: 0x06001F8B RID: 8075 RVA: 0x0008FE46 File Offset: 0x0008E046
			// (set) Token: 0x06001F8C RID: 8076 RVA: 0x0008FE4E File Offset: 0x0008E04E
			public string name { get; private set; }

			// Token: 0x170002C1 RID: 705
			// (get) Token: 0x06001F8D RID: 8077 RVA: 0x0008FE57 File Offset: 0x0008E057
			// (set) Token: 0x06001F8E RID: 8078 RVA: 0x0008FE5F File Offset: 0x0008E05F
			public bool matchWithInputManager { get; private set; }

			// Token: 0x06001F8F RID: 8079 RVA: 0x0008FE68 File Offset: 0x0008E068
			public VirtualAxis(string name) : this(name, true)
			{
			}

			// Token: 0x06001F90 RID: 8080 RVA: 0x0008FE72 File Offset: 0x0008E072
			public VirtualAxis(string name, bool matchToInputSettings)
			{
				this.name = name;
				this.matchWithInputManager = matchToInputSettings;
			}

			// Token: 0x06001F91 RID: 8081 RVA: 0x0008FE88 File Offset: 0x0008E088
			public void Remove()
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(this.name);
			}

			// Token: 0x06001F92 RID: 8082 RVA: 0x0008FE95 File Offset: 0x0008E095
			public void Update(float value)
			{
				this.m_Value = value;
			}

			// Token: 0x170002C2 RID: 706
			// (get) Token: 0x06001F93 RID: 8083 RVA: 0x0008FE9E File Offset: 0x0008E09E
			public float GetValue
			{
				get
				{
					return this.m_Value;
				}
			}

			// Token: 0x170002C3 RID: 707
			// (get) Token: 0x06001F94 RID: 8084 RVA: 0x0008FEA6 File Offset: 0x0008E0A6
			public float GetValueRaw
			{
				get
				{
					return this.m_Value;
				}
			}

			// Token: 0x04001D14 RID: 7444
			private float m_Value;
		}

		// Token: 0x02000554 RID: 1364
		public class VirtualButton
		{
			// Token: 0x170002C4 RID: 708
			// (get) Token: 0x06001F95 RID: 8085 RVA: 0x0008FEAE File Offset: 0x0008E0AE
			// (set) Token: 0x06001F96 RID: 8086 RVA: 0x0008FEB6 File Offset: 0x0008E0B6
			public string name { get; private set; }

			// Token: 0x170002C5 RID: 709
			// (get) Token: 0x06001F97 RID: 8087 RVA: 0x0008FEBF File Offset: 0x0008E0BF
			// (set) Token: 0x06001F98 RID: 8088 RVA: 0x0008FEC7 File Offset: 0x0008E0C7
			public bool matchWithInputManager { get; private set; }

			// Token: 0x06001F99 RID: 8089 RVA: 0x0008FED0 File Offset: 0x0008E0D0
			public VirtualButton(string name) : this(name, true)
			{
			}

			// Token: 0x06001F9A RID: 8090 RVA: 0x0008FEDA File Offset: 0x0008E0DA
			public VirtualButton(string name, bool matchToInputSettings)
			{
				this.name = name;
				this.matchWithInputManager = matchToInputSettings;
			}

			// Token: 0x06001F9B RID: 8091 RVA: 0x0008FF00 File Offset: 0x0008E100
			public void Pressed()
			{
				if (this.m_Pressed)
				{
					return;
				}
				this.m_Pressed = true;
				this.m_LastPressedFrame = Time.frameCount;
			}

			// Token: 0x06001F9C RID: 8092 RVA: 0x0008FF1D File Offset: 0x0008E11D
			public void Released()
			{
				this.m_Pressed = false;
				this.m_ReleasedFrame = Time.frameCount;
			}

			// Token: 0x06001F9D RID: 8093 RVA: 0x0008FF31 File Offset: 0x0008E131
			public void Remove()
			{
				CrossPlatformInputManager.UnRegisterVirtualButton(this.name);
			}

			// Token: 0x170002C6 RID: 710
			// (get) Token: 0x06001F9E RID: 8094 RVA: 0x0008FF3E File Offset: 0x0008E13E
			public bool GetButton
			{
				get
				{
					return this.m_Pressed;
				}
			}

			// Token: 0x170002C7 RID: 711
			// (get) Token: 0x06001F9F RID: 8095 RVA: 0x0008FF46 File Offset: 0x0008E146
			public bool GetButtonDown
			{
				get
				{
					return this.m_LastPressedFrame - Time.frameCount == -1;
				}
			}

			// Token: 0x170002C8 RID: 712
			// (get) Token: 0x06001FA0 RID: 8096 RVA: 0x0008FF57 File Offset: 0x0008E157
			public bool GetButtonUp
			{
				get
				{
					return this.m_ReleasedFrame == Time.frameCount - 1;
				}
			}

			// Token: 0x04001D18 RID: 7448
			private int m_LastPressedFrame = -5;

			// Token: 0x04001D19 RID: 7449
			private int m_ReleasedFrame = -5;

			// Token: 0x04001D1A RID: 7450
			private bool m_Pressed;
		}
	}
}
