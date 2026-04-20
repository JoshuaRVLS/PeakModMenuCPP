using System;

namespace Peak.Dev
{
	// Token: 0x020003F1 RID: 1009
	public static class Pretty
	{
		// Token: 0x06001A98 RID: 6808 RVA: 0x00083334 File Offset: 0x00081534
		public static string Print(object obj)
		{
			if (obj == null)
			{
				return "NULL";
			}
			IPrettyPrintable prettyPrintable = obj as IPrettyPrintable;
			if (prettyPrintable != null)
			{
				return prettyPrintable.ToPrettyString();
			}
			float[] array = obj as float[];
			if (array != null && array.Length == CharacterAfflictions.NumStatusTypes)
			{
				return CharacterAfflictions.PrettyPrintStaminaBar(array, 40, true);
			}
			return "[UNSUPPORTED PRETTY PRINT]";
		}
	}
}
