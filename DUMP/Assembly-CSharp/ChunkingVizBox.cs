using System;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class ChunkingVizBox : MonoBehaviour
{
	// Token: 0x0600055F RID: 1375 RVA: 0x0001FEAD File Offset: 0x0001E0AD
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(base.transform.position, base.transform.localScale);
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x0001FED4 File Offset: 0x0001E0D4
	private void LateUpdate()
	{
		Vector3 position = MainCamera.instance.transform.position;
		Bounds bounds = new Bounds(base.transform.position, base.transform.localScale);
		bool flag = bounds.Contains(position);
		if (this.m_lastState != flag)
		{
			GameObject[] array = this.objects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
		}
		this.m_lastState = flag;
	}

	// Token: 0x0400059B RID: 1435
	public GameObject[] objects;

	// Token: 0x0400059C RID: 1436
	private bool m_lastState = true;
}
