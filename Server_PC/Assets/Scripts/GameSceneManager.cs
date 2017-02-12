using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour {

    public Server server;
    public Calibration calibration;
    public GameObject target;
    [SerializeField] private bool targetDebug = true;
	public static GameSceneManager instance;
	public int score;
	public Text scoreText;
	public Enemies myEnemiesComp;

	void Awake(){
		if (instance == null) {
			instance = this;
		}
	}

    // Use this for initialization
    void Start () {
        server.SetupServer();
        server.RegisterHandler(ActionMessage.id, OnReceivedActionMessage);
        server.RegisterHandler(CalibrationMessage.id, calibration.OnCalibrationMessageReceived);
		myEnemiesComp = GameObject.Find ("EnemiesManager").GetComponent<Enemies> ();
    }
	
    private void OnReceivedActionMessage(NetworkMessage netMsg) {
        ActionMessage msg = netMsg.ReadMessage<ActionMessage>();
        DeviceInfo device = server.getRegisteredDevice(netMsg.conn.connectionId);
		Vector3 offset = (calibration.bottomRight - calibration.topLeft);

        if (msg.triggered) {
			
			GameObject.Find("SpawnTarget").GetComponent<SpawnTarget>().Spawn(msg.position, device.target.GetComponent<SpriteRenderer>().color);
            Debug.Log("Message received from device : " + device.name);
            Debug.Log(msg.ToString());
        } else {
            if (device.target == null) {
                device.target = Instantiate(target);
                device.target.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
            }
            if (targetDebug)
                device.target.SetActive(true);
            else
                device.target.SetActive(false);
			device.target.transform.position = msg.position;
        }
    }

    public void SendCalibrationMessage(float minX, float minY, float maxX, float maxY, int id) {
        CalibrationMessage msg = new CalibrationMessage();
        msg.minX = minX;
        msg.minY = minY;
        msg.maxX = maxX;
        msg.maxY = maxY;

        Debug.Log("MinX = " + minX + "MinY = " + minY + "MaxX = " + maxX + "MaxY = " + maxY);

        NetworkServer.SendToClient(id, CalibrationMessage.id, msg);
    }

	public void AddPoints (int amount){
		score += amount;
		if (scoreText != null) {
			scoreText.text = score+ "Pts";
		}
	}
}
