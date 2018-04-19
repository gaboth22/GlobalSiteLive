using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSphere : MonoBehaviour {
	
	void Start () {
		var sphere = GameObject.Find ("PointerSphere");
		sphere.GetComponent<Renderer> ().material.color = Color.red;
	}
}
