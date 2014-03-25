using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour {
	public string playerPrefabName = "Character";
	[HideInInspector]
	public bool createFail = false;
	void OnJoinedRoom(){
		StartGame();
	}

	//leave Room then show Room Scene GUI
	IEnumerator OnLeftRoom() {
		while(PhotonNetwork.room != null || PhotonNetwork.connected == false)
			yield return 0;
		//Application.LoadLevel()
	}

	void OnDisconnectedFromPhoton() {
		Debug.LogWarning("Disconnect photon");
	}
	void OnConnectionFail() {

	
	}
	void OnPhotonCreateRoomFailed() {
		createFail = true;
		Debug.Log("create faile plear try again");
	}

	//GamePlayer enter Game 
	void StartGame() {
		//generate a local players Character 
		PhotonNetwork.Instantiate(this.playerPrefabName, Vector3.zero, Quaternion.identity, 0);
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
