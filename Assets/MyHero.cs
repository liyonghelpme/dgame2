using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyHero : Photon.MonoBehaviour {
	CharacterController controller;
	float speed = 3.0f;
	bool move = false;
	private Vector3 tarPos;
	private Vector3 moveDir = Vector3.zero;
	public float TurnSmooth = 5;
	public float Speed = 2;
	public Vector3 velocity = Vector3.zero;
	private GenWorld gw;
	float pushPower = 2.0f;
	private Plane p;

	//float frozetime = 0;
	//float attackTime = 0;
	int attackStack = 0;

	bool attacking = false;
	//bool diddamage = false;
	string aniName = "attack1";

	//attack range
	private float Radius = 1;
	private HashSet<GameObject> listObjHitted = new HashSet<GameObject>();
	private float Direction = 0.5f;
	private int Force = 500;

	private bool diddamage = false;

	public AudioClip []attackSound;


	HashSet<GameObject> listedHit = new HashSet<GameObject>();
	PhotonView pv;
	CharacterNet cn;
	MyStatus ms;

	//int wp = 0;
	public void Relive() {
		realRelive();
		photonView.RPC("getUp", PhotonTargets.Others);
	}

	void realRelive() {
		transform.position = new Vector3(0, 1, 0);
		gameObject.SetActive(true);
		//var ms = GetComponent<MyStatus>();
		ms.isDead = false;
		ms.HP = ms.HPmax;
		attacking = false;
		showSelf();
		tarPos = transform.position;
		controller.enabled = true;
	}
	public Transform rightHand;

	//Player HP increase to max
	//how to reflect other player HP change?
	[RPC]
	void getUp() {
		Debug.Log("show Self rpc");
		realRelive();
		//ms.HP = ms.HPmax;
	}


	void showSelf() {
		var rc = GetComponentsInChildren<Renderer>();
		foreach(var i in rc)
			i.enabled =  true;
	}
	void Awake() {
		cn = GetComponent<CharacterNet>();
		
		pv = GetComponent<PhotonView>();
		ms = GetComponent<MyStatus>();
		ms.isHero = true;
		p = new Plane(Vector3.up, Vector3.zero);
		gw = (GenWorld)FindObjectOfType(typeof(GenWorld));
		
		controller = GetComponent<CharacterController>();
		gameObject.animation.CrossFade("idle");
		tarPos = transform.position;
		initWeapon();
	}
	void initWeapon() {
		if(ms.wp == 0) {
			var sw = (GameObject)Instantiate(Resources.Load("Sword"));
			//var ch = transform.Find("weaponPoint");
			if(rightHand) {
				if(rightHand.childCount > 0) {
					var c = rightHand.GetChild(0);
					GameObject.Destroy(c.gameObject);
				}

				//Debug.Log("child find "+ch.ToString());
				Vector3 op = sw.transform.localPosition;
				Vector3 os = sw.transform.localScale;
				Quaternion oq = sw.transform.localRotation;
				sw.transform.parent = rightHand;
				sw.transform.localPosition = op;
				sw.transform.localRotation = oq;
				sw.transform.localScale = os;
			}
		}
	}
	// Use this for initialization
	void Start () {

	}

	[RPC]
	void pushGod(int gid, float x, float y, float z) {
		if(gid > 0 && gid < gw.gg.Length) {
			var god = gw.gg[gid-1];
			god.transform.position = Vector3.Lerp(god.transform.position, new Vector3(x, y, z), Time.deltaTime*5);
		}
	}


	bool inPush = false;
	float pTime = 0;
	MyGod pw;
	[HideInInspector]
	public float axeTime = 0;
	public void getAxe(GameObject axe){
		Debug.Log("getAxe right Hand");
		if(rightHand) {
			ms.wp = 1;
			Radius = 2.0f;
			axeTime = Time.time;
			GameObject.Destroy(rightHand.GetChild(0).gameObject);

			var ch = axe.transform.GetChild(0);
			ch.animation.enabled = false;
			 
			ch.transform.parent = rightHand;
			ch.transform.localPosition = Vector3.zero;
			ch.transform.localRotation = Quaternion.identity;
			ch.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);


			var col = (GameObject)Instantiate(Resources.Load("CoinCollectFlash"));
			col.transform.position = axe.transform.position;
			GameObject.Destroy(col, 1);
			GameObject.Destroy(axe);
		}
	}

	public void getCoin(GameObject c) {
		coin++;
		var col = (GameObject)Instantiate(Resources.Load("CoinCollectFlash"));
		col.transform.position = c.transform.position;
		GameObject.Destroy(col, 1);
		GameObject.Destroy(c);
	}
	[HideInInspector]
	public int coin = 0;
	void OnControllerColliderHit(ControllerColliderHit hit){
		Debug.Log("collider object "+hit.collider.tag);
		if(hit.collider.tag == "Item") {
			return;
			/*
			coin++;
			var col = (GameObject)Instantiate(Resources.Load("CoinCollectFlash"));
			col.transform.position = hit.collider.transform.position;
			GameObject.Destroy(col, 1);
			GameObject.Destroy(hit.collider.gameObject);
			return;
			*/
		}


		var body = hit.collider.attachedRigidbody;
		if(body == null) return;

		if(hit.moveDirection.y < -0.3) return;
		var pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		body.velocity = pushDir*pushPower;

		//Only my hero push take effect show to others
		if(photonView.isMine) {
			inPush = true;
			pTime = Time.time;
			pw = body.gameObject.GetComponent<MyGod>();
		}
		//photonView.RPC("pushGod", PhotonTargets.Others, );
	}


	void Move(Vector3 dir) {
		moveDir = dir;
		if(attacking)
			moveDir /= 2;
		if(moveDir.magnitude > 0.1f) {
			var newRotation = Quaternion.LookRotation(moveDir);
			float ts = Time.deltaTime*TurnSmooth;
			if(attacking)
				ts *= 0.5f;
			transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, ts);
		}
		//move Speed  = Speed * current forward and dir forward diffrence
		moveDir *= Speed*0.5f*(Vector3.Dot(gameObject.transform.forward, moveDir)+1);
		if(moveDir.magnitude > 0.001f){
			float speedanimation = moveDir.magnitude*3;
			animation.CrossFade("run");
			/*
			if(speedanimation < 1) {
				speedanimation = 1;
			}
			*/
			//animation["run"].speed = speedanimation;
		} else {
			animation.CrossFade("idle");
		}

	}

	void UpdateMove(){

		if(Time.time-pTime >= 0.1f && inPush && pw != null){
			inPush = false;
			var objs = new object[4];
			objs[0] = pw.myId;
			objs[1] = pw.transform.position.x;
			objs[2] = pw.transform.position.y;
			objs[3] = pw.transform.position.z;
			photonView.RPC("pushGod", PhotonTargets.Others, objs);
		}

		//Debug.Log("move dir "+moveDir.ToString());
		controller.SimpleMove(moveDir);
	}
	//view data divided show view by client 
	//show data by rpc
	[RPC]
	void finishFight(int vid, int dam) {
		//ms.targetViewId = vid;
		var p = PhotonView.Find(vid);
		if(p) {
			var h = p.GetComponent<MyStatus>();
			if(h){
				h.getDamage(gameObject, dam);
			}
		}
		targetViewId = vid;
		showFightAni();
	}

	int targetViewId = -1;
	[RPC]
	void fightNow(int vid) {
		targetViewId = vid;
		showFightAni();
	}

	void showFightAni() {
		//attacking = false;
		//diddamage = false;
		var r = Random.Range(1, 3);
		aniName = "attack"+r.ToString();
		animation.Play(aniName, PlayMode.StopAll);
		
		//block idle animation
		animation[aniName].layer = 2;
		animation[aniName].blendMode = AnimationBlendMode.Blend;
		//if(ms.wp == 0)
		animation[aniName].speed = 2;
	}

	float attackTimeStamp = 0;
	//most attack Time cost
	void fightAnimation() {
		//if not in attack
		if((!attacking && !animation[aniName].enabled) || (Time.time-attackTimeStamp) > 2) {
			attacking = true;
			diddamage = false;
			attackTimeStamp = Time.time;

			//fightNow make sure attackTarget animation calculate
			var objs = new object[1];
			objs[0] = objectTarget.GetComponent<PhotonView>().viewID;
			photonView.RPC("fightNow", PhotonTargets.Others, objs);
			showFightAni();
		//cache this attack
		} else {
			//attackStack++;
			//attackStack = Mathf.Min(3, attackStack);
		}
	}
	//buffer most three attack
	void Attack() {
		//if(frozetime <= 0) {
			//attackTime = Time.time;
			fightAnimation();
			//attackStack++;
			//attackStack = Mathf.Min(3, attackStack);
		//}
	}
	//real hurt effect
	void DoAttack() {
		//DoDamage function 
		var colliders = Physics.OverlapSphere(transform.position, Radius);
		foreach(var hit in colliders){
			if(!hit || hit.gameObject.tag != "Enemy" ) {
				continue;
			}

			//|| listedHit.Contains(hit.gameObject)
			//listedHit.Add(hit.gameObject);
			//group attack need to check whether attack
			/*
			if(listObjHitted.Contains(hit.gameObject)){
				continue;
			}
			*/
			var dir = (hit.transform.position-transform.position).normalized;
			var direction = Vector3.Dot (dir, transform.forward);
			if(direction < Direction){
				continue;
			}
			var dirforce = (transform.forward+transform.up)*Force;
			if(hit.gameObject.GetComponent<MyStatus>()) {
				int damage = gameObject.GetComponent<MyStatus>().Damage;
				int damageCal = (int)Random.Range(damage/2.0f, damage)+1;
				var status = hit.gameObject.GetComponent<MyStatus>();
				int takedamage = status.ApplyDamage(damageCal, dirforce, gameObject);
				if(attackSound.Length > 0){
					int rd = Random.Range(0, attackSound.Length);
					AudioSource.PlayClipAtPoint(attackSound[rd], transform.position);
				}

				status.AddParticle(hit.transform.position+Vector3.up);

				var objs = new object[2];
				objs[0] = status.GetComponent<PhotonView>().viewID;
				objs[1] = takedamage;
				photonView.RPC("finishFight", PhotonTargets.Others, objs);
				//listObjHitted.Add(hit.gameObject);
				if(status.HP <= 0 && PhotonNetwork.isMasterClient ){
					//if(PhotonNetwork.isMasterClient)
					status.Dead();
				}
			}

			//orbit target object shake camera 
			//if zombie attack hero
			//push rigibody 
			//var dirforce = (transform.forward+transform.up)*Force;
		}
		//listedHit.Clear();
	}
	void UpdateAttack() {
		AnimationState aniState = animation[aniName];

		/*
		if(aniState.time >= aniState.length*0.1f){
			attacking = true;
		}
		*/


		if(aniState.time >= aniState.length*0.8f & !diddamage) {
			attacking = false;
			diddamage = true;
			//if(!diddamage) {
			//	diddamage = true;
				DoAttack();
				
				if(attackStack>1){
					attackStack--;
					fightAnimation();
				}else {
					animation.Play("idle");
				}
				//startDamage
			//}
		}

	}
	private GameObject objectTarget;
	// Update is called once per frame
	void Update() {
		if(!pv.isMine) {
			ms.ShowMove();
			return;
		}
		if(ms.isDead)
			return;

		if(gw.over)
			return;
		if(gw.start)
			return;
		if(!gw.isFinish)
			return;
		if(ms.wp == 1 && Time.time-axeTime > 20.0f) {
			ms.wp = 0;
			Radius = 1;
			initWeapon();
		}
		var direction = Vector3.zero;
		if(Input.GetMouseButtonDown(0)){
			float dist;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(p.Raycast(ray, out dist)) {
				tarPos = ray.GetPoint(dist);
				Debug.Log("ray cast pos "+tarPos.ToString()+" "+dist.ToString());
			}


			/*
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 100)){
				tarPos = hit.point;
			}
			*/
		}

		if(!attacking && !animation[aniName].enabled) {
			bool findAttack = false;
			var colliders = Physics.OverlapSphere(this.transform.position, Radius);
			foreach(var hit in colliders){
				if(!hit || hit.gameObject.tag != "Enemy")
					continue;
				//if enemy dead don't care
				var sta = hit.gameObject.GetComponent<MyStatus>();
				if(sta != null && sta.isDead)
					continue;

				//judge whether direction has enemy no enemy just dont attack		
				var dir = (hit.transform.position-transform.position).normalized;
				var td = Vector3.Dot(dir, transform.forward);
				if(td < Direction){
					continue;
				}
				objectTarget = hit.gameObject;
				findAttack = true;
				Attack();
				break;
			}
			//run away just not attack anymore
			if(!findAttack) {
				attackStack = 0;
			}
		}

		//Debug.Log("tarPos "+tarPos.ToString()+" "+transform.position.ToString());
		var py = tarPos;
		py.y = transform.position.y;
		direction = py - transform.position;
		Debug.Log("direction size "+direction.ToString());
		if(direction.sqrMagnitude > 0.5) {
			direction.Normalize();
			Move(direction);
		} else {
			Move(Vector3.zero);
		}
		UpdateMove();

		UpdateAttack();
	}
}
