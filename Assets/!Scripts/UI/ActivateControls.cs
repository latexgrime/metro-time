using System;
using UnityEngine;
using UnityEngine.Events;
public class ActivateControls : MonoBehaviour
{
    public GameObject controlsCanvas;
    public UnityEvent controlsEvent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            controlsEvent.Invoke();
        }
    }
}
