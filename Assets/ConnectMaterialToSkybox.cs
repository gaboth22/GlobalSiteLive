using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectMaterialToSkybox : MonoBehaviour {

	void Start () {
		var skyboxMaterial = Resources.Load ("SkyboxMaterial", typeof(Material)) as Material;
		RenderSettings.skybox = (Material)skyboxMaterial;
	}
}
