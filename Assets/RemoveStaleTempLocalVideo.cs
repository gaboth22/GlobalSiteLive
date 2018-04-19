using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Video;
using UnityEngine;

public class RemoveStaleTempLocalVideo : MonoBehaviour {

	private VideoPlayer videoPlayer;

	void Start () {
		videoPlayer = GetComponent<VideoPlayer> ();
	}

	void Update () {
		if (!DataModel.LocalStaleVideoQueueBusy) {
			DataModel.LocalStaleVideoQueueBusy = true;

			if (DataModel.LocalStaleVideoQueue.Count > 0 && videoPlayer.isPlaying) {
				var staleVid = DataModel.LocalStaleVideoQueue.Dequeue ();
				Debug.Log ("Deleting stale vid: " + staleVid);
				File.Delete (staleVid);
			}
			DataModel.LocalStaleVideoQueueBusy = false;
		}
	}
}
