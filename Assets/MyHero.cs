using UnityEngine;
using System.Collections;

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
	// Use this for initialization
	void Start () {
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
		if(moveDir.magnitude > 0.1f) {
			var newRotation = Quaternion.LookRotation(moveDir);
			transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime*TurnSmooth);
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
		Debug.Log("move dir "+moveDir.ToString());
		controller.SimpleMove(moveDir);
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
		Debug.Log("tarPos "+tarPos.ToString()+" "+transform.position.ToString());
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
	}
}
