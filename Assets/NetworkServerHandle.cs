using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkServerHandle : MonoBehaviour {

	private ProfessorPointerPositionMessage profPosPointMsg;
	private SlidesInfoMesssage slidesInfoMsg;
	private WaitForSeconds wait100Ms;
	private WaitUntil waitForServerPortToBeSet;
	private Vector3 lastProfessorPointerPosition;
	private int lastSlidesIndex;
	private bool lastSlidesEnabledState;

	void Start () {
		profPosPointMsg = new ProfessorPointerPositionMessage ();
		slidesInfoMsg = new SlidesInfoMesssage ();
		wait100Ms = new WaitForSeconds (0.1f);
		waitForServerPortToBeSet = new WaitUntil (() => DataModel.VideoServerPort != -1);
		StartCoroutine (StartNetworkServer ());
	}

	IEnumerator StartNetworkServer() {
		yield return waitForServerPortToBeSet;

		DataModel.AppStateServerPort = DataModel.VideoServerPort + 1;
		NetworkServer.Listen (DataModel.AppStateServerPort);

		lastProfessorPointerPosition = DataModel.ProfessorPointerPosition;
		lastSlidesEnabledState = DataModel.ShouldDisplaySlides;
		lastSlidesIndex = DataModel.JpgSlideListIndex;

		StartCoroutine (SendPositionMessage ());
		StartCoroutine (SendSlidesInfo ());
	}
	
	IEnumerator SendPositionMessage() {
		while (true) {
			yield return wait100Ms;

			if (lastProfessorPointerPosition != DataModel.ProfessorPointerPosition) {

				lastProfessorPointerPosition = DataModel.ProfessorPointerPosition;
				profPosPointMsg.position = DataModel.ProfessorPointerPosition;
				NetworkServer.SendToAll (CustomMessageType.ProfessorPointerPosition, profPosPointMsg);
			}
		}
	}

	IEnumerator SendSlidesInfo() {
		while (true) {
			yield return wait100Ms;

			if (lastSlidesIndex != DataModel.JpgSlideListIndex ||
			   lastSlidesEnabledState != DataModel.ShouldDisplaySlides) {

				lastSlidesIndex = DataModel.JpgSlideListIndex;
				lastSlidesEnabledState = DataModel.ShouldDisplaySlides;
				slidesInfoMsg.slidesEnabled = lastSlidesEnabledState;
				slidesInfoMsg.slidesIndex = lastSlidesIndex;
				NetworkServer.SendToAll (CustomMessageType.SlidesInfo, slidesInfoMsg);
			}
		}
	}
}
