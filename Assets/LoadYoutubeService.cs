using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadYoutubeService : MonoBehaviour {
	private string youtubeServiceUrl = "http://127.0.0.1:50000/start_app";
	private Dictionary<string, string> header;
	private YoutubeServiceData ytData;
	private WaitUntil waitUntilUrlIsSet = 
		new WaitUntil(() => DataModel.YoutubeLiveVideoUrl != string.Empty);

	void Start () {
		header = new Dictionary<string, string> ();
		ytData = new YoutubeServiceData ();

		StartCoroutine (BeginYoutubeService ());
	}

	IEnumerator BeginYoutubeService() {
		yield return waitUntilUrlIsSet;

		header.Add ("User-Agent", DataModel.RequestUserAgent);
		header.Add ("Content-Type", "application/json");
		ytData.serve_port = DataModel.VideoServerPort;
		ytData.video_path = "video";
		ytData.youtube_url = DataModel.YoutubeLiveVideoUrl;
		Debug.Log ("Path to save video:" + ytData.video_path);
		var jsonBytes = 
			System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(ytData));
		
		using (WWW www = new WWW (youtubeServiceUrl, jsonBytes, header)) {
			while (!www.isDone) {
				yield return null;
			}

			Debug.Log ("Youtube service response: " + www.text);
		}
			
	}
}
