using System;
using TMPro;
using UnityEngine;

// Token: 0x02000237 RID: 567
[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class CircularTextMeshPro : MonoBehaviour
{
	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06001172 RID: 4466 RVA: 0x00058050 File Offset: 0x00056250
	// (set) Token: 0x06001173 RID: 4467 RVA: 0x00058058 File Offset: 0x00056258
	[Tooltip("The radius of the text circle arc")]
	public float Radius
	{
		get
		{
			return this.m_radius;
		}
		set
		{
			this.m_radius = value;
			this.OnCurvePropertyChanged();
		}
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x00058067 File Offset: 0x00056267
	private void Awake()
	{
		this.m_TextComponent = base.gameObject.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x0005807A File Offset: 0x0005627A
	private void OnEnable()
	{
		this.m_TextComponent.OnPreRenderText += this.UpdateTextCurve;
		this.OnCurvePropertyChanged();
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x00058099 File Offset: 0x00056299
	private void OnDisable()
	{
		this.m_TextComponent.OnPreRenderText -= this.UpdateTextCurve;
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x000580B2 File Offset: 0x000562B2
	protected void OnCurvePropertyChanged()
	{
		this.UpdateTextCurve(this.m_TextComponent.textInfo);
		this.m_TextComponent.ForceMeshUpdate(false, false);
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x000580D4 File Offset: 0x000562D4
	protected void UpdateTextCurve(TMP_TextInfo textInfo)
	{
		for (int i = 0; i < textInfo.characterInfo.Length; i++)
		{
			if (textInfo.characterInfo[i].isVisible)
			{
				int vertexIndex = textInfo.characterInfo[i].vertexIndex;
				int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
				Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
				Vector3 vector = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, textInfo.characterInfo[i].baseLine);
				vertices[vertexIndex] += -vector;
				vertices[vertexIndex + 1] += -vector;
				vertices[vertexIndex + 2] += -vector;
				vertices[vertexIndex + 3] += -vector;
				Matrix4x4 matrix4x = this.ComputeTransformationMatrix(vector, textInfo, i);
				vertices[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex]);
				vertices[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 1]);
				vertices[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 2]);
				vertices[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 3]);
			}
		}
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x00058270 File Offset: 0x00056470
	protected Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, TMP_TextInfo textInfo, int charIdx)
	{
		float num = this.m_radius + textInfo.lineInfo[textInfo.characterInfo[charIdx].lineNumber].baseline;
		float num2 = 2f * num * 3.1415927f;
		float f = ((charMidBaselinePos.x / num2 - 0.5f) * 360f + 90f) * 0.017453292f;
		float num3 = Mathf.Cos(f);
		float num4 = Mathf.Sin(f);
		Vector2 vector = new Vector2(num3 * num, -num4 * num);
		float angle = -Mathf.Atan2(num4, num3) * 57.29578f - 90f;
		return Matrix4x4.TRS(new Vector3(vector.x, vector.y, 0f), Quaternion.AngleAxis(angle, Vector3.forward), Vector3.one);
	}

	// Token: 0x04000F49 RID: 3913
	private TextMeshProUGUI m_TextComponent;

	// Token: 0x04000F4A RID: 3914
	[SerializeField]
	[HideInInspector]
	private float m_radius = 10f;
}
