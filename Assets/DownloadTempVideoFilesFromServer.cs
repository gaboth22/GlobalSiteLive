using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DownloadTempVideoFilesFromServer : MonoBehaviour {

	private uint videoNumber;
	private string videoExtension;
	private Dictionary<string, string> requestHeader;
	private bool runningCoroutine;
	private string rawVideoData;

	void Start () {
		videoNumber = 0;
		videoExtension = ".mp4";
		requestHeader = new Dictionary<string, string> ();
		requestHeader.Add ("User-Agent", DataModel.RequestUserAgent);
		runningCoroutine = false;
	}

	void Update () {
		if (!DataModel.LocalVideoQueueBusy && 
			DataModel.LocalVideoQueue.Count < 5) {
			StartCoroutine (GetVideoFromServer ());
		}
	}

	IEnumerator GetVideoFromServer() {
		if (!runningCoroutine) {
			runningCoroutine = true;
			for (int i = 0; i < 5; i++) {
				var fullVideoUrl =
					"http://" +
					DataModel.ServerIpAddress +
					":" +
					DataModel.VideoServerPort.ToString () +
					"/video/" +
					videoNumber.ToString () +
					videoExtension;

				while (!(fullVideoUrl.Length > 20)) {
					fullVideoUrl =
						"http://" +
						DataModel.ServerIpAddress +
						":" +
						DataModel.VideoServerPort.ToString () +
						"/video/" +
						videoNumber.ToString () +
						videoExtension;
					yield return null;
				}

				Debug.Log ("Getting video: " + fullVideoUrl);

				rawVideoData = null;
				StartCoroutine (GetVideoRawDataFromUrl (fullVideoUrl));

				while (rawVideoData == null) {
					yield return null;
				}

				var videoRawData = Convert.FromBase64String (rawVideoData);
				var currentVideoLocalPath = Application.temporaryCachePath + videoNumber.ToString () + videoExtension;
				Debug.Log ("Saving temp file to: " + currentVideoLocalPath);
				File.WriteAllBytes (
					currentVideoLocalPath, 
					videoRawData);

				while (!File.Exists (currentVideoLocalPath)) {
					Debug.Log ("Waiting for temp file to be created");
					yield return null;
				}

				while (DataModel.LocalVideoQueueBusy) {
					yield return null;
				}

				DataModel.LocalVideoQueueBusy = true;
				DataModel.LocalVideoQueue.Enqueue (currentVideoLocalPath);
				DataModel.LocalVideoQueueBusy = false;

				videoNumber++;

				yield return null;
			}
			runningCoroutine = false;
			yield return null;
		}

		yield return null;
	}

	IEnumerator GetVideoRawDataFromUrl(string fullVideoUrl) {
		using (WWW www = new WWW(fullVideoUrl, null, requestHeader))
		{
			while (!www.isDone) {
				yield return null;
			}

			if (www.error != null) {
				Debug.Log (www.error);
				rawVideoData = null;
				yield return null;
				StartCoroutine (GetVideoRawDataFromUrl (fullVideoUrl));
			} 
			else {
				rawVideoData = Convert.ToBase64String(www.bytes);
			}
		}

		yield return null;
	}
}
