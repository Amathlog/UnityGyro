using UnityEngine;
using UnityEngine.Networking;

public class Calibration : MonoBehaviour {

    [SerializeField] private GameObject calibrateTargets;
    [SerializeField] private Transform topLeft;
    [SerializeField] private Transform botRight;

    // Use this for initialization
    void Start () {
        Disable();
    }

    public void OnCalibrationMessageReceived(NetworkMessage netMsg) {
        CalibrationMessage msg = netMsg.ReadMessage<CalibrationMessage>();
        if (msg.enable) {
            Enable();
            GameObject.Find("Server").GetComponent<Server>().SendCalibrationMessage(topLeft.position.x, topLeft.position.y, botRight.position.x, botRight.position.y, netMsg.conn.connectionId);
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
