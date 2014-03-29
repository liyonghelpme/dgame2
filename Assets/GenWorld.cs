using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenWorld : Photon.MonoBehaviour {
	public GameObject courner1A;//cross
	public GameObject courner1C; //corner
	public GameObject courner1B; //T
	public GameObject modulaA; // -
	public GameObject levelEdge;

	private int[] map;
	[HideInInspector]
	public int Width = 8;
	private int Height = 8;
	public bool isFinish = false;

	public GameObject disc;
	public GameObject god;

	//private GameObject []allGod;
	[HideInInspector]
	public MyGod []gg;
	private Effect []allDisc;
	public GameObject winShow;
	public bool over = false;
	[HideInInspector]
	public GameObject player;
	public GUISkin skin;
	public GUISkin serskin;

	private string ow;
	public Texture2D tb;
	public bool start = true;
	public string [] zombies;

	private int eneCount = 1;
	private float timeStamp = 0;
	private float timeSpawn = 3;
	public AdMobPlugin ad;
	private bool showAdYet = false;
	private bool overShowAd = false;
	public Texture2D wb;
	bool hideYet = false;

	public string playerPrefabName = "Character";
	private bool createFail = false;
	bool inCreate = false;
	bool gameStartYet = false;
	//[HideInInspector]
	//public bool isOwner = false;

	void OnJoinedRoom() {
		gameStartYet = true;
		seed = (int)PhotonNetwork.room.customProperties["seed"];
		Random.seed = seed;
		Debug.Log("roomSeed "+seed.ToString());
		inCreate = false;
		StartGame();
	}
	bool conFail = false;
	void OnConnectionFail(DisconnectCause cause) {
		Debug.Log("connection fail "+cause.ToString());
		conFail = true;
	}

	//leave room get back to load view
	IEnumerator OnLeftRoom() {
		while(PhotonNetwork.room != null || PhotonNetwork.connected == false)
			yield return 0;
		//Application.LoadLevel()
	}
	void OnDisconnectedFromPhoton() {
		Debug.LogWarning("Disconnect photon");
	}
	void OnPhotonCreateRoomFailed() {
		createFail = true;
		inCreate = false;
		Debug.Log("create faile plear try again");
	}

	void OnPhotonJoinRoomFailed() {
		inCreate = false;
		createFail = true;
	}

	void StartGame() {

		object[] objs = null;
		if(PhotonNetwork.isMasterClient) {
			objs = new object[1];
			objs[0] = seed;
		}
		player = PhotonNetwork.Instantiate(this.playerPrefabName, Vector3.zero, Quaternion.identity, 0, objs);
		//first get Player Data then get other information
		StartCoroutine(initBuilds());
	}
	//GameManager gm;
	void Awake(){
		if(!PhotonNetwork.connected)
			PhotonNetwork.ConnectUsingSettings("v1.0");
		PhotonNetwork.playerName = PlayerPrefs.GetString("playerName", "Guest"+Random.Range(1, 9999));
	}
	private string roomName = "myRoom";
	private bool ava = false;

	[HideInInspector]
	public int seed;
	//public bool initSeedYet = false;
	//     2 
	// 4        1
	//     8
	// Use this for initialization
	void Start () {
		//gm = GetComponent<GameManager>();
		seed = Random.Range(0, 99999);
		Random.seed = seed;
		Debug.Log("initSeed is "+seed.ToString());
		//Random.seed = 1;
		//ad.Hide();


		/*
		GameObject gc = (GameObject)Instantiate(courner1C, Vector3.zero, Quaternion.AngleAxis(-180, Vector3.up));	
		gc.SetActive(true);
		GameObject ma = (GameObject)Instantiate(modulaA, Vector3.zero, Quaternion.AngleAxis(90, Vector3.up));
		ma.SetActive(true);
		GameObject gb = (GameObject)Instantiate(courner1B, Vector3.zero, Quaternion)
		*/
	}
	void initMapData() {
		map = new int[Width*Height];
		map[0] = 3;
		Debug.Log("le "+map.Length.ToString());
		for(var i =0; i < Width; i++) {
			for(var j =0; j < Height; j++) {
				var v = 15;
				if(i == 0) {
					v -= 4; 
				}
				if(i == Width-1) {
					v -= 1;
				}
				if(j == 0) {
					v -= 8;
				}
				if(j == Height-1) {
					v -= 2;
				}
				//Debug.Log("vl "+(j*Width+i).ToString());
				map[j*Width+i] = v;
			}
		}
		debugMap();
		//Width-1 * Height row edge
		//Height-1 * Width  column edge 
		// 0 1  Width-2 
		//0, 1 Height-2
		
		//random cut times
		var totalE = (Width-1)*Height+(Height-1)*Width;
		var minNum = Width*Height-1;
		var maxCut = totalE-minNum;
		//remove how many edges
		var cutNum = Random.Range(maxCut/4, maxCut+1);
		Debug.Log("cut Num "+cutNum.ToString());
		
		for(var i = 0; i < cutNum; i++) {
			//remove row Edge
			int id1, id2;
			int v1, v2;
			if(i%2 == 0) {
				//col
				var cn = Random.Range(0, Width-1);
				//row
				var cr = Random.Range(0, Height);
				Debug.Log("cn cr "+cn.ToString()+" "+cr.ToString());
				
				id1 = cr*Width+cn;
				v1 = map[id1];
				//no edge 
				if((v1 & 1) == 0)
					continue;
				id2 = cr*Width+cn+1;
				v2 = map[id2];
				map[id1] &= 14;
				map[id2] &= 11;
				//remove col Edge
			}else {
				var cn = Random.Range(0, Width);
				var cr = Random.Range(0, Height-1);
				Debug.Log("cn cr odd "+cn.ToString()+" "+cr.ToString()); 
				id1 = cr*Width+cn;
				v1 = map[id1];
				//no edge
				if((v1&2) == 0)
					continue;
				id2 = (cr+1)*Width+cn;
				v2 = map[id2];
				map[id1] &= 13;
				map[id2] &= 7;
			}
			bool r = checkConnect();
			//not connect recover edge
			if(!r) {
				map[id1] = v1;
				map[id2] = v2;
			}
		}
		Debug.Log("out map");
		debugMap();
	}
	IEnumerator initBuilds(){
		initMapData();
		initDungeon();
		initDist();
		initGod();
		isFinish = true;
		yield return 0;
	}
	//only when one god changed into certain pos need to chech
	public void checkQueue(){
		if(over)
			return;
		bool checkOk = true;
		//first check allGod in disc
		for(var i=0; i < gg.Length; i++) {
			var g = gg[i];
			if(!g.inPos){
				checkOk = false;
				break;
			}
		}

		//check all position not collision with other
		if(checkOk) {
			for(var i = 0; i < gg.Length; i++) {
				var g = gg[i];
				var ok = checkSingle(g.oldPos.x, g.oldPos.y);
				if(!ok) {
					checkOk = false;
					break;
				}
			}
		}
		//all God Ok
		if(checkOk) {
			Debug.Log("all God Ok");
			over = true;
			ow = "You Win!";
			var v = player.transform.position;
			winShow.SetActive(true);
			winShow.transform.position = v;

		} else {
			Debug.Log("God not ok");
		}
	}
	void showContent() {
	}
	void ShowConnectingGUI() {
		if(serskin)
			GUI.skin = serskin;
		//GUI.skin = null;
		GUILayout.BeginArea(new Rect((Screen.width-400)/2, (Screen.height-300)/2, 400, 300));
		GUILayout.Label("Connecting to Game server.");
		GUILayout.Label("Please wait for a while");
		if(GUILayout.Button("ReLoad?")) {
			Application.LoadLevel(Application.loadedLevel);
		}
		GUILayout.EndArea();
	}
	void OnCreatedRoom() {
	}

	void ShowSorry() {
		if(serskin)
			GUI.skin = serskin;
		GUILayout.BeginArea(new Rect((Screen.width-400)/2, (Screen.height-300)/2, 400, 300));
		GUILayout.Label("Lost connection to game Server");
		if(GUILayout.Button("Quit")) {
			Application.LoadLevel(Application.loadedLevel);
		}

		GUILayout.EndArea();
	}

	void OnGUI(){
		if(inCreate){
			if(serskin)
				GUI.skin = serskin;
			GUILayout.BeginArea(new Rect((Screen.width-400)/2, (Screen.height-300)/2, 400, 300));
			GUILayout.Label("Entering Game");
			GUILayout.EndArea();
			return;
		}

		if(!PhotonNetwork.connected) {
			//game Start Yet 
			if(conFail || gameStartYet) {
				ShowSorry();
			}else
				ShowConnectingGUI();
			return;
		}

		//not connect room yet
		if(PhotonNetwork.room == null) {
			if(serskin)
				GUI.skin = serskin;
			//GUI.skin = null;

			GUILayout.BeginArea(new Rect((Screen.width-400)/2, (Screen.height-300)/2, 400, 300), "server");
			GUILayout.Label("Main Menu");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Your Name", GUILayout.Width(150));
			GUILayout.Label(PhotonNetwork.playerName);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if(createFail) {
				GUILayout.Label("create Room Error try again");
			}

			if(PhotonNetwork.GetRoomList().Length == 0){
				GUILayout.Label(".. no games available...");
				ava = false;
			}else {
				GUILayout.Label(".. room availabel ..");
				ava = true;
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Your Room", GUILayout.Width(150));
			GUILayout.Label(roomName);
			if(GUILayout.Button("Start Game")) {
				inCreate = true;
				if(!ava) {
					Hashtable ht = new Hashtable();
					ht.Add("seed", seed);
					string[] send = new string[1];
					send[0] = "seed";
					PhotonNetwork.CreateRoom(roomName, true, true, 20 , ht, send);
					//isOwner = true;
				}else {

					PhotonNetwork.JoinRoom(roomName);
					//isOwner = false;
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.EndArea();
			return;
		}

		if(skin)
			GUI.skin = skin;
		if(start) {
			if(!showAdYet) {
				showAdYet = true;
				ad.Show();
			}

			//ad.Show();
			//GUI.skin.box.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
			//GUI.skin.box.
			//GUI.Window(1, new Rect(Screen.width/4, Screen.height/2-100, Screen.width/2, 80), showContent, );
			GUI.DrawTexture(new Rect(Screen.width/4, Screen.height/2-100, Screen.width/2, 80), wb, ScaleMode.ScaleAndCrop);
			GUI.color = Color.black;
			GUI.Box(new Rect(Screen.width/4, Screen.height/2-100, Screen.width/2, 80), "You need to put Eight God in different Row, column and slash. Do your best!", "mybox");
			GUI.color = Color.white;
			if(GUI.Button(new Rect(Screen.width/2-80, Screen.height/2, 160, 80), "Start")) {
				start = false;
				ad.Hide();
			}
			return;
		}

		if(player.GetComponent<MyStatus>().isDead) {
			if(!overShowAd) {
				overShowAd = true;
				ad.Show();
			}
			GUI.skin.button.alignment = TextAnchor.UpperCenter;
			GUI.skin.button.fontSize = 50;
			if(GUI.Button(new Rect(Screen.width/2-80, Screen.height/2, 160, 80), "Resume")) {
				ad.Hide();

				//overShowAd = false;
				//showAdYet = false;
				//Application.LoadLevel("test2");
				player.GetComponent<MyHero>().Relive();
				return;
			}
			GUI.DrawTexture(new Rect(Screen.width/4, Screen.height/2-100, Screen.width/2, 80), tb, ScaleMode.ScaleToFit);
			GUI.skin.label.normal.background = null;
			GUI.skin.label.fontSize = 50;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.skin.label.normal.textColor = Color.white;
			
			GUI.Label(new Rect(Screen.width/4, Screen.height/2-100, Screen.width/2, 80), "You Dead!");
			return;
		}

		if(!over){
			if(ad.IsVisible()) {
				ad.Hide();
				//hideYet = true;
			}
			return;
		}
		if(!overShowAd) {
			overShowAd = true;
			ad.Show();
		}
		GUI.skin.button.alignment = TextAnchor.UpperCenter;
		GUI.skin.button.fontSize = 50;
		if(GUI.Button(new Rect(Screen.width/2-80, Screen.height/2, 160, 80), "Resume")) {
			//Application.LoadLevel("test2");
			player.GetComponent<MyHero>().Relive();
			ad.Hide();
			return;
		}
		//GUI.skin.box.normal.background = GUI.
		//GUI.TextField(new Rect(Screen.width/4, Screen.height/2-110, Screen.width/2, 80),"hello world");
		//GUI.TextArea(new Rect(Screen.width/4, Screen.height/2-110, Screen.width/2, 80),"hello world");
		//GUI.Box(new Rect(Screen.width/4, Screen.height/2-110, Screen.width/2, 80),"hello world");

		GUI.DrawTexture(new Rect(Screen.width/4, Screen.height/2-100, Screen.width/2, 80), tb, ScaleMode.ScaleToFit);
		GUI.skin.label.normal.background = null;
		GUI.skin.label.fontSize = 50;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.normal.textColor = Color.white;

		GUI.Label(new Rect(Screen.width/4, Screen.height/2-100, Screen.width/2, 80), ow);
	}
	//width height 
	bool checkSingle(int x, int y) {
		bool single = true;
		for(var i = 0; i < Width; i++) {
			//row
			if(i != x) {
				if(allDisc[y*Width+i].hasGod){
					single = false;
					return single;
				}
			}
		}
		for(var j=0; j < Height; j++){
			//col
			if(j != y) {
				if(allDisc[j*Width+x].hasGod){
					single = false;
					return single;
				}
			}
		}
		for(var i=0; i < Width; i++) {
			var col = y-x+i;
			if(col >= 0 && col < Height){
				if(i != x && col != y) {
					if(allDisc[col*Width+i].hasGod)
						return false;
				}
			}
		}
		for(var j =0; j < Height; j++){
			var row = x+y-j;
			if(row >= 0 && row < Width) {
				if(row != x && j != y) {
					if(allDisc[j*Width+row].hasGod)
						return false;
				}
			}
		}
		return true;
	}


	//Width God
	void initGod() {
		//allGod = new GameObject[Width];
		gg = new MyGod[Width];

		for(var i = 0; i < Width; i++) {
			Vector3 pos = new Vector3(i*10*2, 0, i*10*2);
			if(i == 0) {
				pos = new Vector3(0, 1, -1);
			}
			var g = (GameObject)Instantiate(god, pos, Quaternion.identity);
			g.SetActive(true);
			//allGod[i] =g;
			gg[i] = g.GetComponent<MyGod>();
			//god Id
			gg[i].myId = i+1;
		}



		/*
		var ps = new int[,]{{1, 0}, {0, 2}, {3, 1}, {2, 3}};

		for(var i = 0; i < Width; i++) {
			var g = (GameObject)Instantiate(god, new Vector3(ps[i, 0]*10*2, 0, ps[i, 1]*10*2), Quaternion.identity);
			g.SetActive(true);
			//allGod[i] =g;
			gg[i] = g.GetComponent<MyGod>();
		}
		*/
	}

	//Width * Height = dist
	void initDist() {
		allDisc = new Effect[Width*Height];
		for(var i = 0; i < Width; i++) {
			for(var j = 0; j < Height; j++) {
				var d = (GameObject)Instantiate(disc, new Vector3(i*10*2, 0.3f, j*10*2), Quaternion.identity);
				d.SetActive(true);
				var e = d.GetComponent<Effect>();
				e.x = i;
				e.y = j;
				allDisc[j*Width+i] = e;
			}
		}
	}

	void initDungeon(){
		var off = 5.0f;
		for(var i = 0; i < Width; i++) {
			for(var j=0; j < Height; j++) {
				var v = j*Width+i;
				var vv = map[v];
				//right end
				if(vv == 1){
					var gc = (GameObject)Instantiate(modulaA, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(90, Vector3.up));
					gc.SetActive(true);
					var le = (GameObject)Instantiate(levelEdge, new Vector3(10*i*2-off, 0, 10*j*2), Quaternion.AngleAxis(90, Vector2.up));
					le.SetActive(true);
				//up end
				}else if(vv == 2) {
					var gc = (GameObject)Instantiate( modulaA , new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(0, Vector3.up));
					gc.SetActive(true);
					var le = (GameObject)Instantiate(levelEdge, new Vector3(10*i*2, 0, 10*j*2-off), Quaternion.AngleAxis(0, Vector2.up));
					le.SetActive(true);
					//left
				} else if(vv == 4) {
					var gc = (GameObject)Instantiate( modulaA , new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(90, Vector3.up));
					gc.SetActive(true);
					var le = (GameObject)Instantiate(levelEdge, new Vector3(10*i*2+off, 0, 10*j*2), Quaternion.AngleAxis(90, Vector2.up));
					le.SetActive(true);
					//bottom 
				} else if(vv == 8) {
					var gc = (GameObject)Instantiate( modulaA , new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(0, Vector3.up));
					gc.SetActive(true);
					var le = (GameObject)Instantiate(levelEdge, new Vector3(10*i*2, 0, 10*j*2+off), Quaternion.AngleAxis(0, Vector2.up));
					le.SetActive(true);
					//16 codition 
				} else if(vv == 3) {
					var gc = (GameObject)Instantiate(courner1C, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(180, Vector3.up));
					gc.SetActive(true);

				} else if(vv == 5){
					var gc = (GameObject)Instantiate(modulaA, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(90, Vector3.up));
					gc.SetActive(true);
				} else if(vv == 6) {
					var gc = (GameObject)Instantiate(courner1C, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(90, Vector3.up));
					gc.SetActive(true);
				} else if(vv == 7) {
					var gc = (GameObject)Instantiate(courner1B, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(90, Vector3.up));
					gc.SetActive(true);
				} 
				/*else if(vv == 8) {
					var gc = (GameObject)Instantiate(modulaA, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(0, Vector3.up));
					gc.SetActive(true);
					var le = (GameObject)Instantiate(levelEdge, new Vector3(10*i*2, 0, 10*j*2+6), Quaternion.AngleAxis(0, Vector2.up));
					le.SetActive(true);

				}*/ 
				else if(vv == 9) {
					var gc = (GameObject)Instantiate(courner1C, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(-90, Vector3.up));
					gc.SetActive(true);
				} else if(vv == 10) {
					var gc = (GameObject)Instantiate(modulaA, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(0, Vector3.up));
					gc.SetActive(true);
				} else if(vv == 11) {
					var gc = (GameObject)Instantiate(courner1B, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(180, Vector3.up));
					gc.SetActive(true);
				} else if(vv == 12) {
					var gc = (GameObject)Instantiate(courner1C, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(0, Vector3.up));
					gc.SetActive(true);
				} else if(vv == 13) {
					var gc = (GameObject)Instantiate(courner1B, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(-90, Vector3.up));
					gc.SetActive(true);
				} else if(vv == 14) {
					var gc = (GameObject)Instantiate(courner1B, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(0, Vector3.up));
					gc.SetActive(true);
				} else if(vv == 15) {
					var gc = (GameObject)Instantiate(courner1A, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(0, Vector3.up));
					gc.SetActive(true);
				}

				var r = map[v] & 1;
				if(r > 0) {
					var gc = (GameObject)Instantiate(modulaA, new Vector3(10*i*2+10, 0, 10*j*2), Quaternion.AngleAxis(90, Vector3.up));
					gc.SetActive(true);
				}
				var u = map[v] & 2;
				if(u > 0) {
					var gc = (GameObject)Instantiate(modulaA, new Vector3(10*i*2, 0, 10*j*2+10), Quaternion.AngleAxis(0, Vector3.up));
					gc.SetActive(true);
				}
				/*
				var l = map[v] & 4;
				if(l == 1) {
					var gc = (GameObject)Instantiate(modulaA, new Vector3(10*i*2-10, 0, 10*j*2), Quaternion.AngleAxis(90, Vector3.up));
					gc.SetActive(true);
				}
				var b = map[v] & 8;
				if(b == 1) {
					var gc = (GameObject)Instantiate(modulaA, new Vector3(10*i*2, 0, 10*j*2-10), Quaternion.AngleAxis(0, Vector3.up));
					gc.SetActive(true);
				}
				*/
			}
		}
	}
	//debug print map data 
	void debugMap() {
		for(var j = 0; j < Height; j++) {
			var s = "";
			for(var i=0; i < Width; i++) {
				s += map[j*Width+i].ToString()+",";
			}
			Debug.Log(s);
		}
	}
	bool checkConnect(){
		var openList = new int[Width*Height];
		var len = 1;
		openList[0] = 0;
		var closedList = new HashSet<int>();

		while(len > 0){
			var v = openList[--len];
			closedList.Add(v);
			var col = v%Width;
			var row = v/Width;

			var r = map[v] & 1;
			var rv = row*Width+col+1;
			if(!closedList.Contains(rv)) {
				if(r > 0) {
					openList[len++] = rv;
				}
			}

			var uv = (row+1)*Width+col;
			if(!closedList.Contains(uv)) {
				var u = map[v] & 2;
				if(u > 0) {
					openList[len++] = uv;
				}
			}
			var lv = row*Width+col-1;
			if(!closedList.Contains(lv)) {
				var l = map[v] & 4;
				if(l > 0) {
					openList[len++] = lv;
				}
			}
			var bv = (row-1)*Width+col;
			if(!closedList.Contains(bv)){
				var b = map[v] & 8;
				if(b > 0) {
					openList[len++] = bv;
				}
			}
		}
		if(closedList.Count == Width*Height) {
			return true;
		}
		return false;
	}

	// Update is called once per frame
	void Update () {
		if(!PhotonNetwork.isMasterClient)
			return;
		if(start)
			return;

		var gos = GameObject.FindGameObjectsWithTag("Enemy");
		if(gos.Length < eneCount && Time.time > timeStamp+timeSpawn){
			var pp = player.transform.position;
			var pcol = Mathf.FloorToInt(pp.x/10);
			var prow = Mathf.FloorToInt(pp.z/10);


			timeStamp = Time.time;
			var row = Random.Range(Mathf.Max(0, pcol-1), Mathf.Min(Width, pcol+1));
			var col = Random.Range(Mathf.Max(0, prow-1), Mathf.Min(Height, prow+1));
			Vector3 v = new Vector3(row*10*2, 1, col*10*2);
			//room exist but long path perhaps not exist
			var rd = Random.Range(0, zombies.Length);

			//var z = (GameObject)GameObject.Instantiate(zombies[rd], v, Quaternion.identity);


			var lv = player.GetComponent<MyStatus>().LEVEL;
			object[] objs = new object[1];
			objs[0] = lv;
			var z = PhotonNetwork.Instantiate(zombies[rd], v, Quaternion.identity, 0, objs);
			//z.SetActive(true);
			z.GetComponent<MyStatus>().SetLevel(lv);
		}
	}
}
