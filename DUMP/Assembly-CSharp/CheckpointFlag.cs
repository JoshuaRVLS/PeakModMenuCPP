using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class CheckpointFlag : MonoBehaviour
{
	// Token: 0x0600115C RID: 4444 RVA: 0x000576B0 File Offset: 0x000558B0
	public void Initialize(Character flagPlanterCharacter)
	{
		this.planterCharacter = flagPlanterCharacter;
		this.currentStatuses = new float[Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE)).Length];
		Array.Copy(flagPlanterCharacter.refs.afflictions.currentStatuses, this.currentStatuses, flagPlanterCharacter.refs.afflictions.currentStatuses.Length);
		this.planterCharacter.data.checkpointFlags.Add(this);
		base.transform.rotation = Quaternion.identity;
		base.Invoke("DisableAnim", 1f);
		if (base.TryGetComponent<PhotonView>(out this._networkView))
		{
			this._networkView.RPC("SetColor", RpcTarget.AllBuffered, new object[]
			{
				this.planterCharacter.refs.customization.PlayerColorAsVector
			});
			return;
		}
		Debug.LogWarning("Can't SetColor because " + base.name + " has no PhotonView", this);
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x000577A1 File Offset: 0x000559A1
	private void DisableAnim()
	{
		this.anim.enabled = false;
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x000577B0 File Offset: 0x000559B0
	[PunRPC]
	public void SetColor(Vector3 c)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetColor(CheckpointFlag.FlagColorPropertyId, new Color(c.x, c.y, c.z));
		this.flagRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x000577F1 File Offset: 0x000559F1
	public void DestroySelf()
	{
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x000577FE File Offset: 0x000559FE
	private void OnDisable()
	{
		if (this.planterCharacter != null)
		{
			this.planterCharacter.data.checkpointFlags.Remove(this);
		}
		Debug.Log("Checkpoint Flag Disabled");
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x00057830 File Offset: 0x00055A30
	private Color MaxSatVal(Color input)
	{
		float h;
		float s;
		float v;
		Color.RGBToHSV(input, out h, out s, out v);
		s = 1f;
		v = 1f;
		return Color.HSVToRGB(h, s, v);
	}

	// Token: 0x04000F2B RID: 3883
	private static readonly int FlagColorPropertyId = Shader.PropertyToID("_BaseColor");

	// Token: 0x04000F2C RID: 3884
	private PhotonView _networkView;

	// Token: 0x04000F2D RID: 3885
	[HideInInspector]
	public float[] currentStatuses;

	// Token: 0x04000F2E RID: 3886
	[HideInInspector]
	public Character planterCharacter;

	// Token: 0x04000F2F RID: 3887
	public Renderer flagRenderer;

	// Token: 0x04000F30 RID: 3888
	public Animator anim;
}
