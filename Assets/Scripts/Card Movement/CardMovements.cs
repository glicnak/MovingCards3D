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
  private float oppMovementFactor = 0.6f;

  [Header("Tween Ease Functions")]  

  [SerializeField]
  private Ease easeIn = Ease.Quart;  

  [SerializeField]
  private Ease easeOut = Ease.Quad;  

  [SerializeField]
  private Ease easeOutHeight = Ease.Bounce; 

  [Header("Movement Options")]  

  [SerializeField, Range(0.0f, 10.0f)]
  private float moveDuration = 0.777f; 

  [SerializeField, Range(0.0f, 10.0f)]
  private float movementHeight = 0.42f; 

  [SerializeField, Range(0.0f, 1.0f)]
  private float moveMovementFactor = 0.32f; 

  [SerializeField]
  private Ease moveEaseIn = Ease.Sine;  

  [SerializeField]
  private Ease moveEaseOut = Ease.Quad;  

  [SerializeField]
  private Ease moveEaseOut1Height = Ease.Back; 

  [SerializeField]
  private Ease moveEaseOut2Height = Ease.Bounce; 

  private float startingYRotation;

    public void playPlayerCard(GameObject location){
        if (location == null || !GetComponent<CardValues>().isSelectable){
          return;
        }
        GetComponent<CardValues>().isSelectable = false; 

        //Generate random rotation
        System.Random rng = new System.Random();
        float randomRotation = rng.Next(-(int)GetComponent<CardDrag>().dropRotationRange*2, Math.Max(0, (int)GetComponent<CardDrag>().dropRotationRange*2-1)) * 0.5f;
        if(randomRotation == 0){
            randomRotation = GetComponent<CardDrag>().dropRotationRange;
        }

        //Get Y Rotation
        startingYRotation = transform.eulerAngles.y;
        if(transform.parent != null && transform.parent.gameObject == PlayerManager.Instance.hand && startingYRotation > 180){
          startingYRotation += -360;
        }
        

        //SetParent
        transform.SetParent(location.transform.parent.GetChild(0), true);
        
        //play card
        GetComponent<CardDrag>().currentYRotation = location.transform.eulerAngles.y + randomRotation;
        createTweenPlayCard(transform.position, startingYRotation, location.transform.position, location.transform.position.y, GetComponent<CardDrag>().currentYRotation, GetComponent<CardDrag>().regularScale, playDuration, movementFactor, easeIn, easeOut, easeOutHeight);
    }

    public void playOpponentCard(GameObject location){
        if (location == null || !GetComponent<CardValues>().isSelectable){
          return;
        }
        GetComponent<CardValues>().isSelectable = false; 

        //Generate random rotation
        System.Random rng = new System.Random();
        float randomRotation = rng.Next(-(int)GetComponent<CardDrag>().dropRotationRange*2, Math.Max(0, (int)GetComponent<CardDrag>().dropRotationRange*2-1)) * 0.5f;
        if(randomRotation == 0){
            randomRotation = GetComponent<CardDrag>().dropRotationRange;
            }

        //SetParent
        transform.SetParent(location.transform.parent.GetChild(0), true);

        //Get Y rotation
        startingYRotation = 180;
        if(location.transform.parent.GetSiblingIndex() > 10){
          startingYRotation = -180;
        }

        //play card
        GetComponent<CardDrag>().currentYRotation = location.transform.eulerAngles.y + randomRotation;
        createTweenPlayCard(transform.position, startingYRotation, location.transform.position, location.transform.position.y, GetComponent<CardDrag>().currentYRotation, GetComponent<CardDrag>().regularScale, oppPlayDuration, oppMovementFactor, easeIn, easeOut, easeOutHeight);
    }

    public void moveCard(GameObject location){
        if (location == null || !GetComponent<CardValues>().isSelectable){
          return;
        }
        GetComponent<CardValues>().isSelectable = false;

        //Generate random rotation
        System.Random rng = new System.Random();
        float randomRotation = rng.Next(-(int)GetComponent<CardDrag>().dropRotationRange*2, Math.Max(0, (int)GetComponent<CardDrag>().dropRotationRange*2-1)) * 0.5f;
        if(randomRotation == 0){
            randomRotation = GetComponent<CardDrag>().dropRotationRange;
            }

        //SetParent
        transform.SetParent(location.transform.parent.GetChild(0), true);

        //play card
        GetComponent<CardDrag>().currentYRotation = location.transform.eulerAngles.y + randomRotation;
        createTweenMoveCardUp(transform.position, location.transform.position, GetComponent<CardDrag>().currentYRotation, moveDuration, moveMovementFactor);
    }

    private void createTweenPlayCard(Vector3 dragOriginPosition, float startingYRotation, Vector3 desiredPosition, float height, float desiredYRotation, float desiredScale, float duration, float placementFactor, FronkonGames.TinyTween.Ease easeIn, FronkonGames.TinyTween.Ease easeOut, FronkonGames.TinyTween.Ease easeOutHeight){
      if (desiredYRotation >180){
        startingYRotation += 360;
      }
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
        .Origin(startingYRotation)
        .Destination(desiredYRotation)
        .Duration(duration)
        .EasingIn(easeIn)
        .EasingOut(easeOut)
        .OnUpdate(tween => transform.rotation = Quaternion.Euler(transform.eulerAngles.x, tween.Value, transform.eulerAngles.z))
        .Owner(this)
        .Start();
      TweenFloat.Create()
        .Origin(transform.localScale.x)
        .Destination(desiredScale)
        .Duration(duration)
        .EasingIn(easeIn)
        .EasingOut(easeOut)
        .OnUpdate(tween => transform.localScale = new Vector3(tween.Value, transform.localScale.y, transform.localScale.z))
        .Owner(this)
        .Start();
      TweenFloat.Create()
        .Origin(transform.localScale.z)
        .Destination(desiredScale)
        .Duration(duration)
        .EasingIn(easeIn)
        .EasingOut(easeOut)
        .OnUpdate(tween => transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, tween.Value))
        .OnEnd(_ => GetComponent<CardValues>().isSelectable = true)
        .Owner(this)
        .Start();
    }

      private void createTweenMoveCardUp(Vector3 dragOriginPosition, Vector3 desiredPosition, float desiredYRotation, float duration, float placementFactor){
        if (desiredYRotation >180){
          startingYRotation += 360;
        }
        TweenFloat.Create()
          .Origin(dragOriginPosition.x)
          .Destination(dragOriginPosition.x + (desiredPosition.x-dragOriginPosition.x)/2)
          .Duration(duration * placementFactor)
          .EasingIn(moveEaseIn)
          .EasingOut(moveEaseIn)
          .OnUpdate(tween => transform.position = new Vector3(tween.Value, transform.position.y, transform.position.z))
          .Owner(this)
          .Start();
        TweenFloat.Create()
          .Origin(dragOriginPosition.y)
          .Destination(movementHeight)
          .Duration(duration * placementFactor)
          .EasingIn(moveEaseIn)
          .EasingOut(moveEaseIn)
          .OnUpdate(tween => transform.position = new Vector3(transform.position.x, tween.Value, transform.position.z))
          .Owner(this)
          .Start();
        TweenFloat.Create()
          .Origin(dragOriginPosition.z)
          .Destination(dragOriginPosition.z + (desiredPosition.z-dragOriginPosition.z)/2)
          .Duration(duration * placementFactor)
          .EasingIn(moveEaseIn)
          .EasingOut(moveEaseIn)
          .OnUpdate(tween => transform.position = new Vector3(transform.position.x, transform.position.y, tween.Value))
          .Owner(this)
          .Start();
        TweenFloat.Create()
          .Origin(transform.eulerAngles.y)
          .Destination(transform.eulerAngles.y + (desiredYRotation-transform.eulerAngles.y)/2)
          .Duration(duration * placementFactor)
          .EasingIn(moveEaseIn)
          .EasingOut(moveEaseIn)
          .OnUpdate(tween => transform.rotation = Quaternion.Euler(transform.eulerAngles.x, tween.Value, transform.eulerAngles.z))
          .OnEnd(_ => createTweenMoveCardDown(transform.position, desiredPosition, desiredYRotation, duration, placementFactor))
          .Owner(this)
          .Start();
    }

      private void createTweenMoveCardDown(Vector3 dragOriginPosition, Vector3 desiredPosition, float desiredYRotation, float duration, float placementFactor){
        if (desiredYRotation >180){
          startingYRotation += 360;
        }
        TweenFloat.Create()
          .Origin(dragOriginPosition.x)
          .Destination(desiredPosition.x)
          .Duration(duration * (1-placementFactor))
          .EasingIn(moveEaseOut)
          .EasingOut(moveEaseOut)
          .OnUpdate(tween => transform.position = new Vector3(tween.Value, transform.position.y, transform.position.z))
          .Owner(this)
          .Start();
        TweenFloat.Create()
          .Origin(dragOriginPosition.y)
          .Destination(desiredPosition.y)
          .Duration(duration * (1-placementFactor))
          .EasingIn(moveEaseOut1Height)
          .EasingOut(moveEaseOut2Height)
          .OnUpdate(tween => transform.position = new Vector3(transform.position.x, tween.Value, transform.position.z))
          .Owner(this)
          .Start();
        TweenFloat.Create()
          .Origin(dragOriginPosition.z)
          .Destination(desiredPosition.z)
          .Duration(duration * (1-placementFactor))
          .EasingIn(moveEaseOut)
          .EasingOut(moveEaseOut)
          .OnUpdate(tween => transform.position = new Vector3(transform.position.x, transform.position.y, tween.Value))
          .Owner(this)
          .Start();
        TweenFloat.Create()
          .Origin(transform.eulerAngles.y)
          .Destination(desiredYRotation)
          .Duration(duration * (1-placementFactor))
          .EasingIn(moveEaseOut)
          .EasingOut(moveEaseOut)
          .OnUpdate(tween => transform.rotation = Quaternion.Euler(transform.eulerAngles.x, tween.Value, transform.eulerAngles.z))
          .OnEnd(_ => GetComponent<CardValues>().isSelectable = true)
          .Owner(this)
          .Start();
    }


    void Update(){
        /*
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

        if(Input.GetKeyDown("m") && gameObject == UnityEditor.Selection.activeGameObject){
            moveCard(transform.parent.parent.GetChild(transform.parent.GetSiblingIndex()+1).GetChild(transform.parent.parent.GetChild(transform.parent.GetSiblingIndex()+1).childCount-1).gameObject);
        }

        if(Input.GetKeyDown("n") && gameObject == UnityEditor.Selection.activeGameObject){
            moveCard(transform.parent.parent.GetChild(transform.parent.GetSiblingIndex()-1).GetChild(transform.parent.parent.GetChild(transform.parent.GetSiblingIndex()-1).childCount-1).gameObject);
        }*/
    }
}
