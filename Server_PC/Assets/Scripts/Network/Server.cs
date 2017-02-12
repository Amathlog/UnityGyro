using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {

    private string IPAdress = "192.168.43.76";
    private int port = 4444;
    private Dictionary<int, DeviceInfo> registeredDevices;

    // Use this for initialization
    void Start () {
        SetupServer();
        this.registeredDevices = new Dictionary<int, DeviceInfo>();
    }

    public void SendMessageToAllClients(short id, MessageBase msg) {
        NetworkServer.SendToAll(id, msg);
    }


    // Create a server and listen on a port
    public void SetupServer() {
        if (NetworkServer.listenPort != port) {
            Debug.Log("Starting Server...");
            NetworkServer.Listen(port);
            Debug.Log("Server Started... Waiting for clients");
            NetworkServer.RegisterHandler(MsgType.Connect, OnConnectedClient);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnectedClient);
            NetworkServer.RegisterHandler(RegisterHostMessage.id, OnReceivedRegisterHostMessage);
        }
    }

    public void RegisterHandler(short id, NetworkMessageDelegate callback) {
        NetworkServer.RegisterHandler(id, callback);
    }

    // client function
    private void OnConnectedClient(NetworkMessage netMsg) {
        Debug.Log("Connected to client");
    }

    // client function
    private void OnDisconnectedClient(NetworkMessage netMsg) {
        Debug.Log("Disconnected to client ");
        this.registeredDevices.Remove(netMsg.conn.connectionId);
        Debug.Log("Unregistered : " + netMsg.conn.connectionId);
    }

    private void OnReceivedRegisterHostMessage(NetworkMessage netMsg) {
        RegisterHostMessage msg = netMsg.ReadMessage<RegisterHostMessage>();
        Debug.Log("Message received : ");
        Debug.Log(msg.ToString());

        DeviceInfo newDevice = new DeviceInfo(netMsg.conn.connectionId, msg.deviceName, msg.accelerometerCompatible);
        Debug.Log("ConnectionID : " + newDevice.id);
        this.registeredDevices.Add(newDevice.id, newDevice);
    }

    public DeviceInfo getRegisteredDevice(int id) {
        DeviceInfo res;
        if (registeredDevices.TryGetValue(id, out res))
            return res;
        return null;
    }

    public int getNumberRegisteredDevices() {
        return registeredDevices.Count;
    }

    
}
