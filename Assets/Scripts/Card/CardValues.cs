using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardValues : MonoBehaviour
{
    //For Card Drag
    public bool isSelectable = true;
    public bool isBeingDragged = false;

    //Round Start
    public Transform roundStartParent;

    void Start(){
        roundStartParent = transform.parent;
        GetComponent<CardDrag>().draggedCardParent = GameObject.Find("Selected Card Area");
    }

}
