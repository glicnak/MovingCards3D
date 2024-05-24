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
/// Card tilter.
/// </summary>
public sealed class CardTilter : MonoBehaviour
{

  [Header("Pitch")]

  [SerializeField, Header("Force")]
  private float pitchForce = 10.0f;  

  [SerializeField, Header("Minimum Angle")]
  private float pitchMinAngle = -25.0f;  

  [SerializeField, Header("Maximum Angle")]
  private float pitchMaxAngle = 25.0f;  

  [Space]

  [Header("Roll")]

  [SerializeField, Header("Force")]
  private float rollForce = 10.0f;  

  [SerializeField, Header("Minimum Angle")]
  private float rollMinAngle = -25.0f;  

  [SerializeField, Header("Maximum Angle")]
  private float rollMaxAngle = 25.0f;  

  [Space]

  [SerializeField]
  public float restTime = 1.0f;  

  // Pitch angle and velocity.
  private float pitchAngle, pitchVelocity;

  // Roll angle and velocity.
  private float rollAngle, rollVelocity;

  // To calculate the velocity vector.
  private Vector3 oldPosition;

  // The original rotation
  public Vector3 originalAngles;

  private void Awake()
  {
    oldPosition = transform.position;
    originalAngles = transform.rotation.eulerAngles;
  }

  private void Update()
  {
    // Calculate offset.
    Vector3 currentPosition = transform.position;
    Vector3 offset = currentPosition - oldPosition;

    // Limit the angle ranges.
    if (offset.sqrMagnitude > Mathf.Epsilon)
    {
      pitchAngle = Mathf.Clamp(pitchAngle + offset.z * pitchForce, pitchMinAngle, pitchMaxAngle);
      rollAngle = Mathf.Clamp(rollAngle + offset.x * rollForce, rollMinAngle, rollMaxAngle);
    }

    // The angles have 0 with time
    pitchAngle = Mathf.SmoothDamp(pitchAngle, 0.0f, ref pitchVelocity, GetComponent<CardDrag>().currentTiltTime * Time.deltaTime * 10.0f);
    rollAngle = Mathf.SmoothDamp(rollAngle, 0.0f, ref rollVelocity, GetComponent<CardDrag>().currentTiltTime* Time.deltaTime * 10.0f);

    // Update the card rotation.
    transform.rotation = Quaternion.Euler(originalAngles.x + pitchAngle,
                                          transform.eulerAngles.y,
                                          originalAngles.z - rollAngle);
    
    oldPosition = currentPosition;
    
  }
}