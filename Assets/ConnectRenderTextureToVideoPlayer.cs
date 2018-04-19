using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ConnectRenderTextureToVideoPlayer : MonoBehaviour {

	void Start () {
		var skyboxRenderTexture = Resources.Load ("SkyboxRenderTexture", typeof(RenderTexture)) as RenderTexture;
		var videoPlayer = GetComponent<VideoPlayer>();
		videoPlayer.playOnAwake = false;
		videoPlayer.targetTexture = skyboxRenderTexture;
		videoPlayer.source = VideoSource.Url;
	}
}
