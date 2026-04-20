using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000202 RID: 514
public class AmbienceAudio : MonoBehaviour
{
	// Token: 0x0600100A RID: 4106 RVA: 0x0004E880 File Offset: 0x0004CA80
	private void Start()
	{
		this.ambienceVolumes = base.GetComponent<Animator>();
		this.dayNight = Object.FindAnyObjectByType<DayNightManager>();
		this.character = base.GetComponentInParent<Character>();
		this.stingerSource.clip = this.startStinger[UnityEngine.Random.Range(0, this.startStinger.Length)];
		this.stingerSource.Play();
		this.volcanoObj = GameObject.Find("VolcanoModel");
		if (GameObject.Find("Airport"))
		{
			base.gameObject.SetActive(false);
			if (this.voice)
			{
				this.reverbFilter.enabled = false;
				this.echoFilter.enabled = false;
				this.lowPassFilter.enabled = false;
			}
		}
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x0004E93C File Offset: 0x0004CB3C
	private void Update()
	{
		if (this.character.inAirport)
		{
			return;
		}
		this.naturelessTerrain -= 6f * Time.deltaTime;
		if (this.naturelessTerrain > 0f)
		{
			this.ambienceVolumes.SetBool("Natureless", true);
		}
		if (this.naturelessTerrain < 0f)
		{
			this.ambienceVolumes.SetBool("Natureless", false);
		}
		this.ambienceVolumes.SetBool("Tomb", this.inTomb);
		try
		{
			float x = math.saturate(LightVolume.Instance().SamplePositionAlpha(base.transform.position));
			x = math.saturate(1f - math.remap(0f, 0.3f, 0f, 1f, x));
			this.reverb.room = (int)math.remap(0f, 1f, -4000f, -100f, x);
		}
		catch
		{
			Debug.LogError("You probably need to bake the lightmap");
		}
		if (this.volcanoObj)
		{
			this.vulcanoT -= Time.deltaTime;
			if (this.vulcanoT <= 0f)
			{
				this.volcano = false;
				this.vulcanoT = 0f;
				this.reverb.enabled = true;
			}
			if (this.vulcanoT > 0f)
			{
				this.volcano = true;
				this.reverb.enabled = false;
			}
			if (Vector3.Distance(base.transform.position, this.volcanoObj.transform.position) < 200f)
			{
				this.vulcanoT = 10f;
			}
			this.ambienceVolumes.SetBool("Volcano", this.volcano);
		}
		if (this.ambienceVolumes && this.dayNight)
		{
			this.ambienceVolumes.SetFloat("Height", base.transform.position.y);
			this.ambienceVolumes.SetFloat("Time", this.dayNight.timeOfDay);
			if (base.transform.position.z > this.alpineStingerZ - 500f && !this.alpineSunTime1 && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
			{
				this.ambienceVolumes.SetBool("Desert", true);
			}
			if (!this.inTomb)
			{
				if (this.dayNight.timeOfDay > 5.5f && this.dayNight.timeOfDay < 6.5f && this.t != 1)
				{
					this.t = 1;
					this.stingerSource.clip = this.sunRiseStinger[UnityEngine.Random.Range(0, this.sunRiseStinger.Length)];
					if (base.transform.position.z > this.tropicsStingerZ - 500f && !this.tropicsSunTime2)
					{
						if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Tropics))
						{
							this.stingerSource.clip = this.tropicsSunrise;
						}
						else if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Roots))
						{
							this.stingerSource.clip = this.rootsSunrise;
						}
						if (this.priorityMusicTimer <= 0f)
						{
							this.tropicsSunTime2 = true;
						}
					}
					if (base.transform.position.z > this.alpineStingerZ - 500f && !this.alpineSunTime2)
					{
						if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Alpine))
						{
							this.stingerSource.clip = this.alpineSunrise;
						}
						else if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
						{
							this.stingerSource.clip = this.desertSunrise;
						}
						if (this.priorityMusicTimer <= 0f)
						{
							this.alpineSunTime2 = true;
						}
					}
					if (!this.volcano && base.transform.position.z < this.calderaStingerZ)
					{
						this.stingerSource.Play();
					}
				}
				if (this.dayNight.timeOfDay > 19.5f && this.dayNight.timeOfDay < 20f && this.t != 2)
				{
					this.t = 2;
					this.stingerSource.clip = this.sunSetStinger[UnityEngine.Random.Range(0, this.sunSetStinger.Length)];
					if (base.transform.position.z > this.tropicsStingerZ - 500f && !this.tropicsSunTime1)
					{
						if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Tropics))
						{
							this.stingerSource.clip = this.tropicsSunset;
						}
						else if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Roots))
						{
							this.stingerSource.clip = this.rootsSunset;
						}
						if (this.priorityMusicTimer <= 0f)
						{
							this.tropicsSunTime1 = true;
						}
					}
					if (base.transform.position.z > this.alpineStingerZ - 500f && !this.alpineSunTime1)
					{
						if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Alpine))
						{
							this.stingerSource.clip = this.alpineSunset;
						}
						else if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
						{
							this.stingerSource.clip = this.desertSunset;
						}
						this.ambienceVolumes.SetBool("Desert", true);
						if (this.priorityMusicTimer <= 0f)
						{
							this.alpineSunTime1 = true;
						}
					}
					if (!this.volcano && base.transform.position.z < this.calderaStingerZ)
					{
						this.stingerSource.Play();
					}
				}
				if (this.dayNight.timeOfDay > 21.2f && this.dayNight.timeOfDay < 26f && this.t != 3)
				{
					this.t = 3;
					this.stingerSource.clip = this.nightStinger[UnityEngine.Random.Range(0, this.nightStinger.Length)];
					if (!this.volcano && base.transform.position.z < this.calderaStingerZ)
					{
						this.stingerSource.Play();
					}
				}
			}
		}
		this.priorityMusicTimer -= Time.deltaTime;
		CharacterData data = this.character.data;
		if (data.sinceDead > 0.5f && !Character.localCharacter.warping && !data.passedOut && !data.dead && !data.fullyPassedOut && !this.inTomb)
		{
			if (base.transform.position.z > this.beachStingerZ && !this.playedBeach)
			{
				this.playedBeach = true;
				this.mainMusic.clip = this.climbStingerBeach;
				this.mainMusic.volume = 0.35f;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played beach stinger");
			}
			if (base.transform.position.z > this.tropicsStingerZ && !this.playedTropicsOrRoots && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Tropics))
			{
				this.playedTropicsOrRoots = true;
				this.mainMusic.clip = this.climbStingerTropics;
				this.mainMusic.volume = 0.5f;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played tropics stinger");
			}
			if (base.transform.position.z > this.tropicsStingerZ && !this.playedTropicsOrRoots && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Roots))
			{
				this.playedTropicsOrRoots = true;
				this.mainMusic.clip = this.climbStingerRoots;
				this.mainMusic.volume = 0.5f;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played tropics stinger");
			}
			if (base.transform.position.z > this.alpineStingerZ && !this.playedAlpineOrMesa && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Alpine))
			{
				this.mainMusic.volume = 0.4f;
				this.playedAlpineOrMesa = true;
				this.mainMusic.clip = this.climbStingerAlpine;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played alpine stinger");
			}
			if (base.transform.position.z > this.alpineStingerZ && !this.playedAlpineOrMesa && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
			{
				this.mainMusic.volume = 0.4f;
				this.playedAlpineOrMesa = true;
				this.mainMusic.clip = this.climbStingerMesa;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played mesa stinger");
			}
			if (base.transform.position.z > this.calderaStingerZ && !this.playedCaldera)
			{
				if (!this.volcanoObj)
				{
					this.volcanoObj = GameObject.Find("VolcanoModel");
				}
				this.mainMusic.volume = 0.75f;
				this.playedCaldera = true;
				this.mainMusic.clip = this.climbStingerCaldera;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
				Debug.Log("Played caldera stinger");
			}
			if (base.transform.position.y > this.kilnStingerY && !this.playedKiln)
			{
				this.inKiln -= Time.deltaTime;
				if (this.inKiln < -2f)
				{
					this.mainMusic.volume = 0.6f;
					this.playedKiln = true;
					this.mainMusic.clip = this.climbStingerKiln;
					this.mainMusic.Play();
					this.priorityMusicTimer = 120f;
					Debug.Log("Played kiln stinger");
				}
			}
			else
			{
				this.inKiln = 0f;
			}
			if (base.transform.position.z > this.peaksTingerZ && !this.playedPeak)
			{
				this.mainMusic.volume = 1f;
				this.playedPeak = true;
				this.mainMusic.clip = this.climbStingerPeak;
				this.mainMusic.Play();
				this.priorityMusicTimer = 120f;
			}
		}
		else
		{
			this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0f, 0.05f);
			this.mainMusic.volume = Mathf.Lerp(this.mainMusic.volume, 0f, 0.05f);
		}
		if (this.priorityMusicTimer > 0f)
		{
			this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0f, 0.05f);
		}
		if (this.priorityMusicTimer <= 0f)
		{
			this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0.35f, 0.05f);
			this.priorityMusicTimer = 0f;
		}
		if (this.inTomb)
		{
			if (base.transform.position.z > 700f && !this.playedTomb)
			{
				this.playedTomb = true;
				this.mainMusic.clip = this.tombClimb;
				this.mainMusic.Play();
			}
			this.mainMusic.volume = 0.5f;
		}
		if (base.transform.position.y > 450f)
		{
			this.inTomb = false;
		}
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x0004F47C File Offset: 0x0004D67C
	private void Coverage()
	{
		float num = 8f;
		this.ceiling = false;
		if (Physics.Linecast(base.transform.position, base.transform.position + Vector3.up * 8f * num, out this.hit, this.layer))
		{
			this.ceiling = true;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.forward * num, out this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.forward * -num, out this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.right * num, out this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.right * -num, out this.hit, this.layer))
		{
			this.coverage += 1f;
		}
		if (Physics.Linecast(base.transform.position, base.transform.position + base.transform.up * num * 4f, out this.hit, this.layer))
		{
			this.coverage += 2f;
		}
	}

	// Token: 0x04000DAB RID: 3499
	public float obstruction;

	// Token: 0x04000DAC RID: 3500
	private float coverage;

	// Token: 0x04000DAD RID: 3501
	public bool ceiling;

	// Token: 0x04000DAE RID: 3502
	public bool inTomb;

	// Token: 0x04000DAF RID: 3503
	public LayerMask layer;

	// Token: 0x04000DB0 RID: 3504
	private RaycastHit hit;

	// Token: 0x04000DB1 RID: 3505
	public AudioReverbZone reverb;

	// Token: 0x04000DB2 RID: 3506
	private DayNightManager dayNight;

	// Token: 0x04000DB3 RID: 3507
	private Animator ambienceVolumes;

	// Token: 0x04000DB4 RID: 3508
	private int t;

	// Token: 0x04000DB5 RID: 3509
	public AudioSource stingerSource;

	// Token: 0x04000DB6 RID: 3510
	public AudioClip[] startStinger;

	// Token: 0x04000DB7 RID: 3511
	public AudioClip[] sunRiseStinger;

	// Token: 0x04000DB8 RID: 3512
	public AudioClip[] sunSetStinger;

	// Token: 0x04000DB9 RID: 3513
	public AudioClip[] nightStinger;

	// Token: 0x04000DBA RID: 3514
	public bool volcano;

	// Token: 0x04000DBB RID: 3515
	public GameObject volcanoObj;

	// Token: 0x04000DBC RID: 3516
	public float vulcanoT;

	// Token: 0x04000DBD RID: 3517
	public float naturelessTerrain;

	// Token: 0x04000DBE RID: 3518
	public AudioSource mainMusic;

	// Token: 0x04000DBF RID: 3519
	public AudioClip climbStingerBeach;

	// Token: 0x04000DC0 RID: 3520
	private bool playedBeach;

	// Token: 0x04000DC1 RID: 3521
	public AudioClip climbStingerTropics;

	// Token: 0x04000DC2 RID: 3522
	public AudioClip climbStingerRoots;

	// Token: 0x04000DC3 RID: 3523
	private bool playedTropicsOrRoots;

	// Token: 0x04000DC4 RID: 3524
	public AudioClip climbStingerAlpine;

	// Token: 0x04000DC5 RID: 3525
	private bool playedAlpineOrMesa;

	// Token: 0x04000DC6 RID: 3526
	public AudioClip climbStingerMesa;

	// Token: 0x04000DC7 RID: 3527
	public AudioClip climbStingerCaldera;

	// Token: 0x04000DC8 RID: 3528
	private bool playedCaldera;

	// Token: 0x04000DC9 RID: 3529
	public AudioClip climbStingerKiln;

	// Token: 0x04000DCA RID: 3530
	private bool playedKiln;

	// Token: 0x04000DCB RID: 3531
	public AudioClip climbStingerPeak;

	// Token: 0x04000DCC RID: 3532
	private bool playedPeak;

	// Token: 0x04000DCD RID: 3533
	public AudioClip tombClimb;

	// Token: 0x04000DCE RID: 3534
	public AudioSource bingBongStatue;

	// Token: 0x04000DCF RID: 3535
	private float priorityMusicTimer;

	// Token: 0x04000DD0 RID: 3536
	public float beachStingerZ;

	// Token: 0x04000DD1 RID: 3537
	public float tropicsStingerZ;

	// Token: 0x04000DD2 RID: 3538
	public float alpineStingerZ;

	// Token: 0x04000DD3 RID: 3539
	public float calderaStingerZ;

	// Token: 0x04000DD4 RID: 3540
	public float kilnStingerY;

	// Token: 0x04000DD5 RID: 3541
	public float peaksTingerZ;

	// Token: 0x04000DD6 RID: 3542
	public Transform voice;

	// Token: 0x04000DD7 RID: 3543
	public AudioReverbFilter reverbFilter;

	// Token: 0x04000DD8 RID: 3544
	public AudioEchoFilter echoFilter;

	// Token: 0x04000DD9 RID: 3545
	public AudioLowPassFilter lowPassFilter;

	// Token: 0x04000DDA RID: 3546
	private float inKiln;

	// Token: 0x04000DDB RID: 3547
	private bool tropicsSunTime1;

	// Token: 0x04000DDC RID: 3548
	private bool tropicsSunTime2;

	// Token: 0x04000DDD RID: 3549
	public AudioClip tropicsSunrise;

	// Token: 0x04000DDE RID: 3550
	public AudioClip tropicsSunset;

	// Token: 0x04000DDF RID: 3551
	public AudioClip rootsSunrise;

	// Token: 0x04000DE0 RID: 3552
	public AudioClip rootsSunset;

	// Token: 0x04000DE1 RID: 3553
	private bool alpineSunTime1;

	// Token: 0x04000DE2 RID: 3554
	private bool alpineSunTime2;

	// Token: 0x04000DE3 RID: 3555
	public AudioClip alpineSunrise;

	// Token: 0x04000DE4 RID: 3556
	public AudioClip alpineSunset;

	// Token: 0x04000DE5 RID: 3557
	public AudioClip desertSunrise;

	// Token: 0x04000DE6 RID: 3558
	public AudioClip desertSunset;

	// Token: 0x04000DE7 RID: 3559
	private bool playedTomb;

	// Token: 0x04000DE8 RID: 3560
	private Character character;
}
