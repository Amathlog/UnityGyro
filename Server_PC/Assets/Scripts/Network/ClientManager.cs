using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager {
    private static ClientManager instance;
    private ArrayList clientIds;

    private ClientManager() {
        clientIds = new ArrayList();
        SocketClient.GetInstance().connectionEventCallbacks += Connection;
    }

    public void Connection(string id, bool connection) {
        if (connection)
            clientIds.Add(id);
        else
            clientIds.Remove(id);
    }

    public int getNumberOfClients() {
        return clientIds.Count;
    }
}
