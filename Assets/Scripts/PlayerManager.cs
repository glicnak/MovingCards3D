using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject cardTemplate;

    public GameObject hand;
    public GameObject locations;

    public static PlayerManager Instance;

    void Awake(){
        Instance = this;
    }

    void Start(){
        hand = GameObject.Find("Hand");
        locations = GameObject.Find("Locations");
    }

    void Update(){
        if(Input.GetKeyDown("space")){
            GameObject card = Instantiate(cardTemplate, new Vector3(0,0,0), Quaternion.identity, hand.transform);
            HandManager.Instance.reManageHand();
        }
    }
}
