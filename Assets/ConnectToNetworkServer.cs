using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectToNetworkServer : MonoBehaviour {

	int connectionRetries;
	const int MaxConnectionRetries = 10;

	void Start() {
		connectionRetries = 0;
		DataModel.ApplicationNetworkClient = new NetworkClient ();
		DataModel.ApplicationNetworkClient.RegisterHandler (MsgType.Connect, OnConnectionEstablished);
		DataModel.ApplicationNetworkClient.RegisterHandler (MsgType.Disconnect, OnDisconnect);
		StartCoroutine (ConnectToServer ());
	}

	IEnumerator ConnectToServer() {
		yield return new WaitUntil (() => DataModel.VideoServerPort != -1);
	
		DataModel.AppStateServerPort = DataModel.VideoServerPort + 1;
		Debug.Log ("Connection to Unity server on port:" + DataModel.AppStateServerPort);
		DataModel.ApplicationNetworkClient.Connect (
			DataModel.ServerIpAddress, 
			DataModel.AppStateServerPort); 
	}

	void OnDisconnect(NetworkMessage msg) {
		if (connectionRetries < MaxConnectionRetries) {
			connectionRetries++;
			Debug.Log ("Connection to server failed. Retry: " + connectionRetries);
			DataModel.ApplicationNetworkClient.Connect (DataModel.ServerIpAddress, DataModel.AppStateServerPort);
		} 
		else {
			Application.Quit ();
		}
	}
		
	void OnConnectionEstablished(NetworkMessage msg) {
		connectionRetries = 0;
		Debug.Log ("Succesfully connected to server");
	}
}