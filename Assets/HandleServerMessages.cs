using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HandleServerMessages : MonoBehaviour {

	void Start () {
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
}