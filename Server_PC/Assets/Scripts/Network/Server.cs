using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {

    private string IPAdress = "192.168.43.76";
    private int port = 4444;
    private Dictionary<int, DeviceInfo> registeredDevices;
    [SerializeField] private Calibration calibration;
    [SerializeField] private GameObject target;
    [SerializeField] private bool targetDebug = true;

    // Use this for initialization
    void Start () {
        SetupServer();
        this.registeredDevices = new Dictionary<int, DeviceInfo>();
    }


    // Create a server and listen on a port
    private void SetupServer() {
        Debug.Log("Starting Server...");
        NetworkServer.Listen(port);
        Debug.Log("Server Started... Waiting for clients");
        NetworkServer.RegisterHandler(MsgType.Connect, OnConnectedClient);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnectedClient);
        NetworkServer.RegisterHandler(RegisterHostMessage.id, OnReceivedRegisterHostMessage);
        NetworkServer.RegisterHandler(ActionMessage.id, OnReceivedActionMessage);
        NetworkServer.RegisterHandler(CalibrationMessage.id, calibration.OnCalibrationMessageReceived);
    }

    // client function
    private void OnConnectedClient(NetworkMessage netMsg) {
        Debug.Log("Connected to client");
    }

    // client function
    private void OnDisconnectedClient(NetworkMessage netMsg) {
        Debug.Log("Disconnected to client ");
        this.registeredDevices.Remove(netMsg.conn.connectionId);
    }

    private void OnReceivedRegisterHostMessage(NetworkMessage netMsg) {
        RegisterHostMessage msg = netMsg.ReadMessage<RegisterHostMessage>();
        Debug.Log("Message received : ");
        Debug.Log(msg.ToString());

        DeviceInfo newDevice = new DeviceInfo(netMsg.conn.connectionId, msg.deviceName, msg.accelerometerCompatible);
        Debug.Log("ConnectionID : " + newDevice.id);
        this.registeredDevices.Add(newDevice.id, newDevice);
    }

    private void OnReceivedActionMessage(NetworkMessage netMsg) {
        ActionMessage msg = netMsg.ReadMessage<ActionMessage>();
       
        if (msg.triggered) {
            GameObject.Find("SpawnTarget").GetComponent<SpawnTarget>().Spawn(msg.position, registeredDevices[netMsg.conn.connectionId].target.GetComponent<SpriteRenderer>().color);
            Debug.Log("Message received from device : " + registeredDevices[netMsg.conn.connectionId].name);
            Debug.Log(msg.ToString());
        } else{
            if (registeredDevices[netMsg.conn.connectionId].target == null) {
                registeredDevices[netMsg.conn.connectionId].target = Instantiate(target);
                registeredDevices[netMsg.conn.connectionId].target.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
            }
            if (targetDebug)
                registeredDevices[netMsg.conn.connectionId].target.SetActive(true);
            else
                registeredDevices[netMsg.conn.connectionId].target.SetActive(false);
            registeredDevices[netMsg.conn.connectionId].target.transform.position = msg.position;
        }
    }

    public void SendCalibrationMessage(float minX, float minY, float maxX, float maxY, int id) {
        CalibrationMessage msg = new CalibrationMessage();
        msg.minX = minX;
        msg.minY = minY;
        msg.maxX = maxX;
        msg.maxY = maxY;

        NetworkServer.SendToClient(id, CalibrationMessage.id, msg);
    }
}
