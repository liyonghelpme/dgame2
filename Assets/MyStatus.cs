using UnityEngine;
using System.Collections;

public class MyStatus : MonoBehaviour {
	public int HP = 16;
	public int SP = 18;

	public int HPmax = 16;
	public int SPmax = 18;

	public int Damage = 2;
	public int Defend = 0;

	private GameObject killer;
	private Vector3 velocityDamage = Vector3.zero;
	[HideInInspector]
	public bool isDead = false;

	public GameObject ParticleObject;

	public GameObject DeadModel;
	public bool disappear = true;

	private int EXPgive = 2;
	int EXP = 0;
	int EXPmax = 2;
	int LEVEL = 1;

	int STR = 2;
	int AGI = 2;
	int INT = 2;

	public GameObject LevelUpFx;

	public bool isHero = false;
	[HideInInspector]
	public float frozeTime = 0;
	bool hited = false;

	void GotHit(float t){
		if(!isHero) {
			if(animation["hit"]){
				animation.Play("hit", PlayMode.StopAll);
			}
			frozeTime = t;
			hited = true;
		}
	}


	public int ApplyDamage(int damage, Vector3 dirdamage, GameObject attacker) {
		if(HP < 0)
			return 0;
		GotHit(0.5f);

		//TODO: GotHit
		var damval = damage-Defend;
		Debug.Log("show Damage "+damval.ToString());
		if(damval < 1)
			damval = 1;
		killer = attacker;
		HP -= damval;
		AddFloaingText(transform.position, damval.ToString());

		velocityDamage = dirdamage;
		if(HP <= 0){
			Dead();
		}

		return damval;
	}
	//TODO: Dead
	void Dead() {
		if(isDead)
			return;
		isDead = true;

		if(DeadModel) {
			var deadBody = (GameObject)Instantiate(DeadModel, transform.position, transform.rotation);
			CopyTransformRecurse(transform, deadBody.transform);
			Destroy(deadBody, 5f);
		}
		if(GetComponent<DeadNow>())
			GetComponent<DeadNow>().OnDead();
		GiveExpToKiller();
		//TODO:DeadModel
		if(disappear)
			Destroy(gameObject);
	}
	void GiveExpToKiller() {
		if(killer) {
			if(killer.GetComponent<MyStatus>()) {
				killer.GetComponent<MyStatus>().ApplyExp(EXPgive);
			}
		}
	}

	public void ApplyExp(int ad) {
		EXP += ad;
		while(EXP >= EXPmax) {
			LevelUp();
		}
	}
	//TODO: damage reculate
	void LevelUp(){
		EXP -= EXPmax;
		EXPmax += 50;
		LEVEL++;
		STR++;
		AGI++;
		INT++;

		if(LevelUpFx) {
			GameObject lup = (GameObject)Instantiate(LevelUpFx, transform.position, Quaternion.identity);
			lup.gameObject.transform.parent = transform;
		}
	}


	void CopyTransformRecurse(Transform tp, Transform child){

	}

	//TODO: float Text and particle effect
	public void AddFloaingText(Vector3 pos, string t) {

	}
	public void AddParticle(Vector3 pos) {
		if(ParticleObject) {
			var bloodEffect = (GameObject)Instantiate(ParticleObject, pos, transform.rotation);
			GameObject.Destroy(bloodEffect, 1);
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(hited) {
			if(frozeTime > 0){
				frozeTime -= Time.deltaTime;
			}else {
				hited = false;
				frozeTime = 0;
				animation.Play("idle");
			}
		}
	}
}
