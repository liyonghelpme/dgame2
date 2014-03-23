using UnityEngine;
using System.Collections;

public class MyGod : MonoBehaviour {
	Effect []allPos;
	public Effect oldPos;
	public bool inPos = false;
	private GenWorld gw;
	// Use this for initialization
	void Start () {
		gw = (GenWorld)FindObjectOfType(typeof(GenWorld));
		allPos = (Effect[])FindObjectsOfType(typeof(Effect));

	}

	// Update is called once per frame
	void Update () {
		var p = transform.position;
		var find = false;
		for(var i=0; i < allPos.Length; i++) {
			var ap = allPos[i].transform.position;
			ap.y = 0;
			var dist = (ap-p).sqrMagnitude;
			if(dist < 1f) {
				find = true;
				inPos = true;
				if(oldPos == allPos[i])
					break;
				if(oldPos != null) {
					oldPos.hideEffect();
				}

				oldPos = allPos[i];
				allPos[i].showEffect();
				gw.checkQueue();
				break;
			}
		}
		if(!find) {
			inPos = false;
			if(oldPos != null) {
				oldPos.hideEffect();
				oldPos = null;
			}
		}
	}
}
