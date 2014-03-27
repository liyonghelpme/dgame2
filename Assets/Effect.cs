using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Effect : MonoBehaviour {
	public GameObject cs, ps1, ps2;
	public int x, y;
	public bool hasGod = false;
	public GameObject num;

	bool showNum = false;
	List<GameObject> nums;
	GameObject player;
	bool isDirty = false;

	GenWorld gw;
	// Use this for initialization
	void Start () {
		gw = (GenWorld)FindObjectOfType(typeof(GenWorld));
		nums = new List<GameObject>();
		//player = GameObject.FindGameObjectWithTag("Player");
		player = gw.player;
		//showEffect();
	}
	public void showEffect(){
		isDirty = true;
		hasGod = true;
		Color c = new Color(0, 251/255.0f, 2/255.0f);
		cs.renderer.material.SetColor("_TintColor", c);
		ps1.renderer.material.SetColor("_TintColor", c);
		ps2.renderer.material.SetColor("_TintColor", c);
	}
	public void hideEffect() {
		isDirty = true;
		hasGod = false;
		Color c = new Color(75/255.0f, 77/255.0f, 75/255.0f);
		cs.renderer.material.SetColor("_TintColor", c);
		ps1.renderer.material.SetColor("_TintColor", c);
		ps2.renderer.material.SetColor("_TintColor", c);
	}
	
	// Update is called once per frame
	//calculate all effect on same row
	void Update () {
		if(player == null) {
			//player = GameObject.FindGameObjectWithTag("Player");
			player = gw.player;
		}
		if(player == null)
			return;
		var dis = (transform.position-player.transform.position).sqrMagnitude;

		//nearby and not isDirty if change Effect then dirty 
		if(dis < 10*10 && !isDirty) {
			//Debug.Log("dis effect "+dis.ToString());
			if(!showNum){
				showNum = true;
				var allGod = GameObject.FindGameObjectsWithTag("God");
				//eight god
				var poffset = new Vector3[]{
					new Vector3(-1, 0, 1), 
					new Vector3(0, 0, 1), 
					new Vector3(1, 0, 1), 
					new Vector3(-1, 0, 0), 
					new Vector3(0, 0, 0), 
					new Vector3(1, 0, 0 ),
					new Vector3(-1, 0, -1),
					new Vector3(1, 0, -1),
				};
				foreach(var g in allGod){
					//Debug.Log("allGod is ");
					var gg = g.GetComponent<MyGod>();
					if(gg.inPos) {
						//Debug.Log("God in pos "+gg.oldPos.x.ToString()+" "+x.ToString());
						if(gg.oldPos.x == x || gg.oldPos.y == y || (gg.oldPos.x-x) == (gg.oldPos.y-y) || (gg.oldPos.x-x) == -(gg.oldPos.y-y)) {
							var gobj = new GameObject();
							var n = (GameObject)Instantiate(num);
							n.transform.parent = gobj.transform;
							gobj.transform.position = transform.position+poffset[gg.myId-1];
							gobj.transform.rotation = Quaternion.identity;

							n.SetActive(true);
							n.GetComponent<TextMesh>().text = gg.myId.ToString();

							nums.Add(gobj);
						
						}
					} else {
						
					}
				}
			}
		}else {
			if(showNum) {
				showNum = false;
				foreach(var n in nums) {
					Destroy(n);
				}
				nums.Clear(); 
			}
			//move current god
			isDirty = false;
		}
	}
}
