using System;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    { 
        Debug.Log("Entering door");
    }

    void OnTriggerExit(Collider other)
    {
        
        Debug.Log("Exit door");
    }
}
