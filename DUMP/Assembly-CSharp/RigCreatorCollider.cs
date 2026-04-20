using System;
using UnityEngine;

// Token: 0x02000037 RID: 55
[ExecuteInEditMode]
public class RigCreatorCollider : MonoBehaviour
{
	// Token: 0x060003A7 RID: 935 RVA: 0x00018C6D File Offset: 0x00016E6D
	private void Start()
	{
		if (this.disableOnStart)
		{
			this.Col().enabled = false;
			return;
		}
		base.GetComponentInParent<CharacterRagdoll>().colliderList.Add(this.Col());
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x00018C9A File Offset: 0x00016E9A
	private void Awake()
	{
		if (this.IsEditor())
		{
			this.SetValues();
			return;
		}
		this.RegisterCollider();
		this.Col();
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x00018CB8 File Offset: 0x00016EB8
	private void RegisterCollider()
	{
		base.transform.parent.GetComponent<Bodypart>().RegisterCollider(this);
	}

	// Token: 0x060003AA RID: 938 RVA: 0x00018CD0 File Offset: 0x00016ED0
	private bool IsEditor()
	{
		return Application.isEditor && !Application.isPlaying;
	}

	// Token: 0x060003AB RID: 939 RVA: 0x00018CE3 File Offset: 0x00016EE3
	private void OnDestroy()
	{
		if (!this.IsEditor())
		{
			return;
		}
		if (!this.RigCreator())
		{
			return;
		}
		this.RigCreator().RemoveCollider(this);
	}

	// Token: 0x060003AC RID: 940 RVA: 0x00018D08 File Offset: 0x00016F08
	internal CapsuleCollider Col()
	{
		if (!this.col)
		{
			this.col = base.GetComponent<CapsuleCollider>();
		}
		return this.col;
	}

	// Token: 0x060003AD RID: 941 RVA: 0x00018D29 File Offset: 0x00016F29
	private RigCreator RigCreator()
	{
		if (!this.rigCreator)
		{
			this.rigCreator = base.GetComponentInParent<RigCreator>();
		}
		return this.rigCreator;
	}

	// Token: 0x060003AE RID: 942 RVA: 0x00018D4A File Offset: 0x00016F4A
	private void Update()
	{
		if (this.IsEditor())
		{
			this.CheckEditorDataChanged();
		}
	}

	// Token: 0x060003AF RID: 943 RVA: 0x00018D5C File Offset: 0x00016F5C
	private void CheckEditorDataChanged()
	{
		if (this.position != base.transform.localPosition || this.rotation != base.transform.localRotation || this.scale != base.transform.localScale || this.height != this.Col().height || this.radius != this.Col().radius)
		{
			this.RigCreator().ColliderChanged(this, base.transform.localPosition, base.transform.localRotation, base.transform.localScale, this.height, this.radius);
			this.SetValues();
		}
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x00018E18 File Offset: 0x00017018
	private void SetValues()
	{
		this.position = base.transform.localPosition;
		this.rotation = base.transform.localRotation;
		this.scale = base.transform.localScale;
		this.height = this.Col().height;
		this.radius = this.Col().radius;
	}

	// Token: 0x040003F8 RID: 1016
	internal CapsuleCollider col;

	// Token: 0x040003F9 RID: 1017
	internal Vector3 position;

	// Token: 0x040003FA RID: 1018
	internal Quaternion rotation;

	// Token: 0x040003FB RID: 1019
	internal Vector3 scale;

	// Token: 0x040003FC RID: 1020
	internal float height;

	// Token: 0x040003FD RID: 1021
	internal float radius;

	// Token: 0x040003FE RID: 1022
	public bool disableOnStart;

	// Token: 0x040003FF RID: 1023
	internal RigCreator rigCreator;
}
