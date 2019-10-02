using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform lookAt; // Player //object we are looking at
    public Vector3 offset;
    public Vector3 rotation;
    public float smoothSpeed = 10f;
    
    private void LateUpdate()
    {
       
        Vector3 desiredPosition = lookAt.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition,smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), smoothSpeed * Time.deltaTime);
        transform.rotation = smoothedRotation;
        
    }
}
