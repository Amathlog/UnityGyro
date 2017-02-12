using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Server : MonoBehaviour {

    private string IPAdress = "192.168.43.76";
    private int port = 4444;
    private Dictionary<int, DeviceInfo> registeredDevices;
    private HashSet<int> clientConfirmation;
    public bool doneWaiting = true;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this.gameObject);
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
        Debug.Log(getNumberRegisteredDevices());
        this.registeredDevices.Add(newDevice.id, newDevice);
        Debug.Log(getNumberRegisteredDevices());
    }

    public DeviceInfo getRegisteredDevice(int id) {
        Debug.Log(getNumberRegisteredDevices());
        DeviceInfo res;
        if (this.registeredDevices.TryGetValue(id, out res))
            return res;
        return null;
    }

    public int getNumberRegisteredDevices() {
        return this.registeredDevices.Count;
    }

    public void requestSceneChange(int sceneNumber) {
        doneWaiting = false;
        SceneManager.LoadScene(sceneNumber);
        RequestSceneChangeMessage msg = new RequestSceneChangeMessage();
        msg.serverSpeaking = true;
        msg.sceneNumber = sceneNumber;
        clientConfirmation = new HashSet<int>();
        RegisterHandler(RequestSceneChangeMessage.id, OnReceivedSceneChangeConfirmation);
        SendMessageToAllClients(RequestSceneChangeMessage.id, msg);
    }

    public void OnReceivedSceneChangeConfirmation(NetworkMessage netMsg) {
        RequestSceneChangeMessage msg = netMsg.ReadMessage<RequestSceneChangeMessage>();
        if (!msg.serverSpeaking && msg.done)
            clientConfirmation.Add(netMsg.conn.connectionId);
        if(clientConfirmation.Count == registeredDevices.Count)
            doneWaiting = true;
    }

    
}
