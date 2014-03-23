  using UnityEngine;
using System.Collections;

public class MyEne : MonoBehaviour {
	private float aiTime = 0;
	private int aiState = 0;
	private GameObject objectTarget;
	private float TurnSpeed = 5;
	private float DistanceAttack = 1;
	private float DistanceMoveTo = 20;
	Vector3 moveDir = Vector3.zero;
	private float Speed = 1.5f;
	private float awakeTime = 1.0f;
	private Quaternion tarDir;

	private CharacterController controller;
	//
	// Use this for initialization
	void Start () {
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
	//AI 随机游走
	//attack nearby player
	// Update is called once per frame
	void Update () {
		var direction = Vector3.zero;
		if(aiTime <= 0){
			aiState = Random.Range(0, 1);
			aiTime = Random.Range(10, 50);
		}else {
			aiTime--;
		}
		if(objectTarget) {
			float distance = (objectTarget.transform.position-transform.position).magnitude;
			Quaternion targetRotation = Quaternion.LookRotation(objectTarget.transform.position-this.transform.position);
			targetRotation.x = 0;
			targetRotation.z = 0;
			float str = Mathf.Min(TurnSpeed*Time.deltaTime, 1);
			float ra = objectTarget.GetComponent<CharacterController>().radius;
			ra *= objectTarget.transform.localScale.x;

			//nearby then try to attack
			if(distance <= Mathf.Max(ra*1.2f, DistanceAttack)){

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
				if(dist < length) {
					length = dist;
					objectTarget = targets[i];
				}
			}
		}

		direction.Normalize();
		Move(direction);
		UpdateMove();
	}
}
