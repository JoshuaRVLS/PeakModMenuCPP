using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000222 RID: 546
public class BreakableRopeAnchor : MonoBehaviour
{
	// Token: 0x060010BD RID: 4285 RVA: 0x0005344A File Offset: 0x0005164A
	private void Awake()
	{
		this.anchor = base.GetComponent<RopeAnchorWithRope>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060010BE RID: 4286 RVA: 0x00053464 File Offset: 0x00051664
	private void Start()
	{
		this.willBreakInTime = this.breakableTimeMinMax.PRndRange();
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x00053478 File Offset: 0x00051678
	private void Update()
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		int num = 0;
		foreach (Character character in allPlayerCharacters)
		{
			if (character.data.isRopeClimbing && character.data.heldRope == this.anchor.rope)
			{
				num++;
			}
		}
		if (num > 0)
		{
			this.willBreakInTime -= Time.deltaTime;
		}
		if (this.willBreakInTime > 0f)
		{
			return;
		}
		if (this.isBreaking)
		{
			return;
		}
		base.StartCoroutine(this.<Update>g__Break|9_0());
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x0005356F File Offset: 0x0005176F
	[CompilerGenerated]
	private IEnumerator <Update>g__Break|9_0()
	{
		this.isBreaking = true;
		Debug.Log(string.Format("Break: segments {0}", this.anchor.rope.Segments));
		this.anchor.rope.Segments += this.dropSegments;
		float elapsed = 0f;
		float startSegments = this.anchor.rope.Segments;
		while (elapsed < this.breakAnimTime)
		{
			elapsed += Time.deltaTime;
			this.anchor.rope.Segments = Mathf.Lerp(startSegments, startSegments + 1f, elapsed / 0.5f);
			yield return null;
		}
		Debug.Log("Detach_Rpc");
		this.anchor.rope.photonView.RPC("Detach_Rpc", RpcTarget.AllBuffered, new object[]
		{
			this.anchor.rope.Segments
		});
		yield break;
	}

	// Token: 0x04000EBD RID: 3773
	public float breakAnimTime = 3f;

	// Token: 0x04000EBE RID: 3774
	public Vector2 breakableTimeMinMax = new Vector2(3f, 8f);

	// Token: 0x04000EBF RID: 3775
	public float dropSegments = 1f;

	// Token: 0x04000EC0 RID: 3776
	private float willBreakInTime;

	// Token: 0x04000EC1 RID: 3777
	private RopeAnchorWithRope anchor;

	// Token: 0x04000EC2 RID: 3778
	private PhotonView photonView;

	// Token: 0x04000EC3 RID: 3779
	private bool isBreaking;
}
