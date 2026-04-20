using System;
using UnityEngine;

// Token: 0x02000330 RID: 816
public class SegTest : MonoBehaviour
{
	// Token: 0x060015C1 RID: 5569 RVA: 0x0006E74C File Offset: 0x0006C94C
	private void Start()
	{
		ConfigurableJoint component = base.transform.GetChild(0).GetComponent<ConfigurableJoint>();
		this.joint2 = base.transform.GetChild(1).GetComponent<ConfigurableJoint>();
		this.joint2.connectedBody = component.GetComponent<Rigidbody>();
	}

	// Token: 0x060015C2 RID: 5570 RVA: 0x0006E793 File Offset: 0x0006C993
	private void Update()
	{
		this.joint2.connectedAnchor = new Vector3(0f, Mathf.Lerp(0.5f, -0.5f, this.val), 0f);
	}

	// Token: 0x040013CD RID: 5069
	[Range(0f, 1f)]
	public float val;

	// Token: 0x040013CE RID: 5070
	private ConfigurableJoint joint2;
}
