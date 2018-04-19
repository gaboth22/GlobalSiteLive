using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BroadcastStartPlaybackMessage : MonoBehaviour {
	private bool showStartInfo;
	private bool broadcastSent;
	private StartPlaybackMessage msg;

	void Start() {
		showStartInfo = true;
		broadcastSent = false;
		msg = new StartPlaybackMessage ();
	}

	void Update(){
		if (Input.GetKey (KeyCode.S) && !broadcastSent) {
			broadcastSent = true;
			showStartInfo = false;
			msg.start = true;
			NetworkServer.SendToAll (CustomMessageType.StartPlayback, msg);
			DataModel.VideoPlaybackEnabled = true;
		}
	}
		
	void OnGUI() {
		if (showStartInfo) {
			GUI.Box (new Rect (40, 29, 80, 40), "Connected\n" + Network.connections.Length);
			GUI.Box (new Rect (40, 70, 80, 40), "Press S \nto start");
		}
	}
}
