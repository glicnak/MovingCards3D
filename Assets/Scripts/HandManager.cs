using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    public static HandManager Instance;
    public int handSize;

    [Header("Radial Layout Info")]

    [SerializeField, Range(0, 20)]
    public int maxHandSize = 7;

    [SerializeField]
    public float radius = 18;

    void Awake(){
        Instance = this;
    }

    void Start(){
        //Set Radius
        GetComponent<LayoutGroup3D>().Radius = radius;
    }

    public void reManageHand(){
        enabled = false;
        enabled = true;
    }

    void Update(){
        //Count the number of cards in hand
        handSize = transform.childCount;

        //Fix the layout based on the number of cards
        if(handSize < 2){
            GetComponent<LayoutGroup3D>().MaxArcAngle = 0;
            GetComponent<LayoutGroup3D>().StartAngleOffset = -90;
        }
        else{
            GetComponent<LayoutGroup3D>().MaxArcAngle = -1f + ((float)maxHandSize/2)*handSize;
            GetComponent<LayoutGroup3D>().StartAngleOffset = -89.5f - ((float)maxHandSize/4)*handSize;
        }

        //Set the angles of the cards
        if(handSize%2 == 0){   //if even number of cards
            for (int i=0; i<transform.childCount; i++){
                float cardRotation = (2*i + 1 - handSize)*GetComponent<LayoutGroup3D>().MaxArcAngle/(2*handSize + 1); 
                transform.GetChild(i).rotation = Quaternion.Euler(transform.GetChild(i).rotation.x, cardRotation, transform.GetChild(i).rotation.z);
            }
        }
        else {   //if odd number of cards
            for (int i=0; i<transform.childCount; i++){
                float cardRotation = (i-(handSize-1)/2)*GetComponent<LayoutGroup3D>().MaxArcAngle/handSize;
                transform.GetChild(i).rotation = Quaternion.Euler(transform.GetChild(i).rotation.x, cardRotation, transform.GetChild(i).rotation.z); 
            }
        }

        //Set the heights of the cards
        for (int i=0; i<transform.childCount; i++){
            transform.GetChild(i).position = new Vector3(transform.GetChild(i).position.x, transform.position.y-(float)(handSize-i-1)/(100*handSize), transform.GetChild(i).position.z);
        }

        //Set the scale of the cards
        foreach(Transform child in transform){
            child.localScale = new Vector3 (child.GetComponent<CardDrag>().riseScale, child.localScale.y, child.GetComponent<CardDrag>().riseScale);
        }
    }
}
