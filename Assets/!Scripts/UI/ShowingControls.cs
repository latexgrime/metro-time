using System;
using UnityEngine;
using UnityEngine.Events;

public class ShowingControls : MonoBehaviour
{
    public GameObject controlsCanvas;
    
    public void SetCanvasActive()
    {
        controlsCanvas.SetActive(true);
    }

    public void DeactivateCanvas()
    {
        controlsCanvas.SetActive(false);
    }
}
