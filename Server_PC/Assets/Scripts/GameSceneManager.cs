﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour {

    private Server server;
    private SocketClient socketClient;
    public Calibration calibration;
    public GameObject target;
    [SerializeField] private bool targetDebug = true;
	public static GameSceneManager instance;
	public float score, timer;
	private Text scoreText, timerText;
	public Enemies myEnemiesComp;
	public Material enemyMat;
    public Texture2D defaultEnemyTex;
    public float speedMultiplier=1;
	private CanvasGroup calibrateScreen;
    private MouseTesting mouseTesting;
    private GameObject canvas;
    private Vector2 canvasTransform;

    private int nextWaveType = 0;

    public bool gameStarted = false;
    private ParticleSystem explo;
    public bool mouseDebug = false;

    private Dictionary<string, GameObject> targets;
    private Dictionary<string, Vector3> positions;
    private Dictionary<string, bool> firing;

    void Awake(){
		if (instance == null) {
			instance = this;
		}
	}

    private void OnApplicationQuit() {
        SocketClient.CleanUp();
    }

    void UpdatePositions() {
        foreach(KeyValuePair<string, Vector3> entry in positions) {
            if (!targets.ContainsKey(entry.Key)) {
                targets[entry.Key] = Instantiate(target);
                targets[entry.Key].transform.SetParent(canvas.transform);
                targets[entry.Key].GetComponent<Image>().color = new Color(Random.value, Random.value, Random.value);
            }
            targets[entry.Key].transform.position = entry.Value;
            //Debug.Log(entry.Value);
            if (firing.ContainsKey(entry.Key)) {
                if (firing[entry.Key]) {
                    Vector3 spawnPos = Camera.main.ScreenToWorldPoint(targets[entry.Key].transform.position + Vector3.forward * 10);
                    GameObject.Find("SpawnTarget").GetComponent<SpawnTarget>().Spawn(spawnPos, targets[entry.Key].GetComponent<Image>().color);
                    firing[entry.Key] = false;
                }
            }
        }
    }

    private void FixedUpdate() {
        if (mouseDebug) {
            positions["mouse"] = mouseTesting.position;
            if (mouseTesting.fire) {
                firing["mouse"] = true;
                mouseTesting.fire = false;
            }
        }
        UpdatePositions();
    }

    // Use this for initialization
    void Start () {
        explo = GameObject.Find("Explosions").GetComponent<ParticleSystem>();
        //server = GameObject.Find("Server").GetComponent<Server>();
		scoreText = GameObject.Find ("ScoreText").GetComponent<Text> ();
		timerText = GameObject.Find ("TimerText").GetComponent<Text> ();
		calibrateScreen = GameObject.Find ("CalibrateScreen").GetComponent<CanvasGroup> ();
        canvas = GameObject.Find("Canvas");
        canvasTransform = canvas.GetComponent<RectTransform>().position;
		score = 0;
		timer = 10;
		timerText.text = timer.ToString();
        positions = new Dictionary<string, Vector3>();
        targets = new Dictionary<string, GameObject>();
        firing = new Dictionary<string, bool>();
        mouseTesting = GameObject.Find("MouseTesting").GetComponent<MouseTesting>();

        if (TextureCharacter.getInstance().tex != null){
			enemyMat.mainTexture = TextureCharacter.getInstance ().tex;
		} else {
            enemyMat.mainTexture = defaultEnemyTex;
        }
        //server.SetupServer();
        //server.RegisterHandler(ActionMessage.id, OnReceivedActionMessage);
        //server.RegisterHandler(CalibrationMessage.id, calibration.OnCalibrationMessageReceived);
        socketClient = SocketClient.GetInstance();
        socketClient.ChangeMode(1);
        socketClient.updateEventCallbacks += OnReceivedUpdateMessage;
        socketClient.fireEventCallbacks += OnReceivedFireMessage;
        myEnemiesComp = GameObject.Find ("EnemiesManager").GetComponent<Enemies> ();
		StartCoroutine (GameCoroutine ());
    }

    public void Explode(Vector3 pos) {
        explo.transform.position = pos;
        explo.Play();
    }

	IEnumerator GameCoroutine(){
		float iterator = 0;
		//while (iterator <= 1) {
		//	iterator += 0.02f;
		//	calibrateScreen.alpha = iterator;
		//	yield return new WaitForSeconds (0.02f);
		//}

		//while (timer > 0) {
		//	timer--;
		//	timerText.text = timer.ToString();
		//	yield return new WaitForSeconds (1.0f);
		//}

		//while (iterator > 0) {
		//	iterator -= 0.02f;
		//	calibrateScreen.alpha = iterator;
		//}
		timer = 10;
		timerText.text = timer.ToString ();

		while (timer > 0) {
			yield return new WaitForSeconds (1f);
			timer--;
			timerText.text = timer.ToString();
		}

		timer = 60;
		timerText.text = timer.ToString();
        gameStarted = true;

        myEnemiesComp.SpawnEnemies (8, nextWaveType);
		nextWaveType++;
		while (timer > 0) {
			yield return new WaitForSeconds (1f);
			timer--;
			timerText.text = timer.ToString();
			if (myEnemiesComp.aliveEnemies.Count == 0) {
				myEnemiesComp.aliveEnemiesReadyForPattern = false;
				myEnemiesComp.SpawnEnemies (8, nextWaveType);
				if (nextWaveType == 1) {
					nextWaveType = 0;
					speedMultiplier*=0.8f;
				} else {
					nextWaveType++;
				}
			}
		}

        yield return new WaitForSeconds(3.0f);
        socketClient.updateEventCallbacks -= OnReceivedUpdateMessage;
        socketClient.fireEventCallbacks -= OnReceivedFireMessage;
        SceneManager.LoadScene(2);
        //server.requestSceneChange(2);

    }

    private void OnReceivedUpdateMessage(string id, float x, float y) {
        float new_x = (x - 1.0f) * -0.5f;
        float new_y = (y + 1.0f) * 0.5f;
        positions[id] = new Vector3(new_x * Screen.width, new_y * Screen.height);
    }

    private void OnReceivedFireMessage(string id) {
        firing[id] = true;
        //Debug.Log("Fire from : " + id);
    }

    private void OnReceivedActionMessage(NetworkMessage netMsg) {
        ActionMessage msg = netMsg.ReadMessage<ActionMessage>();
        Debug.Log("Device : " + netMsg.conn.connectionId);
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

	public void AddPoints (float amount){
		score += amount;
		if (scoreText != null) {
			scoreText.text = "Score : "+Mathf.CeilToInt(score)+ "Pts";
		}
	}
}
