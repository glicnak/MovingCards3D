using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Mirror;
using TMPro;

public class PlayerManager : NetworkBehaviour
{
//Cards
    public GameObject villainFullTemplate;
    public GameObject villainTemplate;
    public GameObject villainVizTemplate;
    public GameObject markFullTemplate;
    public GameObject markTemplate;
    public GameObject markVizTemplate;

//Areas
    public GameObject hand;
    public GameObject markZone;
    public GameObject confirmButton;
    public GameObject undoButton;
    public GameObject payManaScreen;
    public GameObject cardInfoScreen;
    public GameObject playerInfoBox;

//Info
    public int streetChaos;
    public int mana;
    public List<GameObject> cardActions;
    public GameObject[] currentlyPlayedCard = new GameObject[2];
    public bool turnResolving;

    public static PlayerManager Instance;

//Start
    void Awake(){
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    void Start(){
        hand = GameObject.Find("Hand");
        markZone = GameObject.Find("MarkZone");
        //confirmButton = GameObject.Find("Confirm Button");
        //undoButton = GameObject.Find("Undo Button");
        //cardInfoScreen = GameObject.Find("Card Info Screen").transform.GetChild(0).gameObject;
        //playerInfoBox = GameObject.Find("Player Info").transform.GetChild(0).gameObject;
        streetChaos = 0;
        mana = 0;
    }


    public override void OnStartClient(){
        base.OnStartClient();
        if(isLocalPlayer){
            CmdCreatePlayerInfo(NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>(), DeckManager.Instance.myDeck);
        }
    }

    [Command]
    void CmdCreatePlayerInfo(NetworkIdentity id, List<Card> myDeck){
        GameManager.Instance.addPlayer(id);
        GameManager.Instance.createPlayerDeck(id, myDeck);
        if(GameManager.Instance.startGame()){
            foreach (NetworkIdentity netId in GameManager.Instance.players){
                TargetResetButtons(netId.connectionToClient);
            }
            //gmBothDrawCard(3);
        }
    }

    [TargetRpc]
    void TargetResetButtons(NetworkConnection connec){
        //NetworkClient.localPlayer.gameObject.GetComponent<PlayerManager>().roundStartButtons();
    }

//Draw A Card
    public void drawCard(int number){
        if(isLocalPlayer){
            for (int i=0; i<number; i++){
                CmdDrawCard(NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>());
            }
        }
    }

    [Command(requiresAuthority = false)]
    void CmdDrawCard(NetworkIdentity id){
        if (GameManager.Instance.getHand(id).Count < GameManager.Instance.getMaxHandSize(id)){
            Card topCard = GameManager.Instance.getTopCardOfDeck(id);
            if(topCard == null){
                //If there are no cards in deck left
                return;
            }
            GameManager.Instance.addCardInHand(id, topCard);
            GameManager.Instance.removeTopCardInDeck(id);
            TargetDrawCard(id.connectionToClient, topCard);
        }
    }        

    [TargetRpc]
    void TargetDrawCard(NetworkConnection netConnec, Card cardToDraw){
        GameObject card = Instantiate(villainFullTemplate, new Vector3(0,0,0), Quaternion.identity, hand.transform);
        card.GetComponent<CardValues>().setCardInfo(cardToDraw);
        HandManager.Instance.reManageHand();
        card.GetComponent<CardValues>().handIndex = card.transform.GetSiblingIndex();
    }

    [Server]
    public void gmBothDrawCard(int number){
        for (int i=0; i<number; i++){
            foreach (NetworkIdentity id in GameManager.Instance.players){
                if (GameManager.Instance.getHand(id).Count < GameManager.Instance.getMaxHandSize(id)){
                    Card topCard = GameManager.Instance.getTopCardOfDeck(id);
                    if(topCard == null){
                        //If there are no cards left in deck
                        return;
                    }
                    GameManager.Instance.addCardInHand(id, topCard);
                    GameManager.Instance.removeTopCardInDeck(id);
                    TargetDrawCard(id.connectionToClient, topCard);
                }
            }
        }
    }

//Destroy Cards
    public void destroyCardInHand(GameObject card){
        CmdDestroyCardInHand(NetworkClient.localPlayer.gameObject.GetComponent<NetworkIdentity>(), card.GetComponent<CardValues>().handIndex);
        Destroy(card);
    }

    [Command] //maybe requires authority = false?
    public void CmdDestroyCardInHand(NetworkIdentity id, int cardIndex){
        GameManager.Instance.destroyCardInHand(id, cardIndex);
    }

//Update
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
