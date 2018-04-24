﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SlidesController : MonoBehaviour {

	private Material slidesMat; 
	private GameObject slides;
	private GameObject leftArrowSphere;
	private GameObject rightArrowSphere;
	private bool slidesState;
	private Texture2D slidesText;
	private int lastSlidesIndex;
	private bool showingFullScreenSlides;
	private Camera slidesCam;
	private Camera mCam;

	void Start () {
		slidesMat = Resources.Load ("SlidesMaterial", typeof(Material)) as Material;
		slides = GameObject.Find("Slides");
		slides.SetActive (DataModel.ShouldDisplaySlides);
		slidesText = new Texture2D (10, 10);
		lastSlidesIndex = DataModel.JpgSlideListIndex;
		leftArrowSphere = GameObject.Find ("BackButtonSphere");
		rightArrowSphere = GameObject.Find ("FwdButtonSphere");
		leftArrowSphere.SetActive (DataModel.ShouldDisplaySlides);
		rightArrowSphere.SetActive (DataModel.ShouldDisplaySlides);
		showingFullScreenSlides = false;
		slidesCam = GameObject.Find ("SlidesCamera").GetComponent<Camera> ();
		slidesCam.enabled = false;
		mCam = Camera.main;
		StartCoroutine (PopulateFirstSlide());
	}

	void SetTextureImage() {
		var rawSlide = 
			File.ReadAllBytes (DataModel.JpgSlideList [DataModel.JpgSlideListIndex]);
		slidesText.LoadImage (rawSlide);
		slidesMat.SetTexture ("_MainTex", slidesText);
	}

	IEnumerator PopulateFirstSlide() {
		yield return new WaitUntil(() => DataModel.JpgSlideList.Count > 0);
		SetTextureImage ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.S)) {
			DataModel.ShouldDisplaySlides = !DataModel.ShouldDisplaySlides;
			slides.SetActive (DataModel.ShouldDisplaySlides);
			leftArrowSphere.SetActive (DataModel.ShouldDisplaySlides);
			rightArrowSphere.SetActive (DataModel.ShouldDisplaySlides);
		}
		if (lastSlidesIndex != DataModel.JpgSlideListIndex) {
			lastSlidesIndex = DataModel.JpgSlideListIndex;
			SetTextureImage ();
		}
		if ( Input.GetMouseButtonDown (0)){ 
			if (showingFullScreenSlides) {
				showingFullScreenSlides = false;
				SwitchCameras ();
			} 
			else {
				RaycastHit hit; 
				Ray ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
				if (Physics.Raycast (ray, out hit, 100.0f)) {
					Debug.Log ("Raycast hit");
					if (hit.transform.gameObject == leftArrowSphere) {
						DecreaseIndex ();
					} 
					else if (hit.transform.gameObject == rightArrowSphere) {
						IncreaseIndex ();
					}
					else if (hit.transform.gameObject == slides) {
						showingFullScreenSlides = true;
						SwitchCameras ();
					}
				}
			}
		}
	}

	void DecreaseIndex() {
		Debug.Log("Clicked on left slides button");
		if (DataModel.JpgSlideListIndex > 0) {
			DataModel.JpgSlideListIndex--;
		}
	}

	void IncreaseIndex() {
		Debug.Log("Clicked on right slides button");
		if (DataModel.JpgSlideListIndex < DataModel.JpgSlideList.Count - 1) {
			DataModel.JpgSlideListIndex++;
		}
	}

	void SwitchCameras() {
		if (mCam.enabled) {
			slidesCam.enabled = true;
			mCam.enabled = false;
		} 
		else {
			mCam.enabled = true;
			slidesCam.enabled = false;
		}
	}
}