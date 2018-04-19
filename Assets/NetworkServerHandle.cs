using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkServerHandle : MonoBehaviour {

	private ProfessorPointerPositionMessage msg;
	private WaitForSeconds waitForSecs;
	private WaitUntil waitForServerPortToBeSet;

	void Start () {
		msg = new ProfessorPointerPositionMessage ();
		waitForSecs = new WaitForSeconds (0.1f);
		waitForServerPortToBeSet = new WaitUntil (() => DataModel.VideoServerPort != -1);
		StartCoroutine (StartNetworkServer ());
	}

	IEnumerator StartNetworkServer() {
		yield return waitForServerPortToBeSet;

		DataModel.AppStateServerPort = DataModel.VideoServerPort + 1;
		NetworkServer.Listen (DataModel.AppStateServerPort);

		StartCoroutine (SendPositionMessage ());
	}
	
	IEnumerator SendPositionMessage() {
		while (true) {
			yield return waitForSecs;
			msg.position = DataModel.ProfessorPointerPosition;
			NetworkServer.SendToAll (CustomMessageType.ProfessorPointerPosition, msg);
		}
	}
}
