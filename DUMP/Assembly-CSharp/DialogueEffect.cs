using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public class DialogueEffect : MonoBehaviour
{
	// Token: 0x06000E7B RID: 3707 RVA: 0x00048AC6 File Offset: 0x00046CC6
	private void Awake()
	{
		this.m_TextComponent = base.GetComponent<TMP_Text>();
		this.DTanimator = new DOTweenTMPAnimator(this.m_TextComponent);
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x00048AE5 File Offset: 0x00046CE5
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x00048AED File Offset: 0x00046CED
	private void OnEnable()
	{
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x00048AEF File Offset: 0x00046CEF
	private void OnDisable()
	{
		this.TryDestroy();
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x00048AF7 File Offset: 0x00046CF7
	public virtual void Init()
	{
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x00048AF9 File Offset: 0x00046CF9
	private void TryDestroy()
	{
		this.destroyed = true;
		Object.Destroy(this);
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x00048B08 File Offset: 0x00046D08
	private void LateUpdate()
	{
		if (!this.destroyed)
		{
			this.EffectRoutine();
		}
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x00048B18 File Offset: 0x00046D18
	protected virtual void EffectRoutine()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
		if (characterCount == 0)
		{
			return;
		}
		for (int i = 0; i < characterCount; i++)
		{
			this.UpdateCharacter(i);
		}
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x00048B59 File Offset: 0x00046D59
	public virtual void UpdateCharacter(int index)
	{
	}

	// Token: 0x04000C30 RID: 3120
	protected TMP_Text m_TextComponent;

	// Token: 0x04000C31 RID: 3121
	protected TMP_TextInfo textInfo;

	// Token: 0x04000C32 RID: 3122
	public DOTweenTMPAnimator DTanimator;

	// Token: 0x04000C33 RID: 3123
	private bool destroyed;
}
