using System;
using System.Collections.Generic;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class RockSpawnerGD : MonoBehaviour
{
	// Token: 0x06000BE4 RID: 3044 RVA: 0x0003F90C File Offset: 0x0003DB0C
	public void createDeck()
	{
		this.deck.Clear();
		for (int i = 0; i < this.objectsToSpawn.Length; i++)
		{
			for (int j = 0; j < this.objectsToSpawn[i].maxCount; j++)
			{
				this.deck.Add(this.objectsToSpawn[i]);
			}
		}
		this.shuffleDeck();
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x0003F968 File Offset: 0x0003DB68
	public void shuffleDeck()
	{
		for (int i = 0; i < this.deck.Count; i++)
		{
			SpawnObject value = this.deck[i];
			int index = Random.Range(i, this.objectsToSpawn.Length);
			this.deck[i] = this.deck[index];
			this.deck[index] = value;
		}
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x0003F9CC File Offset: 0x0003DBCC
	public SpawnObject DrawFromDeck()
	{
		SpawnObject result = this.deck[0];
		this.deck.RemoveAt(0);
		return result;
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x0003F9E8 File Offset: 0x0003DBE8
	public void spawnObjects()
	{
		this.clearList();
		this.createDeck();
		int count = this.deck.Count;
		int num = count / this.layerCount;
		if (this.layerCount > count)
		{
			num = count;
		}
		for (int i = 0; i < count; i++)
		{
			float p = (float)i * this.yBias + 1f;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position - base.transform.up + (base.transform.right * Random.Range(-1f, 1f) * this.shape.size.x / 2f + base.transform.forward * (Mathf.Pow(Random.Range(-1f, 1f), p) * this.shape.size.z / 2f)), -base.transform.up, out raycastHit))
			{
				SpawnObject spawnObject = this.DrawFromDeck();
				GameObject gameObject = Object.Instantiate<GameObject>(spawnObject.prefab);
				gameObject.transform.position = raycastHit.point + new Vector3(Random.Range(-spawnObject.posJitter.x, spawnObject.posJitter.x), Random.Range(-spawnObject.posJitter.y, spawnObject.posJitter.y), Random.Range(-spawnObject.posJitter.z, spawnObject.posJitter.z));
				gameObject.transform.eulerAngles += new Vector3(Random.Range(-spawnObject.randomRot.x, spawnObject.randomRot.x), Random.Range(-spawnObject.randomRot.y, spawnObject.randomRot.y), Random.Range(-spawnObject.randomRot.z, spawnObject.randomRot.z));
				gameObject.transform.localScale += new Vector3(Random.Range(-spawnObject.randomScale.x, spawnObject.randomScale.x), Random.Range(-spawnObject.randomScale.y, spawnObject.randomScale.y), Random.Range(-spawnObject.randomScale.z, spawnObject.randomScale.z));
				gameObject.transform.localScale += Vector3.one * Random.Range(-spawnObject.uniformScale, spawnObject.uniformScale);
				gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, Vector3.one - new Vector3((float)Random.Range(0f, spawnObject.inversion.x).PCeilToInt(), (float)Random.Range(0f, spawnObject.inversion.y).PCeilToInt(), (float)Random.Range(0f, spawnObject.inversion.z).PCeilToInt()).normalized * 2f);
				gameObject.transform.localScale *= spawnObject.scaleMultiplier;
				this.spawnedObjects.Add(gameObject);
				gameObject.transform.parent = base.transform;
				if (i % num == 0)
				{
					Physics.SyncTransforms();
				}
			}
		}
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x0003FD88 File Offset: 0x0003DF88
	public void clearList()
	{
		for (int i = 0; i < this.spawnedObjects.Count; i++)
		{
			Object.DestroyImmediate(this.spawnedObjects[i]);
		}
		this.spawnedObjects.Clear();
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x0003FDC7 File Offset: 0x0003DFC7
	public void OnValidate()
	{
		this.shape.size = new Vector3(this.colliderScale.x, 0f, this.colliderScale.y);
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x0003FDF4 File Offset: 0x0003DFF4
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x04000ADA RID: 2778
	public Vector2 colliderScale;

	// Token: 0x04000ADB RID: 2779
	public SpawnObject[] objectsToSpawn;

	// Token: 0x04000ADC RID: 2780
	public List<GameObject> spawnedObjects;

	// Token: 0x04000ADD RID: 2781
	public List<SpawnObject> deck;

	// Token: 0x04000ADE RID: 2782
	public Vector2 castShape;

	// Token: 0x04000ADF RID: 2783
	public BoxCollider shape;

	// Token: 0x04000AE0 RID: 2784
	public float yBias;

	// Token: 0x04000AE1 RID: 2785
	[Range(1f, 99f)]
	public int layerCount;
}
