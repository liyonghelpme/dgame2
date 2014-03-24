using UnityEngine;
using System.Collections;

public class MyStatus : MonoBehaviour {
	[HideInInspector]
	public int HP = 100;
	[HideInInspector]
	public int SP = 100;
	[HideInInspector]
	public int HPmax = 100;
	[HideInInspector]
	public int SPmax = 100;
	[HideInInspector]
	public int Damage = 2;
	[HideInInspector]
	public int Defend = 0;

	private GameObject killer;
	private Vector3 velocityDamage = Vector3.zero;
	[HideInInspector]
	public bool isDead = false;

	public GameObject ParticleObject;

	public GameObject DeadModel;
	public bool disappear = true;

	private int EXPgive = 2;
	[HideInInspector]
	public int EXP = 0;
	[HideInInspector]
	public int EXPmax = 2;

	[HideInInspector]
	public int LEVEL = 1;

	int STR = 2;
	int AGI = 2;
	int INT = 2;

	int HPregen = 1;
	int SPregen = 1;
	int BaseHPmax = 16;
	int BaseSPmax = 18;
	int BaseDamage = 1;
	int BaseDefend = 0;

	public GameObject FloatingText;

	float lastRegen = 0;

	public GameObject LevelUpFx;

	public bool isHero = false;
	[HideInInspector]
	public float frozeTime = 0;
	bool hited = false;
	public AudioClip hitSound;
	// Use this for initialization
	void Start () {
		HP = 100;
		SP = 100;
	}

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
		if(hitSound){
			AudioSource.PlayClipAtPoint(hitSound, transform.position);
		}

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
	public void SetLevel(int l) {
		while(LEVEL < l) {
			LevelUp();
		}
	}

	//TODO: damage reculate
	void LevelUp(){
		HP = HPmax;
		EXP -= EXPmax;
		EXPmax += 50;
		LEVEL++;
		STR++;
		AGI++;
		INT++;
		//level Up add More Exp
		EXPgive += 10;

		if(LevelUpFx) {
			GameObject lup = (GameObject)Instantiate(LevelUpFx, transform.position, Quaternion.identity);
			lup.gameObject.transform.parent = transform;
		}
	}


	void CopyTransformRecurse(Transform tp, Transform child){

	}

	//TODO: float Text and particle effect
	public void AddFloaingText(Vector3 pos, string t) {
		if(FloatingText) {
			var floatText = (GameObject)Instantiate(FloatingText, pos, transform.rotation);
			if(floatText.GetComponent<FloatingText>()){
				floatText.GetComponent<FloatingText>().Text = t;
			}
			GameObject.Destroy(floatText, 1);
		}
	}
	public void AddParticle(Vector3 pos) {
		if(ParticleObject) {
			var bloodEffect = (GameObject)Instantiate(ParticleObject, pos, transform.rotation);
			GameObject.Destroy(bloodEffect, 1);
		}
	}

	
	// Update is called once per frame
	void Update () {
		if(Time.time-lastRegen > 1) {
			lastRegen = Time.time;
			HP += HPregen;
			SP += SPregen;
		}
		SPmax = BaseSPmax+(INT*3);
		HPmax = BaseHPmax+(STR*5);
		HPregen = Mathf.Max(1, (int)(HPmax/15));
		SPregen = Mathf.Max(1, (int)(SPmax/15));
		if(HP > HPmax)
			HP = HPmax;
		if(SP > SPmax)
			SP = SPmax;
		Damage = (STR*2)+BaseDamage;
		Defend = BaseDefend;


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
