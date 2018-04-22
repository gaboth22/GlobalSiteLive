using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadTempVideoFilesFromServer : MonoBehaviour {

	private uint videoNumber;
	private string videoExtension;
	private byte[] rawVideoData;
	private WaitForSeconds waitHalfASec = new WaitForSeconds(0.5f);
	private WaitForSeconds waitTwoSecs = new WaitForSeconds(2);
	private WaitUntil waitForQueue = new WaitUntil(() => DataModel.LocalVideoQueue.Count < 5);

	void Start () {
		videoNumber = 0;
		videoExtension = ".mp4";
		Debug.Log ("Searching stale files in: " + Application.temporaryCachePath);
		var tempFiles = 
			Directory.GetFiles (Application.temporaryCachePath, "*.mp4", SearchOption.TopDirectoryOnly);
		Debug.Log("Found files: " + tempFiles.Length);
		for (int i = 0; i < tempFiles.Length; i++) {
			Debug.Log (tempFiles [i]);
				File.Delete(tempFiles[i]);
		}

		StartCoroutine (GetVideoFromServer ());
	}

	IEnumerator GetVideoFromServer() {
		while (true) {
			yield return waitForQueue;

			var fullVideoUrl = string.Empty;
			fullVideoUrl =
			"http://" +
			DataModel.ServerIpAddress +
			":" +
			DataModel.VideoServerPort.ToString () +
			"/video/" +
			videoNumber.ToString () +
			videoExtension;
		
			Debug.Log ("Getting video: " + fullVideoUrl);

			rawVideoData = null;
			StartCoroutine (GetVideoRawDataFromUrl (fullVideoUrl));

			yield return waitHalfASec;

			if (rawVideoData == null) {
				yield return waitTwoSecs;
				StartCoroutine (GetVideoFromServer ());
				yield break;
			}
			if (rawVideoData.Length < 100) {
				yield return waitTwoSecs;
				StartCoroutine (GetVideoFromServer ());
				yield break;
			}
			
			var currentVideoLocalPath = Application.temporaryCachePath + "/" + videoNumber.ToString () + videoExtension;
			Debug.Log ("Saving temp file to: " + currentVideoLocalPath);

			File.WriteAllBytes (
				currentVideoLocalPath, 
				rawVideoData);

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
	}

	IEnumerator GetVideoRawDataFromUrl(string fullVideoUrl) {
		using (UnityWebRequest www = UnityWebRequest.Get(fullVideoUrl))
		{
			www.SetRequestHeader ("User-Agent", DataModel.RequestUserAgent);
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				rawVideoData = null;
			} 
			else {
				rawVideoData = www.downloadHandler.data;
				Debug.Log ("Length: " + rawVideoData.Length);
			}
		}

		yield return null;
	}
}
