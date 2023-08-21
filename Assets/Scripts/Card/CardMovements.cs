using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FronkonGames.TinyTween;

public class CardMovements : MonoBehaviour
{
  [Header("Player Plays Cards")]

  [SerializeField, Range(0.0f, 10.0f)]
  private float playDuration = 1.8f; 

  [SerializeField, Range(0.0f, 1.0f)]
  private float movementFactor = 0.6f;  

  [Header("Opponent Plays Cards")]

  [SerializeField, Range(0.0f, 10.0f)]
  private float oppPlayDuration = 1.8f; 

  [SerializeField, Range(0.0f, 1.0f)]
  private float oppMovementFactor = 0.4f;

  [Header("Movement Functions")]  

  [SerializeField]
  private Ease easeIn = Ease.Sine;  

  [SerializeField]
  private Ease easeOut = Ease.Quad;  

  [SerializeField]
  private Ease easeOutHeight = Ease.Bounce;    

    public void playPlayerCard(GameObject location){
        //Generate random rotation
        System.Random rng = new System.Random();
        float randomRotation = rng.Next(-(int)GetComponent<CardDrag>().dropRotationRange*2, Math.Max(0, (int)GetComponent<CardDrag>().dropRotationRange*2-1)) * 0.5f;
        if(randomRotation == 0){
            randomRotation = GetComponent<CardDrag>().dropRotationRange;
            }

        //SetParent
        if(GetComponent<CardDrag>().dropHasEmptyParent){
          transform.SetParent(location.transform.parent, true);
          transform.SetSiblingIndex(location.transform.parent.childCount -2);
        }
        else{
          transform.SetParent(location.transform, true);
        }

        //play card
        GetComponent<CardDrag>().currentYRotation = UnityEditor.TransformUtils.GetInspectorRotation(location.transform).y + randomRotation;
        createTweenPlayCardMove(transform.position, 0.0f, location.transform.position, location.transform.position.y, GetComponent<CardDrag>().currentYRotation, playDuration, movementFactor, easeIn, easeOut, easeOutHeight);
    }

    public void playOpponentCard(GameObject location){
        //Generate random rotation
        System.Random rng = new System.Random();
        float randomRotation = rng.Next(-(int)GetComponent<CardDrag>().dropRotationRange*2, Math.Max(0, (int)GetComponent<CardDrag>().dropRotationRange*2-1)) * 0.5f;
        if(randomRotation == 0){
            randomRotation = GetComponent<CardDrag>().dropRotationRange;
            }

        //SetParent
        if(GetComponent<CardDrag>().dropHasEmptyParent){
          transform.SetParent(location.transform.parent, true);
          transform.SetSiblingIndex(location.transform.parent.childCount -2);
        }
        else{
          transform.SetParent(location.transform, true);
        }

        //play card
        GetComponent<CardDrag>().currentYRotation = UnityEditor.TransformUtils.GetInspectorRotation(location.transform).y + randomRotation;
        createTweenPlayCardMove(transform.position, 180, location.transform.position, location.transform.position.y, GetComponent<CardDrag>().currentYRotation, oppPlayDuration, oppMovementFactor, easeIn, easeOut, easeOutHeight);
    }

    private void createTweenPlayCardMove(Vector3 dragOriginPosition, float startingZRotation, Vector3 desiredPosition, float height, float desiredYRotation, float duration, float placementFactor, FronkonGames.TinyTween.Ease easeIn, FronkonGames.TinyTween.Ease easeOut, FronkonGames.TinyTween.Ease easeOutHeight){
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
        .Origin(UnityEditor.TransformUtils.GetInspectorRotation(transform).y)
        .Destination(desiredYRotation)
        .Duration(duration)
        .EasingIn(easeIn)
        .EasingOut(easeOut)
        .OnUpdate(tween => transform.rotation = Quaternion.Euler(UnityEditor.TransformUtils.GetInspectorRotation(transform).x, tween.Value, UnityEditor.TransformUtils.GetInspectorRotation(transform).z))
        .Owner(this)
        .Start();
      TweenFloat.Create()
        .Origin(startingZRotation)
        .Destination(0)
        .Duration(duration * placementFactor)
        .EasingIn(easeIn)
        .EasingOut(easeOut)
        .OnUpdate(tween => transform.rotation = Quaternion.Euler(UnityEditor.TransformUtils.GetInspectorRotation(transform).x, UnityEditor.TransformUtils.GetInspectorRotation(transform).y, tween.Value))
        .OnEnd(_ => GetComponent<CardValues>().isSelectable = true)
        .Owner(this)
        .Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("a") && gameObject == UnityEditor.Selection.activeGameObject){
            playPlayerCard(GameObject.Find("Cube BL (3)"));
        }

        if(Input.GetKeyDown("s") && gameObject == UnityEditor.Selection.activeGameObject){
            playPlayerCard(GameObject.Find("Cube BL (2)"));
        }
        
        if(Input.GetKeyDown("d") && gameObject == UnityEditor.Selection.activeGameObject){
            playPlayerCard(GameObject.Find("Cube BL (1)"));
        }

        if(Input.GetKeyDown("g") && gameObject == UnityEditor.Selection.activeGameObject){
            playPlayerCard(GameObject.Find("Cube BR (1)"));
        }

        if(Input.GetKeyDown("h") && gameObject == UnityEditor.Selection.activeGameObject){
            playPlayerCard(GameObject.Find("Cube BR (2)"));
        }
        
        if(Input.GetKeyDown("j") && gameObject == UnityEditor.Selection.activeGameObject){
            playPlayerCard(GameObject.Find("Cube BR (3)"));
        }

        if(Input.GetKeyDown("q") && gameObject == UnityEditor.Selection.activeGameObject){
            playOpponentCard(GameObject.Find("Cube TL (3)"));
        }

        if(Input.GetKeyDown("w") && gameObject == UnityEditor.Selection.activeGameObject){
            playOpponentCard(GameObject.Find("Cube TL (2)"));
        }

        if(Input.GetKeyDown("e") && gameObject == UnityEditor.Selection.activeGameObject){
            playOpponentCard(GameObject.Find("Cube TL (1)"));
        }

        if(Input.GetKeyDown("t") && gameObject == UnityEditor.Selection.activeGameObject){
            playOpponentCard(GameObject.Find("Cube TR (1)"));
        }

        if(Input.GetKeyDown("y") && gameObject == UnityEditor.Selection.activeGameObject){
            playOpponentCard(GameObject.Find("Cube TR (2)"));
        }

        if(Input.GetKeyDown("u") && gameObject == UnityEditor.Selection.activeGameObject){
            playOpponentCard(GameObject.Find("Cube TR (3)"));
        }
    }
}
