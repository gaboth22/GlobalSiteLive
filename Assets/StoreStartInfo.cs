using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreStartInfo : MonoBehaviour {
	InputField youtubeUrlInput;
	InputField pdfSlidesPath;
	Button storeDataButton;

	void Start () {
		youtubeUrlInput = GameObject.Find ("YoutubeUrlInputField").GetComponent<InputField> ();
		pdfSlidesPath = GameObject.Find ("PdfSlidesPathInputField").GetComponent<InputField> ();
		storeDataButton = GameObject.Find ("Button").GetComponent<Button> ();
		storeDataButton.onClick.AddListener (OnButtonClick);
	}

	public void OnButtonClick() {
		var url = youtubeUrlInput.text.ToString ();
		var pdfPath = pdfSlidesPath.text.ToString ();

		// Store youtube URL
		DataModel.ServerIpAddress = "127.0.0.1";
		DataModel.VideoServerPort = 40000;
		DataModel.YoutubeLiveVideoUrl = url;
		DataModel.PdfSlidesInputPath = pdfPath;
		Debug.Log ("Youtube URL: " + url);
		Debug.Log ("PDF slides path: " + pdfPath);
	}
}
