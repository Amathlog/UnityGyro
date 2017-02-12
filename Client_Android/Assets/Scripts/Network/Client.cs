using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour {

    private NetworkClient myClient = null;
    private string IPAdress = "192.168.43.76";
    private int port = 4444;

    // Use this for initialization
    void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    // Create a client and connect to the server port
    public void SetupClient(NetworkMessageDelegate callbackOnConnect) {
        if (isConnected())
            return;
        
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, callbackOnConnect);
        myClient.RegisterHandler(RequestSceneChangeMessage.id, OnReceivedSceneChangeMessage);
        myClient.Connect(IPAdress, port);
    }

    public bool isConnected() {
        return myClient != null && myClient.isConnected;
    }

    public int getConnectionId() {
        return myClient.connection.connectionId;
    }

    public void RegisterHandler(short id, NetworkMessageDelegate callback) {
        myClient.RegisterHandler(id, callback);
    }

    public void SendMessage(short id, MessageBase msg) {
        myClient.Send(id, msg);
    }

    public void SendRegisterHostMessage(NetworkMessage netMsg) {
        RegisterHostMessage msg = new RegisterHostMessage();
        msg.deviceId = getConnectionId();
        msg.deviceName = SystemInfo.deviceName;
        msg.version = SystemInfo.deviceModel;
        msg.accelerometerCompatible = SystemInfo.supportsAccelerometer;
        SendMessage(RegisterHostMessage.id, msg);
    }

    private void OnDestroy() {
        if (myClient != null && myClient.isConnected)
            myClient.Disconnect();
    }

    public void OnReceivedSceneChangeMessage(NetworkMessage netMsg) {
        myClient.Disconnect();
        RequestSceneChangeMessage msg = netMsg.ReadMessage<RequestSceneChangeMessage>();
        if (msg.serverSpeaking) {
            SceneManager.LoadScene(msg.sceneNumber);
            RequestSceneChangeMessage msgSend = new RequestSceneChangeMessage();
            msgSend.serverSpeaking = false;
            msgSend.done = true;
            SendMessage(RequestSceneChangeMessage.id, msgSend);
        }
    }
}
