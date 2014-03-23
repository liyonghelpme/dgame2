using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenWorld : MonoBehaviour {
	public GameObject courner1A;//cross
	public GameObject courner1C; //corner
	public GameObject courner1B; //T
	public GameObject modulaA; // -
	public GameObject levelEdge;

	private int[] map;
	private int Width = 4;
	private int Height = 4;
	public bool isFinish = false;

	public GameObject disc;
	public GameObject god;

	//private GameObject []allGod;
	[HideInInspector]
	public MyGod []gg;
	private Effect []allDisc;
	public GameObject winShow;
	public bool over = false;
	public GameObject player;
	public GUISkin skin;
	private string ow;
	public Texture2D tb;
	public bool start = true;
	public GameObject zombies;

	private int eneCount = 10;
	private float timeStamp = 0;
	private float timeSpawn = 3;
	//     2 
	// 4        1
	//     8
	// Use this for initialization
	void Start () {
		Random.seed = 1;
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
		initDungeon();
		initDist();
		initGod();
		isFinish = true;
		/*
		GameObject gc = (GameObject)Instantiate(courner1C, Vector3.zero, Quaternion.AngleAxis(-180, Vector3.up));	
		gc.SetActive(true);
		GameObject ma = (GameObject)Instantiate(modulaA, Vector3.zero, Quaternion.AngleAxis(90, Vector3.up));
		ma.SetActive(true);
		GameObject gb = (GameObject)Instantiate(courner1B, Vector3.zero, Quaternion)
		*/
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
	void OnGUI(){
		if(skin)
			GUI.skin = skin;
		if(start) {
			//GUI.skin.box.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
			//GUI.skin.box.

			GUI.Box(new Rect(Screen.width/4, Screen.height/2-100, Screen.width/2, 80), "You need to put Eight God in different Row, column and slash. Do your best!", "mybox");
			if(GUI.Button(new Rect(Screen.width/2-80, Screen.height/2, 160, 80), "Start")) {
				start = false;
			}
			return;
		}

		if(!over){
			return;
		}

		GUI.skin.button.alignment = TextAnchor.UpperCenter;
		GUI.skin.button.fontSize = 50;
		if(GUI.Button(new Rect(Screen.width/2-80, Screen.height/2, 160, 80), "Resume")) {
			Application.LoadLevel("test2");
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
			var g = (GameObject)Instantiate(god, new Vector3(i*10*2, 0, i*10*2), Quaternion.identity);
			g.SetActive(true);
			//allGod[i] =g;
			gg[i] = g.GetComponent<MyGod>();
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
					var gc = (GameObject)Instantiate(courner1B, new Vector3(10*i*2, 0, 10*j*2), Quaternion.AngleAxis(90, Vector3.up));
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
		var gos = GameObject.FindGameObjectsWithTag("Enemy");
		if(gos.Length < eneCount && Time.time > timeStamp+timeSpawn){
			timeStamp = Time.time;
			var row = Random.Range(0, Width);
			var col = Random.Range(0, Height);
			Vector3 v = new Vector3(row*10*2, 1, col*10*2);
			//room exist but long path perhaps not exist
			var z = (GameObject)GameObject.Instantiate(zombies, v, Quaternion.identity);
			z.SetActive(true);
		}
	}
}
