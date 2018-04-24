using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class StreamVideoIntoSkybox : MonoBehaviour {
	private VideoPlayer videoPlayer;
	private bool shouldAttemptToPlayFirstVideo;
	private WaitUntil waitForQueueToHaveData;
	private WaitUntil waitForQueueNotToBeBusy;

	void Start() {
		videoPlayer = GetComponent<VideoPlayer> ();
		videoPlayer.loopPointReached += LoadNewVideo;
		videoPlayer.errorReceived += SkipVideo;
		shouldAttemptToPlayFirstVideo = true;
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

			while (!File.Exists (currentVideoToPlay)) {
				yield return null;
			}

			videoPlayer.url = currentVideoToPlay;
			videoPlayer.Prepare ();

			while (!videoPlayer.isPrepared) {
				yield return null;
			}
			
			videoPlayer.Play ();
		} 
		else {
			DataModel.LocalVideoQueueBusy = false;
		}
		yield break;
	}

	void LoadNewVideo(VideoPlayer videoPlayer) {
		DataModel.LocalStaleVideoQueueBusy = true;
		DataModel.LocalStaleVideoQueue.Enqueue (videoPlayer.url);
		DataModel.LocalStaleVideoQueueBusy = false;

		StartCoroutine (StreamVideo());
	}
}