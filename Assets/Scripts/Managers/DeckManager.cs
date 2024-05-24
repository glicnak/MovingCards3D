using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;

    public List<Card> allCards;
    public List<Card> myDeck;
    
    void Awake(){
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    void Start(){
        makeAllCardList();
        List<string> myDeckString = new List<string>(){"Nick Klein", "Joker", "Circus Freak"};
        foreach (string s in myDeckString){
            myDeck.Add(Resources.Load<Card>("Cards/"+s));
        }
        makeMyDeck();
    }

    public void makeAllCardList(){
        Card[] allCardsArray = Resources.LoadAll<Card>("Cards/");
        foreach (Card c in allCardsArray){
            allCards.Add(c);
        }
        shuffleDeck(allCards);
    }

    public void makeMyDeck(){
        foreach (Card c in allCards){
            if(myDeck.Count >= 13){
                break;
            }
            if (!myDeck.Contains(c)){
                myDeck.Add(c);
                shuffleDeck(myDeck);
            }
        }
    }

    private static Random rng = new Random();  
    
    public void shuffleDeck(List<Card> deck){
        int n = deck.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            Card value = deck[k];  
            deck[k] = deck[n];  
            deck[n] = value;  
        }  
    }
}
