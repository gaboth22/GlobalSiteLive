using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour {
	private GameObject sphere;

	void Start () {
		sphere = GameObject.Find ("PointerSphere");
	}

	void Update () {
		DataModel.ProfessorPointerPosition = sphere.transform.position;
	}
}
