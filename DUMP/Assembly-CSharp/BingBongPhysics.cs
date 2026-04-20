using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000219 RID: 537
[DefaultExecutionOrder(1000001)]
public class BingBongPhysics : MonoBehaviour
{
	// Token: 0x06001071 RID: 4209 RVA: 0x00051C1E File Offset: 0x0004FE1E
	private void OnEnable()
	{
		this.bingBongPowers = base.GetComponent<BingBongPowers>();
		this.bingBongPowers.SetTexts("PHYSICS", this.descr);
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x00051C42 File Offset: 0x0004FE42
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x00051C50 File Offset: 0x0004FE50
	private void Update()
	{
		this.CheckInuput();
		float cd = this.GetCD();
		bool auto = this.GetAuto();
		this.counter += Time.unscaledDeltaTime;
		if (this.counter < cd)
		{
			return;
		}
		if (auto && !Input.GetKey(KeyCode.Mouse0))
		{
			return;
		}
		if (!auto && !Input.GetKeyDown(KeyCode.Mouse0))
		{
			return;
		}
		this.DoEffect();
		this.counter = 0f;
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x00051CC0 File Offset: 0x0004FEC0
	private void DoEffect()
	{
		PhotonNetwork.Instantiate(this.GetEffect().name, base.transform.position, base.transform.rotation, 0, null).GetComponent<PhotonView>().RPC("RPCA_BingBongInitObj", RpcTarget.All, new object[]
		{
			this.view.ViewID
		});
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x00051D20 File Offset: 0x0004FF20
	private GameObject GetEffect()
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return this.effect_Blow;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return this.effect_Suck;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return this.effect_Push;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return this.effect_Push_Gentle;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForceGrab)
		{
			return this.effect_Grab;
		}
		return null;
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x00051D7D File Offset: 0x0004FF7D
	private bool GetAuto()
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return true;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return true;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return false;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return false;
		}
		BingBongPhysics.PhysicsType physicsType = this.physicsType;
		return true;
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x00051DB4 File Offset: 0x0004FFB4
	private float GetCD()
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return 0.25f;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return 0.25f;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return 0f;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return 0f;
		}
		BingBongPhysics.PhysicsType physicsType = this.physicsType;
		return 0.25f;
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x00051E0C File Offset: 0x0005000C
	private void CheckInuput()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			this.SetState(BingBongPhysics.PhysicsType.Blow);
		}
		if (Input.GetKeyDown(KeyCode.T))
		{
			this.SetState(BingBongPhysics.PhysicsType.Suck);
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			this.SetState(BingBongPhysics.PhysicsType.ForceGrab);
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			this.SetState(BingBongPhysics.PhysicsType.ForcePush);
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			this.SetState(BingBongPhysics.PhysicsType.ForcePush_Gentle);
		}
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x00051E69 File Offset: 0x00050069
	private void SetState(BingBongPhysics.PhysicsType setType)
	{
		this.physicsType = setType;
		this.bingBongPowers.SetTip(setType.ToString(), 0);
	}

	// Token: 0x04000E62 RID: 3682
	public BingBongPhysics.PhysicsType physicsType;

	// Token: 0x04000E63 RID: 3683
	private PhotonView view;

	// Token: 0x04000E64 RID: 3684
	private BingBongPowers bingBongPowers;

	// Token: 0x04000E65 RID: 3685
	private string descr = "Blow: [R]\n\nSuck: [T]\n\nForce Grab: [F]\n\nForce Push: [C]\n\nForce Push Gentle: [V]";

	// Token: 0x04000E66 RID: 3686
	private float counter;

	// Token: 0x04000E67 RID: 3687
	public GameObject effect_Blow;

	// Token: 0x04000E68 RID: 3688
	public GameObject effect_Suck;

	// Token: 0x04000E69 RID: 3689
	public GameObject effect_Push;

	// Token: 0x04000E6A RID: 3690
	public GameObject effect_Push_Gentle;

	// Token: 0x04000E6B RID: 3691
	public GameObject effect_Grab;

	// Token: 0x020004F6 RID: 1270
	public enum PhysicsType
	{
		// Token: 0x04001BC2 RID: 7106
		Blow,
		// Token: 0x04001BC3 RID: 7107
		Suck,
		// Token: 0x04001BC4 RID: 7108
		ForcePush,
		// Token: 0x04001BC5 RID: 7109
		ForcePush_Gentle,
		// Token: 0x04001BC6 RID: 7110
		ForceGrab
	}
}
