using UnityEngine;
using System.Collections;

public class Radar : MonoBehaviour {
	public Texture2D bg;
	public Texture2D ar;
	float distance = 60;
	private GameObject player;
	private GenWorld gw;
	private MyGod []allGod;
	public Texture2D ene;
	// Use this for initialization
	void Start () {
		//player = (MyHero)FindObjectOfType(typeof(MyHero));

		gw = GetComponent<GenWorld>();
		player = gw.player;
	}
	
	// Update is called once per frame
	void Update () {
		if(gw.isFinish && allGod == null) {
			allGod = gw.gg;
		}
	}
	float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);
		
		if (dir > 0f) {
			return 1f;
		} else if (dir < 0f) {
			return -1f;
		} else {
			return 0f;
		}
	}
	void drawGod(MyGod g) {
		if(player == null)
			return;
		//if(player == null)
		//	player = 
		Vector3 dir = g.transform.position-player.transform.position;
		dir.y = 0;
		if(dir.sqrMagnitude < distance*distance) {
			float dist = dir.magnitude;
			dir.Normalize();
			float angle = Vector3.Angle(Vector3.forward, dir);
			float ad = AngleDir(Vector3.forward, dir, Vector3.up);
			angle *= ad;
			Vector3 rd = dist/distance*75*dir;
			GUI.color = Color.green;
			GUI.DrawTexture(new Rect(75+rd.x-ene.width/2, 75-rd.z-ene.height/2, ene.width, ene.height), ene);
			GUI.color = Color.white;
		}
	}
	void drawEnemy() {
		var enemy = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(var e in enemy) {
			Vector3 dir = e.transform.position-player.transform.position;
			dir.y = 0;
			if(dir.sqrMagnitude < distance*distance) {
				float dist = dir.magnitude;
				dir.Normalize();
				float angle = Vector3.Angle(Vector3.forward, dir);
				float ad = AngleDir(Vector3.forward, dir, Vector3.up);
				angle *= ad;
				Vector3 rd = dist/distance*75*dir;
				GUI.color = Color.red;
				GUI.DrawTexture(new Rect(75+rd.x-ene.width/2, 75-rd.z-ene.height/2, ene.width, ene.height), ene);
				GUI.color = Color.white;
			}
		}
	}
	void drawPlayer() {
		var p = GameObject.FindGameObjectsWithTag("Player");
		foreach(var e in p) {
			if(e != player) {
				Vector3 dir = e.transform.position-player.transform.position;
				dir.y = 0;
				if(dir.sqrMagnitude < distance*distance) {
					float dist = dir.magnitude;
					dir.Normalize();
					float angle = Vector3.Angle(Vector3.forward, dir);
					float ad = AngleDir(Vector3.forward, dir, Vector3.up);
					angle *= ad;
					Vector3 rd = dist/distance*75*dir;
					GUI.color = Color.blue;
					GUI.DrawTexture(new Rect(75+rd.x-ene.width/2, 75-rd.z-ene.height/2, ene.width, ene.height), ene);
					GUI.color = Color.white;
				}
			}
		}
	}
	void OnGUI() {
		if(gw.start)
			return;
		if(player == null) {
			//player = (MyHero)FindObjectOfType(typeof(MyHero));
			player = gw.player;
		}

		var scale = 150f/bg.width;
		drawEnemy();
		if(allGod != null) {
			for(var i = 0; i < allGod.Length; i++){
				drawGod(allGod[i]);
			}
		}

		drawPlayer();

		GUI.DrawTexture(new Rect(0, 0, 150, 150), bg);
		if(player) {
			Vector3 v = player.transform.forward;
			float angle = Vector3.Angle(Vector3.forward, v);
			var ad = AngleDir(Vector3.forward, v, Vector3.up);
			angle *= ad;
			var p = new Vector2(75, 75);
			GUIUtility.RotateAroundPivot(angle, p);
			GUI.DrawTexture(new Rect(0, 0, 150, 150), ar);
			GUIUtility.RotateAroundPivot(0, Vector2.zero);
		}
	}
}
