using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quobject.SocketIoClientDotNet.Client;
using System;
using Newtonsoft.Json;
using UnityEngine.Events;

// Class Message
// Contains the password to secure communications with the server
// It also have the name of the message and jsonData that also need to
// be serialize/deserialized
class Message {

    public Message(string name, string data)
    {
        this.name = name;
        this.jsonData = data;
    }

	public string password = "f7X%3&_<7S9h@U}Sd5weR>x<nkWE#w{";
    public string name;
    public string jsonData;
}

class ConnectionToServer{
	public static readonly string name = "connectiontoserver";
	public string id = "server";
}

class Vote {
    public static readonly string name = "vote";
    public string id = "";
    public int vote = 0;
}

class Connection{
	public static readonly string name = "handleperson";
	public string id = "";
	public bool connection = true;
}

class UpdatePosition {
	public static readonly string name = "updatePosition";
	public string id = "";
	public float x = 0.0f, y = 0.0f;
}
	
class RequestSceneChange {
	public static readonly string name = "requestscenechange";
	public int mode;
}

class VoteStatus {
    public static readonly string name = "voteStatus";
    public bool voting;
}

class Fire {
    public static readonly string name = "fire";
    public string id = "";
}

// Singleton containing all the network backend.
// For each message, an event will be triggered.
// Other classes need to register to those events
public class SocketClient : IDisposable {

    private Socket socket;
    public bool connected = false;

    private static SocketClient instance = null;

    // Callbacks for each event
    public delegate void connectionEvent(string id, bool connected);
    public event connectionEvent connectionEventCallbacks;

    public delegate void voteEvent(string id, int vote);
    public event voteEvent voteEventCallbacks;

    public delegate void updateEvent(string id, float x, float y);
    public event updateEvent updateEventCallbacks;

    public delegate void fireEvent(string id);
    public event fireEvent fireEventCallbacks;

    // Private constructor, cannot instanciate it.
    // Need to use GetInstance()
    private SocketClient() { }
    
    // Send a message through the socket
    private void SendMessage(Message msg) {
        socket.Emit("serverMessage", JsonConvert.SerializeObject(msg));
    }

    ~SocketClient() {
        Dispose();
    }

    public static void CleanUp() {
        if (instance != null) {
            instance.Dispose();
        }
    }

    public void Dispose() {
        Dispose(true);
        instance = null;
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            if(socket != null) {
                socket.Disconnect();
                socket.Close();
            }
        }
    }

    public static SocketClient GetInstance() {
        if (instance == null) {
            instance = new SocketClient();
            instance.SetupSocket();
            while (!instance.connected) { }
        }
        return instance;
    }

    public void ChangeMode(int mode) {
        RequestSceneChange data = new RequestSceneChange {
            mode = mode
        };
        SendMessage(new Message(RequestSceneChange.name, JsonConvert.SerializeObject(data)));
    }

    public void ChangeVoteStatus(bool voting) {
        VoteStatus data = new VoteStatus {
            voting = voting
        };
        SendMessage(new Message(VoteStatus.name, JsonConvert.SerializeObject(data)));
    }

    // Need to be called only once, to connect to the server
    // and configure 
    public void SetupSocket() {
        socket = IO.Socket("http://localhost:3000");
        socket.On(Socket.EVENT_CONNECT, () => {
            connected = true;
            Message msg = new Message(ConnectionToServer.name, JsonConvert.SerializeObject(new ConnectionToServer()));
            socket.Emit("userId", JsonConvert.SerializeObject(msg));
        });

        socket.On(Connection.name, (data) => {
            string jsonMsg = data.ToString();
            Message msg = JsonConvert.DeserializeObject<Message>(jsonMsg);
            Connection handle = JsonConvert.DeserializeObject<Connection>(msg.jsonData);

            connectionEventCallbacks(handle.id, handle.connection);
        });

        socket.On(UpdatePosition.name, UpdatePositionHandle);

        socket.On(Vote.name, VoteHandle);

        socket.On(Fire.name, (data) => {
            string jsonMsg = data.ToString();
            Message msg = JsonConvert.DeserializeObject<Message>(jsonMsg);
            Fire handle = JsonConvert.DeserializeObject<Fire>(msg.jsonData);

            fireEventCallbacks(handle.id);
        });
    }

    void VoteHandle(object data) {
        string jsonMsg = data.ToString();
        Message msg = JsonConvert.DeserializeObject<Message>(jsonMsg);
        Vote handle = JsonConvert.DeserializeObject<Vote>(msg.jsonData);

        voteEventCallbacks(handle.id, handle.vote);
    }

    void UpdatePositionHandle(object data) {
        string jsonMsg = data.ToString();
        Message msg = JsonConvert.DeserializeObject<Message>(jsonMsg);
        UpdatePosition handle = JsonConvert.DeserializeObject<UpdatePosition>(msg.jsonData);
        //Debug.Log("x: " + handle.x.ToString() + "; y:" + handle.y.ToString());

        updateEventCallbacks(handle.id, handle.x, handle.y);
    }
}
