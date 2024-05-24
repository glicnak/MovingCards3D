using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager: NetworkManager {

    public static MyNetworkManager Instance;

    public override void Awake(){
        base.Awake();
        Instance = this;
    }

    public override void OnServerConnect(NetworkConnectionToClient conn){
        base.OnServerConnect(conn);
    }

}
