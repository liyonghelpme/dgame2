using UnityEngine;
using System.Collections;

public class PickItem : MonoBehaviour {
	float inGen = 0;
	bool colYet = false;
	public int kind = 0;
	void Awake() {
		collider.enabled = false;
		inGen = Time.time;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time-inGen > 0.4f){
			collider.enabled = true;
		}
	}

	void OnTriggerEnter(Collider col) {
		//if(Time.time-inGen > 0.3f) {
		if(col.tag == "Player") {
			Debug.Log("col with player "+kind.ToString());
			var he = col.collider.GetComponent<MyHero>();
			if(he != null) {
				if(kind == 0)
					he.getCoin(gameObject);
				else if(kind == 1)
					he.getAxe(gameObject);
			}
		} else {
			Debug.Log("trigger obj "+col.gameObject.name);
		}
		//}
	}

	void OnCollisionEnter(Collision col) {
		if(col.collider.tag == "Player") {

		}
	}
}
