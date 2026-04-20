using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000208 RID: 520
public class Antlion : MonoBehaviour
{
	// Token: 0x0600101C RID: 4124 RVA: 0x0004F8E4 File Offset: 0x0004DAE4
	private void Start()
	{
		if (RunSettings.GetValue(RunSettings.SETTINGTYPE.Hazard_Antlion, false) == 0)
		{
			base.enabled = false;
			return;
		}
		this.view = base.GetComponent<PhotonView>();
		this.anim = base.GetComponentInChildren<Animator>(true);
		this.collisionModifier = base.GetComponentInChildren<CollisionModifier>();
		this.climbModifierSurface = base.GetComponentInChildren<ClimbModifierSurface>();
		this.climbModifierSurface.alwaysClimbableRange = 16f;
		this.collisionModifier.standableRange = 16f;
		this.head = base.transform.Find("Head").gameObject;
		this.slideObj = base.transform.Find("Hill/Slide").gameObject;
		this.slideMat = this.slideObj.GetComponent<MeshRenderer>().material;
		this.slideMat.SetFloat("_Str", this.str);
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x0004F9B9 File Offset: 0x0004DBB9
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.escapeRadiusForAchievement);
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x0004F9DC File Offset: 0x0004DBDC
	private void Update()
	{
		this.TestAchievement();
		if (!this.attacking)
		{
			this.attackCounter += Time.deltaTime;
		}
		this.GetClosestTarget();
		if (this.closestTarget != null)
		{
			this.DoActive();
			this.activeFor += Time.deltaTime;
			return;
		}
		this.DoInactive();
		this.activeFor = 0f;
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x0004FA48 File Offset: 0x0004DC48
	private void DoActive()
	{
		if (this.str > 0.95f)
		{
			this.collisionModifier.hasStandableRange = true;
			this.climbModifierSurface.hasAlwaysClimbableRange = true;
		}
		if (Vector3.Distance(base.transform.position, this.closestTarget.Center) < 5f && this.attackCounter > 0.15f && this.view.IsMine)
		{
			this.attackCounter = 0f;
			this.view.RPC("RPCA_Attack", RpcTarget.All, new object[]
			{
				this.closestTarget.refs.view.ViewID
			});
		}
		this.ActiveVisuals();
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x0004FAFB File Offset: 0x0004DCFB
	[PunRPC]
	public void RPCA_Attack(int targetID)
	{
		this.Attack(PhotonView.Find(targetID).GetComponent<Character>());
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x0004FB10 File Offset: 0x0004DD10
	private void Attack(Character target)
	{
		Antlion.<>c__DisplayClass19_0 CS$<>8__locals1 = new Antlion.<>c__DisplayClass19_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.target = target;
		base.StartCoroutine(CS$<>8__locals1.<Attack>g__IAttack|0());
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x0004FB3E File Offset: 0x0004DD3E
	private void SinkLuggage()
	{
		this.luggage.transform.DOLocalMoveY(-2f, 1f, false);
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x0004FB5C File Offset: 0x0004DD5C
	private void GetClosestTarget()
	{
		float num = 12f;
		if (this.closestTarget != null)
		{
			num = 16f;
		}
		float num2 = num;
		Character character = null;
		foreach (Character character2 in Character.AllCharacters)
		{
			float num3 = Vector3.Distance(base.transform.position, character2.Center);
			if (num3 < num)
			{
				character2.ClampSinceGrounded(1f + Mathf.Clamp01(this.activeFor / 3f));
			}
			if (num3 < num2)
			{
				num2 = num3;
				character = character2;
			}
		}
		if (character != this.closestTarget && this.view.IsMine)
		{
			this.view.RPC("RPCA_SetClosestTarget", RpcTarget.All, new object[]
			{
				(character != null) ? character.refs.view.ViewID : -1
			});
		}
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x0004FC64 File Offset: 0x0004DE64
	[PunRPC]
	private void RPCA_SetClosestTarget(int targetID)
	{
		if (targetID == -1)
		{
			this.closestTarget = null;
			return;
		}
		this.closestTarget = PhotonView.Find(targetID).GetComponent<Character>();
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x0004FC83 File Offset: 0x0004DE83
	private void DoInactive()
	{
		this.collisionModifier.hasStandableRange = false;
		this.climbModifierSurface.hasAlwaysClimbableRange = false;
		this.InactiveVisuals();
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x0004FCA4 File Offset: 0x0004DEA4
	private void ActiveVisuals()
	{
		if (!this.firstActivation)
		{
			this.firstActivation = true;
			this.SinkLuggage();
		}
		if (this.anim.gameObject.activeInHierarchy)
		{
			this.anim.SetBool("Active", true);
		}
		if (!this.slideObj.activeSelf)
		{
			this.slideObj.SetActive(true);
			this.head.SetActive(true);
		}
		if (this.closestTarget)
		{
			this.head.transform.rotation = Quaternion.Lerp(this.head.transform.rotation, Quaternion.LookRotation(this.closestTarget.Center - this.head.transform.position), Time.deltaTime * 2f);
		}
		this.str = Mathf.MoveTowards(this.str, 1f, Time.deltaTime);
		this.slideMat.SetFloat("_Str", this.str);
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x0004FDA4 File Offset: 0x0004DFA4
	private void InactiveVisuals()
	{
		if (this.anim.gameObject.activeInHierarchy)
		{
			this.anim.SetBool("Active", false);
		}
		this.str = Mathf.MoveTowards(this.str, 0f, Time.deltaTime);
		if (this.slideObj.activeSelf)
		{
			if (this.str < 0.01f)
			{
				this.slideObj.SetActive(false);
				this.head.SetActive(false);
			}
			this.slideMat.SetFloat("_Str", this.str);
		}
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x0004FE38 File Offset: 0x0004E038
	private void TestAchievement()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		if (this.bitLocalPlayer)
		{
			if (Character.localCharacter.data.dead)
			{
				this.bitLocalPlayer = false;
				return;
			}
			if (Character.localCharacter.data.fullyConscious && Vector3.Distance(base.transform.position, Character.localCharacter.Center) > this.escapeRadiusForAchievement)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.MegaentomologyBadge);
				this.bitLocalPlayer = false;
			}
		}
	}

	// Token: 0x04000DF3 RID: 3571
	private GameObject head;

	// Token: 0x04000DF4 RID: 3572
	private Material slideMat;

	// Token: 0x04000DF5 RID: 3573
	private GameObject slideObj;

	// Token: 0x04000DF6 RID: 3574
	private CollisionModifier collisionModifier;

	// Token: 0x04000DF7 RID: 3575
	private ClimbModifierSurface climbModifierSurface;

	// Token: 0x04000DF8 RID: 3576
	private float str;

	// Token: 0x04000DF9 RID: 3577
	private float activeFor;

	// Token: 0x04000DFA RID: 3578
	private Animator anim;

	// Token: 0x04000DFB RID: 3579
	private PhotonView view;

	// Token: 0x04000DFC RID: 3580
	private bool bitLocalPlayer;

	// Token: 0x04000DFD RID: 3581
	public float escapeRadiusForAchievement;

	// Token: 0x04000DFE RID: 3582
	private Character closestTarget;

	// Token: 0x04000DFF RID: 3583
	private float attackCounter;

	// Token: 0x04000E00 RID: 3584
	private bool attacking;

	// Token: 0x04000E01 RID: 3585
	public GameObject luggage;

	// Token: 0x04000E02 RID: 3586
	private bool firstActivation;
}
