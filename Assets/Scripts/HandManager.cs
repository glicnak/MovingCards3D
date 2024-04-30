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
    public float radius = 4.2f;

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
            GetComponent<LayoutGroup3D>().StartAngleOffset = -113;
        }
        else if(handSize == 2){
            GetComponent<LayoutGroup3D>().MaxArcAngle = 30;
            GetComponent<LayoutGroup3D>().StartAngleOffset = -130;
        }
        else if(handSize == 3){
            GetComponent<LayoutGroup3D>().MaxArcAngle = 54;
            GetComponent<LayoutGroup3D>().StartAngleOffset = -142;
        }
        else{
            GetComponent<LayoutGroup3D>().MaxArcAngle = 90f - 6*(System.Math.Max(maxHandSize,handSize)-handSize);
            GetComponent<LayoutGroup3D>().StartAngleOffset = -155f + 3*(System.Math.Max(maxHandSize,handSize)-handSize);
        }

        //Set the angles of the cards
        if(handSize < 2){
            for (int i=0; i<transform.childCount; i++){
                transform.GetChild(i).rotation = Quaternion.Euler(transform.GetChild(i).rotation.x, -6, transform.GetChild(i).rotation.z); 
            }
        }
        else{
            for (int i=0; i<transform.childCount; i++){
                float cardRotation = -35 + 155 + GetComponent<LayoutGroup3D>().StartAngleOffset + i*(48 - (90-GetComponent<LayoutGroup3D>().MaxArcAngle)/2)/handSize;
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
