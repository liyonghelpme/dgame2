/// <summary>
/// Character HU.
/// Using for Showing a Healt bar on the character object
/// </summary>


using UnityEngine;
using System.Collections;

public class CharacterHUD : MonoBehaviour {

	public GUISkin Skin;
	public bool AlwayShow;
	public Texture2D Bar_bg,Bar_hp,Bar_sp, Bar_exp, Bar_white;

	//CharacterStatus character;
	MyStatus character;
	void Start () {
		if(this.gameObject.GetComponent<MyStatus>()){
			character = this.gameObject.GetComponent<MyStatus>();	
		}
	}

	void OnGUI(){

		if(Skin!=null)
		GUI.skin = Skin;
		
		if(Camera.main){
		Vector3 screenPos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);	
		var dir	= (Camera.main.transform.position - this.transform.position).normalized;
	    var direction = Vector3.Dot(dir,Camera.main.transform.forward);
	    
		if(direction < 0.6f){
			if(character){
				if(AlwayShow || character.HP < character.HPmax){
					GUI.BeginGroup(new Rect(screenPos.x - 42,Screen.height - screenPos.y + 20,84,28));
					GUI.DrawTexture(new Rect(0,0,84,9),Bar_bg);
					GUI.DrawTexture(new Rect(2,2,(80.0f / character.HPmax) * character.HP,5),Bar_hp);
					if(character.SPmax>0){
						GUI.DrawTexture(new Rect(0,9,84,7),Bar_bg);
						GUI.DrawTexture(new Rect(2,9,(80.0f / character.SPmax) * character.SP,5),Bar_sp);
					}
					
					GUI.EndGroup();
				}
				if(Bar_exp) {
						//GUI.color = Color.white;
						//GUI.skin.label.fontSize = 20;
					GUI.Label(new Rect(Screen.width-140, 5, 100, 50), "LV."+character.LEVEL.ToString(), "newStyle");

					GUI.Label(new Rect(Screen.width-140, 55, 100, 50), "EXP", "newStyle");
						//GUI.skin.label.fontSize = 50;
						//GUI.color = Color.white;

					GUI.DrawTexture(new Rect(Screen.width-90, 55, 84, 20), Bar_white);
					GUI.DrawTexture(new Rect(Screen.width-90, 56, (80.0f/character.EXPmax)*character.EXP, 18), Bar_exp);
				}
			}	
		}
		}
		
	}
}
