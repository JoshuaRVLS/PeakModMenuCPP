using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200012A RID: 298
public class OrbThatMakesYouSleepy : MonoBehaviour
{
	// Token: 0x060009B9 RID: 2489 RVA: 0x00033AEE File Offset: 0x00031CEE
	private void Start()
	{
		this.anim.speed = this.animSpeed;
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x00033B01 File Offset: 0x00031D01
	public void Tick()
	{
		if (RunSettings.GetValue(RunSettings.SETTINGTYPE.Hazard_NapberryHypno, false) == 0)
		{
			return;
		}
		this.UpdateHypnosis(false);
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00033B18 File Offset: 0x00031D18
	private void LateUpdate()
	{
		if (this.fakeBerry != null && this.fakeBerry.gameObject.activeInHierarchy)
		{
			this.anim.speed = this.animSpeed;
			this.ambientParticles.gameObject.SetActive(true);
			this.fakeBerry.transform.localPosition = new Vector3(-0.013f, -0.22f, 0.008f);
			this.fakeBerry.transform.localEulerAngles = Vector3.zero;
			return;
		}
		this.anim.speed = 0f;
		this.ambientParticles.gameObject.SetActive(false);
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x00033BC2 File Offset: 0x00031DC2
	private void DebugHypnosis()
	{
		this.UpdateHypnosis(true);
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x00033BCC File Offset: 0x00031DCC
	private void UpdateHypnosis(bool debug = false)
	{
		if (!base.enabled)
		{
			return;
		}
		if (!this.fakeBerry || !this.fakeBerry.gameObject.activeInHierarchy)
		{
			return;
		}
		if (!Character.localCharacter.UnityObjectExists<Character>() || !Character.localCharacter.data.fullyConscious)
		{
			return;
		}
		Vector3 to = Character.localCharacter.Center - this.orb.transform.position;
		if (debug)
		{
			Debug.Log("distance to character: " + to.magnitude.ToString());
		}
		if (to.magnitude > this.maxDistance)
		{
			return;
		}
		if (!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(MainCamera.instance.cam), new Bounds(this.orb.transform.position, Vector3.one * 0.5f)))
		{
			if (debug)
			{
				Debug.Log("Not inside view frustum");
			}
			return;
		}
		Collider collider = HelperFunctions.LineCheck(this.orb.transform.position, MainCamera.instance.cam.transform.position, HelperFunctions.LayerType.AllPhysical, 0f, QueryTriggerInteraction.Ignore).collider;
		if (collider == null && debug)
		{
			Debug.Log("Hit nothing");
		}
		if (collider.gameObject.GetComponentInParent<Character>() == Character.localCharacter && debug)
		{
			Debug.Log("Hit our own character");
		}
		float value = Vector3.Angle(-MainCamera.instance.cam.transform.forward, to);
		float num = Mathf.InverseLerp(this.maxDistance, 2f, to.magnitude);
		float num2 = Mathf.Lerp(10f, 110f, num);
		float b = Mathf.InverseLerp(num2, num2 / 2f, value);
		float amount = Mathf.Lerp(this.minDrowsyPerTick, this.maxDrowsyPerTick, Mathf.Min(num, b));
		Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, amount, false, true, true);
		this.particle.Play();
	}

	// Token: 0x040008FA RID: 2298
	public Transform orb;

	// Token: 0x040008FB RID: 2299
	public float maxDistance;

	// Token: 0x040008FC RID: 2300
	public float minDrowsyPerSecond;

	// Token: 0x040008FD RID: 2301
	public float maxDrowsyPerSecond;

	// Token: 0x040008FE RID: 2302
	public float minDrowsyPerTick;

	// Token: 0x040008FF RID: 2303
	public float maxDrowsyPerTick;

	// Token: 0x04000900 RID: 2304
	public ParticleSystem particle;

	// Token: 0x04000901 RID: 2305
	public Animator anim;

	// Token: 0x04000902 RID: 2306
	public float animSpeed = 1f;

	// Token: 0x04000903 RID: 2307
	public ParticleSystem ambientParticles;

	// Token: 0x04000904 RID: 2308
	public FakeItem fakeBerry;

	// Token: 0x04000905 RID: 2309
	private Plane[] planes = new Plane[6];
}
