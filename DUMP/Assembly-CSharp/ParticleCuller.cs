using System;
using UnityEngine;

// Token: 0x020002C3 RID: 707
public class ParticleCuller : MonoBehaviour
{
	// Token: 0x060013F8 RID: 5112 RVA: 0x0006517F File Offset: 0x0006337F
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.cullDistance);
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x000651A1 File Offset: 0x000633A1
	public void OnEnable()
	{
		if (ParticleManager.instance != null)
		{
			ParticleManager.instance.Register(this);
		}
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x000651BB File Offset: 0x000633BB
	public void OnDisable()
	{
		if (ParticleManager.instance != null)
		{
			ParticleManager.instance.Unregister(this);
		}
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x000651D8 File Offset: 0x000633D8
	public void Scan()
	{
		if (Character.observedCharacter)
		{
			bool flag = Vector3.Distance(Character.observedCharacter.Center, base.transform.position) < this.cullDistance;
			for (int i = 0; i < this.systems.Length; i++)
			{
				if (flag && !this.systems[i].isPlaying)
				{
					this.systems[i].Play();
				}
				if (!flag && this.systems[i].isPlaying)
				{
					this.systems[i].Stop();
				}
			}
		}
	}

	// Token: 0x04001234 RID: 4660
	public ParticleSystem[] systems;

	// Token: 0x04001235 RID: 4661
	public float cullDistance = 50f;
}
