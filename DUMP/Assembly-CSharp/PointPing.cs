using System;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020002D7 RID: 727
public class PointPing : MonoBehaviour
{
	// Token: 0x1700014B RID: 331
	// (get) Token: 0x0600144F RID: 5199 RVA: 0x00066F54 File Offset: 0x00065154
	public Vector3 PingerForward
	{
		get
		{
			return (base.transform.position - this.pointPinger.character.Head).normalized;
		}
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x00066F89 File Offset: 0x00065189
	public void Init(Character character)
	{
		this.character = character;
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x00066F92 File Offset: 0x00065192
	private void Awake()
	{
		this.material = this.renderer.material;
	}

	// Token: 0x06001452 RID: 5202 RVA: 0x00066FA5 File Offset: 0x000651A5
	private void Start()
	{
		this.camera = Camera.main;
		this.Go();
		this.pingSound.Play(base.transform.position);
	}

	// Token: 0x06001453 RID: 5203 RVA: 0x00066FCE File Offset: 0x000651CE
	public void Update()
	{
		this.Go();
	}

	// Token: 0x06001454 RID: 5204 RVA: 0x00066FD8 File Offset: 0x000651D8
	private void Go()
	{
		float num = this.camera.SizeOfFrustumAtDistance(Vector3.Distance(Character.localCharacter.Center, base.transform.position));
		this.character.refs.animations.point = base.gameObject;
		num = this.minMaxScale.PClampFloat(num);
		base.transform.localScale = (num * this.sizeOfFrustum).xxx();
		Vector3 to = base.transform.position - this.camera.transform.position;
		float me = Vector3.Angle(this.PingerForward, to);
		Vector3 vector = Vector3.Lerp(-this.hitNormal, this.PingerForward, me.Remap(0f, this.angleThing, 0f, 1f));
		float me2 = Vector3.Angle(vector, to);
		Vector3 forward = Vector3.Lerp(-Vector3.up, vector, me2.Remap(0f, this.angleThing, 0f, 1f));
		base.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
	}

	// Token: 0x0400128A RID: 4746
	public float sizeOfFrustum = 0.1f;

	// Token: 0x0400128B RID: 4747
	public Vector2 minMaxScale = new Vector2(0.2f, 3f);

	// Token: 0x0400128C RID: 4748
	public Vector2 visibilityFullNoneNoLos = new Vector2(30f, 50f);

	// Token: 0x0400128D RID: 4749
	public float NoLosVisibilityMul = 0.5f;

	// Token: 0x0400128E RID: 4750
	public float angleThing = 90f;

	// Token: 0x0400128F RID: 4751
	public MeshRenderer renderer;

	// Token: 0x04001290 RID: 4752
	public SpriteRenderer ringRenderer;

	// Token: 0x04001291 RID: 4753
	public SFX_Instance pingSound;

	// Token: 0x04001292 RID: 4754
	public Material material;

	// Token: 0x04001293 RID: 4755
	public Vector3 hitNormal;

	// Token: 0x04001294 RID: 4756
	public PointPinger pointPinger;

	// Token: 0x04001295 RID: 4757
	private Camera camera;

	// Token: 0x04001296 RID: 4758
	private Character character;
}
