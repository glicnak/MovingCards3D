using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance;

    //Server Variables
    public List<NetworkIdentity> players; 
    public List<NetworkIdentity> playersConfirmedOrder; 
    public Dictionary <NetworkIdentity, List<GameObject>> playersConfirmedActions = new Dictionary <NetworkIdentity, List<GameObject>>();
    public List<GameObject> cardActions;
    public int roundCount;
    public int turnCount;
    public int defaultMaxHandSize = 7;
    public float timeToWait = 0.5f;

    //Player Variables
    public Dictionary<NetworkIdentity, List<Card>> playerDecks = new Dictionary<NetworkIdentity, List<Card>>();
    public Dictionary<NetworkIdentity, int> playerStreetChaos = new Dictionary<NetworkIdentity, int>();
    public Dictionary<NetworkIdentity, int> playerMana = new Dictionary<NetworkIdentity, int>();
    public Dictionary<NetworkIdentity, int> playerMaxHandSize = new Dictionary<NetworkIdentity, int>();
    public Dictionary<NetworkIdentity, int> playerBonusManaThisTurn = new Dictionary<NetworkIdentity, int>();
    public Dictionary<NetworkIdentity, int> playerBonusManaNextTurn = new Dictionary<NetworkIdentity, int>();
    public Dictionary<NetworkIdentity, List<Card>> playerHands = new Dictionary<NetworkIdentity, List<Card>>();
    //for Inspector purposes
    public List<Card> player1Deck;
    public List<Card> player2Deck;
    public List<Card> player1Hand;
    public List<Card> player2Hand;
    public int player1StreetChaos;
    public int player2StreetChaos;
    public int player1Mana;
    public int player2Mana;
    public int player1MaxHandSize;
    public int player2MaxHandSize;
    public int player1BonusManaThisTurn;
    public int player2BonusManaThisTurn;
    public int player1BonusManaNextTurn;
    public int player2BonusManaNextTurn;
    //end

//Start
    public void Awake(){
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

//Inspector Stuff
    public void updateInspectorInfo(){
        if (players.Count>1){
            player1Deck = playerDecks[players[0]];
            player1Hand = playerHands[players[0]];
            player2Deck = playerDecks[players[1]];
            player2Hand = playerHands[players[1]];
            player1StreetChaos = playerStreetChaos[players[0]];
            player2StreetChaos = playerStreetChaos[players[1]];
            player1Mana = playerMana[players[0]];
            player2Mana = playerMana[players[1]];
            player1MaxHandSize = playerMaxHandSize[players[0]];
            player2MaxHandSize = playerMaxHandSize[players[1]];
            player1BonusManaThisTurn = playerBonusManaThisTurn[players[0]];
            player2BonusManaThisTurn = playerBonusManaThisTurn[players[1]];
            player1BonusManaNextTurn = playerBonusManaThisTurn[players[0]];
            player2BonusManaNextTurn = playerBonusManaThisTurn[players[1]];
        }
    }


//When Players Join
    public void addPlayer(NetworkIdentity id){
        List<Card> cardsInHand = new List<Card>();
        players.Add(id);
        playerHands.Add(id, cardsInHand);
        playerStreetChaos.Add(id, 0);
        playerMana.Add(id, 0);
        playerMaxHandSize.Add(id,defaultMaxHandSize);
        playerBonusManaThisTurn.Add(id, 0);
        playerBonusManaNextTurn.Add(id, 0);
    }

    public void createPlayerDeck(NetworkIdentity id, List<Card> playerCards){
        List<Card> playerDeck = new List<Card>();
        foreach (Card a in playerCards){
            foreach (Card b in DeckManager.Instance.allCards){
                if(a.name == b.name){
                    playerDeck.Add(b);
                }
            }
        }
        playerDecks.Add(id, playerDeck);
        updateInspectorInfo();
    }

//Start Game
    public bool startGame(){
        if (players.Count == 2){
            roundCount = 1;
            return true;
        }
        return false;
    }

//Retrieve Player Info
    public List<Card> getHand(NetworkIdentity id){
        foreach(KeyValuePair<NetworkIdentity, List<Card>> entry in playerHands){
            if (id == entry.Key){
                return entry.Value;
            }  
        }
        return null; 
    }

    public Card getTopCardOfDeck(NetworkIdentity id){
        foreach(KeyValuePair<NetworkIdentity, List<Card>> entry in playerDecks){
            if (id == entry.Key && entry.Value.Any()){
                return entry.Value.First();
            }  
        }
        return null;      
    }

    public int getMaxHandSize(NetworkIdentity id){
        foreach(KeyValuePair<NetworkIdentity, int> entry in playerMaxHandSize){
            if (id == entry.Key){
                return entry.Value;
            }  
        }
        return defaultMaxHandSize; 
    }
    
//Cards in Hand & Deck
    public void addCardInHand(NetworkIdentity id, Card card){
        foreach(KeyValuePair<NetworkIdentity, List<Card>> entry in playerHands){
            if (id == entry.Key){
                entry.Value.Add(card);
            }
        }
        updateInspectorInfo();
    }

    public void removeTopCardInDeck(NetworkIdentity id){
        foreach(KeyValuePair<NetworkIdentity, List<Card>> entry in playerDecks){
            if (id == entry.Key && entry.Value.Any()){
                entry.Value.RemoveAt(0);
            }
        }
        updateInspectorInfo();
    }

    public void removeCardInDeck(NetworkIdentity id, Card card){
        foreach(KeyValuePair<NetworkIdentity, List<Card>> entry in playerDecks){
            if (id == entry.Key && entry.Value.Contains(card)){
                entry.Value.Remove(card);
            }
        }
        updateInspectorInfo();
    }

    public void addCardToDeck(NetworkIdentity id, Card card){
        foreach(KeyValuePair<NetworkIdentity, List<Card>> entry in playerDecks){
            if (id == entry.Key){
                entry.Value.Add(card);
            }
        }
        updateInspectorInfo();
    }

//Destroy Cards
    public void destroyCardInHand(NetworkIdentity id, int cardIndex){
        foreach(KeyValuePair<NetworkIdentity, List<Card>> entry in playerHands){
            if (id == entry.Key){
                entry.Value.RemoveAt(cardIndex);
            }
        }
        updateInspectorInfo();
    }
}
