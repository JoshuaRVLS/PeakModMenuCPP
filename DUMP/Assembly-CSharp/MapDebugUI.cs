using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x0200013B RID: 315
[ConsoleClassCustomizer("MapHandler")]
public class MapDebugUI : MonoBehaviour
{
	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000A3D RID: 2621 RVA: 0x00036818 File Offset: 0x00034A18
	private static bool UiIsVisible
	{
		get
		{
			return GUIManager.instance == null || GUIManager.instance.hudCanvas.isActiveAndEnabled;
		}
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x06000A3E RID: 2622 RVA: 0x00036838 File Offset: 0x00034A38
	private static bool CanvasIsVisible
	{
		get
		{
			return MapDebugUI.canvasEnabled && MapDebugUI.UiIsVisible;
		}
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x00036848 File Offset: 0x00034A48
	private void Start()
	{
		if (!Application.isEditor && !Debug.isDebugBuild)
		{
			Object.Destroy(this.canvasToDestroy);
			return;
		}
		this._sb = new StringBuilder();
		SceneManager.sceneLoaded += this.OnSceneLoad;
		base.StartCoroutine(this.StartInitialization());
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x00036898 File Offset: 0x00034A98
	private void OnSceneLoad(Scene _, LoadSceneMode __)
	{
		this._ambience = Object.FindFirstObjectByType<AmbienceAudio>();
		this._guiManager = Object.FindFirstObjectByType<GUIManager>().GetComponent<AudioSource>();
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x000368B5 File Offset: 0x00034AB5
	[ConsoleCommand]
	public static void IncrementLevel()
	{
		NextLevelService.debugLevelIndexOffset++;
		MapDebugUI.forceInitialization = true;
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x000368C9 File Offset: 0x00034AC9
	[ConsoleCommand]
	public static void ToggleDebugText()
	{
		MapDebugUI.canvasEnabled = !MapDebugUI.canvasEnabled;
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x000368D8 File Offset: 0x00034AD8
	private IEnumerator StartInitialization()
	{
		this.initialized = false;
		MapDebugUI.forceInitialization = false;
		this.audioInitialized = true;
		int loopstop = 10;
		NextLevelService service;
		for (;;)
		{
			int num = loopstop;
			loopstop = num - 1;
			if (num <= 0)
			{
				goto IL_E0;
			}
			this._ambience = Object.FindFirstObjectByType<AmbienceAudio>();
			GUIManager guimanager = Object.FindFirstObjectByType<GUIManager>();
			this._guiManager = ((guimanager != null) ? guimanager.GetComponent<AudioSource>() : null);
			service = GameHandler.GetService<NextLevelService>();
			if (service.Data.IsSome)
			{
				break;
			}
			yield return new WaitForSeconds(1f);
		}
		this.scene = SingletonAsset<MapBaker>.Instance.GetLevel(service.Data.Value.CurrentLevelIndex + NextLevelService.debugLevelIndexOffset);
		Debug.Log("Initialized.");
		IL_E0:
		this.initialized = true;
		yield break;
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x000368E8 File Offset: 0x00034AE8
	private void UpdateRunId()
	{
		if (RunManager.Instance && MapDebugUI.CanvasIsVisible)
		{
			this.runID.text = RunManager.Instance.RunId.ToString();
			return;
		}
		this.runID.text = "";
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x0003693C File Offset: 0x00034B3C
	private void Update()
	{
		this.UpdateRunId();
		if (MapDebugUI.forceInitialization)
		{
			base.StartCoroutine(this.StartInitialization());
			return;
		}
		if (!this.initialized)
		{
			return;
		}
		if (!this._foundAmbience && MapHandler.ExistsAndInitialized)
		{
			this._ambience = Object.FindFirstObjectByType<AmbienceAudio>();
			this._foundAmbience = this._ambience;
		}
		this.text.enabled = MapDebugUI.CanvasIsVisible;
		if (Character.localCharacter && MainCamera.instance)
		{
			this.position = MainCamera.instance.transform.position.ToString();
		}
		else
		{
			this.position = "";
		}
		BuildVersion buildVersion = new BuildVersion(Application.version, "???");
		string text = buildVersion.ToString();
		string text2 = this.scene;
		this._sb.Clear();
		this._sb.AppendLine(string.Concat(new string[]
		{
			text,
			"\nMap: ",
			text2,
			"\nCameraPos: ",
			this.position
		}));
		if (this._ambience && this._ambience.isActiveAndEnabled)
		{
			this._sb.AppendLine("MainMusic: " + MapDebugUI.<Update>g__GetAudioString|23_0(this._ambience.mainMusic));
			this._sb.AppendLine("Stinger: " + MapDebugUI.<Update>g__GetAudioString|23_0(this._ambience.stingerSource));
			this._sb.AppendLine("GUIManager: " + MapDebugUI.<Update>g__GetAudioString|23_0(this._guiManager));
		}
		this.text.text = this._sb.ToString();
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x00036B08 File Offset: 0x00034D08
	[CompilerGenerated]
	internal static string <Update>g__GetAudioString|23_0(AudioSource source)
	{
		if (!source.clip)
		{
			return "<color=#d3d3d388>null</color>";
		}
		int num = Mathf.RoundToInt(source.volume * 100f);
		if (source.isPlaying && !source.isVirtual && source.volume > 0.1f)
		{
			return string.Format("<color={0}>{1} [{2}%]</color>", "#ffffff", source.clip.name, num);
		}
		return string.Format("<color={0}>{1} [{2}%]</color>", "#d3d3d388", source.clip.name, num);
	}

	// Token: 0x04000992 RID: 2450
	private bool initialized;

	// Token: 0x04000993 RID: 2451
	private bool audioInitialized;

	// Token: 0x04000994 RID: 2452
	private string scene;

	// Token: 0x04000995 RID: 2453
	public TextMeshProUGUI text;

	// Token: 0x04000996 RID: 2454
	public TextMeshProUGUI runID;

	// Token: 0x04000997 RID: 2455
	public GameObject canvasToDestroy;

	// Token: 0x04000998 RID: 2456
	private bool _foundAmbience;

	// Token: 0x04000999 RID: 2457
	private AmbienceAudio _ambience;

	// Token: 0x0400099A RID: 2458
	private AudioSource _guiManager;

	// Token: 0x0400099B RID: 2459
	private StringBuilder _sb;

	// Token: 0x0400099C RID: 2460
	public static bool canvasEnabled = true;

	// Token: 0x0400099D RID: 2461
	public static bool forceInitialization;

	// Token: 0x0400099E RID: 2462
	private string position;
}
