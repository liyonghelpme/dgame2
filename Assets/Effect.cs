using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour {
	public GameObject cs, ps1, ps2;
	public int x, y;
	public bool hasGod = false;
	// Use this for initialization
	void Start () {
		//showEffect();
	}
	public void showEffect(){
		hasGod = true;
		Color c = new Color(0, 251/255.0f, 2/255.0f);
		cs.renderer.material.SetColor("_TintColor", c);
		ps1.renderer.material.SetColor("_TintColor", c);
		ps2.renderer.material.SetColor("_TintColor", c);
	}
	public void hideEffect() {
		hasGod = false;
		Color c = new Color(75/255.0f, 77/255.0f, 75/255.0f);
		cs.renderer.material.SetColor("_TintColor", c);
		ps1.renderer.material.SetColor("_TintColor", c);
		ps2.renderer.material.SetColor("_TintColor", c);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
