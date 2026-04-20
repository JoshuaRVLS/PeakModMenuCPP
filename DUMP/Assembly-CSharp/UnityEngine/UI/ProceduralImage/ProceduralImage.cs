using System;
using UnityEngine.Events;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x02000385 RID: 901
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Procedural Image")]
	public class ProceduralImage : Image
	{
		// Token: 0x1700016E RID: 366
		// (get) Token: 0x0600177C RID: 6012 RVA: 0x00079149 File Offset: 0x00077349
		// (set) Token: 0x0600177D RID: 6013 RVA: 0x00079171 File Offset: 0x00077371
		private static Material DefaultProceduralImageMaterial
		{
			get
			{
				if (ProceduralImage.materialInstance == null)
				{
					ProceduralImage.materialInstance = new Material(Shader.Find("UI/Procedural UI Image"));
				}
				return ProceduralImage.materialInstance;
			}
			set
			{
				ProceduralImage.materialInstance = value;
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x0600177E RID: 6014 RVA: 0x00079179 File Offset: 0x00077379
		// (set) Token: 0x0600177F RID: 6015 RVA: 0x00079181 File Offset: 0x00077381
		public float BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				this.borderWidth = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06001780 RID: 6016 RVA: 0x00079190 File Offset: 0x00077390
		// (set) Token: 0x06001781 RID: 6017 RVA: 0x00079198 File Offset: 0x00077398
		public float FalloffDistance
		{
			get
			{
				return this.falloffDistance;
			}
			set
			{
				this.falloffDistance = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06001782 RID: 6018 RVA: 0x000791A7 File Offset: 0x000773A7
		// (set) Token: 0x06001783 RID: 6019 RVA: 0x000791E7 File Offset: 0x000773E7
		protected ProceduralImageModifier Modifier
		{
			get
			{
				if (this.modifier == null)
				{
					this.modifier = base.GetComponent<ProceduralImageModifier>();
					if (this.modifier == null)
					{
						this.ModifierType = typeof(FreeModifier);
					}
				}
				return this.modifier;
			}
			set
			{
				this.modifier = value;
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06001784 RID: 6020 RVA: 0x000791F0 File Offset: 0x000773F0
		// (set) Token: 0x06001785 RID: 6021 RVA: 0x00079200 File Offset: 0x00077400
		public System.Type ModifierType
		{
			get
			{
				return this.Modifier.GetType();
			}
			set
			{
				if (this.modifier != null && this.modifier.GetType() != value)
				{
					if (base.GetComponent<ProceduralImageModifier>() != null)
					{
						Object.DestroyImmediate(base.GetComponent<ProceduralImageModifier>());
					}
					base.gameObject.AddComponent(value);
					this.Modifier = base.GetComponent<ProceduralImageModifier>();
					this.SetAllDirty();
					return;
				}
				if (this.modifier == null)
				{
					base.gameObject.AddComponent(value);
					this.Modifier = base.GetComponent<ProceduralImageModifier>();
					this.SetAllDirty();
				}
			}
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x00079294 File Offset: 0x00077494
		protected override void OnEnable()
		{
			base.OnEnable();
			this.Init();
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x000792A2 File Offset: 0x000774A2
		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_OnDirtyVertsCallback = (UnityAction)Delegate.Remove(this.m_OnDirtyVertsCallback, new UnityAction(this.OnVerticesDirty));
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x000792CC File Offset: 0x000774CC
		private void Init()
		{
			this.FixTexCoordsInCanvas();
			this.m_OnDirtyVertsCallback = (UnityAction)Delegate.Combine(this.m_OnDirtyVertsCallback, new UnityAction(this.OnVerticesDirty));
			base.preserveAspect = false;
			this.material = null;
			if (base.sprite == null)
			{
				base.sprite = EmptySprite.Get();
			}
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x00079328 File Offset: 0x00077528
		protected void OnVerticesDirty()
		{
			if (base.sprite == null)
			{
				base.sprite = EmptySprite.Get();
			}
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x00079344 File Offset: 0x00077544
		protected void FixTexCoordsInCanvas()
		{
			Canvas componentInParent = base.GetComponentInParent<Canvas>();
			if (componentInParent != null)
			{
				this.FixTexCoordsInCanvas(componentInParent);
			}
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x00079368 File Offset: 0x00077568
		protected void FixTexCoordsInCanvas(Canvas c)
		{
			c.additionalShaderChannels |= (AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.TexCoord3);
		}

		// Token: 0x0600178C RID: 6028 RVA: 0x00079378 File Offset: 0x00077578
		private Vector4 FixRadius(Vector4 vec)
		{
			Rect rect = base.rectTransform.rect;
			vec = new Vector4(Mathf.Max(vec.x, 0f), Mathf.Max(vec.y, 0f), Mathf.Max(vec.z, 0f), Mathf.Max(vec.w, 0f));
			float d = Mathf.Min(Mathf.Min(Mathf.Min(Mathf.Min(rect.width / (vec.x + vec.y), rect.width / (vec.z + vec.w)), rect.height / (vec.x + vec.w)), rect.height / (vec.z + vec.y)), 1f);
			return vec * d;
		}

		// Token: 0x0600178D RID: 6029 RVA: 0x0007944D File Offset: 0x0007764D
		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			base.OnPopulateMesh(toFill);
			this.EncodeAllInfoIntoVertices(toFill, this.CalculateInfo());
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x00079463 File Offset: 0x00077663
		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this.FixTexCoordsInCanvas();
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x00079474 File Offset: 0x00077674
		private ProceduralImageInfo CalculateInfo()
		{
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			float pixelSize = 1f / Mathf.Max(0f, this.falloffDistance);
			Vector4 a = this.FixRadius(this.Modifier.CalculateRadius(pixelAdjustedRect));
			float num = Mathf.Min(pixelAdjustedRect.width, pixelAdjustedRect.height);
			return new ProceduralImageInfo(pixelAdjustedRect.width + this.falloffDistance, pixelAdjustedRect.height + this.falloffDistance, this.falloffDistance, pixelSize, a / num, this.borderWidth / num * 2f);
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x00079504 File Offset: 0x00077704
		private void EncodeAllInfoIntoVertices(VertexHelper vh, ProceduralImageInfo info)
		{
			UIVertex uivertex = default(UIVertex);
			Vector2 v = new Vector2(info.width, info.height);
			Vector2 v2 = new Vector2(this.EncodeFloats_0_1_16_16(info.radius.x, info.radius.y), this.EncodeFloats_0_1_16_16(info.radius.z, info.radius.w));
			Vector2 v3 = new Vector2((info.borderWidth == 0f) ? 1f : Mathf.Clamp01(info.borderWidth), info.pixelSize);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref uivertex, i);
				uivertex.position += (uivertex.uv0 - new Vector3(0.5f, 0.5f)) * info.fallOffDistance;
				uivertex.uv1 = v;
				uivertex.uv2 = v2;
				uivertex.uv3 = v3;
				vh.SetUIVertex(uivertex, i);
			}
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x00079630 File Offset: 0x00077830
		private float EncodeFloats_0_1_16_16(float a, float b)
		{
			Vector2 rhs = new Vector2(1f, 1.5259022E-05f);
			return Vector2.Dot(new Vector2(Mathf.Floor(a * 65534f) / 65535f, Mathf.Floor(b * 65534f) / 65535f), rhs);
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06001792 RID: 6034 RVA: 0x0007967D File Offset: 0x0007787D
		// (set) Token: 0x06001793 RID: 6035 RVA: 0x00079699 File Offset: 0x00077899
		public override Material material
		{
			get
			{
				if (this.m_Material == null)
				{
					return ProceduralImage.DefaultProceduralImageMaterial;
				}
				return base.material;
			}
			set
			{
				base.material = value;
			}
		}

		// Token: 0x040015F4 RID: 5620
		[SerializeField]
		private float borderWidth;

		// Token: 0x040015F5 RID: 5621
		private ProceduralImageModifier modifier;

		// Token: 0x040015F6 RID: 5622
		private static Material materialInstance;

		// Token: 0x040015F7 RID: 5623
		[SerializeField]
		private float falloffDistance = 1f;
	}
}
