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

	IEnumerator StreamVideo() {

		while (DataModel.LocalVideoQueue.Count == 0) {
			yield return null;
		}

		while (DataModel.LocalVideoQueueBusy) {
			yield return null;
		}

		DataModel.LocalVideoQueueBusy = true;

		if (DataModel.LocalVideoQueue.Count > 0) {
			videoPlayer.url = DataModel.LocalVideoQueue.Dequeue ();
			DataModel.LocalVideoQueueBusy = false;
			videoPlayer.Prepare ();

			while (!videoPlayer.isPrepared) {
				yield return null;
			}
			
			videoPlayer.Play ();
		}
		DataModel.LocalVideoQueueBusy = false;
		yield return null;
	}

	void LoadNewVideo(VideoPlayer videoPlayer) {
		DataModel.LocalStaleVideoQueueBusy = true;
		DataModel.LocalStaleVideoQueue.Enqueue (videoPlayer.url);
		DataModel.LocalStaleVideoQueueBusy = false;

		StartCoroutine (StreamVideo());
	}
}