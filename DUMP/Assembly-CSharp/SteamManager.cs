using System;
using System.Text;
using AOT;
using Steamworks;
using UnityEngine;

// Token: 0x020001B8 RID: 440
[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000E19 RID: 3609 RVA: 0x0004703A File Offset: 0x0004523A
	public static SteamManager Instance
	{
		get
		{
			return SteamManager.s_instance;
		}
	}

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000E1A RID: 3610 RVA: 0x00047041 File Offset: 0x00045241
	public static bool Initialized
	{
		get
		{
			return SteamManager.s_EverInitialized && SteamManager.Instance.m_bInitialized;
		}
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x00047056 File Offset: 0x00045256
	[MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	protected static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x0004705E File Offset: 0x0004525E
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void InitOnPlayMode()
	{
		SteamManager.s_EverInitialized = false;
		SteamManager.s_instance = null;
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x0004706C File Offset: 0x0004526C
	protected virtual void Awake()
	{
		if (SteamManager.s_instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		SteamManager.s_instance = this;
		if (SteamManager.s_EverInitialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary(new AppId_t(3527290U)))
			{
				Debug.Log("[Steamworks.NET] Shutting down because RestartAppIfNecessary returned true. Steam will restart the application.");
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException ex)
		{
			string str = "[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n";
			DllNotFoundException ex2 = ex;
			Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null), this);
			Application.Quit();
			return;
		}
		this.m_bInitialized = SteamAPI.Init();
		if (!this.m_bInitialized)
		{
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
			return;
		}
		SteamManager.s_EverInitialized = true;
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x00047158 File Offset: 0x00045358
	protected virtual void OnEnable()
	{
		if (SteamManager.s_instance == null)
		{
			SteamManager.s_instance = this;
		}
		if (!this.m_bInitialized)
		{
			return;
		}
		if (this.m_SteamAPIWarningMessageHook == null)
		{
			this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
		}
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x000471A6 File Offset: 0x000453A6
	protected virtual void OnDestroy()
	{
		if (SteamManager.s_instance != this)
		{
			return;
		}
		SteamManager.s_instance = null;
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.Shutdown();
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x000471CA File Offset: 0x000453CA
	protected virtual void Update()
	{
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.RunCallbacks();
	}

	// Token: 0x04000BE7 RID: 3047
	protected static bool s_EverInitialized;

	// Token: 0x04000BE8 RID: 3048
	protected static SteamManager s_instance;

	// Token: 0x04000BE9 RID: 3049
	protected bool m_bInitialized;

	// Token: 0x04000BEA RID: 3050
	protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
}
