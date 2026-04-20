using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x02000266 RID: 614
public class FakeCursor : MonoBehaviour
{
	// Token: 0x0600122B RID: 4651 RVA: 0x0005B15C File Offset: 0x0005935C
	private void Update()
	{
		Vector2 screenPoint = Mouse.current.position.ReadValue();
		Vector2 v;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.target.parent as RectTransform, screenPoint, null, out v);
		this.target.localPosition = v;
	}

	// Token: 0x04001035 RID: 4149
	public Transform target;
}
