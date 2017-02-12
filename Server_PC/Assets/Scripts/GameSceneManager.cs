﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameSceneManager : MonoBehaviour {

    public Server server;
    public Calibration calibration;
    public GameObject target;
    [SerializeField] private bool targetDebug = true;

    // Use this for initialization
    void Start () {
        server.SetupServer();
        server.RegisterHandler(ActionMessage.id, OnReceivedActionMessage);
        server.RegisterHandler(CalibrationMessage.id, calibration.OnCalibrationMessageReceived);
    }
	
    private void OnReceivedActionMessage(NetworkMessage netMsg) {
        ActionMessage msg = netMsg.ReadMessage<ActionMessage>();
        DeviceInfo device = server.getRegisteredDevice(netMsg.conn.connectionId);

        if (msg.triggered) {
            GameObject.Find("SpawnTarget").GetComponent<SpawnTarget>().Spawn(msg.position, device.target.GetComponent<SpriteRenderer>().color);
            Debug.Log("Message received from device : " + device.name);
            Debug.Log(msg.ToString());
        } else {
            if (device.target == null) {
                device.target = Instantiate(target);
                device.target.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
            }
            if (targetDebug)
                device.target.SetActive(true);
            else
                device.target.SetActive(false);
            device.target.transform.position = msg.position;
        }
    }

    public void SendCalibrationMessage(float minX, float minY, float maxX, float maxY, int id) {
        CalibrationMessage msg = new CalibrationMessage();
        msg.minX = minX;
        msg.minY = minY;
        msg.maxX = maxX;
        msg.maxY = maxY;

        Debug.Log("MinX = " + minX + "MinY = " + minY + "MaxX = " + maxX + "MaxY = " + maxY);

        NetworkServer.SendToClient(id, CalibrationMessage.id, msg);
    }
}
