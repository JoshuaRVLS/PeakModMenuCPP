using System;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class FRILerp : MonoBehaviour
{
	// Token: 0x06001260 RID: 4704 RVA: 0x0005C5FD File Offset: 0x0005A7FD
	private void Start()
	{
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x0005C5FF File Offset: 0x0005A7FF
	public static Vector3 Lerp(Vector3 from, Vector3 target, float speed, bool useTimeScale = true)
	{
		return Vector3.Lerp(from, target, 1f - Mathf.Exp(-speed * (useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime)));
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x0005C625 File Offset: 0x0005A825
	public static Vector3 PLerp(Vector3 from, Vector3 target, float speed, float dt)
	{
		return Vector3.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x0005C63D File Offset: 0x0005A83D
	public static Quaternion PLerp(Quaternion from, Quaternion target, float speed, float dt)
	{
		return Quaternion.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x0005C655 File Offset: 0x0005A855
	public static float PLerp(float from, float target, float speed, float dt)
	{
		return Mathf.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
	}

	// Token: 0x06001265 RID: 4709 RVA: 0x0005C66D File Offset: 0x0005A86D
	public static Vector3 LerpFixed(Vector3 from, Vector3 target, float speed, bool useTimeScale = true)
	{
		return Vector3.Lerp(from, target, 1f - Mathf.Exp(-speed * (useTimeScale ? Time.fixedDeltaTime : Time.unscaledDeltaTime)));
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x0005C693 File Offset: 0x0005A893
	public static Vector3 LerpUnclamped(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x06001267 RID: 4711 RVA: 0x0005C6AF File Offset: 0x0005A8AF
	public static float Lerp(float from, float target, float speed, bool useTimeScale = true)
	{
		return Mathf.Lerp(from, target, 1f - Mathf.Exp(-speed * (useTimeScale ? Time.fixedDeltaTime : Time.unscaledDeltaTime)));
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x0005C6D5 File Offset: 0x0005A8D5
	public static float LerpUnclamped(float from, float target, float speed)
	{
		return Mathf.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x0005C6F1 File Offset: 0x0005A8F1
	public static Vector3 Slerp(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.Slerp(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x0600126A RID: 4714 RVA: 0x0005C70D File Offset: 0x0005A90D
	public static Vector3 SlerpUnclamped(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.SlerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x0600126B RID: 4715 RVA: 0x0005C729 File Offset: 0x0005A929
	public static Quaternion Lerp(Quaternion from, Quaternion target, float speed)
	{
		return Quaternion.Lerp(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x0600126C RID: 4716 RVA: 0x0005C745 File Offset: 0x0005A945
	public static Quaternion LerpUnclamped(Quaternion from, Quaternion target, float speed)
	{
		return Quaternion.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}
}
