using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentHighlight : MonoBehaviour {
	private GameObject sphere;
	private WaitUntil waitForProfPosition;

	void Start () {
		sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		sphere.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
		sphere.GetComponent<Renderer> ().material.color = Color.red;

		waitForProfPosition = 
			new WaitUntil (() => DataModel.ProfessorPointerPosition != new Vector3 (-1f, -1f, -1f));
		StartCoroutine (DisplaySphere ());
	}

	IEnumerator DisplaySphere() {
		yield return waitForProfPosition;

		while (true) {				
			sphere.transform.position = DataModel.ProfessorPointerPosition;
			yield return null;
		}
	}
}
