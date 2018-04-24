using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class GetJpgSlidesFromBackend : MonoBehaviour {
	private WaitForSeconds wait20Secs = new WaitForSeconds(20);
	private WaitForSeconds waitHalfASec = new WaitForSeconds (0.5f);
	private int countToAssumeEndOfSlides;
	private string pathToSaveSlides;
	private byte[] rawImageData;
	private int slideNumber;
	private string slideExtension = ".jpg";
	private const int oneHundredK = 100000;

	void Start () {
		countToAssumeEndOfSlides = 0;
		slideNumber = 1;
		pathToSaveSlides = Application.temporaryCachePath;
		StartCoroutine (DelayToStartGettingSlides ());
	}
		
	IEnumerator DelayToStartGettingSlides() {
		yield return wait20Secs;
		yield return StartCoroutine (GetJpgSlides ());
	}

	IEnumerator GetJpgSlides() {
		if (countToAssumeEndOfSlides > 5)
			yield break;

		var slideName = 
			slideNumber.ToString().Length > 2 ? 
			"page-0" + slideNumber.ToString() : 
			"page-00" + slideNumber.ToString();
				
		var fullSlideUrl = string.Empty;
		fullSlideUrl =
			"http://" +
			DataModel.ServerIpAddress +
			":" +
			DataModel.VideoServerPort.ToString () +
			"/slides/" +
			slideName +
			slideExtension;

		Debug.Log ("Getting slide: " + fullSlideUrl);

		rawImageData = null;
		using (UnityWebRequest www = UnityWebRequest.Get(fullSlideUrl))
		{
			www.SetRequestHeader ("User-Agent", DataModel.RequestUserAgent);
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				rawImageData = null;
				countToAssumeEndOfSlides++;
			} 
			else {
				rawImageData = www.downloadHandler.data;
				countToAssumeEndOfSlides = 0;
				Debug.Log ("Img Length: " + rawImageData.Length);
			}
		}

		yield return waitHalfASec;

		if (rawImageData == null) {
			yield return StartCoroutine (GetJpgSlides ());
			yield break;
		}
		else if (rawImageData.Length < oneHundredK) {
			yield return StartCoroutine (GetJpgSlides ());
			yield break;
		}

		var currentImageLocalPath = 
			pathToSaveSlides + "/" + slideName + slideExtension;
		Debug.Log ("Saving slide to: " + currentImageLocalPath);

		File.WriteAllBytes (
			currentImageLocalPath, 
			rawImageData);

		DataModel.JpgSlideList.Add (currentImageLocalPath);

		slideNumber++;
		yield return StartCoroutine (GetJpgSlides());
	}
}
