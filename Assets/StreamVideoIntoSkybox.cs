using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class StreamVideoIntoSkybox : MonoBehaviour {
	private VideoPlayer videoPlayer;
	private bool shouldAttemptToPlayFirstVideo;

	void Start() {
		videoPlayer = GetComponent<VideoPlayer> ();
		videoPlayer.loopPointReached += LoadNewVideo;
		videoPlayer.errorReceived += SkipVideo;
		shouldAttemptToPlayFirstVideo = true;
	}

	void Update() {
		if (shouldAttemptToPlayFirstVideo) {
			if (!DataModel.LocalVideoQueueBusy) {
				DataModel.LocalVideoQueueBusy = true;

				if (DataModel.LocalVideoQueue.Count >= 5 && DataModel.VideoPlaybackEnabled) {
					shouldAttemptToPlayFirstVideo = false;
					DataModel.LocalVideoQueueBusy = false;
					StartCoroutine (StreamVideo ());
				} 
				else {
					DataModel.LocalVideoQueueBusy = false;
				}
			}
		}
	}

	void SkipVideo(VideoPlayer vidPlayer, string message) {
		Debug.Log ("Skipping video: " + vidPlayer.url + " because: \n" + message);
		LoadNewVideo (vidPlayer);
	}

	IEnumerator StreamVideo() {

		while (DataModel.LocalVideoQueue.Count < 2) {
			yield return null;
		}

		while (DataModel.LocalVideoQueueBusy) {
			yield return null;
		}

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
		yield return null;
	}

	void LoadNewVideo(VideoPlayer videoPlayer) {
		DataModel.LocalStaleVideoQueueBusy = true;
		DataModel.LocalStaleVideoQueue.Enqueue (videoPlayer.url);
		DataModel.LocalStaleVideoQueueBusy = false;

		StartCoroutine (StreamVideo());
	}
}