using UnityEngine;
using System.Collections;

public class ZombieNet : Photon.MonoBehaviour {
	void Awake() {
		if(photonView.isMine){
		}else {
		}
		gameObject.name = gameObject.name+photonView.viewID;
	}
	// Use this for initialization
	void Start () {
	
	}
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		//write my data to server
		if(stream.isWriting){
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}else {
			correctPos = (Vector3)stream.ReceiveNext();
			correctRot = (Quaternion)stream.ReceiveNext();
		}
	}
	//only master server zombie can attack 
	[HideInInspector]
	public Vector3 correctPos = Vector3.zero;
	[HideInInspector]
	public Quaternion correctRot = Quaternion.identity;
	
	// Update is called once per frame
	void Update () {
		if(!photonView.isMine) {
			transform.position = Vector3.Lerp(transform.position, correctPos, Time.deltaTime*5);
			transform.rotation = Quaternion.Slerp(transform.rotation, correctRot, Time.deltaTime*5);
		}
	}
	//level 
	void OnPhotonInstantiate(PhotonMessageInfo info) {
		object[] objs = photonView.instantiationData;
		int lv = (int)(objs[0]);
		GetComponent<MyStatus>().SetLevel(lv);
		correctPos = transform.position;
		correctRot = transform.rotation;
	}
}

