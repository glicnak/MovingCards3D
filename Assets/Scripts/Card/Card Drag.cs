////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2022 Martin Bustos @FronkonGames <fronkongames@gmail.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of
// the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;
using FronkonGames.TinyTween;

/// <summary>
/// Drag card.
/// </summary>
[RequireComponent(typeof(Collider))]
public sealed class CardDrag : MonoBehaviour, IDrag
{
  //defines the mouse position
  [HideInInspector]
  public Vector3 worldPosition;
  //Gives the current X rotation of the card
  [HideInInspector]
  public float currentXRotation;
  //Gives the current Y rotation of the card
  [HideInInspector]
  public float currentYRotation;
  //Gives the current YZ rotation of the card
  [HideInInspector]
  public float currentZRotation;
  //Changes what the rest Time should be depending on the ongoing action
  [HideInInspector]
  public float currentTiltTime;

  public bool IsDraggable { get; private set; } = true;

  public bool Dragging { get; set; }

  [SerializeField]
  private Ease riseEaseIn = Ease.Linear;  

  [SerializeField]
  private Ease riseEaseOut = Ease.Linear;  

  [SerializeField]
  private Ease riseEaseOutHeight = Ease.Linear;  

  [SerializeField, Range(0.0f, 5.0f)]
  private float riseDuration = 0.2f;

  //How much quicker the non-height movements finish. Helps with following the cursor more properly
  [SerializeField, Range(0.0f, 1.0f)]
  private float risePlacementFactor = 0.5f;

  [SerializeField, Range(0.0f, 500.0f)]
  private float riseScale = 130.0f;    

  [SerializeField]
  private Ease dropEaseIn = Ease.Linear;  

  [SerializeField]
  private Ease dropEaseOut = Ease.Linear;  

  [SerializeField]
  private Ease dropEaseOutHeight = Ease.Linear;  

  [SerializeField, Range(0.0f, 5.0f)]
  private float dropDuration = 0.2f;

  //How much quicker the non-height movements finish.  
  [SerializeField, Range(0.0f, 1.0f)]
  private float dropPlacementFactor = 0.7f;  

  [SerializeField]
  private Ease invalidDropEaseIn = Ease.Linear;  

  [SerializeField]
  private Ease invalidDropEaseOut = Ease.Linear;  

  [SerializeField]
  private Ease invalidDropEaseOutHeight = Ease.Linear;  

  [SerializeField, Range(0.0f, 5.0f)]
  private float invalidDropDuration = 0.2f;  
  
  //How much quicker the non-height movements finish.  
  [SerializeField, Range(0.0f, 1.0f)]
  private float invalidDropPlacementFactor = 0.9f;

  [SerializeField, Range(0.0f, 45.0f)]
  private float dropRotationRange = 2.0f;  

  [SerializeField]
  private bool dropHasEmptyParent = true;  

  private Vector3 dragOriginPosition;
  private Vector3 initalScale;  
  private float droppableYValue; 

  public void OnPointerEnter(Vector3 position) { }

  public void OnPointerExit(Vector3 position) { }

  public void OnBeginDrag(Vector3 position)
  {
    if(GetComponent<CardValues>().isSelectable && !GetComponent<CardValues>().isBeingDragged){
      GetComponent<CardValues>().isBeingDragged = true;
      GetComponent<CardValues>().isSelectable = false;
      dragOriginPosition = transform.position;
      currentTiltTime = GetComponent<CardTilter>().restTime;
      float height = position.y;

      IsDraggable = false;

      createTweenMoves(dragOriginPosition, transform.localScale, worldPosition, height, 0, riseScale, riseDuration, risePlacementFactor, riseEaseIn, riseEaseOut, riseEaseOutHeight, true);
      
      currentYRotation = 0;

    }
  }

  public void OnDrag(Vector3 deltaPosition, GameObject droppable)
  {
    if(GetComponent<CardValues>().isBeingDragged){
      GetComponent<CardValues>().isSelectable = false;
      deltaPosition.y = 0.0f;
      transform.position += deltaPosition;
    }
  }

