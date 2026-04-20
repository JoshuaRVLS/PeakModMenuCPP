using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

// Token: 0x02000138 RID: 312
[ExecuteAlways]
public class LocalizedText : MonoBehaviour
{
	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000A0C RID: 2572 RVA: 0x00035C10 File Offset: 0x00033E10
	public static Dictionary<string, List<string>> mainTable
	{
		get
		{
			if (LocalizedText.MAIN_TABLE == null)
			{
				LocalizedText.LoadMainTable(true);
			}
			return LocalizedText.MAIN_TABLE;
		}
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x00035C24 File Offset: 0x00033E24
	public static void TryInitTables()
	{
		if (LocalizedText.MAIN_TABLE == null)
		{
			LocalizedText.LoadMainTable(true);
		}
		if (LocalizedText.DIALOGUE_TABLE == null)
		{
			LocalizedText.InitDialogueTable(false);
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000A0E RID: 2574 RVA: 0x00035C40 File Offset: 0x00033E40
	public static Dictionary<string, List<string>> dialogueTable
	{
		get
		{
			if (LocalizedText.DIALOGUE_TABLE == null)
			{
				LocalizedText.InitDialogueTable(false);
			}
			return LocalizedText.DIALOGUE_TABLE;
		}
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00035C54 File Offset: 0x00033E54
	private void OnEnable()
	{
		if (string.IsNullOrEmpty(this.index))
		{
			this.index = this.row.ToString();
		}
		this.TryFindTextAsset();
		if (Application.isPlaying)
		{
			this.InitDisplayType();
		}
		this.RefreshText();
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x00035C8D File Offset: 0x00033E8D
	public void DebugReload()
	{
		LocalizedText.LoadMainTable(true);
		this.OnEnable();
		LocalizedText.RefreshAllText();
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x00035CA0 File Offset: 0x00033EA0
	public static void ReloadAll()
	{
		LocalizedText.LoadMainTable(true);
		LocalizedText.RefreshAllText();
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x00035CAD File Offset: 0x00033EAD
	private void InitDisplayType()
	{
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x00035CAF File Offset: 0x00033EAF
	[ContextMenu("Debug Serialization")]
	private void DebugSerialization()
	{
		LocalizedText.SerializeMainTable();
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x00035CB8 File Offset: 0x00033EB8
	private static string SerializeMainTable()
	{
		string text = JsonConvert.SerializeObject(LocalizedText.mainTable);
		File.WriteAllText("Assets/Resources/Localization/SerializedTermsData.txt", text);
		return text;
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x00035CDC File Offset: 0x00033EDC
	private static string SerializeDialogueTable()
	{
		string text = JsonConvert.SerializeObject(LocalizedText.dialogueTable);
		File.WriteAllText("Assets/Resources/Localization/SerializedDialogueData.txt", text);
		return text;
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00035D00 File Offset: 0x00033F00
	public static void LoadMainTable(bool forceSerialization = true)
	{
		if (Application.isEditor && forceSerialization)
		{
			LocalizedText.MAIN_TABLE = CSVReader.SplitCsvDict((Resources.Load("Localization/Localized_Text") as TextAsset).text, 0, false);
			TextAsset textAsset = Resources.Load("Localization/Unlocalized_Text") as TextAsset;
			LocalizedText.MAIN_TABLE = LocalizedText.MAIN_TABLE.Concat(CSVReader.SplitCsvDict(textAsset.text, 0, false)).ToDictionary((KeyValuePair<string, List<string>> x) => x.Key, (KeyValuePair<string, List<string>> x) => x.Value);
			LocalizedText.SerializeMainTable();
		}
		else
		{
			LocalizedText.MAIN_TABLE = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>((Resources.Load("Localization/SerializedTermsData") as TextAsset).text);
		}
		if (!Application.isPlaying)
		{
			using (Dictionary<string, List<string>>.KeyCollection.Enumerator enumerator = LocalizedText.MAIN_TABLE.Keys.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string key = enumerator.Current;
					LocalizedText.lineLength = LocalizedText.MAIN_TABLE[key].Count;
				}
			}
		}
		LocalizedText.InitPlatformSpecificTables();
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x00035E34 File Offset: 0x00034034
	public static void InitDialogueTable(bool forceSerialization = false)
	{
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x00035E38 File Offset: 0x00034038
	private static void InitPlatformSpecificTables()
	{
		LocalizedText.TABLE_PC = new Dictionary<string, List<string>>();
		LocalizedText.TABLE_XB = new Dictionary<string, List<string>>();
		LocalizedText.TABLE_SW = new Dictionary<string, List<string>>();
		LocalizedText.TABLE_PS = new Dictionary<string, List<string>>();
		List<string> list = new List<string>();
		foreach (string text in LocalizedText.MAIN_TABLE.Keys)
		{
			if (LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_PC, text, LocalizedText.MAIN_TABLE[text], "_PC") || LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_XB, text, LocalizedText.MAIN_TABLE[text], "_XB") || LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_SW, text, LocalizedText.MAIN_TABLE[text], "_SW") || LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_PS, text, LocalizedText.MAIN_TABLE[text], "_PS"))
			{
				list.Add(text);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			LocalizedText.MAIN_TABLE.Remove(list[i]);
		}
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00035F5C File Offset: 0x0003415C
	private static bool TryInsertIntoTable(Dictionary<string, List<string>> table, string i, List<string> contents, string refToRemove)
	{
		if (i.EndsWith(refToRemove))
		{
			string text = i.Substring(0, i.LastIndexOf(refToRemove));
			table.Add(text.ToUpperInvariant(), contents);
			return true;
		}
		return false;
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x00035F93 File Offset: 0x00034193
	private void TryFindTextAsset()
	{
		this.tmp = base.GetComponent<TMP_Text>();
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x00035FA4 File Offset: 0x000341A4
	public void RefreshText()
	{
		if (this.useDebugLanguage && !Application.isPlaying)
		{
			LocalizedText.CURRENT_LANGUAGE = this.debugLanguage;
		}
		if (!this.tmp)
		{
			this.TryFindTextAsset();
		}
		if (this.autoSet)
		{
			this.currentText = this.GetText();
			this.currentText += this.addendum;
			if (this.tmp)
			{
				this.tmp.text = this.currentText;
				if (!Application.isPlaying && this.tripleIt)
				{
					this.tmp.text = this.tmp.text + this.tmp.text + this.tmp.text;
				}
			}
		}
		this.UpdateSpriteAsset();
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x00036070 File Offset: 0x00034270
	private void UpdateSpriteAsset()
	{
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x00036072 File Offset: 0x00034272
	private static string FailsafeParsing(string s)
	{
		s = s.Replace("\"\"", "\"");
		LocalizedText.Language current_LANGUAGE = LocalizedText.CURRENT_LANGUAGE;
		return s;
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x00036090 File Offset: 0x00034290
	private static string Frenchify(string s)
	{
		for (int i = 0; i < s.Length; i++)
		{
			if (LocalizedText.unbreakableSpaceRequiredChars.Contains(s[i]))
			{
				if (i == 0)
				{
					s = s.Insert(i, '\u00a0'.ToString());
				}
				else
				{
					char c = s[i - 1];
					if (c == ' ')
					{
						s = s.Remove(i - 1, 1).Insert(i - 1, '\u00a0'.ToString());
					}
					else if (c != '\u00a0')
					{
						s = s.Insert(i - 1, '\u00a0'.ToString());
					}
				}
			}
		}
		return s;
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x00036135 File Offset: 0x00034335
	private static string ReplaceCustomValues(string s)
	{
		return s;
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x06000A20 RID: 2592 RVA: 0x00036138 File Offset: 0x00034338
	public static bool languageSupportsAllCaps
	{
		get
		{
			return LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Russian && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Ukrainian && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.SimplifiedChinese && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.TraditionalChinese && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Japanese && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Korean;
		}
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x00036174 File Offset: 0x00034374
	public string GetText()
	{
		string text = LocalizedText.GetText(this.index, true);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return "";
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x0003619D File Offset: 0x0003439D
	public void SetText(string text)
	{
		if (!this.tmp)
		{
			this.TryFindTextAsset();
		}
		if (this.tmp)
		{
			this.tmp.text = text;
		}
		this.index = "";
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x000361D6 File Offset: 0x000343D6
	public void SetTextLocalized(string id)
	{
		this.SetText(LocalizedText.GetText(id, true));
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x000361E5 File Offset: 0x000343E5
	public void SetIndex(string index)
	{
		if (!this.index.Equals(index))
		{
			this.index = index;
			this.RefreshText();
		}
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x00036202 File Offset: 0x00034402
	private static string GetText(int intid)
	{
		return LocalizedText.GetText(intid.ToString(), true);
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x00036214 File Offset: 0x00034414
	public static string GetText(string id, bool printDebug = true)
	{
		List<string> list = null;
		id = id.ToUpperInvariant();
		string result;
		try
		{
			List<string> list2;
			if (LocalizedText.mainTable.TryGetValue(id, out list2))
			{
				list = list2;
			}
			if (list != null)
			{
				string text = list[(int)LocalizedText.CURRENT_LANGUAGE];
				text = LocalizedText.FailsafeParsing(text);
				if (text.IsNullOrEmpty())
				{
					text = list[0];
					text = LocalizedText.FailsafeParsing(text);
					text = LocalizedText.ReplaceCustomValues(text);
				}
				result = text;
			}
			else if (printDebug)
			{
				result = "LOC: " + id;
			}
			else
			{
				Debug.LogError("Failed to load text: " + id);
				result = "";
			}
		}
		catch (Exception ex)
		{
			string str = "Failed to load text: ";
			string str2 = id;
			string str3 = "\n";
			Exception ex2 = ex;
			Debug.LogError(str + str2 + str3 + ((ex2 != null) ? ex2.ToString() : null));
			result = "";
		}
		return result;
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x000362DC File Offset: 0x000344DC
	public static string GetText(string id, LocalizedText.Language language)
	{
		string result;
		try
		{
			result = LocalizedText.mainTable[id.ToUpperInvariant()][(int)language];
		}
		catch (Exception)
		{
			result = "";
		}
		return result;
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x0003631C File Offset: 0x0003451C
	public static string GetText(string id, TextMeshProUGUI text)
	{
		if (text != null && text.GetComponent<LocalizedText>() == null)
		{
			text.gameObject.AddComponent<LocalizedText>().autoSet = false;
		}
		return LocalizedText.GetText(id, true);
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0003634D File Offset: 0x0003454D
	public static string GetText(string id, TextMeshPro text)
	{
		if (text != null && text.GetComponent<LocalizedText>() == null)
		{
			text.gameObject.AddComponent<LocalizedText>().autoSet = false;
		}
		return LocalizedText.GetText(id, true);
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0003637E File Offset: 0x0003457E
	public static string GetText(string id, Text text)
	{
		if (text != null && text.GetComponent<LocalizedText>() == null)
		{
			text.gameObject.AddComponent<LocalizedText>().autoSet = false;
		}
		return LocalizedText.GetText(id, true);
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x000363B0 File Offset: 0x000345B0
	public static LocalizedText.Language GetSystemLanguage()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		if (systemLanguage <= SystemLanguage.French)
		{
			if (systemLanguage == SystemLanguage.English)
			{
				return LocalizedText.Language.English;
			}
			if (systemLanguage == SystemLanguage.French)
			{
				return LocalizedText.Language.French;
			}
		}
		else
		{
			if (systemLanguage == SystemLanguage.German)
			{
				return LocalizedText.Language.German;
			}
			switch (systemLanguage)
			{
			case SystemLanguage.Italian:
				return LocalizedText.Language.Italian;
			case SystemLanguage.Japanese:
				return LocalizedText.Language.Japanese;
			case SystemLanguage.Korean:
				return LocalizedText.Language.Korean;
			case SystemLanguage.Polish:
				return LocalizedText.Language.Polish;
			case SystemLanguage.Portuguese:
				return LocalizedText.Language.BRPortuguese;
			case SystemLanguage.Russian:
				return LocalizedText.Language.Russian;
			case SystemLanguage.Spanish:
				return LocalizedText.Language.SpanishSpain;
			case SystemLanguage.Turkish:
				return LocalizedText.Language.Turkish;
			case SystemLanguage.Ukrainian:
				return LocalizedText.Language.Ukrainian;
			case SystemLanguage.ChineseSimplified:
				return LocalizedText.Language.SimplifiedChinese;
			case SystemLanguage.ChineseTraditional:
				return LocalizedText.Language.SimplifiedChinese;
			}
		}
		return LocalizedText.Language.English;
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x0003645E File Offset: 0x0003465E
	public static void SetLanguageToSystemLanguage()
	{
		LocalizedText.CURRENT_LANGUAGE = LocalizedText.GetSystemLanguage();
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x0003646A File Offset: 0x0003466A
	public static void SetLanguage(int languageInt)
	{
		Debug.Log("Setting language to" + languageInt.ToString());
		if (languageInt == -1)
		{
			LocalizedText.SetLanguageToSystemLanguage();
		}
		else
		{
			LocalizedText.CURRENT_LANGUAGE = (LocalizedText.Language)languageInt;
		}
		LocalizedText.RefreshAllText();
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00036498 File Offset: 0x00034698
	public static void RefreshAllText()
	{
		LocalizedText[] array = Object.FindObjectsByType<LocalizedText>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RefreshText();
		}
		Action onLangugageChanged = LocalizedText.OnLangugageChanged;
		if (onLangugageChanged == null)
		{
			return;
		}
		onLangugageChanged();
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x000364D2 File Offset: 0x000346D2
	public static void AppendCSVLine(string line, string basicPath, string fullPath)
	{
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x000364D4 File Offset: 0x000346D4
	public static string GetNameIndex(string displayName)
	{
		return "NAME_" + displayName;
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x000364E1 File Offset: 0x000346E1
	public static string GetDescriptionIndex(string displayName)
	{
		return "DESC_" + displayName;
	}

	// Token: 0x04000965 RID: 2405
	private static Dictionary<string, List<string>> MAIN_TABLE;

	// Token: 0x04000966 RID: 2406
	private static Dictionary<string, List<string>> TABLE_PC = new Dictionary<string, List<string>>();

	// Token: 0x04000967 RID: 2407
	private static Dictionary<string, List<string>> TABLE_SW = new Dictionary<string, List<string>>();

	// Token: 0x04000968 RID: 2408
	private static Dictionary<string, List<string>> TABLE_XB = new Dictionary<string, List<string>>();

	// Token: 0x04000969 RID: 2409
	private static Dictionary<string, List<string>> TABLE_PS = new Dictionary<string, List<string>>();

	// Token: 0x0400096A RID: 2410
	private static Dictionary<string, List<string>> DIALOGUE_TABLE;

	// Token: 0x0400096B RID: 2411
	public static Action OnLangugageChanged;

	// Token: 0x0400096C RID: 2412
	public const string MAIN_PATH = "Localization/Localized_Text";

	// Token: 0x0400096D RID: 2413
	public const string FULL_PATH = "Assets/Resources/Localization/Localized_Text.csv";

	// Token: 0x0400096E RID: 2414
	public const string UNLOCALIZED_PATH = "Localization/Unlocalized_Text";

	// Token: 0x0400096F RID: 2415
	public const string FULL_PATH_UNLOCALIZED = "Assets/Resources/Localization/Unlocalized_Text.csv";

	// Token: 0x04000970 RID: 2416
	public const string SERIALIZED_TERMS_PATH = "Localization/SerializedTermsData";

	// Token: 0x04000971 RID: 2417
	public const string SERIALIZED_DIALOGUE_PATH = "Localization/SerializedDialogueData";

	// Token: 0x04000972 RID: 2418
	public const string SERIALIZED_TERMS_PATH_FULL = "Assets/Resources/Localization/SerializedTermsData.txt";

	// Token: 0x04000973 RID: 2419
	public const string SERIALIZED_DIALOGUE_PATH_FULL = "Assets/Resources/Localization/SerializedDialogueData.txt";

	// Token: 0x04000974 RID: 2420
	public const int LANGUAGE_COUNT = 14;

	// Token: 0x04000975 RID: 2421
	public static LocalizedText.Language CURRENT_LANGUAGE = LocalizedText.Language.English;

	// Token: 0x04000976 RID: 2422
	public string index;

	// Token: 0x04000977 RID: 2423
	public TMP_Text tmp;

	// Token: 0x04000978 RID: 2424
	public bool autoSet = true;

	// Token: 0x04000979 RID: 2425
	private int row;

	// Token: 0x0400097A RID: 2426
	[SerializeField]
	private string currentText;

	// Token: 0x0400097B RID: 2427
	public bool useDebugLanguage;

	// Token: 0x0400097C RID: 2428
	public LocalizedText.Language debugLanguage;

	// Token: 0x0400097D RID: 2429
	public bool tripleIt;

	// Token: 0x0400097E RID: 2430
	private const string defaultHeaderName = "Muli";

	// Token: 0x0400097F RID: 2431
	private static int lineLength;

	// Token: 0x04000980 RID: 2432
	public string addendum;

	// Token: 0x04000981 RID: 2433
	public LocalizedText.FontStyle fontStyle;

	// Token: 0x04000982 RID: 2434
	public const char UNBREAKABLE_SPACE = '\u00a0';

	// Token: 0x04000983 RID: 2435
	private static List<char> unbreakableSpaceRequiredChars = new List<char>
	{
		'?',
		'!',
		':',
		';',
		'"',
		'%'
	};

	// Token: 0x02000480 RID: 1152
	public enum Language
	{
		// Token: 0x040019C3 RID: 6595
		English,
		// Token: 0x040019C4 RID: 6596
		French,
		// Token: 0x040019C5 RID: 6597
		Italian,
		// Token: 0x040019C6 RID: 6598
		German,
		// Token: 0x040019C7 RID: 6599
		SpanishSpain,
		// Token: 0x040019C8 RID: 6600
		SpanishLatam,
		// Token: 0x040019C9 RID: 6601
		BRPortuguese,
		// Token: 0x040019CA RID: 6602
		Russian,
		// Token: 0x040019CB RID: 6603
		Ukrainian,
		// Token: 0x040019CC RID: 6604
		SimplifiedChinese,
		// Token: 0x040019CD RID: 6605
		TraditionalChinese,
		// Token: 0x040019CE RID: 6606
		Japanese,
		// Token: 0x040019CF RID: 6607
		Korean,
		// Token: 0x040019D0 RID: 6608
		Polish,
		// Token: 0x040019D1 RID: 6609
		Turkish
	}

	// Token: 0x02000481 RID: 1153
	public enum FontStyle
	{
		// Token: 0x040019D3 RID: 6611
		Normal,
		// Token: 0x040019D4 RID: 6612
		Shadow,
		// Token: 0x040019D5 RID: 6613
		Fuzzy,
		// Token: 0x040019D6 RID: 6614
		Outline,
		// Token: 0x040019D7 RID: 6615
		Custom
	}
}
