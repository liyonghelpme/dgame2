using UnityEngine;
using System.Collections;

//Camera Monitor role
public class Monitor : Orbit {
	GameObject p;
	public float CameraLength = 5;
	// Use this for initialization
	private Vector3 cameraPosition = Vector3.zero;

	private float tarZen;
	private float frozeTime = 0;
	private int tNum = 0;
	GenWorld gw;
	void Start () {
		gw = (GenWorld)FindObjectOfType(typeof(GenWorld));
		Data.Zenith = 0.5f;
		tarZen = Data.Azimuth;
		Data.Length = CameraLength;

		//Camera.main.transform.position = cameraPosition;

		//p = (MyHero)FindObjectOfType(typeof(MyHero));

	}

	
	// Update is called once per frame
	//lerp slerp 
	void Update () {
		if(p){
			var lookAt = p.transform.position+Data.Position;
			gameObject.transform.position = lookAt;
			gameObject.transform.LookAt(p.transform.position);
			RaycastHit hit;
			if(Physics.Linecast(transform.position, p.transform.position, out hit)) {
				//Debug.Log("hitObject "+hit.collider.gameObject.name);
				if(hit.collider.gameObject != p.gameObject && hit.collider.gameObject.tag == "Untagged") {
					if(Mathf.Abs(Data.Azimuth-tarZen) < 0.1f) {
						tNum++;
						tNum %= 4;
						tarZen = tNum*1.5f;
					}
				}
			}

			//Debug.Log("tarZen " +tarZen.ToString());
			//if(tarZen != Data.Azimuth) {
			Data.Azimuth = Mathf.Lerp(Data.Azimuth, tarZen, Time.deltaTime*0.5f);
			//}
		}else {
			//p = (MyHero)FindObjectOfType(typeof(MyHero));
			p = gw.player;
		}
	}
}
