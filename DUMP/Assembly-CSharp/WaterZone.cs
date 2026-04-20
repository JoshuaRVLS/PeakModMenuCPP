using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x020000B6 RID: 182
public class WaterZone : SerializedMonoBehaviour
{
	// Token: 0x060006D6 RID: 1750 RVA: 0x0002742B File Offset: 0x0002562B
	private void Awake()
	{
		this.zoneBounds.center = base.transform.position;
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00027443 File Offset: 0x00025643
	private void OnEnable()
	{
		base.StartCoroutine(this.UpdateTrackedCharactersRoutine());
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00027452 File Offset: 0x00025652
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x0002745A File Offset: 0x0002565A
	private void Update()
	{
		this.UpdateCharacterVisuals();
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00027464 File Offset: 0x00025664
	public void setZoneBounds()
	{
		if (this.waterPlane != null)
		{
			this.zoneBounds = new Bounds(this.waterPlane.bounds.center, new Vector3(this.waterPlane.bounds.size.x, this.waterDepth, this.waterPlane.bounds.size.z));
		}
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x000274D8 File Offset: 0x000256D8
	private IEnumerator UpdateTrackedCharactersRoutine()
	{
		while (base.isActiveAndEnabled)
		{
			foreach (Character key in Character.AllCharacters)
			{
				if (!this.trackedCharacters.ContainsKey(key))
				{
					this.trackedCharacters.Add(key, WaterZone.InWaterState.NONE);
				}
			}
			foreach (Character key2 in Character.AllBotCharacters)
			{
				if (!this.trackedCharacters.ContainsKey(key2))
				{
					this.trackedCharacters.Add(key2, WaterZone.InWaterState.NONE);
				}
			}
			this.cachedCharacters = this.trackedCharacters.Keys.ToArray<Character>();
			foreach (Character character in this.cachedCharacters)
			{
				if (character == null)
				{
					yield return null;
				}
				else
				{
					if (this.zoneBounds.Contains(character.Center))
					{
						this.trackedCharacters[character] = WaterZone.InWaterState.Deep;
					}
					else if (this.zoneBounds.Contains(character.GetBodypart(BodypartType.Foot_L).transform.position) || this.zoneBounds.Contains(character.GetBodypart(BodypartType.Foot_R).transform.position))
					{
						this.trackedCharacters[character] = WaterZone.InWaterState.Shallow;
					}
					else
					{
						this.trackedCharacters[character] = WaterZone.InWaterState.NONE;
					}
					yield return null;
				}
			}
			Character[] array = null;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x000274E8 File Offset: 0x000256E8
	private void UpdateCharacterVisuals()
	{
		foreach (KeyValuePair<Character, WaterZone.InWaterState> keyValuePair in this.trackedCharacters)
		{
			if (keyValuePair.Value == WaterZone.InWaterState.Deep)
			{
				keyValuePair.Key.data.inWater = this.animModDeep;
			}
			else if (keyValuePair.Value == WaterZone.InWaterState.Shallow)
			{
				keyValuePair.Key.data.inWater = this.animModShallow;
			}
		}
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00027578 File Offset: 0x00025778
	private void FixedUpdate()
	{
		this.TryAddForceToCharacter();
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x00027580 File Offset: 0x00025780
	private void TryAddForceToCharacter()
	{
		foreach (KeyValuePair<Character, WaterZone.InWaterState> keyValuePair in this.trackedCharacters)
		{
			if (keyValuePair.Value == WaterZone.InWaterState.Deep)
			{
				keyValuePair.Key.refs.movement.SetWaterMovementModifier(this.movementModifierDeep);
			}
			else if (keyValuePair.Value == WaterZone.InWaterState.Shallow)
			{
				keyValuePair.Key.refs.movement.SetWaterMovementModifier(this.movementModifierShallow);
			}
		}
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x0002761C File Offset: 0x0002581C
	private void OnDrawGizmosSelected()
	{
		this.zoneBounds.center = base.transform.position;
		Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
		Gizmos.DrawCube(this.zoneBounds.center, this.zoneBounds.extents * 2f);
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(this.zoneBounds.center, this.zoneBounds.extents * 2f);
	}

	// Token: 0x040006CE RID: 1742
	public MeshRenderer waterPlane;

	// Token: 0x040006CF RID: 1743
	public float waterDepth;

	// Token: 0x040006D0 RID: 1744
	public Bounds zoneBounds;

	// Token: 0x040006D1 RID: 1745
	[SerializeField]
	public Dictionary<Character, WaterZone.InWaterState> trackedCharacters = new Dictionary<Character, WaterZone.InWaterState>();

	// Token: 0x040006D2 RID: 1746
	public float movementModifierShallow = 0.85f;

	// Token: 0x040006D3 RID: 1747
	public float movementModifierDeep = 0.7f;

	// Token: 0x040006D4 RID: 1748
	public float animModDeep = 0.25f;

	// Token: 0x040006D5 RID: 1749
	public float animModShallow = 0.3f;

	// Token: 0x040006D6 RID: 1750
	private Character[] cachedCharacters;

	// Token: 0x02000453 RID: 1107
	public enum InWaterState
	{
		// Token: 0x040018FD RID: 6397
		NONE,
		// Token: 0x040018FE RID: 6398
		Shallow,
		// Token: 0x040018FF RID: 6399
		Deep
	}
}
