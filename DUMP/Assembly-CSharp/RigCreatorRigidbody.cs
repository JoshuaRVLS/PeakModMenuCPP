using System;
using UnityEngine;

// Token: 0x02000039 RID: 57
[ExecuteInEditMode]
public class RigCreatorRigidbody : MonoBehaviour
{
	// Token: 0x060003BA RID: 954 RVA: 0x00018FE3 File Offset: 0x000171E3
	private void Awake()
	{
		if (!Application.isEditor || Application.isPlaying)
		{
			Object.Destroy(this);
			return;
		}
		this.SetValues();
	}

	// Token: 0x060003BB RID: 955 RVA: 0x00019000 File Offset: 0x00017200
	private Rigidbody Rig()
	{
		if (!this.rig)
		{
			this.rig = base.GetComponentInParent<Rigidbody>();
		}
		return this.rig;
	}

	// Token: 0x060003BC RID: 956 RVA: 0x00019021 File Offset: 0x00017221
	private RigCreator RigCreator()
	{
		if (!this.rigCreator)
		{
			this.rigCreator = base.GetComponentInParent<RigCreator>();
		}
		return this.rigCreator;
	}

	// Token: 0x060003BD RID: 957 RVA: 0x00019042 File Offset: 0x00017242
	private void Update()
	{
		if (this.mass != this.Rig().mass)
		{
			this.RigCreator().RigidbodyChanged(this, this.Rig().mass);
			this.SetValues();
		}
	}

	// Token: 0x060003BE RID: 958 RVA: 0x00019074 File Offset: 0x00017274
	private void SetValues()
	{
		this.mass = this.Rig().mass;
	}

	// Token: 0x04000404 RID: 1028
	internal float mass;

	// Token: 0x04000405 RID: 1029
	internal Rigidbody rig;

	// Token: 0x04000406 RID: 1030
	internal RigCreator rigCreator;
}
