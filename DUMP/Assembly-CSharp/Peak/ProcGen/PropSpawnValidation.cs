using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peak.ProcGen
{
	// Token: 0x020003ED RID: 1005
	public static class PropSpawnValidation
	{
		// Token: 0x06001A93 RID: 6803 RVA: 0x00083234 File Offset: 0x00081434
		public static Color GetValidationColorImpl(this IValidatable self)
		{
			return PropSpawnValidation.ValidationColors[self.ValidationState];
		}

		// Token: 0x04001781 RID: 6017
		public static readonly Dictionary<ValidationState, Color> ValidationColors = new Dictionary<ValidationState, Color>
		{
			{
				ValidationState.Unknown,
				Color.yellow
			},
			{
				ValidationState.Passed,
				Color.green
			},
			{
				ValidationState.Failed,
				Color.red
			}
		};
	}
}
