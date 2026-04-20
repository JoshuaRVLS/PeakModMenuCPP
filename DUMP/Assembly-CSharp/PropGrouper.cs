using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Peak.ProcGen;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public class PropGrouper : MonoBehaviour, IValidatable
{
	// Token: 0x0600147E RID: 5246 RVA: 0x00067864 File Offset: 0x00065A64
	public void RunAll(bool updateLightmap = true)
	{
		PropGrouper.<>c__DisplayClass3_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (!this.Verify())
		{
			return;
		}
		this.ClearAll();
		LevelGenStep[] componentsInChildren = base.GetComponentsInChildren<LevelGenStep>();
		List<LevelGenStep> list = new List<LevelGenStep>();
		CS$<>8__locals1.late = new List<LevelGenStep>();
		CS$<>8__locals1.deferredSteps = new Dictionary<DeferredStepTiming, List<IDeferredStep>>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			PropGrouper.PropGrouperTiming propGrouperTiming = componentsInChildren[i].GetComponentInParent<PropGrouper>().timing;
			if (propGrouperTiming == PropGrouper.PropGrouperTiming.Early)
			{
				list.Add(componentsInChildren[i]);
			}
			else if (propGrouperTiming == PropGrouper.PropGrouperTiming.Late)
			{
				CS$<>8__locals1.late.Add(componentsInChildren[i]);
			}
		}
		foreach (LevelGenStep levelGenStep in list)
		{
			levelGenStep.Execute();
			if (levelGenStep.DeferredTiming != DeferredStepTiming.None)
			{
				this.<RunAll>g__AddToStepList|3_2(levelGenStep.DeferredTiming, levelGenStep.ConstructDeferred(levelGenStep), ref CS$<>8__locals1);
			}
		}
		this.<RunAll>g__ExecuteAndClearDeferredStepsFor|3_1(DeferredStepTiming.AfterCurrentGroupTiming, ref CS$<>8__locals1);
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x0006795C File Offset: 0x00065B5C
	public void Validate()
	{
		this.ValidationState = this.DoValidation();
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x0006796A File Offset: 0x00065B6A
	public Color GetValidationColor()
	{
		return this.GetValidationColorImpl();
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x00067974 File Offset: 0x00065B74
	public ValidationState DoValidation()
	{
		ValidationState validationState = ValidationState.Passed;
		foreach (IValidatable validatable in from v in base.GetComponentsInChildren<IValidatable>()
		where !(v is PropGrouper)
		select v)
		{
			ValidationState validationState2 = validatable.DoValidation();
			if ((validationState2 == ValidationState.Unknown && validationState == ValidationState.Passed) || validationState2 == ValidationState.Failed)
			{
				validationState = validationState2;
			}
		}
		return validationState;
	}

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06001482 RID: 5250 RVA: 0x000679F4 File Offset: 0x00065BF4
	// (set) Token: 0x06001483 RID: 5251 RVA: 0x000679FC File Offset: 0x00065BFC
	public ValidationState ValidationState { get; private set; }

	// Token: 0x06001484 RID: 5252 RVA: 0x00067A08 File Offset: 0x00065C08
	private bool Verify()
	{
		foreach (PropSpawner propSpawner in base.GetComponentsInChildren<PropSpawner>())
		{
			if (propSpawner.props == null)
			{
				Debug.LogError("Missing spawns on " + propSpawner.name, propSpawner.gameObject);
				return false;
			}
			GameObject[] props = propSpawner.props;
			for (int j = 0; j < props.Length; j++)
			{
				if (props[j] == null)
				{
					Debug.LogError("Missing prefab on " + propSpawner.name, propSpawner.gameObject);
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x00067A98 File Offset: 0x00065C98
	public void ClearAll()
	{
		LevelGenStep[] componentsInChildren = base.GetComponentsInChildren<LevelGenStep>(true);
		int num = 0;
		foreach (LevelGenStep levelGenStep in componentsInChildren)
		{
			if (!(levelGenStep == null))
			{
				levelGenStep.Clear();
				num++;
			}
		}
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x00067ADC File Offset: 0x00065CDC
	[CompilerGenerated]
	private void <RunAll>g__Done|3_0(ref PropGrouper.<>c__DisplayClass3_0 A_1)
	{
		foreach (LevelGenStep levelGenStep in A_1.late)
		{
			levelGenStep.Execute();
			if (levelGenStep.DeferredTiming != DeferredStepTiming.None)
			{
				this.<RunAll>g__AddToStepList|3_2(levelGenStep.DeferredTiming, levelGenStep.ConstructDeferred(levelGenStep), ref A_1);
			}
		}
		this.<RunAll>g__ExecuteAndClearDeferredStepsFor|3_1(DeferredStepTiming.AfterCurrentGroupTiming, ref A_1);
		if (this.ValidateAfterwards)
		{
			this.Validate();
		}
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x00067B60 File Offset: 0x00065D60
	[CompilerGenerated]
	private void <RunAll>g__ExecuteAndClearDeferredStepsFor|3_1(DeferredStepTiming key, ref PropGrouper.<>c__DisplayClass3_0 A_2)
	{
		if (!A_2.deferredSteps.ContainsKey(key))
		{
			return;
		}
		Debug.Log(string.Format("Executing {0} steps now that group is finished.", A_2.deferredSteps[key].Count));
		foreach (IDeferredStep deferredStep in A_2.deferredSteps[key])
		{
			deferredStep.DeferredGo();
		}
		A_2.deferredSteps[key].Clear();
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x00067BFC File Offset: 0x00065DFC
	[CompilerGenerated]
	private void <RunAll>g__AddToStepList|3_2(DeferredStepTiming key, IDeferredStep stepToAdd, ref PropGrouper.<>c__DisplayClass3_0 A_3)
	{
		if (!A_3.deferredSteps.ContainsKey(key))
		{
			A_3.deferredSteps.Add(key, new List<IDeferredStep>());
		}
		A_3.deferredSteps[key].Add(stepToAdd);
	}

	// Token: 0x040012AA RID: 4778
	public PropGrouper.PropGrouperTiming timing;

	// Token: 0x040012AB RID: 4779
	[SerializeField]
	private bool ValidateAfterwards;

	// Token: 0x02000525 RID: 1317
	public enum PropGrouperTiming
	{
		// Token: 0x04001C6E RID: 7278
		Early,
		// Token: 0x04001C6F RID: 7279
		Late
	}
}
