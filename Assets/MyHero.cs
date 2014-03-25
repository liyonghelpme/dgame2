using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyHero : MonoBehaviour {
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

	private float Radius = 1;
	private HashSet<GameObject> listObjHitted = new HashSet<GameObject>();
	private float Direction = 0.5f;
	private int Force = 500;

	private bool diddamage = false;

	public AudioClip []attackSound;


	HashSet<GameObject> listedHit = new HashSet<GameObject>();


	public void Relive() {
		transform.position = new Vector3(0, 1, 0);
		gameObject.SetActive(true);
		var ms = GetComponent<MyStatus>();
		ms.isDead = false;
		ms.HP = ms.HPmax;
		attacking = false;
	}

	// Use this for initialization
	void Start () {
		GetComponent<MyStatus>().isHero = true;
		p = new Plane(Vector3.up, Vector3.zero);
		gw = (GenWorld)FindObjectOfType(typeof(GenWorld));

		controller = GetComponent<CharacterController>();
		gameObject.animation.CrossFade("idle");
		tarPos = transform.position;
	}
	void OnControllerColliderHit(ControllerColliderHit hit){
		var body = hit.collider.attachedRigidbody;
		if(body == null) return;
		if(hit.moveDirection.y < -0.3) return;
		var pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		body.velocity = pushDir*pushPower;
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
		//Debug.Log("move dir "+moveDir.ToString());
		controller.SimpleMove(moveDir);
	}

	void fightAnimation() {
		//if not in attack
		if(!attacking && !animation[aniName].enabled) {
			attacking = true;
			diddamage = false;
			//attacking = false;
			//diddamage = false;
			var r = Random.Range(1, 3);
			aniName = "attack"+r.ToString();
			animation.Play(aniName, PlayMode.StopAll);
			//block idle animation
			animation[aniName].layer = 2;
			animation[aniName].blendMode = AnimationBlendMode.Blend;
			animation[aniName].speed = 2;
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
				//listObjHitted.Add(hit.gameObject);
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

	// Update is called once per frame
	void Update () {
		if(gw.over)
			return;
		if(gw.start)
			return;
		if(!gw.isFinish)
			return;
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
			var colliders = Physics.OverlapSphere(this.transform.position, 2);
			foreach(var hit in colliders){
				if(!hit || hit.gameObject.tag != "Enemy")
					continue;
				//if enemy dead don't care
				
				//judge whether direction has enemy no enemy just dont attack		
				var dir = (hit.transform.position-transform.position).normalized;
				var td = Vector3.Dot(dir, transform.forward);
				if(td < Direction){
					continue;
				}
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
