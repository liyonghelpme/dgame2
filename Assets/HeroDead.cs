using UnityEngine;
using System.Collections;

public class HeroDead : DeadNow {
	public override void OnDead() {
		gameObject.SetActive(false);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
