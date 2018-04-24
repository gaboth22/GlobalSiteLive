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

	void Start () {
		countToAssumeEndOfSlides = 0;
		slideNumber = 1;
		pathToSaveSlides = Application.temporaryCachePath;
		StartCoroutine (DelayToStartGettingSlides ());
	}
		
	IEnumerator DelayToStartGettingSlides() {
		yield return wait20Secs;
		StartCoroutine (GetJpgSlides ());
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
		StartCoroutine (GetImageRawData (fullSlideUrl));

		yield return waitHalfASec;

		if (rawImageData == null) {
			StartCoroutine (GetJpgSlides ());
			yield break;
		}
		if (rawImageData.Length < 100) {
			StartCoroutine (GetJpgSlides ());
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
		StartCoroutine (GetJpgSlides());
		yield break;
	}

	IEnumerator GetImageRawData(string fullVideoUrl) {
		using (UnityWebRequest www = UnityWebRequest.Get(fullVideoUrl))
		{
			www.SetRequestHeader ("User-Agent", DataModel.RequestUserAgent);
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				rawImageData = null;
				countToAssumeEndOfSlides++;
			} 
			else {
				rawImageData = www.downloadHandler.data;
				Debug.Log ("Img Length: " + rawImageData.Length);
			}
		}

		yield return null;
	}
}
