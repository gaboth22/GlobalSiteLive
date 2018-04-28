using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.IO;

public class StreamVideoIntoSkybox : MonoBehaviour {
	private VideoPlayer videoPlayer;
	private WaitUntil waitForQueueToHaveData;
	private WaitUntil waitForQueueNotToBeBusy;
	#if !(UNITY_ANDROID)
	private VideoSyncMessage msg = new VideoSyncMessage();
	#endif

	void Start() {
		videoPlayer = GetComponent<VideoPlayer> ();
		videoPlayer.loopPointReached += LoadNewVideo;
		videoPlayer.errorReceived += SkipVideo;
		waitForQueueToHaveData = new WaitUntil (() => DataModel.LocalVideoQueue.Count > 0);
		waitForQueueNotToBeBusy = new WaitUntil (() => !DataModel.LocalVideoQueueBusy);
		StartCoroutine (PlayFirstVideo ());
	}
		
	IEnumerator PlayFirstVideo() {
		yield return new WaitUntil (
			() => DataModel.LocalVideoQueue.Count >= 5 && DataModel.VideoPlaybackEnabled);

		yield return StartCoroutine (StreamVideo ());
	}

	void SkipVideo(VideoPlayer vidPlayer, string message) {
		Debug.Log ("Skipping video: " + vidPlayer.url + " because: \n" + message);
		LoadNewVideo (vidPlayer);
	}

	IEnumerator StreamVideo() {

		yield return waitForQueueToHaveData;
		yield return waitForQueueNotToBeBusy;

		DataModel.LocalVideoQueueBusy = true;

		if (DataModel.LocalVideoQueue.Count > 0) {
			var currentVideoToPlay = DataModel.LocalVideoQueue.Dequeue ();
			DataModel.LocalVideoQueueBusy = false;

			#if !(UNITY_ANDROID)
			if (DataModel.VideosPlayedCount >= 8) {
				DataModel.VideosPlayedCount = 0;
				DataModel.CurrentVideoNumber = GetVideoNumberFromName(currentVideoToPlay);
				BrodcastVideoNumber (DataModel.CurrentVideoNumber);
			}
			#endif

			while (!File.Exists (currentVideoToPlay)) {
				yield return null;
			}

			videoPlayer.url = currentVideoToPlay;
			videoPlayer.Prepare ();

			while (!videoPlayer.isPrepared) {
				yield return null;
			}

			#if !(UNITY_ANDROID)
			DataModel.VideosPlayedCount++;
			#endif

			videoPlayer.Play ();
		} 
		else {
			DataModel.LocalVideoQueueBusy = false;
		}
		yield break;
	}

	#if !(UNITY_ANDROID)
	int GetVideoNumberFromName(string name) {
		char[] delimiter = { '\\', '/', '.'};
		string[] brokenDownPath = name.Split (delimiter);
		var videoNumber = brokenDownPath [brokenDownPath.Length - 2]; 
		return Convert.ToInt32 (videoNumber);
	}

	void BrodcastVideoNumber(int videoNumber) {
		msg.currentVideoNumber = videoNumber;
		NetworkServer.SendToAll (CustomMessageType.VideoSync, msg);
	}
	#endif

	void LoadNewVideo(VideoPlayer videoPlayer) {
		DataModel.LocalStaleVideoQueueBusy = true;
		DataModel.LocalStaleVideoQueue.Enqueue (videoPlayer.url);
		DataModel.LocalStaleVideoQueueBusy = false;

		StartCoroutine (StreamVideo());
	}
}