  public void OnEndDrag(Vector3 position, GameObject droppable)
  {
    float height = position.y;
    GetComponent<CardValues>().isBeingDragged = false;
    
    //Generate random rotation
    System.Random rng = new System.Random();
    float randomRotation = rng.Next(-(int)dropRotationRange*2, Math.Max(0, (int)dropRotationRange*2-1)) * 0.5f;
    if(randomRotation == 0){
      randomRotation = dropRotationRange;
    }

    if (droppable != null && droppable.transform.GetComponent<IDrop>() is { IsDroppable: true } && droppable.transform.GetComponent<IDrop>().AcceptDrop(this) == true){
      
      //Set Droppable's parent as new parent (the object becoming the parent needs a scale of 1,1,1)
      if(dropHasEmptyParent){
        transform.SetParent(droppable.transform.parent, true);
        transform.SetSiblingIndex(droppable.transform.parent.childCount -2);
      }
      else{
        transform.SetParent(droppable.transform, true);
      }

      //Get the rotation of the droppable
      droppableYValue = UnityEditor.TransformUtils.GetInspectorRotation(droppable.transform).y;

      //Do the move
      currentTiltTime = Math.Max(0, dropDuration * 0.9f);
      createTweenMoves(transform.position, transform.localScale, droppable.transform.position, height, droppableYValue + randomRotation, initalScale.x, dropDuration, dropPlacementFactor, dropEaseIn, dropEaseOut, dropEaseOutHeight, true);
      
    }
    
    else
    {
      //Return the object to where it was
      IsDraggable = false;
      currentTiltTime = Math.Max(0, invalidDropDuration * 0.9f);
      createTweenMoves(transform.position, transform.localScale, dragOriginPosition, height, droppableYValue + randomRotation, initalScale.x, invalidDropDuration, invalidDropPlacementFactor, invalidDropEaseIn, invalidDropEaseOut, invalidDropEaseOutHeight, true);

    }

    currentYRotation = droppableYValue + randomRotation;
    
  }

  private void OnEnable()
  {
    dragOriginPosition = transform.position;
    initalScale = transform.localScale;
    currentYRotation = transform.rotation.y;
    droppableYValue = currentYRotation;
    currentTiltTime = GetComponent<CardTilter>().restTime;
  }

  private void createTweenMoves(Vector3 dragOriginPosition, Vector3 dragOriginScale, Vector3 desiredPosition, float height, float desiredYRotation, float desiredScale, float duration, float placementFactor, FronkonGames.TinyTween.Ease easeIn, FronkonGames.TinyTween.Ease easeOut, FronkonGames.TinyTween.Ease easeOutHeight, bool isItDraggable){
      TweenFloat.Create()
        .Origin(dragOriginPosition.x)
        .Destination(desiredPosition.x)
        .Duration(duration * placementFactor)
        .EasingIn(easeIn)
        .EasingOut(easeOut)
        .OnUpdate(tween => transform.position = new Vector3(tween.Value, transform.position.y, transform.position.z))
        .Owner(this)
        .Start();
      TweenFloat.Create()
        .Origin(dragOriginPosition.y)
        .Destination(height)
        .Duration(duration)
        .EasingIn(easeIn)
        .EasingOut(easeOutHeight)
        .OnUpdate(tween => transform.position = new Vector3(transform.position.x, tween.Value, transform.position.z))
        .Owner(this)
        .Start();
      TweenFloat.Create()
        .Origin(dragOriginPosition.z)
        .Destination(desiredPosition.z)
        .Duration(duration * placementFactor)
        .EasingIn(easeIn)
        .EasingOut(easeOut)
        .OnUpdate(tween => transform.position = new Vector3(transform.position.x, transform.position.y, tween.Value))
        .Owner(this)
        .Start();
      TweenFloat.Create()
        .Origin(currentYRotation)
        .Destination(desiredYRotation)
        .Duration(duration)
        .EasingIn(easeIn)
        .EasingOut(easeOut)
        .OnUpdate(tween => transform.rotation = Quaternion.Euler(UnityEditor.TransformUtils.GetInspectorRotation(transform).x, tween.Value, UnityEditor.TransformUtils.GetInspectorRotation(transform).z))
        .Owner(this)
        .Start();
      TweenFloat.Create()
        .Origin(dragOriginScale.x)
        .Destination(desiredScale)
        .Duration(duration)
        .EasingIn(easeIn)
        .EasingOut(easeOut)
        .OnUpdate(tween => transform.localScale = new Vector3(tween.Value, transform.localScale.y, transform.localScale.z))
        .OnEnd(_ => IsDraggable = isItDraggable)
        .Owner(this)
        .Start();
      TweenFloat.Create()
        .Origin(dragOriginScale.z)
        .Destination(desiredScale)
        .Duration(duration)
        .EasingIn(easeIn)
        .EasingOut(easeOut)
        .OnUpdate(tween => transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, tween.Value))
        .OnEnd(_ => GetComponent<CardValues>().isSelectable = true)
        .Owner(this)
        .Start();
  }

  void Update(){
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y);
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
  }
}