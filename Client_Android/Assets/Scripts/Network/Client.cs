using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Client : MonoBehaviour {

    private NetworkClient myClient = null;
    private bool connected = false;
    private string IPAdress = "192.168.43.76";
    private int port = 4444;
    private MobileSensor mobileSensor;

    // Use this for initialization
    void Start() {
        mobileSensor = GameObject.Find("MobileSensor").GetComponent<MobileSensor>();
        GameObject.Find("MobileButton").GetComponent<Button>().onClick.AddListener(SetupClient);
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update() {
        if (connected && mobileSensor.calibrated && SystemInfo.deviceType != DeviceType.Desktop) {
            //PrintLog(mobileSensor.getCurrentRotation().ToString());
        }
    }

    // Create a client and connect to the server port
    public void SetupClient() {
        PrintLog("Connecting...");
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnConnectedServer);
        myClient.RegisterHandler(CalibrationMessage.id, mobileSensor.OnCalibrationMessageReceived);
        myClient.Connect(IPAdress, port);
    }


    // server function
    private void OnConnectedServer(NetworkMessage netMsg) {
        PrintLog("Connected to server");
        connected = true;

        GameObject button = GameObject.Find("MobileButton");

        button.SetActive(false);
        mobileSensor.calibrateButton.gameObject.SetActive(true);
        //button.GetComponentInChildren<Text>().text = "Send Data";
        //button.GetComponent<Button>().onClick.RemoveAllListeners();
        //button.GetComponent<Button>().onClick.AddListener(SendActionData);

        RegisterHostMessage msg = new RegisterHostMessage();
        msg.deviceId = myClient.connection.connectionId;
        msg.deviceName = SystemInfo.deviceName;
        msg.version = SystemInfo.deviceModel;
        msg.accelerometerCompatible = SystemInfo.supportsAccelerometer;
        myClient.Send(RegisterHostMessage.id, msg);
    }


    // Print to server text
    public void PrintLog(string s) {
        GameObject.Find("ServerText").GetComponent<Text>().text = s;
    }

    public void SendActionData() {
        // Only on mobile
        if (SystemInfo.deviceType == DeviceType.Desktop) {
            Debug.Log("Only on mobile");
            return;
        }

        if (!mobileSensor.calibrated)
            return;

        ActionMessage msg = new ActionMessage();
        msg.idDevice = myClient.connection.connectionId;
        msg.position = mobileSensor.spawnPosition();
        msg.triggered = true;
        myClient.Send(ActionMessage.id, msg);
    }

    public void SendPositionData() {
        // Only on mobile
        if (SystemInfo.deviceType == DeviceType.Desktop) {
            Debug.Log("Only on mobile");
            return;
        }

        if (!mobileSensor.calibrated)
            return;

        ActionMessage msg = new ActionMessage();
        msg.idDevice = myClient.connection.connectionId;
        msg.position = mobileSensor.spawnPosition();
        msg.triggered = false;
        myClient.Send(ActionMessage.id, msg);
    }

    public void SendCalibrationData(bool value) {
        CalibrationMessage msg = new CalibrationMessage();
        msg.enable = value;
        msg.idDevice = myClient.connection.connectionId;
        myClient.Send(CalibrationMessage.id, msg);
    }

    private void OnDestroy() {
        if (myClient != null && myClient.isConnected)
            myClient.Disconnect();
    }
}
