using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class DataModel {
	public static string RequestUserAgent =
		"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2227.1 Safari/537.36";	
	public static string ServerIpAddress = string.Empty;
	public static int VideoServerPort = -1;
	public static int AppStateServerPort = -1;
	public static NetworkClient ApplicationNetworkClient = null; 
	public static bool LocalVideoQueueBusy = false;
	public static bool LocalStaleVideoQueueBusy = false;
	public static Queue<string> LocalVideoQueue = new Queue<string> ();
	public static Queue<string> LocalStaleVideoQueue = new Queue<string> ();
	public static Vector3 ProfessorPointerPosition = new Vector3(-1f, -1f, -1f);
	public static bool VideoPlaybackEnabled = false;
	public static string YoutubeLiveVideoUrl = string.Empty;
	public static string PdfSlidesInputPath = string.Empty;
	public static List<string> JpgSlideList = new List<string>();
	public static int JpgSlideListIndex = 0;
	public static bool ShouldDisplaySlides = false;
}