using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001C6 RID: 454
public class EmoteWheel : UIWheel
{
	// Token: 0x06000E85 RID: 3717 RVA: 0x00048B63 File Offset: 0x00046D63
	private void Start()
	{
		this.nextButton.onClick.AddListener(new UnityAction(this.TabNext));
		this.prevButton.onClick.AddListener(new UnityAction(this.TabPrev));
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x00048B9D File Offset: 0x00046D9D
	protected override void Update()
	{
		if (Character.localCharacter.input.selectSlotBackwardWasPressed)
		{
			this.Tab(-1);
		}
		else if (Character.localCharacter.input.selectSlotForwardWasPressed)
		{
			this.Tab(1);
		}
		base.Update();
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x00048BD7 File Offset: 0x00046DD7
	public void OnEnable()
	{
		this.InitWheel();
	}

	// Token: 0x06000E88 RID: 3720 RVA: 0x00048BDF File Offset: 0x00046DDF
	public void OnDisable()
	{
		this.Choose();
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x00048BE8 File Offset: 0x00046DE8
	public void InitWheel()
	{
		this.chosenEmoteData = null;
		for (int i = 0; i < this.slices.Length; i++)
		{
			int num = i + 8 * this.page;
			this.slices[i].Init(this.data[num], this);
		}
		this.selectedEmoteName.text = "";
		this.nextButton.gameObject.SetActive(this.page + 1 < this.pages);
		this.prevButton.gameObject.SetActive(this.page > 0);
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x00048C79 File Offset: 0x00046E79
	private void TabNext()
	{
		this.Tab(1);
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x00048C82 File Offset: 0x00046E82
	private void TabPrev()
	{
		this.Tab(-1);
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x00048C8B File Offset: 0x00046E8B
	private void Tab(int index)
	{
		this.page += index;
		this.page = Mathf.Clamp(this.page, 0, this.pages - 1);
		this.InitWheel();
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x00048CBB File Offset: 0x00046EBB
	public void Choose()
	{
		if (this.chosenEmoteData != null)
		{
			Character.localCharacter.refs.animations.PlayEmote(this.chosenEmoteData.anim);
		}
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x00048CEA File Offset: 0x00046EEA
	public void Hover(EmoteWheelData emoteWheelData)
	{
		this.selectedEmoteName.text = LocalizedText.GetText(emoteWheelData.emoteName, true);
		this.chosenEmoteData = emoteWheelData;
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x00048D0A File Offset: 0x00046F0A
	public void Dehover(EmoteWheelData emoteWheelData)
	{
		if (this.chosenEmoteData == emoteWheelData)
		{
			this.selectedEmoteName.text = "";
			this.chosenEmoteData = null;
		}
	}

	// Token: 0x06000E90 RID: 3728 RVA: 0x00048D34 File Offset: 0x00046F34
	protected override void TestSelectSliceGamepad(Vector2 gamepadVector)
	{
		float num = 0f;
		EmoteWheelSlice emoteWheelSlice = null;
		if (gamepadVector.sqrMagnitude >= 0.5f)
		{
			for (int i = 0; i < this.slices.Length; i++)
			{
				float num2 = Vector3.Angle(gamepadVector, this.slices[i].GetUpVector());
				if (emoteWheelSlice == null || num2 < num)
				{
					emoteWheelSlice = this.slices[i];
					num = num2;
				}
			}
		}
		if (emoteWheelSlice != null)
		{
			EventSystem.current.SetSelectedGameObject(emoteWheelSlice.button.gameObject);
			emoteWheelSlice.Hover();
			return;
		}
		EventSystem.current.SetSelectedGameObject(null);
		this.Dehover(this.chosenEmoteData);
	}

	// Token: 0x04000C34 RID: 3124
	public EmoteWheelSlice[] slices;

	// Token: 0x04000C35 RID: 3125
	public EmoteWheelData[] data;

	// Token: 0x04000C36 RID: 3126
	public TextMeshProUGUI selectedEmoteName;

	// Token: 0x04000C37 RID: 3127
	private EmoteWheelData chosenEmoteData;

	// Token: 0x04000C38 RID: 3128
	public Button nextButton;

	// Token: 0x04000C39 RID: 3129
	public Button prevButton;

	// Token: 0x04000C3A RID: 3130
	private int page;

	// Token: 0x04000C3B RID: 3131
	public int pages = 2;
}
