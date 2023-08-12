using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Accept draggable objects.
/// </summary>
public class DroppableFloor : MonoBehaviour, IDrop
{
  public bool IsDroppable => true;

  // We accept all IDrags.
  public bool AcceptDrop(IDrag drag) => true;

  public void OnDrop(IDrag drag) { }
}

