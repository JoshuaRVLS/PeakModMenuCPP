using System;
using UnityEngine;

// Token: 0x02000283 RID: 643
public class IsLookedAt : MonoBehaviour
{
	// Token: 0x0600129B RID: 4763 RVA: 0x0005D550 File Offset: 0x0005B750
	private void Start()
	{
		if (this.characterInteractible.character == Character.localCharacter)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.index = GUIManager.instance.playerNames.Init(this.characterInteractible);
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x0005D59C File Offset: 0x0005B79C
	private void Update()
	{
		bool visible = false;
		float num = Vector3.Distance(MainCamera.instance.transform.position, base.transform.position);
		float num2 = Vector3.Angle(MainCamera.instance.transform.forward, base.transform.position - MainCamera.instance.transform.position);
		if (num < this.visibleDistance && num2 < this.visibleAngle + (this.visibleDistance - num) / this.visibleDistance * this.angleDistRatio)
		{
			visible = true;
		}
		if (this.mouth.character.data.isBlind)
		{
			visible = false;
		}
		GUIManager.instance.playerNames.UpdateName(this.index, this.playerNamePos.position, visible, this.mouth.amplitudeIndex);
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x0005D66F File Offset: 0x0005B86F
	private void OnDisable()
	{
		GUIManager.instance.playerNames.DisableName(this.index);
	}

	// Token: 0x040010AA RID: 4266
	public AnimatedMouth mouth;

	// Token: 0x040010AB RID: 4267
	public CharacterInteractible characterInteractible;

	// Token: 0x040010AC RID: 4268
	public float visibleDistance = 8f;

	// Token: 0x040010AD RID: 4269
	public float visibleAngle = 45f;

	// Token: 0x040010AE RID: 4270
	public float angleDistRatio = 45f;

	// Token: 0x040010AF RID: 4271
	public Transform playerNamePos;

	// Token: 0x040010B0 RID: 4272
	private int index;
}
