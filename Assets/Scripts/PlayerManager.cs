using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject villainTemplate;
    public GameObject markTemplate;

    public GameObject hand;
    public GameObject markZone;

    public static PlayerManager Instance;

    void Awake(){
        Instance = this;
    }

    void Start(){
        hand = GameObject.Find("Hand");
        markZone = GameObject.Find("MarkZone");
    }

    void Update(){
        if(Input.GetKeyDown("return")){
            GameObject card = Instantiate(villainTemplate, new Vector3(0,0,0), Quaternion.identity, hand.transform);
            HandManager.Instance.reManageHand();
        }
        if(Input.GetKeyDown("space")){
            GameObject card = Instantiate(markTemplate, new Vector3(0,0,0), Quaternion.identity, markZone.transform);
        }
    }
}
