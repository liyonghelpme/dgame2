using UnityEngine;
using System.Collections;

public class stickTo : MonoBehaviour {
public Transform StickTo;
public bool disabled;
private Vector3 resetPosition;
	// Use this for initialization
	void Start () {
	this.resetPosition = this.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
	if (this.disabled)
		{
			this.transform.localPosition = this.resetPosition;
		}
		else
		{
			this.transform.position = this.StickTo.position;
		}
	}
}
