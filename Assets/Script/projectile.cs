using UnityEngine;
using System.Collections;

public class projectile : MonoBehaviour {
private Vector3 velocity;
private bool disabled;
private bool setSpeed;
private float life;
private float shootTime;
private bool goBackToQuiver;
private Vector3 quiverLocalPosition;
private Vector3 quiverLocalRotation;
public Transform quiver;
	// Use this for initialization
	void Start () {
	quiverLocalPosition = transform.localPosition;
		quiverLocalRotation = transform.localRotation.eulerAngles;
		quiver = transform.parent;
		this.disabled = true;
		this.life = 2f;
	}
	
	// Update is called once per frame
	void Update () {
	if (!this.disabled)
		{
			if (!this.setSpeed)
			{
				this.velocity = this.transform.up * (float)-30;
				this.setSpeed = true;
				this.transform.parent = null;
				this.shootTime = Time.time;
			}
			this.velocity.y = this.velocity.y - (float)5 * Time.deltaTime;
			this.transform.position = this.transform.position + this.velocity * Time.deltaTime;
			this.transform.LookAt(this.transform.position + this.velocity);
			float x = this.transform.localRotation.eulerAngles.x - (float)90;
			Quaternion localRotation = this.transform.localRotation;
			Vector3 eulerAngles = localRotation.eulerAngles;
			float num = eulerAngles.x = x;
			Vector3 vector = localRotation.eulerAngles = eulerAngles;
			Quaternion quaternion = this.transform.localRotation = localRotation;
			if (Time.time > this.shootTime + this.life)
			{
				this.disabled = true;
				this.setSpeed = false;
				if (this.goBackToQuiver)
				{
					this.transform.parent = this.quiver;
					this.transform.localPosition = this.quiverLocalPosition;
					Vector3 eulerAngles2 = this.quiverLocalRotation;
					Quaternion localRotation2 = this.transform.localRotation;
					Vector3 vector2 = localRotation2.eulerAngles = eulerAngles2;
					Quaternion quaternion2 = this.transform.localRotation = localRotation2;
				}
			}
		}
	}
	public  void Shoot()
	{
		this.disabled = false;
	}
	public  void GoBackToQuiver()
	{
		this.goBackToQuiver = true;
	}
}
