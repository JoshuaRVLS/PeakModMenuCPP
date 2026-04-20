using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003C1 RID: 961
	public static class CLI
	{
		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06001971 RID: 6513 RVA: 0x0008108E File Offset: 0x0007F28E
		private static Dictionary<string, string> ParsedArgs
		{
			get
			{
				if (CLI._cmdLineArgs == null)
				{
					CLI._cmdLineArgs = CLI.ParseCommandLineArgs();
				}
				return CLI._cmdLineArgs;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06001972 RID: 6514 RVA: 0x000810A8 File Offset: 0x0007F2A8
		public static string ForceVersionArg
		{
			get
			{
				string result;
				if (CLI.TryGetArg("forceVersion", out result))
				{
					return result;
				}
				return string.Empty;
			}
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x000810CC File Offset: 0x0007F2CC
		private static Dictionary<string, string> ParseCommandLineArgs()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				string text = commandLineArgs[i];
				if (text.StartsWith('-'))
				{
					string text2 = text;
					text = text2.Substring(1, text2.Length - 1);
					if (i + 1 < commandLineArgs.Length)
					{
						string text3 = commandLineArgs[i + 1];
						if (!text3.StartsWith('-'))
						{
							dictionary[text] = text3;
						}
						else
						{
							dictionary[text] = "";
						}
					}
					else
					{
						dictionary[text] = "";
					}
				}
			}
			return dictionary;
		}

		// Token: 0x06001974 RID: 6516 RVA: 0x00081156 File Offset: 0x0007F356
		public static bool TryGetArg(string arg, out string result)
		{
			return CLI.ParsedArgs.TryGetValue(arg, out result);
		}

		// Token: 0x06001975 RID: 6517 RVA: 0x00081164 File Offset: 0x0007F364
		public static bool TryGetArg(string arg, out bool result)
		{
			result = false;
			string value;
			return CLI.TryGetArg(arg, out value) && bool.TryParse(value, out result);
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x00081188 File Offset: 0x0007F388
		public static bool HasArg(string arg)
		{
			string text;
			bool result = CLI.ParsedArgs.TryGetValue(arg, out text);
			if (text != string.Empty)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Arg ",
					arg,
					" has a value (",
					text,
					") which we're ignoring."
				}));
			}
			return result;
		}

		// Token: 0x04001736 RID: 5942
		private static Dictionary<string, string> _cmdLineArgs;

		// Token: 0x02000566 RID: 1382
		public static class Args
		{
			// Token: 0x04001D3D RID: 7485
			public const string ForceVersion = "forceVersion";

			// Token: 0x04001D3E RID: 7486
			public const string BuildPath = "buildPath";

			// Token: 0x04001D3F RID: 7487
			public const string CleanBuild = "cleanBuild";

			// Token: 0x04001D40 RID: 7488
			public const string GPUCulling = "gpuCulling";
		}
	}
}
