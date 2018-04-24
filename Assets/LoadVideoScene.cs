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

		StartCoroutine (CheckIfVideoSceneCanBeLoaded ());
	}

	IEnumerator CheckIfVideoSceneCanBeLoaded() {
		#if UNITY_ANDROID
		yield return new WaitUntil (() => DataModel.VideoServerPort != -1);
		yield return new WaitUntil (() => !string.IsNullOrEmpty(DataModel.ServerIpAddress));
		#else
		yield return new WaitUntil (() => !string.IsNullOrEmpty(DataModel.YoutubeLiveVideoUrl));
		yield return new WaitUntil (() => !string.IsNullOrEmpty(DataModel.PdfSlidesInputPath));
		#endif

		#if UNITY_ANDROID
		SceneManager.LoadScene ("360VideoScene");
		#else
		SceneManager.LoadScene ("360VideoSceneDesktop");
		#endif
	}
}
