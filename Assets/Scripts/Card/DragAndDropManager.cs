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
using System;
using UnityEngine;

/// <summary>
/// Drag and Drop manager.
/// </summary>
public sealed class DragAndDropManager : MonoBehaviour
{
  // Layer of the objects to be detected.
  [SerializeField]
  private LayerMask raycastMask;

  [SerializeField, Range(0.1f, 2.0f)]
  private float dragSpeed = 1.0f;

  // Height at which we want the card to be in a drag operation.
  [SerializeField, Range(0.0f, 10.0f)]
  private float height = 1.0f;

  [SerializeField]
  private Vector2 cardSize;

  // Object to which we are doing a drag operation
  // or null if no drag operation currently exists.
  private IDrag currentDrag;
  
  // IDrag objects that the mouse passes over.
  private IDrag possibleDrag;

  // To know the position of the drag object.
  private Transform currentDragTransform;

  // How many impacts of the beam we want to obtain.
  private const int HitsCount = 5;
  
  // Information on the impacts of shooting a ray.
  private readonly RaycastHit[] raycastHits = new RaycastHit[HitsCount];

  // Information on impacts from the corners of a card.
  private readonly RaycastHit[] cardHits = new RaycastHit[4];
  
  // Ray created from the camera to the projection of the mouse
  // coordinates on the scene.  
  private Ray mouseRay;

  // To calculate the mouse offset (in world-space).
  private Vector3 oldMouseWorldPosition;
  
  private Vector3 MousePositionToWorldPoint()
  {
    Vector3 mousePosition = Input.mousePosition;
    if (Camera.main.orthographic == false)
      mousePosition.z = 10.0f;

    return Camera.main.ScreenToWorldPoint(mousePosition);
  }

  private void ResetCursor() => Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

  /// <summary>
  /// Returns the Transfrom of the object closest to the origin
  /// of the ray.
  /// </summary>
  /// <returns>Transform or null if there is no impact.</returns>
  private Transform MouseRaycast()
  {
    Transform hit = null;
    
    // Fire the ray!
    if (Physics.RaycastNonAlloc(mouseRay,
                                raycastHits,
                                Camera.main.farClipPlane,
                                raycastMask) > 0)
    {
      // We order the impacts according to distance.
      System.Array.Sort(raycastHits, (x, y) => x.distance.CompareTo(y.distance));
      
      // We are only interested in the first one.
      hit = raycastHits[0].transform;
    }
    
    return hit;
  }
  
  /// <summary>Detects an IDrop object under the mouse pointer.</summary>
  /// <returns>IDrop or null.</returns>
  private GameObject DetectDroppable()
  {
    GameObject droppable = null;
    
    // The four corners of the card.
    Vector3 cardPosition = currentDragTransform.position;
    Vector2 halfCardSize = cardSize * 0.5f;
    Vector3[] cardConner =
    {
      new(cardPosition.x + halfCardSize.x, cardPosition.y, cardPosition.z - halfCardSize.y),
      new(cardPosition.x + halfCardSize.x, cardPosition.y, cardPosition.z + halfCardSize.y),
      new(cardPosition.x - halfCardSize.x, cardPosition.y, cardPosition.z - halfCardSize.y),
      new(cardPosition.x - halfCardSize.x, cardPosition.y, cardPosition.z + halfCardSize.y)
    };

    int cardHitIndex = 0;
    Array.Clear(cardHits, 0, cardHits.Length);

    // We launch the four rays.
    for (int i = 0; i < cardConner.Length; ++i)
    {
      Ray ray = new(cardConner[i], Vector3.down);

      int hits = Physics.RaycastNonAlloc(ray, raycastHits, Camera.main.farClipPlane, raycastMask);
      if (hits > 0)
      {
        // We order the impacts by distance from the origin of the ray.
        Array.Sort(raycastHits, (x, y) => x.transform != null ? x.distance.CompareTo(y.distance) : -1);

        // We are only interested in the closest one that isn't the original gameObject.
        if (raycastHits[0].collider.gameObject == gameObject){
          if(raycastHits[1].collider.gameObject != null){
            raycastHits[0] = raycastHits[1];
            cardHits[cardHitIndex++] = raycastHits[0];
          }
        }
        else {
          cardHits[cardHitIndex++] = raycastHits[0];
        }
      }
    }

    if (cardHitIndex > 0)
    {
      // We are looking for the nearest possible IDrop.
      Array.Sort(cardHits, (x, y) => x.transform != null ? x.distance.CompareTo(y.distance) : -1);

      if (cardHits[0].transform != null)
        droppable = cardHits[0].transform.gameObject;
    }

    return droppable;
  }
  
