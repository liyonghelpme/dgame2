using UnityEngine;
using System.Collections;

public class mipmap : MonoBehaviour {
public float Mipmap;
	// Use this for initialization
	void Start () {
	this.renderer.material.mainTexture.mipMapBias = this.Mipmap;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
