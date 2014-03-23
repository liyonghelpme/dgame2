﻿using UnityEngine;
using System.Collections;

public class Radar : MonoBehaviour {
	public Texture2D bg;
	public Texture2D ar;
	float distance = 60;
	private MyHero player;
	private GenWorld gw;
	private MyGod []allGod;
	public Texture2D ene;
	// Use this for initialization
	void Start () {
		player = (MyHero)FindObjectOfType(typeof(MyHero));
		gw = GetComponent<GenWorld>();
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
	void OnGUI() {
		var scale = 150f/bg.width;
		if(allGod != null) {
			for(var i = 0; i < allGod.Length; i++){
				drawGod(allGod[i]);
			}
		}

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