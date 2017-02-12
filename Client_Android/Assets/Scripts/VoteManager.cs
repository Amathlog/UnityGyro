using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VoteManager : MonoBehaviour {

    public Client client;
    private Button[] choices;
    private Text voteText;
    private bool voteTime = false;
    private int currentChoice = -1;

	// Use this for initialization
	void Start () {
        client = GameObject.Find("Client").GetComponent<Client>();
        choices = new Button[4];
        for(int i = 1; i <= 4; i++) {
            choices[i-1] = GameObject.Find("Choice" + i).GetComponent<Button>();
        }
        voteText = GameObject.Find("VoteText").GetComponent<Text>();

        client.SetupClient(client.SendRegisterHostMessage);

        client.RegisterHandler(VoteMessage.id, onReceivedVoteMessage);
        FinishVoteTime(true);
    }

    public void onReceivedVoteMessage(NetworkMessage netMsg) {
        VoteMessage msg = netMsg.ReadMessage<VoteMessage>();
        if(msg.serverSpeaking && msg.start != voteTime) {
            voteTime = msg.start;
            if (voteTime) {
                SetupVoteTime();
            } else {
                FinishVoteTime(msg.isThereNext);
            }
        } else if(msg.serverSpeaking && msg.choice == -1) {
            VoteMessage msgSend = new VoteMessage();
            msgSend.serverSpeaking = false;
            msgSend.start = true;
            client.SendMessage(VoteMessage.id, msgSend);
        }
    }

    public void SendVoteMessage(int choice) {
        if (currentChoice == choice)
            return;
        currentChoice = choice;
        ChangeVoteText("Current choice :" + choice);
        VoteMessage msg = new VoteMessage();
        msg.serverSpeaking = false;
        msg.choice = choice;
        client.SendMessage(VoteMessage.id, msg);
    }

    void SetupVoteTime() {
        ChangeVoteText("TIME TO VOTE");
        for (int i = 0; i < 4; i++) {
            int value = i + 1;
            choices[i].onClick.AddListener(delegate { SendVoteMessage(value); });
        }
    }

    void FinishVoteTime(bool isThereNext) {
        if(isThereNext)
            ChangeVoteText("Vote Over... Wait for next");
        else
            ChangeVoteText("Vote Over... ");
        for (int i = 0; i < 4; i++) {
            choices[i].onClick.RemoveAllListeners();
        }
    }

    void ChangeVoteText(string s) {
        voteText.text = s;
    }
}
