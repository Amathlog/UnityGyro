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
    private GameObject rawImage;
    private Image[] choices;
    private Image[] monster;
    private Text[] scores;
    private Text voteText;
    private Text time;
    private Texture2D tex;
    private float timeRemaining;
    public float timePerChoice = 5.0f;
    private bool voteTime = false;
    [Header("Order : Mouth - Eyes - Hat - Arms")]
    public Sprite[] spritesChoices;
    [Header("Bodies (Fall/Summer/Winter/Spring)")]
    public Sprite[] bodies;
    private VoteType currentVoteType = VoteType.MOUTH;
    private int numberOfClients;
    private bool waitingToBeReady = false;
    private HashSet<int> clientsReady;
    private Dictionary<int, int> clientResponse;
    private int[] responses;

    // Use this for initialization
    void Start () {
        rawImage = GameObject.Find("RawImage");
        rawImage.SetActive(false);
        choices = new Image[4];
        scores = new Text[4];
		for(int i = 0; i < 4; i++) {
            choices[i] = GameObject.Find("Choice" + (i + 1).ToString()).GetComponent<Image>();
            scores[i] = GameObject.Find("Score" + (i + 1).ToString()).GetComponent<Text>();
        }
        monster = new Image[5];
        monster[(int)VoteType.MOUTH] = GameObject.Find("Mouth").GetComponent<Image>();
        monster[(int)VoteType.EYES] = GameObject.Find("Eyes").GetComponent<Image>();
        monster[(int)VoteType.HAT] = GameObject.Find("Hat").GetComponent<Image>();
        monster[(int)VoteType.ARMS] = GameObject.Find("Arms").GetComponent<Image>();
        monster[4] = GameObject.Find("Body").GetComponent<Image>();
        monster[4].sprite = bodies[2];
        monster[4].color = Color.white;

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
            time.text = ((int)Mathf.Ceil(timeRemaining)).ToString();
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
            if (currentVoteType == VoteType.MOUTH) {
                choices[i].GetComponent<RectTransform>().sizeDelta = new Vector3(700, 700);
                Vector3 newPos = choices[i].GetComponent<RectTransform>().position;
                newPos.y += 100;
                choices[i].GetComponent<RectTransform>().position = newPos;
            } else if (currentVoteType == VoteType.EYES) {
                choices[i].GetComponent<RectTransform>().sizeDelta = new Vector3(600, 600);
                Vector3 newPos = choices[i].GetComponent<RectTransform>().position;
                newPos.y -= 50;
                choices[i].GetComponent<RectTransform>().position = newPos;
            } else if(currentVoteType == VoteType.HAT) {
                choices[i].GetComponent<RectTransform>().sizeDelta = new Vector3(500, 500);
                Vector3 newPos = choices[i].GetComponent<RectTransform>().position;
                newPos.y -= 200;
                choices[i].GetComponent<RectTransform>().position = newPos;
            } else {
                choices[i].GetComponent<RectTransform>().sizeDelta = new Vector3(225, 225);
                Vector3 newPos = choices[i].GetComponent<RectTransform>().position;
                newPos.y += 200;
                choices[i].GetComponent<RectTransform>().position = newPos;
            }
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
        int res = 0;
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
        int winner = GetMaxScore();
        voteText.text = "Winner is " + winner;
        voteTime = false;
        monster[(int)currentVoteType].sprite = choices[winner - 1].sprite;
        monster[(int)currentVoteType].color = Color.white;
        currentVoteType += 1;
        VoteMessage msg = new VoteMessage();
        msg.serverSpeaking = true;
        msg.start = false;
        msg.isThereNext = ((int)currentVoteType < 4);
        server.SendMessageToAllClients(VoteMessage.id, msg);
        if (msg.isThereNext) {
            StartCoroutine(WaitALittle());
        } else {
            StartCoroutine(WaitALittleEnd());
            
        }
    }

    IEnumerator WaitALittle() {
        yield return new WaitForSeconds(1);
        PrepareVote();
    }

    IEnumerator WaitALittleEnd() {
        yield return new WaitForSeconds(2);
        CreateTexture();
    }

    void CreateTexture() {
        tex = new Texture2D((int)monster[0].sprite.rect.width, (int)monster[0].sprite.rect.height);
        Color[] currPixels = null;
        int[] order = { 3, 4, 2, 1, 0 };
        foreach (int i in order){
            Color[] pixels = monster[i].sprite.texture.GetPixels();
            if (currPixels == null)
                currPixels = pixels;
            else
                for (int j = 0; j < currPixels.Length; j++) {
                    if (pixels[j].a != 0.0f)
                        currPixels[j] = pixels[j];
                }
        }
        tex.SetPixels(currPixels);
        tex.Apply();
        TextureCharacter.getInstance().tex = tex;
    }
}
