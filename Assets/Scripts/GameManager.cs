using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance;

    public void Awake(){
        Instance = this;
    }

}
