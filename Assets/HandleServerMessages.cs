using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class HandleServerMessages : MonoBehaviour {
	private VideoPlayer videoPlayer;

	void Start () {
		videoPlayer = GetComponent<VideoPlayer> ();
		StartCoroutine (WaitForClientInitialization ());
	}

	IEnumerator WaitForClientInitialization() {
		yield return new WaitUntil (() => DataModel.ApplicationNetworkClient != null);
		StartCoroutine (RegisterHandlers ());
	}
		
	IEnumerator RegisterHandlers() {
		DataModel.ApplicationNetworkClient.RegisterHandler (
			CustomMessageType.ProfessorPointerPosition,
			OnProfessorPointerPositionUpdate);

		DataModel.ApplicationNetworkClient.RegisterHandler (
			CustomMessageType.StartPlayback,
			OnStartPlaybackMessage);

		DataModel.ApplicationNetworkClient.RegisterHandler (
			CustomMessageType.SlidesInfo,
			OnSlidesInfoMessage);

		DataModel.ApplicationNetworkClient.RegisterHandler (
			CustomMessageType.VideoSync,
			OnVideoSyncMessage);
		
		yield return null;
	}

	void OnProfessorPointerPositionUpdate(NetworkMessage netMsg) {
		var msg = netMsg.ReadMessage<ProfessorPointerPositionMessage> ();
		DataModel.ProfessorPointerPosition = msg.position;
		Debug.Log ("Received updated professor pointer position: " +
					DataModel.ProfessorPointerPosition.ToString ());
	}

	void OnStartPlaybackMessage(NetworkMessage netMsg) {
		var msg = netMsg.ReadMessage<StartPlaybackMessage> ();
		if (msg.start) {
			Debug.Log ("Received start playback message");
			DataModel.VideoPlaybackEnabled = true;
		}
	}

	void OnSlidesInfoMessage(NetworkMessage netMsg) {
		var msg = netMsg.ReadMessage<SlidesInfoMesssage> ();
		DataModel.JpgSlideListIndex = msg.slidesIndex;
		DataModel.ShouldDisplaySlides = msg.slidesEnabled;
		Debug.Log ("Received slides update message");
	}

	void OnVideoSyncMessage(NetworkMessage netMsg) {
		var msg = netMsg.ReadMessage<VideoSyncMessage> ();
		DataModel.LocalVideoQueueBusy = true;
		DataModel.LocalStaleVideoQueueBusy = true;

		bool stop = false;
		while (DataModel.LocalVideoQueue.Count > 0 && !stop) {
			var nextVid = DataModel.LocalVideoQueue.Peek ();
			var vidNum = GetVideoNumberFromName (nextVid);

			if (vidNum < msg.currentVideoNumber) {
				DataModel.LocalStaleVideoQueue.Enqueue (
					DataModel.LocalVideoQueue.Dequeue ());
			} 
			else {
				stop = true;
			}
		}

		DataModel.LocalVideoQueueBusy = false;
		DataModel.LocalStaleVideoQueueBusy = false;
	}

	int GetVideoNumberFromName(string name) {
		char[] delimiter = { '\\', '/', '.'};
		string[] brokenDownPath = name.Split (delimiter);
		var videoNumber = brokenDownPath [brokenDownPath.Length - 2]; 
		return Convert.ToInt32 (videoNumber);
	}
}