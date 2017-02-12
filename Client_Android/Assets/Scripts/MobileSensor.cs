﻿using UnityEngine;
using UnityEngine.Networking;

public class MobileSensor : MonoBehaviour {

    private Vector3 calibratedRotation;
    [SerializeField] float minYSwipeDetect = 75.0f;

    private float minX, minY, maxX, maxY, minAngleX, minAngleY, maxAngleX, maxAngleY;


	// Use this for initialization
	void Start () {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update () {
        calibratedRotation += Input.gyro.rotationRate;
        FormatVector();
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

    public void ResetCalibratedRotation() {
        calibratedRotation = Vector3.zero;
    }

    public void SetMinAngle() {
        minAngleX = -calibratedRotation.z;
        minAngleY = -calibratedRotation.x;
    }

    public void SetMaxAngle() {
        maxAngleX = -calibratedRotation.z;
        maxAngleY = -calibratedRotation.x;
    }

    public Vector3 spawnPosition() {
        float angleX = -calibratedRotation.z;
        float angleY = -calibratedRotation.x;
        float x = 0.0f;
        float y = 0.0f;
        if(angleX < 0) {
            x = minX / minAngleX * angleX;
        } else {
            x = maxX / maxAngleX * angleX;
        }
        if (angleY < 0) {
            y = maxY / minAngleY * angleY;
        } else {
            y = minY / maxAngleY * angleY;
        }
        return new Vector3(x,y, 0);
    }

    public bool DetectSwipe() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            if(touchDeltaPosition.y > minYSwipeDetect) {
                return true;
            }
        }
        return false;
    }
}
