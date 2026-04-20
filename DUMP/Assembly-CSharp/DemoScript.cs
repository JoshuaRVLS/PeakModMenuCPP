using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002AA RID: 682
public class DemoScript : MonoBehaviour
{
	// Token: 0x0600134F RID: 4943 RVA: 0x00061AB8 File Offset: 0x0005FCB8
	private void Start()
	{
		this.originalRotation = base.transform.localRotation;
		Renderer component = this.LightBulb.GetComponent<Renderer>();
		if (Application.isPlaying)
		{
			component.sharedMaterial = component.material;
		}
		this.lightBulbMaterial = component.sharedMaterial;
	}

	// Token: 0x06001350 RID: 4944 RVA: 0x00061B01 File Offset: 0x0005FD01
	private void Update()
	{
		this.RotateMirror();
		this.MoveLightBulb();
		this.UpdateMouseLook();
		this.UpdateMovement();
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x00061B1B File Offset: 0x0005FD1B
	public void MirrorRecursionToggled()
	{
		this.ChangeMirrorRecursion();
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x00061B24 File Offset: 0x0005FD24
	public void ChangeMirrorRecursion()
	{
		foreach (GameObject gameObject in this.Mirrors)
		{
			gameObject.GetComponent<MirrorScript>().MirrorRecursion = this.RecursionToggle.isOn;
		}
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x00061B84 File Offset: 0x0005FD84
	private void UpdateMovement()
	{
		float num = 4f * Time.deltaTime;
		if (Input.GetKey(KeyCode.W))
		{
			base.transform.Translate(0f, 0f, num);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			base.transform.Translate(0f, 0f, -num);
		}
		if (Input.GetKey(KeyCode.A))
		{
			base.transform.Translate(-num, 0f, 0f);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			base.transform.Translate(num, 0f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			this.RecursionToggle.isOn = !this.RecursionToggle.isOn;
		}
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x00061C44 File Offset: 0x0005FE44
	private void RotateMirror()
	{
		GameObject gameObject = this.Mirrors[0];
		float num = gameObject.transform.rotation.eulerAngles.y;
		if (num > 65f && num < 100f)
		{
			this.rotationModifier = -this.rotationModifier;
			num -= 65f;
			gameObject.transform.Rotate(0f, -num, 0f);
			return;
		}
		if (num > 100f && num < 295f)
		{
			this.rotationModifier = -this.rotationModifier;
			num = 295f - num;
			gameObject.transform.Rotate(0f, num, 0f);
			return;
		}
		gameObject.transform.Rotate(0f, this.rotationModifier * Time.deltaTime * 20f, 0f);
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x00061D18 File Offset: 0x0005FF18
	private void MoveLightBulb()
	{
		float num = this.LightBulb.transform.position.x;
		if (num > 5f)
		{
			this.moveModifier = -this.moveModifier;
			num = 5f;
		}
		else if (num < -5f)
		{
			this.moveModifier = -this.moveModifier;
			num = -5f;
		}
		else
		{
			num += Time.deltaTime * this.moveModifier;
		}
		Light component = this.LightBulb.GetComponent<Light>();
		this.LightBulb.transform.position = new Vector3(num, this.LightBulb.transform.position.y, this.LightBulb.transform.position.z);
		float num2 = Mathf.Min(1f, component.intensity);
		this.lightBulbMaterial.SetColor("_EmissionColor", new Color(num2, num2, num2));
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x00061DFC File Offset: 0x0005FFFC
	private void UpdateMouseLook()
	{
		if (this.axes == DemoScript.RotationAxes.MouseXAndY)
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
			this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
			this.rotationX = DemoScript.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
			this.rotationY = DemoScript.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
			Quaternion rhs = Quaternion.AngleAxis(this.rotationX, Vector3.up);
			Quaternion rhs2 = Quaternion.AngleAxis(this.rotationY, -Vector3.right);
			base.transform.localRotation = this.originalRotation * rhs * rhs2;
			return;
		}
		if (this.axes == DemoScript.RotationAxes.MouseX)
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
			this.rotationX = DemoScript.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
			Quaternion rhs3 = Quaternion.AngleAxis(this.rotationX, Vector3.up);
			base.transform.localRotation = this.originalRotation * rhs3;
			return;
		}
		this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
		this.rotationY = DemoScript.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
		Quaternion rhs4 = Quaternion.AngleAxis(-this.rotationY, Vector3.right);
		base.transform.localRotation = this.originalRotation * rhs4;
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x00061FA0 File Offset: 0x000601A0
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	// Token: 0x0400118D RID: 4493
	public List<GameObject> Mirrors;

	// Token: 0x0400118E RID: 4494
	public GameObject LightBulb;

	// Token: 0x0400118F RID: 4495
	public Toggle RecursionToggle;

	// Token: 0x04001190 RID: 4496
	private float rotationModifier = -1f;

	// Token: 0x04001191 RID: 4497
	private float moveModifier = 1f;

	// Token: 0x04001192 RID: 4498
	private Material lightBulbMaterial;

	// Token: 0x04001193 RID: 4499
	private DemoScript.RotationAxes axes;

	// Token: 0x04001194 RID: 4500
	private float sensitivityX = 15f;

	// Token: 0x04001195 RID: 4501
	private float sensitivityY = 15f;

	// Token: 0x04001196 RID: 4502
	private float minimumX = -360f;

	// Token: 0x04001197 RID: 4503
	private float maximumX = 360f;

	// Token: 0x04001198 RID: 4504
	private float minimumY = -60f;

	// Token: 0x04001199 RID: 4505
	private float maximumY = 60f;

	// Token: 0x0400119A RID: 4506
	private float rotationX;

	// Token: 0x0400119B RID: 4507
	private float rotationY;

	// Token: 0x0400119C RID: 4508
	private Quaternion originalRotation;

	// Token: 0x02000510 RID: 1296
	private enum RotationAxes
	{
		// Token: 0x04001C23 RID: 7203
		MouseXAndY,
		// Token: 0x04001C24 RID: 7204
		MouseX,
		// Token: 0x04001C25 RID: 7205
		MouseY
	}
}
