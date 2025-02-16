using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            Debug.Log("hit " + other.gameObject.name);
            Destroy(gameObject);
        }
        
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("hit " + other.gameObject.name);
            Destroy(gameObject);
        }
    }
}
