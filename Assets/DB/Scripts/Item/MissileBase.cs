using UnityEngine;
using System.Collections;

public class MissileBase : MonoBehaviour {

	public int Damage;
	public GameObject Owner;
	public GameObject ExplosiveObject;
	public float Speed = 4;
	public float LifeTime = 3;
	
	void Start(){
		Destroy(this.gameObject,LifeTime);	
	}
	
	void Update(){
		if(this.gameObject.rigidbody){
			this.gameObject.rigidbody.velocity = this.gameObject.transform.forward * Speed;
		}else{
			this.gameObject.transform.position += this.gameObject.transform.forward * Speed * Time.deltaTime;
		}
	}

	//other dead then not attack
	//collision with other player
	void OnTriggerEnter(Collider other) {
		//Debug.Log(other.tag+" VS "+Owner.gameObject.tag+"    "+other.gameObject.name+" _ "+this.Owner.gameObject.name);
		if(other && Owner){
			//enemy attack other building or anything which stop attack
			//Debug.Log("missile other is "+other.gameObject.ToString()+" tag "+other.tag+" "+Owner.gameObject.tag);
			if(other.tag != Owner.gameObject.tag && other.tag != "God"){
				if(ExplosiveObject){
					GameObject expspawned = (GameObject)GameObject.Instantiate(ExplosiveObject,this.transform.position,this.transform.rotation);
					GameObject.Destroy(expspawned,2);
				}

				if(other.gameObject.GetComponent<MyStatus>()){
					other.gameObject.GetComponent<MyStatus>().ApplyDamage(Damage,Vector3.zero,Owner);
				}

				GameObject.Destroy(this.gameObject);
			}
		}
	}

}
