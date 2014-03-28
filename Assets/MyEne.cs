using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyEne : Photon.MonoBehaviour {
	private float aiTime = 0;
	private int aiState = 0;
	private GameObject objectTarget;
	private float TurnSpeed = 5;
	public float DistanceAttack = 1;
	private float DistanceMoveTo = 20;
	Vector3 moveDir = Vector3.zero;
	private float Speed = 1.5f;
	private float awakeTime = 1.0f;
	private Quaternion tarDir;

	private CharacterController controller;

	MyStatus ms;
	private float Radius = 1;
	//private float frozeTime = 0;
	bool attacking = false;
	int attackStack = 0;
	string aniName = "attack";
	float Direction = 0.5f;
	private int Force = 500;
	float attackTimeStamp = 0;
	private bool diddamage = false;

	private HashSet<GameObject> listedHit = new HashSet<GameObject>();
	public GameObject projectTile;
	//
	// Use this for initialization
	void Awake() {
		ms = GetComponent<MyStatus>();
		controller = GetComponent<CharacterController>();
	}

	void Move(Vector3 dir) {
		moveDir = dir;
		moveDir *= Speed*0.5f*(Vector3.Dot(gameObject.transform.forward, moveDir)+1);
		if(moveDir.sqrMagnitude > 0.03f) {
			animation.CrossFade("run");
		}else {
			animation.CrossFade("idle");
		}
	}
	void UpdateMove() {
		controller.SimpleMove(moveDir);
	}
	void Attack() {
		if(ms.frozeTime <= 0) {
			fightAnimation();
		}
	}

	//zombie find hero do damage to him
	[RPC]
	void finishFight(int vid, int dam) {
		targetViewId = vid;
		//ms.targetViewId = vid;
		//zombiew attack player
		var p = PhotonView.Find(vid);
		if(p) {
			Debug.Log("zombie attack player");
			var h = p.GetComponent<MyStatus>();
			if(h){
				h.getDamage(gameObject, dam);
			}
		}

		if(ms.frozeTime <= 0) {
			showFightAni();
			StartCoroutine(showRange());
		}
	}

	void showFightAni() {
		aniName = "attack";
		animation.Play(aniName, PlayMode.StopAll);
		//block idle animation
		animation[aniName].layer = 2;
		animation[aniName].blendMode = AnimationBlendMode.Blend;
	}

	private int targetViewId = -1;
	//doAttack Animation
	//check Attack Effect
	//make attack Animation slow
	[RPC]
	void fightNow(int vid) {
		targetViewId = vid;
		if(ms.frozeTime <= 0) {
			showFightAni();
			//if has projectTile then send it out
			StartCoroutine(showRange());
		}
	}
	//wait for a time then send missile
	//range zombie show range attack
	//only when target not dead will do attack

	//other Client zombie fight showRange 
	IEnumerator showRange() {
		if(projectTile){
			var pt = (GameObject)Instantiate(projectTile, transform.position+new Vector3(0, 1f, 0), Quaternion.identity);
			pt.transform.forward = transform.forward;
			if(pt.GetComponent<MissileBase>()){
				pt.GetComponent<MissileBase>().Owner = gameObject;
				pt.GetComponent<MissileBase>().Damage = ms.Damage;
			}
		}
		yield return 0;
	}



	void fightAnimation() {
		var aniState = animation[aniName];
		//Debug.Log("aniName "+aniState.enabled.ToString()+" "+aniState.weight.ToString()+" "+aniState.time.ToString()+" "+attacking);
		if((!attacking && animation[aniName].enabled == false) || (Time.time - attackTimeStamp > 2 ) ) {
			var objs = new object[1];
			objs[0] = objectTarget.GetComponent<PhotonView>().viewID;
			photonView.RPC("fightNow", PhotonTargets.Others, objs);

			attackTimeStamp = Time.time;
			attacking = true;
			diddamage = false;
			//attacking = false;
			//diddamage = false;

			showFightAni();
			/*
			animation.Play(aniName, PlayMode.StopAll);
			//block idle animation
			animation[aniName].layer = 2;
			animation[aniName].blendMode = AnimationBlendMode.Blend;
			*/
			//cache this attack
		} else {
			//attackStack++;
			//attackStack = Mathf.Min(3, attackStack);
		}
	}

	//When fightAnimation attacking = true
	//when finish Animation attacking = false
	//when frozeTime attack = false
	void UpdateAttack() {
		//froze now just stop attack
		if(ms.frozeTime > 0)
			attacking = false;

		AnimationState aniState = animation[aniName];
		/*
		if(aniState.time > aniState.length*0.1f) {
			attacking = true;
		}
		*/

		if(aniState.time > aniState.length*0.8f && !diddamage && attacking) {
			attacking = false;
			diddamage = true;
			if(ms.wtype == WeaponType.Melee)
				DoAttack();
			else
				DoRangeAttack();

			if(attackStack>1){
				attackStack--;
				//not froze then attack
				Attack();
				//fightAnimation();
			}else {
				animation.Play("idle");
			}
		}
	}
	void DoRangeAttack() {
		if(projectTile){
			var pt = (GameObject)Instantiate(projectTile, transform.position+new Vector3(0, 1f, 0), Quaternion.identity);
			pt.transform.forward = transform.forward;
			if(pt.GetComponent<MissileBase>()){
				pt.GetComponent<MissileBase>().Owner = gameObject;
				pt.GetComponent<MissileBase>().Damage = ms.Damage;
			}
		}
	}

	void DoAttack() {
		var colliders = Physics.OverlapSphere(transform.position, Radius);
		foreach(var hit in colliders){
			if(!hit || hit.gameObject.tag != "Player" )
				continue;

			//|| listedHit.Contains(hit.gameObject)
			//listedHit.Add(hit.gameObject);

			var dir = (hit.transform.position-transform.position).normalized;
			var direction = Vector3.Dot(dir, transform.forward);
			if(direction < Direction){
				continue;
			}
			var dirforce = (transform.forward+transform.up)*Force;
			if(hit.gameObject.GetComponent<MyStatus>()) {
				int damage = gameObject.GetComponent<MyStatus>().Damage;
				int damageCal = (int)Random.Range(damage/2.0f, damage)+1;
				var status = hit.gameObject.GetComponent<MyStatus>();
				int takedamage = status.ApplyDamage(damageCal, dirforce, gameObject);

				if(!status.isDead)
					status.AddParticle(hit.transform.position+Vector3.up);

				//Master Client 
				var objs = new object[2];
				objs[0] = status.GetComponent<PhotonView>().viewID;
				objs[1] = takedamage;

				//doharm to player
				if(PhotonNetwork.isMasterClient)
					photonView.RPC("finishFight", PhotonTargets.Others, objs);

				Debug.Log("zombiew kill "+status.HP.ToString()+" client "+PhotonNetwork.isMasterClient.ToString());
				if(status.HP <= 0 && PhotonNetwork.isMasterClient ){
					//if(PhotonNetwork.isMasterClient)
					status.Dead();
				}
				//listObjHitted.Add(hit.gameObject);
			}
		}

		//listedHit.Clear();
	}

	//AI 随机游走
	//attack nearby player
	// Update is called once per frame
	void Update () {
		//not control by myserver 
		if(!photonView.isMine) {
			ms.ShowMove();
			return;
		}

		if(ms.isDead)
			return;

		var direction = Vector3.zero;
		if(aiTime <= 0){
			aiState = Random.Range(0, 1);
			aiTime = Random.Range(10, 50);
		}else {
			aiTime--;
		}
		if(objectTarget && !objectTarget.GetComponent<MyStatus>().isDead) {
			float distance = (objectTarget.transform.position-transform.position).magnitude;
			Quaternion targetRotation = Quaternion.LookRotation(objectTarget.transform.position-this.transform.position);
			targetRotation.x = 0;
			targetRotation.z = 0;
			float str = Mathf.Min(TurnSpeed*Time.deltaTime, 1);
			float ra = objectTarget.GetComponent<CharacterController>().radius;
			ra *= objectTarget.transform.localScale.x;

			//nearby then try to attack
			//ranged attack then check DistanceAttack
			if(distance <= Mathf.Max(ra*1.2f, DistanceAttack)){
				//need to rotation my self
				transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
				Attack();
			//move nearby
			}else {
				//nearby player
				if(distance <= DistanceMoveTo){
					transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
					direction = this.transform.forward;
				} else if(distance <= 30) {
					awakeTime = awakeTime+Time.deltaTime;
					if(awakeTime >= 1.0f) {
						awakeTime = 0;
						tarDir = Quaternion.AngleAxis(Random.value*360, Vector3.up);
						transform.rotation = Quaternion.Lerp(transform.rotation, tarDir, str);
					}
					direction = this.transform.forward;
				} else {
					objectTarget = null;
				}
			}
		}else {
			GameObject[] targets = (GameObject[])GameObject.FindGameObjectsWithTag("Player");
			float length = float.MaxValue;
			for(int i = 0; i < targets.Length; i++){
				float dist = (targets[i].transform.position-transform.position).sqrMagnitude;
				if(targets[i] != null && dist < length && !targets[i].GetComponent<MyStatus>().isDead) {
					length = dist;
					objectTarget = targets[i];
				}
			}
		}

		direction.Normalize();
		Move(direction);
		UpdateMove();
		UpdateAttack();
	}
}
