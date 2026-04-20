using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class CheckpointConstructable : Constructable
{
	// Token: 0x060008EF RID: 2287 RVA: 0x00030DAC File Offset: 0x0002EFAC
	public override GameObject FinishConstruction()
	{
		GameObject gameObject = base.FinishConstruction();
		CheckpointFlag checkpointFlag;
		if (gameObject && gameObject.TryGetComponent<CheckpointFlag>(out checkpointFlag))
		{
			checkpointFlag.Initialize(this.item.holderCharacter);
		}
		else
		{
			Debug.LogWarning("Failed to construct our checkpoint flag! That's bad, right?");
		}
		return gameObject;
	}
}
