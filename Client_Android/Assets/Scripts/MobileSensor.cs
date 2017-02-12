using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MobileSensor : MonoBehaviour {

    private Vector3 calibratedRotation;
    public Button calibrateButton;
    private Text text;
    public bool calibrated = false;
    private bool waitForPositionData = false;
    private bool waitForFireAgain = false;
    private bool waitForTouchToBeOver = false;
    private Client client;
    [SerializeField] float timeBetweenTwoPositions = 0.1f;
    [SerializeField] float timeBetweenFire = 0.7f;
    [SerializeField] float minYSwipeDetect = 75.0f;

    private float minX, minY, maxX, maxY, minAngleX, minAngleY, maxAngleX, maxAngleY;


	// Use this for initialization
	void Start () {
        text = GameObject.Find("ServerText").GetComponent<Text>();
        calibrateButton = GameObject.Find("Calibrate").GetComponent<Button>();
        calibrateButton.onClick.AddListener(Calibrate);
        calibrateButton.gameObject.SetActive(false);
        client = GameObject.Find("Client").GetComponent<Client>();
    }

    public void Calibrate() {
        calibrated = false;
        client.SendCalibrationData(true);
        text.text = "Point the center";
        calibrateButton.onClick.RemoveAllListeners();
        calibrateButton.onClick.AddListener(CalibratedCenter);
        
    }

    public void CalibratedCenter() {
        calibratedRotation = new Vector3();
        text.text = "Point the top left";
        calibrateButton.onClick.RemoveAllListeners();
        calibrateButton.onClick.AddListener(CalibratedTopLeft);
    }

    public void CalibratedTopLeft() {
        minAngleX = -calibratedRotation.z;
        minAngleY = calibratedRotation.x;
        text.text = "Point the bot right";
        calibrateButton.onClick.RemoveAllListeners();
        calibrateButton.onClick.AddListener(CalibratedBotRight);
    }

    public void CalibratedBotRight() {
        maxAngleX = -calibratedRotation.z;
        maxAngleY = calibratedRotation.x;
        client.PrintLog("Swipe!");
        client.SendCalibrationData(false);
        calibrateButton.onClick.RemoveAllListeners();
        calibrateButton.onClick.AddListener(Calibrate);
        calibrateButton.gameObject.SetActive(false);
        calibrated = true;
    }

    // Update is called once per frame
    void Update () {
        calibratedRotation += Input.gyro.rotationRate;
        FormatVector();
        DetectSwipe();
        if (calibrated && !waitForPositionData) {
            waitForPositionData = true;
            client.SendPositionData();
            StartCoroutine(WaitForPositionDataCoroutine());
        }
	}

    IEnumerator WaitForPositionDataCoroutine() {
        yield return new WaitForSeconds(timeBetweenTwoPositions);
        waitForPositionData = false;
    }

    IEnumerator WaitForFireAgainCoroutine() {
        yield return new WaitForSeconds(timeBetweenFire);
        waitForFireAgain = false;
        client.PrintLog("Swipe!");
    }

    void FormatVector() {
        if (calibratedRotation.x >= 180.0f)
            calibratedRotation.x -= 360.0f;
        else if (calibratedRotation.x <= -180.0f)
            calibratedRotation.x += 360.0f;

        if (calibratedRotation.y >= 180.0f)
            calibratedRotation.y -= 360.0f;
        else if (calibratedRotation.y <= -180.0f)
            calibratedRotation.y += 360.0f;

        if (calibratedRotation.z >= 180.0f)
            calibratedRotation.z -= 360.0f;
        else if (calibratedRotation.z <= -180.0f)
            calibratedRotation.z += 360.0f;
    }

    public void OnCalibrationMessageReceived(NetworkMessage netMsg) {
        CalibrationMessage msg = netMsg.ReadMessage<CalibrationMessage>();
        minX = msg.minX;
        minY = msg.minY;
        maxX = msg.maxX;
        maxY = msg.maxY;
    }

    public Vector3 getCurrentRotation() {
        return calibratedRotation;
    }

    public Vector3 spawnPosition() {
        float angleX = -calibratedRotation.z;
        float angleY = calibratedRotation.x;
        return new Vector3((angleX - minAngleX) / (maxAngleX - minAngleX) * (maxX - minX) + minX, (angleY - minAngleY) / (maxAngleY - minAngleY) * (maxY - minY) + minY, 0);
    }

    public void Fire() {
        if (!waitForFireAgain && calibrated) {
            client.PrintLog("FIRE");
            waitForFireAgain = true;
            client.SendActionData();
            StartCoroutine(WaitForFireAgainCoroutine());
        }
    }

    public void DetectSwipe() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && !waitForTouchToBeOver) {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            if(touchDeltaPosition.y > minYSwipeDetect) {
                Fire();
            }
        }
    }
}
