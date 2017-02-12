using UnityEngine;
using UnityEngine.Networking;

public class Calibration : MonoBehaviour {

    [SerializeField] private GameObject calibrateTargets;
    public GameSceneManager gameSceneManager;

    // Use this for initialization
    void Start () {
        Disable();
    }

    public void OnCalibrationMessageReceived(NetworkMessage netMsg) {
        CalibrationMessage msg = netMsg.ReadMessage<CalibrationMessage>();
        if (msg.enable) {
            //Enable();
			Vector3 topLeft, bottomRight;
			topLeft = Camera.main.ScreenToWorldPoint (new Vector3 (0f, Camera.main.pixelHeight, 10f));
			bottomRight = Camera.main.ScreenToWorldPoint (new Vector3 (Camera.main.pixelWidth, 0f, 10f));
            gameSceneManager.SendCalibrationMessage(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y, netMsg.conn.connectionId);
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
