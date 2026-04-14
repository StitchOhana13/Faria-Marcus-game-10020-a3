using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public Transform Character;       // Drag your Player object here in the Inspector
    public Vector3 offset;        // e.g., (0, 5, -10) for a standard 3D view

    void LateUpdate()
    {
        // Follow position only
        transform.position = Character.position + offset;
    }
}