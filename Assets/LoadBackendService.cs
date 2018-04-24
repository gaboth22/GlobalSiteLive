using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadBackendService : MonoBehaviour {
	private string backendServiceUrl = "http://127.0.0.1:50000/start_app";
	private Dictionary<string, string> header;
	private BackendServiceData beData;
	private WaitUntil waitUntilUrlIsSet = 
		new WaitUntil(() => DataModel.YoutubeLiveVideoUrl != string.Empty);
	private WaitUntil waitUntilPdfPathIsSet = 
		new WaitUntil(() => DataModel.PdfSlidesInputPath != string.Empty);

	void Start () {
		header = new Dictionary<string, string> ();
		beData = new BackendServiceData ();

		StartCoroutine (BeginYoutubeService ());
	}

	IEnumerator BeginYoutubeService() {
		yield return waitUntilUrlIsSet;
		yield return waitUntilPdfPathIsSet;

		header.Add ("User-Agent", DataModel.RequestUserAgent);
		header.Add ("Content-Type", "application/json");
		beData.serve_port = DataModel.VideoServerPort;
		beData.video_path = "video";
		beData.youtube_url = DataModel.YoutubeLiveVideoUrl;
		beData.input_slides_path = DataModel.PdfSlidesInputPath;
		beData.output_slides_path = "slides";
		Debug.Log ("Path to save video:" + beData.video_path);
		Debug.Log ("Pdf slides input path: " + beData.input_slides_path);
		var jsonBytes = 
			System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(beData));
		
		using (WWW www = new WWW (backendServiceUrl, jsonBytes, header)) {
			while (!www.isDone) {
				yield return null;
			}

			Debug.Log ("Youtube service response: " + www.text);
		}
			
	}
}
