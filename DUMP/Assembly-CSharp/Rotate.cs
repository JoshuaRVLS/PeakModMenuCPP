using System;
using UnityEngine;

// Token: 0x02000326 RID: 806
public class Rotate : MonoBehaviour
{
	// Token: 0x06001579 RID: 5497 RVA: 0x0006CC6D File Offset: 0x0006AE6D
	private void Update()
	{
		this.tf.transform.Rotate(this.rotation * Time.deltaTime);
	}

	// Token: 0x040013A3 RID: 5027
	public Transform tf;

	// Token: 0x040013A4 RID: 5028
	public Vector3 rotation;
}
