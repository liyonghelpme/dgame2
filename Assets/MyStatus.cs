using UnityEngine;
using System.Collections;
public enum WeaponType {
	Melee = 0,
	Ranged = 1,
};
public class MyStatus : Photon.MonoBehaviour {
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

	[HideInInspector]
	public int wp = 0;

	//[HideInInspector]
	//public float Radius = 1;

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
	public WeaponType wtype = WeaponType.Melee;

	public GameObject FloatingText;

	float lastRegen = 0;

	public GameObject LevelUpFx;

	public bool isHero = false;
	[HideInInspector]
	public float frozeTime = 0;
	bool hited = false;
	public AudioClip hitSound;
	[HideInInspector]
	public int targetViewId = -1;
	CharacterNet cn;
	ZombieNet zn;
	// Use this for initialization
	void Start () {
		cn = GetComponent<CharacterNet>();
		zn = GetComponent<ZombieNet>();
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
	public void ShowMove() {
		if(cn != null) {
			if((cn.correctPos-transform.position).sqrMagnitude > 0.001f)
				animation.CrossFade("run");
			else
				animation.CrossFade("idle");
		}else if(zn != null) {
			if((zn.correctPos-transform.position).sqrMagnitude > 0.001f)
				animation.CrossFade("run");
			else
				animation.CrossFade("idle");
		}
	}

	//other give Hurt
	//me giveHurt
	//other player kill zombie
	public void getDamage(GameObject k, int damval) {
		killer = k;

		Debug.Log("player get damval");
		//GotHit(0.5f);
		AddFloaingText(transform.position, damval.ToString());
		AddParticle(transform.position+Vector3.up);
		HP -= damval;

		GotHit(0.5f);
		if(hitSound){
			AudioSource.PlayClipAtPoint(hitSound, transform.position);
		}

		if(HP <= 0 && PhotonNetwork.isMasterClient) {
			Dead();
		}
	}

	//master server calculate damage
	//send result to other player
	public int ApplyDamage(int damage, Vector3 dirdamage, GameObject attacker) {
		if(HP < 0)
			return 0;

		GotHit(0.5f);
		if(hitSound){
			AudioSource.PlayClipAtPoint(hitSound, transform.position);
		}

		//master control Zombie attack other user otheruser get hurt
		/*
		if(isHero) {
			if(photonView.isMine) {
				//return 0;
			}else {
				//do harm to other player
				photonView.RPC("doDamage");
			}
		}
		*/


		//TODO: GotHit
		var damval = damage-Defend;
		Debug.Log("show Damage "+damval.ToString());
		if(damval < 1)
			damval = 1;
		killer = attacker;
		HP -= damval;
		AddFloaingText(transform.position, damval.ToString());

		velocityDamage = dirdamage;

		//zombie attack master 
		//zombie attack other player

		//zombie attack other player other player just show hurt not Destory it
		//only change its HP 
		//only master run this code

		//other player kill zombie


		//master player kill zombie
		//zombie kill other player
		//zombie kill master player
		//server make player dead then player get notify
		//only Master can kill objects changeStage client can't do that
		/*
		if(HP <= 0 && PhotonNetwork.isMasterClient ){
			//if(PhotonNetwork.isMasterClient)
			Dead();
		}
		*/

		return damval;
	}

	//kill Zombie
	//kill hero 

	//zombie master player  killBy masterServer
	//other player kill by other user

	//master tell client zombie killed or player killed 

	//only master can destroy zombie will receive kill information
 	
	//kill zombie  by master player or by client player

	//kill player  master zombie kill other player

	//master zombie kill master player
	//why client not receive killMe?
	[RPC]
	void killMe(int oid) {
		if(!isDead) {
			isDead = true;
			if(DeadModel) {
				var deadBody = (GameObject)Instantiate(DeadModel, transform.position, transform.rotation);
				CopyTransformRecurse(transform, deadBody.transform);
				Destroy(deadBody, 5f);
			}
			//gameObject.SetActive(false);
			hideObj();
			//}
			//gameObject.renderer.enabled = false;
			//Destroy(gameObject, 5);
			//isMasterClient kill this zombie
			destroyZom();
		}
	}
	 
	IEnumerator rzom() {
		yield return new WaitForSeconds(5);
		PhotonNetwork.Destroy(gameObject);
	}
	//only masterClient send destroy message to other zombies
	void destroyZom() {
		if(!isHero && PhotonNetwork.isMasterClient) {
			//Destroy(gameObject, 5);
			StartCoroutine(rzom());
		}
	}

	void hideObj() {
		var rc = GetComponentsInChildren<Renderer>();
		foreach(var i in rc)
			i.enabled = false;
		var cc = GetComponent<CharacterController>();
		if(cc != null)
			cc.enabled = false;
	}
	//TODO: Dead

	void makeCoin() {
		var coin = (GameObject)Instantiate(Resources.Load("Coin"));
		coin.transform.position = transform.position;

	}
	void makeAxe() {
		var axe = (GameObject)Instantiate(Resources.Load("axeAni"));
		axe.transform.position = transform.position+new Vector3(Random.value*0.5f-1, 0, Random.value*0.5f-1);

	}
	//Master Zombie killed by master player
	public void Dead() {
		if(isDead)
			return;

		//kill Zombie
		//kill Zombie or kill hero by whom?
		//if(!isHero) {
		var obj = new object[1];
		//who kill me?
		if(killer != null && killer.GetComponent<PhotonView>()) {
			obj[0] = killer.GetComponent<PhotonView>().viewID;
		}else
			obj[0] = -1;

		//only Master can change state
		if(PhotonNetwork.isMasterClient) {
			photonView.RPC("killMe", PhotonTargets.Others, obj);
			//zombie show coin pick coin
			if(!isHero) {
				var rd = Random.Range(0, 2);

				if(rd == 0) {
					makeCoin();
					photonView.RPC("showCoin", PhotonTargets.Others);
				} 
				rd = Random.Range(0, 10);
				if(rd == 0) {
					makeAxe();
				}
			}
		}
		//}

		isDead = true;

		if(DeadModel) {
			var deadBody = (GameObject)Instantiate(DeadModel, transform.position, transform.rotation);
			CopyTransformRecurse(transform, deadBody.transform);
			Destroy(deadBody, 5f);
		}
		if(GetComponent<DeadNow>())
			GetComponent<DeadNow>().OnDead();
		GiveExpToKiller();
		//wait For network syn at last remove object
		hideObj();
			//gameObject.renderer.enabled = false;
			//gameObject.SetActive(false);
			//Destroy(gameObject, 5);

		//only when I'am master I will delete gameobject
		//master make it
		//zombie dead destroy after 5 seconds

		destroyZom();

		//heroDead just setActive false
		//masterHeroDead notify others
		//other player dead notify masterHero
	}

	void GiveExpToKiller() {
		if(killer) {
			if(killer.GetComponent<MyStatus>()) {
				killer.GetComponent<MyStatus>().ApplyExp(EXPgive);
			}
		}
	}

	//only master can calculate exp
	//zombiew can't get exp
	public void ApplyExp(int ad) {
		if(PhotonNetwork.isMasterClient && isHero) {
			EXP += ad;
			bool l = false;
			while(EXP >= EXPmax) {
				LevelUp();
				l = true;
			}
			if(l){
				var objs = new object[2];
				objs[0] = LEVEL;
				objs[1] = EXP;
				photonView.RPC("getLevel", PhotonTargets.Others, objs);
			}
		}
	}

	[RPC]
	void getLevel(int nl, int exp) {
		while(LEVEL < nl) {
			LevelUp();
		}
		EXP = exp;
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
		if(isDead)
			return;
	
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
		//2 rate axe
		if(wp == 1) {
			Damage *= 2;
		}
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
