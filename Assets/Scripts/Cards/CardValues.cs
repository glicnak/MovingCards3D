using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class CardValues : NetworkBehaviour
{
    //For Card Drag
    public bool isSelectable = true;
    public bool isBeingDragged = false;
    public int handIndex = 0;

    //Round Start
    public Transform roundStartParent;

    //Card Properties
    public Card card;
    public GameObject displayText;
    public string villain;
    public string type;
    public string artworkName;
    public Abilities abilities;

    [SyncVar]
    public int attack;
    [SyncVar]
    public int health;
    [SyncVar]
    public int cost;
    [SyncVar]
    public List<string> statuses;
    [SyncVar]
    public bool isSpawned;
    [SyncVar]
    public bool isDead = false;
    [SyncVar]
    public bool oncePerRound = false;

    void Start(){
        roundStartParent = transform.parent;
        GetComponent<CardDrag>().draggedCardParent = GameObject.Find("Selected Card Area");
    }

    public void setCardInfo(Card cardInfo){
        if(cardInfo == null){
            cardInfo = card;
        }
        card = cardInfo;
        gameObject.name = card.name;
        villain = card.villain;
        attack = card.attack;
        health = card.health;
        cost = card.cost;
        type = card.type;
        artworkName = card.artworkName;
        foreach (Card a in DeckManager.Instance.allCards){
            if (card.name == a.name){
                abilities = a.abilities;
            }
        }
        updateCardInfo();
        updateCardImages();
    }

    public void updateCardInfo(){
        //check for death and Update Health
        if(checkForDeath()){
            if(transform.parent.name == "Hand"){
                NetworkClient.localPlayer.gameObject.GetComponent<PlayerManager>().destroyCardInHand(gameObject);
            }
            else {
                //not sure if anything to do here
            }
        }

        /*
        displayText.transform.GetChild(0).GetComponent<TextMeshPro>().text = card.name;
        displayText.transform.GetChild(1).GetComponent<TextMeshPro>().text = attack.ToString();
        displayText.transform.GetChild(2).GetComponent<TextMeshPro>().text = health.ToString();
        if(transform.parent == null || transform.parent.name != "Hand"){
            displayText.transform.GetChild(3).GetComponent<TextMeshPro>().text = "";
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
        else {
            displayText.transform.GetChild(3).GetComponent<TextMeshPro>().text = cost.ToString();
            transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }
        displayText.transform.GetChild(4).GetComponent<TextMeshPro>().text = card.description;
        */
    }

    public void updateCardImages(){
        /*
        //Get Front Materials
        Texture2D charArt = Resources.Load("Art/Character Art/"+gameObject.name+" Full", typeof(Texture2D)) as Texture2D;
        Texture2D frameArt = Resources.Load("Art/Borders/FullBorder"+colors[color], typeof(Texture2D)) as Texture2D;
        Texture2D backColor = Resources.Load("Art/Background/Background"+colors[color], typeof(Texture2D)) as Texture2D;
        Texture2D frontFX = Resources.Load("Art/Misc/BlackRain", typeof(Texture2D)) as Texture2D;
        Texture2D backgroundArt = Resources.Load("Art/Background/"+artworkName, typeof(Texture2D)) as Texture2D;
        if(isSpawned){
            charArt = Resources.Load("Art/Character Art/"+gameObject.name, typeof(Texture2D)) as Texture2D;
            frameArt = Resources.Load("Art/Borders/FieldBorder"+colors[color], typeof(Texture2D)) as Texture2D;
        }

        //Set Front Materials
        GetComponent<Renderer>().materials[0].SetTexture("_BackgroundTex", backColor);
        GetComponent<Renderer>().materials[0].SetTexture("_BackFXTex", backgroundArt);
        GetComponent<Renderer>().materials[0].SetTexture("_BaseMap", charArt);
        GetComponent<Renderer>().materials[0].SetTexture("_FrontFXTex", frontFX);
        GetComponent<Renderer>().materials[0].SetTexture("_FrameTex", frameArt);
        */
    }

    public void takeDamage(int dmg){
        health += -dmg;
        updateCardInfo();
    }

    public void healCard(int amt){
        health += amt;
        if (health > card.health){
            health = card.health;
        }
        updateCardInfo();
    }

    public bool checkForDeath(){
        if(health <= 0){
            health = 0;
            isDead=true;
        }
        return isDead;
    }

}
