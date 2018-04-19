using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadVideoScene : MonoBehaviour {

	Button serverDataInputDoneButton;

	void Start() {
		serverDataInputDoneButton = GameObject.Find ("Button").GetComponent<Button>();
		serverDataInputDoneButton.onClick.AddListener (OnButtonClick);
	}

	public void OnButtonClick() {
		Debug.Log ("Loading video scene");

		StartCoroutine (CheckIfServerDataHasBeenSaved ());
	}

	IEnumerator CheckIfServerDataHasBeenSaved() {
		yield return new WaitUntil (() => DataModel.VideoServerPort != -1);
		yield return new WaitUntil (() => !string.IsNullOrEmpty(DataModel.ServerIpAddress));

		#if UNITY_ANDROID
		SceneManager.LoadScene ("360VideoScene");
		#elif UNITY_IOS
		SceneManager.LoadScene ("360VideoScene");
		#else
		SceneManager.LoadScene ("360VideoSceneDesktop");
		#endif
	}
}
