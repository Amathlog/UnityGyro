using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quobject.SocketIoClientDotNet.Client;

public class SocketB : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Socket socket = IO.Socket("http://localhost:3000");
		socket.On(Socket.EVENT_CONNECT, () =>
			{
				socket.Emit("hi", "Hello");

			});

		socket.On("hi", (data) =>
			{
				Debug.Log(data);
				socket.Disconnect();
			});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
