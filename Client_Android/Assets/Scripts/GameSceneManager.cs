using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour {

    public Client client;
    public MobileSensor mobileSensor;
    private bool calibrated = false;
    private Button calibrateButton;
    private bool waitForPositionData = false;
    private bool waitForFireAgain = false;
    private CalibrateImageManager calImgMng;
    [SerializeField] float timeBetweenTwoPositions = 0.1f;
    [SerializeField] float timeBetweenFire = 0.7f;


    // Use this for initialization
    void Start () {
        GameObject.Find("MobileButton").GetComponent<Button>().onClick.AddListener(SetupClient);
        calImgMng = GameObject.Find("CalibrateDir").GetComponent<CalibrateImageManager>();
        calibrateButton = GameObject.Find("Calibrate").GetComponent<Button>();
        calibrateButton.onClick.AddListener(Calibrate);
        calibrateButton.gameObject.SetActive(false);
        SetupClient();
    }

    private void SetupClient() {
        PrintLog("Connecting...");
        if (client.isConnected())
            OnConnectedServer(null);
        else
            client.SetupClient(OnConnectedServer);
        client.RegisterHandler(CalibrationMessage.id, mobileSensor.OnCalibrationMessageReceived);
    }

    // Print to server text
    public void PrintLog(string s) {
        GameObject.Find("ServerText").GetComponent<Text>().text = s;
    }

    // Update is called once per frame
    void Update () {
        //PrintLog(mobileSensor.getCurrentRotation().ToString());
        if (mobileSensor.DetectSwipe()) {
            Fire();
        }
        if (calibrated && !waitForPositionData) {
            waitForPositionData = true;
            SendPositionData();
            StartCoroutine(WaitForPositionDataCoroutine());
        }
    }

    private void OnConnectedServer(NetworkMessage netMsg) {
        PrintLog("Connected to server");

        GameObject button = GameObject.Find("MobileButton");

        button.SetActive(false);
        calibrateButton.gameObject.SetActive(true);

        if(netMsg != null)
            client.SendRegisterHostMessage(netMsg);
    }

    public void SendActionData() {
        // Only on mobile
        if (SystemInfo.deviceType == DeviceType.Desktop) {
            Debug.Log("Only on mobile");
            return;
        }

        if (!calibrated)
            return;

        ActionMessage msg = new ActionMessage();
        msg.idDevice = client.getConnectionId();
        msg.position = mobileSensor.spawnPosition();
        msg.triggered = true;
        client.SendMessage(ActionMessage.id, msg);
    }

    public void SendPositionData() {
        // Only on mobile
        if (SystemInfo.deviceType == DeviceType.Desktop) {
            Debug.Log("Only on mobile");
            return;
        }

        if (!calibrated)
            return;

        ActionMessage msg = new ActionMessage();
        msg.idDevice = client.getConnectionId();
        msg.position = mobileSensor.spawnPosition();
        msg.triggered = false;
        client.SendMessage(ActionMessage.id, msg);
    }

    public void SendCalibrationData(bool value) {
        CalibrationMessage msg = new CalibrationMessage();
        msg.enable = value;
        msg.idDevice = client.getConnectionId();
        client.SendMessage(CalibrationMessage.id, msg);
    }

    public void Calibrate() {
        calibrated = false;
        SendCalibrationData(true);
        PrintLog("");
        calImgMng.SetCenter();
        calibrateButton.onClick.RemoveAllListeners();
        calibrateButton.onClick.AddListener(CalibratedCenter);

    }

    public void CalibratedCenter() {
        mobileSensor.ResetCalibratedRotation();
        calImgMng.SetTopLeft();
        calibrateButton.onClick.RemoveAllListeners();
        calibrateButton.onClick.AddListener(CalibratedTopLeft);
    }

    public void CalibratedTopLeft() {
        mobileSensor.SetMinAngle();
        calImgMng.SetBotRight();
        calibrateButton.onClick.RemoveAllListeners();
        calibrateButton.onClick.AddListener(CalibratedBotRight);
    }

    public void CalibratedBotRight() {
        mobileSensor.SetMaxAngle();
        calImgMng.Disable();
        PrintLog("Swipe!");
        SendCalibrationData(false);
        calibrateButton.onClick.RemoveAllListeners();
        calibrateButton.onClick.AddListener(Calibrate);
        calibrateButton.gameObject.SetActive(false);
        calibrated = true;
    }
    public void Fire() {
        if (!waitForFireAgain && calibrated) {
            PrintLog("FIRE");
            waitForFireAgain = true;
            SendActionData();
            StartCoroutine(WaitForFireAgainCoroutine());
        }
    }

    IEnumerator WaitForPositionDataCoroutine() {
        yield return new WaitForSeconds(timeBetweenTwoPositions);
        waitForPositionData = false;
    }

    IEnumerator WaitForFireAgainCoroutine() {
        yield return new WaitForSeconds(timeBetweenFire);
        waitForFireAgain = false;
        PrintLog("Swipe!");
    }
}
