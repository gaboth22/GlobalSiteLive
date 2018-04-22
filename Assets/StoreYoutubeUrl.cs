using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreYoutubeUrl : MonoBehaviour {
	InputField youtubeUrlInput;
	Button storeServerDataButton;

	void Start () {
		youtubeUrlInput = GameObject.Find ("YoutubeUrlInputField").GetComponent<InputField> ();
		storeServerDataButton = GameObject.Find ("Button").GetComponent<Button> ();
		storeServerDataButton.onClick.AddListener (OnButtonClick);
	}

	public void OnButtonClick() {
		var url = youtubeUrlInput.text.ToString ();

		// Store youtube URL
		DataModel.ServerIpAddress = "127.0.0.1";
		DataModel.VideoServerPort = 40000;
		DataModel.YoutubeLiveVideoUrl = url;
		Debug.Log ("Youtube URL: " + url);
	}
}
