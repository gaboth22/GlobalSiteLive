using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BroadcastStartPlaybackMessage : MonoBehaviour {
	private bool showStartInfo;
	private bool broadcastSent;
	private string localIp;
	private StartPlaybackMessage msg;

	void Start() {
		showStartInfo = true;
		broadcastSent = false;
		localIp = GetLocalIpAddress ();
		msg = new StartPlaybackMessage ();
	}

	string GetLocalIpAddress() {
		string hostName = Dns.GetHostName();
		return Dns.GetHostByName(hostName).AddressList[0].ToString(); 
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
			GUI.Box (new Rect (40, 30, 150, 20), "Connected: " + Network.connections.Length);
			GUI.Box (new Rect (40, 50, 150, 40), "Share IP and port \nwith students");
			GUI.Box (new Rect (40, 90, 150, 20), "IP: " + localIp);
			GUI.Box (new Rect (40, 110, 150, 20), "Port: " + DataModel.VideoServerPort);
			GUI.Box (new Rect (40, 130, 150, 20), "Press S to start");
		}
	}
}
