using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum VoteType {
    MOUTH=0,
    EYES=1,
    HAT=2,
    ARMS=3
}
 
public class VoteManager : MonoBehaviour {

    private Server server;
    private Image[] choices;
    private Text[] scores;
    private Text voteText;
    private Text time;
    private float timeRemaining;
    public float timePerChoice = 5.0f;
    private bool voteTime = false;
    [Header("Order : Mouth - Eyes - Hat - Arms")]
    public Sprite[] spritesChoices;
    private VoteType currentVoteType = VoteType.MOUTH;
    private int numberOfClients;
    private bool waitingToBeReady = false;
    private HashSet<int> clientsReady;
    private Dictionary<int, int> clientResponse;
    private int[] responses;

    // Use this for initialization
    void Start () {
        choices = new Image[4];
        scores = new Text[4];
		for(int i = 0; i < 4; i++) {
            choices[i] = GameObject.Find("Choice" + (i + 1).ToString()).GetComponent<Image>();
            scores[i] = GameObject.Find("Score" + (i + 1).ToString()).GetComponent<Text>();
        }
        voteText = GameObject.Find("VoteTime").GetComponent<Text>();
        time = GameObject.Find("Time").GetComponent<Text>();
        server = GameObject.Find("Server").GetComponent<Server>();
        server.SetupServer();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (voteTime && timeRemaining > 0.0f) {
            timeRemaining -= Time.fixedDeltaTime;
            if (timeRemaining < 0.0f)
                timeRemaining = 0.0f;
            time.text = "Time: " + ((int)Mathf.Floor(timeRemaining)).ToString() + "s";
            if(timeRemaining == 0.0f) {
                EndVote();
            }
        }
	}

    public void SetupBeginVote() {
        GameObject.Find("BeginVote").SetActive(false);
        numberOfClients = server.getNumberRegisteredDevices();
        server.RegisterHandler(VoteMessage.id, onVoteMessageReceived);
        PrepareVote();
    }

    void PrepareVote() {
        voteText.text = "Waiting for clients...";
        timeRemaining = timePerChoice;
        for (int i = 0; i < 4; i++) {
            choices[i].sprite = spritesChoices[(int)currentVoteType * 4 + i];
        }
        SendAllPrepareVote();
    }

    void SendAllPrepareVote() {
        VoteMessage msg = new VoteMessage();
        msg.serverSpeaking = true;
        msg.start = false;
        msg.choice = -1;
        clientsReady = new HashSet<int>();
        waitingToBeReady = true;
        server.SendMessageToAllClients(VoteMessage.id, msg);
    }

    void onVoteMessageReceived(NetworkMessage netMsg) {
        VoteMessage msg = netMsg.ReadMessage<VoteMessage>();
        if (waitingToBeReady) {
            try {
                clientsReady.Add(netMsg.conn.connectionId);
            } catch (Exception e) {
                
            }
            if(clientsReady.Count == numberOfClients) {
                waitingToBeReady = false;
                StartVote();
            }
        } else if (voteTime) {
            if (clientResponse.ContainsKey(netMsg.conn.connectionId)) {
                responses[clientResponse[netMsg.conn.connectionId]-1]--;
            }
            clientResponse[netMsg.conn.connectionId] = msg.choice;
            responses[clientResponse[netMsg.conn.connectionId]-1]++;
            UpdateVoteScore();
        }
    }

    void UpdateVoteScore() {
        for(int i = 0; i < 4; i++) {
            if (clientResponse.Count == 0)
                scores[i].text = "0%";
            else
                scores[i].text = ((int)(Mathf.Floor((float)(responses[i]) / clientResponse.Count * 100.0f))).ToString() + "%";
        }
    }

    public int GetMaxScore() {
        int max = 0;
        int res = -1;
        for (int i = 0; i < 4; i++) {
            if (responses[i] > max) {
                max = responses[i];
                res = i;
            } else if (responses[i] == max) {
                if (UnityEngine.Random.value >= 0.5) {
                    max = responses[i];
                    res = i;
                }
            }
        }
        return res+1;
    }

    void StartVote() {
        voteText.text = "VOTE !";
        voteTime = true;
        responses = new int[4];
        for(int i = 0; i < 4; i++) {
            responses[i] = 0;
        }
        clientResponse = new Dictionary<int, int>();
        UpdateVoteScore();
        VoteMessage msg = new VoteMessage();
        msg.serverSpeaking = true;
        msg.start = true;
        server.SendMessageToAllClients(VoteMessage.id, msg);
    }

    void EndVote() {
        voteText.text = "Winner is " + GetMaxScore();
        voteTime = false;
        currentVoteType += 1;
        VoteMessage msg = new VoteMessage();
        msg.serverSpeaking = true;
        msg.start = false;
        msg.isThereNext = ((int)currentVoteType < 4);
        server.SendMessageToAllClients(VoteMessage.id, msg);
        if (msg.isThereNext) {
            StartCoroutine(WaitALittle());
        }
    }

    IEnumerator WaitALittle() {
        yield return new WaitForSeconds(10);
        PrepareVote();
    }
}
