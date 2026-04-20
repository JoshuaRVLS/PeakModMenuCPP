using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class FootStepPlayer : MonoBehaviour
{
	// Token: 0x0600125C RID: 4700 RVA: 0x0005BEDE File Offset: 0x0005A0DE
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x0005BEF8 File Offset: 0x0005A0F8
	private void Update()
	{
		this.doStep = 0;
		using (IEnumerator enumerator = base.transform.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (((Transform)enumerator.Current).gameObject.activeSelf)
				{
					this.doStep++;
				}
			}
		}
		if (this.doStep == 0)
		{
			this.t = false;
		}
		if (this.doStep > 0 && !this.t)
		{
			this.PlayStep();
		}
		if (this.character.data.sinceGrounded <= 0f && !this.onGround.active)
		{
			this.onGround.SetActive(true);
			this.offGround.SetActive(false);
			this.PlayStep();
		}
		if (this.character.data.sinceGrounded > 0.25f && !this.offGround.active)
		{
			this.offGround.SetActive(true);
			this.onGround.SetActive(false);
			this.PlayStep();
		}
		if (this.bingBongRoom)
		{
			this.timer += Time.deltaTime;
			if (this.timer > 4f)
			{
				this.ambience.bingBongStatue.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x0005C054 File Offset: 0x0005A254
	private void PlayStep()
	{
		if (Physics.Linecast(base.transform.position, base.transform.position + Vector3.down * 100f, out this.hit, this.floorLayer))
		{
			MeshRenderer component = this.hit.collider.GetComponent<MeshRenderer>();
			if (this.hit.collider.name == "BigRoom")
			{
				this.bingBongRoom = true;
			}
			if (component)
			{
				if (component.material.name == this.beachSand.name + " (Instance)")
				{
					this.surfaceLookup.PlayStep(base.transform.position, 1, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.beachRock.name + " (Instance)")
				{
					this.surfaceLookup.PlayStep(base.transform.position, 2, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.desertSand.name + " (Instance)")
				{
					this.surfaceLookup.PlayStep(base.transform.position, 1, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.desertSand.name)
				{
					this.surfaceLookup.PlayStep(base.transform.position, 1, this.audioSourceOverride);
					this.t = true;
					return;
				}
				foreach (Material material in this.jungleGrass)
				{
					if (component.material.name == material.name + " (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 3, this.audioSourceOverride);
						}
						this.t = true;
					}
				}
				if (component.material.name == this.jungleRock.name + " (Instance)")
				{
					this.surfaceLookup.PlayStep(base.transform.position, 4, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.iceRock.name + " (Instance)")
				{
					if (this.ambience)
					{
						this.ambience.naturelessTerrain = 30f;
					}
					this.surfaceLookup.PlayStep(base.transform.position, 5, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.iceSnow.name + " (Instance)")
				{
					if (this.ambience)
					{
						this.ambience.naturelessTerrain = 30f;
					}
					this.surfaceLookup.PlayStep(base.transform.position, 6, this.audioSourceOverride);
					this.t = true;
					return;
				}
				if (component.material.name == this.volcanoRock.name + " (Instance)")
				{
					if (this.ambience)
					{
						this.ambience.naturelessTerrain = 30f;
						this.ambience.vulcanoT = 10f;
					}
					this.surfaceLookup.PlayStep(base.transform.position, 9, this.audioSourceOverride);
					this.t = true;
					return;
				}
				foreach (Material material2 in this.metal)
				{
					if (component.material.name == material2.name + " (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 7, this.audioSourceOverride);
						}
						this.t = true;
					}
				}
				foreach (Material material3 in this.wood)
				{
					if (component.material.name == material3.name)
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 8, this.audioSourceOverride);
						}
						this.t = true;
					}
					if (component.material.name == material3.name + " (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 8, this.audioSourceOverride);
						}
						this.t = true;
					}
					if (component.material.name == material3.name + " (Instance) (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 8, this.audioSourceOverride);
						}
						this.t = true;
					}
				}
				if (!this.t)
				{
					this.surfaceLookup.PlayStep(base.transform.position, 0, this.audioSourceOverride);
					this.t = true;
				}
			}
			else
			{
				this.surfaceLookup.PlayStep(base.transform.position, 0, this.audioSourceOverride);
				this.t = true;
			}
		}
		else
		{
			this.surfaceLookup.PlayStep(base.transform.position, 0, this.audioSourceOverride);
			this.t = true;
		}
		this.t = true;
	}

	// Token: 0x0400106D RID: 4205
	private Character character;

	// Token: 0x0400106E RID: 4206
	public LayerMask floorLayer;

	// Token: 0x0400106F RID: 4207
	public StepSoundCollection surfaceLookup;

	// Token: 0x04001070 RID: 4208
	private int doStep;

	// Token: 0x04001071 RID: 4209
	private bool t;

	// Token: 0x04001072 RID: 4210
	public Material beachSand;

	// Token: 0x04001073 RID: 4211
	public Material beachRock;

	// Token: 0x04001074 RID: 4212
	public Material desertSand;

	// Token: 0x04001075 RID: 4213
	public Material[] jungleGrass;

	// Token: 0x04001076 RID: 4214
	public Material jungleRock;

	// Token: 0x04001077 RID: 4215
	public Material iceSnow;

	// Token: 0x04001078 RID: 4216
	public Material iceRock;

	// Token: 0x04001079 RID: 4217
	public Material volcanoRock;

	// Token: 0x0400107A RID: 4218
	public Material[] metal;

	// Token: 0x0400107B RID: 4219
	public Material[] wood;

	// Token: 0x0400107C RID: 4220
	private RaycastHit hit;

	// Token: 0x0400107D RID: 4221
	public GameObject onGround;

	// Token: 0x0400107E RID: 4222
	public GameObject offGround;

	// Token: 0x0400107F RID: 4223
	public AmbienceAudio ambience;

	// Token: 0x04001080 RID: 4224
	public AudioSource audioSourceOverride;

	// Token: 0x04001081 RID: 4225
	private bool bingBongRoom;

	// Token: 0x04001082 RID: 4226
	private float timer;
}
