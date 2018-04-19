using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayGuidanceBox : MonoBehaviour {
	private Image canvasArrow;
	private Image canvasArrowBg;
	private Camera mCam;
	private Vector3 targetPosOnScreen;
	private Vector3 targetPosAbsolute;
	private Vector3 screenCenter;
	private WaitForSeconds waitForSecs;

	void Start () {
		mCam = Camera.main;
		canvasArrow = GameObject.Find ("GuidanceArrow").GetComponent<Image> ();
		canvasArrowBg = GameObject.Find ("GuidanceArrowBg").GetComponent<Image> ();

		StartCoroutine (StartDisplayingGuidanceBox ());
	}

	static bool TargetIsVisible(Vector3 targetPosition) {
		return targetPosition.z > 0 &&
			targetPosition.x > 0 &&
			targetPosition.x < 1 &&
			targetPosition.y > 0 &&
			targetPosition.y < 1;
	}
						
	IEnumerator StartDisplayingGuidanceBox() {

		yield return new WaitUntil (
			() => DataModel.ProfessorPointerPosition != new Vector3(-1f, -1f, -1f));

		while (true) {

			targetPosAbsolute = DataModel.ProfessorPointerPosition;
			targetPosOnScreen = 
				mCam.WorldToScreenPoint(DataModel.ProfessorPointerPosition);

			if (!TargetIsVisible (mCam.WorldToViewportPoint (targetPosAbsolute))) {

				screenCenter = 
					new Vector3 (Screen.width / 2, Screen.height / 2, 0);
				
				var targetAngle = 
					(Mathf.Atan2(
						targetPosOnScreen.x - screenCenter.x,
						Screen.height - targetPosOnScreen.y - screenCenter.y) 
					* Mathf.Rad2Deg) + 90;

				canvasArrow.enabled = true;
				canvasArrowBg.enabled = true;

				if(targetPosOnScreen.z > 0) {
					canvasArrow.transform.localRotation = 
						Quaternion.Euler (0, 0, targetAngle);
				}
			} 
			else {
				canvasArrow.enabled = false;
				canvasArrowBg.enabled = false;
			}
										
			yield return null;
		}
	}
}
