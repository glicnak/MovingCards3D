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

/// <summary>
/// Draggable object.
/// </summary>
public interface IDrag
{
  /// <summary> Can it be draggable? </summary>
  public bool IsDraggable { get; }

  /// <summary> A Drag operation is currently underway. </summary>
  public bool Dragging { get; set; }
    
  /// <summary> Mouse enters the object. </summary>
  /// <param name="position">Mouse position.</param>
  public void OnPointerEnter(Vector3 position);
    
  /// <summary> Mouse exits object. </summary>
  /// <param name="position">Mouse position.</param>
  public void OnPointerExit(Vector3 position);

  /// <summary> Drag begins. </summary>
  /// <param name="position">Mouse position.</param>
  public void OnBeginDrag(Vector3 position);

  /// <summary>A drag is being made. </summary>
  /// <param name="deltaPosition"> Mouse offset position. </param>
  /// <param name="droppable">Object on which a drop may be made, or null.</param>
  public void OnDrag(Vector3 deltaPosition, GameObject droppable);

  /// <summary> The drag operation is completed. </summary>
  /// <param name="position">Mouse position.</param>
  /// <param name="droppable">Object on which a drop may be made, or null.</param>
  public void OnEndDrag(Vector3 position, GameObject droppable);
}