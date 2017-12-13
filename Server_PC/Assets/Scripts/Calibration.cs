using UnityEngine;
using UnityEngine.Networking;

public class Calibration : MonoBehaviour {

    [SerializeField] private GameObject calibrateTargets;
    public GameSceneManager gameSceneManager;
	public Vector3 topLeft, bottomRight;
    // Use this for initialization
    void Start () {
        Enable();
    }

    public void OnCalibrationMessageReceived(NetworkMessage netMsg) {
        CalibrationMessage msg = netMsg.ReadMessage<CalibrationMessage>();
        if (msg.enable) {
            //Enable();
			Vector3 topLeft, bottomRight;
			topLeft = Camera.main.ScreenToWorldPoint (new Vector3 (0f, Camera.main.pixelHeight, 10f));
			bottomRight = Camera.main.ScreenToWorldPoint (new Vector3 (Camera.main.pixelWidth, 0f, 10f));
            Debug.Log(topLeft+" "+bottomRight);
            gameSceneManager.SendCalibrationMessage(topLeft.x, bottomRight.y, bottomRight.x, topLeft.y, netMsg.conn.connectionId);
        } else {
            Disable();
        }
    }
	
	public void Enable() {
        calibrateTargets.SetActive(true);
    }

    public void Disable() {
        calibrateTargets.SetActive(false);
    }
}
