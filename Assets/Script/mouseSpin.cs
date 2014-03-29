using UnityEngine;
using System.Collections;

public class mouseSpin : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	if (Input.GetMouseButton(0))
		{
			float y = this.transform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * Time.deltaTime * -100f;
			Quaternion rotation = this.transform.rotation;
			Vector3 eulerAngles = rotation.eulerAngles;
			float num = eulerAngles.y = y;
			Vector3 vector = rotation.eulerAngles = eulerAngles;
			Quaternion quaternion = this.transform.rotation = rotation;
		}
	}
}
