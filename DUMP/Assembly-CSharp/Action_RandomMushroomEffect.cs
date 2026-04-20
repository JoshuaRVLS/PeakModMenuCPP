using System;
using System.Collections;
using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class Action_RandomMushroomEffect : ItemAction
{
	// Token: 0x060008A1 RID: 2209 RVA: 0x0002FA30 File Offset: 0x0002DC30
	public override void RunAction()
	{
		int effect;
		if (!this.useDebugEffect)
		{
			if (MushroomManager.instance == null)
			{
				return;
			}
			int num = this.mushroomTypeIndex % MushroomManager.instance.mushroomEffects.Length;
			effect = MushroomManager.instance.mushroomEffects[num];
			int num2 = MushroomManager.instance.mushroomStamAmt[num];
			base.character.AddExtraStamina((float)num2 * 0.05f);
		}
		else
		{
			effect = this.debugEffect;
		}
		base.character.StartCoroutine(this.RunRandomEffect(effect));
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0002FAB2 File Offset: 0x0002DCB2
	private IEnumerator RunRandomEffect(int effect)
	{
		Character cachedCharacter = base.character;
		Debug.Log("Triggered effect " + effect.ToString());
		switch (effect)
		{
		case 0:
		{
			yield return new WaitForSeconds(3f);
			Affliction_InfiniteStamina affliction = new Affliction_InfiniteStamina(4f);
			cachedCharacter.refs.afflictions.AddAffliction(affliction, false);
			yield break;
		}
		case 1:
		{
			yield return new WaitForSeconds(3f);
			Affliction_FasterBoi affliction2 = new Affliction_FasterBoi
			{
				moveSpeedMod = 0.5f,
				climbSpeedMod = 1.5f,
				totalTime = 5f,
				climbDelay = 1f
			};
			cachedCharacter.refs.afflictions.AddAffliction(affliction2, false);
			yield break;
		}
		case 2:
		{
			yield return new WaitForSeconds(3f);
			Affliction_LowGravity affliction3 = new Affliction_LowGravity(3, 15f);
			cachedCharacter.refs.afflictions.AddAffliction(affliction3, false);
			yield break;
		}
		case 3:
		{
			Affliction_Invincibility affliction4 = new Affliction_Invincibility
			{
				totalTime = 10f
			};
			cachedCharacter.refs.afflictions.AddAffliction(affliction4, false);
			yield break;
		}
		case 4:
			cachedCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Hunger, -0.15f, false);
			cachedCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Injury, -0.15f, false);
			cachedCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Poison, -0.15f, false);
			base.character.refs.afflictions.ClearPoisonAfflictions();
			yield break;
		case 5:
			yield return new WaitForSeconds(3f);
			GameUtils.instance.SpawnResourceAtPositionNetworked("VFX_SporeExploExploEdibleSpawn", cachedCharacter.Center, RpcTarget.Others);
			GameUtils.instance.RPC_SpawnResourceAtPosition("VFX_SporeExploExploEdibleSpawn_NoKnockback", cachedCharacter.Center);
			cachedCharacter.AddForceToBodyPart(cachedCharacter.GetBodypartRig(BodypartType.Hip), Vector3.zero, Vector3.up * this.fartForce);
			yield break;
		case 6:
		{
			yield return new WaitForSeconds(3f);
			Affliction_Blind affliction5 = new Affliction_Blind
			{
				totalTime = 60f
			};
			cachedCharacter.refs.afflictions.AddAffliction(affliction5, false);
			yield break;
		}
		case 7:
			yield return new WaitForSeconds(3f);
			cachedCharacter.Fall(8f, 0f);
			yield break;
		case 8:
			cachedCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Spores, 0.25f, false);
			yield break;
		case 9:
		{
			yield return new WaitForSeconds(3f);
			Affliction_Numb affliction6 = new Affliction_Numb
			{
				totalTime = 60f
			};
			cachedCharacter.refs.afflictions.AddAffliction(affliction6, false);
			yield break;
		}
		default:
			yield break;
		}
	}

	// Token: 0x04000836 RID: 2102
	public int mushroomTypeIndex;

	// Token: 0x04000837 RID: 2103
	public const string RESOURCE_PATH_EXPLOSION = "VFX_SporeExploExploEdibleSpawn";

	// Token: 0x04000838 RID: 2104
	public const string RESOURCE_PATH_EXPLOSION_NO_KNOCKBACK = "VFX_SporeExploExploEdibleSpawn_NoKnockback";

	// Token: 0x04000839 RID: 2105
	public bool useDebugEffect;

	// Token: 0x0400083A RID: 2106
	public int debugEffect;

	// Token: 0x0400083B RID: 2107
	public float fartForce = 100f;

	// Token: 0x0400083C RID: 2108
	public static int[] GoodEffects = new int[]
	{
		0,
		1,
		2,
		3,
		4
	};

	// Token: 0x0400083D RID: 2109
	public static int[] BadEffects = new int[]
	{
		5,
		6,
		7,
		8,
		9
	};
}