  /// <summary>Detects an IDrag object under the mouse pointer.</summary>
  /// <returns>IDrag or null.</returns>
  public GameObject DetectDraggable()
  {
    GameObject draggable = null;

    mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

    Transform hit = MouseRaycast();
    if (hit != null)
    {
      draggable = hit.transform.gameObject;
      if (draggable.GetComponent<IDrag>() is { IsDraggable: true })
        currentDragTransform = hit;
      else
        draggable = null;
    }

    return draggable;
  }

  private void Update()
  {
    if (currentDrag == null)
    {
      GameObject draggable = DetectDraggable();

      // Left mouse button pressed?
      if (Input.GetMouseButtonDown(0) == true)
      {
        // Is there an IDrag object under the mouse pointer?
        if (draggable != null)
        {
          // We already have an object to start the drag operation!
          currentDrag = draggable.GetComponent<IDrag>();
          //currentDragTransform = hit;
          oldMouseWorldPosition = MousePositionToWorldPoint();

          // Hide the mouse icon.
          Cursor.visible = false;
          // And we lock the movements to the window frame,
          // so we can't move objects out of the camera's view.          
          Cursor.lockState = CursorLockMode.Confined;

          // The drag operation begins.
          currentDrag.Dragging = true;
          currentDrag.OnBeginDrag(new Vector3(raycastHits[0].point.x, height, raycastHits[0].point.z));
          //currentDrag.OnBeginDrag(new Vector3(raycastHits[0].point.x, raycastHits[0].point.y + height, raycastHits[0].point.z));
        }
      }
      else
      {
        // Left mouse button not pressed?
        
        // We pass over a new IDrag?
        if (draggable != null && possibleDrag == null)
        {
          // We execute its OnPointerEnter.
          possibleDrag = draggable.GetComponent<IDrag>();
          possibleDrag.OnPointerEnter(raycastHits[0].point);
        }

        // We are leaving an IDrag?
        if (draggable == null && possibleDrag != null)
        {
          // We execute its OnPointerExit.
          possibleDrag.OnPointerExit(raycastHits[0].point);
          possibleDrag = null;
          
          ResetCursor();
        }
      }
    }
    else
    {
      GameObject droppable = DetectDroppable();

      // Is the left mouse button held down?
      if (Input.GetMouseButton(0) == true)
      {
        // Calculate the offset of the mouse with respect to its previous position.
        Vector3 mouseWorldPosition = MousePositionToWorldPoint();
        Vector3 offset = (mouseWorldPosition - oldMouseWorldPosition) * dragSpeed;
        
        // OnDrag is executed.
        currentDrag.OnDrag(offset, droppable);

        oldMouseWorldPosition = mouseWorldPosition;
      }
      else if (Input.GetMouseButtonUp(0) == true)
      {
        // The left mouse button is released and
        // the drag operation is finished.
        currentDrag.Dragging = false;
        currentDrag.OnEndDrag(raycastHits[0].point, droppable);
        currentDrag = null;
        currentDragTransform = null;

        // We return the mouse icon to its normal state.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
      }
    }
  }

  private void OnEnable()
  {
    possibleDrag = null;
    currentDragTransform = null;
    
    ResetCursor();
  }
}
