using UnityEngine;
using System.Collections;

public class MyGod : MonoBehaviour {
	Effect []allPos;
	public Effect oldPos;
	public bool inPos = false;
	private GenWorld gw;
	[HideInInspector]
	public int myId = 1;
	[HideInInspector]
	//public bool isDirty = true;

	//GameObject []myNum;
	// Use this for initialization
	void Start () {
		gw = (GenWorld)FindObjectOfType(typeof(GenWorld));
		//32 number need to show but just show if user can see it
		//myNum = new GameObject[gw.Width*4];
		//each time user can only see one circle
		allPos = (Effect[])FindObjectsOfType(typeof(Effect));

	}

	/*
	void putNumber() {
	}
	*/

	// Update is called once per frame
	void Update () {
		var p = transform.position;
		var find = false;
		for(var i=0; i < allPos.Length; i++) {
			var ap = allPos[i].transform.position;
			ap.y = 0;
			var dist = (ap-p).sqrMagnitude;
			if(dist < 4f) {
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
				//isDirty = true;
				break;
			}
		}

		if(!find) {
			inPos = false;
			if(oldPos != null) {
				oldPos.hideEffect();
				oldPos = null;
				//isDirty = true;
			}
		}
	}
}
