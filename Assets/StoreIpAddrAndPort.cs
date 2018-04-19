using System;
using UnityEngine;
using UnityEngine.UI;

public class StoreIpAddrAndPort : MonoBehaviour {
	InputField serverIp;
	InputField serverPort;
	Button storeServerDataButton;

	void Start () {
		serverIp = GameObject.Find("IpAddressInputField").GetComponent<InputField>();
		serverPort = GameObject.Find ("PortInputField").GetComponent<InputField>();
		storeServerDataButton = GameObject.Find ("Button").GetComponent<Button>();
		storeServerDataButton.onClick.AddListener (OnButtonClick);
	}

	public void OnButtonClick() {
		var serverIpAddr = serverIp.text.ToString ();
		var port = Convert.ToInt32(serverPort.text.ToString());

		// Store user input server IP and port
		DataModel.ServerIpAddress = serverIpAddr;
		DataModel.VideoServerPort = port;

		Debug.Log ("Server IP: " + serverIpAddr);
		Debug.Log ("Server Port: " + port);
	}
}
