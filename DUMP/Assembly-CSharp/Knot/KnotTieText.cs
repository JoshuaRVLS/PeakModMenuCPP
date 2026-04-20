using System;
using pworld.Scripts.Extensions;
using pworld.Scripts.PPhys;
using UnityEngine;

namespace Knot
{
	// Token: 0x020003B4 RID: 948
	public class KnotTieText : MonoBehaviour
	{
		// Token: 0x0600195C RID: 6492 RVA: 0x00080158 File Offset: 0x0007E358
		private void Awake()
		{
			this.spring = base.GetComponent<PPhysSpringBase>();
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x00080166 File Offset: 0x0007E366
		private void Start()
		{
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x00080168 File Offset: 0x0007E368
		private void Update()
		{
			this.timeAlive += Time.deltaTime;
			if (this.timeAlive > this.lifeTime)
			{
				Object.Destroy(base.gameObject);
			}
			if (this.timeAlive > this.lifeTime - 1f)
			{
				this.spring.Target = 0.ToVec();
			}
			base.transform.position += Vector3.up * (Time.deltaTime * this.velocity);
		}

		// Token: 0x04001721 RID: 5921
		public float velocity;

		// Token: 0x04001722 RID: 5922
		public PPhysSpringBase spring;

		// Token: 0x04001723 RID: 5923
		public float lifeTime;

		// Token: 0x04001724 RID: 5924
		private float timeAlive;
	}
}
