using UnityEngine;
using System.Collections;

public class CharacterNet : Photon.MonoBehaviour {
	void Awake() {
	}
	// Use this for initialization
	void Start () {
		if(photonView.isMine){
		}else {
		}
		gameObject.name = gameObject.name+photonView.viewID;
	}
	//send Protocol save with read Protocol 
	//send Data to certain ViewId
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
	//when initial other player just initial its position and rotation not just send simple data
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

	//according to initial Data determine to do what
	void OnPhotonInstantiate(PhotonMessageInfo info) {

		object[] objs = photonView.instantiationData;
	}

}
