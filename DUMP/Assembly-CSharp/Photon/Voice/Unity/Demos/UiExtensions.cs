using System;
using Photon.Voice.Unity.Demos.DemoVoiceUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	// Token: 0x0200039D RID: 925
	public static class UiExtensions
	{
		// Token: 0x06001869 RID: 6249 RVA: 0x0007BFDA File Offset: 0x0007A1DA
		public static void SetPosX(this RectTransform rectTransform, float x)
		{
			rectTransform.anchoredPosition3D = new Vector3(x, rectTransform.anchoredPosition3D.y, rectTransform.anchoredPosition3D.z);
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x0007BFFE File Offset: 0x0007A1FE
		public static void SetHeight(this RectTransform rectTransform, float h)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x0007C008 File Offset: 0x0007A208
		public static void SetValue(this Toggle toggle, bool isOn)
		{
			toggle.SetIsOnWithoutNotify(isOn);
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x0007C011 File Offset: 0x0007A211
		public static void SetValue(this Slider slider, float v)
		{
			slider.SetValueWithoutNotify(v);
		}

		// Token: 0x0600186D RID: 6253 RVA: 0x0007C01A File Offset: 0x0007A21A
		public static void SetValue(this InputField inputField, string v)
		{
			inputField.SetTextWithoutNotify(v);
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x0007C024 File Offset: 0x0007A224
		public static void DestroyChildren(this Transform transform)
		{
			if (null != transform && transform)
			{
				for (int i = transform.childCount - 1; i >= 0; i--)
				{
					Transform child = transform.GetChild(i);
					if (child && child.gameObject)
					{
						Object.Destroy(child.gameObject);
					}
				}
				transform.DetachChildren();
			}
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x0007C083 File Offset: 0x0007A283
		public static void Hide(this CanvasGroup canvasGroup, bool blockRaycasts = false, bool interactable = false)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.blocksRaycasts = blockRaycasts;
			canvasGroup.interactable = interactable;
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x0007C09E File Offset: 0x0007A29E
		public static void Show(this CanvasGroup canvasGroup, bool blockRaycasts = true, bool interactable = true)
		{
			canvasGroup.alpha = 1f;
			canvasGroup.blocksRaycasts = blockRaycasts;
			canvasGroup.interactable = interactable;
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x0007C0B9 File Offset: 0x0007A2B9
		public static bool IsHidden(this CanvasGroup canvasGroup)
		{
			return canvasGroup.alpha <= 0f;
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x0007C0CB File Offset: 0x0007A2CB
		public static bool IsShown(this CanvasGroup canvasGroup)
		{
			return canvasGroup.alpha > 0f;
		}

		// Token: 0x06001873 RID: 6259 RVA: 0x0007C0DA File Offset: 0x0007A2DA
		public static void SetSingleOnClickCallback(this Button button, UnityAction action)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(action);
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x0007C0F3 File Offset: 0x0007A2F3
		public static void SetSingleOnValueChangedCallback(this Toggle toggle, UnityAction<bool> action)
		{
			toggle.onValueChanged.RemoveAllListeners();
			toggle.onValueChanged.AddListener(action);
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x0007C10C File Offset: 0x0007A30C
		public static void SetSingleOnValueChangedCallback(this InputField inputField, UnityAction<string> action)
		{
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(action);
		}

		// Token: 0x06001876 RID: 6262 RVA: 0x0007C125 File Offset: 0x0007A325
		public static void SetSingleOnEndEditCallback(this InputField inputField, UnityAction<string> action)
		{
			inputField.onEndEdit.RemoveAllListeners();
			inputField.onEndEdit.AddListener(action);
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x0007C13E File Offset: 0x0007A33E
		public static void SetSingleOnValueChangedCallback(this Dropdown inputField, UnityAction<int> action)
		{
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(action);
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x0007C157 File Offset: 0x0007A357
		public static void SetSingleOnValueChangedCallback(this Slider slider, UnityAction<float> action)
		{
			slider.onValueChanged.RemoveAllListeners();
			slider.onValueChanged.AddListener(action);
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x0007C170 File Offset: 0x0007A370
		public static void SetSingleOnValueChangedCallback(this MicrophoneSelector selector, UnityAction<MicType, DeviceInfo> action)
		{
			selector.onValueChanged.RemoveAllListeners();
			selector.onValueChanged.AddListener(action);
		}
	}
}
